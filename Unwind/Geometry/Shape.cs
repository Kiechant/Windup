using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;

namespace Unwind
{
	/* Defines a polygon by a sequential set of vertices,
	 and triangles for mesh calculations. */
	public class Shape : IDisposable
	{
		public Vector2[] vertices;
		public int[] triangles;
		public PrimitiveType type;
		public Vector4 colour;

		public int positionBuffer { get; private set; }
		private int positionAttrib;
		private int indexBuffer;
		private int indexAttrib;
		private static int nextAttribPtrHandle = 0;

		public Shape()
		{
			vertices = new Vector2[0];
			triangles = new int[0];
			Setup();
		}

		public Shape(Vector2[] vertices, int[] triangles)
		{
			this.vertices = vertices;
			this.triangles = triangles;
			Setup();
		}

		private void Setup()
		{
			positionBuffer = GL.GenBuffer();
			Update();
		}

		public void Update()
		{
			// Generates triangles as a sequence of floats and assigns them to VBO

			int n = triangles.Length;
			int count = 2 * n;
			float[] newTriangles = new float[count];

			for (int i = 0; i < n; i++)
			{
				Vector2 vertex = vertices[triangles[i]];
				newTriangles[2 * i] = vertex.X;
				newTriangles[2 * i + 1] = vertex.Y;
			}

			GL.BindBuffer(BufferTarget.ArrayBuffer, positionBuffer);
			GL.BufferData(BufferTarget.ArrayBuffer, count * sizeof(float), newTriangles, BufferUsageHint.DynamicDraw);
		}

		public void Draw()
		{
			GL.BindBuffer(BufferTarget.ArrayBuffer, positionBuffer);

			GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 0, 0);
			GL.EnableVertexAttribArray(0);

			GL.DrawArrays(type, 0, triangles.Length);

			GL.DisableVertexAttribArray(0);
		}

		public void Dispose()
		{
			GL.DeleteBuffer(positionBuffer);
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