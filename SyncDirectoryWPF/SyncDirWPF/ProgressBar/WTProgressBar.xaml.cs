using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WordTrain
{
    /// <summary>
    /// Interaction logic for ProgressBar.xaml
    /// </summary>
    public partial class WTProgressBar : UserControl
    {
        private string OperationName;
        private Stopwatch st;
        private bool manualPercent;
        private long CurrentIterationNumber;
        public WTProgressBar()
        {
            InitializeComponent();
            OperationName = "";
            stackPanel.Visibility = Visibility.Collapsed;
        }

        public void startBar(string text, int numOfIteration = 0, bool manualPercent = false)
        {
            stackPanel.Visibility = Visibility.Visible;
            this.manualPercent = manualPercent;
            textLabel.Content = text;
            OperationName = text;
            st = Stopwatch.StartNew();
            if (!manualPercent && numOfIteration == 0) barGrid.Visibility = Visibility.Collapsed;
            else barGrid.Visibility = Visibility.Visible;
            if (manualPercent) numOfIteration = 100;
            progressBar.Maximum = numOfIteration;
            progressBar.Minimum = 0;
            textProshlo.Content = "";
            textRemaining.Content = "";
            textPercent.Text = "";
            CurrentIterationNumber = 0;
            
        }

        public void plusBar(double OuterPercent = -1, string text = "")
        {
            
            textLabel.Content = OperationName;
            if (text != "") textLabel.Content += " (" + text + ")";
            textProshlo.Content = String.Format("Прошло: {0:d2}:{1:d2}:{2:d2}", st.Elapsed.Hours, st.Elapsed.Minutes, st.Elapsed.Seconds);
            double percent = 1.0;
            if (manualPercent && OuterPercent >= 0) percent = OuterPercent;
            else if (!manualPercent && progressBar.Maximum != 0) { percent = CurrentIterationNumber / progressBar.Maximum; CurrentIterationNumber++; }

            progressBar.Value = progressBar.Maximum * percent;
            textPercent.Text = (percent * 100.0).ToString("0.00") + "%";
            int remainingseconds = (int)(st.Elapsed.TotalSeconds * (1 / percent - 1));
            int remaininghours = remainingseconds / 3600;
            int remainingminutes = remainingseconds / 60;
            remainingseconds -= (remaininghours * 3600 + remainingminutes * 60);

            if (remaininghours >= 0 && remainingminutes >= 0 && remainingseconds >= 0)
                textRemaining.Content = String.Format("Осталось: {0:d2}:{1:d2}:{2:d2}", remaininghours, remainingminutes, remainingseconds);
            else textRemaining.Content = "";
        }

        public void closeBar()
        {
            stackPanel.Visibility = Visibility.Collapsed;
        }
    }
}
