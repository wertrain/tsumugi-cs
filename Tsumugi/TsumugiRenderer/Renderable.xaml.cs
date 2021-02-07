using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TsumugiRenderer
{
    /// <summary>
    /// UserControl1.xaml の相互作用ロジック
    /// </summary>
    public partial class Renderable : UserControl
    {
        /// <summary>
        /// レンダラー
        /// </summary>
        private D3D11Renderer Renderer { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Renderable()
        {

        }

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="handle"></param>
        public void Initialize(IntPtr handle)
        {
            Renderer = new D3D11Renderer();
            Renderer.Initialize(handle, (int)Width, (int)Height);
        }

        /// <summary>
        /// 破棄
        /// </summary>
        public void Dispose()
        {
            Renderer?.Dispose();
            Renderer = null;
        }

        /// <summary>
        /// リサイズが発生したときのイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Renderer?.Resize((int)e.NewSize.Width, (int)e.NewSize.Height);
        }
    }
}
