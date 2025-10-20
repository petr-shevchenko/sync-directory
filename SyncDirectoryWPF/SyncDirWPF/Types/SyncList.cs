using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Serialization.DataContract;
using System.IO;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SyncDirectory
{
    public class SyncList
    {
        public List<SyncListRecord> Data { get; set; }
        public double ColumnWidthSource { get; set; }
        public double ColumnWidthDestination { get; set; }

        public SyncList()
        {
            Data = new List<SyncListRecord>(10);
            ColumnWidthSource = 250;
            ColumnWidthDestination = 250;
        }

    }

    public class SyncListRecord:INotifyPropertyChanged
    {
        public string SourceDirectory { get; set; }
        public string DestinationDirectory { get; set; }
        public DateTime LastSynchronization { get; set; }

        public double LastSynchronizationDuration { get; set; }

        private string status;
        public string Status { get { return status; } set { status = value; OnPropertyChanged(); } }

        [NonSerialized]
        public CancellationTokenSource Cancel = new CancellationTokenSource();
        [NonSerialized]
        public Task SyncTask;

        public SyncListRecord()
        {
            Status = "";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
     
    }
}
