using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace DoubleClick
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        private bool _isBusy;
        private bool _isCanExecuteEnabled;
        private bool _isDoubleClickBlockerEnabled;
        public ICommand Command { get; }
        public ObservableCollection<string> Clicks { get; }


        public bool IsBusy
        {
            get { return _isBusy; }
            private set
            {
                _isBusy = value;
                OnPropertyChanged();
            }
        }

        public bool IsCanExecuteEnabled
        {
            get { return _isCanExecuteEnabled; }
            set
            {
                _isCanExecuteEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool IsDoubleClickBlockerEnabled
        {
            get { return _isDoubleClickBlockerEnabled; }
            set
            {
                _isDoubleClickBlockerEnabled = value;
                OnPropertyChanged();
            }
        }

        public MainWindow()
        {
            DataContext = this;
            Command = new ActionCommand(Exec, () => !IsDoubleClickBlockerEnabled || !IsBusy);
            Clicks = new ObservableCollection<string>();
            InitializeComponent();
        }

        private async void Exec()
        {
            IsBusy = true;
            Clicks.Insert(0, "click " + DateTime.Now.Millisecond);
            Console.WriteLine();
            await Task.Delay(300);
            IsBusy = false;
            Clicks.Insert(0, "");
            Clicks.Insert(0, "");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void UIElement_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {            
            if (IsDoubleClickBlockerEnabled && e.ClickCount >= 2)
            {
                e.Handled = true;
            }
        }
    }

    class ActionCommand : ICommand
    {
        private readonly Action _action;
        private readonly Func<bool> _isBusy;

        public ActionCommand(Action action, Func<bool> isBusy)
        {
            _action = action;
            _isBusy = isBusy;
        }

        public bool CanExecute(object parameter)
        {
            return _isBusy();
        }

        public void Execute(object parameter)
        {
            _action();
        }

        public event EventHandler CanExecuteChanged;
    }

    class VisibleWhenTrue : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
