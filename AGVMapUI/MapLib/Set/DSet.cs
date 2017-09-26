using SharpGL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AGVMap
{
    /// <summary>
    /// 具執行緒安全的可繪集合
    /// </summary>
    public interface IDSet<T> : IDrawable, IDSetProperty, ISet<T> where T : ICCWVertex
    {
        /// <summary>
        /// 圖片偏移
        /// </summary>
        IPair Shift { get; set; }
    }

    /// <summary>
    /// 可繪集合顯示屬性
    /// </summary>
    public interface IDSetProperty
    {
        /// <summary>
        /// 顏色
        /// </summary>
        IColor Color { get; set; }

        /// <summary>
        /// 圖層
        /// </summary>
        ELayer Layer { get; set; }
    }

    /// <summary>
    /// 提供執行緒安全的可繪集合，並以 gl.CallList 加速繪圖
    /// </summary>
    public abstract class DSet<T> : IDSet<T> where T : ICCWVertex
    {
        #region - 顏色 -

        /// <summary>
        /// 顏色
        /// </summary>
        public IColor Color { get { return mColor; } set { mColor = new Color(value); } }

        private Color mColor = new Color();

        #endregion - 顏色 -

        /// <summary>
        /// 元素數
        /// </summary>
        public int Count { get { lock (Key) return mSet.Count; } }

        /// <summary>
        /// 是否強制重繪
        /// </summary>
        public bool ForceReDraw { get; set; } = false;

        /// <summary>
        /// 執行緒鎖
        /// </summary>
        public object Key { get; } = new object();

        /// <summary>
        /// 圖層
        /// </summary>
        public ELayer Layer { get; set; } = ELayer.Buttom;

        /// <summary>
        /// 獲得唯讀值
        /// </summary>
        public ReadOnlyCollection<T> AsReadOnly()
        {
            lock (Key)
            {
                return mSet.AsReadOnly();
            }
        }

        /// <summary>
        /// 繪圖
        /// </summary>
        public void Draw(OpenGL gl)
        {
            lock (Key)
            {
                if (ForceReDraw)
                {
                    if (mGLList != 0)
                    {
                        gl.DeleteLists(mGLList, 1);
                        mGLList = 0;
                    }
                }
                gl.PushMatrix();
                {
                    gl.Translate(Shift.X, Shift.Y, (int)Layer);
                    gl.Color(Color.ToArray);
                    if (mGLList != 0)
                    {
                        gl.CallList(mGLList);
                    }
                    else
                    {
                        mGLList = gl.GenLists(1);
                        gl.NewList(mGLList, OpenGL.GL_COMPILE_AND_EXECUTE);
                        {
                            ReDraw(gl);
                        }
                        gl.EndList();
                    }
                }
                gl.PopMatrix();
            }
        }

        /// <summary>
        /// 對所有陣列操作
        /// </summary>
        public void ForEach(Action<T> action)
        {
            lock (Key)
            {
                mSet.ForEach(action);
            }
        }

        /// <summary>
        /// 排序
        /// </summary>
        public void Sort(Comparison<T> comparison)
        {
            lock (Key)
            {
                mSet.Sort(comparison);
            }
        }

        /// <summary>
        /// 重繪
        /// </summary>
        protected abstract void ReDraw(OpenGL gl);

        #region - 平移/旋轉 -

        /// <summary>
        /// 圖片偏移
        /// </summary>
        public IPair Shift { get { return mShift; } set { mShift = new Pair(value); } }

        private IPair mShift = new Pair();

        #endregion - 平移/旋轉 -

        #region - 新增 -

        /// <summary>
        /// 加入新元素
        /// </summary>
        public void Add(T item)
        {
            lock (Key)
            {
                mSet.Add(item);
                ForceReDraw = true;
            }
        }

        /// <summary>
        /// 加入新元素
        /// </summary>
        public void AddRange(IEnumerable<T> collection)
        {
            lock (Key)
            {
                mSet.AddRange(collection);
                ForceReDraw = true;
            }
        }

        /// <summary>
        /// 加入新元素
        /// </summary>
        public void AddRange(ISet<T> collection)
        {
            lock (Key)
            {
                if (collection is DSet<T>)
                {
                    DSet<T> tmp = (DSet<T>)collection;
                    mSet.AddRange(tmp.mSet);
                    ForceReDraw = true;
                    return;
                }
                throw new Exception("Type Error");
            }
        }

        #endregion - 新增 -

        #region - 清除 -

        /// <summary>
        /// 清除資料
        /// </summary>
        public void Clear()
        {
            lock (Key)
            {
                mSet.Clear();
            }
        }

        /// <summary>
        /// 清除舊資料，並加入新元素
        /// </summary>
        public void ClearAndAdd(T item)
        {
            lock (Key)
            {
                mSet.ClearAndAdd(item);
                ForceReDraw = true;
            }
        }

        /// <summary>
        /// 清除舊資料，並加入新元素
        /// </summary>
        public void ClearAndAddRange(ISet<T> collection)
        {
            lock (Key)
            {
                mSet.ClearAndAddRange(collection);
                ForceReDraw = true;
            }
        }

        /// <summary>
        /// 清除舊資料，並加入新元素
        /// </summary>
        public void ClearAndAddRange(IEnumerable<T> collection)
        {
            lock (Key)
            {
                mSet.ClearAndAddRange(collection);
            }
        }

        /// <summary>
        /// 移除符合的項目
        /// </summary>
        public int RemoveAll(Predicate<T> match)
        {
            lock (Key)
            {
                ForceReDraw = true;
                return mSet.RemoveAll(match);
            }
        }

        #endregion - 清除 -

        #region - 搜尋 -

        /// <summary>
        /// 由引索值獲得元素
        /// </summary>
        public T At(int index)
        {
            lock (Key)
            {
                return mSet.At(index);
            }
        }

        /// <summary>
        /// 搜尋
        /// </summary>
        public int BinarySearch(T item)
        {
            lock (Key)
            {
                return mSet.BinarySearch(item);
            }
        }

        /// <summary>
        /// 搜尋
        /// </summary>
        public T Find(Predicate<T> match)
        {
            lock (Key)
            {
                return mSet.Find(match);
            }
        }

        #endregion - 搜尋 -

        /// <summary>
        /// GL 圖片編號
        /// </summary>
        private uint mGLList = 0;

        /// <summary>
        /// 陣列資料
        /// </summary>
        private ListSet<T> mSet = new ListSet<T>();
    }
}
