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
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using Unwind;

namespace StarterKit
{
    class Game : GameWindow
    {
        Updater updater;
		Controller controller;
		Shader shader;

        public Game()
			: base(800, 600, GraphicsMode.Default, "Unwind")
        {
            VSync = VSyncMode.On;

			//IGraphicsContext context = new GraphicsContext(GraphicsMode.Default, WindowInfo, 2, 1, GraphicsContextFlags.ForwardCompatible);
			//context.MakeCurrent(WindowInfo);

			IGraphicsContext _context = GraphicsContext.CurrentContext;
			Console.WriteLine(_context.GraphicsMode);
			Console.WriteLine(_context.IsCurrent);
			Console.WriteLine(GL.GetString(StringName.Version));

			shader = new Shader("./res/BasicShader");

			// Or move to OnUpdateFrame??
			shader.Bind();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

			Time.Start();

			controller = new LevelController();
			controller.Start();

			updater = new Updater();
			updater.Updated += Time.OnUpdate;
			updater.Updated += controller.OnUpdate;

            GL.ClearColor(0.1f, 0.2f, 0.5f, 0.0f);
            GL.Enable(EnableCap.DepthTest);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);

            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, Width / (float)Height, 1.0f, 64.0f);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref projection);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

			Console.WriteLine("\nGameWindow update frame");

			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			updater.OnUpdate();

			//GL.Begin(PrimitiveType.Triangles);

			//GL.Color3(1.0f, 1.0f, 0.0f); GL.Vertex3(-1.0f, -1.0f, 4.0f);
			//GL.Color3(1.0f, 0.0f, 0.0f); GL.Vertex3(1.0f, -1.0f, 4.0f);
			//GL.Color3(0.2f, 0.9f, 1.0f); GL.Vertex3(0.0f, 1.0f, 4.0f);

			//GL.End();

            if (Keyboard[Key.Escape])
                Exit();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            Matrix4 modelview = Matrix4.LookAt(Vector3.Zero, Vector3.UnitZ, Vector3.UnitY);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref modelview);

			//List<Vector2> vertices = new List<Vector2>();
			//vertices.AddRange(((LevelController)controller).GetVertices());
			//Vector2[] verticesArr = vertices.ToArray();

			//GL.Begin(PrimitiveType.Triangles);

			((LevelController)controller).OnRender(this, new EventArgs());

            SwapBuffers();
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