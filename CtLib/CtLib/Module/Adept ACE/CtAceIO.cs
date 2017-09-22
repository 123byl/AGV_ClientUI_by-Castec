using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Ace.Adept.Server.Controls;
using Ace.Adept.Server.Motion.Robots;

using CtLib.Library;

namespace CtLib.Module.Adept {

    /// <summary>
    /// Adept Robot 與 SmartController 之 I/O 控制
    /// <para>此部分提供多種讀取與寫入I/O方法</para>
    /// </summary>
    public class CtAceIO {

        #region Declaration - Properties
        /// <summary>取得或設定是否含有SmartController</summary>
        public bool SmartController { get; set; }
        #endregion

        #region Declaration - Members
        /// <summary>SmartController，如果含有控制器，使用此物件控制相關I/O</summary>
        private IAdeptController mICtrl;
        /// <summary>iCobra，如果不含控制器(如i600)，使用此物件控制相關I/O</summary>
        private iCobra miCobra;
        #endregion

        #region Function - Constructors
        /// <summary>建立 Adept I/O 相關控制</summary>
        /// <param name="smartCtrl">是否含有SmartController  (True)有控制器 (False)無</param>
        /// <param name="obj">已連線之IAdeptController或iCobra</param>
        public CtAceIO(bool smartCtrl, object obj) {
            SmartController = smartCtrl;

            if (smartCtrl) mICtrl = obj as IAdeptController;
            else miCobra = obj as iCobra;
        }
        #endregion

        #region Function - I/O

        /// <summary>
        /// 更改當前I/O狀態。
        /// 正整數表示開啟該I/O，負整數表關閉。如 2001 = 開啟2001; -2014 = 關閉2014
        /// </summary>
        /// <param name="io">
        /// 欲更改之I/O編號。
        /// 可帶入陣列，如 int[] IOs = new int[] {2001, -2002, -2003, 2004}
        /// 也可採用逗號分隔，如 SetIO(2001, -2002, -2003, 2004)
        /// </param>
        /// <example><code>
        /// CtAce mAce = new CtAce();
        /// 
        /// List&lt;int&gt; ioList = new List&lt;int&gt; { 43, -44, -97, 98 };
        /// mAce.IO.SetIO(ioList.ToArray());    //設定 43(ON) 44(OFF) 97(OFF) 98(ON)
        /// mAce.IO.SetIO(2001, -2002);         //設定 2001(ON) 2002(OFF)
        /// </code></example>
        public void SetIO(params int[] io) {
            if (!SmartController)
                miCobra.SetDigitalIO(io);
            else {
                if (mICtrl != null) {
                    lock (mICtrl) {
                        mICtrl.SetDigitalIO(io);
                    }
                }
            }
        }

        /// <summary>
        /// 更改當前I/O狀態。以布林(Boolean)表示ON/OFF狀態
        /// </summary>
        /// <param name="io">欲更改之I/O編號</param>
        /// <param name="stt">(True)ON   (False)OFF</param>
        /// <example><code>
        /// CtAce mAce = new CtAce();
        /// 
        /// mAce.IO.SetIO(97, false);   //設定 IO 編號 97 為 OFF
        /// mAce.IO.SetIO(43, true);    //設定 IO 編號 43 為 ON
        /// </code></example>
        public void SetIO(int io, bool stt) {
            if (!SmartController)
                miCobra.SetDigitalIO(io * ((stt) ? 1 : -1));
            else {
                if (mICtrl != null) {
                    lock (mICtrl) {
                        mICtrl.SetDigitalIO(io * ((stt) ? 1 : -1));
                    }
                }
            }
        }

        /// <summary>
        /// 更改多個I/O狀態。以布林(Boolean)集合表示ON/OFF狀態
        /// </summary>
        /// <param name="ioList">欲更改之I/O編號集合</param>
        /// <param name="ioStt">相對應 ioList 順序之狀態  (True)ON   (False)OFF</param>
        /// <example><code>
        /// CtAce mAce = new CtAce();
        /// 
        /// List&lt;int&gt; ioList = new List&lt;int&gt; { 2001, 2002, 2003, 2004, 2005 };
        /// List&lt;bool&gt; ioStt = new List&lt;bool&gt; { false, true, true, false, false };
        /// mAce.IO.SetIO(ioList, ioStt);   //設定 2001(OFF) 2002(ON) 2003(ON) 2004(OFF) 2005(OFF)
        /// </code></example>
        public void SetIO(List<int> ioList, List<bool> ioStt) {
            if (ioList.Count == ioStt.Count) {
                List<int> ioTemp = new List<int>();
                for (int idx = 0; idx < ioList.Count; idx++)
                    ioTemp.Add(ioList[idx] * ((ioStt[idx]) ? 1 : -1));

                if (!SmartController)
                    miCobra.SetDigitalIO(ioTemp.ToArray());
                else {
                    if (mICtrl != null) {
                        lock (mICtrl) {
                            mICtrl.SetDigitalIO(ioTemp.ToArray());
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 更改多個I/O狀態。以 <see cref="Dictionary&lt;TKey,TValue&gt;"/> 表示 I/O 及其欲更改之狀態
        /// </summary>
        /// <param name="ioDict">欲更改之 I/O 編號及其對應狀態</param>
        /// <example><code>
        /// CtAce mAce = new CtAce();
        /// 
        /// Dictionary&lt;int, bool&gt; ioList = new Dictionary&lt;int, bool&gt; { {2001, true}, {2002, true}, {2003, false} };
        /// mAce.IO.SetIO(ioList);  //設定 2001(ON) 2002(ON) 2003(OFF)
        /// </code></example>
        public void SetIO(Dictionary<int, bool> ioDict) {
            List<int> ioTemp = new List<int>();
            foreach (KeyValuePair<int, bool> io in ioDict) {
                ioTemp.Add(io.Key * (io.Value ? 1 : -1));
            }

            if (!SmartController)
                miCobra.SetDigitalIO(ioTemp.ToArray());
            else {
                if (mICtrl != null) {
                    lock (mICtrl) {
                        mICtrl.SetDigitalIO(ioTemp.ToArray());
                    }
                }
            }
        }

        /// <summary>取得當前單一I/O狀態</summary>
        /// <param name="io">欲查詢之I/O編號</param>
        /// <param name="value">回傳之當前 On(True) / Off(False) 狀態</param>
        /// <example><code>
        /// CtAce mAce = new CtAce();
        /// bool stt;
        /// mAce.IO.GetIO(97, out stt); //將 I/O 編號 97 之狀態存放至 stt 變數裡
        /// </code></example>
        public void GetIO(int io, out bool value) {
            bool bolTemp = false;
            if (!SmartController) {
                bolTemp = miCobra.GetDigitalIO(io);
            } else {
                if (mICtrl != null) {
                    lock (mICtrl) {
                        bolTemp = mICtrl.GetDigitalIO(io);
                    }
                }
            }
            value = bolTemp;
        }

        /// <summary>取得當前單一I/O狀態，並直接回傳該布林值(Boolean)</summary>
        /// <param name="io">欲查詢之I/O編號</param>
        /// <returns>回傳該布林值(Boolean)。 (True)ON; (False)OFF</returns>
        /// /// <example><code>
        /// CtAce mAce = new CtAce();
        /// bool stt = mAce.IO.GetIO(97); //將 I/O 編號 97 之狀態存放至 stt 變數裡
        /// </code></example>
        public bool GetIO(int io) {
            bool bolTemp = false;
            /*-- 因為沒有回傳Status，所以這邊不判斷mICtrl/iCobra是不是有建立，如有錯則直接上他跳Exception --*/
            if (!SmartController) {
                bolTemp = miCobra.GetDigitalIO(io);
            } else {
                lock (mICtrl) {
                    bolTemp = mICtrl.GetDigitalIO(io);
                }
            }
            return bolTemp;
        }

        /// <summary>取得當前多個I/O狀態</summary>
        /// <param name="io">欲查詢之I/O編號陣列，如 int[] IOs = new int[] {94, 96, 105, 106}</param>
        /// <param name="value">回傳之當前 On(True) / Off(False) 集合陣列</param>
        /// <returns>Status Code</returns>
        /// <example><code>
        /// CtAce mAce = new CtAce();
        /// 
        /// int[] ioList = new int[] { 94, 96, 105, 106 };
        /// bool[] ioStt;
        /// mAce.IO.GetIO(ioList, out ioStt);
        /// </code></example>
        public void GetIO(int[] io, out bool[] value) {
            bool[] bolTemp = null;
            if (!SmartController) {
                bolTemp = new bool[io.Length];
                for (int i = 0; i < io.Length; i++) {
                    bolTemp[i] = GetIO(io[i]);
                }
            } else {
                if (mICtrl != null) {
                    lock (mICtrl) {
                        bolTemp = mICtrl.GetDigitalIOs(io);
                    }
                }
            }
            value = bolTemp;
        }

        /// <summary>取得當前多個I/O狀態，並直接回傳該布林值(Boolean)陣列</summary>
        /// <param name="io">欲查詢之I/O編號陣列，如 int[] IOs = new int[] {94, 96, 105, 106}</param>
        /// <returns>回傳該布林值(Boolean)陣列。 (True)ON; (False)OFF</returns>
        /// <example><code>
        /// CtAce mAce = new CtAce();
        /// List&lt;bool&gt; ioStt = mAce.IO.GetIO(94, 96, 105, 106);
        /// </code></example>
        public List<bool> GetIO(params int[] io) {
            List<bool> bolTemp = new List<bool>();
            if (!SmartController) {
                for (int i = 0; i < io.Length; i++) {
                    bolTemp.Add(GetIO(io[i]));
                }
            } else {
                lock (mICtrl) {
                    bolTemp = mICtrl.GetDigitalIOs(io).ToList();
                }
            }
            return bolTemp;
        }

        /// <summary>取得當前多個I/O狀態</summary>
        /// <param name="io">欲查詢之I/O編號陣列，如 int[] IOs = new int[] {94, 96, 105, 106}</param>
        /// <param name="value">回傳之當前 On(True) / Off(False) List集合</param>
        /// <example><code>
        /// CtAce mAce = new CtAce();
        /// 
        /// int[] ioList = new int[] { 94, 96, 105, 106 };
        /// List&lt;bool&gt; ioStt;
        /// mAce.IO.GetIO(ioList, out ioStt);
        /// </code></example>
        public void GetIO(int[] io, out List<bool> value) {
            List<bool> bolTemp = null;
            if (!SmartController) {
                bolTemp = new List<bool>();
                for (int i = 0; i < io.Length; i++) {
                    bolTemp.Add(GetIO(io[i]));
                }
            } else {
                if (mICtrl != null) {
                    lock (mICtrl) {
                        bolTemp = mICtrl.GetDigitalIOs(io).ToList();
                    }
                }
            }
            value = bolTemp;
        }
        #endregion
    }
}
