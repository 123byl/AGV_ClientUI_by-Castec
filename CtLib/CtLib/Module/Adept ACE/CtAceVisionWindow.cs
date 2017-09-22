using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using CtLib.Library;

using Ace.Adept.Client;
using Ace.Core.Client;
using Ace.Core.Server;
using Ace.Core.Server.Event;
using Ace.Core.Util;
using Ace.HSVision.Client;
using Ace.HSVision.Client.ImageDisplay;
using Ace.HSVision.Client.Editors;
using Ace.HSVision.Server.Control;
using Ace.HSVision.Server.Integration;
using Ace.HSVision.Server.Tools;
using Ace.Process.Client;
using CtLib.Module.Ultity;

namespace CtLib.Module.Adept {
    /// <summary>建立一個與Adept ACE Vision Window 關聯的控制項元件</summary>
    /// <remarks>以AceDemo之ImageBufferDisplayControl為基礎修改</remarks>
    /// <example><code>
    /// /*-- Create and connect to Adept ACE --*/
    /// CtAce mAce = new CtAce();
    /// mAce.Connect(CtAce.ControllerType.WITH_SMARTCONTROLLER);
    /// 
    /// /*-- Create VisionWindow --*/
    /// CtAceVisionWindow visWindow = new CtAceVisionWindow();
    /// visWindow.Parent = Panel1;          //Assume this vision window is attached to a panel object 
    /// visWindow.Dock = DockStyle.Fill;    //Set the dock style
    /// visWindow.Show();                   //You can show first or after conenct, it all bout you
    /// 
    /// /*-- Connect to Vision Server and start requesting --*/
    /// visWindow.Connect(mAce);
    /// </code></example>
    public partial class CtAceVisionWindow : UserControl {

        #region Version

        /// <summary>CtAceVisionWindow 版本訊息</summary>
        public static readonly CtVersion @Version = new CtVersion(1, 0, 2, "2014/10/19", "Ahern Kuo");

        /*---------- Version Note ----------
         * 
         * 1.0.0  <Ahern> [2014/09/09]
         *      + 完成初版介面
         *      
         * 1.0.1  <Ahern> [2014/09/19]
         *      + 修復VisionServer之問題，因Adept ACE當地語系問題造成，已修復並可使用
         *      \ Connect現在會自動進行與VisionServer之事件連結，避免此介面若較晚開啟，會造成CtAce負擔較重
         *      
         * 1.0.2  <Ahern> [2014/10/19]
         *      + Dispose
         *      
         *----------------------------------*/

        #endregion

        #region Declaration - Members
        private CtAce rAce;
        private IAceClient mIClient;
        #endregion

        #region Function - Core

        /// <summary>建構元</summary>
        public CtAceVisionWindow() {
            InitializeComponent();
        }

        /// <summary>與當前的 CtAce 所連接之 Adept ACE 進行 Vision Server 之連結，並取得 Vision 資源</summary>
        /// <param name="ace">已連接之 <see cref="CtAce"/></param>
        /// <returns>Status Code</returns>
        public Stat Connect(CtAce ace) {
            Stat stt = Stat.SUCCESS;
            try {
                if (ace == null) {
                    stt = Stat.ER_SYS_CLSCON;
                    throw (new Exception("傳入一個為空的IAceClient物件"));
                }

                rAce = ace;
                mIClient = ace.GetClient();
                rAce.Vision.AddVisionEventHandler();

                rAce.Vision.ImageBufferAdded += mIVisPlug_ServerBufferAdded;
                rAce.Vision.ImageBufferRemoved += mIVisPlug_ServerBufferRemoved;
                rAce.Vision.ImageBufferRenamed += mIVisPlug_ServerBufferNameChanged;
                rAce.Vision.VisionServerChagned += rAce_VisionServerChagned;

                rAce.Vision.RaiseVisionWindow();
            } catch (Exception ex) {
                stt = Stat.ER_SYSTEM;
                CtStatus.Report(stt, "Connect", ex.Message);
            }
            return stt;
        }

        #endregion

        #region Function - Event
        private delegate void ImageBufferHandler(object sender, ImageBufferModifiedEventArgs e);

        void mIVisPlug_ServerBufferAdded(object sender, ImageBufferModifiedEventArgs e) {
            try {
                /*-- Invoke --*/
                if (this.InvokeRequired) {
                    this.BeginInvoke(new ImageBufferHandler(mIVisPlug_ServerBufferAdded), new object[] { sender, e });
                    return;
                }

                /*-- 取得顯示控制物件，並將IAceClient與IImageBuffer餵進去 --*/
                IImageDisplayControl imgDispCtrl = mIClient.CreateObject(typeof(IImageDisplayControl)) as IImageDisplayControl;
                imgDispCtrl.Client = mIClient;
                imgDispCtrl.Buffer = e.Buffer;
                imgDispCtrl.AutomaticRendering = true;  //當Vision Tool自動呈交/連動時，取得並顯示在此物件上

                /*-- 將顯示物件視為Control，後續才能將之餵進去TabPage裡 --*/
                Control tabDispCtrl = imgDispCtrl as Control;
                tabDispCtrl.Dock = DockStyle.Fill;
                tabDispCtrl.Name = e.Buffer.Name;

                /*-- 建立好Control後，將之Assign到TabPage裡，後續再將他丟到TabControl裡 --*/
                TabPage tabPage = new TabPage(e.Buffer.Name);
                tabPage.Tag = imgDispCtrl;
                tabPage.Controls.Add(tabDispCtrl);

                /*-- 新增並顯示到TabControl裡 --*/
                tabWindow.TabPages.Add(tabPage);

            } catch (Exception ex) {
                CtStatus.Report(Stat.ER_SYSTEM, "rx_ServerBufferAdded", ex.Message);
            }
        }

        void mIVisPlug_ServerBufferRemoved(object sender, ImageBufferModifiedEventArgs e) {
            try {
                /*-- Invoke --*/
                if (this.InvokeRequired) {
                    this.BeginInvoke(new ImageBufferHandler(mIVisPlug_ServerBufferRemoved), new object[] { sender, e });
                    return;
                }

                /*-- 搜尋現在裡面所有的TabPage，如果有一樣的就把他刪了 --*/
                IImageDisplayControl imgDispCtrl;
                foreach (TabPage tabPage in tabWindow.TabPages) {
                    imgDispCtrl = tabPage.Tag as IImageDisplayControl;
                    if (imgDispCtrl.Buffer == e.Buffer) {
                        tabWindow.TabPages.Remove(tabPage);
                        imgDispCtrl.Buffer = null;
                        imgDispCtrl.Dispose();
                        break;
                    }
                }
            } catch (Exception ex) {
                CtStatus.Report(Stat.ER_SYSTEM, "rx_ServerBufferRemoved", ex.Message);
            }
        }

        private delegate void ImageBufferNameHandler(object sender, ImageBufferNameChangeEventArgs e);

        void mIVisPlug_ServerBufferNameChanged(object sender, ImageBufferNameChangeEventArgs e) {
            try {
                /*-- Invoke --*/
                if (this.InvokeRequired) {
                    this.BeginInvoke(new ImageBufferNameHandler(mIVisPlug_ServerBufferNameChanged), new object[] { sender, e });
                    return;
                }

                /*-- 搜尋現在裡面所有的TabPage，如果有一樣的就把他刪了 --*/
                IImageDisplayControl imgDispCtrl;
                foreach (TabPage tabPage in tabWindow.TabPages) {
                    imgDispCtrl = tabPage.Tag as IImageDisplayControl;
                    if (imgDispCtrl.Buffer == e.Buffer) {
                        tabPage.Name = e.Buffer.Name;
                        tabPage.Text = e.Buffer.Name;
                        break;
                    }
                }
            } catch (Exception ex) {
                CtStatus.Report(Stat.ER_SYSTEM, "rx_ServerBufferNameChanged", ex.Message);
            }
        }

        void rAce_VisionServerChagned(object sender, VisionServerStateChangeEventArgs e) {
            try {
                /*-- 檢查是否是已經執行完畢之狀態，如果不是則忽略掉 --*/
                if (e.NewStatus != VisionServerState.ToolExecutionCompleted)
                    return;

                /*-- 如非Source則不顯示 --*/
                IVisionImageSource imgSrc = e.ExecutingTool as IVisionImageSource;
                if (imgSrc == null)
                    return;

                /*-- Invoke --*/
                if (this.InvokeRequired == true) {
                    this.Invoke(new EventHandler<VisionServerStateChangeEventArgs>(rAce_VisionServerChagned), sender, e);
                    return;
                }

                /*-- 尋找相對應的Image並顯示(強制更新) --*/
                foreach (TabPage tabPage in this.tabWindow.TabPages) {
                    IImageDisplayControl imgDispCtrl = tabPage.Tag as IImageDisplayControl;
                    if (imgDispCtrl.Buffer == imgSrc.Buffer) {
                        tabWindow.SelectedTab = tabPage;

                        Control control = imgDispCtrl as Control;
                        control.Refresh();
                        break;
                    }
                }
            } catch (Exception ex) {
                CtStatus.Report(Stat.ER_SYSTEM, "rx_VisionServerChagned", ex.Message);
            }
        }

        #endregion
    }
}
