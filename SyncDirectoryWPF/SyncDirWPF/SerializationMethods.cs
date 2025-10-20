using System;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace Serialization
{
    namespace Xml
    {
        /// <summary>
        /// Класс для выполнения Xml сериализации
        /// </summary>
        public static class XmlMethods
        {
            #region "XmlFile" Serialization/Deserialization
                /// <summary>
                /// Сохраняет тип данных в xml формат
                /// </summary>
                /// <param name="ClassToSave">Экземпляр сохраняемого объекта</param>
                /// <param name="DataFilePath">Путь до места сохранения</param>
                public static void Save<T>(T ClassToSave, String DataFilePath)
                {
                    try
                    {
                        using (StreamWriter FileStrm = new StreamWriter(DataFilePath))
                        {
                            XmlSerializer sr = new XmlSerializer(ClassToSave.GetType());
                            TextWriter xmlwrt = new StreamWriter(FileStrm.BaseStream);
                            sr.Serialize(xmlwrt, ClassToSave);
                        }
                    }
                    catch (Exception e)
                    {
                        throw ExceptionInfo.GetException(
                            ClassToSave,
                            e,
                            "Ошибка при сохранении объекта в файл " + "\"" + DataFilePath + "\"");
                    }
                }

                /// <summary>
                /// Восстанавливает тип данных из xml файла
                /// </summary>
                /// <param name="ClassToRestore">Тип объекта для восстановления</param>
                /// <param name="DataFilePath">Путь к xml файлу,хранящему представление объекта</param>
                public static void Restore<T>(ref T ClassToRestore, String DataFilePath)
                {
                    try 
                    {
                        using (Stream FileStrm = File.Open(DataFilePath, FileMode.Open))
                        {
                            XmlSerializer sr = new XmlSerializer(ClassToRestore.GetType());
                            ClassToRestore = (T) sr.Deserialize(FileStrm);
                        }
                    }
                    catch (Exception e)
                    {
                        throw ExceptionInfo.GetException(
                            ClassToRestore,
                            e,
                            "Ошибка при восстановлении объекта из файла " + "\"" + DataFilePath + "\"");
                    }
                }
            #endregion

            #region "Stream" Serialization\Deserialization
                /// <summary>
                /// Сохраняет объект в xml-совместимом формате в поток. После выполнения поток остается открытым.
                /// </summary>
                /// <param name="ClassToSave">экземпляр объекта для сохранения</param>
                /// <param name="StrmStorage">экземпляр объекта потока (MemoryStream,FileStream и т.д.)</param>
                /// <typeparam name="T">объект любого типа</typeparam>
                public static void Save<T>(T ClassToSave, ref Stream StrmStorage)
                {
                    StreamSerialization.Save(ClassToSave,ref StrmStorage);
                }

                /// <summary>
                /// Восстанавливает тип данных из xml формата
                /// </summary>
                /// <param name="ClassToRestore">экземпляр объекта для восстановления</param>
                /// <param name="StrmStorage">открытый поток с сохраненным объектом</param>
                /// <typeparam name="T">объект любого типа</typeparam>
                public static void Restore<T>(ref T ClassToRestore,ref Stream StrmStorage)
                {
                    StreamSerialization.Restore(ref ClassToRestore,ref StrmStorage);
                }

            #endregion
        }
    }

    namespace DataContract
    {
        /// <summary>
        /// Класс для выполнения сериализации контрактов данных
        /// </summary>
        public static class DataContractMethods
        {
            #region "XmlLikeFile" Serialization/Deserialization
                /// <summary>
                /// Сохраняет тип данных в xml-совместимый формат
                /// </summary>
                /// <param name="ClassToSave">Экземпляр сохраняемого объекта</param>
                /// <param name="DataFilePath">Путь до места сохранения</param>
                public static void Save<T>(T ClassToSave, String DataFilePath)
                {
                    try
                    {
                        XmlWriterSettings XmlWrtSetts=new XmlWriterSettings();
                        XmlWrtSetts.Indent = true;
                        using (XmlWriter XmlWrt = XmlWriter.Create(DataFilePath,XmlWrtSetts))
                        {
                            DataContractSerializer sr = new DataContractSerializer(ClassToSave.GetType());
                            sr.WriteObject(XmlWrt, ClassToSave);
                        }
                    }
                    catch (Exception e)
                    {
                        throw ExceptionInfo.GetException(
                            ClassToSave,
                            e,
                            "Ошибка при сохранении объекта в файл " + "\"" + DataFilePath + "\"");
                    }
                }

                /// <summary>
                /// Восстанавливает тип данных из xml-совместимого формата
                /// </summary>
                /// <param name="ClassToRestore">Тип объекта для восстановления</param>
                /// <param name="DataFilePath">Путь к файлу,хранящему представление объекта</param>
                public static void Restore<T>(ref T ClassToRestore, String DataFilePath)
                {
                    try
                    {
                        using (Stream FileStrm = File.Open(DataFilePath, FileMode.Open))
                        {
                            DataContractSerializer sr = new DataContractSerializer(ClassToRestore.GetType());
                            ClassToRestore = (T)sr.ReadObject(FileStrm);
                        }
                    }
                    catch (Exception e)
                    {
                        throw ExceptionInfo.GetException(
                            ClassToRestore,
                            e,
                            "Ошибка при восстановлении объекта из файла " + "\"" + DataFilePath + "\"");
                    }
                }
            #endregion

            #region "Stream" Serialization/Deserialization
                /// <summary>
                /// Сохраняет объект в xml-совместимом формате в поток. После выполнения поток остается открытым.
                /// </summary>
                /// <param name="ClassToSave">экземпляр объекта для сохранения</param>
                /// <param name="StrmStorage">экземпляр объекта потока (MemoryStream,FileStream и т.д.)</param>
                /// <typeparam name="T">объект любого типа</typeparam>
                public static void Save<T>(T ClassToSave,ref Stream StrmStorage)
                {
                    StreamSerialization.Save(ClassToSave,ref StrmStorage);
                }

                /// <summary>
                /// Восстанавливает тип данных из xml-совместимого формата
                /// </summary>
                /// <param name="ClassToRestore">экземпляр объекта для восстановления</param>
                /// <param name="StrmStorage">открытый поток с сохраненным объектом</param>
                /// <typeparam name="T">объект любого типа</typeparam>
                public static void Restore<T>(ref T ClassToRestore,ref Stream StrmStorage)
                {
                    StreamSerialization.Restore(ref ClassToRestore,ref StrmStorage);
                }
            #endregion
        } 
    }

    internal static class StreamSerialization//:IStreamSerialization
    {
        public static void Save<T>(T ClassToSave,ref Stream StrmStorage)
        {
            try
            {
                //using (StrmStorage)
                if (StrmStorage!=null)
                {
                    DataContractSerializer sr = new DataContractSerializer(ClassToSave.GetType());
                    sr.WriteObject(StrmStorage, ClassToSave);
                }
            }
            catch (Exception e)
            {
                throw ExceptionInfo.GetException(
                    ClassToSave,
                    e,
                    "Ошибка при сохранении объекта в поток " + "\"" + StrmStorage.GetType() + "\"");
            }

        }

 
        public static void Restore<T>(ref T ClassToRestore, ref Stream StrmStorage)
        {
            try
            {
               // using (StrmStorage)
                if (StrmStorage!=null)
                {
                    StrmStorage.Seek(0, SeekOrigin.Begin);
                    DataContractSerializer sr = new DataContractSerializer(ClassToRestore.GetType());
                    ClassToRestore = (T)sr.ReadObject(StrmStorage);
                }
            }
            catch (Exception e)
            {
                throw ExceptionInfo.GetException(
                    ClassToRestore,
                    e,
                    "Ошибка при восстановлении объекта из потока " + "\"" + StrmStorage.GetType() + "\"");
            }
        }
    }
   
    /// <summary>
    /// Класс для извлечения информации из объекта типа Exception
    /// </summary>
    internal static class ExceptionInfo
    {
        /// <summary>
        /// Обрабатывает ошибки при сериализации
        /// </summary>
        /// <param name="ObjectWithException">Тип объекта, при работе с которым возникла ошибка</param>
        /// <param name="e">исключение, отловившее ошибку</param>
        /// <param name="CaptureInfo">заголовок сообщения об ошибке</param>
        public static Exception GetException<T>(T ObjectWithException, Exception e,String CaptureInfo)
        {
            StringBuilder StrBldErrLog = new StringBuilder();
            StrBldErrLog.AppendLine(CaptureInfo);
            StrBldErrLog.AppendLine("Тип объекта: " + ObjectWithException.GetType());
            StrBldErrLog.AppendLine("Текст ошибки: " + e.Message);
                                
            GetDeepExceptionInfo(e, ref StrBldErrLog);
            return new Exception(StrBldErrLog.ToString());
        }

        /// <summary>
        /// Получение полной информации об ошибках
        /// </summary>
        /// <param name="e">исключение, отловившее ошибку</param>
        /// <param name="StrBldErrLog">строка для вывода подробных сообщений</param>
        public static void GetDeepExceptionInfo(Exception e, ref StringBuilder StrBldErrLog)
        {
            if (e.InnerException != null)
            {
                ConcatExceptionInfo(e,ref StrBldErrLog);
                GetDeepExceptionInfo(e.InnerException, ref StrBldErrLog);
            }
        }

        /// <summary>
        /// Запись информации из исключения в StringBuilder
        /// </summary>
        /// <param name="e">исключение</param>
        /// <param name="StrBldErrLog">объект StringBuilder, в который будет записана информация из исключения </param>
        public static void ConcatExceptionInfo(Exception e,ref StringBuilder StrBldErrLog)
        {
            StrBldErrLog.AppendLine(e.Message);
            StrBldErrLog.AppendLine(e.Data.ToString());
            StrBldErrLog.AppendLine(e.StackTrace);
        }
    
    }
}
