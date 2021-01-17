using System;

namespace Tsumugi.Text.Commanding.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class WaitTimeCommand : CommandBase
    {
        /// <summary>
        /// 
        /// </summary>
        public ReferenceVariable<int> Time { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="time"></param>
        public WaitTimeCommand(int time)
        {
            Time = new ReferenceVariable<int>(time);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="time"></param>
        public WaitTimeCommand(string variable)
        {
            Time = new ReferenceVariable<int>(variable);
        }
    }
}
