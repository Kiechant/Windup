using System;

namespace Unwind
{
	public class Updater
	{
		public event EventHandler<EventArgs> Updated;

		public virtual void OnUpdate()
		{
			if (Updated != null) Updated(this, EventArgs.Empty);
		}
	}
}
