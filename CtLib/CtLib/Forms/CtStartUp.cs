using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using System.Windows.Forms;

using CtLib.Library;
using CtLib.Module.Ultity;

namespace CtLib.Forms {

    /// <summary>
    /// CASTEC Style 啟動視窗控制
    /// <para>在新的執行緒開啟介面，讓整個啟動視窗動畫可以順利執行</para>
    /// </summary>
    /// <example>
    /// 以下為利用 Enum 方式建立 StartUp 之示範
    /// <code>
    /// enum Step : byte {
    ///     LOADING = 0,
    ///     STEP_1 = 1,
    ///     STEP_2 = 2,
    ///     STEP_3 = 3,
    ///     FINISH = 4
    /// }
    /// 
    /// private void StartUpDemo() {
    ///     Stat stt = Stat.SUCCESS;
    ///     Step step = Step.LOADING;
    ///     CtStartUp startUp = new CtStartUp((int)Step.FINISH + 1);
    ///     try {
    ///         do {
    ///             step++;
    ///             startUp.UpdateProcess(Enum.GetName(typeof(Step), step), (int)step);
    /// 
    ///             switch (step) {
    ///                 case Step.STEP_1:
    ///                     /*-- Do something here --*/
    ///                     break;
    ///                 
    ///                 case Step.STEP_2:
    ///                     /*-- Do something here --*/
    ///                     break;
    ///                 
    ///                 case Step.STEP_3:
    ///                     /*-- Do something here --*/
    ///                     break;
    ///                 
    ///                 default:
    ///                     stt = Stat.ER_SYS_INVIDX;
    ///                     throw (new Exception("流程錯誤"));
    ///             }
    ///         } while (step &gt; Step.FINISH);
    ///     } catch (Exception ex) {
    ///         CtStatus.Report(stt, ex);
    ///     } finally {
    ///         if (startUp != null) startUp.Close();
    ///     }
    /// }
    /// </code></example>
    public class CtStartUp {

        #region Version

        /// <summary>CtStartUp 版本訊息</summary>
        /// <remarks><code>
        /// 1.0.0  Ahern [2014/09/24]
        ///     + 建立基礎模組
        /// </code></remarks>
        public static readonly CtVersion @Version = new CtVersion(1, 0, 0, "2014/09/24", "Ahern Kuo");

        #endregion

        #region Declaration - Members

        /// <summary>最大步數</summary>
        private int mMaxStep = -1;
        /// <summary>[Thread] 開啟CtStartUp_Ctrl介面</summary>
        /// <remarks>另開執行緒執行，避免Form在做其他動作時會卡住</remarks>
        private Thread mThrForm;
        /// <summary>[Flag] 觸發於Thread更新介面</summary>
        private bool mUpdate = false;
        /// <summary>儲存欲更新之訊息</summary>
        private string mInfo = "";
        /// <summary>儲存欲更新之當前步數</summary>
        private int mCurrStep = -1;

        #endregion

        #region Function - Constructor

        /// <summary>啟動CtStartUp_Ctrl，並設定最大步數</summary>
        /// <param name="maxStep">最大步數</param>
        public CtStartUp(int maxStep) {
            mMaxStep = maxStep;

            CtThread.CreateThread(ref mThrForm, "CtStartUp", tsk_Startup);
        }

        #endregion

        #region Function - Core

        /// <summary>更新進度</summary>
        /// <param name="info">訊息</param>
        /// <param name="currStep">當前步數</param>
        public void UpdateProcess(string info, int currStep) {
            mInfo = info;
            mCurrStep = currStep;
            mUpdate = true;
        }

        /// <summary>關閉視窗</summary>
        public void Close() {
            CtThread.KillThread(ref mThrForm);
        }

        /// <summary>[Thread] 建立CtStart_Ctrl，並等待觸發更新</summary>
        private void tsk_Startup() {
            /*-- 建立CtStartUp_Ctrl --*/
            CtStartUp_Ctrl mStartUp = new CtStartUp_Ctrl(mMaxStep);

            try {
                do {
                    /* 如有觸發更新，更新介面 */
                    if (mUpdate) {
                        //將Flag清掉
                        mUpdate = false;
                        //更新介面
                        mStartUp.UpdateProcess(mInfo, mCurrStep);
                    }
                    Thread.Sleep(1);
                    Application.DoEvents();
                } while (mThrForm.IsAlive);
            } catch (ThreadAbortException) {
            } catch (ThreadInterruptedException) {
            } catch (Exception) {
                /*-- 此Try_Catch是用來避免執行緒關閉時會跳Exception的問題，所以Catch裡不做任何事 --*/
            } finally {
                /* 離開後將介面關閉 */
                mStartUp.Close();
            }

        }

        #endregion
    }

}
