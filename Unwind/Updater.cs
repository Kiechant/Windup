using System;

namespace Unwind
{
	public class Updater
	{
		public event EventHandler<EventArgs> FrameUpdated;

		public virtual void OnUpdateFrame()
		{
			if (FrameUpdated != null) FrameUpdated(this, EventArgs.Empty);
		}
	}
}
