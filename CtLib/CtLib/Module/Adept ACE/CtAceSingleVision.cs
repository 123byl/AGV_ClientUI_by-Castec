using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using CtLib.Library;

using Ace.HSVision.Client.ImageDisplay;
using Ace.HSVision.Server.Tools;
using Ace.HSVision.Server.Integration;
using CtLib.Module.Ultity;

namespace CtLib.Module.Adept {
    /// <summary>顯示單一 VisionTool 畫面</summary>
    /// <example><code>
    /// /*-- Create and connect Adept ACE --*/
    /// CtAce mAce = new CtAce();
    /// mAce.Connect(CtAce.ControllerType.WITH_SMARTCONTROLLER);
    /// 
    /// /*-- Create an object --*/
    /// CtAceSingleVision singleVision = new CtAceSingleVision();   //New a form or drag it into form from Visualize Utility
    /// 
    /// /*-- Select the tool that you want to show --*/
    /// CtAceSingleVision.ToolType toolType = CtAceSingleVision.ToolType.VirtualCamera;
    /// string visPath = @"/Equipment/Vision/Device/Virtual Camera";
    /// 
    /// /*-- Show it --*/
    /// singleVision.Connect(toolType, mAce, visPath);
    /// </code></example>
    public partial class CtAceSingleVision : UserControl {

        #region Version

        /// <summary>CtAce 版本訊息</summary>
        public static readonly CtVersion @Version = new CtVersion(1, 0, 0, "2014/09/10", "Ahern Kuo");

        /*---------- Version Note ----------
         * 
         * 1.0.0  <Ahern> [2014/09/10]
         *      + 建立Connect模組
         *      
         *----------------------------------*/

        #endregion

        #region Declaration - Properties

        /// <summary>是否顯示尺標</summary>
        [DefaultValue(false)]
        public bool OPTION_RULER {
            get;
            set;
        }

        /// <summary>是否顯示滑條</summary>
        [DefaultValue(false)]
        public bool OPTION_SCROLLBAR {
            get;
            set;
        }

        /// <summary>是否顯示工作列</summary>
        [DefaultValue(false)]
        public bool OPTION_TOOLBAR {
            get;
            set;
        }

        /// <summary>是否顯示狀態列</summary>
        [DefaultValue(false)]
        public bool OPTION_STATUSBAR {
            get;
            set;
        }

        /// <summary>是否顯示執行時間</summary>
        [DefaultValue(false)]
        public bool OPTION_EXECTIME {
            get;
            set;
        }

        /// <summary>是否顯示自動清除</summary>
        [DefaultValue(true)]
        public bool OPTION_AUTOCLEAR {
            get;
            set;
        }

        #endregion

        #region Declaration - Enumeration

        /// <summary>VisionTool類型</summary>
        public enum ToolType : byte {
            /// <summary>攝影機(Virtual Camera)</summary>
            VirtualCamera = 0,
            /// <summary>定位點工具(Locator)</summary>
            Locator = 1,
            /// <summary>自定義影像工具(Custom Vision Tool, CVT)</summary>
            CSharpCustomTool = 2,
            /// <summary>Blob Analyzer</summary>
            Blob = 3,
            /// <summary>影像處理工具(Image Process)</summary>
            ImageProcess = 4
        }

        #endregion

        #region Function - Core
        /// <summary>建構一單影像之使用者控制項</summary>
        public CtAceSingleVision() {
            InitializeComponent();
        }
        
        /// <summary>與透過CtAce連接影像工具，並顯示在畫面上</summary>
        /// <param name="toolType">影像工具類型</param>
        /// <param name="ace">CtAce元件，用於連接並取的影像</param>
        /// <param name="path">影像工具之路徑，如 @"/Equipment/Vision/Device/Virtual Camera"</param>
        /// <returns>Status Code</returns>
        public Stat Connect(ToolType toolType, CtAce ace, string path) {
            Stat stt = Stat.SUCCESS;
            try {
                /*-- 取得相對應工具之IImageBuffer --*/
                IImageBuffer imgBuf = null;
                switch ( toolType ) {
                    case ToolType.VirtualCamera:
                        IVisionImageSource imgSrc = ace.FindObject(path) as IVisionImageSource;
                        if ( imgSrc != null ) {
                            imgBuf = imgSrc.Buffer;
                        } else
                            stt = Stat.ER_SYS_ILFLPH;
                        break;

                    case ToolType.Locator:
                        ILocatorTool imgLoc = ace.FindObject(path) as ILocatorTool;
                        if ( imgLoc != null ) {
                            imgBuf = imgLoc.ImageSource.Buffer;
                        } else
                            stt = Stat.ER_SYS_ILFLPH;
                        break;

                    case ToolType.CSharpCustomTool:
                        ICSharpCustomTool imgCVT = ace.FindObject(path) as ICSharpCustomTool;
                        if ( imgCVT != null ) {
                            imgBuf = imgCVT.ImageSource.Buffer;
                        } else
                            stt = Stat.ER_SYS_ILFLPH;
                        break;

                    case ToolType.Blob:
                        IBlobAnalyzerTool imgBlob = ace.FindObject(path) as IBlobAnalyzerTool;
                        if ( imgBlob != null ) {
                            imgBuf = imgBlob.ImageSource.Buffer;
                        } else
                            stt = Stat.ER_SYS_ILFLPH;
                        break;

                    case ToolType.ImageProcess:
                        IImageProcessingTool imgProc = ace.FindObject(path) as IImageProcessingTool;
                        if ( imgProc != null ) {
                            imgBuf = imgProc.ImageOperandSource.Buffer;
                        } else
                            stt = Stat.ER_SYS_ILFLPH;
                        break;

                    default:
                        stt = Stat.ER_SYS_INVARG;
                        throw ( new Exception("無相對應的VisionTool，如是新型態請加入Enum並讀取Buffer") );
                }

                /*-- 如果有正確取得IImageBuffer，建立Control並顯示 --*/
                if ( imgBuf != null ) {

                    /*-- 在IAceClient建立DisplayControl --*/
                    IImageDisplayControl imgCtrl = ace.GetClient().CreateObject(typeof(IImageDisplayControl)) as IImageDisplayControl;

                    /*-- 設定DisplayControl --*/
                    imgCtrl.Client = ace.GetClient();
                    imgCtrl.Buffer = imgBuf;
                    imgCtrl.AutomaticRendering = true;
                    imgCtrl.RulersVisible = OPTION_RULER;
                    imgCtrl.ScrollBarsVisible = OPTION_SCROLLBAR;
                    imgCtrl.ToolBarVisible = OPTION_TOOLBAR;
                    imgCtrl.StatusBarVisible = OPTION_STATUSBAR;
                    imgCtrl.ExecutionTimeVisible = OPTION_EXECTIME;
                    imgCtrl.AutoClearGraphics = OPTION_AUTOCLEAR;
                    
                    /*-- 轉為Windows之Control --*/
                    Control ctrl = imgCtrl as Control;
                    ctrl.Dock = DockStyle.Fill;
                    ctrl.Name = imgBuf.Name;
                    ctrl.Visible = false;
                    ctrl.Visible = true;

                    /*-- Assign --*/
                    this.Controls.Add(ctrl);
                }

            } catch ( Exception ex ) {
                if ( stt == Stat.SUCCESS )
                    stt = Stat.ER_SYSTEM;
                CtStatus.Report(stt, "SngVisConnect", ex.Message);
            }
            return stt;
        }

        #endregion
    }
}
