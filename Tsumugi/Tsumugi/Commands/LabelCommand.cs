using System;
using System.Collections.Generic;
using System.Text;

namespace Tsumugi.Commands
{
    public class LabelCommand : CommandBase
    {
        public string Name { get; }

        public LabelCommand(string name)
        {
            Name = name;
        }
    }
}
