using System;
using System.Threading.Tasks;

namespace ConsoleSample
{
    class Program
    {
        /// <summary>
        /// Tsumugi コマンド実行クラスのコンソール版
        /// </summary>
        private class CommandExecutor : Tsumugi.Text.Executing.ICommandExecutor
        {
            public void PrintText(string text)
            {
                Console.Write("{0}", text);
            }

            public void StartNewLine()
            {
                Console.WriteLine();
            }

            public void WaitAnyKey()
            {
                Console.ReadKey(true);
            }

            public void StartNewPage()
            {
                Console.Clear();
            }

            public void WaitTime(int millisec)
            {
                Task.Delay(millisec).Wait();
            }

            public void DebugPring(Tsumugi.Text.Commanding.CommandQueue queue)
            {
                queue.Each((int index, Tsumugi.Text.Commanding.CommandBase command) => { Console.WriteLine("[{0}] {1}", index, command.GetType().ToString()); });
            }

        }

        static void Main(string[] args)
        {
            var script = "" +
            ":start|開始位置" +
            "[var wtime=1000][jump target=notfound]" +
            "こんにちは[r]" +
            "これは Tsumugi のテスト[wait time=wtime]です。[l][cm]" +
            "ページをクリアしました。[l][r][cm][jump target=start]" +
            "[l]";
            var interpreter = new Tsumugi.Interpreter();
            interpreter.Executor = new CommandExecutor();
            interpreter.Execute(script);
        }
    }
}
