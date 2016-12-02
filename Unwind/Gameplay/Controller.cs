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
			OnResize(game, new EventArgs());
		}

		public virtual void OnUpdate(object source, EventArgs e)
		{
			backdrop.Update();
		}

		public virtual void OnRender(object source, EventArgs e)
		{
			var game = source as Game;
			int channelsCount = 4;
			int imageSize = game.Width * game.Height;
			var backTexture = new byte[imageSize * channelsCount];

			// Sets modelview and projection matrices of effects shader to current matrices.

			game.effectsShader.Bind();

			Matrix4 modelview = Matrix4.Identity;
			GL.GetFloat(GetPName.ModelviewMatrix, out modelview);
			Matrix4 projection;
			GL.GetFloat(GetPName.ProjectionMatrix, out projection);
			float aspect = game.Width / (float)game.Height;

			GL.UniformMatrix4(game.effectsShader.uniforms.modelviewMatrix, false, ref modelview);
			GL.UniformMatrix4(game.effectsShader.uniforms.projectionMatrix, false, ref projection);
			GL.Uniform1(game.effectsShader.uniformAspect, aspect);
			GL.Uniform1(game.effectsShader.uniformMipmapLevel, 0.0f);

			// Binds and resets new frame buffer. Draws backdrop onto buffer.

			game.basicShader.Bind();

			modelview.Column0 = new Vector4(-modelview.Column0.X, modelview.Column0.Y, modelview.Column0.Z, modelview.Column0.W);
			GL.UniformMatrix4(game.basicShader.uniforms.modelviewMatrix, false, ref modelview);

			GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBuffer);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			FramebufferErrorCode status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
			if (status != FramebufferErrorCode.FramebufferComplete)
				throw new Exception("Frame buffer not complete!\n");
			
			backdrop.Draw(game.effectsShader);

			modelview.Column0 = new Vector4(-modelview.Column0.X, modelview.Column0.Y, modelview.Column0.Z, modelview.Column0.W);
			GL.UniformMatrix4(game.basicShader.uniforms.modelviewMatrix, false, ref modelview);

			// Retrieves texture from frame buffer and attaches to target texture sampler.

			GL.ActiveTexture(TextureUnit.Texture0);
			GL.BindTexture(TextureTarget.Texture2D, textureBuffer);
			GL.ReadPixels(0, 0, game.Width, game.Height, PixelFormat.Rgba, PixelType.UnsignedByte, backTexture);
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, game.Width, game.Height, 0,
						  PixelFormat.Rgba, PixelType.UnsignedByte, backTexture);

			// Draws backdrop to default frame bluffer.

			GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
			backdrop.Draw(game.effectsShader);

			Debug.GetError();
		}

		private void SetupFrameBuffer(Game game)
		{
			game.effectsShader.Bind();

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

			textureBuffer = GL.GenTexture();
			GL.BindTexture(TextureTarget.Texture2D, textureBuffer);

			// Generates mipmaps.
			int w = game.Width, h = game.Height;
			int maxLevel = 0;
			//int maxLevel = 3;
			//for (int i = 0; i <= maxLevel; i++, w /= 2, h /= 2)
			//{
			//	GL.TexImage2D(TextureTarget.Texture2D, i, PixelInternalFormat.Rgba, w, h, 0, PixelFormat.Rgba,
			//				  PixelType.UnsignedByte, new IntPtr());
			//}
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, game.Width, game.Height, 0, PixelFormat.Rgba,
							  PixelType.UnsignedByte, new IntPtr());

			// Sets texture parameters.
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapNearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBaseLevel, 0);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel, maxLevel);

			GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0,
									TextureTarget.Texture2D, textureBuffer, 0);

			// Generates and attaches render buffer for storing depth and (optional) stencil information from buffers.
			int rboDepth = GL.GenRenderbuffer();
			GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, rboDepth);
			GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent, game.Width, game.Height);
			GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment,
									   RenderbufferTarget.Renderbuffer, rboDepth);

			// Checks for errors and binds default frame buffer.
			FramebufferErrorCode status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
			if (status != FramebufferErrorCode.FramebufferComplete)
				throw new Exception("Frame buffer not complete!\n");
			Debug.GetError();
			GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
			game.basicShader.Bind();
		}

		public virtual void OnResize(object source, EventArgs e)
		{
			var game = source as Game;
			int w = game.Width, h = game.Height;
			float aspect;
			RectangleF bounds;

			if (w >= h)
			{
				aspect = w / (float)h;
				bounds = RectangleF.FromLTRB(-aspect, 1, aspect, -1);
			}
			else
			{
				aspect = h / (float)w;
				bounds = RectangleF.FromLTRB(-1, aspect, 1, -aspect);
			}

			backdrop.Resize(bounds);
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
