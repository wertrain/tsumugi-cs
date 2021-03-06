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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TsumugiEditor.Views.Panes
{
    /// <summary>
    /// PreviewPane.xaml の相互作用ロジック
    /// </summary>
    public partial class PreviewPane : UserControl
    {
        /// <summary>
        /// 
        /// </summary>
        public bool Running { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public TsumugiRenderer.RenderablePanel RenderablePanel { get { return _renderablePanel; } }

        /// <summary>
        /// 
        /// </summary>
        public PreviewPane()
        {
            InitializeComponent();

            StartPreview();
        }

        /// <summary>
        /// 
        /// </summary>
        private async void StartPreview()
        {
            Running = true;

            DateTime previousGameTime = DateTime.Now;

            while (Running)
            {
                TimeSpan gameTime = DateTime.Now - previousGameTime;
                previousGameTime = previousGameTime + gameTime;

                _renderablePanel.Update(gameTime.Milliseconds);
                _renderablePanel.Render();

                await Task.Delay(8);
            }

            _renderablePanel.Close();

            GC.Collect();
        }
    }
}
