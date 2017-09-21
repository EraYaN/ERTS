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
