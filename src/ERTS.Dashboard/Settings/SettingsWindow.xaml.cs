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
            this.Hide();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void SettingsTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch ((sender as TabControl).SelectedIndex)
            {
                case CONTROL_TAB_INDEX:
                    //Controls
                    StartControlsTab();
                    break;
                default:
                    StopControlsTab();
                    break;
            }
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            if (SettingsTabControl.SelectedIndex == CONTROL_TAB_INDEX)
            {
                if (GlobalData.input != null)
                {
                    StopControlsTab();
                }
            }
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            if (SettingsTabControl.SelectedIndex == CONTROL_TAB_INDEX)
            {
                StartControlsTab();
            }
        }

        void StartControlsTab()
        {
            if (GlobalData.input != null)
            {
                if (GlobalData.input.IsInputEngaged)
                {
                    GlobalData.input.DisengageInput();
                    GlobalData.input.InputEvent += Input_InputEvent;
                    Debug.WriteLine("Aquire all devices.");
                    GlobalData.input.AquireAllDevices(MainWindowHandle);
                }

            }
        }

        private void Input_InputEvent(object sender, Input.InputEventArgs e)
        {
            Debug.WriteLine(String.Format("Got input for binding. Device: {0}, Offset: {1}, Value: {2}", e.DeviceGuid, e.StateUpdate.RawOffset, e.StateUpdate.Value));
        }

        void StopControlsTab()
        {
            if (GlobalData.input != null)
            {
                GlobalData.input.InputEvent -= Input_InputEvent;
                if (!GlobalData.input.IsInputEngaged)
                {
                    GlobalData.input.EngageInput();
                    if (GlobalData.patchbox != null)
                    {
                        Debug.WriteLine("Unaquire all devices and aquire required devices.");
                        GlobalData.input.UnaquireAllDevices();
                        GlobalData.input.AquireDevices(GlobalData.patchbox.DeviceGuids, MainWindowHandle);
                    }
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
    }
}
