using System.Collections.Generic;

namespace Tsumugi.Text.Commanding.Commands
{
    public class DefineVariablesCommand : CommandBase
    {
        /// <summary>
        /// 変数
        /// </summary>
        public class Variable
        {
            /// <summary>
            /// 
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string Value { get; set; }
        }

        /// <summary>
        /// 定義する変数のリスト
        /// </summary>
        public List<Variable> Variables { get; set; }
    }
}
