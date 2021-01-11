using Tsumugi.Text.Commanding;

namespace Tsumugi.Text.Executing
{
    /// <summary>
    /// 標準のコマンド実行クラス
    /// </summary>
    public class CommandExecutor : ICommandExecutor
    {
        public CommandExecutor()
        {
            Environment = Environment.Default();
        }

        /// <summary>
        /// 環境設定
        /// </summary>
        public Environment Environment { get; set; }

        /// <summary>
        /// コマンドの実行
        /// </summary>
        /// <param name="queue"></param>
        /// <returns></returns>
        public int Execute(CommandQueue queue)
        {
            CommandBase command = null;

            while((command = queue.Dequeue()) != null)
            {
                switch (command)
                {
                    case Commanding.Commands.PrintTextCommand cmd:
                        PrintText(cmd.Text);
                        break;

                    case Commanding.Commands.NewLineCommand cmd:
                        StartNewLine();
                        break;

                    case Commanding.Commands.WaitKeyCommand cmd:
                        WaitAnyKey();
                        break;

                    case Commanding.Commands.NewPageCommand cmd:
                        StartNewPage();
                        break;

                    case Commanding.Commands.WaitTimeCommand cmd:
                        WaitTime(cmd.Time);
                        break;

                    case Commanding.Commands.InsertIndentCommand cmd:
                        Indent(Environment.Indentation);
                        break;

                    case Commanding.Commands.JumpCommand cmd:
                        var labels = queue.FindCommands<Commanding.Commands.LabelCommand>();
                        var label = labels.Find(c => c.Name == cmd.Target);
                        queue.Seek(label);
                        break;

                }
            }
            
            return 0;
        }

        /// <summary>
        /// 文字列の表示
        /// </summary>
        /// <param name="text"></param>
        public virtual void PrintText(string text) { }

        /// <summary>
        /// 改行
        /// </summary>
        public virtual void StartNewLine() { }

        /// <summary>
        /// キー入力待ち
        /// </summary>
        public virtual void WaitAnyKey() { }

        /// <summary>
        /// 改ページ
        /// </summary>
        public virtual void StartNewPage() { }

        /// <summary>
        /// 指定時間の待ち
        /// </summary>
        public virtual void WaitTime(int millisec) { }

        /// <summary>
        /// インデント
        /// </summary>
        /// <param name="size"></param>
        public virtual void Indent(int count) { }
    }
}
