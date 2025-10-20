using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SyncDirectory
{
    /// <summary>
    /// Interaction logic for wAddFolder.xaml
    /// </summary>
    public partial class wAddFolder : Window
    {
        public SyncListRecord NewSLR;
        public wAddFolder()
        {
            InitializeComponent();
            Binding b = new Binding() { Source = this.TvSourceDir, Path = new PropertyPath("CurrentDirectory") };
            TbSourceDir.SetBinding(TextBox.TextProperty, b);
            b = new Binding() { Source = this.TvDestDir, Path = new PropertyPath("CurrentDirectory") };
            TbDestDir.SetBinding(TextBox.TextProperty, b);
        }

      

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            NewSLR = null;
            this.Close();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            NewSLR = new SyncListRecord() { SourceDirectory = TbSourceDir.Text, DestinationDirectory = TbDestDir.Text };
            this.Close();
        }


    }
}
