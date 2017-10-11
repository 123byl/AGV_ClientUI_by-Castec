using AGV.Map.Common;
using AGV.Map.Core.GLExtension;
using SharpGL;
using System.Collections.Generic;

namespace AGV.Map.Core
{
    /// <summary>
    /// 可繪面集合
    /// </summary>
    internal abstract class MutiArea : IMuti<IArea>
    {
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
        /// 繪圖
        /// </summary>
        public void Draw(OpenGL gl)
        {
            float z = (int)GLSetting.Type;
            gl.Color(GLSetting.MainColor.GetFloats());
            gl.Disable(OpenGL.GL_DEPTH_TEST);
            gl.Begin(OpenGL.GL_QUADS);
            {
                DataList.SaftyForLoop((item) =>
                {
                    gl.Vertex(item.Min.X, item.Min.Y, z);
                    gl.Vertex(item.Max.X, item.Min.Y, z);
                    gl.Vertex(item.Max.X, item.Max.Y, z);
                    gl.Vertex(item.Min.X, item.Max.Y, z);
                });
            }
            gl.End();
            gl.Enable(OpenGL.GL_DEPTH_TEST);
        }
    }

    /// <summary>
    /// 可繪線集合
    /// </summary>
    internal abstract class MutiLine : IMuti<ILine>
    {
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
                gl.Begin(OpenGL.GL_LINES);
                {
                    DataList.SaftyForLoop((item) =>
                    {
                        gl.Vertex(item.Begin.X, item.Begin.Y, z);
                        gl.Vertex(item.End.X, item.End.Y, z);
                    });
                }
                gl.End();
                gl.Enable(OpenGL.GL_DEPTH_TEST);
            }
            gl.EndStipple();
        }
    }

    /// <summary>
    /// 可繪點集合
    /// </summary>
    internal abstract class MutiPair : IMuti<IPair>
    {
        public MutiPair()
        {
        }

        public MutiPair(IEnumerable<IPair> collection)
        {
            DataList = new SaftyList<IPair>(collection);
        }

        public MutiPair(IPair item)
        {
            DataList.Add(item);
        }

        /// <summary>
        /// 集合資料
        /// </summary>
        public ISaftyList<IPair> DataList { get; } = new SaftyList<IPair>();

        /// <summary>
        /// 繪圖設定
        /// </summary>
        public IGLSetting GLSetting { get; protected set; } = new GLSetting();

        /// <summary>
        /// 物件名稱
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 繪圖
        /// </summary>
        public void Draw(OpenGL gl)
        {
            float z = (int)GLSetting.Type;
            gl.Color(GLSetting.MainColor.GetFloats());
            gl.PointSize(GLSetting.PointSize);
            gl.Disable(OpenGL.GL_DEPTH_TEST);
            gl.Begin(OpenGL.GL_POINTS);
            {
                DataList.SaftyForLoop((item) =>
                {
                    gl.Vertex(item.X, item.Y, z);
                });
            }
            gl.End();
            gl.Enable(OpenGL.GL_DEPTH_TEST);
        }
    }
}