namespace TsumugiReadEvalPrintLoop
{
    class Program
    {
        static void Main(string[] args)
        {
            var repl = new Tsumugi.Script.ReadEvalPrintLoop();
            repl.Start();
        }
    }
}
