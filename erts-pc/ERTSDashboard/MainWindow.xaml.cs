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

namespace ERTS.Dashboard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
 
        public MainWindow()
        {
            InitializeComponent();

            TextBoxTraceListener tbtl = new TextBoxTraceListener(DebugTraceTextBox);
            Debug.Listeners.Add(tbtl);
            Debug.WriteLine("Welcome, Debug redirection enabled.");

            var vm = new MainViewModel();
            Closing += vm.OnWindowClosing;
            this.DataContext = vm;
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            //Stop enter, space and such to do weird things with the buttons on the form. DirectInput only.
            e.Handled = true;
        }
    }
}
