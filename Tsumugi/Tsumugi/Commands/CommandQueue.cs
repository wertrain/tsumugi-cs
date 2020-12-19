using System;
using System.Collections.Generic;
using System.Text;

namespace Tsumugi.Commands
{
    public class CommandQueue
    {
        public CommandQueue()
        {
            _queue = new Queue<CommandBase>();
        }

        public void Enqueue(CommandBase command)
        {
            _queue.Enqueue(command);
        }

        public CommandBase Dequeue()
        {
            if (_queue.Count == 0)
            {
                return null;
            }

            return _queue.Dequeue();
        }

        private Queue<CommandBase> _queue;
    }
}
