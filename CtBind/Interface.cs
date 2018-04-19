using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace CtBind {

    /// <summary>
    /// 資料顯示介面
    /// </summary>
    /// <typeparam name="TSource">資料來源</typeparam>
    public interface IDataDisplay<TSource> where TSource : IDataSource {

        /// <summary>
        /// 資料綁定
        /// </summary>
        /// <param name="source">資料來源</param>
        void Bindings(TSource source);
    }

    /// <summary>
    /// 資料來源
    /// </summary>
    public interface IDataSource : INotifyPropertyChanged {

        /// <summary>
        /// Invoke委派方法
        /// </summary>
        Action<MethodInvoker> DelInvoke { get; set; }
    }
}