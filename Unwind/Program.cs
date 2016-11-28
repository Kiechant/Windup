// C# example program to demonstrate OpenTK
//
// Steps:
// 1. Create an empty C# console application project in Visual Studio
// 2. Place OpenTK.dll in the directory of the C# source file
// 3. Add System.Drawing and OpenTK as References to the project
// 4. Paste this source code into the C# source file
// 5. Run. You should see a colored triangle. Press ESC to quit.
//
// Copyright (c) 2013 Ashwin Nanjappa
// Released under the MIT License

using System;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

// TODO: Remove/move global error check function
static class Debug
{
	public static void GetError()
	{
		ErrorCode errorCode = GL.GetError();
		if (errorCode != ErrorCode.NoError)
			throw new System.Exception("OpenGL Error : " + errorCode);
	}
}

namespace Unwind
{
	public class Game : GameWindow
	{
		//public GameParameters parameters { get; private set; }
		Controller controller;

		public ShaderProgram basicShader { get; private set; }
		public EffectsShaderProgram effectsShader { get; private set; }

		public Game()
			: base(600, 600, GraphicsMode.Default, "Unwind")
		{
			VSync = VSyncMode.On;

			//IGraphicsContext context = new GraphicsContext(GraphicsMode.Default, WindowInfo, 2, 1, GraphicsContextFlags.ForwardCompatible);
			//context.MakeCurrent(WindowInfo);

			IGraphicsContext _context = GraphicsContext.CurrentContext;
			Console.WriteLine(_context.GraphicsMode);
			Console.WriteLine(_context.IsCurrent);
			Console.WriteLine(GL.GetString(StringName.Version));

			//parameters = new GameParameters();
			basicShader = new ShaderProgram("./res/BasicShader");
			effectsShader = new EffectsShaderProgram("./res/EffectsShader");

			basicShader.Bind();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			Time.Start();

			controller = new LevelController();
			controller.Start(this);

			SetupEvents();

			// Prepares modelview matrix
			Matrix4 modelview = Matrix4.LookAt(Vector3.Zero, Vector3.UnitZ, Vector3.UnitY);
			GL.MatrixMode(MatrixMode.Modelview);
			GL.LoadMatrix(ref modelview);
			GL.UniformMatrix4(basicShader.uniforms.modelviewMatrix, false, ref modelview);

			GL.ClearColor(1.0f, 0.886f, 0.2f, 0.0f);
			GL.Enable(EnableCap.DepthTest);
		}

		private void SetupEvents()
		{
			UpdateFrame += Time.OnUpdate;

			UpdateFrame += controller.OnUpdate;
			RenderFrame += controller.OnRender;
			Resize += controller.OnResize;
			MouseUp += controller.OnMouseUp;
			MouseDown += controller.OnMouseDown;

			UpdateFrame += EndUpdateFrame;
			RenderFrame += EndRenderFrame;
		}

		protected override void OnResize(EventArgs e)
		{
			GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);

			// Sets new projection matrix to avoid stretching.
			GL.MatrixMode(MatrixMode.Projection);
			GL.LoadIdentity();
			if (Width >= Height)
			{
				double aspect = Width / (double)Height;
				GL.Ortho(-aspect, aspect, -1, 1, -1, 1);
			}
			else
			{
				double aspect = Height / (double)Width;
				GL.Ortho(-1, 1, -aspect, aspect, -1, 1);
			}
			Matrix4 projection;
			GL.GetFloat(GetPName.ProjectionMatrix, out projection);
			GL.UniformMatrix4(basicShader.uniforms.projectionMatrix, false, ref projection);

			// Sets new projection matrix.
			//Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, Width / (float)Height, 1.0f, 64.0f);
			//GL.MatrixMode(MatrixMode.Projection);
			//GL.LoadMatrix(ref projection);
			//projection = Matrix4.Identity;
			//GL.UniformMatrix4(shader.projectionLocation, false, ref projection);
		}

		protected override void OnUpdateFrame(FrameEventArgs e)
		{
			base.OnUpdateFrame(e);
		}

		protected void EndUpdateFrame(object sender, FrameEventArgs e)
		{
			if (Keyboard[Key.Escape])
				Exit();
		}

		protected override void OnRenderFrame(FrameEventArgs e)
		{
			base.OnRenderFrame(e);

			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			// Attaches modelview matrix to uniform variables in shader
			Matrix4 modelview;
			GL.GetFloat(GetPName.ModelviewMatrix, out modelview);
			GL.UniformMatrix4(basicShader.uniforms.modelviewMatrix, false, ref modelview);

			Debug.GetError();
		}

		protected void EndRenderFrame(object sender, FrameEventArgs e)
		{
			SwapBuffers();
		}

		protected override void OnUnload(EventArgs e)
		{
			base.OnUnload(e);

			controller.Dispose();
			basicShader.Destroy();
			effectsShader.Destroy();
		}

		[STAThread]
		static void Main()
		{
			// The 'using' idiom guarantees proper resource cleanup.
			// We request 30 UpdateFrame events per second, and unlimited
			// RenderFrame events (as fast as the computer can handle).
			using (Game game = new Game())
			{
				game.Run(30.0);
			}
		}
	}
}