using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleSample
{
    class Program
    {
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
        }

        static void Main(string[] args)
        {
            Tsumugi.Parser parser = new Tsumugi.Parser();
            parser.Parse("こんにちは[r]これは Tsumugi のテストです。[l][cm]ページをクリアしました。[l][r]");

            CommandExecutor executor = new CommandExecutor();
            executor.Execute(parser.CommandQueue);
        }
    }
}
