using System;

namespace Unwind
{
	public class GameEventsManager
	{
		public GameParameters parameters { get; private set; }

		public GameEventsManager(GameParameters parameters)
		{
			this.parameters = parameters;
		}

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

		public event EventHandler<EventArgs> MouseDown;
		public virtual void OnMouseDown()
		{
			if (MouseDown != null) MouseDown(this, EventArgs.Empty);
		}

		public event EventHandler<EventArgs> MouseUp;
		public virtual void OnMouseUp()
		{
			if (MouseDown != null) MouseUp(this, EventArgs.Empty);
		}
	}
}
