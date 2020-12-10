﻿using System;
using System.IO;
using System.Text;

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
            var stringBuilder = new StringBuilder();
            using (var reader = new StringReader(script))
            {
                char c = (char)reader.Peek();

                switch(c)
                {
                    default:
                        stringBuilder.Append(c);
                        break;
                }
            }
        }
    }
}
