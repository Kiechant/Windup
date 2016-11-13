using OpenTK;
using System;
using System.Collections.Generic;
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
			float cos = (float)Math.Cos(angle);
			float sin = (float)Math.Sin(angle);

			float x = point.X * cos - point.Y * sin;
			float y = point.X * sin + point.Y * cos;

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