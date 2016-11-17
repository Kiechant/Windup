using System;

namespace Unwind
{
	public class GameEventsManager
	{
		public event EventHandler<EventArgs> Update;

		public virtual void OnUpdate()
		{
			if (Update != null) Update(this, EventArgs.Empty);
		}

		public event EventHandler<EventArgs> Render;

		public virtual void OnRender()
		{
			if (Render != null) Render(this, EventArgs.Empty);
		}
	}
}
