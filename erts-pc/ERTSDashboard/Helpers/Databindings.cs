using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ERTS.Dashboard
{
	/// <summary>
	/// Class used as a custom "ViewModel" without actually being a ViewModel.
	/// </summary>
	public class Databindings : INotifyPropertyChanged
	{
        public event PropertyChangedEventHandler PropertyChanged;
        //TODO move all code to MainViewModel
        /*
		

        public string WindowTitle {
            get {
                Assembly currAss = Assembly.GetExecutingAssembly();
                string fileVersion = FileVersionInfo.GetVersionInfo(currAss.Location).FileVersion;
                string processorArchitecture = currAss.GetName().ProcessorArchitecture.ToString();
#if DEBUG
                string Branch = "Debug";
#else
                string Branch = "Release";
#endif
                return String.Format("SoCeBa Director v{0} {1} {2} by Erwin de Haan", fileVersion, processorArchitecture, Branch);
            }
        }

        public string DebugInfo {
            get {
                return VersionInfo;
            }
        }

        public string VersionInfo
		{
			get
			{
				StringBuilder sb = new StringBuilder();
				Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
				foreach (Assembly a in assemblies)
				{
					AssemblyName an = a.GetName();
					if(a.GlobalAssemblyCache==false)
						sb.AppendLine(String.Format("{0} v{1} {2}", an.Name, an.Version, an.ProcessorArchitecture));					
				}
				
				return sb.ToString();
			}
		}
		public string SerialPortStatus
		{
			get
			{
				if (GlobalData.com == null)
					return "NULL";
				if (GlobalData.com.IsOpen)
				{
					return GlobalData.com.BytesInRBuffer + "|" + GlobalData.com.BytesInTBuffer;
				}
				else
				{
					return "NC";
				}
			}
		}

		public string LastPing
		{
			get
			{
				if (GlobalData.ctr != null)
				{
					if (GlobalData.ctr.LastPing == -1)
						return "xx ms";
					else
						return string.Format("{0:f1} ms", GlobalData.ctr.LastPing);
				}
				else
				{
					return "ctr is null";
				}
			}
		}

        public string AlgorithmRate {
            get {
                if (GlobalData.ctr != null) {
                    if (GlobalData.ctr.AlgorithmRate <= 0)
                        return "xx Hz";
                    else
                        return string.Format("{0:f1} Hz", GlobalData.ctr.AlgorithmRate);
                } else {
                    return "ctr is null";
                }
            }
        }

        public Brush SerialPortStatusColor
		{
			get
			{
				if (GlobalData.com == null)
					return Brushes.Red;
				if (GlobalData.com.IsOpen)
				{
					int b = GlobalData.com.BytesInRBuffer + GlobalData.com.BytesInTBuffer;
					if (b == 0)
					{
						return Brushes.Green;
					}
					else if (b > 0 && b <= 2)
					{
						return Brushes.LightGreen;
					}
					else
					{
						return Brushes.Orange;
					}
				}
				else
				{
					return Brushes.OrangeRed;
				}
			}
		}
		
		public double PowerGraphMaxTime
		{
			get
			{
                if (GlobalData.cfg == null) {
                    return 1;
                }

                if (GlobalData.ctr == null)
					return Controller.HistoryPoints * GlobalData.cfg.TickTime / GlobalData.cfg.UpdateTickInterval;

				return GlobalData.cfg.TickTime/GlobalData.cfg.UpdateTickInterval * GlobalData.ctr.PowerHistory.Length;
			}
		}

        public double DutyGraphMaxTime {
            get {
                if (GlobalData.cfg == null) {
                    return 1;
                }
                if (GlobalData.ctr == null)
                    return Controller.HistoryPoints * GlobalData.cfg.TickTime / GlobalData.cfg.UpdateTickInterval;

                return GlobalData.cfg.TickTime / GlobalData.cfg.UpdateTickInterval * GlobalData.ctr.DutyHistory.Length;
            }
        }

        public double CurrentGraphMaxTime {
            get {
                if (GlobalData.cfg == null) {
                    return 1;
                }
                if (GlobalData.ctr == null)
                    return Controller.HistoryPoints * GlobalData.cfg.TickTime / GlobalData.cfg.UpdateTickInterval;

                return GlobalData.cfg.TickTime / GlobalData.cfg.UpdateTickInterval * GlobalData.ctr.CurrentHistory.Length;
            }
        }
        public List<ScatterPoint> PowerPoints {
            get {
                List<ScatterPoint> l = new List<ScatterPoint>();
                if (GlobalData.ctr == null)
                    return l;

                double Duty = 0;
                double Current = 0;
                double Power = 0;
                long size = Math.Min(Math.Min(GlobalData.ctr.PowerHistory.Length, GlobalData.ctr.CurrentHistory.Length), GlobalData.ctr.DutyHistory.Length);
                for (int i = 0; i < size; i++) {
                    Duty = GlobalData.ctr.DutyHistory.Seq[i];
                    Current = GlobalData.ctr.CurrentHistory.Seq[i];
                    Power = GlobalData.ctr.PowerHistory.Seq[i];
                    if (Power > 0) {
                        l.Add(new ScatterPoint(Duty, Current, 4, Power));
                    }                  
                }
                return l;
            }
        }
        public List<DataPoint> PowerGraphPoints
		{
			get
			{
				List<DataPoint> l = new List<DataPoint>();
				if (GlobalData.ctr == null)
					return l;

				double x = 0;
				foreach (double d in GlobalData.ctr.PowerHistory.Seq)
				{
					l.Add(new DataPoint(x, d));
					x += GlobalData.cfg.TickTime / GlobalData.cfg.UpdateTickInterval;
				}
				return l;
			}
		}
        public List<DataPoint> RealVoltageGraphPoints {
            get {
                List<DataPoint> l = new List<DataPoint>();
                if (GlobalData.ctr == null)
                    return l;

                double x = 0;
                foreach (double d in GlobalData.ctr.RealVoltageHistory.Seq) {
                    l.Add(new DataPoint(x, d));
                    x += GlobalData.cfg.TickTime / GlobalData.cfg.UpdateTickInterval;
                }
                return l;
            }
        }
        public List<DataPoint> RealCurrentGraphPoints {
            get {
                List<DataPoint> l = new List<DataPoint>();
                if (GlobalData.ctr == null)
                    return l;

                double x = 0;
                foreach (double d in GlobalData.ctr.RealCurrentHistory.Seq) {
                    l.Add(new DataPoint(x, d));
                    x += GlobalData.cfg.TickTime / GlobalData.cfg.UpdateTickInterval;
                }
                return l;
            }
        }
        public List<DataPoint> DutyGraphPoints {
            get {
                List<DataPoint> l = new List<DataPoint>();
                if (GlobalData.ctr == null)
                    return l;

                double x = 0;
                foreach (double d in GlobalData.ctr.DutyHistory.Seq) {
                    l.Add(new DataPoint(x, d));
                    x += GlobalData.cfg.TickTime / GlobalData.cfg.UpdateTickInterval;
                }
                return l;
            }
        }
        public List<DataPoint> CurrentGraphPoints {
            get {
                List<DataPoint> l = new List<DataPoint>();
                if (GlobalData.ctr == null)
                    return l;

                double x = 0;
                foreach (double d in GlobalData.ctr.CurrentHistory.Seq) {
                    l.Add(new DataPoint(x, d));
                    x += GlobalData.cfg.TickTime / GlobalData.cfg.UpdateTickInterval;
                }
                return l;
            }
        }*/

        public void UpdateProperty(string name)
		{
			OnPropertyChanged(name);
		}

		protected void OnPropertyChanged(string name)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(name));
			}
		}
	}
}
