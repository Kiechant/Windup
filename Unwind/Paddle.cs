using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;

namespace Unwind
{
	/* Defines a circular paddle. */
	public class Paddle : Obstacle
	{
		public const float MinSize = MathHelper.PiOver4;
		public const float MaxSize = MathHelper.PiOver4 + MathHelper.PiOver2;
		public const float Thickness = 0.1f;

		private float startAngle;
		private float endAngle;
		private uint steps;

		/* Creates a paddle with a particular start angle and angular size
		 going in the counter-clockwise direction. Number of steps specifies
		 the numberof edges used to draw the arc. */
		public Paddle(float startAngle, float angularSize, uint steps)
		{
			this.startAngle = startAngle;
			this.endAngle = startAngle + angularSize;
			this.angle = startAngle + angularSize * 0.5f;
			this.steps = steps;

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
			float drawRadius = radius;
			float drawThickness = Thickness;

			if (radius < ringRadius - Thickness)
			{
				// TODO: delete object
				disposed = true;
				Dispose();
				return;
			}
			if (radius < ringRadius)
			{
				drawRadius = ringRadius;
				drawThickness = Thickness - (ringRadius - radius);
			}

			SetVertices(drawRadius, drawThickness);

			disposed = false;
		}

		/* Builds the contour of the paddle and assigns the vertices to the mesh.
		 This should be called whenever the shape undergoes distrortion. */
		private void SetVertices(float drawRadius, float drawThickness)
		{
			Vector2 outerStart = new Vector2((float)Math.Cos(startAngle), (float)Math.Sin(startAngle)) * (drawRadius + drawThickness);
			Vector2 innerEnd = new Vector2((float)Math.Cos(endAngle), (float)Math.Sin(endAngle)) * drawRadius;

			ShapeBuilder shapeBuilder = new ShapeBuilder();
			shapeBuilder.Open(outerStart);
			shapeBuilder.AddArc(new Vector2(0, 0), endAngle - startAngle, steps);
			shapeBuilder.AddLine(innerEnd);
			shapeBuilder.AddArc(new Vector2(0, 0), startAngle - endAngle, steps);
			shapeBuilder.AddLine(outerStart);
			shapeBuilder.Close();

			shape.vertices = shapeBuilder.GetVertices();
		}

		/* Calculates the triangles of the paddle shape and assigns them to the mesh.
		 This should be called only once after the vertices are first set. */
		private void SetTriangles()
		{
			shape.type = PrimitiveType.TriangleStrip;

			int n = shape.vertices.Length;
			int[] triangles = new int[n];

			for (int i = 0; i < n / 2; i++)
			{
				triangles[2 * i] = i;
				triangles[2 * i + 1] = (n - 1) - i;
			}

			shape.triangles = triangles;
		}
	}
}
