﻿using System;
using System.Threading;

using System.Windows.Forms;

using CtLib.Library;
using CtLib.Module.Utility;

namespace CtLib.Forms {

	#region Declaration - Enumerations
	/// <summary>樣式選項</summary>
	public enum ProgBarStyle : byte {
		/// <summary>百分比模式</summary>
		Percent = 0,
		/// <summary>倒數模式，設定好秒數後將自動往下倒數</summary>
		Countdown = 1,
		/// <summary>等待載入，進度條將無限循環</summary>
		Loading = 2
	}
	#endregion

	/// <summary>
	/// 以ProgressBar為主的介面，可用於顯示進度百分比、倒數、Loading
	/// <para>此類別為新增執行緒來執行，可減少因為主執行緒因忙碌而無法讓CtProgress_Ctrl跑很順等問題</para>
	/// </summary>
	/// <example>
	/// 由於 ProgressBar 常用在等待某些事件完成，如是使用原本的執行緒(Thread)有可能因為負擔重而導致介面卡卡的，以下示範如何使用此 Class
	/// <para></para>
	/// <para>1. 進度條</para>
	/// <code language="C#">
	/// CtProgress prog = new CtProgress(ProgBarStyle.Percent, "標題", "說明文字", 47, true);
	/// prog.UpdateStep(29, "Step 29: Information");    //更新百分比，同時更新說明文字
	/// prog.Close();
	/// </code>
	/// 
	/// 2. 倒數
	/// <code language="C#">
	/// CtProgress prog = new CtProgress(ProgBarStyle.Countdown, "標題", "說明文字", 20, true);
	/// prog.Close();
	/// </code>
	/// 
	/// 3. 等待條，即Loading
	/// <code language="C#">
	/// CtProgress prog = new CtProgress("標題", "說明文字");
	/// prog.Close();
	/// </code>
	/// </example>
	public class CtProgress : ICtVersion, IDisposable {

        #region Version

        /// <summary>CtProgress 版本訊息</summary>
        /// <remarks><code language="C#">
        /// 1.0.0  Ahern [2014/09/12]
        ///     + 建立基礎模組
        ///     
        /// 1.0.1  Ahern [2015/02/11]
        ///     \ tsk_Thread 補上 Do-Loop 之離開條件 Form.Visible，用於 Countdown 結束時可以直接離開
        /// 
        /// 1.0.2  Ahern [2015/11/11]
        ///     \ Close 改以 Flag 方式，不強制殺執行緒且讓執行緒跳開關閉視窗就好
		///     
		/// 1.0.3  Ahern [2016/02/28]
		///		+ Inherit IDisposable
        /// 
		/// 1.0.4  Bruce [2016/03/17]
		///		\ Start 補上 mTerminate = false 避免再度開啟時執行緒直接結束
		///		
		/// 1.0.5  Ahern [2016/03/29]
		///		\ 將 Style 改名為 ProgBarStyle 並移至全域
		///		
		/// 1.0.6  Ahern [2017/02/11]
		///		\ Start 合併判斷式，Thread.IsAlive == false 時可直接 Create 不須 Kill
		/// 
        /// </code></remarks>
        public CtVersion Version { get { return new CtVersion(1, 0, 6, "2017/02/11", "Ahern Kuo"); } }

        #endregion

        #region Declaration - Fields

        /// <summary>[Thread] Scanning and update interface</summary>
        private Thread mThread;

        /*-- CtProgress_Ctrl objects --*/
        /// <summary>CtProgress_Ctrl</summary>
        private CtProgress_Ctrl mProgCtrl;
        /// <summary>介面樣式</summary>
        private CtProgress_Ctrl.Style mStyle = CtProgress_Ctrl.Style.Percent;
        /// <summary>標題</summary>
        private string mTitle = "";
        /// <summary>進度描述資訊</summary>
        private string mInfo = "";
        /// <summary>最大數值</summary>
        private float mMaxValue = 0F;
        /// <summary>當前數值</summary>
        private float mCurrValue = 0F;

        /// <summary>[Flag] 更新現在進度百分比</summary>
        private bool mUpdCurr = false;
        /// <summary>[Falg] 更新進度與描述文字</summary>
        private bool mUpdCurrInfo = false;
        /// <summary>[Flag] 更新進度、描述文字與標題</summary>
        private bool mUpdCurrInfoTitle = false;
        /// <summary>[Flag] 指出是否關閉視窗</summary>
        private bool mTerminate = false;
        #endregion

        #region Function - Constructor

        /// <summary>建立控制 CtProgress 之程序</summary>
        /// <param name="style">樣式</param>
        /// <param name="title">介面標題</param>
        /// <param name="info">進度描述資訊</param>
        /// <param name="maxValue">最大範圍。進度: 3/47則此處帶入47F ； 倒數: 從15.3秒開始倒數至零則帶入15.3F</param>
        /// <param name="start">是否直接開始執行</param>
        public CtProgress(ProgBarStyle style, string title, string info, float maxValue = 0F, bool start = true) {
            mStyle = (CtProgress_Ctrl.Style)style;
            mTitle = title;
            mInfo = info;
            mMaxValue = maxValue;

            if (start)
                CtThread.CreateThread(ref mThread, "CtProgress", tsk_Thread);
        }

        /// <summary>建立 Loading 之介面</summary>
        /// <param name="title">介面標題</param>
        /// <param name="info">進度描述資訊</param>
        /// <param name="start">是否直接開始執行</param>
        public CtProgress(string title, string info, bool start = true) {
            mStyle = CtProgress_Ctrl.Style.Loading;
            mTitle = title;
            mInfo = info;
            mMaxValue = 0F;

            if (start)
                CtThread.CreateThread(ref mThread, "CtProgress", tsk_Thread);
        }

		#endregion

		#region Function - Dispose

		/// <summary>關閉相關連線並釋放資源</summary>
		public void Dispose() {
			try {
				Dispose(true);
				GC.SuppressFinalize(this);
			} catch (ObjectDisposedException ex) {
				CtStatus.Report(Stat.ER_SYSTEM, ex);
			}
		}

		/// <summary>關閉相關連線並釋放資源</summary>
		/// <param name="isDisposing">是否第一次釋放</param>
		protected virtual void Dispose(bool isDisposing) {
			try {
				if (isDisposing) {
					mUpdCurr = false;
					mUpdCurrInfo = false;
					mUpdCurrInfoTitle = false;
					mTerminate = true;
				}
			} catch (Exception ex) {
				CtStatus.Report(Stat.ER_SYSTEM, ex);
			}
		}
		#endregion

		#region Function - Core
		/// <summary>開啟CtProgress_Ctrl介面，並等待更新</summary>
		public void Start() {
			mTerminate = false;
            if (mThread == null || !mThread.IsAlive) {
                CtThread.CreateThread(ref mThread, "CtProgress", tsk_Thread);
            }
        }

        /// <summary>更新進度</summary>
		/// <param name="currentStep">當前的步驟，如 "步驟 2，共 5 步驟" 則此處帶入 "2"</param>
        public void UpdateStep(float currentStep) {
            mCurrValue = currentStep;
            mUpdCurr = true;
        }

		/// <summary>更新進度</summary>
		/// <param name="currentStep">當前的步驟，如 "步驟 2，共 5 步驟" 則此處帶入 "2"</param>
		/// <param name="info">此步驟的資訊，會顯示於上方 <see cref="Label"/> 上</param>
		public void UpdateStep(float currentStep, string info) {
            mCurrValue = currentStep;
            mInfo = info;
            mUpdCurrInfo = true;
        }

		/// <summary>更新進度</summary>
		/// <param name="currentStep">當前的步驟，如 "步驟 2，共 5 步驟" 則此處帶入 "2"</param>
		/// <param name="info">此步驟的資訊，會顯示於上方 <see cref="Label"/> 上</param>
		/// <param name="title">標題欄訊息，即整個 <see cref="CtProgress_Ctrl"/> 之標題</param>
		public void UpdateStep(float currentStep, string info, string title) {
            mCurrValue = currentStep;
            mInfo = info;
            mTitle = title;
            mUpdCurrInfoTitle = true;
        }

		/// <summary>關閉 <see cref="CtProgress_Ctrl"/> 介面</summary>
		public void Close() {
            mUpdCurr = false;
            mUpdCurrInfo = false;
            mUpdCurrInfoTitle = false;
            mTerminate = true;
        }

        /// <summary>[Thread] 掃瞄並等待更新</summary>
        private void tsk_Thread() {
            mProgCtrl = new CtProgress_Ctrl(mStyle, mTitle, mInfo, mMaxValue);
            try {
                mProgCtrl.Start();

                while (!mTerminate) {

                    if (mUpdCurr) {
                        mUpdCurr = false;
                        mProgCtrl.UpdateStep(mCurrValue);
                    }

                    if (mUpdCurrInfo) {
                        mUpdCurrInfo = false;
                        mProgCtrl.UpdateStep(mCurrValue, mInfo);
                    }

                    if (mUpdCurrInfoTitle) {
                        mUpdCurrInfoTitle = false;
                        mProgCtrl.UpdateStep(mCurrValue, mInfo, mTitle);
                    }

                    CtTimer.Delay(10);
                    Application.DoEvents();
                }

            } catch (Exception) {
                /*-- 這裡是用來避免執行緒關閉時跳的exception，故為空 --*/
            } finally {
                if (mProgCtrl != null) {
                    if (mProgCtrl.Visible) mProgCtrl.Terminate();
                    mProgCtrl.Dispose();
                }
            }
        }
        #endregion
    }
}
