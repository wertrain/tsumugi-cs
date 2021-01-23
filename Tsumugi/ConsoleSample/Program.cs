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
            // var script = "" +
            // ":start|開始位置" +
            // "[var wtime=1000][jump target=notfound]" +
            // "こんにちは[r]" +
            // "これは Tsumugi のテスト[wait time=wtime]です。[l][cm]" +
            // "ページをクリアしました。[l][r][cm][jump target=start]" +
            // "[l]";
            var script = "" +
                "[var wtime=1000 ltime=2]" + Environment.NewLine +
                "[if exp=wtime==1000&&ltime==1]" + Environment.NewLine +
                    "wtime は 1000 です[r]" + Environment.NewLine +
                    "[if exp=(ltime==0)]" + Environment.NewLine +
                        "wtime は 1000 です ltime は 0 です[r]" + Environment.NewLine +
                        "wtime は 1000 です ltime は 0 です[r]" + Environment.NewLine +
                    "[else]" + Environment.NewLine +
                         "wtime は 1000 です ltime はそれ以外です[r]" + Environment.NewLine +
                         "wtime は 1000 です ltime はそれ以外です[r]" + Environment.NewLine +
                    "[endif]" + Environment.NewLine +
                    "wtime は 1000 でした[r]" + Environment.NewLine +
                    "wtime は 1000 でした[r]" + Environment.NewLine +
                    "wtime は 1000 でした[r]" + Environment.NewLine +
                    "wtime は 1000 でした[r]" + Environment.NewLine +
                "[elif exp=(wtime==100)]" + Environment.NewLine +
                    "wtime は 100 です[r]" + Environment.NewLine +
                    "wtime は 100 です[r]" + Environment.NewLine +
                    "wtime は 100 です[r]" + Environment.NewLine +
                "[else]" + Environment.NewLine +
                    "どこにも入りません[r]" + Environment.NewLine +
                    "どこにも入りません[r]" + Environment.NewLine +
                    "どこにも入りません[r]" + Environment.NewLine +
                "[endif]" + Environment.NewLine +
            "";
            var interpreter = new Tsumugi.Interpreter();
            interpreter.OnPrintError += Interpreter_OnPrintError;
            interpreter.Executor = new CommandExecutor();
            interpreter.Execute(script);
        }

        private static void Interpreter_OnPrintError(object sender, string error)
        {
            Console.WriteLine("{0}", error);
        }
    }
}
