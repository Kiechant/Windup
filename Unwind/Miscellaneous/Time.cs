using System;
using System.Diagnostics;

namespace Unwind
{
	public class Time
	{
		private static Stopwatch watch;
		public static long deltaTime { get; private set; }
		public static float deltaTimeSeconds
		{
			get { return 0.001f * deltaTime; }
		}

		public static void Start()
		{
			watch = new Stopwatch();
			watch.Start();
		}

		public static void OnUpdate(object source, EventArgs e)
		{
			deltaTime = watch.ElapsedMilliseconds;
			watch.Restart();
		}
	}
}
