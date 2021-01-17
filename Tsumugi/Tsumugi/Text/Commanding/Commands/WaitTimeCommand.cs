using System;

namespace Tsumugi.Text.Commanding.Commands
{
    /// <summary>
    /// 指定時間待ちコマンド
    /// </summary>
    public class WaitTimeCommand : CommandBase
    {
        /// <summary>
        /// 待ち時間
        /// </summary>
        public ReferenceVariable<int> Time { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="time"></param>
        public WaitTimeCommand(int time) => Time = new ReferenceVariable<int>(time);

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="time"></param>
        public WaitTimeCommand(string variable) => Time = new ReferenceVariable<int>(variable);
    }
}
