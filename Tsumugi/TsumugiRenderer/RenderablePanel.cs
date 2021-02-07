using System;
using System.Windows.Forms;

namespace TsumugiRenderer
{
    public class RenderablePanel : UserControl
    {
        public SharpDX.Direct3D11.Device Device { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public RenderablePanel()
        {
            Renderer = new D3D11Renderer();
            Renderer.Initialize(Handle, ClientSize.Width, ClientSize.Height);

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);
        }

        private D3D11Renderer Renderer { get; set; }


        protected override void OnPaint(PaintEventArgs e)
        {
            Renderer.BeginRendering();
            Renderer.EndRendering();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            Renderer.Resize(ClientSize.Width, ClientSize.Height);
            Invalidate(); // 再描画
        }

        public void Close()
        {
            if (Renderer != null)
            {
                Renderer.Dispose();
                Renderer = null;
            }
        }
    }
}
