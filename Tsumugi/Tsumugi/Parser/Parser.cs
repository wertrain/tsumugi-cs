using System;
using System.Collections.Generic;
using System.Text;

namespace Tsumugi.Parser
{
    /// <summary>
    /// パーサーのインターフェース
    /// </summary>
    interface IParser
    {
        /// <summary>
        /// コマンド
        /// </summary>
        Commands.CommandQueue CommandQueue { get; }

        /// <summary>
        /// パース
        /// </summary>
        /// <param name="script"></param>
        /// <returns></returns>
        int Parse(string script);
    }
}
