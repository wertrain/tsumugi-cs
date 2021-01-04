using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleSample
{
    class Program
    {
        /// <summary>
        /// Tsumugi コマンド実行クラスのコンソール版
        /// </summary>
        private class CommandExecutor : Tsumugi.Executor.TsumugiCommandExecutor
        {
            public override void PrintText(string text)
            {
                Console.Write("{0}", text);
            }

            public override void StartNewLine()
            {
                Console.WriteLine();
            }

            public override void WaitAnyKey()
            {
                Console.ReadKey(true);
            }

            public override void StartNewPage()
            {
                Console.Clear();
            }

            public override void WaitTime(int millisec)
            {
                Task.Delay(millisec).Wait();
            }

            public override void Indent(int count)
            {
                Console.Write("\t");
            }

            public void DebugPring(Tsumugi.Commands.CommandQueue queue)
            {
                queue.Each((int index, Tsumugi.Commands.CommandBase command) => { Console.WriteLine("[{0}] {1}", index, command.GetType().ToString()); });
            }

        }

        static void Main(string[] args)
        {
            var repl = new Tsumugi.Script.ReadEvalPrintLoop();
            repl.Start();
        }
    }
}
