using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

using WeifenLuo.WinFormsUI.Docking;

using System.Threading;
using CtLib.Library;
using CtLib.Forms;

using CAMPro.Module;
namespace ClientUI {

    /// <summary>
    /// Testing功能接口
    /// </summary>
    public interface ITesting {

        #region Properties

        /// <summary>
        /// AGV IP
        /// </summary>
        string HostIP { get; set; }

        bool IsConnected { get; set; }
        /// <summary>
        /// 車子馬達速度
        /// </summary>
        int Velocity { get; set; }
        
        /// <summary>
        /// 伺服端是否還有在運作
        /// </summary>
        /// <remarks>
        /// Modified by Jay 2017/09/14
        /// 改為回傳上一次呼叫CheckIsServerAlive的結果
        /// </remarks>
        bool IsServerAlive { get;}

        /// <summary>
        /// 車子模式
        /// </summary>
        CarMode CarMode { get; set; }

        /// <summary>
        /// 預設地圖檔路徑
        /// </summary>
        string DefMapDir { get; }

        #endregion Properties

        #region Events

        /// <summary>
        /// Testing相關事件發報
        /// </summary>
        event TestingEvent TestingEventTrigger;

        #endregion Events

        #region Methods

        /// <summary>
        /// 取得是否為停止模式
        /// </summary>
        /// <returns></returns>
        bool GetStopMode();

        /// <summary>
        /// 取得伺服馬達激磁狀態
        /// </summary>
        /// <returns></returns>
        bool GetMotorStatus();

        /// <summary>
        /// 設定車子移動速度
        /// </summary>
        /// <param name="velocity">移動速度</param>
        void SetVelocity(int velocity);
        
        /// <summary>
        /// 向Server端要求檔案
        /// </summary>
        /// <param name="type">檔案類型</param>
        /// <param name="fileList">檔案清單</param>
        /// <returns>是否有得到檔案清單</returns>
        bool GetFileList(FileType type, out string fileList);

        /// <summary>
        /// 檢查Server是否在運作中
        /// </summary>
        /// <returns></returns>
        bool CheckIsServerAlive();
        
        /// <summary>
        /// 地圖簡化
        /// </summary>
        void SimplifyOri();

        /// <summary>
        /// 取得雷射
        /// </summary>
        void GetLaser();

        /// <summary>
        /// 車子資訊回傳開關
        /// </summary>
        /// <param name="on"></param>
        bool ChangeSendInfo();

        /// <summary>
        /// 清除Map
        /// </summary>
        void ClearMap();
        
        /// <summary>
        /// 移動控制
        /// </summary>
        /// <param name="direction">移動方向</param>
        /// <param name="velocity">移動速度</param>
        void MotionContorl(MotionDirection direction, int velocity = 0);

        /// <summary>
        /// 馬達Servo On/Off
        /// </summary>
        /// <param name="on">是否進行馬達ServerOn</param>
        void MotorServo(bool on);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        void StartStop(bool start);

        /// <summary>
        /// 地圖修正
        /// </summary>
        void CorrectOri();

        /// <summary>
        /// 確認車子位置
        /// </summary>
        void CarPosConfirm();

        /// <summary>
        /// 設定GL模式
        /// </summary>
        /// <param name="mode">GL模式</param>
        void SetGLMode(GLMode mode);

        /// <summary>
        /// 傳送地圖
        /// </summary>
        void SendMap();
        
        /// <summary>
        /// 檔案下載
        /// </summary>
        /// <param name="fileName">要下載的檔案名稱</param>
        /// <param name="type">檔案類型</param>
        void FileDownload(string fileName, FileType type);

        /// <summary>
        /// 載入檔案
        /// </summary>
        /// <param name="type">要載入的檔案類型</param>
        void LoadFile(FileType type);

        /// <summary>
        /// 向AGV要求檔案
        /// </summary>
        /// <param name="type">檔案類型</param>
        void GetFile(FileType type); 

        #endregion Methods

        }

    /// <summary>
    /// 測試功能介面
    /// </summary>
    public partial class CtTesting : CtDockContent<ITesting> {

        #region Declaration - Fields

        /// <summary>
        /// 車子移動速度，rActFunc無參考時使用
        /// </summary>
        private int mVelocity = 0;
        
        /// <summary>
        /// 車子模式，rActFunc無參考時使用
        /// </summary>
        private CarMode mCarMode = CarMode.OffLine;

        /// <summary>
        /// 預設地圖檔路徑，rActFunc無參考時使用
        /// </summary>
        private string mDefMapDir = @"D:\MapInfo\";

        /// <summary>
        /// 進度條物件
        /// </summary>
        private CtProgress mProg = null;

        #endregion Declaration - Fields

        #region Declaration - Properties

        /// <summary>
        /// 預設地圖檔資料夾路徑
        /// </summary>
        private string DefMapDir { get { return rActFunc?.DefMapDir ?? mDefMapDir; } }

        /// <summary>
        /// 車子移動速度
        /// </summary>
        public int Velocity {
            get {
                return rActFunc?.Velocity ?? mVelocity;
            }set {
                if (rActFunc != null) {
                    rActFunc.Velocity = value;
                }else {
                    mVelocity = value;
                }
            }
        }

        /// <summary>
        /// 車子模式
        /// </summary>
        private CarMode CarMode {
            get {
                return rActFunc?.CarMode ?? mCarMode;
            }
            set {
                if (value != CarMode.OffLine && value != CarMode) {
                    /*-- 切換不同模式前先解除原本按鈕的鎖定 --*/
                    SetBtnModeEnable(CarMode, true);
                }
                if (rActFunc != null) {
                    rActFunc.CarMode = value;
                }else {
                    mCarMode = value;
                }
                /*-- 切換模式後，鎖住對應的按鈕 --*/
                SetBtnModeEnable(CarMode, false);

                CtInvoke.ButtonText(btnScan, CarMode == CarMode.Map ?  "Stop scanning": "Scan");
            }
        }

        /// <summary>
        /// 依照指定Enable狀態設定對應按鈕狀態
        /// </summary>
        /// <param name="mode">車子模式</param>
        /// <param name="enable">是否可致能</param>
        private void SetBtnModeEnable(CarMode mode,bool enable) {
            Button btnMode = null;
            RadioButton rdbMode = null;
            switch (mode) {
                case CarMode.Idle:
                    btnMode = btnIdleMode;
                    rdbMode = rdbIdle;
                    break;
                case CarMode.Map:
                    btnMode = btnMapMode; 
                    rdbMode = rdbMap;
                    break;
                case CarMode.Work:
                    btnMode = btnWorkMode;
                    rdbMode = rdbWork;
                    break;
                default:
                    return;
            }
            if (!enable) CtInvoke.RadioButtonChecked(rdbMode, true);
            CtInvoke.ButtonEnable(btnMode, enable);
        }

        #endregion Declaration - Properties

        #region Function - Construcotrs

        /// <summary>
        /// 空白建構方法
        /// </summary>
        /// <param name="defState"></param>
        public CtTesting(DockState defState = DockState.Float) : this(null, null, defState) {

        }

        /// <summary>
        /// 傳入<see cref="ITesting"/>參考進行建構
        /// </summary>
        ///  <param name="defState">預設的停靠狀態，不可為Unknown</param>
        ///  <param name="testing">Testing方法實作物件參考</param>
        public CtTesting(ITesting testing, DockState defState = DockState.Float)
            :this(testing,null,defState) {
        }
        
        /// <summary>
        /// 共用建構方法
        /// </summary>
        /// <param name="testing">Testing方法實作物件參考</param>
        /// <param name="main">主介面參考</param>
        /// <param name="defState">預設停靠狀態，不可為Unknown</param>
        public CtTesting(ITesting testing, AgvClientUI main,DockState defState = DockState.Float)
            :base(testing,main,defState) {
            InitializeComponent();
            FixedSize = new Size(718, 814);
            int velocity = 0;
            if (int.TryParse(txtVelocity.Text, out velocity) && rActFunc != null) {
                rActFunc.Velocity = velocity;
            }

            rdbIdle.Tag = $"{CarMode.Idle}";
            rdbWork.Tag = $"{CarMode.Work}";
            rdbMap.Tag = $"{CarMode.Map}";
            
        }

        #endregion Function - Constructors

        #region Function - Events

        #region rActFunc

        private void rActFunc_OnTestingEventTrigger(object sender, TestingEventArgs e) {
            switch (e.Type) {
                case TestingEventType.CurOriPath:
                    ChangedOriPath();
                    break;
                case TestingEventType.GetFile:
                    mProg?.Close();
                    mProg = null;
                    break;
            }
        }

        #endregion rActFunc

        #region Button

        private void btnPower_Click(object sender, EventArgs e) {
            /*-- 從底層觸發 --*/
            //rActFunc.SetGLMode(GLMode.Power);
            /*-- 從介面直接調用 --*/
            rMain.SetGLMode(GLMode.Power);
        }

        private void btnCursorMode_Click(object sender, EventArgs e) {
            /*-- 從底層觸發 --*/
            //rActFunc.SetGLMode(GLMode.Cursor);

            /*-- 從介面直接調用 --*/
            rMain.SetGLMode(GLMode.Cursor);
        }

        /// <summary>
        /// 傳送地圖
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private  void btnSendMap_Click(object sender, EventArgs e) {
            rActFunc.SendMap();
        }

        /// <summary>
        /// 地圖化簡
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnSimplifyOri_Click(object sender, EventArgs e) {
            CtProgress prog = new CtProgress("SimplifyOri","SimplifyOri");
            try {
                /*-- 從底層觸發 --*/
                await Task.Run(() => rActFunc.SimplifyOri());
            } catch {

            } finally {
                prog?.Close();
                prog = null;
            }
        }

        /// <summary>
        /// 載入地圖
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLoadMap_Click(object sender, EventArgs e) {
            rActFunc.LoadFile(FileType.Map);
        }

        /// <summary>
        /// 取得地圖檔
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGetMap_Click(object sender, EventArgs e) {
            GetFile(FileType.Map);
        }

        /// <summary>
        /// 移動控制按下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Motion_MouseDown(object sender,MouseEventArgs e) {
            string sDirection = (sender as Control)?.Tag?.ToString();
            int iDirection = 0;
            if (int.TryParse(sDirection,out iDirection) && 
                Enum.IsDefined(typeof(MotionDirection), iDirection)){
                rActFunc.MotionContorl((MotionDirection)iDirection,Velocity);
                if (CarMode != CarMode.Map) CarMode = CarMode.Work;
            }
        }

        /// <summary>
        /// 移動控制放開
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Motion_MouseUp(object sender,MouseEventArgs e) {
            rActFunc.MotionContorl(MotionDirection.Stop);
            if (CarMode != CarMode.Map) CarMode = CarMode.Idle;
        }

        /// <summary>
        /// 取得雷射
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGetLaser_Click(object sender, EventArgs e) {
            rActFunc.GetLaser();
        }

        /// <summary>
        /// 車子資訊回傳開關
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGetCarStatus_Click(object sender, EventArgs e) {
            bool isOn =  rActFunc.ChangeSendInfo();
            CtInvoke.ButtonBackColor(btnGetCarStatus, isOn ? Color.Green : Color.Transparent);
        }

        /// <summary>
        /// 清除地圖
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClrMap_Click(object sender, EventArgs e) {
            ClearMap();
        }

        /// <summary>
        /// 下載地圖
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGetOri_Click(object sender, EventArgs e) {
            GetFile(FileType.Ori);
        }

        private void GetFile(FileType type) {
            string fileList = string.Empty;
            if (rActFunc.GetFileList(type, out fileList)) {
                using (MapList f = new MapList(fileList)) {
                    if (f.ShowDialog() == DialogResult.OK) {
                        mProg = new CtProgress($"Get {type}", $"Donwloading {type} from AGV");
                        rActFunc.FileDownload(f.strMapList, type);
                    }
                }
            }
        }

        /// <summary>
        /// 載入地圖
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLoadOri_Click(object sender, EventArgs e) {
            rActFunc.LoadFile(FileType.Ori);
        }

        /// <summary>
        /// 設定移動速度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSetVelo_Click(object sender, EventArgs e) {
            rActFunc.MotionContorl(MotionDirection.Forward, Velocity);
        }

        /// <summary>
        /// 馬達ServoOn控制
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnServoOnOff_Click(object sender, EventArgs e) {
            Color btnColor = Color.Empty;
            bool on = btnServoOnOff.Text != "OFF";
            string text = string.Empty;
            if (!on) {
                btnColor = Color.Green;
                text = "ON";
            } else {
                btnColor = Color.Red;
                text = "OFF";
            }
            CtInvoke.ButtonBackColor(btnServoOnOff, btnColor);
            CtInvoke.ButtonText(btnServoOnOff, text);
            
            rActFunc.MotorServo(on);
            ChangedServoStt(on);
        }

        /// <summary>
        /// 持續移動開關
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStartStop_Click(object sender, EventArgs e) {
            bool start = btnStartStop.Tag?.ToString() == "Stop";
            string tag = string.Empty;
            Bitmap img = null;
            if (start) {
                tag = "Start";
                img =  Properties.Resources.Stop;
            } else {
                tag = "Stop";
                img = Properties.Resources.play;
            }
            CtInvoke.ButtonTag(btnStartStop, tag);
            CtInvoke.ButtonImage(btnStartStop, img);
            rActFunc.StartStop(start);
        }

        /// <summary>
        /// 與Server連線
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnConnect_Click(object sender, EventArgs e) {
            string btnTxt = "Connect AGV";
            Bitmap btnImg = Properties.Resources.Disconnect;

            CtInvoke.ButtonEnable(btnConnect, false);
            bool isConnect = false;

            if (btnConnect.Text == "Connect AGV") {
                CtProgress prog = new CtProgress("Connect", "Connecting...");
                try {
                    isConnect = await Task<bool>.Run(() => rActFunc.CheckIsServerAlive());
                    if (isConnect) {
                        btnImg = Properties.Resources.Connect;
                        btnTxt = "AGV Connected";
                    }
                } finally {
                    prog?.Close();
                    prog = null;
                }
            }
            ChangedConnectStt(isConnect);
            CtInvoke.ButtonText(btnConnect, btnTxt);
            CtInvoke.ButtonImage(btnConnect, btnImg);
            CtInvoke.ButtonEnable(btnConnect, true);
            rActFunc.IsConnected = isConnect;
        }

        /// <summary>
        /// 地圖修正
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnCorrectOri_Click(object sender, EventArgs e) {
            CtProgress prog = new CtProgress("CorrectOri", "Correcting Ori...");
            try {
                /*-- 底層觸發事件 --*/
                await Task.Run(() => rActFunc.CorrectOri());
            } catch {
            } finally {
                prog?.Close();
                prog = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSetCar_Click(object sender, EventArgs e) {
            /*-- 介面直接調用 --*/
            rMain.SetGLMode(GLMode.SetCar);
            
            rActFunc.GetLaser();
        }

        /// <summary>
        /// 確認車子位置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnPosConfirm_Click(object sender, EventArgs e) {
            CtProgress prog = new CtProgress("Car position Confirm", "Car position Confirm");
            try {
                /*-- 從底層觸發 --*/
                await Task.Run(() => rActFunc.CarPosConfirm());
            } catch {

            } finally {
                prog?.Close();
                prog = null;
            }

        }
        
        /// <summary>
        /// 切換為掃圖模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMapMode_Click(object sender, EventArgs e) {
            CarMode = CarMode.Map;
        }

        private void btnWorkMode_Click(object sender, EventArgs e) {
            CarMode = CarMode.Work;
        }

        private void btnIdleMode_Click(object sender, EventArgs e) {
            CarMode = CarMode.Idle;
        }

        private void btnErase_Click(object sender, EventArgs e) {
            /*-- 從底層觸發 --*/
            //rActFunc.SetGLMode(GLMode.Erase);

            /*-- 從主介面直接調用 --*/
            rMain.SetGLMode(GLMode.Erase);
        }

        private void btnStop_Click(object sender, EventArgs e) {
            /*-- 從底層觸發 --*/
            //rActFunc.SetGLMode(GLMode.Stop);

            /*從主介面直接調用*/
            rMain.SetGLMode(GLMode.Stop);
        }

        #endregion Button

        #region RadioButton

        /// <summary>
        /// 車子模式切換
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdbMode_OnCheckedChanged(object sender,EventArgs e) {
            RadioButton rdbMode = (sender as RadioButton);
            CarMode mode = CarMode.OffLine;

            if ((!rdbMode?.Checked ?? false) ||//沒有選取 
                !(rdbMode?.Tag is string) ||//Tag不為字串
                !Enum.TryParse(rdbMode.Tag.ToString(), out mode)) {//無法轉換為CarMode
                return;
            }

            if (CarMode != mode) {
                CarMode = mode;
                SetBtnModeEnable(CarMode, false);
            }
        }

        #endregion RadioButton

        #endregion Funciton - Events

        #region Function - Public Methods

        #endregion Function - Public Methods

        #region Function - Private Methdos

        /// <summary>
        /// 分配方法物件
        /// </summary>
        /// <param name="actFunc">方法物件</param>
        protected override void AssignmentActFunc(ITesting actFunc) {
            base.AssignmentActFunc(actFunc);
            if (actFunc != null) {
                CtInvoke.ComboBoxAdd(cboHostIP, rActFunc.HostIP);
                CtInvoke.ComboBoxText(cboHostIP, rActFunc.HostIP);
            }
        }

        /// <summary>
        /// Ori載入完畢
        /// </summary>
        private void ChangedOriPath() {
            CtInvoke.ButtonEnable(btnCorrectOri, true);
            CtInvoke.ButtonEnable(btnSimplyOri, true);
        }

        /// <summary>
        /// 清除Map
        /// </summary>
        private void ClearMap() {
            /*-- 呼叫底層觸發MapGL模組清除Map --*/
            rActFunc.ClearMap();

            /*-- 從主介面參考方法調用MapGL模組清除Map --*/
            rMain.ClearMap();
        }
        
        /// <summary>
        /// 訂閱事件
        /// </summary>
        protected override void AddEvent() {
            /*-- 自動產生LostFoucs與KeyPress處理方法進行訂閱 --*/
            TextChecker.Add(txtVelocity,str => Velocity = int.Parse(str));
            if (rActFunc != null) {
                rActFunc.TestingEventTrigger += rActFunc_OnTestingEventTrigger;
            }
        }

        /// <summary>
        /// 取消訂閱事件
        /// </summary>
        protected override void RemoveEvent() {
            TextChecker.Remove(txtVelocity);
            if (rActFunc != null) {
                rActFunc.TestingEventTrigger -= rActFunc_OnTestingEventTrigger;
            }
        }

        /// <summary>
        /// 依照馬達狀態切換介面控制項致能
        /// </summary>
        /// <param name="isOn">是否ServoOn</param>
        private void ChangedServoStt(bool isOn) {
            CtInvoke.ButtonEnable(btnUp, isOn);
            CtInvoke.ButtonEnable(btnDown, isOn);
            CtInvoke.ButtonEnable(btnLeft, isOn);
            CtInvoke.ButtonEnable(btnRight, isOn);
            CtInvoke.ButtonEnable(btnStartStop, isOn);
        }

        /// <summary>
        /// 依照連線狀態切換介面控制項致能
        /// </summary>
        /// <param name="isConnected"></param>
        private void ChangedConnectStt(bool isConnected) {
            CtInvoke.ButtonEnable(btnGetLaser, isConnected);
            CtInvoke.ButtonEnable(btnGetOri, isConnected);
            CtInvoke.ButtonEnable(btnIdleMode, isConnected);
            CtInvoke.ButtonEnable(btnWorkMode, isConnected);
            CtInvoke.ButtonEnable(btnMapMode, isConnected);
            CtInvoke.ButtonEnable(btnGetCarStatus, isConnected);
            CtInvoke.ButtonEnable(btnPosConfirm, isConnected);
            CtInvoke.ButtonEnable(btnServoOnOff, isConnected);
           
            CtInvoke.ButtonEnable(btnSetVelo, isConnected);
            CtInvoke.ButtonEnable(btnGetMap, isConnected);
            CtInvoke.ButtonEnable(btnSendMap, isConnected);
            CtInvoke.TextBoxEnable(txtVelocity, isConnected);
            
            if (!isConnected) {
                CarMode = CarMode.OffLine;
                CtInvoke.ButtonEnable(btnUp, false);
                CtInvoke.ButtonEnable(btnStartStop, false);
                CtInvoke.ButtonEnable(btnLeft, false);
                CtInvoke.ButtonEnable(btnRight, false);
                CtInvoke.ButtonEnable(btnDown, false);
            } else if (CarMode == CarMode.OffLine) {
                CarMode = CarMode.Idle;
            }
        }

        #endregion Function - Private Methods

        private void btnScan_Click(object sender, EventArgs e) {
            if (CarMode != CarMode.Map) {
                CarMode = CarMode.Map;
            }else {
                CarMode = CarMode.Idle;
            }
            
        }

    }

    /// <summary>
    /// 車子運動方向
    /// </summary>
    public enum MotionDirection {
        /// <summary>
        /// 往前
        /// </summary>
        Forward = 0,
        /// <summary>
        /// 往後
        /// </summary>
        Backward = 1,
        /// <summary>
        /// 左旋
        /// </summary>
        LeftTrun = 2,
        /// <summary>
        /// 右璇
        /// </summary>
        RightTurn = 3,
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
