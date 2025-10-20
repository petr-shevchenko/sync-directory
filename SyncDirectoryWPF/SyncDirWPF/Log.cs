using System;
using System.Collections.ObjectModel;

namespace SyncDirectory
{
    public class LogRecord
    {
        public DateTime Time { get; set; }
        public string Source { get; set; }
        public string Message { get; set; }
      
    }
    public class Log : ObservableCollection<LogRecord>
    {
        private object block = new object();
        public Log(int cnt)
            : base()
        { 
            
        }

        public LogRecord AddRecord(string Source, string Message)
        {
            LogRecord LR = new LogRecord() { Time = DateTime.Now, Source = Source, Message = Message };
            lock (block) {
                base.Add(LR); 
            }
            return LR;
        }


        
    }
}
