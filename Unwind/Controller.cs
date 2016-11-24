using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Unwind
{
	public class Controller : IDisposable
	{
		protected int frameBuffer;
		protected int renderedTexture;
		protected int depthBuffer;

		protected bool mouseDown;

		public virtual void Start(int width, int height)
		{
			//// Generates and bind frame buffer
			//frameBuffer = GL.GenFramebuffer();
			//GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBuffer);

			//// Constructs texture buffer
			//renderedTexture = GL.GenTexture();
			//GL.BindTexture(TextureTarget.Texture2D, renderedTexture);
			//var texture = new int[width * height];
			//GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, width, height, 0, PixelFormat.Rgb, PixelType.UnsignedByte, texture);
			//GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
			//GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);

			//// Generates render buffer and attaches to frame buffer
			//depthBuffer = GL.GenRenderbuffer();
			//GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, depthBuffer);
			//GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent, width, height);
			//GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, depthBuffer);

			//// Sets renderedTexture as colour attachment #0
			//GL.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, renderedTexture, 0);

			//// Sets list of draw buffers
			//DrawBuffersEnum[] drawBuffers = { DrawBuffersEnum.ColorAttachment0 };
			//GL.DrawBuffers(drawBuffers.Length, drawBuffers);

			//if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
			//	throw new Exception("Frame buffer not complete!\n" + System.Environment.StackTrace);
		}

		public virtual void OnUpdate(object source, EventArgs e) { }

		public virtual void OnRender(object source, EventArgs e) { }

		public virtual void OnMouseUp(object source, EventArgs e) { mouseDown = false; }

		public virtual void OnMouseDown(object source, EventArgs e) { mouseDown = true; }

		public virtual void Dispose()
		{
			GL.DeleteFramebuffer(frameBuffer);
			GL.DeleteTexture(renderedTexture);
			GL.DeleteRenderbuffer(depthBuffer);
		}
	}
}
