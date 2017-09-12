using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ERTS.Dashboard.Utility
{
    class TextBoxTraceListener : TraceListener {
        private TextBox tBox;

        public TextBoxTraceListener(TextBox box) {
            this.tBox = box;
        }

        public override void Write(string msg) {
            //allows tBox to be updated from different thread
            try {
                tBox.Dispatcher.Invoke((delegate () {
                    tBox.AppendText(msg);
                    tBox.ScrollToEnd();
                }));
            } catch (TaskCanceledException e) {
                Debug.WriteLine(e.ToString());
            }
        }

        public override void WriteLine(string msg) {
            Write(msg + Environment.NewLine);
        }
    }
}
