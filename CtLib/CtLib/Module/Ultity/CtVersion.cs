using System;

namespace CtLib.Module.Ultity
{

    /// <summary>版本資訊介面</summary>
    /// <remarks>請於各 CtLib 或 CAMPro 繼承之，於 Assembly 將可自動讀取各模組之版本訊息</remarks>
    public interface ICtVersion
    {
        /// <summary>取得版本訊息</summary>
        CtVersion Version { get; }
    }

    /// <summary>CAMPro 版本資訊紀錄</summary>
    /// <remarks><code>
    /// 1.0.0  Ahern [2014/07/20]
    ///     + 建立CtVersion模組以儲存相關版本資訊
    /// </code></remarks>
    public class CtVersion
    {

        #region Declaration - Properties

        /// <summary>取得或設定「版本」</summary>
        public uint @Version
        {
            get { return mVer; }
            set { mVer = value; }
        }

        /// <summary>取得或設定「修訂」</summary>
        public uint Revision
        {
            get { return mRev; }
            set { mRev = value; }
        }

        /// <summary>取得或設定「修改」</summary>
        public uint Edit
        {
            get { return mEdit; }
            set { mEdit = value; }
        }

        /// <summary>取得或設定「作者」</summary>
        public string Author
        {
            get { return mAuthor; }
            set { mAuthor = value; }
        }

        /// <summary>取得或設定「最後編輯日期」</summary>
        public DateTime Date
        {
            get { return mDate; }
            set { mDate = value; }
        }

        /// <summary>取得版本字串，如 "1.2.100"</summary>
        public string FullString
        {
            get { return (mVer.ToString() + "." + mRev.ToString() + "." + mEdit.ToString()); }
        }

        /// <summary>取得父層類別名稱</summary>
        public string ParentName
        {
            get
            {
                return GetType().BaseType.Name ?? "";
            }
        }

        #endregion

        #region Declaration - Members

        private uint mVer = 0;
        private uint mRev = 0;
        private uint mEdit = 0;
        private DateTime mDate = DateTime.Now;
        private string mAuthor = "";

        #endregion

        #region Functions - Core

        /// <summary>建立一個全新的 CtVersion 以儲存相關版本資訊</summary>
        public CtVersion() { }

        /// <summary>建立一個 CtVersion 以儲存相關版本資訊</summary>
        /// <param name="ver">版本，Version</param>
        /// <param name="rev">修訂，Revision</param>
        /// <param name="edit">修改，Edit</param>
        /// <param name="date">最後修改日期</param>
        /// <param name="author">作者(可忽略)</param>
        public CtVersion(uint ver, uint rev, uint edit, DateTime date, string author = "")
        {
            mVer = ver;
            mRev = rev;
            mEdit = edit;
            mDate = date;
            mAuthor = author;
        }
        /// <summary>建立一個 CtVersion 以儲存相關版本資訊</summary>
        /// <param name="ver">版本，Version</param>
        /// <param name="rev">修訂，Revision</param>
        /// <param name="edit">修改，Edit</param>
        /// <param name="date">最後修改日期，格式如"2014/07/28"</param>
        /// <param name="author">作者(可忽略)</param>
        public CtVersion(uint ver, uint rev, uint edit, string date, string author = "")
        {
            mVer = ver;
            mRev = rev;
            mEdit = edit;
            mDate = DateTime.Parse(date);
            mAuthor = author;
        }

        #endregion

        #region Function - ToString Override
        /// <summary>將當前版本訊息以字串表示。如 "1.3.6"</summary>
        /// <returns>版本字串</returns>
        public override string ToString()
        {
            return (mVer.ToString() + "." + mRev.ToString() + "." + mEdit.ToString());
        }
        #endregion
    }
}
