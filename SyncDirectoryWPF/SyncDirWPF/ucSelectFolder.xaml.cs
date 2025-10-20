using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SyncDirectory
{
    /// <summary>
    /// Interaction logic for ucSelectFolder.xaml
    /// </summary>
    public partial class ucSelectFolder : UserControl, INotifyPropertyChanged
    {
        public ucSelectFolder()
        {
            InitializeComponent();
            Init();
            Loaded += OnLoaded;
            
        }

        private string _currentDirectory="";
        public string CurrentDirectory { get { return _currentDirectory; } set { 
            _currentDirectory = value; 
            OnPropertyChanged();
        } }

        private string _initialDirectory;
        public string InitialDirectory {
            get { return _initialDirectory; }
            set { _initialDirectory = value;
        //       if (this.IsLoaded) FindInitialDirectory();
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
        //    FindInitialDirectory();
        }

        public void Init()
        {
            InitialDirectory = "";
            DriveInfo[] di = DriveInfo.GetDrives();
            foreach (var d in di)
            {
                TreeViewItem tvi = new TreeViewItem();
                tvi.Header = d.Name.Replace("\\", "");
                tvi.Tag = d.Name;
                tvi.Expanded += OnItemExpanded;
                tvi.Selected += OnItemSelected;
                tvi.Items.Add(new TreeViewItem() { Header = "*" });
                tvFolders.Items.Add(tvi);
            }
        }

        public void OnItemSelected(object sender, RoutedEventArgs e)
        {
           
            TreeViewItem node = e.Source as TreeViewItem;
            if (node != null)
            {
                node.IsExpanded = true;
                CurrentDirectory = node.Tag.ToString();
            }
        }

        public void OnItemExpanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem node = e.Source as TreeViewItem;
            node.IsSelected = true;
            node.Items.Clear();
            try
            {
                DirectoryInfo di = new DirectoryInfo(node.Tag.ToString());
                DirectoryInfo[] dirs = di.GetDirectories();
                foreach (var d in dirs)
                {
                    TreeViewItem tvi = new TreeViewItem();
                    tvi.Header = d.Name;
                    tvi.Tag = d.FullName + "\\";
                    tvi.Items.Add(new TreeViewItem());
                    node.Items.Add(tvi);
                }
                CheckItems(node);
            }
            catch { }
        }

        async private void CheckItems(TreeViewItem Node)
        {
            await Task.Run(() =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    foreach (TreeViewItem child in Node.Items)
                    {
                        try
                        {
                            if (Directory.GetDirectories(child.Tag.ToString()).Count() == 0)
                                child.Items.Clear();
                        }
                        catch { }
                    }
                }
                    );
            });

        }

        private void FindInitialDirectory()
        {
            if ((InitialDirectory==null)||(InitialDirectory=="")) return;
            string[] path = InitialDirectory.Split(new[] {'\\', '/',}, StringSplitOptions.RemoveEmptyEntries);
            
            TreeViewItem resultItem = GoToFolder(tvFolders.Items, path[0]);
            if (resultItem != null)
            {
                resultItem.IsExpanded = true;
                TreeViewItem tmpItem = null;
                for (int i = 1; i < path.Count(); i++)
                {
                    tmpItem = GoToFolder(resultItem.Items, path[i]);
                    if (tmpItem != null)
                    {
                        resultItem = tmpItem;
                        resultItem.IsExpanded = true;
                    }
                    else break;
                }
               
                resultItem.IsSelected = true;
            }
        }

        private TreeViewItem GoToFolder(ItemCollection nodeCollection, string folder)
        {
            var b = from TreeViewItem n in nodeCollection where n.Header.ToString().ToLower() == folder.ToLower() select n;
            if (b.Count() > 0) return b.First();
            else return null;
            
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
