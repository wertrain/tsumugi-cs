using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TsumugiRenderer.Engine.Text;

namespace TsumugiRenderer
{
    /// <summary>
    /// 
    /// </summary>
    public class RenderManager
    {
        /// <summary>
        /// 描画レイヤー
        /// </summary>
        private class RenderLayer
        {
            public float X { get; set; }
            public float Y { get; set; }
            public Layer Layer { get; set; }
            public LayerParameters LayerParameters { get; set; }
        }

        /// <summary>
        /// 
        /// </summary>
        enum LayerTypes
        {
            LayerBackground,
            LayerBackgroundObject,
            LayerMain,
            LayerForegroundObject,
            LayerEffect,
            LayerForeground,
            LayerScreenEffect,
        };

        /// <summary>
        /// 
        /// </summary>
        enum LayerPrioritys
        {
            LayerLowest = LayerTypes.LayerBackground,
            LayerHighest = LayerTypes.LayerScreenEffect
        };

        /// <summary>
        /// 
        /// </summary>
        private List<RenderLayer> Layers;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="renderer"></param>
        public RenderManager(IntPtr handle, int width, int height)
        {
            Renderer = new Renderer();
            Renderer.Initialize(handle, width, height);
            Renderer.ClearColor = System.Drawing.Color.WhiteSmoke;

            Layers = new List<RenderLayer>();
            var layer = new Layer(Renderer.RenderTarget2D);
            var layerParameters = new LayerParameters();
            layerParameters.ContentBounds = new SharpDX.Mathematics.Interop.RawRectangleF(0, 0, width, height);
            layerParameters.GeometricMask = new RectangleGeometry(Renderer.Direct2DFactory, new SharpDX.Mathematics.Interop.RawRectangleF(0, 0, width, height));
            layerParameters.MaskTransform = new SharpDX.Mathematics.Interop.RawMatrix3x2(1, 0.0f, 0, 1, 0, 0);
            layerParameters.OpacityBrush = new SolidColorBrush(Renderer.RenderTarget2D, Utility.ToRawColor4(System.Drawing.Color.White));
            layerParameters.Opacity = 1.0f;
            layerParameters.LayerOptions = LayerOptions.InitializeForCleartype;
           
            Layers.Add(new RenderLayer()
            {
                Layer = layer,
                LayerParameters = layerParameters
            });
        }

        /// <summary>
        /// 
        /// </summary>
        public void BeginRendering() => Renderer.BeginRendering();

        /// <summary>
        /// 
        /// </summary>
        public void Clear() => Renderer.Clear2D();

        /// <summary>
        /// 
        /// </summary>
        public void EndRendering() => Renderer.EndRendering();

        /// <summary>
        /// 
        /// </summary>
        public void Render()
        {
            Renderer.BeginRendering();
            Renderer.Clear2D();
            Renderer.RenderTarget2D.Transform = new SharpDX.Mathematics.Interop.RawMatrix3x2(1, 0, 0, 1, 0, 0);
            //_textEngine.Render();

            //var param = Layers[0].LayerParameters;
            //_renderer.RenderTarget2D.PushLayer(ref param, Layers[0].Layer);
            //_renderer.RenderTarget2D.Transform = new SharpDX.Mathematics.Interop.RawMatrix3x2(1, 0.3f, 0, 1, 0, 0);
            //_renderer.Clear2D();
            // 四角形描画：線のみ
            //var _colorBrush = new SolidColorBrush(_renderer.RenderTarget2D, Utility.ToRawColor4(unchecked((int)0xFF000000)));
            //_renderer.RenderTarget2D.DrawRectangle(new SharpDX.Mathematics.Interop.RawRectangleF(250.0f, 100.0f, 350.0f, 180.0f), _colorBrush);
            // 四角形描画：塗りつぶし
            //_renderer.RenderTarget2D.FillRectangle(new SharpDX.Mathematics.Interop.RawRectangleF(250.0f, 300.0f, 350.0f, 380.0f), _colorBrush);

            // _renderer.RenderTarget2D.PopLayer();

            Renderer.EndRendering();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void Resize(int width, int height)
        {
            Renderer.Resize(width, height);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Renderer?.Dispose();
            Renderer = null;
        }

        /// <summary>
        /// 
        /// </summary>
        public Renderer Renderer { get; private set; }
    }
}
