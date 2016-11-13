using OpenTK;
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
			this.steps = steps;

			ProcessAppearance();
			SetTriangles();
		}

		public override void Update()
		{
			base.Update();

			radius -= Time.deltaTimeSeconds * fallRate;
			ProcessAppearance();

			// DEBUG
			Console.WriteLine(Time.deltaTimeSeconds);
			Console.WriteLine("Paddle " + ID + " updated. r: " + radius);
		}

		/* Updates shape appearance following a transformation. */
		private void ProcessAppearance()
		{
			float ringRadius = 0.1f;
			float drawRadius = radius;
			float drawThickness = Thickness;

			if (radius < ringRadius - Thickness)
			{
				// TODO: delete object
				return;
			}
			if (radius < ringRadius)
			{
				drawRadius = ringRadius;
				drawThickness = Thickness - (ringRadius - radius);
			}

			SetVertices(drawRadius, drawThickness);
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
			int n = shape.vertices.Length;
			int[] triangles = new int[(n - 2) * 3];

			for (int i = 0; i < n / 2 - 1; i++)
			{
				triangles[i * 6 + 0] = i;
				triangles[i * 6 + 1] = i + 1;
				triangles[i * 6 + 2] = n - i - 1;

				triangles[i * 6 + 3] = i + 1;
				triangles[i * 6 + 4] = n - i - 2;
				triangles[i * 6 + 5] = n - i - 1;
			}

			shape.triangles = triangles;
		}
	}
}
