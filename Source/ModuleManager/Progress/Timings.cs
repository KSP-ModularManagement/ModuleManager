using ModuleManager.Utils;

namespace ModuleManager.Progress
{
	public class Timings
	{
		internal Stopwatch Wait = new Stopwatch();
		internal Stopwatch Patching = new Stopwatch();
		internal Stopwatch PostPatching = new Stopwatch();
		internal Stopwatch ShaCalc = new Stopwatch();
	}
}
