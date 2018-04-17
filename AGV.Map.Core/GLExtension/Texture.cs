using AGV.Map.Common;
using AGV.Map.Core.Properties;
using SharpGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AGV.Map.Core
{
    /// <summary>
    /// GL 貼圖
    /// </summary>
    public static class GLTexture
    {
        /// <summary>
        /// 執行緒鎖
        /// </summary>
        private static readonly object mKey = new object();

        /// <summary>
        /// 地圖轉 byte 對應資料
        /// </summary>
        private static Dictionary<string, uint> mData = new Dictionary<string, uint>();

        /// <summary>
        /// 使用貼圖繪製矩形，如果貼圖不存在，則用 color 著色
        /// </summary>
        public static void TextureBmp(this OpenGL gl, string name, IPair size, IColor color, EType type)
        {
            lock (mKey)
            {
                if (!mData.ContainsKey(name)) ConvertBmpToBytes(gl, name);
                if (mData.ContainsKey(name))
                {
                    float z = (int)type;
                    gl.Color(1.0, 1.0, 1.0, 1.0);
                    gl.BindTexture(OpenGL.GL_TEXTURE_2D, mData[name]);
                    gl.Enable(OpenGL.GL_TEXTURE_2D);
                    {
                        gl.Begin(OpenGL.GL_QUADS);
                        {
                            gl.TexCoord(0.0, 0.0);
                            gl.Vertex(-size.X / 2, -size.Y / 2, z);
                            gl.TexCoord(1.0, 0.0);
                            gl.Vertex(+size.X / 2, -size.Y / 2, z);
                            gl.TexCoord(1.0, 1.0);
                            gl.Vertex(+size.X / 2, +size.Y / 2, z);
                            gl.TexCoord(0.0, 1.0);
                            gl.Vertex(-size.X / 2, +size.Y / 2, z);
                        }
                        gl.End();
                    }
                    gl.Disable(OpenGL.GL_TEXTURE_2D);
                }
                else
                {
                    float z = (int)type;
                    gl.Color(color.GetFloats());
                    gl.Begin(OpenGL.GL_QUADS);
                    {
                        gl.Vertex(-size.X / 2, -size.Y / 2, z);
                        gl.Vertex(+size.X / 2, -size.Y / 2, z);
                        gl.Vertex(+size.X / 2, +size.Y / 2, z);
                        gl.Vertex(-size.X / 2, +size.Y / 2, z);
                    }
                    gl.End();
                }
            }
        }

        private static void ConvertBmpToBytes(OpenGL gl, string name)
        {
            lock (mKey)
            {
                Bitmap image = (Bitmap)Resources.ResourceManager.GetObject(name);
                if (image != null)
                {
                    image.RotateFlip(RotateFlipType.RotateNoneFlipY);
                    BitmapData bitmapdata;
                    Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
                    bitmapdata = image.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

                    byte[] rgba = new byte[image.Width * image.Height * 4];
                    unsafe
                    {
                        // p 的順序是 BGR
                        byte* p = (byte*)(void*)bitmapdata.Scan0;
                        for (int h = 0; h < image.Height; ++h)
                        {
                            for (int w = 0; w < image.Width; ++w)
                            {
                                int index = h * image.Width * 4 + w * 4;
                                // B
                                rgba[index + 2] = p[0];
                                p++;
                                // G
                                rgba[index + 1] = p[0];
                                p++;
                                // R
                                rgba[index + 0] = p[0];
                                p++;
                                if (rgba[index + 0] == 255 && rgba[index + 1] == 255 && rgba[index + 2] == 255)
                                {
                                    rgba[index + 3] = 0;
                                }
                                else
                                {
                                    rgba[index + 3] = 255;
                                }
                            }
                        }
                    }
                    int size = Marshal.SizeOf(rgba[0]) * rgba.Length;

                    IntPtr ptr = Marshal.AllocHGlobal(size);
                    Marshal.Copy(rgba, 0, ptr, rgba.Length);
                    uint[] tArray = new uint[1];
                    gl.GenTextures(1, tArray);

                    gl.PixelStore(OpenGL.GL_UNPACK_ALIGNMENT, 1);

                    gl.BindTexture(OpenGL.GL_TEXTURE_2D, tArray[0]);
                    gl.Build2DMipmaps(OpenGL.GL_TEXTURE_2D, 4, image.Width, image.Height, OpenGL.GL_RGBA, OpenGL.GL_UNSIGNED_BYTE, ptr);

                    gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR_MIPMAP_NEAREST);
                    gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_LINEAR_MIPMAP_LINEAR);

                    mData.Add(name, tArray[0]);
                }
            }
        }
    }
}