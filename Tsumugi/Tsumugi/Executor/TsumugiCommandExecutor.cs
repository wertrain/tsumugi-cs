using System;
using System.Collections.Generic;
using System.Text;

namespace Tsumugi.Executor
{
    /// <summary>
    /// Tsumugi 標準のコマンド実行クラス
    /// </summary>
    public class TsumugiCommandExecutor : ICommandExecutor
    {
        public TsumugiCommandExecutor()
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
        public int Execute(Commands.CommandQueue queue)
        {
            Commands.CommandBase command = null;

            while((command = queue.Dequeue()) != null)
            {
                switch (command)
                {
                    case Commands.PrintTextCommand cmd:
                        PrintText(cmd.Text);
                        break;

                    case Commands.NewLineCommand cmd:
                        StartNewLine();
                        break;

                    case Commands.WaitKeyCommand cmd:
                        WaitAnyKey();
                        break;

                    case Commands.NewPageCommand cmd:
                        StartNewPage();
                        break;

                    case Commands.WaitTimeCommand cmd:
                        WaitTime(cmd.Time);
                        break;

                    case Commands.InsertIndentCommand cmd:
                        Indent(Environment.Indentation);
                        break;

                    case Commands.JumpCommand cmd:
                        var labels = queue.FindCommands<Commands.LabelCommand>();
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
