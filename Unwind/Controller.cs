using System;

namespace Unwind
{
	public class Controller
	{
		public virtual void Start() { }

		public virtual void OnUpdate(object source, EventArgs e) { }

		public virtual void OnRender(object source, EventArgs e) { }
	}
}
