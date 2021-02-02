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
using TsumugiEditor.Localize;

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
        private void MenuItemToolTheme_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            foreach (MenuItem sibling in (menuItem.Parent as MenuItem).Items)
            {
                sibling.IsChecked = menuItem == sibling;
            }
            _dockingManager.Theme = menuItem.Tag as Xceed.Wpf.AvalonDock.Themes.Theme;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemExecutionRun_Click(object sender, RoutedEventArgs e)
        {
            var document = _documentPane.SelectedContent as Xceed.Wpf.AvalonDock.Layout.LayoutDocument;
            var editor = document.Content as ICSharpCode.AvalonEdit.TextEditor;

            var outputPane = _anchorablePane.Children.ToList().Find(child => child.ContentId == "Output".Localize());
            if (outputPane != null)
            {
                var interpreter = new Tsumugi.Interpreter();
                interpreter.Executor = new CommandExecutor(outputPane.Content as TextBox);
                interpreter.Execute(editor.Text);
            }
        }
    }

    class CommandExecutor : Tsumugi.Text.Executing.ICommandExecutor
    {
        public CommandExecutor(TextBox textbox)
        {
            TextBox = textbox;
            TextBox.Clear();
        }

        public void PrintText(string text)
        {
            TextBox.Text += ($"{0}", text);
        }

        public void StartNewLine()
        {
            TextBox.Text += ($"{0}", System.Environment.NewLine);
        }

        public void WaitAnyKey()
        {
            //Console.ReadKey(true);
        }

        public void StartNewPage()
        {
            TextBox.Clear();
        }

        public void WaitTime(int millisec)
        {
            Task.Delay(millisec).Wait();
        }

        public void DebugPring(Tsumugi.Text.Commanding.CommandQueue queue)
        {
            queue.Each((int index, Tsumugi.Text.Commanding.CommandBase command) => { Console.WriteLine("[{0}] {1}", index, command.GetType().ToString()); });
        }

        private TextBox TextBox { get; set; }
    }
}
