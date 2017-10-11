using AGV.Map.Common;
using SharpGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGV.Map.Core.GLExtension
{
    public static class GLStipple
    {
        public static void BeginStipple(this OpenGL gl, ELineStyle style, int factor = 5)
        {
            if (style == ELineStyle._1111111111111111) return;
            ushort pattern = Convert.ToUInt16(style.ToString().Substring(1), 2);
            gl.Enable(OpenGL.GL_LINE_STIPPLE);
            gl.LineStipple(factor, pattern);
        }

        public static void EndStipple(this OpenGL gl)
        {
            gl.Disable(OpenGL.GL_LINE_STIPPLE);
        }
    }
}