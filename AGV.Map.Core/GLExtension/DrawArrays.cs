using SharpGL;
using System;

namespace AGV.Map.Core.GLExtension
{
    public static class GLDrawArrays
    {
        public static void DrawArrays(this OpenGL gl, uint mode, int length, IntPtr ptr2D)
        {
            if (length <= 0) return;
            gl.EnableClientState(OpenGL.GL_VERTEX_ARRAY);
            gl.VertexPointer(2, OpenGL.GL_INT, 0, ptr2D);
            gl.DrawArrays(mode, 0, length);
            gl.DisableClientState(OpenGL.GL_VERTEX_ARRAY);
        }
    }
}
