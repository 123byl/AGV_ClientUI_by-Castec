using AGV.Map.Common;
using SharpGL;
using System;

namespace AGV.Map.Core
{
    /// <summary>
    /// 車
    /// </summary>
    [Serializable]
    internal class AGV : SingleTowardPair, IAGV
    {
        public AGV(string name) : base(name)
        {
            GLSetting = new GLSetting(EType.AGV);
        }

        public AGV(int x, int y, double toward, string name) : base(x, y, toward, name)
        {
            GLSetting = new GLSetting(EType.AGV);
        }

        public AGV(int x, int y, IAngle toward, string name) : base(x, y, toward, name)
        {
            GLSetting = new GLSetting(EType.AGV);
        }

        public AGV(ITowardPair towardPair, string name) : base(towardPair, name)
        {
            GLSetting = new GLSetting(EType.AGV);
        }

        /// <summary>
        /// 當下是否可以拖曳
        /// </summary>
        public override bool CanDrag { get; } = false;

        /// <summary>
        /// 建立拖曳點陣列
        /// </summary>
        public override IDragPoint[] CreatDragPoints()
        {
            return new IDragPoint[] {
                new DragPoint(Data.Position, GLSetting.Size, null),
            };
        }

        /// <summary>
        /// 繪圖
        /// </summary>
        public override void Draw(OpenGL gl)
        {
            gl.PushMatrix();
            {
                gl.Translate(Data.Position.X, Data.Position.Y, 0);
                gl.Rotate(Data.Toward.Theta, 0, 0, 1);
                gl.TextureBmp(GLSetting.BmpName, GLSetting.Size, GLSetting.MainColor, GLSetting.Type);
            }
            gl.PopMatrix();
        }
    }
}