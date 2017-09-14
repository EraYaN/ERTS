﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace ERTS.Dashboard
{
	/// <summary>
	/// Interaction logic for SettingsWindow.xaml
	/// </summary>
	public partial class SettingsWindow : Window
	{		
		public SettingsWindow()
		{			
			InitializeComponent();
			//Read out available comports 
			string[] ports = SerialPort.GetPortNames();
			foreach (string s in ports)
			{				
				((ConfigurationViewModel)DataContext).Comports.Add(s);
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

		private void openControlPanelButton_Click(object sender, RoutedEventArgs e)
		{
			if (GlobalData.cfg != null)
			{
				
			}
		}
        		
	}
}