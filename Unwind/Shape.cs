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

		private int positionBuffer;
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
			uint[] indices = new uint[triangles.Length];

			for (uint i = 0; i < indices.Length; i++)
			{
				indices[i] = i;
			}

			//indexBuffer = GL.GenBuffer();
			//GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);
			//GL.BufferData(BufferTarget.ElementArrayBuffer, triangles.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

			//indexAttrib = nextAttribPtrHandle++;
			//GL.VertexAttribPointer(indexAttrib, 1, VertexAttribPointerType.UnsignedInt, false, 0, 0);
			//GL.EnableVertexAttribArray(indexAttrib);

			positionBuffer = GL.GenBuffer();
			Update();
		}

		public void Update()
		{
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

			//GL.DrawElements(PrimitiveType.Triangles, triangles.Length, DrawElementsType.UnsignedInt, indexBuffer);
			GL.DrawArrays(PrimitiveType.Triangles, 0, 2 * triangles.Length);

			GL.DisableVertexAttribArray(0);
		}

		public void Dispose()
		{
			GL.DeleteBuffer(positionBuffer);
			//GL.DeleteBuffer(indexBuffer);

			Console.WriteLine("Disposing shape");
			return;
		}
	}
}