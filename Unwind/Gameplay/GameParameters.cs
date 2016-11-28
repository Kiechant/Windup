using System;

namespace Unwind
{
	public class GameParameters
	{
		public ShaderProgram basicShader { get; private set; }
		public EffectsShaderProgram effectsShader { get; private set; }

		public GameParameters()
		{
			basicShader = new ShaderProgram("./res/BasicShader");
			effectsShader = new EffectsShaderProgram("./res/EffectsShader");
		}
	}
}
