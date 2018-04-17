using AGV.Map.Common;
using AGV.Map.Core.GLExtension;
using SharpGL;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace AGV.Map.Core
{
    /// <summary>
    /// 可繪面集合
    /// </summary>
    internal abstract class MutiArea : IMuti<IArea>
    {
        /// <summary>
        /// 執行緒鎖
        /// </summary>
        private readonly object mKey = new object();

        private IntPtr mPtr;

        /// <summary>
        /// 頂點數
        /// </summary>
        private int mVertexCount = -1;

        private uint preDataEditVersion = 0;

        public MutiArea()
        {
        }

        public MutiArea(IEnumerable<IArea> collection)
        {
            DataList = new SaftyList<IArea>(collection);
        }

        public MutiArea(IArea item)
        {
            DataList.Add(item);
        }

        /// <summary>
        /// 集合資料
        /// </summary>
        public ISaftyList<IArea> DataList { get; } = new SaftyList<IArea>();

        /// <summary>
        /// 繪圖設定
        /// </summary>
        public IGLSetting GLSetting { get; protected set; } = new GLSetting();

        /// <summary>
        /// 物件名稱
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 重新生成頂點陣列(加速顯示)
        /// </summary>
        public void BuildVertexArray()
        {
            lock (mKey)
            {
                mVertexCount = -1;
                if (mPtr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(mPtr);
                    mPtr = IntPtr.Zero;
                }
            }
        }

        /// <summary>
        /// 繪圖
        /// </summary>
        public void Draw(OpenGL gl)
        {
            float z = (int)GLSetting.Type;
            gl.Color(GLSetting.MainColor.GetFloats());
            gl.Disable(OpenGL.GL_DEPTH_TEST);
            GenVertexArray();
            gl.DrawArrays(OpenGL.GL_QUADS, mVertexCount, mPtr);
            gl.End();
            gl.Enable(OpenGL.GL_DEPTH_TEST);
        }

        private void GenVertexArray()
        {
            lock (mKey)
            {
                if (preDataEditVersion == DataList.EditVersion && mVertexCount != -1) return;
                BuildVertexArray();
                preDataEditVersion = DataList.EditVersion;
                int[] vertex = new int[DataList.Count * 8];
                mVertexCount = vertex.Length / 2;
                int index = 0;
                DataList.SaftyForLoop((item) =>
                {
                    vertex[index] = item.Min.X;
                    index++;
                    vertex[index] = item.Min.Y;
                    index++;
                    vertex[index] = item.Max.X;
                    index++;
                    vertex[index] = item.Min.Y;
                    index++;
                    vertex[index] = item.Max.X;
                    index++;
                    vertex[index] = item.Max.Y;
                    index++;
                    vertex[index] = item.Min.X;
                    index++;
                    vertex[index] = item.Max.Y;
                    index++;
                });
                mPtr = Marshal.AllocHGlobal(vertex.Length * sizeof(int));
                Marshal.Copy(vertex, 0, mPtr, vertex.Length);
            }
        }
    }

    /// <summary>
    /// 可繪線集合
    /// </summary>
    internal abstract class MutiLine : IMuti<ILine>
    {
        /// <summary>
        /// 執行緒鎖
        /// </summary>
        private readonly object mKey = new object();

        private IntPtr mPtr;

        /// <summary>
        /// 頂點數
        /// </summary>
        private int mVertexCount = -1;

        private uint preDataEditVersion = 0;

        public MutiLine()
        {
        }

        public MutiLine(IEnumerable<ILine> collection)
        {
            DataList = new SaftyList<ILine>(collection);
        }

        public MutiLine(ILine item)
        {
            DataList.Add(item);
        }

        /// <summary>
        /// 集合資料
        /// </summary>
        public ISaftyList<ILine> DataList { get; } = new SaftyList<ILine>();

        /// <summary>
        /// 繪圖設定
        /// </summary>
        public IGLSetting GLSetting { get; protected set; } = new GLSetting();

        /// <summary>
        /// 物件名稱
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 重新生成頂點陣列(加速顯示)
        /// </summary>
        public void BuildVertexArray()
        {
            lock (mKey)
            {
                mVertexCount = -1;
                if (mPtr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(mPtr);
                    mPtr = IntPtr.Zero;
                }
            }
        }

        /// <summary>
        /// 繪圖
        /// </summary>
        public void Draw(OpenGL gl)
        {
            float z = (int)GLSetting.Type;
            gl.LineWidth(GLSetting.LineWidth);
            gl.Color(GLSetting.MainColor.GetFloats());
            gl.BeginStipple(GLSetting.LineStyle);
            {
                gl.Disable(OpenGL.GL_DEPTH_TEST);
                GenVertexArray();
                gl.DrawArrays(OpenGL.GL_LINES, mVertexCount, mPtr);
                gl.Enable(OpenGL.GL_DEPTH_TEST);
            }
            gl.EndStipple();
        }

        private void GenVertexArray()
        {
            lock (mKey)
            {
                if (preDataEditVersion == DataList.EditVersion && mVertexCount != -1) return;
                BuildVertexArray();
                preDataEditVersion = DataList.EditVersion;
                int[] vertex = new int[DataList.Count * 4];
                mVertexCount = vertex.Length / 2;
                int index = 0;
                DataList.SaftyForLoop((item) =>
                {
                    vertex[index] = item.Begin.X;
                    index++;
                    vertex[index] = item.Begin.Y;
                    index++;
                    vertex[index] = item.End.X;
                    index++;
                    vertex[index] = item.End.Y;
                    index++;
                });
                mPtr = Marshal.AllocHGlobal(vertex.Length * sizeof(int));
                Marshal.Copy(vertex, 0, mPtr, vertex.Length);
            }
        }
    }

    /// <summary>
    /// 可繪點集合
    /// </summary>
    internal abstract class MutiPair : IMuti<IPair>
    {
        /// <summary>
        /// 執行緒鎖
        /// </summary>
        private readonly object mKey = new object();

        private IntPtr mPtr;

        /// <summary>
        /// 頂點數
        /// </summary>
        private int mVertexCount = -1;

        private uint preDataEditVersion = 0;

        public MutiPair()
        {
        }

        public MutiPair(IEnumerable<IPair> collection)
        {
            mDataList = new SaftyList<IPair>(collection);
        }

        public MutiPair(IPair item)
        {
            DataList.Add(item);
        }

        /// <summary>
        /// 集合資料
        /// </summary>
        public ISaftyList<IPair> DataList { get { lock (mKey) return mDataList; } }

        /// <summary>
        /// 繪圖設定
        /// </summary>
        public IGLSetting GLSetting { get; protected set; } = new GLSetting();

        /// <summary>
        /// 物件名稱
        /// </summary>
        public string Name { get; set; } = string.Empty;

        private ISaftyList<IPair> mDataList { get; } = new SaftyList<IPair>();

        /// <summary>
        /// 重新生成頂點陣列(加速顯示)
        /// </summary>
        public void BuildVertexArray()
        {
            lock (mKey)
            {
                mVertexCount = -1;
                if (mPtr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(mPtr);
                    mPtr = IntPtr.Zero;
                }
            }
        }

        /// <summary>
        /// 繪圖
        /// </summary>
        public void Draw(OpenGL gl)
        {
            float z = (int)GLSetting.Type;
            gl.Color(GLSetting.MainColor.GetFloats());
            gl.PointSize(GLSetting.PointSize);
            gl.Disable(OpenGL.GL_DEPTH_TEST);
            GenVertexArray();
            gl.DrawArrays(OpenGL.GL_POINTS, mVertexCount, mPtr);
            gl.Enable(OpenGL.GL_DEPTH_TEST);
        }

        private void GenVertexArray()
        {
            lock (mKey)
            {
                if (preDataEditVersion == DataList.EditVersion && mVertexCount != -1) return;
                BuildVertexArray();
                preDataEditVersion = DataList.EditVersion;
                int[] vertex = new int[DataList.Count * 2];
                mVertexCount = vertex.Length / 2;
                int index = 0;
                DataList.SaftyForLoop((item) =>
                {
                    vertex[index] = item.X;
                    index++;
                    vertex[index] = item.Y;
                    index++;
                });
                mPtr = Marshal.AllocHGlobal(vertex.Length * sizeof(int));
                Marshal.Copy(vertex, 0, mPtr, vertex.Length);
            }
        }
    }
}