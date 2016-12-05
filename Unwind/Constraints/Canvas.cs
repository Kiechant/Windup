using System.Drawing;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Unwind
{
	public class Canvas
	{
		public RectangleF frame;

		public Canvas(RectangleF frame)
		{
			this.frame = frame;
		}

		public Canvas(float left, float right, float bottom, float top)
		{
			this.frame = RectangleF.FromLTRB(left, top, right, bottom);
		}

		public void Draw(Game game)
		{
			GL.Begin(PrimitiveType.LineLoop);

			float z = 0.2f;
			GL.Color4(0.0f, 1.0f, 0.0f, 1.0f);

			Vector2 bottomLeft = LevelController.ScreenToWorld(BottomLeft(), game.Width, game.Height);
			Vector2 topLeft = LevelController.ScreenToWorld(TopLeft(), game.Width, game.Height);
			Vector2 topRight = LevelController.ScreenToWorld(TopRight(), game.Width, game.Height);
			Vector2 bottomRight = LevelController.ScreenToWorld(BottomRight(), game.Width, game.Height);

			GL.Vertex4(bottomLeft.X, bottomLeft.Y, z, 1);
			GL.Vertex4(topLeft.X, topLeft.Y, z, 1);
			GL.Vertex4(topRight.X, topRight.Y, z, 1);
			GL.Vertex4(bottomRight.X, bottomRight.Y, z, 1);

			GL.End();

			Debug.GetError();
		}

		public float Left
		{
			get { return frame.Left; }
			set {
				frame.Width += value - frame.Left;
				frame.X = value;
			}
		}

		public float Right
		{
			get { return frame.Right; }
			set { frame.Width += value - frame.Right; }
		}

		public float Bottom
		{
			get { return frame.Bottom; }
			set { frame.Height -= value - frame.Bottom; }
		}

		public float Top
		{
			get { return frame.Top; }
			set {
				frame.Height -= value - frame.Top;
				frame.Y = value;
			}
		}

		public Vector2 BottomLeft()
		{
			return new Vector2(frame.Left, frame.Bottom);
		}

		public Vector2 BottomCentre()
		{
			return new Vector2(frame.Left + frame.Width / 2, frame.Bottom);
		}

		public Vector2 BottomRight()
		{
			return new Vector2(frame.Right, frame.Bottom);
		}

		public Vector2 CentreLeft()
		{
			return new Vector2(frame.Left, frame.Top + frame.Height / 2);
		}

		public Vector2 Centre()
		{
			return new Vector2(frame.Left + frame.Width / 2, frame.Top + frame.Height / 2);
		}

		public Vector2 CentreRight()
		{
			return new Vector2(frame.Right, frame.Top + frame.Height / 2);
		}

		public Vector2 TopLeft()
		{
			return new Vector2(frame.Left, frame.Top);
		}

		public Vector2 TopCentre()
		{
			return new Vector2(frame.Left + frame.Width / 2, frame.Top);
		}

		public Vector2 TopRight()
		{
			return new Vector2(frame.Right, frame.Top);
		}
	}
}