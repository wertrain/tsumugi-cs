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

namespace TsumugiEditor
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            using (var stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("TsumugiEditor.Resources.Tsumugi.xshd"))
            {
                using (var reader = new System.Xml.XmlTextReader(stream))
                {
                    _textEditor.SyntaxHighlighting = ICSharpCode.AvalonEdit.Highlighting.Xshd.HighlightingLoader.Load(
                        reader, ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance);
                }
            }

            _textEditor.Options.ShowSpaces = true;
            _textEditor.Options.ShowTabs = true;
        }

        /// <summary>
        /// メニューアイテム「テーマ」の子アイテムがクリックされた時のイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemTheme_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            foreach (MenuItem sibling in (menuItem.Parent as MenuItem).Items)
            {
                sibling.IsChecked = menuItem == sibling;
            }
            _dockingManager.Theme = menuItem.Tag as Xceed.Wpf.AvalonDock.Themes.Theme;
        }
    }
}
