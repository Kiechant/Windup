using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Unwind
{
	public class Raindrop : Obstacle
	{
		public const float BaseA = 0.08f;
		public const float AOverB = 2.0f;

		private float a;
		private float b;
		private uint steps;

		public Raindrop(float angle, uint steps)
		{
			this.angle = angle;
			a = BaseA;
			b = a / AOverB;
			this.steps = steps;
			this.fallRate = BaseFallRate + 0.05f;

			bool disposed;
			ProcessAppearance(out disposed);
			SetTriangles();
		}

		public override void Update(out bool disposed)
		{
			radius -= Time.deltaTimeSeconds * fallRate;
			ProcessAppearance(out disposed);

			shape.Update();
		}

		/* Updates shape appearance following a transformation. */
		private void ProcessAppearance(out bool disposed)
		{
			float ringRadius = 0.2f;
			float drawA = a;

			if (radius < ringRadius + a * 0.5f)
			{
				disposed = true;
				Dispose();
				return;
			}
			if (radius < ringRadius + a)
			{
				drawA = radius - ringRadius;
			}

			SetVertices(drawA);

			disposed = false;
		}

		/* Builds the contour of the paddle and assigns the vertices to the mesh.
		 This should be called whenever the shape undergoes distrortion. */
		private void SetVertices(float drawA)
		{
			ShapeBuilder shapeBuilder = new ShapeBuilder();

			double cos = Math.Cos(angle);
			double sin = Math.Sin(angle);
			Vector2 centre = new Vector2((float)(radius * cos), (float)(radius * sin));
			Vector2 outer = new Vector2((float)((radius + drawA) * cos), (float)((radius + drawA) * sin));

			shapeBuilder.Open(centre);
			shapeBuilder.AddLine(outer);
			shapeBuilder.AddEllipticalCurve(centre, drawA / b, MathHelper.TwoPi, steps);
			shapeBuilder.Close();

			shape.vertices = shapeBuilder.GetVertices();
		}

		/* Calculates the triangles of the paddle shape and assigns them to the mesh.
		 This should be called only once after the vertices are first set. */
		private void SetTriangles()
		{
			shape.type = PrimitiveType.TriangleFan;

			int n = shape.vertices.Length;
			int[] triangles = new int[n + 1];

			for (int i = 0; i < n; i++)
			{
				triangles[i] = i;
			}
			triangles[n] = 1;

			shape.triangles = triangles;
		}
	}
}
