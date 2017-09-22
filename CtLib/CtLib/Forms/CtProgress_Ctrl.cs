using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

using CtLib.Library;
using CtLib.Module.Ultity;

namespace CtLib.Forms {
    /// <summary>
    /// 以ProgressBar為主的介面，可用於顯示進度百分比、倒數、Loading
    /// <para>因如果直接ShowDialog，將導致顯示會頓頓的，已新增CtProgress.cs改善</para>
    /// <para>建議可以使用CtProgress取代直接呼叫CtProgress_Ctrl</para>
    /// </summary>
    internal partial class CtProgress_Ctrl : Form {

        #region Version

        /// <summary>CtProgress 版本訊息</summary>
        /// <remarks><code>
        /// 1.0.0  Ahern [2014/09/12]
        ///     + 建立基礎模組
        ///     
        /// 1.0.1  Ahern [201410/28]
        ///     + Logo
        ///     \ 統一Enumeration大小寫
        /// </code></remarks>
        public static readonly CtVersion @Version = new CtVersion(1, 0, 1, "2014/1/28", "Ahern Kuo");

        #endregion

        #region Declaration - Enumeration

        /// <summary>樣式選項</summary>
        public enum Style : byte {
            /// <summary>百分比模式</summary>
            PERCENT = 0,
            /// <summary>倒數模式，設定好秒數後將自動往下倒數</summary>
            COUNTDOWN = 1,
            /// <summary>等待載入，進度條將無限循環</summary>
            LOADING = 2
        }

        #endregion

        #region Declaration - Properties

        /// <summary>是否已達到最大值，或是倒數完畢</summary>
        public bool IsFinished {
            get;
            set;
        }

        #endregion

        #region Declaration - Members

        /// <summary>紀錄當前樣式</summary>
        private Style mStyle = Style.LOADING;
        /// <summary>百分比進度之當前進度</summary>
        private float mCurrStep = 0;
        /// <summary>百分比進度之最大進度</summary>
        private float mMaxStep = 0;
        /// <summary>用於倒數計時之計時器(Timer)</summary>
        private System.Threading.Timer mCdTmr;

        #endregion

        #region Function - Methods

        /// <summary>計算當前進度百分比。(當前進度/最大百分比)*100%</summary>
        /// <returns>進度百分比</returns>
        private float CalcPercent() {
            float sngTemp = -1.0F;
            try {
                sngTemp = (mCurrStep / mMaxStep) * 100F;
            } catch (Exception ex) {
                CtStatus.Report(Stat.ER_SYSTEM, "CalcPercent", ex.Message);
            }
            return sngTemp;
        }

        /// <summary>[Callback] 倒數計時之計時器Callback</summary>
        /// <param name="State">物件之狀態。此處會自動帶入建立Timer時的State數值</param>
        private void tmrCallback(object State) {
            try {
                mCurrStep -= 0.01F;
                if (mCurrStep <= 0F) {
                    IsFinished = true;
                    this.DialogResult = System.Windows.Forms.DialogResult.Abort;
                    CtInvoke.FormClose(this);
                    mCdTmr.Dispose();
                } else UpdateState();
            } catch (Exception ex) {
                CtStatus.Report(Stat.ER_SYSTEM, "tmrCallback", ex.Message);
            }
        }

        #endregion

        #region Function - Core
        /// <summary>建立一個具ProgressBar之介面</summary>
        /// <param name="sty">樣式，包含進度百分比/倒數計時/Loading</param>
        /// <param name="title">表單標題</param>
        /// <param name="caption">該項目敘述文字</param>
        /// <param name="maxValue">最大範圍。進度: 3/47則此處帶入47F ； 倒數: 從15.3秒開始倒數至零則帶入15.3F</param>
        public CtProgress_Ctrl(Style sty, string title, string caption, float maxValue = 0F) {
            InitializeComponent();

            mStyle = sty;
            CtInvoke.FormText(this, title);
            CtInvoke.LabelText(lbCaption, caption);

            switch (sty) {
                case Style.PERCENT:
                    mCurrStep = 0F;
                    mMaxStep = maxValue;
                    CtInvoke.LabelText(lbPercent,"0.00%");
                    CtInvoke.ProgressBarRange(progProcess, 0, 100);
                    CtInvoke.ProgressBarStyle(progProcess, ProgressBarStyle.Continuous);
                    break;
                case Style.COUNTDOWN:
                    mMaxStep = maxValue;
                    mCurrStep = maxValue;
                    CtInvoke.LabelText(lbPercent, mMaxStep.ToString("##0.0s"));
                    CtInvoke.ProgressBarRange(progProcess, 0, 100);
                    CtInvoke.ProgressBarStyle(progProcess, ProgressBarStyle.Continuous);
                    break;
                case Style.LOADING:
                    CtInvoke.LabelVisible(lbPercent, false);
                    CtInvoke.ProgressBarStyle(progProcess, ProgressBarStyle.Marquee);
                    break;
                default:
                    break;
            }
        }

        /// <summary>啟動介面
        /// 倒數計時 → 將會立即顯示介面並開始倒數
        /// 進度百分比 → 請先設定好當前進度，否則以預設0%顯示
        /// 載入 → 立即顯示介面</summary>
        /// <returns>Status Code</returns>
        public Stat Start() {
            Stat stt = Stat.SUCCESS;
            try {
                IsFinished = false;
                if (mStyle == Style.COUNTDOWN) {
                    mCdTmr = new System.Threading.Timer(new TimerCallback(tmrCallback), "CtProg_Countdown", 10, 10);
                }
                this.Show();
            } catch (Exception ex) {
                stt = Stat.ER_SYSTEM;
                CtStatus.Report(stt, "Start", ex.Message);
            }
            return stt;
        }

        /// <summary>更新進度</summary>
        private void UpdateState() {
            try {
                if (mStyle == Style.COUNTDOWN) {
                    CtInvoke.LabelText(lbPercent, mCurrStep.ToString("##0.0") + "s");
                    CtInvoke.ProgressBarValue(progProcess, (int) CalcPercent());
                } else if (mStyle == Style.PERCENT) {
                    float sngCurVal = CalcPercent();
                    CtInvoke.LabelText(lbPercent, sngCurVal.ToString("##0.00") + "%");
                    CtInvoke.ProgressBarValue(progProcess, (int) sngCurVal);
                }
                Application.DoEvents();
            } catch (Exception ex) {
                CtStatus.Report(Stat.ER_SYSTEM, "UpdateState", ex.Message);
            }
        }

        /// <summary>更新進度</summary>
        /// <param name="currentStep">當前進度。如 5/47，則輸入5F</param>
        /// <returns>Status Code</returns>
        public Stat UpdateStep(float currentStep) {
            Stat stt = Stat.SUCCESS;
            try {
                mCurrStep = currentStep;
                UpdateState();
            } catch (Exception ex) {
                stt = Stat.ER_SYSTEM;
                CtStatus.Report(stt, "UpdateStep", ex.Message);
            }
            return stt;
        }

        /// <summary>更新進度</summary>
        /// <param name="currentStep">當前進度。如 5/47，則輸入5F</param>
        /// <param name="caption">目前項目敘述</param>
        /// <returns>Status Code</returns>
        public Stat UpdateStep(float currentStep, string caption) {
            Stat stt = Stat.SUCCESS;
            try {
                CtInvoke.LabelText(lbCaption, caption);
                mCurrStep = currentStep;
                UpdateState();
            } catch (Exception ex) {
                stt = Stat.ER_SYSTEM;
                CtStatus.Report(stt, "UpdateStep_cap", ex.Message);
            }
            return stt;
        }

        /// <summary>更新進度</summary>
        /// <param name="currentStep">當前進度。如 5/47，則輸入5F</param>
        /// <param name="caption">目前項目敘述</param>
        /// <param name="title">更改表單(Form)的標題</param>
        /// <returns>Status Code</returns>
        public Stat UpdateStep(float currentStep, string caption, string title) {
            Stat stt = Stat.SUCCESS;
            try {
                CtInvoke.FormText(this, title);
                CtInvoke.LabelText(lbCaption, caption);
                mCurrStep = currentStep;
                UpdateState();
            } catch (Exception ex) {
                stt = Stat.ER_SYSTEM;
                CtStatus.Report(stt, "UpdateStep_tit", ex.Message);
            }
            return stt;
        }

        /// <summary>關閉表單並停止計時</summary>
        public void Terminate() {
            try {
                IsFinished = true;
                this.DialogResult = System.Windows.Forms.DialogResult.Abort;
                this.Close();
                if (mCdTmr != null) mCdTmr.Dispose();
            } catch (Exception ex) {
                CtStatus.Report(Stat.ER_SYSTEM, "Terminate", ex.Message);
            }
        }

        #endregion

    }
}
