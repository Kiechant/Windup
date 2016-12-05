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

		protected EffectsShaderProgram blurShader;

		protected bool mouseDown;

		Backdrop backdrop;

		public virtual void Start(Game game)
		{
			blurShader = new EffectsShaderProgram("./res/TextureShader.vs", "./res/BlurShader.fs");
			backdrop = new Backdrop(ZoneName.Dawn, RectangleF.FromLTRB(-1, 1, 1, -1));
			SetupFrameBuffer(game);
		}

		public virtual void OnUpdate(object source, EventArgs e)
		{
			backdrop.Update();
		}

		private void UpdateBlurShader(Game game)
		{
			blurShader.Bind();

			Matrix4 modelview;
			GL.GetFloat(GetPName.ModelviewMatrix, out modelview);
			Matrix4 projection;
			GL.GetFloat(GetPName.ProjectionMatrix, out projection);
			float aspect = game.Width / (float)game.Height;

			GL.UniformMatrix4(blurShader.uniforms.modelviewMatrix, false, ref modelview);
			GL.UniformMatrix4(blurShader.uniforms.projectionMatrix, false, ref projection);
			GL.Uniform1(blurShader.uniformMipmapLevel, 0.0f);
			GL.Uniform1(blurShader.uniformAspect, aspect);

			game.basicShader.Bind();
		}

		public virtual void OnRender(object source, EventArgs e)
		{
			var game = source as Game;
			int channelsCount = 4;
			int imageSize = game.Width * game.Height;
			var backTexture = new byte[channelsCount * imageSize];

			// Draws background on non-visible interim frame buffer with basic shader.
			GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBuffer);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			backdrop.Draw(game.effectsShader);

			// Retrieves texture from frame buffer and attaches to target texture sampler.
			GL.ActiveTexture(TextureUnit.Texture0);
			GL.BindTexture(TextureTarget.Texture2D, textureBuffer);
			GL.ReadPixels(0, 0, game.Width, game.Height, PixelFormat.Rgba, PixelType.UnsignedByte, backTexture);
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, game.Width, game.Height, 0,
						  PixelFormat.Rgba, PixelType.UnsignedByte, backTexture);

			// Re-draws backdrop to visible default frame buffer.
			GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
			backdrop.Draw(game.effectsShader);

			Debug.GetError();
		}

		private void SetupFrameBuffer(Game game)
		{
			// Geeerates and binds frame buffer.
			frameBuffer = GL.GenFramebuffer();
			GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBuffer);

			// Setups texture buffer and binds to frame buffer.
			CreateTextureBuffer(game);
			GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0,
									TextureTarget.Texture2D, textureBuffer, 0);

			// Generates and attaches render buffer for storing depth information from buffers.
			int rboDepth = GL.GenRenderbuffer();
			GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, rboDepth);
			GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent, game.Width, game.Height);
			GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment,
									   RenderbufferTarget.Renderbuffer, rboDepth);

			// Checks for errors in frame buffer creation.
			FramebufferErrorCode status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
			if (status != FramebufferErrorCode.FramebufferComplete)
				throw new Exception("Frame buffer not complete!\n");
			Debug.GetError();

			// Binds default frame buffer and shader.
			GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
			game.basicShader.Bind();
		}

		private void CreateTextureBuffer(Game game)
		{
			textureBuffer = GL.GenTexture();
			GL.BindTexture(TextureTarget.Texture2D, textureBuffer);

			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, game.Width, game.Height, 0,
						  PixelFormat.Rgba, PixelType.UnsignedByte, new IntPtr());

			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapNearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBaseLevel, 0);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel, 0);
		}

		public virtual void OnResize(object source, EventArgs e)
		{
			mouseDown = false;

			var game = source as Game;
			double aspect = game.Width / (double)game.Height;
			RectangleF bounds;

			if (game.Width >= game.Height)
				bounds = RectangleF.FromLTRB((float)-aspect, 1, (float)aspect, -1);
			else
				bounds = RectangleF.FromLTRB(-1, (float)(1 / aspect), 1, (float)(-1 / aspect));

			backdrop.Resize(bounds);

			GL.DeleteFramebuffer(frameBuffer);
			GL.DeleteTexture(textureBuffer);
			SetupFrameBuffer(game);

			UpdateBlurShader(game);
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
		}
	}
}
