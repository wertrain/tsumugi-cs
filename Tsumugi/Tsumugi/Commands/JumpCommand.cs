using System;
using System.Collections.Generic;
using System.Text;

namespace Tsumugi.Commands
{
    public class JumpCommand : CommandBase
    {
        public string Target { get; }

        public JumpCommand(string target)
        {
            Target = target;
        }
    }
}
