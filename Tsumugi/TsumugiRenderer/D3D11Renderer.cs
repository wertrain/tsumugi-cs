using SharpDX.Direct2D1;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DirectWrite;
using SharpDX.DXGI;
using System;

namespace TsumugiRenderer
{
    class D3D11Renderer : IDisposable
    {
        //--------------------------------------------------------------//
        //                         DirectX設定                          //
        //--------------------------------------------------------------//
        /// <summary>
        /// Direct3Dのデバイス
        /// </summary>
        public SharpDX.Direct3D11.Device Device { get { return _device; } }
        private SharpDX.Direct3D11.Device _device = null;

        /// <summary>
        /// スワップチェーン
        /// ※デバイスが描いた画像をウィンドウに表示する機能
        /// </summary>
        SwapChain _swapChain;
        Texture2D _backBuffer;



        #region Direct2D関連
        /// <summary>
        /// レンダーターゲット2D
        /// </summary>
        public RenderTarget RenderTarget2D { get { return _RenderTarget2D; } }
        private RenderTarget _RenderTarget2D;
        /// <summary>
        /// Direct2Dで描画用のファクトリーオブジェクト
        /// </summary>
        private SharpDX.Direct2D1.Factory _factory2D;

        /// <summary>
        /// DirectWriteで描画用のファクトリーオブジェクト
        /// </summary>
        private SharpDX.DirectWrite.Factory _factory;
        /// <summary>
        /// 描画ブラシ
        /// </summary>
        private SolidColorBrush _ColorBrush;
        /// <summary>
        /// Direct3D用フォント
        /// </summary>
        public TextFormat _TextFont;
        #endregion

        /// <summary>
        /// 表示対象ハンドル
        /// </summary>
        protected IntPtr DisplayHandle { get; set; }

        /// <summary>
        /// DirectXデバイスの初期化
        /// </summary>
        public void Initialize(IntPtr handle, int width, int height)
        {
            // 描画ハンドルの設定
            DisplayHandle = handle;

            // スワップチェーン設定
            var desc = new SwapChainDescription()
            {
                // バッファ数
                // ※ダブルバッファリングを行う場合は2を指定
                BufferCount = 1,
                // 描画情報
                ModeDescription = new ModeDescription(width, height, new Rational(60, 1), Format.R8G8B8A8_UNorm),
                // ウィンドウモードの有効・無効
                IsWindowed = true,
                // 描画対象ハンドル
                OutputHandle = DisplayHandle,
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

            // Windowsの不要なイベントを無効にする
            var factory = _swapChain.GetParent<SharpDX.DXGI.Factory>();
            factory.MakeWindowAssociation(DisplayHandle, WindowAssociationFlags.IgnoreAll);

            // バックバッファーを保持する
            _backBuffer = Texture2D.FromSwapChain<Texture2D>(_swapChain, 0);

            // 2D用の初期化を行う
            InitializeDirect2D();
        }

        #region DirectXデバイス基本初期設定
        /// <summary>
        /// Direct2D 関連の初期化
        /// </summary>
        public void InitializeDirect2D()
        {
            // Direct2Dリソースを作成
            _factory2D = new SharpDX.Direct2D1.Factory();
            using (var surface = _backBuffer.QueryInterface<Surface>())
            {
                _RenderTarget2D = new RenderTarget(_factory2D, surface, new RenderTargetProperties(new PixelFormat(Format.Unknown, SharpDX.Direct2D1.AlphaMode.Premultiplied)));
            }
            // 非テキストプリミティブのエッジのレンダリング方法を指定
            _RenderTarget2D.AntialiasMode = AntialiasMode.PerPrimitive;
            // テキストの描画に使用されるアンチエイリアスモードについて指定
            _RenderTarget2D.TextAntialiasMode = SharpDX.Direct2D1.TextAntialiasMode.Cleartype;


            // DirectWriteオブジェクトを生成するために必要なファクトリオブジェクトを生成
            _factory = new SharpDX.DirectWrite.Factory();

            // ブラシを生成
            _ColorBrush = new SolidColorBrush(_RenderTarget2D, new SharpDX.Mathematics.Interop.RawColor4(255.0f, 0, 0, 255.0f));

            // フォントを作成
            _TextFont = new TextFormat(_factory, "MS UI Gothic", 24.0f)
            {
                // レイアウトに沿った文字の左右配置
                // ※読み方向軸に沿った段落テキストの相対的な配置を指定します
                TextAlignment = TextAlignment.Leading,
                // レイアウトに沿った文字の上下配置
                // ※相対フロー方向軸に沿った段落テキストの配置を指定します
                ParagraphAlignment = ParagraphAlignment.Near,
            };
        }

        public void BeginRendering()
        {
            _RenderTarget2D?.BeginDraw();
            _RenderTarget2D?.Clear(new SharpDX.Mathematics.Interop.RawColor4(255.0f, 0, 0, 255.0f));
        }

        public void EndRendering()
        {
            _RenderTarget2D?.EndDraw();
        }

        public void Resize(int width, int height)
        {
            if (_swapChain != null)
            {
                var description = new ModeDescription()
                {
                    Format = Format.R8G8B8A8_UNorm,
                    RefreshRate = new Rational(60, 1),
                    Width = width,
                    Height = height,
                    Scaling = DisplayModeScaling.Stretched,
                    ScanlineOrdering = DisplayModeScanlineOrder.Progressive
                };
                _swapChain.ResizeTarget(ref description);
            }
        }

        public void Dispose()
        {
            if (_backBuffer != null)
            {
                _backBuffer.Dispose();
                _backBuffer = null;
            }

            if (_swapChain != null)
            {
                _swapChain.Dispose();
                _swapChain = null;
            }

            if (_factory != null)
            {
                _factory.Dispose();
                _factory = null;
            }

            if (_factory2D != null)
            {
                _factory2D.Dispose();
                _factory2D = null;
            }
        }
        #endregion
    }
}
