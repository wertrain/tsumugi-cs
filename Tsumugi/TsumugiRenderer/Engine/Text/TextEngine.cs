using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TsumugiRenderer.Engine.Text
{
    /// <summary>
    /// テキスト描画エンジン
    /// </summary>
    public class TextEngine : IDisposable
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
            _marginTop = 20;
            RenderText = new StringBuilder();
            _fileFontLoaders = new List<ResourceFont.FileFontLoader>();
            _fontCollections = new List<FontCollection>();
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

            if (File.Exists(font.FontFilePath))
            {
                var loader = new ResourceFont.FileFontLoader(_renderer.DirectWriteFactory, font.FontFilePath);
                var collection = new FontCollection(_renderer.DirectWriteFactory, loader, loader.Key);

                _fileFontLoaders.Add(loader);
                _fontCollections.Add(collection);

                fontSet.TextFont = CreateTextFormat(font.Face, font.Bold, font.Size, collection);
            }
            else
            {
                fontSet.TextFont = CreateTextFormat(font.Face, font.Bold, font.Size);
            }

            fontSet.TextColor = new SolidColorBrush(_renderer.RenderTarget2D, Utility.ToRawColor4(font.Color));
            fontSet.ShadowTextColor = new SolidColorBrush(_renderer.RenderTarget2D, Utility.ToRawColor4(font.ShadowColor));
            fontSet.EdgeTextColor = new SolidColorBrush(_renderer.RenderTarget2D, Utility.ToRawColor4(font.EdgeColor));
            fontSet.Parameter = font;

            return fontSet;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="face"></param>
        /// <param name="bold"></param>
        /// <param name="size"></param>
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
        /// <param name="face"></param>
        /// <param name="bold"></param>
        /// <param name="size"></param>
        /// <param name="fontCollection"></param>
        /// <returns></returns>
        private TextFormat CreateTextFormat(string face, bool bold, int size, FontCollection fontCollection)
        {
            var format = new TextFormat(_renderer.DirectWriteFactory, face, fontCollection, bold ? FontWeight.Bold : FontWeight.Normal, FontStyle.Normal, FontStretch.Normal, size)
            {
                // レイアウトに沿った文字の左右配置
                // ※読み方向軸に沿った段落テキストの相対的な配置を指定します
                TextAlignment = TextAlignment.Leading,
                // レイアウトに沿った文字の上下配置
                // ※相対フロー方向軸に沿った段落テキストの配置を指定します
                ParagraphAlignment = ParagraphAlignment.Near,
            };
            return format;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Render()
        {
            var currentFont = _font;

            float shadowOffset = currentFont.Parameter.ShadowOffset;
            var allText = RenderText.ToString();

            var text = allText.Substring(0, _textPosition);

            if (currentFont.Parameter.Shadow)
            {
                _renderer.RenderTarget2D.DrawText(text, currentFont.TextFont,
                    new SharpDX.Mathematics.Interop.RawRectangleF(_marginLeft + shadowOffset, _marginTop + shadowOffset, Width - _marginLeft + shadowOffset, Height - _marginTop + shadowOffset), _font.ShadowTextColor);
            }

            if (currentFont.Parameter.Edge)
            {
                var offset = 1.5f;
                var left = new float[] { -offset, offset,    0.0f,   0.0f }; 
                var top = new float[]  {    0.0f,   0.0f, -offset, offset };

                for (int i = 0; i < left.Length; ++i)
                {
                    _renderer.RenderTarget2D.DrawText(text, currentFont.TextFont, new SharpDX.Mathematics.Interop.RawRectangleF(
                            _marginLeft + left[i], _marginTop + top[i],
                            Width - _marginLeft + left[i], Height - _marginTop + top[i]),
                            currentFont.EdgeTextColor);
                }

            }

            _renderer.RenderTarget2D.DrawText(text, _font.TextFont, 
                new SharpDX.Mathematics.Interop.RawRectangleF(_marginLeft, _marginTop, Width - _marginLeft, Height - _marginTop), _font.TextColor);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="delta"></param>
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
        public void Dispose()
        {
            _defaultFont?.Dispose();
            _defaultFont = null;
            _font?.Dispose();
            _font = null;

            foreach (var collection in _fontCollections)
            {
                collection.Dispose();
            }
            foreach (var loader in _fileFontLoaders)
            {
                loader.Dispose();
            }
            _fontCollections.Clear();
            _fileFontLoaders.Clear();
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
        private List<ResourceFont.FileFontLoader> _fileFontLoaders;

        /// <summary>
        /// 
        /// </summary>
        private List<FontCollection> _fontCollections;

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
        public float RubyOffset { get; set; }

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
        /// 影の位置オフセット
        /// </summary>
        public float ShadowOffset { get; set; }

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

        /// <summary>
        /// フォントをファイルから読み込む場合のファイル名
        /// </summary>
        public string FontFilePath { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Font()
        {
            ShadowOffset = 1.5f;
        }
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

        /// <summary>
        /// フォント情報
        /// </summary>
        public Font Parameter { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            TextFont?.Dispose();
            TextFont = null;
            TextColor?.Dispose();
            TextColor = null;
            ShadowTextColor?.Dispose();
            ShadowTextColor = null;
            EdgeTextColor?.Dispose();
            EdgeTextColor = null;
        }
    }
}
