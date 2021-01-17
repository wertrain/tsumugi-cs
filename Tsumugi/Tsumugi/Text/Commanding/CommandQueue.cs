﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Tsumugi.Text.Commanding
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
            _queueIndex = 0;
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
        public CommandBase dequeue()
        {
            if (_queue.Count == 0)
            {
                return null;
            }

            return _queue.Dequeue();
        }

        /// <summary>
        /// 非破壊的デキュー
        /// </summary>
        /// <returns></returns>
        public CommandBase Dequeue()
        {
            if (_queue.Count <= _queueIndex)
            {
                return null;
            }

            return _queue.ElementAtOrDefault(_queueIndex++);
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
        /// コマンドを取得
        /// </summary>
        /// <returns></returns>
        public List<T> FindCommands<T>() where T : CommandBase
        {
            var commands = new List<T>();
            foreach (var command in _queue)
            {
                if (command.GetType()== typeof(T))
                {
                    commands.Add((T)command);
                }
            }
            return commands;
        }

        /// <summary>
        /// 次の指定のコマンドを取得
        /// </summary>
        /// <typeparam name="T">取得するコマンドの型</typeparam>
        /// <returns>見つかったコマンド</returns>
        public T FindNext<T>() where T : CommandBase
        {
            for (var index = _queueIndex; index < _queue.Count; ++index)
            {
                var command = _queue.ElementAtOrDefault(index);

                if (command.GetType().Equals(typeof(T)))
                {
                    return (T)command;
                }
            }
            return null;
        }

        /// <summary>
        /// 指定されたコマンドの位置に移動する
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public bool Seek(CommandBase target)
        {
            foreach (var (command, index) in new List<CommandBase>(_queue.ToArray()).Select((item, index) => (item, index)))
            {
                if (command == target)
                {
                    _queueIndex = index;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// コマンドキュー
        /// </summary>
        private Queue<CommandBase> _queue;

        /// <summary>
        /// キューのインデックス
        /// </summary>
        private int _queueIndex;
    }
}
