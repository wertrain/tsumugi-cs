using System;
using System.Collections.Generic;
using System.Text;

namespace Tsumugi.Text
{
    public class Environment
    {
        public int Indentation { get; set; }

        public static Environment Default()
        {
            var env = new Environment();
            env.Indentation = 4;
            return env;
        }
    }
}
