using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

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

        public void Each(Action<int, CommandBase> action)
        {
            foreach (var (command, index) in new List<CommandBase>(_queue.ToArray()).Select((item, index) => (item, index)))
            {
                action(index, command);
            }
        }

        private Queue<CommandBase> _queue;
    }
}
