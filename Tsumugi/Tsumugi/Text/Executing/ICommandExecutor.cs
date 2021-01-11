namespace Tsumugi.Text.Executing
{
    /// <summary>
    /// コマンド実行の基底クラス
    /// </summary>
    public interface ICommandExecutor
    {
        /// <summary>
        /// 環境設定
        /// </summary>
        Environment Environment { get; }

        /// <summary>
        /// コマンドの実行
        /// </summary>
        /// <param name="queue">実行するコマンドキュー</param>
        /// <returns></returns>
        int Execute(Commanding.CommandQueue queue);
    }
}
