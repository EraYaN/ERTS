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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Diagnostics;
using SharpDX.DirectInput;
using System.Windows.Interop;
using ERTS.Dashboard.ViewModel;
using ERTS.Dashboard.Utility;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using ERTS.Dashboard.Configuration;

namespace ERTS.Dashboard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SettingsWindow settingsWindow;

        public MainWindow()
        {     
            Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            Thread.CurrentThread.Priority = ThreadPriority.Highest;           
                      

            InitializeComponent();
        }
        #region UI Event Handlers

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            //Stop enter, space and such to do weird things with the buttons on the form. DirectInput only.
            e.Handled = true;
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            /*TextBoxTraceListener tbtl = new TextBoxTraceListener(DebugTraceTextBox);
            Debug.Listeners.Add(tbtl);
            Debug.WriteLine("Welcome, Debug redirection enabled.");*/
            GlobalData.InitStageOne(new WindowInteropHelper(this).Handle);

            ((MainViewModel)DataContext).InitStageOne();
            settingsWindow = new SettingsWindow(new WindowInteropHelper(this).Handle);
        }
        private void SettingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            settingsWindow.Show();
            settingsWindow.Focus();
        }

        private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow aboutWindow = new AboutWindow();
            aboutWindow.ShowDialog();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            settingsWindow.Close();
            GlobalData.Dispose();
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Application.Current.Shutdown();
        }

        private void StartAllButton_Click(object sender, RoutedEventArgs e)
        {
            GlobalData.InitStageTwo();

            ((MainViewModel)DataContext).InitStageTwo();
        }

        #endregion


    }
}
