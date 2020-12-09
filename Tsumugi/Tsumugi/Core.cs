using System;
using System.IO;

namespace Tsumugi
{
    public class Core
    {
        public static readonly string Version = "0.0.0.0";

        public static void Compile()
        {

        }

        public static void Execute(string script)
        {
            using (var reader = new StringReader(script))
            {
                char c = (char)reader.Peek();
            }
        }
    }
}
