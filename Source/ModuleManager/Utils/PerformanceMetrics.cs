using System;
using System.Diagnostics;
using UnityEngine;

namespace ModuleManager.Utils
{
	internal class PerformanceMetrics
	{
		private static PerformanceMetrics INSTANCE;
		internal static PerformanceMetrics Instance => INSTANCE ?? (INSTANCE = new PerformanceMetrics());

		private readonly Stopwatch totalTime = new Stopwatch();
		private readonly int vSyncCount;
		private readonly int targetFrameRate;

		internal bool IsRunning => this.totalTime.IsRunning;
		internal float ElapsedTimeInSecs => ((float)this.totalTime.ElapsedMilliseconds / 1000);

		private PerformanceMetrics()
		{
			this.vSyncCount = QualitySettings.vSyncCount;
			this.targetFrameRate = Application.targetFrameRate;
		}

		internal void Start()
		{
			if (this.IsRunning) return; // Playing safe

			Application.targetFrameRate = 99999; // What the heck, why not? :D
			QualitySettings.vSyncCount = 0;
			this.totalTime.Start();
		}

		internal void Stop()
		{
			this.totalTime.Stop();
			Application.targetFrameRate = this.targetFrameRate;
			QualitySettings.vSyncCount = this.vSyncCount;
		}

		internal void Destroy()
		{
			if (this.IsRunning) this.Stop();  // Playing safe
			INSTANCE = null;
		}
	}
}
