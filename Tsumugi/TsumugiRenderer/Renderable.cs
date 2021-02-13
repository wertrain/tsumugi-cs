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
        /// 描画管理
        /// </summary>
        public RenderManager RenderManager;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Renderable()
        {
            Renderer = new Renderer();
            Renderer.Initialize(Handle, 800, 600);

            RenderManager = new RenderManager(Renderer);

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);
        }

        /// <summary>
        /// 再描画が発生した時のイベント
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            RenderManager.Render();
        }

        /// <summary>
        /// サイズ変更が発生した時のイベント
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSizeChanged(EventArgs e)
        {
            RenderManager.Resize(ClientSize.Width, ClientSize.Height);
            Invalidate();
        }

        /// <summary>
        /// 破棄処理
        /// </summary>
        public void Close()
        {
            RenderManager.Dispose();
        }

        /// <summary>
        /// レンダラー
        /// </summary>
        private Renderer Renderer { get; set; }
    }
}
