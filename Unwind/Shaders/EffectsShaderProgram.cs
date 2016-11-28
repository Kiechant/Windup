using OpenTK.Graphics.OpenGL;

namespace Unwind
{
	// TODO: Encapsulate shader programs
	// This is basically a duplicate of ShaderProgram

	public class EffectsShaderProgram : ShaderProgram
	{
		public int uniformTexFrameBuffer;

		public EffectsShaderProgram(string fileName) : base(fileName) { }

		protected override void SetupAttribsUniforms()
		{
			base.SetupAttribsUniforms();

			uniformTexFrameBuffer = GL.GetUniformLocation(program, "texFramebuffer");
		}
	}
}
