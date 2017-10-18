using ERTS.Dashboard.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimingDividerTests
{
    class Program
    {
        static int counter_dmp = 0;
        static int counter_hb = 0;
        static Stopwatch stopwatch_hb;
        static Stopwatch stopwatch_dmp;
        const int DIVIDER_DMP = 9;
        const int DIVIDER_HB = 9;

        static void Main(string[] args)
        {
            stopwatch_hb = new Stopwatch();
            stopwatch_dmp = new Stopwatch();
            MultimediaTimer RCTimer = new MultimediaTimer(10);

            RCTimer.Elapsed += RCTimer_Elapsed;
            RCTimer.Start();
            stopwatch_hb.Start();
            stopwatch_dmp.Start();
            Console.ReadKey();

        }

        private static void RCTimer_Elapsed(object sender, EventArgs e)
        {
            if (counter_dmp == DIVIDER_DMP)
            {
                counter_dmp = 0;
                stopwatch_dmp.Stop();
                Console.WriteLine(String.Format("DMP Event {0:N2} ms since last event.", stopwatch_dmp.Elapsed.TotalMilliseconds));
                stopwatch_dmp.Restart();
                if (counter_hb == DIVIDER_HB)
                {
                    counter_hb = 0;
                    stopwatch_hb.Stop();

                    Console.WriteLine(String.Format("HB Event {0:N2} ms since last event.", stopwatch_hb.Elapsed.TotalMilliseconds));
                    stopwatch_hb.Restart();
                }
                else
                {
                    counter_hb++;
                }
            }
            else
            {
                counter_dmp++;
            }
        }
    }
}
