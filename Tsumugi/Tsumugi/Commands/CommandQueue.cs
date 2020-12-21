using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Tsumugi.Commands
{
    /// <summary>
    /// コマンドをキューで管理するクラス
    /// </summary>
    public class CommandQueue
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CommandQueue()
        {
            _queue = new Queue<CommandBase>();
        }

        /// <summary>
        /// エンキュー
        /// </summary>
        /// <param name="command"></param>
        public void Enqueue(CommandBase command)
        {
            _queue.Enqueue(command);
        }

        /// <summary>
        /// デキュー
        /// </summary>
        /// <returns></returns>
        public CommandBase Dequeue()
        {
            if (_queue.Count == 0)
            {
                return null;
            }

            return _queue.Dequeue();
        }

        /// <summary>
        /// それぞれのコマンドに対して処理を行う
        /// </summary>
        /// <param name="action"></param>
        public void Each(Action<int, CommandBase> action)
        {
            foreach (var (command, index) in new List<CommandBase>(_queue.ToArray()).Select((item, index) => (item, index)))
            {
                action(index, command);
            }
        }

        /// <summary>
        /// コマンドキュー
        /// </summary>
        private Queue<CommandBase> _queue;
    }
}
