using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using CtLib.Library;

using Ace.Core.Client;
using Ace.HSVision.Client.Wizard;
using Ace.HSVision.Server.Tools;
using CtLib.Module.Ultity;

namespace CtLib.Module.Adept {

    /// <summary>
    /// Adept ACE Locator Model之編輯視窗
    /// <para>創建時可不使用Show，直接透過Connect()即可顯示Editor</para>
    /// </summary>
    /// <example><code>
    /// /*-- Create and connect to Adept ACE --*/
    /// CtAce mAce = new CtAce();
    /// mAce.Connect(CtAce.ControllerType.WITH_SMARTCONTROLLER);
    /// 
    /// /*-- Execute Model Editor --*/
    /// CtAceModelEditor modelEditor = new CtAceModelEditor();
    /// modelEditor.Connect(mAce, @"/Equipment/Vision/Test/Locator", @"/Equipment/Vision/Testing/Locator Model");
    /// </code></example>
    public partial class CtAceModelEditor : Form {

        #region Version

        /// <summary>CtAce 版本訊息</summary>
        public static readonly CtVersion @Version = new CtVersion(1, 0, 4, "2014/09/10", "Ahern Kuo");

        /*---------- Version Note ----------
         * 
         * 1.0.0  <Ahern> [2014/09/10]
         *      + 建立基礎模組
         *      
         *----------------------------------*/

        #endregion

        #region Function - Constructor
        /// <summary>建立 Model 編輯視窗</summary>
        public CtAceModelEditor() {
            InitializeComponent();
        }

        #endregion

        #region Function - Core

        /// <summary>
        /// 連結至特定Locator與其Model，並開啟設定介面
        /// <para>使用後請依照Status去判斷是否有完成，如有改變請記得施作存檔動作</para></summary>
        /// <param name="ace">CtAce物件，用於取得路徑中之實際物件</param>
        /// <param name="locPath">Locator路徑，如 @"/Equipment/Vision/Test/Locator"</param>
        /// <param name="modPath">LocatorModel路徑，如 @"/Equipment/Vision/Testing/Locator Model"</param>
        /// <returns>Status Code</returns>
        /// <remarks>
        /// 屆時ACE元件將會需要讀取Resource，目前ACE的資源檔有de/fr/ja/zh (其中de表示Default，即English)
        /// <para>在開啟前請記得更改Thread.CurrentThread.CurrentCulture與CurrentUICultrue</para>
        /// </remarks>
        public Stat Connect(CtAce ace, string locPath, string modPath) {
            Stat stt = Stat.SUCCESS;
            try {
                /*-- 依照傳入之路徑建立相對應物件 --*/
                ILocatorTool locTool = ace.FindObject(locPath) as ILocatorTool;
                ILocatorModel locModel = ace.FindObject(modPath) as ILocatorModel;

                /*-- 開啟ModelEditor --*/
                bool bolResult = VisionToolTeachUtil.TeachLocatorModel(this, ace.GetClient(), locTool, ref locModel);

                /*-- 檢查是否完成 --*/
                if ( !bolResult )
                    stt = Stat.WN_ACE_USRCNC;

                /*-- 讓Locator重新跑一次 --*/
                locTool.Execute(false);
            } catch ( Exception ex ) {
                if ( stt == Stat.SUCCESS )
                    stt = Stat.ER_SYSTEM;
                CtStatus.Report(stt, "ModelConnect", ex.Message);
            }
            return stt;
        }

        /// <summary>
        /// 連結至特定Locator與其Model，並開啟設定介面
        /// <para>使用後請依照Status去判斷是否有完成，如有改變請記得施作存檔動作</para></summary>
        /// <param name="aceClient">IAceClient 用於取得 Plugin</param>
        /// <param name="locator">ILocatorTool，欲執行之Locator物件。可搭配 CtAce 之 FindObject</param>
        /// <param name="model">ILocatorModel，搭配於Locator之Model。可搭配 CtAce 之 FindObject</param>
        /// <returns>Status Code</returns>
        /// <remarks>
        /// 屆時ACE元件將會需要讀取Resource，目前ACE的資源檔有de/fr/ja/zh (其中de表示Default，即English)
        /// <para>在開啟前請記得更改Thread.CurrentThread.CurrentCulture與CurrentUICultrue</para>
        /// </remarks>
        public Stat ConnectWithAceObject(object aceClient,  object locator, object model) {
            Stat stt = Stat.SUCCESS;
            try {

                ILocatorTool locTool = locator as ILocatorTool;
                ILocatorModel locModel = model as ILocatorModel;

                /*-- 開啟ModelEditor --*/
                bool bolResult = VisionToolTeachUtil.TeachLocatorModel(this, aceClient as IAceClient, locTool, ref locModel);

                /*-- 檢查是否完成 --*/
                if (!bolResult)
                    stt = Stat.WN_ACE_USRCNC;

                /*-- 讓Locator重新跑一次 --*/
                locTool.Execute(false);
            } catch (Exception ex) {
                if (stt == Stat.SUCCESS)
                    stt = Stat.ER_SYSTEM;
                CtStatus.Report(stt, "ModelConnect", ex.Message);
            }
            return stt;
        }

        #endregion
    }
}
