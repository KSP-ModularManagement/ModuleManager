using System;
namespace ModuleManager.Utils
{
	public class Stopwatch
	{
		public float Value { get; private set;}
		public bool IsRunning => null != this.timer && this.timer.IsRunning;
		public bool IsValid => null == this.timer && this.Value >= 0.0f;
		private System.Diagnostics.Stopwatch timer = null;

		public Stopwatch()
		{
			this.Value = -1;
		}

		public void Start()
		{
			if (this.IsRunning) return;
			this.Value = -1;
			this.timer = new System.Diagnostics.Stopwatch();
			this.timer.Start();
		}

		public void Stop()
		{
			if (null == this.timer) return;
			this.timer.Stop();
			this.Value = (float)(this.timer.ElapsedMilliseconds) / 1000.0f;
			this.timer = null;
		}

		public static implicit operator float(Stopwatch o) => o.Value;
		public static implicit operator string(Stopwatch o) => o.Value.ToString("F3") + "s";
	}
}
