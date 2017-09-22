using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

using CtLib.Library;

namespace CtLib.Forms {
    /// <summary>
    /// CASTEC Style 對話視窗
    /// <para>可自訂字體、圖樣，並可透過回傳值取得使用者最後按下的按鈕</para>
    /// </summary>
    /// <example>
    /// 請使用 .Show() 來開啟對話視窗，並接收回傳值
    /// <code>
    /// MsgBoxButton button = CtMsgBox.Show("標題", "內容", MsgBoxButton.OK_CANCEL, MsgBoxStyle.INFORMATION);
    /// </code>
    /// 另可在 .Show() 中帶入字型與大小 (共三個多載)
    /// <code>
    /// MsgBoxButton button = CtMsgBox.Show("標題", "內容", "微軟正黑體", 28, MsgBoxButton.OK_CANCEL, MsgBoxStyle.INFORMATION);
    /// </code>
    /// 目前預設按鈕組有 OK_CANCEL, YES_NO，如果有需要其他組合，可用 OR 進行變更
    /// <code>
    /// MsgBoxButton button = CtMsgBox.Show("標題", "內容", MsgBoxButton.OK | MsgBoxButton.NO, MsgBoxStyle.NONE);
    /// </code></example>
    public static class CtMsgBox {
        
        /// <summary>顯示自訂的對話視窗，以預設的字型為主(微軟正黑體、12pt)</summary>
        /// <param name="title">欲顯示於對話視窗上的標題</param>
        /// <param name="content">欲顯示的內容</param>
        /// <param name="buttons">按鈕集合，可使用 OR 進行組裝</param>
        /// <param name="msgStyle">圖案樣式，無、訊息、警告、詢問、錯誤</param>
        /// <returns>使用者最後按下的按鈕</returns>
        public static MsgBoxButton Show(string title, string content, MsgBoxButton buttons = MsgBoxButton.OK, MsgBoxStyle msgStyle = MsgBoxStyle.INFORMATION) {
            MsgBoxButton uiBtn = MsgBoxButton.OK;
            using (CtMessage_Ctrl mMsgBox = new CtMessage_Ctrl(title, content, buttons, msgStyle)) {
                mMsgBox.ShowDialog();
                uiBtn = mMsgBox.UIResult;
            }
            return uiBtn;
        }

        /// <summary>顯示自訂的對話視窗，字型以預設的字體(微軟正黑體)並自訂字體大小</summary>
        /// <param name="title">欲顯示於對話視窗上的標題</param>
        /// <param name="content">欲顯示的內容</param>
        /// <param name="fontSize">自訂的字體大小</param>
        /// <param name="buttons">按鈕集合，可使用 OR 進行組裝</param>
        /// <param name="msgStyle">圖案樣式，無、訊息、警告、詢問、錯誤</param>
        /// <returns>使用者最後按下的按鈕</returns>
        public static MsgBoxButton Show(string title, string content, float fontSize, MsgBoxButton buttons = MsgBoxButton.OK, MsgBoxStyle msgStyle = MsgBoxStyle.NONE) {
            MsgBoxButton uiBtn = MsgBoxButton.OK;
            using (CtMessage_Ctrl mMsgBox = new CtMessage_Ctrl(title, content, fontSize, buttons, msgStyle)) {
                mMsgBox.ShowDialog();
                uiBtn = mMsgBox.UIResult;
            }
            return uiBtn;
        }

        /// <summary>顯示自訂的對話視窗，自訂字型名稱與大小</summary>
        /// <param name="title">欲顯示於對話視窗上的標題</param>
        /// <param name="content">欲顯示的內容</param>
        /// <param name="fontName">自訂的字型名稱</param>
        /// <param name="fontSize">自訂的字體大小</param>
        /// <param name="buttons">按鈕集合，可使用 OR 進行組裝</param>
        /// <param name="msgStyle">圖案樣式，無、訊息、警告、詢問、錯誤</param>
        /// <returns>使用者最後按下的按鈕</returns>
        public static MsgBoxButton Show(string title, string content, string fontName, float fontSize, MsgBoxButton buttons = MsgBoxButton.OK, MsgBoxStyle msgStyle = MsgBoxStyle.NONE) {
            MsgBoxButton uiBtn = MsgBoxButton.OK;
            using (CtMessage_Ctrl mMsgBox = new CtMessage_Ctrl(title, content, fontName, fontSize, buttons, msgStyle)) {
                mMsgBox.ShowDialog();
                uiBtn = mMsgBox.UIResult;
            }
            return uiBtn;
        }

        /// <summary>顯示自訂的對話視窗，自訂字型名稱與大小</summary>
        /// <param name="title">欲顯示於對話視窗上的標題</param>
        /// <param name="content">欲顯示的內容</param>
        /// <param name="font">自訂的字體物件</param>
        /// <param name="buttons">按鈕集合，可使用 OR 進行組裝</param>
        /// <param name="msgStyle">圖案樣式，無、訊息、警告、詢問、錯誤</param>
        /// <returns>使用者最後按下的按鈕</returns>
        public static MsgBoxButton Show(string title, string content, Font font, MsgBoxButton buttons = MsgBoxButton.OK, MsgBoxStyle msgStyle = MsgBoxStyle.NONE) {
            MsgBoxButton uiBtn = MsgBoxButton.OK;
            using (CtMessage_Ctrl mMsgBox = new CtMessage_Ctrl(title, content, font, buttons, msgStyle)) {
                mMsgBox.ShowDialog();
                uiBtn = mMsgBox.UIResult;
            }
            return uiBtn;
        }

        /// <summary>顯示自訂的對話視窗，以預設的字型為主(微軟正黑體、12pt)</summary>
        /// <param name="title">欲顯示於對話視窗上的標題</param>
        /// <param name="content">欲顯示的內容</param>
        /// <param name="buttons">按鈕集合，可使用 OR 進行組裝</param>
        /// <param name="msgStyle">圖案樣式，無、訊息、警告、詢問、錯誤</param>
        /// <returns>使用者最後按下的按鈕</returns>
        public static MsgBoxButton Show(string title, IEnumerable<string> content, MsgBoxButton buttons = MsgBoxButton.OK, MsgBoxStyle msgStyle = MsgBoxStyle.NONE) {
            MsgBoxButton uiBtn = MsgBoxButton.OK;
            using (CtMessage_Ctrl mMsgBox = new CtMessage_Ctrl(title, content, buttons, msgStyle)) {
                mMsgBox.ShowDialog();
                uiBtn = mMsgBox.UIResult;
            }
            return uiBtn;
        }

        /// <summary>顯示自訂的對話視窗，字型以預設的字體(微軟正黑體)並自訂字體大小</summary>
        /// <param name="title">欲顯示於對話視窗上的標題</param>
        /// <param name="content">欲顯示的內容</param>
        /// <param name="fontSize">自訂的字體大小</param>
        /// <param name="buttons">按鈕集合，可使用 OR 進行組裝</param>
        /// <param name="msgStyle">圖案樣式，無、訊息、警告、詢問、錯誤</param>
        /// <returns>使用者最後按下的按鈕</returns>
        public static MsgBoxButton Show(string title, IEnumerable<string> content, float fontSize, MsgBoxButton buttons = MsgBoxButton.OK, MsgBoxStyle msgStyle = MsgBoxStyle.NONE) {
            MsgBoxButton uiBtn = MsgBoxButton.OK;
            using (CtMessage_Ctrl mMsgBox = new CtMessage_Ctrl(title, content, fontSize, buttons, msgStyle)) {
                mMsgBox.ShowDialog();
                uiBtn = mMsgBox.UIResult;
            }
            return uiBtn;
        }

        /// <summary>顯示自訂的對話視窗，自訂字型名稱與大小</summary>
        /// <param name="title">欲顯示於對話視窗上的標題</param>
        /// <param name="content">欲顯示的內容</param>
        /// <param name="fontName">自訂的字型名稱</param>
        /// <param name="fontSize">自訂的字體大小</param>
        /// <param name="buttons">按鈕集合，可使用 OR 進行組裝</param>
        /// <param name="msgStyle">圖案樣式，無、訊息、警告、詢問、錯誤</param>
        /// <returns>使用者最後按下的按鈕</returns>
        public static MsgBoxButton Show(string title, IEnumerable<string> content, string fontName, float fontSize, MsgBoxButton buttons = MsgBoxButton.OK, MsgBoxStyle msgStyle = MsgBoxStyle.NONE) {
            MsgBoxButton uiBtn = MsgBoxButton.OK;
            using (CtMessage_Ctrl mMsgBox = new CtMessage_Ctrl(title, content, fontName, fontSize, buttons, msgStyle)) {
                mMsgBox.ShowDialog();
                uiBtn = mMsgBox.UIResult;
            }
            return uiBtn;
        }

        /// <summary>顯示自訂的對話視窗，自訂字型名稱與大小</summary>
        /// <param name="title">欲顯示於對話視窗上的標題</param>
        /// <param name="content">欲顯示的內容</param>
        /// <param name="font">自訂的字體物件</param>
        /// <param name="buttons">按鈕集合，可使用 OR 進行組裝</param>
        /// <param name="msgStyle">圖案樣式，無、訊息、警告、詢問、錯誤</param>
        /// <returns>使用者最後按下的按鈕</returns>
        public static MsgBoxButton Show(string title, IEnumerable<string> content, Font font, MsgBoxButton buttons = MsgBoxButton.OK, MsgBoxStyle msgStyle = MsgBoxStyle.NONE) {
            MsgBoxButton uiBtn = MsgBoxButton.OK;
            using (CtMessage_Ctrl mMsgBox = new CtMessage_Ctrl(title, content, font, buttons, msgStyle)) {
                mMsgBox.ShowDialog();
                uiBtn = mMsgBox.UIResult;
            }
            return uiBtn;
        }
    }
}
