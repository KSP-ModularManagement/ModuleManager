/*
	This file is part of Module Manager /L
		© 2018-2023 LisiasT

	Module Manager /L is licensed as follows:
		* GPL 3.0 : https://www.gnu.org/licenses/gpl-3.0.txt

	Module Manager /L is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.

	You should have received a copy of the GNU General Public License 3.0
	along with Module Manager /L. If not, see <https://www.gnu.org/licenses/>.
*/
using System;
using System.Diagnostics;
using UnityEngine;
using KU = KSPe.Util;

namespace ModuleManager.Utils
{
	/**
	 * That's the history:
	 * 
	 * The Loading Scene is taking more time than needed on most cases, due the FramePerSecond settings on Unity. It's a collateal
	 * effect from the fact that the Coroutines works around the Update, and the Update is called (Standard conditions for temperature
	 * and pressure) once per frame!
	 * 
	 * So, by trimming down the FPS or activating the V-Sync, you end up sabotaging your loading times!
	 * 
	 * This "fixes" the problem by cranking up the FPS to ome absudly high value (to avoid any trimming down by Unity), and then
	 * restoring the original values once the loading is done.
	 * 
	 * PROBLEM: The original code already did that, as you can check on the commit this message was added. Exactly **why** this
	 * thing gave me a small boost is not understood at this moment. On a first though, this should not had happened...
	 */ 
	internal class PerformanceMetrics
	{
		private static PerformanceMetrics INSTANCE;
		internal static PerformanceMetrics Instance => INSTANCE ?? (INSTANCE = new PerformanceMetrics());

		private readonly KU.Stopwatch totalTime = new KU.Stopwatch();
		private readonly int vSyncCount;
		private readonly int targetFrameRate;
		private readonly bool runInBackground;

		internal bool IsRunning => this.totalTime.IsRunning;
		internal float ElapsedTimeInSecs => this.totalTime;

		private PerformanceMetrics()
		{
			this.vSyncCount = GameSettings.SYNC_VBL;
			this.targetFrameRate = GameSettings.FRAMERATE_LIMIT;
			this.runInBackground = GameSettings.SIMULATE_IN_BACKGROUND;
			Logging.ModLogger.Instance.Log(LogType.Exception, string.Format("PerformanceMetrics: Unity Settings On Constructor: {0} {1} {2}", QualitySettings.vSyncCount, Application.targetFrameRate, Application.runInBackground));
			Logging.ModLogger.Instance.Log(LogType.Exception, string.Format("PerformanceMetrics: GameSettings On Constructor: {0} {1} {2}", this.vSyncCount, this.targetFrameRate, this.runInBackground));
		}

		internal void Start()
		{
			if (this.IsRunning) return; // Playing safe

			Application.targetFrameRate = 99999; // What the heck, why not? :D
			QualitySettings.vSyncCount = 0;
			this.totalTime.Start();
			Logging.ModLogger.Instance.Log(LogType.Exception, string.Format("PerformanceMetrics: Unity Settings On Start: {0} {1} {2}", QualitySettings.vSyncCount, Application.targetFrameRate, Application.runInBackground));
		}

		internal void Stop()
		{
			this.totalTime.Stop();
			Application.targetFrameRate = this.targetFrameRate;		// GameSettings.FRAMERATE_LIMIT ?
			QualitySettings.vSyncCount = this.vSyncCount;			// GameSettings.SYNC_VBL ?
			Application.runInBackground = this.runInBackground;		// GameSettings.SIMULATE_IN_BACKGROUND ?
			Logging.ModLogger.Instance.Log(LogType.Exception, string.Format("PerformanceMetrics: Unity Settings after Stop: {0} {1} {2}", QualitySettings.vSyncCount, Application.targetFrameRate, Application.runInBackground));
		}

		internal void Destroy()
		{
			if (this.IsRunning) this.Stop();  // Playing safe
			INSTANCE = null;
			Logging.ModLogger.Instance.Log(LogType.Exception, string.Format("PerformanceMetrics: Unity Settings On Destroy: {0} {1} {2}", QualitySettings.vSyncCount, Application.targetFrameRate, Application.runInBackground));
		}
	}
}
