using System;
using System.Windows.Forms;

namespace TsumugiRenderer
{
    public class RenderablePanel : UserControl
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public RenderablePanel()
        {
            Renderer = new D3D11Renderer();
            Renderer.Initialize(Handle, 800, 600);

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);
        }

        /// <summary>
        /// 再描画が発生した時のイベント
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            Renderer.BeginRendering();
            Renderer.EndRendering();
        }

        /// <summary>
        /// サイズ変更が発生した時のイベント
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSizeChanged(EventArgs e)
        {
            Renderer.Resize(ClientSize.Width, ClientSize.Height);
            Invalidate(); // 再描画
        }

        /// <summary>
        /// 破棄処理
        /// </summary>
        public void Close()
        {
            if (Renderer != null)
            {
                Renderer.Dispose();
                Renderer = null;
            }
        }

        /// <summary>
        /// レンダラー
        /// </summary>
        private D3D11Renderer Renderer { get; set; }
    }
}
