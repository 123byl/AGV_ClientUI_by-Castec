using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using WeifenLuo.WinFormsUI.Docking;
using CtLib.Library;
using System.Linq;
using System.Threading;
using AGVDefine;

namespace ClientUI
{
    /// <summary>
    /// 測試功能介面
    /// </summary>
    public partial class CtTesting : CtDockContent ,IITesting{

        #region Declaration - Fields

        private readonly object mKey = new object();

        /// <summary>
        /// 紀錄鍵盤按下的方向
        /// </summary>
        private List<MotionDirection> mDirs = new List<MotionDirection>();

        #endregion Declaration - Fields

        #region Function - Construcotrs

        /// <summary>
        /// 共用建構方法
        /// </summary>

        public CtTesting(DockState defState = DockState.Float)
            :base(defState) {
            InitializeComponent();
            FixedSize = new Size(718, 814);
        }

        #endregion Function - Constructors

        #region Implement - ITest

        public event Events.GoalSettingEvents.DelLoadMap LoadMap;
        public event Events.TestingEvents.DelLoadOri LoadOri;
        public event Events.TestingEvents.DelMotion_Down Motion_Down;
        public event Events.TestingEvents.DelMotion_Up Motion_Up;
        public event Events.TestingEvents.DelGetOri GetOri;
        public event Events.TestingEvents.DelGetMap GetMap;
        public event Events.TestingEvents.DelGetLaser GetLaser;
        public event Events.TestingEvents.DelGetCar GetCar;
        public event Events.TestingEvents.DelSendMap SendMap;
        public event Events.TestingEvents.DelSetCarMode SetCarMode;
        public event Events.TestingEvents.DelSimplifyOri SimplifyOri;
        public event Events.TestingEvents.DelSetVelocity SetVelocity;
        public event Events.TestingEvents.DelConnect Connect;
        public event Events.TestingEvents.DelMotorServoOn MotorServoOn;
        public event Events.TestingEvents.DelClearMap ClearMap;
        public event Events.TestingEvents.DelSettingCarPos SettingCarPos;
        public event Events.TestingEvents.DelCarPosConfirm CarPosConfirm;
        public event Events.TestingEvents.DelStartScan StartScan;
        public event Events.TestingEvents.DelShowMotionController ShowMotionController;

        /// <summary>
        /// 依照連線狀態變更UI介面狀態
        /// </summary>
        /// <param name="isConnect"></param>
        public void SetServerStt(bool isConnect) {
            string btnTxt = "Connect AGV";
            Bitmap btnImg = Properties.Resources.Disconnect;
            if (isConnect) {
                btnImg = Properties.Resources.Connect;
                btnTxt = "AGV Connected";
            }
            CtInvoke.ControlTag(btnConnect, isConnect);
            CtInvoke.ControlText(btnConnect, btnTxt);
            CtInvoke.ButtonImage(btnConnect, btnImg);
            //CtInvoke.ControlEnabled(btnGetLaser, isConnect);
            //CtInvoke.ControlEnabled(btnGetOri, isConnect);
            //CtInvoke.ControlEnabled(btnIdleMode, isConnect);
            //CtInvoke.ControlEnabled(btnWorkMode, isConnect);
            //CtInvoke.ControlEnabled(btnMapMode, isConnect);
            //CtInvoke.ControlEnabled(btnGetCarStatus, isConnect);
            //CtInvoke.ControlEnabled(btnPosConfirm, isConnect);
            //CtInvoke.ControlEnabled(btnServoOnOff, isConnect);

            //CtInvoke.ControlEnabled(btnSetVelo, isConnect);
            //CtInvoke.ControlEnabled(btnGetMap, isConnect);
            //CtInvoke.ControlEnabled(btnSendMap, isConnect);
            //CtInvoke.ControlEnabled(txtVelocity, isConnect);

            if (!isConnect) {
                //CarMode = CarMode.OffLine;
                //CtInvoke.ControlEnabled(btnUp, false);
                //CtInvoke.ControlEnabled(btnStartStop, false);
                //CtInvoke.ControlEnabled(btnLeft, false);
                //CtInvoke.ControlEnabled(btnRight, false);
                //CtInvoke.ControlEnabled(btnDown, false);
                //} else if (CarMode == CarMode.OffLine) {
                //    CarMode = CarMode.Idle;
            }
        }

        /// <summary>
        /// 依照雷射自動取得狀態變更UI介面
        /// </summary>
        /// <param name="isGettingLaser"></param>
        public void SetLaserStt(bool isGettingLaser) {
            CtInvoke.ControlBackColor(btnGetCarStatus, isGettingLaser ? Color.Green : Color.Transparent);
        }

        /// <summary>
        /// 是否解鎖Ori檔操作
        /// </summary>
        /// <param name="isUnLock"></param>
        public void UnLockOriOperator(bool isUnLock) {
            CtInvoke.ControlEnabled(btnSimplyOri, isUnLock);
        }

        /// <summary>
        /// 依照馬達狀態變更UI介面
        /// </summary>
        /// <param name="isOn"></param>
        public void ChangedMotorStt(bool isOn) {
            Color color = Color.Empty;
            string content = string.Empty;
            if (isOn) {
                color = Color.Green;
                content = "ON";
            } else {
                color = Color.Red;
                content = "OFF";
            }
            CtInvoke.ControlTag(btnServoOnOff, isOn);
            CtInvoke.ControlBackColor(btnServoOnOff, color);
            CtInvoke.ControlText(btnServoOnOff, content);
        }

        /// <summary>
        /// 設定目標agv IP
        /// </summary>
        /// <param name="host">設定IP</param>
        public void SetHostIP(string host) {
            if (host != cboHostIP.Text) {
                CtInvoke.ControlText(cboHostIP, host);
            }
        }

        /// <summary>
        /// 依照掃圖狀態變更UI介面
        /// </summary>
        /// <param name="isScanning"></param>
        public void ChangedScanStt(bool isScanning) {
            CtInvoke.ControlText(btnScan, isScanning ? "Stop scan" :" Scan");
            CtInvoke.ControlTag(btnScan, isScanning);
        }

        #endregion Implement - ITest

        #region Function  - UI Events

        private void btnConnect_Click(object sender, EventArgs e) {
            if (btnConnect.Tag == null || (btnConnect.Tag is bool && !(bool)btnConnect.Tag)) {
                Connect.Invoke(true, cboHostIP.Text);
            } else {
                Connect.Invoke(false);
            }
        }

        /// <summary>
        /// 移動控制按下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Motion_MouseDown(object sender, MouseEventArgs e) {
            string sDirection = (sender as Control)?.Tag?.ToString();
            int iDirection = 0;
            if (int.TryParse(sDirection, out iDirection) &&
                Enum.IsDefined(typeof(MotionDirection), iDirection)) {
                Motion_Down?.Invoke((MotionDirection)iDirection);
            }
        }

        /// <summary>
        /// 移動控制放開
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Motion_MouseUp(object sender, MouseEventArgs e) {
            Motion_Up?.Invoke();
        }

        private void btnLoadOri_Click(object sender, EventArgs e) {
            LoadOri?.Invoke();
        }
        
        private void btnGetMap_Click(object sender, EventArgs e) {
            GetMap?.BeginInvoke(null,null);
        }

        private void btnLoadMap_Click(object sender, EventArgs e) {
            LoadMap?.Invoke();
        }

        private void btnGetOri_Click(object sender, EventArgs e) {
            GetOri.BeginInvoke(null,null);
        }

        private void btnGetLaser_Click(object sender, EventArgs e) {
            GetLaser?.BeginInvoke(null,null);
        }

        private void btnGetCarStatus_Click(object sender, EventArgs e) {
            GetCar?.BeginInvoke(null,null);
        }

        private void btnSendMap_Click(object sender, EventArgs e) {
            SendMap?.Invoke();
        }

        private void btnMapMode_Click(object sender, EventArgs e) {
            SetCarMode?.Invoke(EMode.Map);
        }

        private void btnWorkMode_Click(object sender, EventArgs e) {
            SetCarMode?.Invoke(EMode.Work);
        }

        private void btnIdleMode_Click(object sender, EventArgs e) {
            SetCarMode?.Invoke(EMode.Idle);
        }

        private void btnSimplyOri_Click(object sender, EventArgs e) {
            SimplifyOri?.Invoke();
        }

        private void btnSetVelo_Click(object sender, EventArgs e) {
            int velocity = 0;
            if (int.TryParse(txtVelocity.Text, out velocity)){
                SetVelocity?.BeginInvoke(velocity,null,null);
            }
        }

        private void btnClrMap_Click(object sender, EventArgs e) {
            ClearMap?.Invoke();
        }

        /// <summary>
        /// 依照連線狀態切換介面控制項致能
        /// </summary>
        /// <param name="isConnected"></param>
        private void ChangedConnectStt(bool isConnected) {
            CtInvoke.ControlEnabled(btnGetLaser, isConnected);
            CtInvoke.ControlEnabled(btnGetOri, isConnected);
            CtInvoke.ControlEnabled(btnIdleMode, isConnected);
            CtInvoke.ControlEnabled(btnWorkMode, isConnected);
            CtInvoke.ControlEnabled(btnMapMode, isConnected);
            CtInvoke.ControlEnabled(btnGetCarStatus, isConnected);
            CtInvoke.ControlEnabled(btnPosConfirm, isConnected);
            CtInvoke.ControlEnabled(btnServoOnOff, isConnected);

            CtInvoke.ControlEnabled(btnSetVelo, isConnected);
            CtInvoke.ControlEnabled(btnGetMap, isConnected);
            CtInvoke.ControlEnabled(btnSendMap, isConnected);
            CtInvoke.ControlEnabled(txtVelocity, isConnected);

        }

        private void btnServoOnOff_Click(object sender, EventArgs e) {
            if (btnServoOnOff.Tag == null || (btnServoOnOff.Tag is bool && !(bool)btnServoOnOff.Tag)) {
                MotorServoOn.BeginInvoke(true,null,null);
            } else {
                MotorServoOn.BeginInvoke(false,null,null);
            }
        }

        private void btnPosConfirm_Click(object sender, EventArgs e) {
            CarPosConfirm?.BeginInvoke(null,null);
        }

        private void btnSetCar_Click(object sender, EventArgs e) {
            SettingCarPos?.BeginInvoke(null,null);
        }

        private void btnScan_Click(object sender, EventArgs e) {
            bool isSacn = btnScan.Tag is bool ? ((bool)btnScan.Tag) : false;
            StartScan?.BeginInvoke(!isSacn,null,null);
        }

        private void btnMotionController_Click(object sender, EventArgs e) {
            ShowMotionController?.Invoke();
        }

        #endregion Funciton - UI Events

    }

    /// <summary>
    /// 車子運動方向
    /// </summary>
    /// <remarks>
    /// 前後左右列舉值為鍵盤方向鍵之對應值
    /// 若是任意更改則會造成鍵盤控制發生例外
    /// </remarks>
    public enum MotionDirection {
        /// <summary>
        /// 往前
        /// </summary>
        Forward = 38,
        /// <summary>
        /// 往後
        /// </summary>
        Backward = 40,
        /// <summary>
        /// 左旋
        /// </summary>
        LeftTrun = 37,
        /// <summary>
        /// 右旋
        /// </summary>
        RightTurn = 39,
        /// <summary>
        /// 停止
        /// </summary>
        Stop = 4
    }

    /// <summary>
    /// Testing事件類型
    /// </summary>
    public enum TestingEventType {
        /// <summary>
        /// Ori檔案路徑變更
        /// </summary>
        CurOriPath,
        GetFile

    }

    /// <summary>
    /// Testing事件參數
    /// </summary>
    public class TestingEventArgs : EventArgs {
        /// <summary>
        /// 事件類型
        /// </summary>
        public TestingEventType Type { get; }
        /// <summary>
        /// 傳遞參數
        /// </summary>
        public object Value { get; }
        /// <summary>
        /// 一般建構方法
        /// </summary>
        /// <param name="type">事件類型</param>
        /// <param name="value">傳遞參數</param>
        public TestingEventArgs(TestingEventType type,object value = null) {
            this.Type = type;
            this.Value = value;
        }
    }

    /// <summary>
    /// Testing事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void TestingEvent(object sender, TestingEventArgs e);

    /// <summary>
    /// Text內容驗證器
    /// </summary>
    public static class TextChecker {

        private class TxtEvent:IDisposable {
            public TextBox Txt;
            public EventHandler LostFocus;
            public KeyPressEventHandler KeyPress;
            public TxtEvent(TextBox txt,Action<object,EventArgs> lostFocus,Action<object,KeyPressEventArgs> keyPress) {
                this.Txt = txt;
                this.LostFocus = new EventHandler(lostFocus);
                this.KeyPress =new KeyPressEventHandler(keyPress);
                this.Txt.LostFocus += this.LostFocus;
                this.Txt.KeyPress += this.KeyPress;
            }

            #region IDisposable Support
            private bool disposedValue = false; // 偵測多餘的呼叫

            protected virtual void Dispose(bool disposing) {
                if (!disposedValue) {
                    if (disposing) {
                        // TODO: 處置 Managed 狀態 (Managed 物件)。
                    }
                    Txt.LostFocus -= this.LostFocus;
                    Txt.KeyPress -= this.KeyPress;
                    Txt = null;
                    this.LostFocus = null;
                    this.KeyPress = null;
                    // TODO: 釋放 Unmanaged 資源 (Unmanaged 物件) 並覆寫下方的完成項。
                    // TODO: 將大型欄位設為 null。

                    disposedValue = true;
                }
            }

            // TODO: 僅當上方的 Dispose(bool disposing) 具有會釋放 Unmanaged 資源的程式碼時，才覆寫完成項。
            // ~TxtEvent() {
            //   // 請勿變更這個程式碼。請將清除程式碼放入上方的 Dispose(bool disposing) 中。
            //   Dispose(false);
            // }

            // 加入這個程式碼的目的在正確實作可處置的模式。
            public void Dispose() {
                // 請勿變更這個程式碼。請將清除程式碼放入上方的 Dispose(bool disposing) 中。
                Dispose(true);
                // TODO: 如果上方的完成項已被覆寫，即取消下行的註解狀態。
                // GC.SuppressFinalize(this);
            }
            #endregion
        }

        private static List<TxtEvent> mTexts = new List<TxtEvent>();

        /// <summary>
        /// 加入驗證清單
        /// </summary>
        /// <remarks>
        /// 用ckFunc與act產生KeyPress與LostFocus的EventHandler
        /// 並且訂閱txt的KeyPress與LostFocus事件
        /// </remarks>
        /// <param name="txt">要驗證的<see cref="TextBox"/></param>
        /// <param name="ckVal">驗證方法</param>
        /// <param name="act">通過驗證後要做的事情</param>
        public static void Add(TextBox txt,Func<string,bool> ckVal,Func<KeyPressEventArgs,bool> ckKey,Action<string> act) {
            /*-- 值驗證方法 --*/
            Action<object> func = obj => {
                TextBox txtBox = obj as TextBox;
                string content = txtBox.Text;
                if (ckVal(content)) {
                    act(content);
                }
            };

            /*-- LostFocus handler --*/
            Action<object, EventArgs> lostFocus = (sender, e) => func(sender);

            /*-- KeyPress handler --*/
            Action<object, KeyPressEventArgs> keyPress = (sender, e) => {
                if (ckKey(e)) {
                    func(sender);
                }
            };

            /*-- 訂閱事件並且加入集合中 --*/
            mTexts.Add(new TxtEvent(
                txt,
                lostFocus,
                keyPress
            ));
        }

        /// <summary>
        /// 加入驗證清單
        /// </summary>
        /// <param name="txt">要驗證的<see cref="TextBox"/></param>
        /// <param name="setting">要寫入的方法</param>
        /// <param name="isFloat">是否為浮點數驗證</param>
        public static void Add(TextBox txt,Action<string> setting,bool isFloat = false) {
            /*-- 驗證方法產生 --*/
            int iVal = 0;
            double dVal = 0d;
            Func<string, bool> ckVal = null;
            if (isFloat) {
                ckVal = str => int.TryParse(str, out iVal);
            }else {
                ckVal = str => double.TryParse(str, out dVal);
            }
            /*-- 控制項事件處理方法 --*/
            Action<object> handler = obj => {
                /*-- 取得輸入字串 --*/
                TextBox txtBox = obj as TextBox;
                string content = txtBox.Text;
                /*-- 數值驗證 --*/
                if (ckVal(content)) {
                    /*-- 寫入變數 --*/
                    setting(content);
                }
            };

            /*-- LostFocus handler --*/
            Action<object, EventArgs> lostFocus = (sender, e) => handler(sender);

            /*-- KeyPress handler --*/
            Action<object, KeyPressEventArgs> keyPress = (sender, e) => {
                /*-- 按鍵檢查 --*/
                if (CheckValue(e,isFloat)) {
                    handler(sender);
                }
            };

            /*-- 訂閱事件並且加入集合中 --*/
            mTexts.Add(new TxtEvent(
                txt,//要處理的TextBox
                lostFocus,//失去焦點事件
                keyPress//按鍵事件
            ));
        }

        /// <summary>
        /// 從驗證清單中移除
        /// </summary>
        /// <param name="txt">要移除的<see cref="TextBox"/></param>
        public static void Remove(TextBox txt) {
            TxtEvent te = mTexts.Find(v => v.Txt == txt);
            if (te != null) {
                te.Dispose();
                mTexts.RemoveAll(v => v == te);
            }
        }

        /// <summary>
        /// 從驗證清單中移除
        /// </summary>
        /// <param name="txts">要移除的<see cref="TextBox"</param>
        public static void Remove(params TextBox[] txts) {
            foreach (TextBox txt in txts) Remove(txt);
        }

        /// <summary>
        /// 限制輸入數值，並在按下Enter時回傳True
        /// </summary>
        /// <param name="e">按鈕事件參數</param>
        /// <returns>是否可以開始進行值驗證</returns>
        public static bool CheckValue(KeyPressEventArgs e ,bool ckFloat) {
            bool ret = false;
            if (((int)e.KeyChar > 47 && (int)e.KeyChar < 58) ||//0~9 
                (int)e.KeyChar == 8 ||//backsapce
                (ckFloat && (int)e.KeyChar == 46))//浮點數驗證且輸入為小數點
                {
                e.Handled = false;
            } else if ((int)e.KeyChar == 13) {  // press the [ENTER] btn.
                ret = true;
            } else {
                e.Handled = true;   // discard this other Keys
            }
            return ret;
        }

    }
    
}
