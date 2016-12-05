using System;
using System.Drawing;

namespace Unwind
{
	public class Constrainer
	{
		Canvas frame;
		public Canvas canvasToConstrain;
		public float[] position { get; private set; }
		Constraint[] constraints = new Constraint[2];

		public Constrainer(Canvas frame)
		{
			this.frame = frame;
			position = new float[2];
		}

		public Constrainer(Canvas frame, Constraint x, Constraint y) : this(frame)
		{
			AttachConstraint(x);
			AttachConstraint(y);
		}

		public void AttachConstraint(Constraint constraint)
		{
			if (constraint.anchor == Anchor.Left || constraint.anchor == Anchor.Right)
			{
				constraints[0] = constraint;
			}
			else
			{
				constraints[1] = constraint;
			}
		}

		public bool IsComplete()
		{
			if (constraints[0] != null && constraints[1] != null)
				return true;
			return false;
		}

		public virtual void OnResize(object source, EventArgs e)
		{
			frame = ((Game)source).mainCanvas;

			for (int i = 0; i < constraints.Length; i++)
			{
				// Modifies constraint
				Constraint c = constraints[i];

				// Takes position of anchor of canvas
				float anchorPos = 0.0f;
				float d = 0.0f;
				int sign = 1;

				switch (c.anchor)
				{
					case Anchor.Left:
						anchorPos = c.canvas.Left;
						sign = 1;
						break;
					case Anchor.Right:
						anchorPos = c.canvas.Right;
						d = canvasToConstrain.frame.Width;
						sign = -1;
						break;
					case Anchor.Bottom:
						anchorPos = c.canvas.Bottom;
						d = canvasToConstrain.frame.Height;
						sign = -1;
						break;
					case Anchor.Top:
						anchorPos = c.canvas.Top;
						sign = 1;
						break;
				}

				if (c.type == ConstraintType.Absolute)
				{
					float pos = anchorPos + sign * c.amount - d;

					if (c.anchor == Anchor.Left || c.anchor == Anchor.Right)
					{
						position[0] = pos;
					}
					else
					{
						position[1] = pos;
					}
				}

				else // c.type == ConstraintType.Relative
				{
					float attachmentPos = 0.0f;

					if (c.attachment == null)
					{
						// Takes position of opposite anchor.
						switch (c.anchor)
						{
							case Anchor.Left:	attachmentPos = c.canvas.Right;		break;
							case Anchor.Right:	attachmentPos = c.canvas.Left;		break;
							case Anchor.Bottom:	attachmentPos = c.canvas.Top;		break;
							case Anchor.Top:	attachmentPos = c.canvas.Bottom;	break;
						}
					}

					else
					{
						// Takes position of attachment at opposite anchor.
						switch (c.anchor)
						{
							case Anchor.Left:	attachmentPos = c.attachment.Right;		break;
							case Anchor.Right:	attachmentPos = c.attachment.Left;		break;
							case Anchor.Bottom:	attachmentPos = c.attachment.Top;		break;
							case Anchor.Top:	attachmentPos = c.attachment.Bottom;	break;
						}
					}

					position[i] = Mathc.Lerp(anchorPos, attachmentPos, c.amount);
				}
			}

			canvasToConstrain.frame.X = position[0];
			canvasToConstrain.frame.Y = position[1];
		}
	}
}
