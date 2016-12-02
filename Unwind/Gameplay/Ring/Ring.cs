using System;
using System.Drawing;
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

		protected Ring()
		{
			radius = 0.2f;
			cursorRadius = 0.04f;
			thickness = 0.015f;

			CreateMainRing();
			CreateCursor();
		}

		public void Draw(ShaderProgram shader)
		{
			mainRing.Draw(shader);

			// Generate new matrix using matrix stack
			GL.MatrixMode(MatrixMode.Modelview);
			GL.PushMatrix();
			GL.Rotate(angle, -Vector3.UnitZ);

			// Attach matrix to shader
			Matrix4 modelview;
			GL.GetFloat(GetPName.ModelviewMatrix, out modelview);
			GL.UniformMatrix4(shader.uniforms.modelviewMatrix, false, ref modelview);

			cursor.Draw(shader);

			// Reset matrix
			GL.PopMatrix();
			GL.GetFloat(GetPName.ModelviewMatrix, out modelview);
			GL.UniformMatrix4(shader.uniforms.modelviewMatrix, false, ref modelview);
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
			mainRing.colour = new Vector4(0.5f, 0.5f, 0.3f, 1.0f);
			mainRing.Update();
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
			cursor.colour = new Vector4(0.5f, 0.5f, 0.3f, 1.0f);
			cursor.Update();
		}

		protected abstract void Wind();
		protected abstract void Unwind();

		public void Dispose()
		{
			mainRing.Dispose();
			cursor.Dispose();
		}
	}
}
