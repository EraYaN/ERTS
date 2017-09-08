using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERTS.Dashboard.Communication {
	/// <summary>
	/// Class to make it easier to save the output from the car (left and right)
	/// </summary>
	public class OutputSequence
	{
		Sequence seq;		
		public double[] Seq
		{
			get { return seq.Data; }
		}		
		public long Index
		{
			get
			{
				return seq.Index;
			}
		}
		public long Length
		{
			get
			{
				return seq.Length;
			}
		}
		
		public OutputSequence(long Size)
		{
			seq = new Sequence(Size);			
		}		
		public double GetCurrentValue()
		{
			return seq.DataElement;
		}		
		public void Forward()
		{
			seq.Forward();
		}
		public void Backward()
		{
			seq.Backward();
		}
		public void Rewind()
		{
			seq.Rewind();
		}
		public void AddToFront(double Value)
		{
			seq.AddToFront(Value);
		}
		public void AddToBack(double Value)
		{
			seq.AddToBack(Value);
		}
		
	}
}
