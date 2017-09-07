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

namespace ERTSDashboard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        InputManager inputManager;
        List<DeviceInstance> devices;
        public MainWindow()
        {
            InitializeComponent();            
        }
        
        private void UseControllerButton_Click(object sender, RoutedEventArgs e)
        {
            if (ControllerComboBox.SelectedItem != null)
            {
                inputManager.BindDevice((DeviceInstance)ControllerComboBox.SelectedItem, new WindowInteropHelper(this).Handle);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (inputManager != null) inputManager.StopThread();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            inputManager = new InputManager();
            devices = inputManager.EnumerateControllers();
            ControllerComboBox.ItemsSource = devices;
            inputManager.StartThread();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            //Stop enter, space and such to do weird things with the buttons on the form. DirectInput only.
            e.Handled = true;
        }
    }
}
