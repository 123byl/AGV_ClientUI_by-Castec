using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using WeifenLuo.WinFormsUI.Docking;

using CtLib.Library;
using VehiclePlanner.Module.Interface;
using static VehiclePlanner.Partial.VehiclePlannerUI.Events.ConsoleEvents;

namespace VehiclePlanner.Module.Implement {
    
    /// <summary>
    /// Console介面
    /// </summary>
    public partial class CtConsole : CtDockContent, IConsole
    {

        #region Declaration - Fields
        
        /// <summary>
        /// 執行緒鎖
        /// </summary>
        private readonly object mKey = new object();

        #endregion Declaration - Fields

        #region Funciton - Constructors

        /// <summary>
        /// 共用建構方法
        /// </summary>
        public CtConsole(DockState defState = DockState.Float)
            : base(defState)
        {
            InitializeComponent();
            FixedSize = new Size(424, 300);
        }

        #endregion Function - Constructors

        #region IIConsole
        
        /// <summary>
        /// 文字已被加入
        /// </summary>
        public event DelConsoleAdded ConsoleAddedEvent;

        /// <summary>
        /// 顯示文字被清除
        /// </summary>
        public event DelConsoleCleared ConsoleClearedEvent;

        /// <summary>
        /// 換行並加入字串
        /// </summary>
        public void AddMsg(string msg)
        {
            lock (mKey) {
                txtMsg.InvokeIfNecessary(() => {
                    txtMsg.Text += DateTime.Now.ToString("[hh:mm:ss.fff] ") + msg + "\r\n";
                    txtMsg.SelectionStart = txtMsg.Text.Length;
                    txtMsg.ScrollToCaret();
                });
            }
            ConsoleAddedEvent?.Invoke(msg);
        }

        /// <summary>
        /// 換行並加入字串
        /// </summary>
        public void AddMsg(string format, params object[] arg)
        {
            AddMsg(string.Format(format, arg));
        }

        /// <summary>
        /// 清除顯示
        /// </summary>
        public void ClearMsg()
        {
            lock (mKey)
            {
                txtMsg.InvokeIfNecessary(()=> { if (txtMsg.Text != "") txtMsg.Text = ""; });
            }
            ConsoleClearedEvent();
        }
        #endregion

    }
}
