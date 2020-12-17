using System;
using System.Collections.Generic;
using System.Text;

namespace Tsumugi.Executor
{
    public interface ICommandExecutor
    {
        /// <summary>
        /// コマンドの実行
        /// </summary>
        /// <param name="queue">実行するコマンドキュー</param>
        /// <returns></returns>
        int Execute(Commands.CommandQueue queue);
    }
}
