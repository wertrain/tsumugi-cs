using SharpDX.Direct2D1;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DirectWrite;
using SharpDX.DXGI;
using System;
using System.Drawing;

namespace TsumugiRenderer
{
    /// <summary>
    /// DirectX 11 を使用したレンダラー
    /// </summary>
    public class Renderer : IDisposable
    {
        /// <summary>
        /// デバイス
        /// </summary>
        public SharpDX.Direct3D11.Device Device { get { return _device; } }

        /// <summary>
        /// クリア色の設定
        /// </summary>
        /// <param name="color"></param>
        public Color ClearColor
        {
            get
            {
                return Utility.FromRawColor4(_clearColor);
            }
            set
            {
                _clearColor = Utility.ToRawColor4(value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public RenderTarget RenderTarget2D { get { return _renderTarget2D; } }

        /// <summary>
        /// 
        /// </summary>
        public SharpDX.DirectWrite.Factory DirectWriteFactory { get { return _directWriteFactory; } }

        /// <summary>
        /// 
        /// </summary>
        public TextFormat TextFormat { set { _textFont = value; } }

        /// <summary>
        /// 
        /// </summary>
        public SolidColorBrush TextColor { set { _colorBrush = value; } }

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void Initialize(IntPtr handle, int width, int height)
        {
            // スワップチェーン設定
            var desc = new SwapChainDescription()
            {
                // バッファ数（ダブルバッファリングを行う場合は 2 を指定）
                BufferCount = 1,
                // 描画情報
                ModeDescription = new ModeDescription(width, height, new Rational(60, 1), Format.R8G8B8A8_UNorm),
                // ウィンドウモードの有効・無効
                IsWindowed = true,
                // 描画対象ハンドル
                OutputHandle = handle,
                // マルチサンプル方法の指定
                SampleDescription = new SampleDescription(1, 0),
                // 描画後の表示バッファの扱い方法の指定
                SwapEffect = SwapEffect.Discard,
                // 描画画像の使用方法
                Usage = Usage.RenderTargetOutput
            };

            // デバイスとスワップチェーンを生成
            SharpDX.Direct3D11.Device.CreateWithSwapChain(
                // デバイスの種類
                DriverType.Hardware,
                // ランタイムレイヤーの有効にするリスト
                DeviceCreationFlags.BgraSupport,
                // フィーチャーレベル
                // ※ある程度のハードウェアのレベルを規定して，それぞれのレベルにあわせたプログラムを書ける仕組み
                // ※DirectX の世代を指定
                new[] { SharpDX.Direct3D.FeatureLevel.Level_11_0 },
                // スワップチェーン設定
                desc,
                // 生成した変数を返す
                out _device, out _swapChain);

            // Windows の不要なイベントを無効にする
            var factory = _swapChain.GetParent<SharpDX.DXGI.Factory>();
            factory.MakeWindowAssociation(handle, WindowAssociationFlags.IgnoreAll);

            // バックバッファを保持する
            _backBuffer = SharpDX.Direct3D11.Resource.FromSwapChain<Texture2D>(_swapChain, 0);

            // クリア色の初期設定
            _clearColor = Utility.ToRawColor4(Color.Blue);

            // 2D用の初期化を行う
            InitializeDirect2D();
        }

        /// <summary>
        /// 
        /// </summary>
        public void InitializeDirect2D()
        {
            // Direct2Dリソースを作成
            _direct2DFactory = new SharpDX.Direct2D1.Factory();
            using (var surface = _backBuffer.QueryInterface<Surface>())
            {
                _renderTarget2D = new RenderTarget(_direct2DFactory, surface, new RenderTargetProperties(new PixelFormat(Format.Unknown, SharpDX.Direct2D1.AlphaMode.Premultiplied)));
            }
            // 非テキストプリミティブのエッジのレンダリング方法を指定
            _renderTarget2D.AntialiasMode = AntialiasMode.PerPrimitive;
            // テキストの描画に使用されるアンチエイリアスモードについて指定
            _renderTarget2D.TextAntialiasMode = SharpDX.Direct2D1.TextAntialiasMode.Cleartype;


            // DirectWrite オブジェクトを生成するために必要なファクトリオブジェクトを生成
            _directWriteFactory = new SharpDX.DirectWrite.Factory();

            // ブラシを生成
            _colorBrush = new SolidColorBrush(_renderTarget2D, new SharpDX.Mathematics.Interop.RawColor4(255.0f, 0, 0, 255.0f));

            // フォントを作成
            _textFont = new TextFormat(_directWriteFactory, "MS UI Gothic", 24.0f)
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
        public void BeginRendering()
        {
            _renderTarget2D?.BeginDraw();
            _renderTarget2D?.Clear(_clearColor);

            // 線描画：線のみ
            _renderTarget2D.DrawLine(
                new SharpDX.Mathematics.Interop.RawVector2(0.0f, 0.0f), 
                new SharpDX.Mathematics.Interop.RawVector2(10.0f, 10.0f), _colorBrush);

            // 四角形描画：線のみ
            _renderTarget2D.DrawRectangle(new SharpDX.Mathematics.Interop.RawRectangleF(250.0f, 100.0f, 350.0f, 180.0f), _colorBrush);
            // 四角形描画：塗りつぶし
            _renderTarget2D.FillRectangle(new SharpDX.Mathematics.Interop.RawRectangleF(250.0f, 300.0f, 350.0f, 380.0f), _colorBrush);

            // 文字描画位置
            var pos = new SharpDX.Mathematics.Interop.RawVector2(30.0f, 10.0f);
            // 文字を描画する領域
            // ※「改行の目安」や「文字のAlignment」などで使用される
            float maxWidth = 1000.0f;
            float maxHeight = 1000.0f;

            // 文字描画
            _renderTarget2D.DrawText("あなたが", _textFont, new SharpDX.Mathematics.Interop.RawRectangleF(pos.X, pos.Y, pos.X + maxWidth, pos.Y + maxHeight), _colorBrush);

        }

        /// <summary>
        /// 
        /// </summary>
        public void EndRendering()
        {
            _renderTarget2D?.EndDraw();
            _swapChain.Present(0, PresentFlags.None);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void Resize(int width, int height)
        {
            if (_swapChain != null)
            {
                var description = new ModeDescription()
                {
                    Format = Format.R8G8B8A8_UNorm,
                    RefreshRate = new Rational(60, 1),
                    Width = width,
                    Height = height
                };
                _swapChain.ResizeTarget(ref description);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            var context =_device.ImmediateContext;
            context.ClearState();
            context.Flush();

            _textFont?.Dispose();
            _colorBrush.Dispose();
            _backBuffer.Dispose();
            _directWriteFactory.Dispose();
            _direct2DFactory.Dispose();
            _swapChain.Dispose();
            _renderTarget2D.Dispose();
            _device.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        private SharpDX.Direct3D11.Device _device;

        /// <summary>
        /// 
        /// </summary>
        private SwapChain _swapChain;

        /// <summary>
        /// 
        /// </summary>
        private Texture2D _backBuffer;

        /// <summary>
        /// 
        /// </summary>
        private SharpDX.Mathematics.Interop.RawColor4 _clearColor;

        /// <summary>
        /// 
        /// </summary>
        private RenderTarget _renderTarget2D;

        /// <summary>
        /// 
        /// </summary>
        private SharpDX.Direct2D1.Factory _direct2DFactory;

        /// <summary>
        /// 
        /// </summary>
        private SharpDX.DirectWrite.Factory _directWriteFactory;

        /// <summary>
        ///
        /// </summary>
        private SolidColorBrush _colorBrush;

        /// <summary>
        /// 
        /// </summary>
        public TextFormat _textFont;

        /// <summary>
        ///
        /// </summary>
        private SolidColorBrush _textColor;
    }
}
