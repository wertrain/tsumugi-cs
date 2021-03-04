using System;
using System.Windows.Forms;
using TsumugiRenderer.Engine.Text;

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
        /// テキスト描画エンジン
        /// </summary>
        public TextEngine TextEngine;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public RenderablePanel()
        {
            RenderManager = new RenderManager(Handle, 800, 600);

            TextEngine = new TextEngine(RenderManager.Renderer, 800, 600);

            TextEngine.SetFont(new Engine.Text.Font()
            {
                Face = @"ラノベポップ",
                Size = 34,
                Bold = false,
                Color = unchecked((int)0xFF000000),
                ShadowColor = unchecked((int)0xFFff3333),
                Shadow = false,
                EdgeColor = unchecked((int)0xFFffff33),
                Edge = true,
            });

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);
        }

        /// <summary>
        /// 描画
        /// </summary>
        public void Render()
        {
            RenderManager.BeginRendering();
            RenderManager.Clear();
            TextEngine.Render();
            RenderManager.EndRendering();
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="delta"></param>
        public void Update(float delta)
        {
            TextEngine.Update(delta);
        }

        /// <summary>
        /// 再描画が発生した時のイベント
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            Render();
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
            TextEngine?.Dispose();
            TextEngine = null;
            RenderManager?.Dispose();
            RenderManager = null;
        }
    }
}
