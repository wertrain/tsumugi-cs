using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tsumugi.Text.Executing;
using TsumugiRenderer.Engine.Text;

namespace TsumugiRenderer
{
    /// <summary>
    /// 描画パネルを使用するコマンド実行クラス
    /// </summary>
    public class RenderableExecutor : ICommandExecutor
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="renderablePanel"></param>
        public RenderableExecutor(RenderablePanel renderablePanel)
        {
            RenderablePanel = renderablePanel;
        }

        public void PrintText(string text)
        {
            TextEngine.ClearText();
            TextEngine.AppendText(text);
        }

        public void SetDefaultFont(Tsumugi.Text.Executing.Font font)
        {
            TextEngine.SetDefaultFont(new Engine.Text.Font()
            {
                Face = font.Face,
                Size = font.Size,
                Bold = font.Bold,
                Color = unchecked((int)font.Color),
                ShadowColor = unchecked((int)font.ShadowColor),
                Shadow = font.Shadow,
                EdgeColor = unchecked((int)font.EdgeColor),
                Edge = font.Edge,
                FontFilePath = font.FontFilePath
            });
        }

        public void SetFont(Tsumugi.Text.Executing.Font font)
        {
            TextEngine.SetFont(new Engine.Text.Font()
            {
                Face = font.Face,
                Size = font.Size,
                Bold = font.Bold,
                Color = unchecked((int)font.Color),
                ShadowColor = unchecked((int)font.ShadowColor),
                Shadow = font.Shadow,
                EdgeColor = unchecked((int)font.EdgeColor),
                Edge = font.Edge,
                FontFilePath = font.FontFilePath
            });
        }

        public void StartNewLine()
        {
            throw new NotImplementedException();
        }

        public void StartNewPage()
        {
            throw new NotImplementedException();
        }

        public void WaitAnyKey()
        {
            throw new NotImplementedException();
        }

        public void WaitTime(int millisec)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 描画対象パネル
        /// </summary>
        private RenderablePanel RenderablePanel { get; set; }

        /// <summary>
        /// 描画エンジンへのアクセサ
        /// </summary>
        private RenderManager RenderManager => RenderablePanel.RenderManager;

        /// <summary>
        /// テキスト描画エンジンへのアクセサ
        /// </summary>
        private TextEngine TextEngine => RenderablePanel.TextEngine;
    }
}
