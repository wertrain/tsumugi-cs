namespace Tsumugi.Text.Commanding.Commands
{
    /// <summary>
    /// ラベルコマンド
    /// </summary>
    public class LabelCommand : CommandBase
    {
        /// <summary>
        /// ラベル名
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// ラベルの見出し
        /// </summary>
        public string Headline { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="name"></param>
        /// <param name="headline"></param>
        public LabelCommand(string name, string headline)
        {
            Name = name;
            Headline = headline;
        }
    }
}
