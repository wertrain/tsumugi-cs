using System;
using System.Collections.Generic;
using System.Text;

namespace Tsumugi.Commands
{
    public class WaitTimeCommand : CommandBase
    {
        public int Time { get; }

        public WaitTimeCommand(int time)
        {
            Time = time;
        }
    }
}
