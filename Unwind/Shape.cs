using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;

namespace Unwind
{
	/* Defines a polygon by a sequential set of vertices,
	 and triangles for mesh calculations. */
	public class Shape
	{
		public Vector2[] vertices;
		public int[] triangles;

		private int vbo;
		private int vao;
		private static int attribPtrIndex = 0;

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
			vao = GL.GenVertexArray();
			vbo = GL.GenBuffer();
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

			GL.BindVertexArray(vao);

			GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
			GL.BufferData(BufferTarget.ArrayBuffer, count * sizeof(float), newTriangles, BufferUsageHint.StaticDraw);

			GL.VertexAttribPointer(attribPtrIndex, 2, VertexAttribPointerType.Float, false, 0, 0);
			GL.EnableVertexAttribArray(attribPtrIndex);

			GL.BindVertexArray(0);
		}

		public void Draw()
		{
			//GL.BindVertexArray(vao);
			GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);

			int[] indices = new int[2 * triangles.Length];

			for (int i = 0; i < indices.Length; i++)
			{
				indices[i] = i;
			}

			GL.DrawElements(PrimitiveType.Triangles, 2 * triangles.Length, DrawElementsType.UnsignedInt, indices);
			//GL.DrawArrays(PrimitiveType.Triangles, 0, 2 * triangles.Length);

			GL.BindVertexArray(0);
		}
	}
}