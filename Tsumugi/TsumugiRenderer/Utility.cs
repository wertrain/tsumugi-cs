using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TsumugiRenderer
{
    /// <summary>
    /// 
    /// </summary>
    class Utility
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static SharpDX.Mathematics.Interop.RawColor4 ToRawColor4(int color)
        {
            return ToRawColor4(Color.FromArgb(color));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static SharpDX.Mathematics.Interop.RawColor4 ToRawColor4(Color color)
        {
            return new SharpDX.Mathematics.Interop.RawColor4(color.R * 1.0f, color.G * 1.0f, color.B * 1.0f, color.A * 1.0f);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Color FromRawColor4(SharpDX.Mathematics.Interop.RawColor4 color)
        {
            return Color.FromArgb((int)color.A, (int)color.R, (int)color.G, (int)color.B);
        }
    }
}
