using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Serialization.DataContract;

namespace SyncDirectory
{
    
    public class FileOperation
    {
        private readonly object _block = new object();

        private FileList FilesInSourceDirectory = new FileList(1);
        private DirectoryList FoldersInSourceDirectory = new DirectoryList(1);
        
        private FileList FilesInDestinationDirectory = new FileList(1);
        private DirectoryList FoldersInDestinationDirectory = new DirectoryList(1);

        private HashTab destinationFilesHashTab = new HashTab(1);
        private string PathFrom = "";
        private string PathTo = "";
        private string HashTabDestFilesPath = "";
        private string LocalHashTabDestFilesPath = "";

        private readonly bool _useMd5 = true;
        private readonly Action<string, string> AddToLog;
        private readonly SyncListRecord SLR;
        private readonly ParallelOptions _po = new ParallelOptions();


        private void AddLog(string message)
        {
            Task.Run(() => { if (AddToLog != null) AddToLog(PathFrom, message); });
        }

        public FileOperation( SyncListRecord SLR, bool UseMD5=false, Action<string, string> LogAdder=null)
        {
            AddToLog = LogAdder;
            this.SLR = SLR;
            PathFrom = SLR.SourceDirectory;
            PathTo = SLR.DestinationDirectory;
            _useMd5 = UseMD5;
            _po.CancellationToken = SLR.Cancel.Token;
        }
        

     
        public void Sync(bool useHashTab)
        {
            try
            {
                DateTime TStart = DateTime.Now;
                AddLog("=====================Начинаем синхронизацию===========================");
                
                if (useHashTab)
                {
                    LocalHashTabDestFilesPath = PathTo + "\\" + Constants.HashTabFilePath;
                    HashTabDestFilesPath = "HashTab_" + HashTabMethods.GetMd5Hash(PathTo) + ".xml";
                    if (File.Exists(HashTabDestFilesPath))
                        DataContractMethods.Restore(ref destinationFilesHashTab, HashTabDestFilesPath);
                    else if (File.Exists(LocalHashTabDestFilesPath))
                        DataContractMethods.Restore(ref destinationFilesHashTab, LocalHashTabDestFilesPath);
                }
                

                AddLog("Начат анализ исходной директории...");
                SLR.Status = "Анализ исходной директории...";
                AnalyseDirectoriesAndFiles(PathFrom, false, false, ref FoldersInSourceDirectory, ref FilesInSourceDirectory);
                AddLog("Начат анализ директории назначения...");
                SLR.Status = "Анализ директории назначения...";
                AnalyseDirectoriesAndFiles(PathTo, true, useHashTab, ref FoldersInDestinationDirectory, ref FilesInDestinationDirectory);
                AddLog("Удаляем файлы в директории назначения.");
                DeleteFiles();
                AddLog("Удаляем папки в директории назначения.");
                DeleteDirs();
                AddLog("Создаем директории.");
                CreateDirs();
                AddLog("Добавляем файлы.");
                AddFiles();
                AddLog("Обновляем файлы.");
                UpdateFiles();
                
                
                if (useHashTab)
                {
                    DataContractMethods.Save(destinationFilesHashTab, HashTabDestFilesPath);
                }

                SLR.LastSynchronization = DateTime.Now;
                TimeSpan ts = DateTime.Now - TStart;
                SLR.LastSynchronizationDuration = ts.TotalSeconds;
                SLR.Status = "Синхронизация закончена " + SLR.LastSynchronization.ToString("yyyy.MM.dd HH:mm") + " за " + ts.TotalSeconds.ToString("0.00") + " секунд";
                AddLog("=====================Синхронизация закончена===========================");
            }
            catch (OperationCanceledException)
            {
                AddLog("Синхронизация прервана пользователем.");
                SLR.Status = "Синхронизация прервана пользователем.";
            }
            catch (Exception e)
            {
                AddLog("Ошибка синхронизации: " + e.Message);
                SLR.Status = "Ошибка синхронизации: "+e.Message;
            }
        }

        private void AnalyseDirectoriesAndFiles(string rootDir, bool isDestDir, bool isUseHashTab, ref DirectoryList directories, ref FileList files)
        {
            try
            {
                int rootDirLength = rootDir.Length;
                DirectoryList directoryList = new DirectoryList(1000);
                FileList fileList = new FileList(1000);
                DirectoryInfo directoryInfo = new DirectoryInfo(rootDir);

                Parallel.ForEach(directoryInfo.EnumerateDirectories("*", SearchOption.AllDirectories).AsParallel(), _po, (s) =>
                {
                    try
                    {
                        lock (_block)
                        {
                            directoryList.Add(new DirRec() { Path = s.FullName.Substring(rootDirLength), FullPath = s.FullName, Info = s});
                        }
                    }
                    catch (Exception e)
                    {
                        AddLog("Ошибка при анализе директорий: " + e.Message);
                    }
                });
                directoryList.SortByFullPath();

                Parallel.ForEach(directoryInfo.EnumerateFiles("*", SearchOption.AllDirectories).AsParallel(), _po, (s) =>
                {
                    try
                    {
                        FileRecord oneFileRecord = new FileRecord();
                        oneFileRecord.FileFullPath = s.FullName;
                        oneFileRecord.FilePath = s.FullName.Substring(rootDirLength);
                        oneFileRecord.SizeGb = s.Length*1.0/1024.0/1024.0/1024.0;
                        oneFileRecord.Info = s;

                        if (_useMd5)
                        {
                            if (isDestDir && isUseHashTab)
                            {
                                int j =
                                    destinationFilesHashTab.FindIndex(
                                        ss => ss.fileFullPath == oneFileRecord.FileFullPath);
                                if (j >= 0) oneFileRecord.Md5Hash = destinationFilesHashTab[j].MD5hash;
                                else
                                {
                                    HashTabRec htr = HashTabMethods.SolveFileMD5(oneFileRecord);
                                    lock (_block)
                                    {
                                        destinationFilesHashTab.Add(htr);
                                    }
                                }
                            }
                            else HashTabMethods.SolveFileMD5(oneFileRecord);
                        }

                        lock (_block)
                        {
                            fileList.Add(oneFileRecord);
                        }
                    }
                    catch (Exception e)
                    {
                        AddLog("Ошибка при анализе файлов: " + e.Message);
                    }
                });

                directories = directoryList;
                files = fileList;
             
            }
            catch (Exception e)
            {
                throw new Exception("Ошибка при анализе директорий и файлов: " + e.Message);
            }

        }


        private void DeleteFiles()
        {
            List<FileRecord> filesForDelete = (FilesInDestinationDirectory.Except(FilesInSourceDirectory, new FileExistComparer())).ToList();
            int cnt = 0;
            int count = filesForDelete.Count();
            Parallel.ForEach(filesForDelete, _po, (s) =>
            {
                cnt++;
                try
                {
                    SLR.Status = "Синхронизация (Удаляем файлы... " + (cnt*100.0/count).ToString("0.00") + "%, " +
                                 cnt.ToString() + " из " + count.ToString() + ")";
                    AddLog("Удалён файл " + s.FileFullPath);
                    File.SetAttributes(s.FileFullPath, FileAttributes.Normal);
                    File.Delete(s.FileFullPath);
                    destinationFilesHashTab.RemoveAll(ss => ss.fileFullPath == s.FileFullPath);
                }
                catch (Exception e)
                {
                    AddLog("Ошибка при удалении файла " + s.FileFullPath + " (" + e.Message + ").");
                }
            }
                );

        }

        private void DeleteDirs()
        {
            List<DirRec> directoiesForDelete = (FoldersInDestinationDirectory.Except(FoldersInSourceDirectory, new DirExistComparer())).ToList();
            int cnt = 0;
            int count = directoiesForDelete.Count();

            for (int j = count - 1; j >= 0; j--)
            {
                cnt++;
                try
                {
                    SLR.Status = "Синхронизация (Удаляем директории... " + (cnt*100.0/count).ToString("0") + "%)";
                    Directory.Delete(directoiesForDelete[j].FullPath);
                    AddLog("Удалена директория " + directoiesForDelete[j].FullPath);
                }
                catch (Exception e)
                {
                    AddLog("Ошибка при удалении директории " + directoiesForDelete[j].FullPath + " (" + e.Message + ").");
                }
            }

        }

        private void CreateDirs()
        {
            List<DirRec> directoiesForCreate = (FoldersInSourceDirectory.Except(FoldersInDestinationDirectory, new DirExistComparer())).ToList();
            int cnt = 0;
            int count = directoiesForCreate.Count();
            foreach (DirRec s in directoiesForCreate)
            {
                cnt++;
                string directoryForCreateFullPath = PathTo + s.Path;
                try
                {
                    SLR.Status = "Синхронизация (Создаём директории... " + (cnt*100.0/count).ToString("0") + "%)";
                    Directory.CreateDirectory(directoryForCreateFullPath);
                    AddLog("Создана директория " + directoryForCreateFullPath);
                }
                catch (Exception e)
                {
                    AddLog("Ошибка при создании директории " + directoryForCreateFullPath + " (" + e.Message + ").");
                }
            }
        }

        private void AddFiles()
        {
            List<FileRecord> filesForAdd = (FilesInSourceDirectory.Except(FilesInDestinationDirectory, new FileExistComparer())).ToList();
            int cnt = 0;
            int count = filesForAdd.Count();

            Parallel.ForEach(filesForAdd, _po, (oneFileInSourceDir) =>
            {
                cnt++;
                string fileForAddFullPath = PathTo + oneFileInSourceDir.FilePath;
                try
                {
                    SLR.Status = "Синхронизация (Добавляем файлы... " + (cnt*100.0/count).ToString("0.00") + "%)";
                    CopyFile(oneFileInSourceDir, fileForAddFullPath);
                    AddLog("Добавлен файл " + fileForAddFullPath);
                }
                catch (Exception e)
                {
                    AddLog("Ошибка при добавлении файла " + fileForAddFullPath + " (" + e.Message + ").");
                }
            });
        }

        private void UpdateFiles()
        {
            List<FileRecord> filesForUpdate = (FilesInSourceDirectory.Except(FilesInDestinationDirectory, new FileNeedUpdateComparer(_useMd5))).ToList();
            int cnt = 0;
            int count = filesForUpdate.Count();

            Parallel.ForEach(filesForUpdate, _po, (oneFileInSourceDir) =>
            {
                cnt++;
                string destFileFullPath = PathTo + oneFileInSourceDir.FilePath;
                try
                {
                    SLR.Status = "Синхронизация (Обновляем файлы... " + (cnt*100.0/count).ToString("0.00") + "%)";
                    File.SetAttributes(destFileFullPath, FileAttributes.Normal);
                    CopyFile(oneFileInSourceDir, destFileFullPath);
                    if (_useMd5) destinationFilesHashTab.UpdateHash(destFileFullPath, oneFileInSourceDir.Md5Hash);
                    AddLog("Обновлен файл " + destFileFullPath);
                }
                catch (Exception e)
                {
                    AddLog("Ошибка при обновлении файла " + destFileFullPath + " (" + e.Message + ").");
                }
            });
        }

        private void CopyFile(FileRecord sourceFile, string destFileName)
        {
            File.Delete(destFileName);
            File.Copy(sourceFile.FileFullPath, destFileName);
            FileInfo destFileInfo = new FileInfo(destFileName);
            destFileInfo.LastWriteTimeUtc = sourceFile.Info.LastWriteTimeUtc;
            destFileInfo.CreationTimeUtc = sourceFile.Info.CreationTimeUtc;
        }

    }
}
