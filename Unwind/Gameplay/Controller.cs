using System;
using System.Drawing;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Unwind
{
	public class Controller : IDisposable
	{
		protected int frameBuffer;
		protected int textureBuffer;

		//TODO: remove variables
		protected int renderedTexture;
		protected int colourBuffer;
		protected int depthBuffer;

		protected bool mouseDown;

		Backdrop backdrop;

		public virtual void Start(Game game)
		{
			OnResize(game, new EventArgs());
			backdrop = new Backdrop(ZoneName.Dawn, RectangleF.FromLTRB(-1, 1, 1, -1));
			SetupFrameBuffer(game);
		}

		public virtual void OnUpdate(object source, EventArgs e)
		{
			backdrop.Update();
		}

		public virtual void OnRender(object source, EventArgs e)
		{
			var game = source as Game;
			var pixels = new byte[game.Width * game.Height * 4];

			game.effectsShader.Bind();
			Matrix4 modelview;
			GL.GetFloat(GetPName.ModelviewMatrix, out modelview);
			GL.UniformMatrix4(game.effectsShader.uniforms.modelviewMatrix, false, ref modelview);
			Matrix4 projection;
			GL.GetFloat(GetPName.ProjectionMatrix, out projection);
			GL.UniformMatrix4(game.effectsShader.uniforms.projectionMatrix, false, ref projection);

			// Bind and reset new frame buffer. Draw backdrop onto buffer.
			game.basicShader.Bind();
			GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBuffer);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			backdrop.Draw(game.effectsShader);

			// Retrieve texture from frame buffer.
			GL.ActiveTexture(TextureUnit.Texture0);
			GL.BindTexture(TextureTarget.Texture2D, textureBuffer);
			GL.ReadPixels(0, 0, game.Width, game.Height, PixelFormat.Rgba, PixelType.UnsignedByte, pixels);
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, game.Width, game.Height, 0,
						  PixelFormat.Rgba, PixelType.UnsignedByte, pixels);

			// Render backdrop.
			GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
			game.effectsShader.Bind();
			Shape back = ShapeBuilder.BuildRectangle(RectangleF.FromLTRB(-1, 1, 1, -1));
			back.colour = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
			back.zPosition = 0.8f;
			back.type = PrimitiveType.Quads;
			back.Update();
			back.Draw(game.effectsShader);
			back.Dispose();

			Debug.GetError();
		}

		private void Attempt1(int width, int height)
		{
			frameBuffer = GL.GenFramebuffer();
			GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBuffer);

			// Constructs texture buffer
			renderedTexture = GL.GenTexture();
			GL.BindTexture(TextureTarget.Texture2D, renderedTexture);
			//var texture = new int[width * height];
			int[] x = { 0 };
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, width, height, 0, PixelFormat.Rgb, PixelType.UnsignedByte, x);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);

			// Generates render buffer and attaches to frame buffer
			depthBuffer = GL.GenRenderbuffer();
			GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, depthBuffer);
			GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent, width, height);
			GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, depthBuffer);

			// Sets renderedTexture as colour attachment #0
			GL.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, renderedTexture, 0);

			// Sets list of draw buffers
			DrawBuffersEnum[] drawBuffers = { DrawBuffersEnum.ColorAttachment1 };
			GL.DrawBuffers(drawBuffers.Length, drawBuffers);
		}

		private void SetupFrameBuffer(Game game)
		{
			Matrix4 modelview;
			GL.GetFloat(GetPName.ModelviewMatrix, out modelview);
			GL.UniformMatrix4(game.effectsShader.uniforms.modelviewMatrix, false, ref modelview);

			Matrix4 projection;
			GL.GetFloat(GetPName.ProjectionMatrix, out projection);
			GL.UniformMatrix4(game.effectsShader.uniforms.projectionMatrix, false, ref projection);

			// Geeerates and binds frame buffer.
			frameBuffer = GL.GenFramebuffer();
			GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBuffer);

			GL.MatrixMode(MatrixMode.Modelview);
			GL.LoadMatrix(ref modelview);
			GL.MatrixMode(MatrixMode.Projection);
			GL.LoadMatrix(ref projection);

			// Generates and attaches texture buffer for storing output of fragment shader.
			textureBuffer = GL.GenTexture();
			GL.BindTexture(TextureTarget.Texture2D, textureBuffer);
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, game.Width, game.Height, 0, PixelFormat.Rgba,
			              PixelType.UnsignedByte, new IntPtr());
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
			GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0,
									TextureTarget.Texture2D, textureBuffer, 0);

			// Generates and attaches render buffer for storing depth and (optional) stencil information from buffers.
			int rboDepth = GL.GenRenderbuffer();
			GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, rboDepth);
			GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent, game.Width, game.Height);
			GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment,
									   RenderbufferTarget.Renderbuffer, rboDepth);

			// Binds default frame buffer and checks for errors.
			GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
			FramebufferErrorCode status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
			if (status != FramebufferErrorCode.FramebufferComplete)
				throw new Exception("Frame buffer not complete!\n");
			Debug.GetError();
		}

		public virtual void OnResize(object source, EventArgs e)
		{
			
		}

		public virtual void OnMouseUp(object source, EventArgs e)
		{
			mouseDown = false;
		}

		public virtual void OnMouseDown(object source, EventArgs e)
		{
			mouseDown = true;
		}

		public virtual void Dispose()
		{
			backdrop.Dispose();

			GL.DeleteFramebuffer(frameBuffer);
			GL.DeleteTexture(textureBuffer);
			GL.DeleteTexture(renderedTexture);
			GL.DeleteRenderbuffer(colourBuffer);
			GL.DeleteRenderbuffer(depthBuffer);
		}
	}
}
