﻿using System;
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
            var vm = new MainViewModel();
            Closing += vm.OnWindowClosing;
            this.DataContext = vm;

            InitializeComponent();
            
            Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            Thread.CurrentThread.Priority = ThreadPriority.Highest;
            if (File.Exists("cfg.bin"))
            {
                try
                {
                    IFormatter formatter = new BinaryFormatter();
                    using (Stream stream = new FileStream("cfg.bin", FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        GlobalData.cfg = (Settings)formatter.Deserialize(stream);
                    }
                }
                catch
                {
                    GlobalData.cfg = new Settings();
                }
                finally
                {
                }
            }
            else
            {
                GlobalData.cfg = new Settings();
            }
            settingsWindow = new SettingsWindow();
            //GlobalData.vis = new Visualization(powerCanvas);
            //GlobalData.vis.draw();
        }
        #region UI Event Handlers

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            //Stop enter, space and such to do weird things with the buttons on the form. DirectInput only.
            e.Handled = true;
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            TextBoxTraceListener tbtl = new TextBoxTraceListener(DebugTraceTextBox);
            Debug.Listeners.Add(tbtl);
            Debug.WriteLine("Welcome, Debug redirection enabled.");
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
        #endregion
    }
}
