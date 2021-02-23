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
using Tsumugi.Text.Executing;
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
        }

        /// <summary>
        /// ウィンドウロードイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            AddNewScriptLayout();

            _layoutAnchorablePreview.FloatingLeft = Left + Width;
            _layoutAnchorablePreview.FloatingTop = Top;
            _layoutAnchorablePreview.Float();
        }

        /// <summary>
        /// ウィンドウクローズイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closed(object sender, EventArgs e)
        {
            _previewPane.Running = false;
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
        /// メニューアイテム「ウィンドウ」以下の子がクリックされた時のイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemWindow_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuItem;
            menuItem.IsChecked = !menuItem.IsChecked;

            var layout = menuItem.Tag as Xceed.Wpf.AvalonDock.Layout.LayoutAnchorable;
            layout.IsVisible = menuItem.IsChecked;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemExecutionRun_Click(object sender, RoutedEventArgs e)
        {
            RunScript();
        }

        /// <summary>
        /// スクリプトの実行
        /// </summary>
        private void RunScript()
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

        /// <summary>
        /// スクリプト用のレイアウトを追加
        /// </summary>
        private void AddNewScriptLayout()
        {
            var textEditor = new ICSharpCode.AvalonEdit.TextEditor();
            textEditor.FontFamily = new FontFamily("Consolas");
            textEditor.FontSize = 14.0;
            textEditor.ShowLineNumbers = true;
            textEditor.Options.ShowSpaces = true;
            textEditor.Options.ShowTabs = true;

            using (var stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("TsumugiEditor.Resources.Tsumugi.xshd"))
            {
                using (var reader = new System.Xml.XmlTextReader(stream))
                {
                    textEditor.SyntaxHighlighting = ICSharpCode.AvalonEdit.Highlighting.Xshd.HighlightingLoader.Load(reader, ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance);
                }
            }

            var document = new Xceed.Wpf.AvalonDock.Layout.LayoutDocument();
            document.Title = "New".Localize();
            document.ContentId = "New".Localize();
            document.Content = textEditor;
            _documentPane.Children.Add(document);
        }

        /// <summary>
        /// アプリケーションの終了
        /// </summary>
        private void Shutdown()
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// 新規作成コマンド
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FileNewCommand(object sender, ExecutedRoutedEventArgs e) => AddNewScriptLayout();

        /// <summary>
        /// 閉じるコマンド
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseCommand(object sender, ExecutedRoutedEventArgs e) => Shutdown();
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
            TextBox.Text += $"{text}";
        }

        public void StartNewLine()
        {
            TextBox.Text += $"{System.Environment.NewLine}";
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

        public void SetFont(Font font)
        {
            throw new NotImplementedException();
        }

        public void SetDefaultFont(Font font)
        {
            throw new NotImplementedException();
        }

        private TextBox TextBox { get; set; }
    }
}
