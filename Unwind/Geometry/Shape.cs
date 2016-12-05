using System;

using OpenTK;
using OpenTK.Graphics.OpenGL;

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
		public float zPosition = 10.0f;

		BufferUsageHint _usageHint;
		public BufferUsageHint usageHint
		{
			get
			{ return _usageHint; }
			set
			{
				Update();
				usageHint = value;
			}
		}

		public int vertexBuffer { get; private set; }

		public Shape()
		{
			vertices = new Vector2[0];
			triangles = new int[0];
			_usageHint = BufferUsageHint.StaticDraw;
			Setup();
		}

		public Shape(BufferUsageHint usageHint) :
		this()
		{
			_usageHint = usageHint;
		}

		public Shape(Vector2[] vertices, int[] triangles)
		{
			this.vertices = vertices;
			this.triangles = triangles;
			_usageHint = BufferUsageHint.StaticDraw;
			Setup();
		}

		public Shape(Vector2[] vertices, int[] triangles, BufferUsageHint usageHint) :
		this(vertices, triangles)
		{
			_usageHint = usageHint;
		}

		/* Constructs a shpae from a series of sequential x and y coordinates. */
		public Shape(float[] vertices)
		{
			var newVertices = new Vector2[vertices.Length / 2];
			var newTriangles = new int[vertices.Length / 2];
			for (int i = 0; i < newVertices.Length; i++)
			{
				Vector2 vertex;
				vertex.X = vertices[2 * i];
				vertex.Y = vertices[2 * i + 1];
				newVertices[i] = vertex;
				newTriangles[i] = i;
			}
			this.vertices = newVertices;
			this.triangles = newTriangles;
			_usageHint = BufferUsageHint.StaticDraw;
			Setup();
		}

		public Shape(float[] vertices, BufferUsageHint usageHint) :
		this(vertices)
		{
			_usageHint = usageHint;
		}

		/* Function to be called by constructors only.
		 Generates vbo for shape and calls Update. */
		private void Setup()
		{
			vertexBuffer = GL.GenBuffer();
			Update();
		}

		public void Update()
		{
			// Generates vertex data with interleaved position and colour attributes

			int n = triangles.Length;
			int a = 10;
			int count = a * n;
			float[] vertexData = new float[count];

			for (int i = 0; i < n; i++)
			{
				// Position data
				Vector2 vertex = vertices[triangles[i]];
				vertexData[a * i] = vertex.X;
				vertexData[a * i + 1] = vertex.Y;
				vertexData[a * i + 2] = zPosition;
				vertexData[a * i + 3] = 1.0f;

				// Colour data
				vertexData[a * i + 4] = colour.X;
				vertexData[a * i + 5] = colour.Y;
				vertexData[a * i + 6] = colour.Z;
				vertexData[a * i + 7] = colour.W;

				// Tex coords
				vertexData[a * i + 8] = 0.5f * vertex.X + 0.5f;
				vertexData[a * i + 9] = 0.5f * vertex.Y + 0.5f;
			}

			GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
			GL.BufferData(BufferTarget.ArrayBuffer, count * sizeof(float), vertexData, _usageHint);
			Debug.GetError();
		}

		public void Draw(ShaderProgram shader)
		{
			Debug.GetError();
			GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);

			int stride = 10 * sizeof(float);

			GL.VertexAttribPointer(shader.attributes.position, 4, VertexAttribPointerType.Float, false, stride, 0);
			GL.EnableVertexAttribArray(shader.attributes.position);
			Debug.GetError();

			GL.VertexAttribPointer(shader.attributes.colour, 4, VertexAttribPointerType.Float, false, stride, 4 * sizeof(float));
			GL.EnableVertexAttribArray(shader.attributes.colour);
			Debug.GetError();

			GL.VertexAttribPointer(shader.attributes.texcoord, 2, VertexAttribPointerType.Float, false, stride, 8 * sizeof(float));
			GL.EnableVertexAttribArray(shader.attributes.texcoord);
			Debug.GetError();

			GL.DrawArrays(type, 0, triangles.Length);

			Debug.GetError();
			GL.DisableVertexAttribArray(shader.attributes.position);
			GL.DisableVertexAttribArray(shader.attributes.colour);
			GL.DisableVertexAttribArray(shader.attributes.texcoord);
		}

		public void Dispose()
		{
			GL.DeleteBuffer(vertexBuffer);
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