using System;
using System.Collections.Generic;
using System.Text;

namespace Tsumugi.Parser
{
    public class Label
    {
        /// <summary>
        /// ラベルの名前
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 見出し名
        /// </summary>
        public string Headline { get; set; }

        /// <summary>
        /// バッファ位置
        /// </summary>
        public int Position { get; set; }
    }
}
