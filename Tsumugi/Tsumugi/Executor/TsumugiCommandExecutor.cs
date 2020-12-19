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
        /// <summary>
        /// 
        /// </summary>
        public List<Listeners.TsumugiExecuteCommandListener> Listeners { get; private set; }

        public TsumugiCommandExecutor()
        {
            Listeners = new List<Listeners.TsumugiExecuteCommandListener>();
        }

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
                }
            }
            
            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        public virtual void PrintText(string text) { }

        /// <summary>
        /// 
        /// </summary>
        public virtual void StartNewLine() { }

        /// <summary>
        /// 
        /// </summary>
        public virtual void WaitAnyKey() { }

        /// <summary>
        /// 
        /// </summary>
        public virtual void StartNewPage() { }
    }
}
