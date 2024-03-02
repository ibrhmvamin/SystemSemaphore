using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp5
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<string> workingThreadNames { get; set; }
        public ObservableCollection<string> createdThreadNames { get; set; }
        public ObservableCollection<string> waitingThreadNames { get; set; }
        public List<Thread> threads { get; set; }
        public Semaphore semaphore;

        public MainWindow()
        {
            InitializeComponent();
            workingThreadNames = new ObservableCollection<string>();
            createdThreadNames = new ObservableCollection<string>();
            waitingThreadNames = new ObservableCollection<string>();
            semaphore = new Semaphore(3, 3, "semaphore");
            DataContext = this;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Thread thread = new Thread(() =>
            {
            });

            thread.Name = "Thread " + thread.ManagedThreadId;
            bool isFinish = false;
            while (!isFinish)
            {
                if (semaphore.WaitOne(3000))
                {
                    try
                    {
                        Dispatcher.Invoke(() =>
                        {
                            createdThreadNames.Add(thread.Name);
                            Thread.Sleep(1000);

                        });
                    }
                    finally
                    {
                        isFinish = true;
                        semaphore.Release();
                    }
                }
            }
            Dispatcher.Invoke(() =>
            {Thread.Sleep(3000);
                createdThreadNames.Remove(thread.Name);
                waitingThreadNames.Add(thread.Name);
            });
            thread.Start();
        }

        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListView listView)
            {
                if (listView.SelectedItem != null)
                {
                    var selectedThread = listView.SelectedItem as string;
                    Dispatcher.Invoke(() =>
                    {
                        waitingThreadNames.Remove(selectedThread);
                        workingThreadNames.Add(selectedThread);
                    });
                }
            }
        }
    }
}