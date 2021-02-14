using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TsumugiRenderer.Engine
{
    /// <summary>
    /// テキスト描画エンジン
    /// </summary>
    class TextEngine
    {
        /// <summary>
        /// 
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// 
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// 文字送りの期間（ミリ秒）
        /// </summary>
        public float CaptionSpeedTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public StringBuilder RenderText { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public TextEngine(Renderer renderer, int width, int height)
        {
            _renderer = renderer;
            Width = width;
            Height = height;
            CaptionSpeedTime = 1.0f;
            _marginLeft = 30;
            _marginTop = 30;
            RenderText = new StringBuilder();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="font"></param>
        public void SetFont(Font font)
        {
            _font = CreateFontSet(font);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="font"></param>
        public void SetDefaultFont(Font font)
        {
            _defaultFont = CreateFontSet(font);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="font"></param>
        /// <returns></returns>
        private FontSet CreateFontSet(Font font)
        {
            var fontSet = new FontSet();

            fontSet.TextFont = CreateTextFormat(font.Face, font.Bold, font.Size);
            fontSet.TextColor = new SolidColorBrush(_renderer.RenderTarget2D, Utility.ToRawColor4(font.Color));
            fontSet.ShadowTextColor = new SolidColorBrush(_renderer.RenderTarget2D, Utility.ToRawColor4(font.ShadowColor));
            fontSet.EdgeTextColor = new SolidColorBrush(_renderer.RenderTarget2D, Utility.ToRawColor4(font.EdgeColor));

            return fontSet;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="font"></param>
        /// <returns></returns>
        private TextFormat CreateTextFormat(string face, bool bold, int size)
        {
            return new TextFormat(_renderer.DirectWriteFactory, face, bold ? FontWeight.Bold : FontWeight.Normal, FontStyle.Normal, size)
            {
                // レイアウトに沿った文字の左右配置
                // ※読み方向軸に沿った段落テキストの相対的な配置を指定します
                TextAlignment = TextAlignment.Leading,
                // レイアウトに沿った文字の上下配置
                // ※相対フロー方向軸に沿った段落テキストの配置を指定します
                ParagraphAlignment = ParagraphAlignment.Near,
            };
        }

        /// <summary>
        /// 
        /// </summary>
        public void Render()
        {
            float shadowOffset = 2.5f;
            var allText = RenderText.ToString();

            var text = allText.Substring(0, _textPosition);
            _renderer.RenderTarget2D.DrawText(text, _font.TextFont,
                new SharpDX.Mathematics.Interop.RawRectangleF(_marginLeft + shadowOffset, _marginTop + shadowOffset, Width - _marginLeft + shadowOffset, Height - _marginTop + shadowOffset), _font.ShadowTextColor);
            _renderer.RenderTarget2D.DrawText(text, _font.TextFont, 
                new SharpDX.Mathematics.Interop.RawRectangleF(_marginLeft, _marginTop, Width - _marginLeft, Height - _marginTop), _font.TextColor);
        }

        public void Update(float delta)
        {
            _deltaTime = delta;

            if (_characterFeedTime > CaptionSpeedTime)
            {
                if (++_textPosition > RenderText.Length)
                {
                    _textPosition = RenderText.Length;
                }
                _characterFeedTime = 0;
            }
            else
            {
                _characterFeedTime += delta;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        public void AppendText(string text)
        {
            RenderText.Append(text);
        }

        /// <summary>
        /// 
        /// </summary>
        public void ClearText()
        {
            RenderText.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        private float _deltaTime;

        /// <summary>
        /// 
        /// </summary>
        private float _characterFeedTime;

        /// <summary>
        /// 
        /// </summary>
        private int _marginLeft;

        /// <summary>
        /// 
        /// </summary>
        private int _marginTop;

        /// <summary>
        /// 文字位置
        /// </summary>
        private int _textPosition;

        /// <summary>
        /// 
        /// </summary>
        private FontSet _defaultFont;

        /// <summary>
        /// 
        /// </summary>
        private FontSet _font;

        /// <summary>
        /// 
        /// </summary>
        private Renderer _renderer;
    }

    /// <summary>
    /// フォント
    /// </summary>
    public class Font
    {
        /// <summary>
        /// 文字サイズ
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// フォント名
        /// </summary>
        public string Face { get; set; }

        /// <summary>
        /// 文字色
        /// </summary>
        public int Color { get; set; }

        /// <summary>
        /// ルビのサイズ
        /// </summary>
        public int RubySize { get; set; }

        /// <summary>
        /// ルビの位置オフセット
        /// </summary>
        public int RubyOffset { get; set; }

        /// <summary>
        /// ルビのフォント名
        /// </summary>
        public string RubyFace { get; set; }

        /// <summary>
        /// 影をつけるか
        /// </summary>
        public bool Shadow { get; set; }

        /// <summary>
        /// 影の色
        /// </summary>
        public int ShadowColor { get; set; }

        /// <summary>
        /// 縁取りするか
        /// </summary>
        public bool Edge { get; set; }

        /// <summary>
        /// 縁取りの色
        /// </summary>
        public int EdgeColor { get; set; }

        /// <summary>
        /// 太字にするか
        /// </summary>
        public bool Bold { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    class FontSet
    {
        /// <summary>
        /// 
        /// </summary>
        public TextFormat TextFont { get; set; }

        /// <summary>
        ///
        /// </summary>
        public SolidColorBrush TextColor { get; set; }

        /// <summary>
        ///
        /// </summary>
        public SolidColorBrush ShadowTextColor { get; set; }

        /// <summary>
        ///
        /// </summary>
        public SolidColorBrush EdgeTextColor { get; set; }
    }
}
