using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Unwind
{
	/* Tool to create a shape defined as a series of vertices
	 joined by straight lines. Shapes can be built as meshes. */
	public class ShapeBuilder
	{
		private List<Vector2> vertices = new List<Vector2>();
		public bool closed {get; private set;}

		/* Prepares shape by creating first point.
		 Does nothing if vertices have already been defined. */
		public void Open(Vector2 point)
		{
			if (vertices.Count == 0)
			{
				vertices.Add(point);
			}
		}

		/* Defines a straight line from the last added point to the passed point.
		 Adds the passed point to the list of vertices. */
		public void AddLine(Vector2 toPoint)
		{
			if (vertices.Count != 0)
			{
				vertices.Add(toPoint);
			}
		}

		/* Defines an arc starting from the last added point sweeping around
		 the z-axis at centre with the defined angular distance.
		 Adds a number of points at regular intevals along the arc defined by steps.
		 Steps must be at least 2. */
		public void AddArc(Vector2 centre, float angularDistance, uint steps)
		{
			if (vertices.Count != 0 && steps >= 2)
			{
				Vector2 start = vertices.Last() - centre;
				float delta = angularDistance / steps;
				float angle = 0.0f;

				for (int i = 1; Math.Abs(angle) < Math.Abs(angularDistance) - Mathc.Epsilon; i++)
				{
					angle = delta * i;
					Vector2 current = Rotate(start, angle);
					vertices.Add(current + centre);
				}
			}
		}

		/* Defines an elliptical curve with a ratio a over b and defined centre point.
		 The curve begins at the last added point and is drawn for the passed
		 angular distance. Adds a number of points at regular intevals along the arc
		 defined by steps. Steps must be at least 2. */
		public void AddEllipticalCurve(Vector2 centre, float aOverB, float angularDistance, uint steps)
		{
			if (vertices.Count != 0 && steps >= 2)
			{
				Vector2 start = vertices.Last() - centre;

				//float phi = Mathc.Angle(Vector2.UnitX, start);
				float phi = Mathc.Angle(start, Vector2.UnitX);
				double cosPhi = Math.Cos(phi);
				double sinPhi = Math.Sin(phi);

				float a = (float)(Mathc.Distance(start, Vector2.Zero));
				float b = a / aOverB;

				float delta = angularDistance / steps;
				float t = 0.0f;

				for (int i = 1; Math.Abs(t) < Math.Abs(angularDistance) - Mathc.Epsilon; i++)
				{
					t = delta * i;

					double cosT = Math.Cos(t);
					double sinT = Math.Sin(t);

					float x = (float)(a * cosT * cosPhi - b * sinT * sinPhi);
					float y = (float)(a * cosT * sinPhi + b * sinT * cosPhi);
					Vector2 current = new Vector2(x, y);

					vertices.Add(current + centre);
				}
			}
		}

		/* Removes the number of points defined by n. If n is greater than the size
		 of vertices, all points are removed and the function returns 0. It is recommended
		 to use this function for simplifying the creation of curves with appropriate endpoints. */
		public void Remove(int n)
		{
			while (n > 0 && vertices.Count >= n)
			{
				vertices.RemoveAt(vertices.Count - 1);
				n--;
			}
		}

		/* Sets the shape to be closed if there are at least three points.
		 The shape will no longer be mutable and can be built as a mesh. */
		public void Close()
		{
			if (vertices.Count >= 3)
			{
				if (vertices.Last() == vertices.First())
				{
					vertices.RemoveAt(vertices.Count - 1);
				}
				closed = true;
			}
		}

		/* Returns a 2D vector rotated around the z-axis at the origin by angle. */
		private static Vector2 Rotate(Vector2 point, float angle)
		{
			double cos = Math.Cos(angle);
			double sin = Math.Sin(angle);

			float x = (float)(point.X * cos - point.Y * sin);
			float y = (float)(point.X * sin + point.Y * cos);

			return new Vector2(x, y);
		}

		/* Transforms closed shape into a mesh and returns the result.
		 Triangles of mesh are calculated using the Triangulation methods. */
		public Shape Build()
		{
			Shape shape = new Shape();

			if (closed)
			{
				List<int> triangles;
				Triangulation.Process(vertices, out triangles);

				int n = vertices.Count;
				Vector2[] newVertices = new Vector2[n];
				for (int i = 0; i < vertices.Count; i++)
				{
					Vector2 vertex = vertices[i];
					newVertices[i] = new Vector2(vertex.X, vertex.Y);
				}
				int[] newTriangles = triangles.ToArray();

				shape.vertices = newVertices;
				shape.triangles = newTriangles;
			}

			return shape;
		}

		/* Creates a new 2D shape with four vertices corresponding to a rectangle
		 defined by bounds. */
		public static Shape BuildRectangle(RectangleF bounds)
		{
			Vector2 bottomLeft = new Vector2(bounds.Left, bounds.Bottom);
			Vector2 topLeft = new Vector2(bounds.Left, bounds.Top);
			Vector2 topRight = new Vector2(bounds.Right, bounds.Top);
			Vector2 bottomRight = new Vector2(bounds.Right, bounds.Bottom);
			Vector2[] vertices = { bottomLeft, topLeft, topRight, bottomRight };
			int[] triangles = { 0, 1, 2, 3 };
			return new Shape(vertices, triangles);
		}

		public Vector2[] GetVertices()
		{
			Vector2[] newVertices = new Vector2[vertices.Count];
			for (int i = 0; i < vertices.Count; i++)
			{
				Vector2 vertex = vertices[i];
				newVertices[i] = new Vector2(vertex.X, vertex.Y);
			}
			return newVertices;
		}

		public void Print()
		{
			foreach (Vector2 vertex in vertices)
			{
				Console.WriteLine(vertex);
			}
		}
	}
}