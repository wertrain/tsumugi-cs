using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TsumugiRenderer.Engine;

namespace TsumugiRenderer
{
    /// <summary>
    /// 
    /// </summary>
    public class RenderManager
    {
        private class RenderLayer
        {
            public float X { get; set; }
            public float Y { get; set; }
            public Layer Layer { get; set; }
            public LayerParameters LayerParameters { get; set; }
        }

        enum LayerTypes
        {
        };

        private List<RenderLayer> Layers;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="renderer"></param>
        public RenderManager(IntPtr handle, int width, int height)
        {
            _renderer = new Renderer();
            _renderer.Initialize(handle, width, height);

            Layers = new List<RenderLayer>();
            var layer = new Layer(_renderer.RenderTarget2D);
            var layerParameters = new LayerParameters();
            layerParameters.ContentBounds = new SharpDX.Mathematics.Interop.RawRectangleF(0, 0, width, height);
            layerParameters.GeometricMask = new RectangleGeometry(_renderer.Direct2DFactory, new SharpDX.Mathematics.Interop.RawRectangleF(0, 0, width, height));
            layerParameters.MaskTransform = new SharpDX.Mathematics.Interop.RawMatrix3x2(1, 0.0f, 0, 1, 0, 0);
            layerParameters.OpacityBrush = new SolidColorBrush(_renderer.RenderTarget2D, Utility.ToRawColor4(System.Drawing.Color.White));
            layerParameters.Opacity = 1.0f;
            layerParameters.LayerOptions = LayerOptions.InitializeForCleartype;

            Layers.Add(new RenderLayer()
            {
                Layer = layer,
                LayerParameters = layerParameters
            });

            _textEngine = new TextEngine(_renderer, width, height);
            _textEngine.SetFont(new Engine.Font()
            {
                Face = "07ラノベPOP",
                Size = 24,
                Color = unchecked((int)0xFFFF0000),
                ShadowColor = unchecked((int)0xFF000000)
            });
            _textEngine.AppendText("メロスは激怒した。必ず、かの邪智暴虐の王を除かなければならぬと決意した。メロスには政治がわからぬ。メロスは、村の牧人である。笛を吹き、羊と遊んで暮して来た。けれども邪悪に対しては、人一倍に敏感であった。きょう未明メロスは村を出発し、野を越え山越え、十里はなれた此このシラクスの市にやって来た。メロスには父も、母も無い。女房も無い。十六の、内気な妹と二人暮しだ。この妹は、村の或る律気な一牧人を、近々、花婿として迎える事になっていた。結婚式も間近かなのである。");
        }

        /// <summary>
        /// 
        /// </summary>
        public void Render()
        {
            _renderer.BeginRendering();
            _renderer.Clear2D();
            _renderer.RenderTarget2D.Transform = new SharpDX.Mathematics.Interop.RawMatrix3x2(1, 0, 0, 1, 0, 0);
            _textEngine.Render();

            var param = Layers[0].LayerParameters;
            _renderer.RenderTarget2D.PushLayer(ref param, Layers[0].Layer);
            //_renderer.RenderTarget2D.Transform = new SharpDX.Mathematics.Interop.RawMatrix3x2(1, 0.3f, 0, 1, 0, 0);
            //_renderer.Clear2D();
            // 四角形描画：線のみ
            //var _colorBrush = new SolidColorBrush(_renderer.RenderTarget2D, Utility.ToRawColor4(unchecked((int)0xFF000000)));
            //_renderer.RenderTarget2D.DrawRectangle(new SharpDX.Mathematics.Interop.RawRectangleF(250.0f, 100.0f, 350.0f, 180.0f), _colorBrush);
            // 四角形描画：塗りつぶし
            //_renderer.RenderTarget2D.FillRectangle(new SharpDX.Mathematics.Interop.RawRectangleF(250.0f, 300.0f, 350.0f, 380.0f), _colorBrush);

            _renderer.RenderTarget2D.PopLayer();

            _renderer.EndRendering();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="delta"></param>
        public void Update(float delta)
        {
            _textEngine.Update(delta);
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

        private TextEngine _textEngine;
    }
}
