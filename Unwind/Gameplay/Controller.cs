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
			int channelsCount = 4;
			int imageSize = game.Width * game.Height;
			//int width = game.Width;
			//int height = game.Height;
			var backTexture = new byte[imageSize * channelsCount];
			var blurTexture = new byte[imageSize * channelsCount];

			// Sets modelview and projection matrices of effects shader to current matrices.
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
			FramebufferErrorCode status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
			if (status != FramebufferErrorCode.FramebufferComplete)
				throw new Exception("Frame buffer not complete!\n");
			backdrop.Draw(game.effectsShader);

			// Retrieves texture from frame buffer.
			GL.ActiveTexture(TextureUnit.Texture0);
			GL.BindTexture(TextureTarget.Texture2D, textureBuffer);
			GL.ReadPixels(0, 0, game.Width, game.Height, PixelFormat.Rgba, PixelType.UnsignedByte, backTexture);

			byte[] mipLvl1, mipLvl2, mipLvl3;
			Mathc.GenerateMipmapTexture(backTexture, 1, game.Width, game.Height, out mipLvl1);
			Mathc.GenerateMipmapTexture(mipLvl1, 1, game.Width / 2, game.Height / 2, out mipLvl2);
			// Proof of concept recursion from backTexture all the way to mipLvl3
			Mathc.GenerateMipmapTexture(backTexture, 3, game.Width, game.Height, out mipLvl3);

			//GL.TexImage2D(TextureTarget.Texture2D, 2, PixelInternalFormat.Rgba, game.Width / 16, game.Height / 16, 0,
			//			  PixelFormat.Rgba, PixelType.UnsignedByte, backTexture);
			//GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, game.Width, game.Height, 0,
						  PixelFormat.Rgba, PixelType.UnsignedByte, backTexture);
			GL.TexImage2D(TextureTarget.Texture2D, 1, PixelInternalFormat.Rgba, game.Width / 2, game.Height / 2, 0,
						  PixelFormat.Rgba, PixelType.UnsignedByte, mipLvl1);
			GL.TexImage2D(TextureTarget.Texture2D, 2, PixelInternalFormat.Rgba, game.Width / 4, game.Height / 4, 0,
						  PixelFormat.Rgba, PixelType.UnsignedByte, mipLvl2);
			GL.TexImage2D(TextureTarget.Texture2D, 3, PixelInternalFormat.Rgba, game.Width / 8, game.Height / 8, 0,
						  PixelFormat.Rgba, PixelType.UnsignedByte, mipLvl3);
			
			// Renders backdrop using effects shader.
			GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
			game.effectsShader.Bind();
			Shape back = ShapeBuilder.BuildRectangle(RectangleF.FromLTRB(-1, 1, 1, -1));
			back.colour = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
			back.zPosition = 0.8f;
			back.type = PrimitiveType.Quads;
			back.Update();
			back.Draw(game.effectsShader);
			back.Dispose();

			// Blurs background texture using gaussian blur and attaches to texture.
			//var watch = new System.Diagnostics.Stopwatch();
			//watch.Start();
			//for (int i = 0; i < channelsCount - 1; i++)
			//{
			//	byte[] src = new byte[imageSize];
			//	byte[] dst = new byte[imageSize];

			//	for (int j = 0; j < imageSize; j++)
			//		src[j] = backTexture[4 * j + i];

			//	Blur.GaussianBlur(src, ref dst, game.Width, game.Height, 20);

			//	for (int j = 0; j < imageSize; j++)
			//		blurTexture[4 * j + i] = src[j];
			//}
			//watch.Stop();
			//Console.WriteLine(watch.ElapsedMilliseconds);
			//GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, game.Width, game.Height, 0,
			              //PixelFormat.Rgba, PixelType.UnsignedByte, blurTexture);

			Debug.GetError();
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
			//GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, game.Width, game.Height, 0, PixelFormat.Rgba,
			//              PixelType.UnsignedByte, new IntPtr());
			// Generates mipmaps.
			int w = game.Width, h = game.Height;
			int maxLevel = 3;
			for (int i = 0; i <= maxLevel; i++, w /= 2, h /= 2)
			{
				GL.TexImage2D(TextureTarget.Texture2D, i, PixelInternalFormat.Rgba, w, h, 0, PixelFormat.Rgba,
							  PixelType.UnsignedByte, new IntPtr());
			}
			
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.NearestMipmapNearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);
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
		}

		public virtual void OnResize(object source, EventArgs e)
		{
			// TODO: fill with something...
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
