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
using System.Drawing;

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
		public RectangleF GLBounds { get; private set; }

		// Canvases and constraints testing. TODO: Encapsulate/refactor
		public Canvas mainCanvas { get; private set; }
		public Canvas gameplayCanvas;
		Constrainer gameplayConstrainer;

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
			basicShader = new ShaderProgram("./res/ColourShader.vs", "./res/BasicShader.fs");
			effectsShader = new EffectsShaderProgram("./res/TextureShader.vs", "./res/EffectsShader.fs");

			basicShader.Bind();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			Time.Start();

			controller = new LevelController();
			controller.Start(this);

			SetCanvases();

			SetupEvents();

			// Prepares default modelview and projection matrices for basic and effects shaders.

			Matrix4 modelview = CreateModelviewMatrix();
			Matrix4 projection = UpdateProjectionMatrix();
			
			GL.UniformMatrix4(basicShader.uniforms.modelviewMatrix, false, ref modelview);
			GL.UniformMatrix4(basicShader.uniforms.projectionMatrix, false, ref projection);

			// Sets up effects shader.
			effectsShader.Bind();
			GL.UniformMatrix4(effectsShader.uniforms.modelviewMatrix, false, ref modelview);
			GL.UniformMatrix4(basicShader.uniforms.projectionMatrix, false, ref projection);
			GL.Uniform1(effectsShader.uniformMipmapLevel, 0.0f);
			basicShader.Bind();

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

			SetCanvases();
			gameplayConstrainer.OnResize(this, new EventArgs());

			base.OnResize(e);

			Matrix4 projection = UpdateProjectionMatrix();

			GL.UniformMatrix4(basicShader.uniforms.projectionMatrix, false, ref projection);

			effectsShader.Bind();
			GL.UniformMatrix4(effectsShader.uniforms.projectionMatrix, false, ref projection);
			GL.Uniform1(effectsShader.uniformAspect, Width / (float)Height);
			basicShader.Bind();
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
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

			base.OnRenderFrame(e);

			mainCanvas.Draw(this);
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

		private Matrix4 CreateModelviewMatrix()
		{
			GL.MatrixMode(MatrixMode.Modelview);
			Matrix4 modelview = Matrix4.LookAt(Vector3.Zero, -Vector3.UnitZ, Vector3.UnitY);
			GL.LoadMatrix(ref modelview);
			return modelview;
		}

		private Matrix4 UpdateProjectionMatrix()
		{
			GL.MatrixMode(MatrixMode.Projection);
			GL.LoadIdentity();

			if (Width >= Height)
			{
				double aspect = Width / (double)Height;
				GL.Ortho(-aspect, aspect, -1, 1, -10, 1000);
				GLBounds = RectangleF.FromLTRB((float)-aspect, 1, (float)aspect, -1);
				//GL.Frustum(-aspect, aspect, -1, 1, 10, 100);
			}
			else
			{
				double aspect = Height / (double)Width;
				GL.Ortho(-1, 1, -aspect, aspect, -10, 1000);
				GLBounds = RectangleF.FromLTRB(-1, (float)aspect, 1, (float)-aspect);
				//GL.Frustum(-1, 1, -aspect, aspect, 10, 100);
			}

			//Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, 1, 10, 100);
			//GL.LoadMatrix(ref projection);
			Matrix4 projection;
			GL.GetFloat(GetPName.ProjectionMatrix, out projection);
			return projection;
		}

		private void SetCanvases()
		{
			mainCanvas = new Canvas(ClientRectangle);
			gameplayCanvas = new Canvas(ClientRectangle);
			gameplayCanvas.Top += 100;

			gameplayConstrainer = new Constrainer(mainCanvas);
			var one = Constraint.CreateAbsolute(mainCanvas, Anchor.Left, 0);
			var two = Constraint.CreateAbsolute(mainCanvas, Anchor.Bottom, 0);
			gameplayConstrainer = new Constrainer(mainCanvas, one, two);
			gameplayConstrainer.canvasToConstrain = gameplayCanvas;
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