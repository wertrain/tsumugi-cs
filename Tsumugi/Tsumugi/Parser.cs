using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Tsumugi
{
    public class Parser
    {
        public static void Execute(string script)
        {
            var stringBuilder = new StringBuilder();

            using (var reader = new StringReader(script))
            {
                char c = (char)reader.Peek();

                switch (c)
                {
                    default:
                        stringBuilder.Append(c);
                        break;
                }


            }
        }
    }
}
