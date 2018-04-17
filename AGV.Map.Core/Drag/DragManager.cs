using AGV.Map.Common;
using AGV.Map.Core.GLExtension;
using SharpGL;
using System;

namespace AGV.Map.Core
{
    /// <summary>
    /// 拖曳控制點管理器
    /// </summary>
    internal class DragManager : IDragManager
    {
        public ISaftyList<ILine> mDragBound = new SaftyList<ILine>();

        public ISaftyList<ILine> mDragPointBound = new SaftyList<ILine>();

        private readonly object mKey = new object();

        private IDragPoint[] mDragPoints = null;

        private int mDragPointsIndex = -1;

        private IDragable mDragTarget = null;

        private IRecode<IPair> mPositionRecode = new Recode<IPair>(new Pair());
        private EDragStatus mStatus = EDragStatus.Idle;

        /// <summary>
        /// 拖曳對象的邊緣
        /// </summary>
        public ISaftyList<ILine> DragBound { get { lock (mKey) return mDragBound; } }

        /// <summary>
        /// 當下拖曳點的邊緣
        /// </summary>
        public ISaftyList<ILine> DragPointBound { get { lock (mKey) return mDragPointBound; } }

        /// <summary>
        /// 拖曳對象
        /// </summary>
        public IDragable DragTaeget {
            get { lock (mKey) return mDragTarget; }
            set {
                lock (mKey)
                {
                    if (value == null)
                    {
                        mDragTarget = null;
                        mDragPoints = null;
                        mDragPointsIndex = -1;
                        Status = EDragStatus.Idle;
                        DragBound.Clear();
                    }
                    else
                    {
                        mDragTarget = value;
                        mDragPoints = mDragTarget.CreatDragPoints();
                        mDragPointsIndex = -1;
                        Status = EDragStatus.Ready;
                        DragBound.Clear();
                    }
                    UpdateBound();
                }
            }
        }

        /// <summary>
        /// 繪圖設定
        /// </summary>
        public IGLSetting GLSetting { get; } = new GLSetting(EType.DragBound);

        /// <summary>
        /// 控制器狀態
        /// </summary>
        public EDragStatus Status { get { lock (mKey) return mStatus; } private set { lock (mKey) mStatus = value; } }

        /// <summary>
        /// 拖曳
        /// </summary>
        public void Drag(IPair newPosition)
        {
            lock (mKey)
            {
                try
                {
                    if (Status == EDragStatus.Dragging && mDragTarget != null)
                    {
                        if (mDragPoints == null || mDragPointsIndex < 0 || mDragPointsIndex >= mDragPoints.Length)
                        {
                            Status = EDragStatus.Ready;
                            return;
                        }
                        mPositionRecode.Now.X = newPosition.X;
                        mPositionRecode.Now.Y = newPosition.Y;
                        IPair pos = mDragPoints[mDragPointsIndex].Point.Add(mPositionRecode.Now).Subtraction(mPositionRecode.Old);
                        mPositionRecode.Push();
                        mDragPoints[mDragPointsIndex]?.UpdateDragPoint(pos);
                        mDragPoints = mDragTarget.CreatDragPoints();
                        UpdateBound();
                    }
                }
                catch
                {
                    // 操作失敗，釋放所有控制權
                    DragTaeget = null;
                }
            }
        }

        /// <summary>
        /// 繪圖
        /// </summary>
        public void Draw(OpenGL gl)
        {
            lock (mKey)
            {
                if (Status != EDragStatus.Idle)
                {
                    mDragPoints = mDragTarget.CreatDragPoints();
                    UpdateBound();
                }
                float z = (int)GLSetting.Type;
                gl.Color(GLSetting.MainColor.GetFloats());
                gl.LineWidth(GLSetting.LineWidth);
                gl.BeginStipple(GLSetting.LineStyle);
                {
                    gl.Begin(OpenGL.GL_LINES);
                    {
                        mDragBound.SaftyForLoop((item) =>
                        {
                            gl.Vertex(item.Begin.X, item.Begin.Y, z);
                            gl.Vertex(item.End.X, item.End.Y, z);
                        });
                    }
                    gl.End();
                }
                gl.EndStipple();

                gl.Begin(OpenGL.GL_LINES);
                {
                    mDragPointBound.SaftyForLoop((item) =>
                    {
                        gl.Vertex(item.Begin.X, item.Begin.Y, z);
                        gl.Vertex(item.End.X, item.End.Y, z);
                    });
                }
                gl.End();
            }
        }

        /// <summary>
        /// 釋放拖曳控制點
        /// </summary>
        public void ReleaseControl()
        {
            lock (mKey)
            {
                if (Status == EDragStatus.Dragging)
                {
                    mDragPoints = null;
                    mDragPointsIndex = -1;
                    Status = EDragStatus.Ready;
                }
            }
        }

        /// <summary>
        /// 使用座標嘗試嘗試取得拖曳控制點
        /// </summary>
        public void TakeControl(IPair position)
        {
            lock (mKey)
            {
                if (Status == EDragStatus.Ready && mDragTarget != null)
                {
                    mDragPoints = mDragTarget.CreatDragPoints();
                    mDragPointsIndex = -1;
                    for (int ii = 0; ii < mDragPoints.Length; ++ii)
                    {
                        IPair sub = mDragPoints[ii].Point.Subtraction(position).Abs();
                        if (sub.X < mDragPoints[ii].Size.X / 2 && sub.Y < mDragPoints[ii].Size.Y / 2)
                        {
                            mPositionRecode.Now.X = position.X;
                            mPositionRecode.Now.Y = position.Y;
                            mPositionRecode.Push();
                            Status = EDragStatus.Dragging;
                            mDragPointsIndex = ii;
                            UpdateBound();
                            break;
                        }
                    }
                }
            }
        }

        private void UpdateBound()
        {
            lock (mKey)
            {
                mDragBound.Clear();
                if (Status == EDragStatus.Dragging || Status == EDragStatus.Ready && mDragPoints != null)
                {
                    foreach (var item in mDragPoints)
                    {
                        Area area = new Area(item.Point, item.Size.X, item.Size.Y);
                        mDragBound.AddRange(area.ConvertToLines());
                    }
                }
                mDragPointBound.Clear();
                if (Status == EDragStatus.Dragging && mDragPoints != null && mDragPointsIndex >= 0 && mDragPointsIndex < mDragPoints.Length)
                {
                    var item = mDragPoints[mDragPointsIndex];
                    Area area = new Area(item.Point, item.Size.X, item.Size.Y);
                    mDragPointBound.AddRange(area.ConvertToLines());
                }
            }
        }
    }
}