using System;
using System.IO;
using OpenTK.Graphics.OpenGL;

namespace Unwind
{
	public class Shader
	{
		/* Handle for OpenGL to reference shader. */
		const uint ShaderCount = 2;

		int program;
		int[] shaders = new int[ShaderCount];

		public Shader(string fileName)
		{
			program = GL.CreateProgram();
			shaders[0] = CreateShader(LoadShader(fileName + ".vs"), ShaderType.VertexShader);
			shaders[1] = CreateShader(LoadShader(fileName + ".fs"), ShaderType.FragmentShader);

			for (uint i = 0; i < ShaderCount; i++)
			{
				GL.AttachShader(program, shaders[i]);
			}

			GL.BindAttribLocation(program, 0, "position");

			GL.LinkProgram(program);
			CheckProgramError(program, GetProgramParameterName.LinkStatus, "ERROR: Program linkage failure: ");

			GL.ValidateProgram(program);
			CheckProgramError(program, GetProgramParameterName.ValidateStatus, "ERROR: Program validation failure: ");
		}

		public void Bind()
		{
			GL.UseProgram(program);
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
				Console.WriteLine(errorMessage + ": '" + error + "'");
			}
		}

		static void CheckShaderError(int shader, ShaderParameter shaderParam, string errorMessage)
		{
			int success;

			GL.GetShader(shader, shaderParam, out success);

			if (success == 0)
			{
				string error = GL.GetShaderInfoLog(shader);
				Console.WriteLine(errorMessage + ": '" + error + "'");
			}
		}
	}
}
