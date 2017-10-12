using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using CtLib.Library;
using CtLib.Module.Utility;

using Ace.Core.Client;
using Ace.Core.Server;
using Ace.Core.Util;
using Ace.HSVision.Client;
using Ace.HSVision.Server.Control;
using Ace.HSVision.Server.Integration;
using Ace.HSVision.Server.Integration.ImageSources.Emulation;
using Ace.HSVision.Server.Tools;

namespace CtLib.Module.Adept {
    /// <summary>
    /// Adept ACE 之影像相關控制
    /// <para>此部分包含 Vision Event 及相關 Vision Tool 之使用</para>
    /// </summary>
	[Serializable]
    public sealed class CtAceVision : IDisposable {

        #region Declaration - Support Functions
        /// <summary>控管 Vision Server 相關事件</summary>
        /// <remarks>原文: Event handler managing the low level events for the vision server</remarks>
        [Serializable]
        private class VisionServerEventHandler : RemotableClientObject, IDisposable {
            /// <summary>欲建立相關事件的VisionServer</summary>
            /// <remarks>Handle to the Vision Server being monitored</remarks>
            [NonSerializedAttribute]
            private IVisionServer mIVisSrv;
            /// <summary>Vision Server 狀態改變之事件</summary>
            /// <remarks>An event raised when the state of the server changes</remarks>
            public event EventHandler<VisionServerStateChangeEventArgs> StateChanged;
            /// <summary>Vision Server 有新增加之 Buffer</summary>
            /// <remarks>An event raised when a buffer has been added to the collection of buffers managed by the server</remarks>
            public event EventHandler<ImageBufferModifiedEventArgs> ImageAdded;
            /// <summary>Vision Server 中有 Buffer 被移除</summary>
            /// <remarks>An event raised when a buffer has been removed from the collection of buffers managed by the server</remarks>
            public event EventHandler<ImageBufferModifiedEventArgs> ImageRemoved;

            /// <summary>初始化並建構一個繼承於<see cref="RemoteAceObjectEventHandler"/>物件</summary>
            /// <param name="aceClient">IAceClient 物件</param>
            /// <param name="visPlugin">IVisionPlugin 物件</param>
            /// <see cref="RemoteAceObjectEventHandler"/>
            /// <remarks>Initializes a new instance of the <see cref="RemoteAceObjectEventHandler"/> class</remarks>
            public VisionServerEventHandler(IAceClient aceClient, IVisionPlugin visPlugin)
                : base(aceClient) {
                mIVisSrv = visPlugin.VisionServer;
                mIVisSrv.BufferAdded += mIVisSrv_BufferAdded;
                mIVisSrv.BufferRemoved += mIVisSrv_BufferRemoved;
                mIVisSrv.StateChanged += mIVisSrv_StateChanged;
            }
            /// <summary>初始化並建構一個繼承於<see cref="RemoteAceObjectEventHandler"/>物件</summary>
            /// <param name="aceClient">IAceClient 物件</param>
            /// <param name="visPlugin">IVisionPlugin 物件</param>
            /// <param name="aceServer">IAceServer 物件</param>
            /// <remarks>Initializes a new instance of the <see cref="RemoteAceObjectEventHandler"/> class</remarks>
            public VisionServerEventHandler(IAceClient aceClient, IAceServer aceServer, IVisionPlugin visPlugin)
                : base(aceClient) {
                mIVisSrv = aceServer.PropertyManager[typeof(IVisionServer).Name] as IVisionServer;
                mIVisSrv.BufferAdded += mIVisSrv_BufferAdded;
                mIVisSrv.BufferRemoved += mIVisSrv_BufferRemoved;
                mIVisSrv.StateChanged += mIVisSrv_StateChanged;
            }

            /// <summary><see cref="IVisionServer"/> 增加 <see cref="IImageBuffer"/> 事件</summary>
            /// <param name="sender">影像來源物件</param>
            /// <param name="e">事件相關資料</param>
            /// <remarks>Handles the BufferAdded event of the visionServer control</remarks>
            public void mIVisSrv_BufferAdded(object sender, ImageBufferModifiedEventArgs e) {
                EventHandler<ImageBufferModifiedEventArgs> handler = ImageAdded;
                if (handler != null)
                    handler(sender, e);
            }

            /// <summary><see cref="IVisionServer"/> 移除 <see cref="IImageBuffer"/> 事件</summary>
            /// <param name="sender">影像來源物件</param>
            /// <param name="e">事件相關資料</param>
            /// <remarks>Handles the BufferRemoved event of the visionServer control</remarks>
            public void mIVisSrv_BufferRemoved(object sender, ImageBufferModifiedEventArgs e) {
                EventHandler<ImageBufferModifiedEventArgs> handler = ImageRemoved;
                if (handler != null)
                    handler(sender, e);
            }

            /// <summary><see cref="IVisionServer"/> 狀態改變事件</summary>
            /// <param name="sender">影像來源物件</param>
            /// <param name="e">事件相關資料</param>
            /// <remarks>Handles the StateChanged event of the visionServer control</remarks>
            public void mIVisSrv_StateChanged(object sender, VisionServerStateChangeEventArgs e) {
                EventHandler<VisionServerStateChangeEventArgs> handler = StateChanged;
                if (handler != null)
                    handler(sender, e);
            }

            /// <summary>覆寫 Dispose，並將事件移除</summary>
            /// <param name="isDisposing">開始進行Dispose</param>
            /// <remarks>Dispose of the object. Release all event handles</remarks>
            protected override void Dispose(bool isDisposing) {
                if (isDisposing) {
                    if (mIVisSrv != null) {
                        mIVisSrv.BufferAdded -= mIVisSrv_BufferAdded;
                        mIVisSrv.BufferRemoved -= mIVisSrv_BufferRemoved;
                        mIVisSrv.StateChanged -= mIVisSrv_StateChanged;
                        mIVisSrv = null;
                    }
                }
                base.Dispose(isDisposing);
            }
        }
        #endregion

        #region Declaration - Events
        /// <summary>發生新增Vision事件</summary>
        public event EventHandler<ImageBufferModifiedEventArgs> ImageBufferAdded;
        /// <summary>發生移除Vision Buffer事件</summary>
        public event EventHandler<ImageBufferModifiedEventArgs> ImageBufferRemoved;
        /// <summary>發生Vision重新命名事件</summary>
        public event EventHandler<ImageBufferNameChangeEventArgs> ImageBufferRenamed;
        /// <summary>發生Vision Server狀態改變</summary>
        public event EventHandler<VisionServerStateChangeEventArgs> VisionServerChagned;

        /// <summary>觸發ImageBuffer新增或移除事件</summary>
        /// <param name="change">True:新增  False:移除</param>
        /// <param name="e">ImageBufferModified事件參數</param>
        private void OnVisionBufferChange(bool change, ImageBufferModifiedEventArgs e) {
            if (FreezeList.Count > 0) {
                if (FreezeList.FindAll(keyWord => e.Buffer.Name.Contains(keyWord)).Count == 0) return;
            }
            EventHandler<ImageBufferModifiedEventArgs> handler;
            handler = (change) ? ImageBufferAdded : ImageBufferRemoved;

            if (handler != null) handler(this, e);
        }

		/// <summary>觸發ImageBuffer重新命名事件</summary>
		/// <param name="e">ImageBufferNameChange事件參數</param>
		private void OnVisionBufferRenamed(ImageBufferNameChangeEventArgs e) {
            EventHandler<ImageBufferNameChangeEventArgs> handler = ImageBufferRenamed;

            if (handler != null) handler(this, e);
        }

		/// <summary>觸發VisionServer狀態改變事件</summary>
		/// <param name="e">VisionServerStateChange事件參數</param>
		private void OnVisionServerStateChange(VisionServerStateChangeEventArgs e) {
            EventHandler<VisionServerStateChangeEventArgs> handler = VisionServerChagned;

            if (handler != null) handler(this, e);
        }
        #endregion

        #region Declaration - Fields
        /// <summary>用於Handle Vision Server 相關事件</summary>
        private VisionServerEventHandler mVisSrvHdl;
        /// <summary>Vision是否已加入過事件</summary>
        private bool mVisionAdded = false;
        /// <summary>AceClient</summary>
        private IAceClient mIClient;
        /// <summary>AceServer</summary>
        private IAceServer mIServer;
        /// <summary>Vision Window Plugin</summary>
        private IVisionPlugin mIVisPlug;
        #endregion

        #region Declaration - Properties
        /// <summary>Vision Window 之 固定</summary>
        public List<string> FreezeList { get; set; }
        #endregion

        #region Function - Constructor
        /// <summary>建立 Adept ACE Vision 子模組</summary>
        /// <param name="aceClient">IAceClient</param>
        /// <param name="aceServer">IAceServer</param>
        public CtAceVision(IAceClient aceClient, IAceServer aceServer) {
            mVisionAdded = false;
            mIClient = aceClient;
            mIServer = aceServer;
            FreezeList = new List<string>();
        }
        #endregion

        #region Function - Dispose
        /// <summary>關閉與 Vision Server 之連線，並釋放資源</summary>
        public void Dispose() {
            try {
                Dispose(true);
                GC.SuppressFinalize(this);
            } catch (ObjectDisposedException ex) {
                CtStatus.Report(Stat.ER_SYSTEM, ex);
            }
        }

		/// <summary>關閉與 Vision Server 之連線，並釋放資源</summary>
		/// <param name="isDisposing">是否為第一次釋放</param>
		private void Dispose(bool isDisposing) {
            try {
                if (isDisposing) {
                    /*-- 移除相關事件 --*/
                    RemoveVisionEventHandler();
                }
            } catch (Exception ex) {
                CtStatus.Report(Stat.ER_SYSTEM, ex);
            }
        }
        #endregion

        #region Function - Methods
        /// <summary>取得ACE內部物件(IAceServer.Root)</summary>
        /// <param name="path">欲取得物件之路徑</param>
        /// <returns>該路徑之物件。請小心判斷是否為 NULL</returns>
        private object FindObject(string path) {
            object objTemp = null;
            if ((path != "") && (mIServer != null)) objTemp = mIServer.Root[path];
            return objTemp;
        }

        /// <summary>取得ACE內部(IAceServer.Root)相同型態且具有關鍵字之物件</summary>
        /// <param name="keyWord">搜尋關鍵字</param>
        /// <param name="obj">回傳符合條件之物件集合</param>
        /// <param name="type">該物件類型</param>
        private void FindObject(string keyWord, Type type, out List<string> obj) {
            List<string> objTemp = new List<string>();

            if ((keyWord != "") && (mIServer != null)) {
                /*-- 搜尋IAceServer裡的物件，並過濾出特定型態之物件，檢查是否含有關鍵字... --*/
                foreach (var item in mIServer.Root.Filter(new ObjectTypeFilter(type), true)) {
                    if (item.FullPath.Contains(keyWord))
                        objTemp.Add(item.FullPath);
                }
            }

            obj = objTemp;
        }

        /// <summary>取得ACE內部(IAceServer.Root)相同型態且具有關鍵字之物件</summary>
        /// <param name="keyWord">搜尋關鍵字</param>
        /// <param name="type">該物件類型</param>
        /// <returns>符合條件之物件集合</returns>
        private List<string> FindObject(string keyWord, Type type) {
            List<string> objTemp = new List<string>();

            if ((keyWord != "") && (mIServer != null)) {
                /*-- 搜尋IAceServer裡的物件，並過濾出特定型態之物件，檢查是否含有關鍵字... --*/
                foreach (var item in mIServer.Root.Filter(new ObjectTypeFilter(type), true)) {
                    if (item.FullPath.Contains(keyWord))
                        objTemp.Add(item.FullPath);
                }
            }

            return objTemp;
        }
        #endregion

        #region Function - Core

        #region Vision Events
        /// <summary>加入VisionServer之Evnet</summary>
        /// <remarks>因為了要讓CtAceVisionWindow可以在需要時才加入與顯示，故新增這個Function</remarks>
        public void AddVisionEventHandler() {
            if (!mVisionAdded) {
                /*-- 增加Vision之Event --*/
                mIVisPlug = mIClient.ClientPropertyManager[typeof(IVisionPlugin).Name] as IVisionPlugin;

                mIVisPlug.ServerBufferAdded += new EventHandler<ImageBufferModifiedEventArgs>(mIVisPlug_ServerBufferAdded);
                mIVisPlug.ServerBufferRemoved += new EventHandler<ImageBufferModifiedEventArgs>(mIVisPlug_ServerBufferRemoved);
                mIVisPlug.ServerBufferNameChanged += new EventHandler<ImageBufferNameChangeEventArgs>(mIVisPlug_ServerBufferNameChanged);

                mVisSrvHdl = new VisionServerEventHandler(mIClient, mIVisPlug);
                mVisSrvHdl.StateChanged += new EventHandler<VisionServerStateChangeEventArgs>(mVisSrvHdl_StateChanged);

                CtTimer.Delay(500);
                mVisionAdded = true;
            }
        }

        /// <summary>移除VisionServer之事件</summary>
        public void RemoveVisionEventHandler() {
            if (mVisionAdded) {
                if (mIVisPlug != null) {
                    mIVisPlug.ServerBufferAdded -= new EventHandler<ImageBufferModifiedEventArgs>(mIVisPlug_ServerBufferAdded);
                    mIVisPlug.ServerBufferRemoved -= new EventHandler<ImageBufferModifiedEventArgs>(mIVisPlug_ServerBufferRemoved);
                    mIVisPlug.ServerBufferNameChanged -= new EventHandler<ImageBufferNameChangeEventArgs>(mIVisPlug_ServerBufferNameChanged);
                    mIVisPlug = null;
                }

                if (mVisSrvHdl != null) {
                    mVisSrvHdl.StateChanged -= new EventHandler<VisionServerStateChangeEventArgs>(mVisSrvHdl_StateChanged);
                    mVisSrvHdl.Dispose();
                    mVisSrvHdl = null;
                }
            }

            mVisionAdded = false;
        }

        /// <summary>將現在VisionServer裡之ImageBuffer利用事件拋出來，以利CtAceVisionWindow或其他程序可以得知當前之影像</summary>
        public void RaiseVisionWindow() {
            IVisionServer visSrv = mIServer.PropertyManager[typeof(IVisionServer).Name] as IVisionServer;
            ReadOnlyCollection<IImageBuffer> imgBuffer = visSrv.ImageBuffers;
            foreach (IImageBuffer item in imgBuffer) {
                OnVisionBufferChange(true, new ImageBufferModifiedEventArgs(item));
            }
        }
        #endregion

        #region Vision Tools

        /// <summary>執行VisionTool</summary>
        /// <param name="path">該VisionTool於ACE之路徑，如 @"/Equipment/Vision/CVT"</param>
        /// <param name="toolType">VisionTool類型</param>
        /// <param name="takePicture">是否重新取得影像? 如是在 Vision On The Fly 請小心 Camera 位置是不是跑走了</param>
        public void ExecuteVisionTool(string path, VisionToolType toolType, bool takePicture) {
            switch (toolType) {
                case VisionToolType.VisionSource:
                    IVisionImageSource imgSrc = FindObject(path) as IVisionImageSource;
                    imgSrc.Execute(takePicture);
                    break;
                case VisionToolType.Locator:
                    ILocatorTool imgLoc = FindObject(path) as ILocatorTool;
                    imgLoc.Execute(takePicture);
                    break;
                case VisionToolType.CustomVisionTool:
                    ICSharpCustomTool imgCVT = FindObject(path) as ICSharpCustomTool;
                    imgCVT.Execute(takePicture);
                    break;
                case VisionToolType.BlobAnalyzer:
                    IBlobAnalyzerTool imgBlob = FindObject(path) as IBlobAnalyzerTool;
                    imgBlob.Execute(takePicture);
                    break;
                case VisionToolType.ImageProcessing:
                    IImageProcessingTool imgProc = FindObject(path) as IImageProcessingTool;
                    imgProc.Execute(takePicture);
                    break;
                default:
                    throw (new Exception("目前尚不支援此類影像工具"));
            }
        }

        /// <summary>執行VisionTool</summary>
        /// <param name="path">該VisionTool於ACE之路徑，如 @"/Equipment/Vision/CVT"</param>
        /// <param name="toolType">VisionTool類型</param>
        /// <param name="takePicture">是否重新取得影像? 如是在 Vision On The Fly 請小心 Camera 位置是不是跑走了</param>
        /// <param name="matchQuality">回傳該Tool之Match Quality，如果沒有結果或此Tool沒有分數選項，則回傳-1</param>
        public void ExecuteVisionTool(string path, VisionToolType toolType, bool takePicture, out float matchQuality) {
            float sngQuality = -1F;

            switch (toolType) {
                case VisionToolType.VisionSource:
                    IVisionImageSource imgSrc = FindObject(path) as IVisionImageSource;
                    imgSrc.Execute(takePicture);
                    break;
                case VisionToolType.Locator:
                    ILocatorTool imgLoc = FindObject(path) as ILocatorTool;
                    imgLoc.Execute(takePicture);
                    if (imgLoc.ResultsAvailable) sngQuality = imgLoc.Results[0].MatchQuality * 100;
                    break;
                case VisionToolType.CustomVisionTool:
                    ICSharpCustomTool imgCVT = FindObject(path) as ICSharpCustomTool;
                    imgCVT.Execute(takePicture);
                    break;
                case VisionToolType.BlobAnalyzer:
                    IBlobAnalyzerTool imgBlob = FindObject(path) as IBlobAnalyzerTool;
                    imgBlob.Execute(takePicture);
                    if (imgBlob.ResultsAvailable) sngQuality = imgBlob.Results[0].Area;
                    break;
                case VisionToolType.ImageProcessing:
                    IImageProcessingTool imgProc = FindObject(path) as IImageProcessingTool;
                    imgProc.Execute(takePicture);
                    break;
                default:
                    throw (new Exception("目前尚不支援此類影像工具"));
            }

            matchQuality = sngQuality;
        }

        /// <summary>執行VisionTool</summary>
        /// <param name="path">該VisionTool於ACE之路徑，如 @"/Equipment/Vision/CVT"</param>
        /// <param name="toolType">VisionTool類型</param>
        /// <param name="takePicture">是否重新取得影像? 如是在 Vision On The Fly 請小心 Camera 位置是不是跑走了</param>
        /// <param name="matchQuality">回傳該Tool之Match Quality，如果沒有結果或此Tool沒有分數選項，則回傳-1</param>
        /// <param name="scaleFactor">回傳該Tool之ScaleFactor，如果沒有結果或此Tool沒有分數選項，則回傳-1</param>
        public void ExecuteVisionTool(string path, VisionToolType toolType, bool takePicture, out float matchQuality, out float scaleFactor) {
            float sngQuality = -1F;
            float sngScale = -1F;

            switch (toolType) {
                case VisionToolType.VisionSource:
                    IVisionImageSource imgSrc = FindObject(path) as IVisionImageSource;
                    imgSrc.Execute(takePicture);
                    break;
                case VisionToolType.Locator:
                    ILocatorTool imgLoc = FindObject(path) as ILocatorTool;
                    imgLoc.Execute(takePicture);
                    if (imgLoc.ResultsAvailable) {
                        sngQuality = imgLoc.Results[0].MatchQuality * 100;
                        sngScale = imgLoc.Results[0].ScaleFactor;
                    }
                    break;
                case VisionToolType.CustomVisionTool:
                    ICSharpCustomTool imgCVT = FindObject(path) as ICSharpCustomTool;
                    imgCVT.Execute(takePicture);
                    break;
                case VisionToolType.BlobAnalyzer:
                    IBlobAnalyzerTool imgBlob = FindObject(path) as IBlobAnalyzerTool;
                    imgBlob.Execute(takePicture);
                    if (imgBlob.ResultsAvailable) sngQuality = imgBlob.Results[0].Area;
                    break;
                case VisionToolType.ImageProcessing:
                    IImageProcessingTool imgProc = FindObject(path) as IImageProcessingTool;
                    imgProc.Execute(takePicture);
                    break;
                default:
                    throw (new Exception("目前尚不支援此類影像工具"));
            }

            matchQuality = sngQuality;
            scaleFactor = sngScale;
        }

        /// <summary>取得特定 Locator 當前所有的 LocatorModel 完整路徑</summary>
        /// <param name="locatorPath">欲取得Model之Locator。如 "/Equipment/Vision/Test Locator"</param>
        /// <returns>LocatorModel 完整路徑。如 "/Equipment/Vision/Models/Test Model"</returns>
        public List<string> GetCurrentModelNames(string locatorPath) {
            List<string> strModule = new List<string>();
            ILocatorTool locator = FindObject(locatorPath) as ILocatorTool;
            foreach (ILocatorModel item in locator.Models) {
                strModule.Add(item.FullPath);
            }
            return strModule;
        }

        /// <summary>取得具有特定名稱之 LocatorModel 路徑</summary>
        /// <param name="keyWord">搜尋關鍵字，可為路徑、名稱
        /// <para>如，特定資料夾內之Model: "/Vision/Models"；名稱: "Test Model"</para></param>
        /// <returns>LocatorModel 完整路徑。如 "/Equipment/Vision/Models/Test Model"</returns>
        public List<string> GetLocatorModelNames(string keyWord) {
            List<string> strModule = new List<string>();
            FindObject(keyWord, typeof(ILocatorModel), out strModule);
            return strModule;
        }

        /// <summary>加入 LocatorModel 至特定 Locator 裡</summary>
        /// <param name="locatorPath">欲加入Model之Locator路徑。如 "/Equipment/Vision/Test Locator"</param>
        /// <param name="modelPath">欲加入之Model路徑。如 "/Equipment/Vision/Models/Test Model"</param>
        public void AddLocatorModel(string locatorPath, string modelPath) {
            ILocatorModel model = FindObject(modelPath) as ILocatorModel;
            if (model != null) {
                ILocatorTool locator = FindObject(locatorPath) as ILocatorTool;
                if (locator != null) {
                    model.ImageSource = locator.ImageSource;
                    locator.AddModel(model);
                }
            }
        }

        /// <summary>加入 LocatorModel 至特定 Locator 裡</summary>
        /// <param name="locatorPath">欲加入Model之Locator路徑。如 "/Equipment/Vision/Test Locator"</param>
        /// <param name="modelPath">欲加入之Model路徑。如 "/Equipment/Vision/Models/Test Model"</param>
        public void AddLocatorModel(string locatorPath, List<string> modelPath) {
            ILocatorTool locator = FindObject(locatorPath) as ILocatorTool;
            if (locator != null) {
                foreach (string item in modelPath) {
                    ILocatorModel model = FindObject(item) as ILocatorModel;
                    if (model != null) locator.AddModel(model);
                }
            }
        }

        /// <summary>移除特定 Locator 裡「所有」的 LocatorModel</summary>
        /// <param name="locatorPath">欲移除所有Model之Locator路徑。如 "/Equipment/Vision/Test Locator"</param>
        /// <returns>Status Code</returns>
        public void RemoveLocatorModel(string locatorPath) {
            ILocatorTool locator = FindObject(locatorPath) as ILocatorTool;
            if (locator != null) {
                List<ILocatorModel> models = new List<ILocatorModel>(locator.Models);

                foreach (ILocatorModel model in models) {
                    locator.RemoveModel(model);
                }
            }
        }

        /// <summary>移除 Locator 裡「特定」的 LocatorMode</summary>
        /// <param name="locatorPath">欲移除Model之Locator路徑。如 "/Equipment/Vision/Test Locator"</param>
        /// <param name="modelKeyword">欲移除之Model關鍵字。可為路徑、名稱
        /// <para>如："/Vision/Models/Test Model"、"Test Model"</para></param>
        public void RemoveLocatorModel(string locatorPath, string modelKeyword) {
            List<ILocatorModel> models = new List<ILocatorModel>();
            ILocatorTool locator = FindObject(locatorPath) as ILocatorTool;
            if (locator != null) {
                foreach (ILocatorModel model in locator.Models) {
                    if (model.FullPath.Contains(modelKeyword)) {
                        models.Add(model);
                    }
                }
                foreach (ILocatorModel model in models) {
                    locator.RemoveModel(model);
                }
            }
        }

        /// <summary>建立一全新的 LocatorModel</summary>
        /// <param name="modelPath">欲存放之資料夾路徑。如 "/Equipment/Vision/Models/"</param>
        /// <param name="modelName">新增之Model名稱，如 "Test Model 02"</param>
        /// <param name="locatorPath">如欲直接新增至特定Locator裡，請輸入該Locator完整路徑。保持為空，如果你並不需要此功能。</param>
        /// <param name="editor">承接locatorPath，加入Locator後是否開啟編輯視窗</param>
        public void CreateLocatorModel(string modelPath, string modelName, string locatorPath = "", bool editor = false) {
            IAceObjectCollection objCollection = mIServer.Root[modelPath] as IAceObjectCollection;
            ILocatorModel model = objCollection.AddObjectOfType(typeof(ILocatorModel), modelName) as ILocatorModel;

            if (model == null) throw (new Exception("創建新Model失敗"));

            if (locatorPath != "") {
                RemoveLocatorModel(locatorPath);
                AddLocatorModel(locatorPath, model.FullPath);
            }

            if ((editor) && (locatorPath != "")) {
                /*-- 依照傳入之路徑建立相對應物件 --*/
                ILocatorTool locTool = FindObject(locatorPath) as ILocatorTool;

                if (locTool != null) {
                    CtAceModelEditor frm = new CtAceModelEditor();
                    frm.ConnectWithAceObject(mIClient, locTool, model);
                    frm.Dispose();
                } else throw (new Exception("無法開啟編輯視窗。可能為新 Model 建立失敗"));
            }
        }

        /// <summary>刪除 EmulationInstance 之圖片 (虛擬攝影機存放圖片之物件)</summary>
        /// <param name="cameraPath">虛擬攝影機路徑，如 @"/Equipment/Vision/Device/Emulation/Emulation Buffer"</param>
        public void EmulationCameraImageRemove(string cameraPath) {
            IEmulationInstance camera = FindObject(cameraPath) as IEmulationInstance;
            if (camera != null) {
                foreach (string filename in camera.ImageList) {
                    camera.DeleteImage(filename);
                }
            }
        }

        /// <summary>刪除 EmulationInstance 內具有特定關鍵字之圖片 (虛擬攝影機存放圖片之物件)</summary>
        /// <param name="cameraPath">虛擬攝影機路徑，如 @"/Equipment/Vision/Device/Emulation/Emulation Buffer"</param>
        /// <param name="keyWord">欲比對之關鍵字</param>
        public void EmulationCameraImageRemove(string cameraPath, string keyWord) {
            IEmulationInstance camera = FindObject(cameraPath) as IEmulationInstance;
            if (camera != null) {
                for (int i = 0; i < camera.ImageList.Length; i++) {
                    if (camera.ImageList[i].Contains(keyWord)) {
                        camera.DeleteImage(camera.ImageList[i]);
                        if ((camera.ImageList.Length > 0) && (i < camera.ImageList.Length)) i--;
                    }
                }
            }
        }

        /// <summary>將圖片加入 EmulationInstance (虛擬攝影機存放圖片之物件)</summary>
        /// <param name="cameraPath">虛擬攝影機路徑，如 @"/Equipment/Vision/Device/Emulation/Emulation Buffer"</param>
        /// <param name="picturePath">實際圖片路徑，如 @"D:\Pictures\DIMM.png"</param>
        public void EmulationCameraImageAdd(string cameraPath, string picturePath) {
            IEmulationInstance camera = FindObject(cameraPath) as IEmulationInstance;
            if ((camera != null) && CtFile.IsFileExist(picturePath)) {
                camera.ImportImage(picturePath);
            }
        }
		#endregion

		#region Tools
		/// <summary>取得對應的 Vision Tool 類型</summary>
		/// <param name="type">欲取得的 VisionToolType</param>
		/// <returns>對應的 Vision Tool 類型</returns>
		public Type GetHexSightToolType(VisionToolType type) {
			Type tarType = null;
			switch (type) {
				case VisionToolType.VisionSource:
					tarType = typeof(IVisionImageSource);
					break;
				case VisionToolType.Locator:
					tarType = typeof(ILocatorTool);
					break;
				case VisionToolType.CustomVisionTool:
					tarType = typeof(ICSharpCustomTool);
					break;
				case VisionToolType.BlobAnalyzer:
					tarType = typeof(IBlobAnalyzerTool);
					break;
				case VisionToolType.ImageProcessing:
					tarType = typeof(IImageProcessingTool);
					break;
				case VisionToolType.LocatorModel:
					tarType = typeof(ILocatorModel);
					break;
				case VisionToolType.EdgeLocator:
					tarType = typeof(IEdgeLocatorTool);
					break;
				case VisionToolType.LineFinder:
					tarType = typeof(ILineFinderTool);
					break;
				case VisionToolType.ArcFinder:
					tarType = typeof(IArcFinderTool);
					break;
				case VisionToolType.PointFinder:
					tarType = typeof(IPointFinderTool);
					break;
				default:
					break;
			}
			return tarType;
		}

		/// <summary>搜尋所有可做為影像來源之工具完整路徑</summary>
		/// <returns>工具完整路徑集合，如 "/Vision/Tools/Locator"</returns>
		public List<string> GetImageSources() {
			return mIServer.Root.FilterType(typeof(IVisionImageSource), true).Select(tool => tool.FullPath).ToList();
		}

		/// <summary>取得所有特定影像工具之完整路徑</summary>
		/// <param name="type">欲尋找的種類</param>
		/// <returns>工具完整路徑集合，如 "/Vision/Tools/Locator"</returns>
		public List<string> GetVisionTools(VisionToolType type) {
			Type sightType = GetHexSightToolType(type);
			return mIServer.Root.FilterType(sightType, true).Select(tool => tool.FullPath).ToList();
		}
		#endregion

		#endregion

		#region Function - IVisionPlugin Events

		void mIVisPlug_ServerBufferNameChanged(object sender, ImageBufferNameChangeEventArgs e) {
			OnVisionBufferRenamed(e);
        }

        void mIVisPlug_ServerBufferRemoved(object sender, ImageBufferModifiedEventArgs e) {
			OnVisionBufferChange(false, e);
        }

        void mIVisPlug_ServerBufferAdded(object sender, ImageBufferModifiedEventArgs e) {
			OnVisionBufferChange(true, e);
        }

        void mVisSrvHdl_StateChanged(object sender, VisionServerStateChangeEventArgs e) {
			OnVisionServerStateChange(e);
        }
        #endregion
    }
}
