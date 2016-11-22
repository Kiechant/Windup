using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Unwind
{
	public abstract class Ring : IDisposable
	{
		public Vector3 rotAxis;

		protected float angle = 0; // in degrees
		protected float windSpeed = 400; // in degrees per second
		public float radius { get; protected set; }
		public float cursorRadius { get; protected set; }
		public float thickness { get; protected set; }

		protected bool unwinding = false;
		protected bool unwound = true;

		protected Shape mainRing;
		protected Shape cursor;

		/* Start and Update are called at the beginning
		of the children's Start and Update functions */

		protected Ring()
		{
			radius = 0.2f;
			cursorRadius = 0.04f;
			thickness = 0.015f;

			CreateMainRing();
			CreateCursor();
		}

		public void OnRender(object sender, EventArgs e)
		{
			mainRing.Draw();

			Console.WriteLine("rotating by " + angle);
			GL.MatrixMode(MatrixMode.Modelview);
			GL.PushMatrix();
			GL.Rotate(angle, Vector3.UnitZ);

			//GL.Translate(radius * Math.Cos(angle), radius * Math.Sin(angle), 0);
			Matrix4 matrix;
			GL.GetFloat(GetPName.ModelviewMatrix, out matrix);

			var manager = (GameEventsManager)sender;
			int loc = GL.GetUniformLocation(manager.program, "modelviewMatrix");
			GL.UniformMatrix4(loc, false, ref matrix);

			cursor.Draw();
			GL.PopMatrix();

			matrix = new Matrix4();
			GL.UniformMatrix4(loc, false, ref matrix);
		}

		private void CreateMainRing()
		{
			ShapeBuilder builder = new ShapeBuilder();
			builder.Open(new Vector2(radius + 0.5f * thickness, 0));
			builder.AddArc(Vector2.Zero, MathHelper.TwoPi, LevelController.AngleSteps);
			builder.AddLine(new Vector2(radius - 0.5f * thickness, 0));
			builder.AddArc(Vector2.Zero, MathHelper.TwoPi, LevelController.AngleSteps);
			builder.Close();

			Vector2[] vertices = builder.GetVertices();
			int n = vertices.Length;
			int halfN = n / 2;
			int[] indices = new int[n];

			for (int i = 0; i < halfN; i++)
			{
				indices[2 * i] = i;
				indices[2 * i + 1] = i + halfN;
			}

			mainRing = new Shape(vertices, indices);
			mainRing.type = PrimitiveType.TriangleStrip;
		}

		private void CreateCursor()
		{
			ShapeBuilder builder = new ShapeBuilder();
			Vector2 centre = new Vector2(0, radius);
			builder.Open(centre);
			builder.AddLine(new Vector2(cursorRadius, radius));
			builder.AddArc(centre, MathHelper.TwoPi, 20);
			builder.Remove(0);
			builder.Close();

			Vector2[] vertices = builder.GetVertices();
			int n = vertices.Length;
			int[] indices = new int[n];

			for (int i = 0; i < n; i++)
			{
				indices[i] = i;
			}

			cursor = new Shape(vertices, indices);
			cursor.type = PrimitiveType.TriangleFan;
		}

		protected abstract void Wind();
		protected abstract void Unwind();

		/* Translates cursor position. */
		protected void UpdateRotation()
		{
			Console.WriteLine("moving cursor to " + angle);
			GL.Translate(radius * Math.Cos(angle), radius * Math.Sin(angle), 0);
			GL.BindBuffer(BufferTarget.ArrayBuffer, cursor.positionBuffer);
			GL.Translate(0, 0, 0);
		}

		public void Dispose()
		{
			mainRing.Dispose();
			cursor.Dispose();
		}
	}
}
