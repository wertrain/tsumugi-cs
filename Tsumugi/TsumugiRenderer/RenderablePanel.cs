using System;
using System.Windows.Forms;

namespace TsumugiRenderer
{
    /// <summary>
    /// 描画
    /// </summary>
    public class RenderablePanel : UserControl
    {
        /// <summary>
        /// 描画管理
        /// </summary>
        public RenderManager RenderManager;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public RenderablePanel()
        {
            RenderManager = new RenderManager(Handle, 800, 600);

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);
        }

        /// <summary>
        /// 描画
        /// </summary>
        public void Render()
        {
            RenderManager.Render();
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="delta"></param>
        public void Update(float delta)
        {
            RenderManager.Update(delta);
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
    }
}
