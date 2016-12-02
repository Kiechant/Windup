using System;
using System.IO;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Unwind
{
	public class ShaderProgram
	{
		const uint ShaderCount = 2;

		public Attributes attributes { get; private set; }
		public struct Attributes
		{
			public int position;
			public int colour;
			public int texcoord;
		}

		public Uniforms uniforms { get; private set; }
		public struct Uniforms
		{
			public int projectionMatrix;
			public int modelviewMatrix;
		}

		public string VSName { get; }
		public string FSName { get; }
		public int program { get; private set; }
		int[] shaders = new int[ShaderCount];

		public ShaderProgram(string vertexShader, string fragmentShader)
		{
			VSName = vertexShader;
			FSName = fragmentShader;
			SetupProgram();
			SetupAttribsUniforms();
		}

		protected virtual void SetupProgram()
		{
			program = GL.CreateProgram();
			shaders[0] = CreateShader(LoadShader(VSName), ShaderType.VertexShader);
			shaders[1] = CreateShader(LoadShader(FSName), ShaderType.FragmentShader);
			for (uint i = 0; i < ShaderCount; i++)
				GL.AttachShader(program, shaders[i]);
			
			GL.BindAttribLocation(program, 0, "position");
			GL.BindAttribLocation(program, 1, "colour");
			GL.BindAttribLocation(program, 2, "texcoord");

			GL.LinkProgram(program);
			CheckProgramError(program, GetProgramParameterName.LinkStatus, "ERROR: Program linkage failure: ");

			GL.ValidateProgram(program);
			CheckProgramError(program, GetProgramParameterName.ValidateStatus, "ERROR: Program validation failure: ");

			Debug.GetError();
		}

		protected virtual void SetupAttribsUniforms()
		{
			Attributes attributes;
			attributes.position = GL.GetAttribLocation(program, "position");
			attributes.colour = GL.GetAttribLocation(program, "colour");
			attributes.texcoord = GL.GetAttribLocation(program, "texcoord");
			this.attributes = attributes;

			Uniforms uniforms;
			uniforms.modelviewMatrix = GL.GetUniformLocation(program, "modelviewMatrix");
			uniforms.projectionMatrix = GL.GetUniformLocation(program, "projectionMatrix");
			this.uniforms = uniforms;

			Debug.GetError();
		}

		public void Bind()
		{
			GL.UseProgram(program);
			Debug.GetError();
		}

		public void Destroy()
		{
			foreach (int shader in shaders)
			{
				GL.DetachShader(program, shader);
				GL.DeleteShader(shader);
			}

			GL.DeleteProgram(program);
		}

		static int CreateShader(string text, ShaderType shaderType)
		{
			int shader = GL.CreateShader(shaderType);

			if (shader == 0) Console.WriteLine("ERROR: Shader creation failed.");

			GL.ShaderSource(shader, text);
			GL.CompileShader(shader);

			CheckShaderError(shader, ShaderParameter.CompileStatus, "ERROR: Shader compilation failure: ");

			return shader;
		}

		// TODO: Encapsulate to a file loader perhaps
		static string LoadShader(string fileName)
		{
			string output;
			using (StreamReader streamReader = new StreamReader(fileName, System.Text.Encoding.UTF8))
			{
				output = streamReader.ReadToEnd();
			}
			return output;
		}

		static void CheckProgramError(int program, GetProgramParameterName programParam, string errorMessage)
		{
			int success;

			GL.GetProgram(program, programParam, out success);

			if (success == 0)
			{
				string error = GL.GetProgramInfoLog(program);
				Console.WriteLine(errorMessage + "'" + error + "'");
			}
		}

		static void CheckShaderError(int shader, ShaderParameter shaderParam, string errorMessage)
		{
			int success;

			GL.GetShader(shader, shaderParam, out success);

			if (success == 0)
			{
				string error = GL.GetShaderInfoLog(shader);
				Console.WriteLine(errorMessage + "'" + error + "'");
			}
		}
	}
}
