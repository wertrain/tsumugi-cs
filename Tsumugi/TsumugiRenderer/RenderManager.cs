﻿using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TsumugiRenderer
{
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
        public TextFormat ShadowTextFont { get; set; }

        /// <summary>
        ///
        /// </summary>
        public SolidColorBrush ShadowTextColor { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public TextFormat EedgeTextFont { get; set; }

        /// <summary>
        ///
        /// </summary>
        public SolidColorBrush EdgeTextColor { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class RenderManager
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="renderer"></param>
        public RenderManager(Renderer renderer)
        {
            _renderer = renderer;
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
            fontSet.ShadowTextFont = CreateTextFormat(font.Face, font.Bold, font.Size);
            fontSet.EedgeTextFont = CreateTextFormat(font.Face, font.Bold, font.Size);
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
            _renderer.BeginRendering();

            // 文字描画位置
            var pos = new SharpDX.Mathematics.Interop.RawVector2(30.0f, 10.0f);
            // 文字を描画する領域
            // ※「改行の目安」や「文字のAlignment」などで使用される
            float maxWidth = 1000.0f;
            float maxHeight = 1000.0f;

            // 文字描画
            _renderer.RenderTarget2D.DrawText("あなたが", _textFont, new SharpDX.Mathematics.Interop.RawRectangleF(pos.X, pos.Y, pos.X + maxWidth, pos.Y + maxHeight), _colorBrush);

            _renderer.EndRendering();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void Resize(int width, int height)
        {
            _renderer.Resize(width, height);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            if (_renderer != null)
            {
                _renderer.Dispose();
                _renderer = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private Renderer _renderer;

        /// <summary>
        /// 行
        /// </summary>
        private int _textLine;

        /// <summary>
        /// 列
        /// </summary>
        private int _textColumn;

        /// <summary>
        /// 
        /// </summary>
        private FontSet _defaultFont;

        /// <summary>
        /// 
        /// </summary>
        private FontSet _font;
    }
}