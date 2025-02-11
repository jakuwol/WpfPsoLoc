using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace WpfPsoLoc
{
    /// <summary>
    /// Interaction logic for ProgressWindow.xaml
    /// </summary>
    public partial class ProgressWindow : Window, INotifyPropertyChanged
    {
        private double _ProgressValue;

        public double ProgressValue
        {
            get { return _ProgressValue; }
            set
            {
                _ProgressValue = value;
                OnPropertyChanged("ProgressValue");
            }
        }

        private string _ProgressText;
        public string ProgressText
        {
            get { return _ProgressText; }
            set
            {
                _ProgressText = value;
                OnPropertyChanged("ProgressText");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public ProgressWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            ProgressValue = 0;
            ProgressText = string.Empty;
        }
    }
}
