using System;
using System.Collections.Generic;
using System.Text;

namespace Tsumugi.Commands
{
    public class LabelCommand : CommandBase
    {
        public string Name { get; }

        public string Headline { get; }

        public LabelCommand(string name, string headline)
        {
            Name = name;
            Headline = headline;
        }
    }
}
