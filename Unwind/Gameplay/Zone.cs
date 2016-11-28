using System;

namespace Unwind
{
	public class Zone
	{
		ZoneName name;
		/* Include attributes for obstacles,
		 game speeds, obstacle sequences, high scores,
		 etc. */

		public Zone() { }
	}

	public enum ZoneName
	{
		Dawn,
		Rainstorm,
		Tempest
	}
}
