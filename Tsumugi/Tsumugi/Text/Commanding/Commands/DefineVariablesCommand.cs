using System.Collections.Generic;

namespace Tsumugi.Text.Commanding.Commands
{
    /// <summary>
    /// 変数定義コマンド
    /// </summary>
    public class DefineVariablesCommand : CommandBase
    {
        /// <summary>
        /// 変数
        /// </summary>
        public class Variable
        {
            /// <summary>
            /// 変数名
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// 変数の値
            /// </summary>
            public string Value { get; set; }
        }

        /// <summary>
        /// 定義する変数のリスト
        /// </summary>
        public List<Variable> Variables { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="variables"></param>
        public DefineVariablesCommand(List<Variable> variables) => Variables = variables;
    }
}
