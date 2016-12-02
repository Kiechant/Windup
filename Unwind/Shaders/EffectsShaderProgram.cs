using OpenTK.Graphics.OpenGL;

namespace Unwind
{
	// TODO: Encapsulate shader programs
	// This is basically a duplicate of ShaderProgram

	public class EffectsShaderProgram : ShaderProgram
	{
		public int uniformTexFrameBuffer;
		public int uniformMipmapLevel;
		public int uniformAspect;

		public EffectsShaderProgram(string vertexShader, string fragmentShader) :
		base(vertexShader, fragmentShader) { }

		protected override void SetupAttribsUniforms()
		{
			base.SetupAttribsUniforms();

			uniformTexFrameBuffer = GL.GetUniformLocation(program, "texFramebuffer");
			uniformMipmapLevel = GL.GetUniformLocation(program, "mipmapLevel");
			uniformAspect = GL.GetUniformLocation(program, "aspect");
		}
	}
}
