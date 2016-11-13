using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Unwind
{
	/* Defines a polygon by a sequential set of vertices,
	 and triangles for mesh calculations. */
	public class Shape
	{
		public Vector2[] vertices;
		public int[] triangles;

		private int[] vbos = new int[0];
		private int vao;

		public Shape()
		{
			vertices = new Vector2[0];
			triangles = new int[0];
		}

		public Shape(Vector2[] vertices, int[] triangles)
		{
			this.vertices = vertices;
			this.triangles = triangles;
		}

		public void Update()
		{
			int n = triangles.Length;
			int count = 2 * n;
			float[] newTriangles = new float[count];
			vbos = new int[count];

			for (int i = 0; i < n; i++)
			{
				Vector2 vertex = vertices[triangles[i]];
				newTriangles[2 * i] = vertex.X;
				newTriangles[2 * i + 1] = vertex.Y;
			}

			GL.Begin(PrimitiveType.Triangles);

			GL.Vertex2(newTriangles[0], newTriangles[1]);
			GL.Vertex2(newTriangles[2], newTriangles[3]);
			GL.Vertex2(newTriangles[4], newTriangles[5]);

			if (newTriangles.Length > 6)
			{
				int i = 6;

				GL.Vertex2(newTriangles[i], newTriangles[i + 1]);
				GL.Vertex2(newTriangles[i + 2], newTriangles[i + 3]);
				GL.Vertex2(newTriangles[i + 4], newTriangles[i + 5]);

				i = 12;

				GL.Vertex2(newTriangles[i], newTriangles[i + 1]);
				GL.Vertex2(newTriangles[i + 2], newTriangles[i + 3]);
				GL.Vertex2(newTriangles[i + 4], newTriangles[i + 5]);
			}

			vao = GL.GenVertexArray();
			GL.BindVertexArray(vao);

			GL.GenBuffers(count, vbos);
			GL.BindBuffer(BufferTarget.ArrayBuffer, vbos[0]);
			GL.BufferData(BufferTarget.ArrayBuffer, count * sizeof(float), newTriangles, BufferUsageHint.StaticDraw);

			GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 0, 0);
			GL.EnableVertexAttribArray(0);

			GL.End();
		}

		public void Draw()
		{
			if (vbos.Length > 0)
			{
				GL.BindVertexArray(vao);

				GL.FrontFace(FrontFaceDirection.Cw);
				GL.DrawArrays(PrimitiveType.Triangles, vbos[0], 2 * triangles.Length);

				GL.FrontFace(FrontFaceDirection.Ccw);
				GL.DrawArrays(PrimitiveType.Triangles, vbos[0], 2 * triangles.Length);

				GL.BindVertexArray(0);
			}
		}
	}
}