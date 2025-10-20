using Microsoft.Win32;
using Serialization.DataContract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace SyncDirectory
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public SyncList SynchList = new SyncList();
        public Log log;

        private bool NeedSave = false;

        private const double ExpanderHeight = 300;
        private const double ExpanderMinHeight = 35;
        public MainWindow()
        {
            InitializeComponent();
            LoadSyncList(Constants.SyncListFilePath);
            log = new Log(1000);
            DgLog.ItemsSource = log;
            NeedSave = false;
        }



        private void Interruption(object sender, RoutedEventArgs e)
        {
            SyncListRecord slr = null;
            if (((sender is Button) && (Convert.ToInt32((sender as Button).Tag) == 1)) || (sender is DataGrid))
            {
                slr = DgDirList.SelectedItem as SyncListRecord;
                InterruptSelected(slr);
            }
            else if ((sender is Button) && (Convert.ToInt32((sender as Button).Tag) == 2))
            {
                foreach (var SelItem in DgDirList.SelectedItems)
                {
                    slr = SelItem as SyncListRecord;
                    InterruptSelected(slr);
                }
            }
            else if ((sender is Button) && (Convert.ToInt32((sender as Button).Tag) == 3))
            {
                foreach (var SelItem in SynchList.Data)
                {
                    slr = SelItem as SyncListRecord;
                    InterruptSelected(slr);
                }
            }
        }

        private void Synchronization(object sender, RoutedEventArgs e)
        {
                SyncListRecord slr = null;
                if (((sender is Button) && (Convert.ToInt32((sender as Button).Tag) == 1)) || (sender is DataGrid))
                {
                    slr = DgDirList.SelectedItem as SyncListRecord;
                    SyncSelected(slr);
                }
                else if ((sender is Button) && (Convert.ToInt32((sender as Button).Tag) == 2))
                {
                    foreach (var SelItem in DgDirList.SelectedItems)
                    {
                        slr = SelItem as SyncListRecord;
                        SyncSelected(slr);
                    }
                }
                else if ((sender is Button) && (Convert.ToInt32((sender as Button).Tag) == 3))
                {
                    foreach (var SelItem in SynchList.Data)
                    {
                        slr = SelItem as SyncListRecord;
                        SyncSelected(slr);
                    }
                }
            
        }

        
        async private void SyncSelected(SyncListRecord slr)
        {
            if (slr != null)
            {
                slr.Cancel = new CancellationTokenSource();
                slr.SyncTask = Task.Run(() =>
                   {
                        FileOperation FO = new FileOperation(slr, LogAdder: AddMessageToLog);
                        FO.Sync(false);
                   });
                await slr.SyncTask;
            }
        }

        private void InterruptSelected(SyncListRecord slr)
        {
            if (slr != null)
                if (slr.SyncTask != null)
                    if (slr.SyncTask.Status == TaskStatus.Running)
                        slr.Cancel.Cancel();
        }



        private void AddMessageToLog(string source, string message)
        {
            DgLog.Dispatcher.Invoke(() =>
            {
                DgLog.ScrollIntoView(log.AddRecord(source, message));
            });
        }


        #region Menu
        
        private void miLoadSyncList_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            ofd.InitialDirectory = Directory.GetCurrentDirectory();
            if (ofd.ShowDialog().Value)
            {
                LoadSyncList(ofd.SafeFileName);                
            }
        }

        private void miSaveSyncList_Click(object sender, RoutedEventArgs e)
        {
            SaveSyncList(Constants.SyncListFilePath);
        }

        private void miSaveSyncListAs_Click(object sender, RoutedEventArgs e)
        {
            SaveSyncListAs();
        }

        private void LoadSyncList(string path)
        {
            try
            {
                SyncList tempSynchList = new SyncList();
                if (File.Exists(path))
                    DataContractMethods.Restore(ref tempSynchList, path);
                foreach (var r in tempSynchList.Data)
                {
                    if (r.LastSynchronization.Year < 1995) r.Status = "Синхронизация не проводилась";
                    else r.Status = "Синхронизировано " + r.LastSynchronization.ToString("yyyy.MM.dd HH:mm") + " за " + r.LastSynchronizationDuration.ToString("0.00") + " секунд"; ;
                }
                SynchList = tempSynchList;
                DgDirList.ItemsSource = SynchList.Data;
                DgDirList.Columns[0].Width = new DataGridLength(SynchList.ColumnWidthSource);
                DgDirList.Columns[1].Width = new DataGridLength(SynchList.ColumnWidthDestination);
                NeedSave = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось сохранить список синхронизируемых папок. Ошибка: " + ex.Message);
            }
        }

        private void SaveSyncList(string path)
        {
            try
            {
                SynchList.ColumnWidthSource = DgDirList.Columns[0].Width.DisplayValue;
                SynchList.ColumnWidthDestination = DgDirList.Columns[1].Width.DisplayValue;


                if (File.Exists(path)) File.Delete(path);
                DataContractMethods.Save(SynchList, path);
                NeedSave = false;
            }
            catch (Exception ex)
            {
                NeedSave = true;
                MessageBox.Show("Не удалось сохранить список синхронизируемых папок. Ошибка: " + ex.Message);
            }
        }

        private void SaveSyncListAs()
        {
            SaveFileDialog ofd = new SaveFileDialog();
            ofd.InitialDirectory = Directory.GetCurrentDirectory();
            if (ofd.ShowDialog().Value)
            {
                SaveSyncList(ofd.SafeFileName);
            }
        }

        private void miAddDir_Click(object sender, RoutedEventArgs e)
        {
            wAddFolder waf = new wAddFolder();
            //waf.TvDestDir.InitialDirectory = @"d:\Distrib\_Ide\Delphi.2007\";
            waf.ShowDialog();
            if (waf.NewSLR != null)
            {
                SynchList.Data.Add(waf.NewSLR);
                DgDirList.Items.Refresh();
                NeedSave = true;
            }

        }
        private void miDelDir_Click(object sender, RoutedEventArgs e)
        {
            NeedSave = true;
            while (DgDirList.SelectedItems.Count > 0)
            {
                SyncListRecord slr = DgDirList.SelectedItems[0] as SyncListRecord;
                if ((slr.SyncTask != null) && (slr.SyncTask.Status == TaskStatus.Running))
                    DgDirList.SelectedItems.Remove(slr);
                else
                {
                    SynchList.Data.Remove(slr);
                    DgDirList.Items.Refresh();
                }
            }
        }
        #endregion

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var b = from SyncListRecord c in SynchList.Data where (c.SyncTask != null) && (c.SyncTask.Status == TaskStatus.Running) select c;
            if (b.Count() > 0)
            {
                if (MessageBox.Show("Идет синхронизация файлов. При выходе она будет прервана. Продолжить?", "Предупреждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    foreach (SyncListRecord slr in b)
                        slr.Cancel.Cancel();
                }
                else e.Cancel = true;
            }
            SaveSyncList(Constants.SyncListFilePath);
        }

        private void exp_Collapsed(object sender, RoutedEventArgs e)
        {
            RowForLogExpander.Height = new GridLength(ExpanderMinHeight);
         
        }

        private void exp_Expanded(object sender, RoutedEventArgs e)
        {
            RowForLogExpander.Height = new GridLength(ExpanderHeight);
        }


        private void MiExit_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
