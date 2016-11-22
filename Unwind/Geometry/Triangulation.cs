using OpenTK;
using System.Collections.Generic;

namespace Unwind
{
	/* Provides implementation for triangulating a shape defined
	 by an outer contour which may not be convex. */
	public static class Triangulation
	{
		/* Triangulate a polygon defined by sequential vertices.
		 Sets triangles as sets of three consecutive vertex indices.
		 Returns true if successful. */
		public static bool Process(List<Vector2> vertices, out List<int> triangles)
		{
			// Allocates and initialises vertices in polygon

			triangles = new List<int>();
			if (vertices.Count < 3) return false;

			int[] V = new int[vertices.Count];

			// Stores V to ensure it is counter-clockwise

			if (Area(vertices) > 0.0f)
			{
				for (int i = 0; i < vertices.Count; i++)
				{
					V[i] = i;
				}
			}

			else
			{
				for (int i = 0; i < vertices.Count; i++)
				{
					V[i] = (vertices.Count - 1) - i;
				}
			}

			// Removes n - 2 vertices, creating 1 triangle each time
			// Detects errors and returns false if found

			// Current number of vertices
			int n = vertices.Count;
			// Tracks current vertex
			int v = n - 1;
			// Error detection variable
			int count = 2 * n;

			while (n > 2)
			{
				// Indicates a loop and a probable bad polygon
				if (--count <= 0) return false;

				// Finds three consecutive vertices in polygon (u, v, w)
				int u = v; if (u >= n) u = 0;
				v = u + 1; if (v >= n) v = 0;
				int w = v + 1; if (w >= n) w = 0;

				if (Snip(vertices, u, v, w, n, V))
				{
					// Defines triangle
					triangles.Add(V[u]);
					triangles.Add(V[v]);
					triangles.Add(V[w]);

					// Remove v from remaining polygon

					int s = v;
					int t = v + 1;

					while (t < n)
					{
						V[s] = V[t];
						s++; t++;
					}

					n--;

					// Reset error detection counter
					count = 2 * n;
				}
			}

			return true;
		}

		/* Calculates area of polygon defined by vertices and returns value. */
		public static float Area(List<Vector2> vertices)
		{
			int n = vertices.Count;
			float area = 0.0f;

			for (int p = n - 1, q = 0; q < n; p = q++)
			{
				Vector2 vp = vertices[p], vq = vertices[q];
				area += vp.X * vq.Y - vq.X * vp.Y;
			}

			return area * 0.5f;
		}

		/* Returns true if point P lies within points A, B and C. */
		public static bool InsideTriangle(Vector2 A, Vector2 B, Vector2 C, Vector2 P)
		{
			// Vectors outline triangle ABC
			Vector2 va = C - B;
			Vector2 vb = A - C;
			Vector2 vc = B - A;

			// Vectors from points A, B and C to point P
			Vector2 pa = P - A;
			Vector2 pb = P - B;
			Vector2 pc = P - C;

			// Cross products of vector components emerging from the same point
			float ca = va.X * pb.Y - va.Y * pb.X;
			float cb = vb.X * pc.Y - vb.Y * pc.X;
			float cc = vc.X * pa.Y - vc.Y * pa.X;

			return ((ca >= 0.0f) && (cb >= 0.0f) && (cc >= 0.0f));
		}

		/* Returns true if vertices u, v, w should be snipped from the polygon
		 to form a new triangle. Vertex n is the current vertex being processed. */
		private static bool Snip(List<Vector2> vertices, int u, int v, int w, int n, int[] V)
		{
			Vector2 A, B, C;
			A = vertices[V[u]];
			B = vertices[V[v]];
			C = vertices[V[w]];

			if ((((B.X - A.X) * (C.Y - A.Y)) - ((B.Y - A.Y) * (C.X - A.X))) < Mathc.Epsilon) return false;

			for (int p = 0; p < n; p++)
			{
				if (p == u || p == v || p == w) continue;
				Vector2 P = vertices[V[p]];
				if (InsideTriangle(A, B, C, P)) return false;
			}

			return true;
		}
	}
}