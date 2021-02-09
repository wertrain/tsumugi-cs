using System;
using System.Windows.Forms;

namespace TsumugiRenderer
{
    /// <summary>
    /// 描画
    /// </summary>
    public class Renderable : UserControl
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Renderable()
        {
            Renderer = new Renderer();
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
            Invalidate();
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
        private Renderer Renderer { get; set; }
    }
}
