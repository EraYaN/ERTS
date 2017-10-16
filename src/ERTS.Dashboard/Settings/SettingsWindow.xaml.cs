using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO.Ports;
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

namespace ERTS.Dashboard.Configuration
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        const int CONTROL_TAB_INDEX = 3;

        bool IsInDiscovery = false;
        //TODO make handle class member of InputManager instead
        IntPtr MainWindowHandle;

        public SettingsWindow(IntPtr _MainWindowHandle)
        {
            MainWindowHandle = _MainWindowHandle;

            InitializeComponent();
            //Read out available comports 
            string[] ports = SerialPort.GetPortNames();
            foreach (string s in ports)
            {
                ((ConfigurationViewModel)DataContext).ComPorts.Add(s);
            }
            //DataContext = this;
        }
        private void SettingsDialogCloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsInDiscovery)
            {
                e.Handled = true;
            }
            else
            {
                this.Hide();
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            if (!IsInDiscovery)
            {
                this.Hide();
            }
        }

        void StartControlDicovery()
        {
            if (GlobalData.input != null)
            {
                if (GlobalData.input.IsInputEngaged)
                {
                    GlobalData.input.DisengageInput();
                }
                GlobalData.input.InputEvent += Input_InputEvent;
                Debug.WriteLine("Aquire all devices.");
                GlobalData.input.AquireAllDevices(MainWindowHandle);

            }
        }

        private void Input_InputEvent(object sender, Input.InputEventArgs e)
        {
            string text = String.Format("Device: {0}, Offset: {1}, Value: {2}", e.DeviceGuid, e.StateUpdate.RawOffset, e.StateUpdate.Value);
            Debug.WriteLine(text);
            LastInputDiscoveryTextBox.Dispatcher.Invoke(() => { LastInputDiscoveryTextBox.Text = text; });
        }

        void StopControlDiscovery()
        {
            if (GlobalData.input != null)
            {
                GlobalData.input.InputEvent -= Input_InputEvent;
                if (!GlobalData.input.IsInputEngaged && GlobalData.ctr != null)
                {

                    GlobalData.input.EngageInput();
                }
                if (GlobalData.patchbox != null)
                {
                    Debug.WriteLine("Unaquire all devices and aquire required devices.");
                    GlobalData.input.UnaquireAllDevices();
                    GlobalData.input.AquireDevices(GlobalData.patchbox.DeviceGuids, MainWindowHandle);
                }                
            }
        }

        private void SettingsSaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (GlobalData.cfg != null)
            {
                GlobalData.cfg.Save();
            }
            this.Hide();
        }

        private void StartControlDiscoveryButton_Click(object sender, RoutedEventArgs e)
        {
            IsInDiscovery = true;
            StartControlDiscoveryButton.IsEnabled = false;
            StopControlDiscoveryButton.IsEnabled = true;
            SettingsSaveButton.IsEnabled = false;
            StartControlDicovery();
            StartedControlDiscoveryTextBox.Visibility = Visibility.Visible;
            //LastInputDiscoveryTextBox.Visibility = Visibility.Visible;
            StoppedControlDiscoveryTextBox.Visibility = Visibility.Hidden;
        }

        private void StopControlDiscoveryButton_Click(object sender, RoutedEventArgs e)
        {
            IsInDiscovery = false;
            StartControlDiscoveryButton.IsEnabled = true;
            StopControlDiscoveryButton.IsEnabled = false;
            SettingsSaveButton.IsEnabled = true;
            StopControlDiscovery();
            StartedControlDiscoveryTextBox.Visibility = Visibility.Hidden;
            //LastInputDiscoveryTextBox.Visibility = Visibility.Hidden;
            StoppedControlDiscoveryTextBox.Visibility = Visibility.Visible;
        }
    }
}
