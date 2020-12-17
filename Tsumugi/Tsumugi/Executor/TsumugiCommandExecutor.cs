using System;
using System.Collections.Generic;
using System.Text;

namespace Tsumugi.Executor
{
    /// <summary>
    /// Tsumugi 標準のコマンド実行クラス
    /// </summary>
    public class TsumugiCommandExecutor : ICommandExecutor
    {
        /// <summary>
        /// 
        /// </summary>
        public List<Listeners.TsumugiExecuteCommandListener> Listeners { get; private set; }

        public TsumugiCommandExecutor()
        {
            Listeners = new List<Listeners.TsumugiExecuteCommandListener>();
        }

        /// <summary>
        /// コマンドの実行
        /// </summary>
        /// <param name="queue"></param>
        /// <returns></returns>
        public int Execute(Commands.CommandQueue queue)
        {
            return 0;
        }
    }
}
