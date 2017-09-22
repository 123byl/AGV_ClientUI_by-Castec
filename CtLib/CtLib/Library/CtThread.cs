using CtLib.Module.Ultity;
using System;
using System.Threading;
//using System.Threading.Tasks;
using System.Windows.Forms;

namespace CtLib.Library {

    /// <summary>
    /// 執行緒相關方法
    /// <para>包含建立、關閉執行緒等方法</para>
    /// <para>長效型執行緒建議使用傳統 Thread，而若是有工作週期者建議使用 Task (.Net 4+) 或 ThreadPool (.Net 3.5+)</para>
    /// </summary>
    /// <remarks>
    /// 由於 Task 的部分比較龐大，他支援許多多載且具有多種回傳型態(<see cref="Action"/>、<see cref="Action&lt;TResult&gt;"/>、<see cref="Func&lt;TResult&gt;"/>、<see cref="Action&lt;T1, TResult&gt;"/> 等)
    /// <para>有些部分在此無法完整寫出，例如 Task.ContinueWith(Action&lt;Task&gt;) 等方法，或許是方法錯誤無法正確加入佇列，未來有機會再繼續補齊</para>
    /// <code>
    /// private void tsk_A() {
    ///     ;
    /// }
    /// 
    /// private void tsk_B() {
    ///     ;
    /// }
    /// 
    /// Task task = CtThread.AddTask(tsk_A);
    /// task.ContinueWith(tsk => tsk_B);        //當 tsk_A 完成後會自動接續執行 tsk_B
    /// </code>
    /// <code>
    /// Task taskA = CtThread.AddTask(tsk_A);
    /// Task taskB = CtThread.AddTask(tsk_B);
    /// Task.WaitAny(taskA, taskB);             //等待所有 taskA 和 taskB 完成
    /// Task.WaitAll(taskA, taskB);             //等待 taskA 或 taskB 任一個完成
    /// </code>
    /// <code>
    /// async Task tsk() {      //宣告此段程式為不同步執行
    ///     ;
    /// }
    /// 
    /// await tsk();            //主程式下此行命令後會等待 tsk() 執行完畢
    /// </code>
    /// </remarks>
    /// <example>
    /// 建立與關閉執行緒請看各方法。
    /// <para>以下示範執行緒之暫停與繼續</para>
    /// <para> </para>
    /// <para>1. 使用 Sleep 與 Interrupt</para>
    /// <code>
    /// ///&lt;summary&gt;主程式&lt;/summary&gt;
    /// static void Main() {
    ///     Thread mThread = CtThread.CreateThread("ThreadName", tsk);  //建立執行緒並執行
    ///     Thread.Sleep(100);
    ///     Console.Wrtie("Waiting for enter");
    ///     Console.ReadLine();
    ///     mThread.Interrupt();    //使用 Interrupt 會將執行緒目前狀態中斷，故可以用來喚醒執行緒
    /// }
    /// 
    /// ///&lt;summary&gt;執行緒執行的程式&lt;/summary&gt;
    /// static void tsk() {
    ///     Console.WriteLine("Thread Executing...");
    ///     
    ///     try {
    ///         Thread.Sleep(Timeout.Infinite);     //進行無止盡睡眠，執行緒將 Hold 在這
    ///     } catch (ThreadInterruptException) {
    ///         //使用 Interrupt 喚醒時會觸發 ThreadInterruptException，純屬正常現象，故這邊不做任何動作
    ///     }
    ///     
    ///     Console.WrtieLine("Awake!");
    /// }
    /// 
    /// /*----------- Output Window -----------*
    /// ┌────────────────────────────────┐
    /// │ Thread Executing...            │
    /// │ Waiting for enter _            │   &lt;&lt; 於此段文字顯示後，按下 Enter 鍵
    /// │ Awake!                         │
    /// │                                │
    /// └────────────────────────────────┘
    /// *-------------------------------------*/
    /// </code>
    /// 2. 使用 <see cref="AutoResetEvent"/> 或 <seealso cref="ManualResetEvent"/> 方法
    /// <code>
    /// AutoResetEvent thrHandle = null;
    /// 
    /// static void Main() {
    ///     CtThread.AddWorkItem(tsk);              //透過 ThreadPool 喚醒執行緒執行 tsk 副程式
    ///     Thread.Sleep(100);
    ///     Console.Wrtie("Waiting for enter");
    ///     Console.ReadLine();                     //等待使用者按下 Enter 鍵
    ///     thrHandle.Set();                        //觸發執行緒訊號，喚醒執行緒。即「繼續」
    /// }
    /// 
    /// static void tsk() {
    ///     Console.WriteLine("Thread Executing...");
    ///     
    ///     thrHandle = new AutoResetEvent(false);
    ///     thrHandle.WaitOne();                    //進入等待訊號狀態。即是「暫停」或「休眠」
    /// 
    ///     Console.WrtieLine("Awake!");
    /// }
    /// 
    /// /*----------- Output Window -----------*
    /// ┌────────────────────────────────┐
    /// │ Thread Executing...            │
    /// │ Waiting for enter _            │   &lt;&lt; 於此段文字顯示後，按下 Enter 鍵
    /// │ Awake!                         │
    /// │                                │
    /// └────────────────────────────────┘
    /// *-------------------------------------*/
    /// </code>
    /// 方法 2 之 <see cref="AutoResetEvent"/> 或 <seealso cref="ManualResetEvent"/>
    /// <para>差別在於 AutoResetEvent.Set() 喚醒時 <see cref="AutoResetEvent"/> 對象是單一執行緒</para>
    /// <para>如果 <see cref="AutoResetEvent"/> 是建立於個別執行緒，則對該執行緒喚醒</para>
    /// <para>但如果 <see cref="AutoResetEvent"/> 建立於共用方法中，例如建立於 void Main() 中，則針對最後一個執行緒作動</para>
    /// <para> </para>
    /// <para>而 <see cref="ManualResetEvent"/> 之 ManualResetEvent.Set() 對象是全域的執行緒</para>
    /// <para>施作 ManualResetEvent.Set() 後將喚醒所有不管是處於 AutoResetEvent.WaitOne() 還是 ManualResetEvent.WaitOne() 之執行緒</para>
    /// </example>
    public static class CtThread {

        #region Version

        /// <summary>CtThread 版本訊息</summary>
        /// <remarks><code>
        /// 1.0.0  Ahern [2014/07/15]
        ///     + 將執行緒相關操作方法獨立至此
        ///     
        /// 1.0.1  Ahern [2015/03/23]
        ///     \ ThreadStart 與 ParameterizedThreadStart 以 Action 取代之
        /// 
        /// 1.1.0  Ahern [2015/03/24]
        ///     + ThreadPool 相關方法
        ///     + Tasks 相關方法
        ///     
        /// 1.1.1  Ahern [2015/05/31]
        ///     \ KillThread 加上 timeout 參數
        /// 
        /// </code></remarks>
        public static readonly CtVersion @Version = new CtVersion(1, 1, 1, "2015/05/31", "Ahern Kuo");

        #endregion

        #region Functions - Core
        /// <summary>建立執行緒，並將該執行緒之函數指向 "不需" 帶參數之副程式</summary>
        /// <param name="thread">欲建立之執行緒。需使用ref關鍵字，將外部物件傳入以建立，否則建立之物件將不屬於Owner</param>
        /// <param name="name">該執行緒名稱</param>
        /// <param name="method">委派副程式位址，指向不需帶參數、無回傳值之副程式</param>
        /// <param name="background">是否為背景執行緒。  (True)背景執行緒，將依附在Owner(前景執行緒)上，關閉時隨之關閉   (False)前景執行緒，不依附於任何Owner/Thread上</param>
        /// <param name="start">是否建立後直接開始?  (True)直接開始  (False)由外部控制該Thread執行</param>
        /// <example>
        /// 以下示範簡單的 Thread 使用方式
        /// <code>
        /// private void tsk() {
        ///     ;
        /// }
        /// 
        /// Thread thread;  //建立執行緒
        /// CtThread.CreateThread(ref thread, "ThreadName", tsk);    //建立執行緒並直接執行
        /// </code></example>
        /// <remarks>
        /// <see cref="System.Threading.ThreadStart"/> 為指向不帶參數之方法的委派
        /// </remarks>
        public static void CreateThread(ref Thread thread, string name, Action method, bool background = true, bool start = true) {
            if (thread != null) KillThread(ref thread,1000);
            thread = new Thread(new ThreadStart(method));
            thread.IsBackground = background;
            thread.Name = name;
            if (start) thread.Start();
        }

        /// <summary>建立執行緒，並將該執行緒之函數指向 "需" 帶參數之副程式。須由外部啟動執行緒並帶入參數，如 "thread.Start(100)"</summary>
        /// <param name="thread">欲建立之執行緒。需使用ref關鍵字，將外部物件傳入以建立，否則建立之物件將不屬於Owner</param>
        /// <param name="name">該執行緒名稱</param>
        /// <param name="method">委派副程式位址，指向帶有單一參數、無回傳值且型態為 <see cref="object"/> 之副程式</param>
        /// <param name="background">是否為背景執行緒  (True)背景執行緒，將依附在Owner(前景執行緒)上，關閉時隨之關閉  (False)前景執行緒，不依附於任何Owner/Thread上</param>
        /// <example>
        /// 以下示範簡單的 Thread 使用方式
        /// <code>
        /// private void tsk(object val) {
        ///     ;
        /// }
        /// 
        /// Thread thread;  //建立執行緒
        /// CtThread.CreateThread(ref thread, "ThreadName", tsk);   //建立執行緒，但尚未開始執行
        /// thread.Start("帶入參數");                                //開始執行，並將字串 "帶入參數" 丟進 tsk 副程式裡
        /// </code>
        /// </example>
        /// <remarks>
        /// <see cref="System.Threading.ParameterizedThreadStart"/>為指向可帶參數之方法的委派
        /// </remarks>

        public static void CreateThread(ref Thread thread, string name, Action<object> method, bool background = true) {
            if (thread != null) KillThread(ref thread);
            thread = new Thread(new ParameterizedThreadStart(method));
            thread.IsBackground = background;
            thread.Name = name;
        }

        /// <summary>回傳新建之執行緒，並將該執行緒之函數指向 "不需" 帶參數之副程式</summary>
        /// <param name="name">該執行緒名稱</param>
        /// <param name="method">委派副程式位址，指向不需帶參數、無回傳值之副程式</param>
        /// <param name="background">是否為背景執行緒  (True)背景執行緒，將依附在Owner(前景執行緒)上，關閉時隨之關閉  (False)前景執行緒，不依附於任何Owner/Thread上</param>
        /// <param name="start">是否建立後直接開始?  (True)直接開始  (False)由外部控制該Thread執行</param>
        /// <example><code>
        /// private void tsk() {
        ///     ;
        /// }
        /// 
        /// Thread thread = CtThread.CreateThread("ThreadName", tsk);   //建立執行緒並直接執行
        /// </code></example>
        public static Thread CreateThread(string name, Action method, bool background = true, bool start = true) {
            Thread thread;

            thread = new Thread(new ThreadStart(method));
            thread.IsBackground = background;
            thread.Name = name;
            if (start) thread.Start();

            return thread;
        }

        /// <summary>回傳新建之執行緒，並將該執行緒之函數指向 "需" 帶參數之副程式。須由外部啟動執行緒並帶入參數，如 "thread.Start(100)"</summary>
        /// <param name="name">該執行緒名稱</param>
        /// <param name="method">委派副程式位址，指向帶有單一參數、無回傳值且型態為 <see cref="object"/> 之副程式</param>
        /// <param name="background">是否為背景執行緒。(True)背景執行緒，將依附在Owner(前景執行緒)上，關閉時隨之關閉  (False)前景執行緒，不依附於任何Owner/Thread上</param>
        /// <example><code>
        /// private void tsk(object obj) {
        ///     ;
        /// }
        /// 
        /// Thread thread = CtThread.CreateThread("ThreadName", tsk)    //建立執行緒但尚未執行
        /// thread.Start(false);                                        //開始執行，並將布林值 "false" 丟進 tsk 副程式裡
        /// </code></example>
        public static Thread CreateThread(string name, Action<object> method, bool background = true) {
            Thread thread;

            thread = new Thread(new ParameterizedThreadStart(method));
            thread.IsBackground = background;
            thread.Name = name;

            return thread;
        }

        /// <summary>關閉執行緒</summary>
        /// <param name="thread">欲關閉之執行緒，需帶入ref關鍵字以帶入該物件記憶體位置，避免此處關閉但外實際上沒有之窘境</param>
        /// <param name="timeout">等待 Thread 完整關閉的時間，超過時間將忽略並繼續往下。 "-1" 表示忽略，等待完全停止</param>
        /// <returns>Status Code</returns>
        public static Stat KillThread(ref Thread thread, int timeout = -1) {
            Stat stt = Stat.SUCCESS;
            try {
                /*-- 檢查是否該執行緒真的存在 --*/
                if (thread != null) {
                    thread.Interrupt(); //插斷執行緒內迴圈
                    thread.Abort();     //停止任何工作，但工作並非馬上會結束
                    if (timeout > 0) thread.Join(timeout); //等待該執行緒丟出ThreadInterruptedException表示已完整結束
                    else thread.Join();
                }

                /*-- 延遲一下下 --*/
                //Application.DoEvents();
                Thread.Sleep(1);
            } catch (ThreadAbortException) {
                /*-- Abort Exception 是關閉時很有可能發生的，這裡的 catch 用來防止 --*/
            } catch (ThreadInterruptedException) {
                /*-- Interrupt Exception 是關閉時很有可能發生的，這裡的 catch 用來防止 --*/
            } 
            catch (Exception ex) {
                stt = Stat.ER_SYSTEM;
                CtStatus.Report(stt, ex);
            } finally {

                /*-- 再次檢查 --*/
                if ((thread != null) && (thread.IsAlive)) {
                    stt = Stat.ER_SYS_KILLPR;  //如果還活著表示沒有關成功
                } else {
                    stt = Stat.SUCCESS;
                    thread = null;             //關閉成功則將記憶體清掉，以利後續重複利用
                }
            }

            return stt;
        }

        /// <summary>
        /// 透過 <see cref="ThreadPool"/> 喚醒休眠中的執行緒(<see cref="Thread"/>)執行副程式
        /// <para>「不建議」帶入長效型副程式，如監控 I/O 等；「建議」用於計算、UI 等使用完即結束之方法</para>
        /// </summary>
        /// <param name="method">委派副程式位址，指向不需帶參數、無回傳值之副程式</param>
        /// <returns>(True)成功加入佇列  (False)無法佇列，且會觸發 <see cref="NotSupportedException"/></returns>
        /// <remarks>
        /// 此方法直接使用 .Net Framework 3.5 SP1 之 <see cref="ThreadPool"/>，如專案為低於 3.5 SP1 之版本請移除或註解此段程式
        /// </remarks>
        /// <example><code>
        /// private void tsk() {
        ///     ;
        /// }
        /// 
        /// CtThread.AddWorkItem(tsk);  //由 ThreadPool 執行 tsk 副程式
        /// </code></example>
        public static bool AddWorkItem(Action<object> method) {
            return ThreadPool.QueueUserWorkItem(new WaitCallback(method));
        }

        /// <summary>
        /// 透過 <see cref="ThreadPool"/> 喚醒休眠中的執行緒(<see cref="Thread"/>)執行副程式
        /// <para>「不建議」帶入長效型副程式，如監控 I/O 等；「建議」用於計算、UI 等使用完即結束之方法</para>
        /// </summary>
        /// <param name="method">委派副程式位址，指向帶有單一參數、無回傳值且型態為 <see cref="object"/> 之副程式</param>
        /// <param name="passInValue">欲傳入副程式之物件</param>
        /// <returns>(True)成功加入佇列  (False)無法佇列，且會觸發 <see cref="NotSupportedException"/></returns>
        /// <remarks>
        /// 此方法直接使用 .Net Framework 3.5 SP1 之 <see cref="ThreadPool"/>，如專案為低於 3.5 SP1 之版本請移除或註解此段程式
        /// </remarks>
        /// <example><code>
        /// private void tsk(object obj) {
        ///     ;
        /// }
        /// 
        /// CtThread.AddWorkItem(tsk, "Parameter"); //由 ThreadPool 執行 tsk 副程式，並帶入引數 "Parameter" 字串
        /// </code></example>
        public static bool AddWorkItem(Action<object> method, object passInValue) {
            return ThreadPool.QueueUserWorkItem(new WaitCallback(method), passInValue);
        }

        ///// <summary>
        ///// 透過 <see cref="Task"/> 喚醒休眠中的執行緒(<see cref="Thread"/>)執行副程式
        ///// <para>「不建議」帶入長效型副程式，如監控 I/O 等；「建議」用於計算、UI 等使用完即結束之方法</para>
        ///// </summary>
        ///// <param name="method">委派副程式位址，指向不需帶參數、無回傳值之副程式</param>
        ///// <param name="auto">是否自動執行?  (True)自動執行  (False)手動執行</param>
        ///// <returns>因應此方法所建立之 <see cref="Task"/></returns>
        ///// <remarks>
        ///// 此方法直接使用 .Net Framework 4.0 之 <see cref="Task"/>，如專案為低於 4.0 之版本請移除或註解此段程式
        ///// </remarks>
        ///// <example><code>
        ///// private void tsk() {
        /////     ;
        ///// }
        ///// 
        ///// CtThread.AddTask(tsk);  //由 Task 執行 tsk 副程式
        ///// </code></example>
        //public static Task AddTask(Action method, bool auto = true) {
        //    if (auto) return Task.Factory.StartNew(method);
        //    else return new Task(method);
        //}

        ///// <summary>
        ///// 透過 <see cref="Task"/> 喚醒休眠中的執行緒(<see cref="Thread"/>)執行副程式
        ///// <para>「不建議」帶入長效型副程式，如監控 I/O 等；「建議」用於計算、UI 等使用完即結束之方法</para>
        ///// </summary>
        ///// <param name="method">委派副程式位址，指向帶有單一參數、無回傳值且型態為 <see cref="object"/> 之副程式</param>
        ///// <param name="passInValue">欲傳入副程式之物件</param>
        ///// <param name="auto">是否自動執行?  (True)自動執行  (False)手動執行</param>
        ///// <returns>因應此方法所建立之 <see cref="Task"/></returns>
        ///// <remarks>
        ///// 此方法直接使用 .Net Framework 4.0 之 <see cref="Task"/>，如專案為低於 4.0 之版本請移除或註解此段程式
        ///// </remarks>
        ///// <example><code>
        ///// private void tsk(object obj) {
        /////     ;
        ///// }
        ///// 
        ///// CtThread.AddTask(tsk, "Parameter"); //由 Task 執行 tsk 副程式，並帶入引數 "Parameter" 字串
        ///// </code></example>
        //public static Task AddTask(Action<object> method, object passInValue, bool auto = true) {
        //    if (auto) return Task.Factory.StartNew(method, passInValue);
        //    else return new Task(method, passInValue);
        //}

        ///// <summary>
        ///// 透過 <see cref="Task"/> 喚醒休眠中的執行緒(<see cref="Thread"/>)執行帶有回傳值之副程式
        ///// <para>「不建議」帶入長效型副程式，如監控 I/O 等；「建議」用於計算、UI 等使用完即結束之方法</para>
        ///// </summary>
        ///// <param name="method">委派副程式位址，指向不需帶參數但「具有」回傳值之副程式</param>
        ///// <param name="auto">是否自動執行?  (True)自動執行  (False)手動執行</param>
        ///// <returns>因應此方法所建立之 <see cref="Task&lt;TResult&gt;"/></returns>
        ///// <remarks>
        ///// 此方法直接使用 .Net Framework 4.0 之 <see cref="Task&lt;TResult&gt;"/>，如專案為低於 4.0 之版本請移除或註解此段程式
        ///// </remarks>
        ///// <example><code>
        ///// private int tsk() {
        /////     return -1;
        ///// }
        ///// 
        ///// Task mTask = CtThread.AddTask(tsk);         //由 Task 執行 tsk 副程式
        ///// MessageBox.Show(mTask.Result.ToString());   //可藉由 <see cref="Task&lt;TResult&gt;.Result"/> 取得回傳值
        ///// </code></example>
        //public static Task<TResult> AddTask<TResult>(Func<TResult> method, bool auto = true) {
        //    if (auto) return Task<TResult>.Factory.StartNew(method);
        //    else return new Task<TResult>(method);
        //}

        ///// <summary>
        ///// 透過 <see cref="Task"/> 喚醒休眠中的執行緒(<see cref="Thread"/>)執行帶有回傳值之副程式
        ///// <para>「不建議」帶入長效型副程式，如監控 I/O 等；「建議」用於計算、UI 等使用完即結束之方法</para>
        ///// </summary>
        ///// <param name="method">委派副程式位址，指向「具有」引數與回傳值之副程式</param>
        ///// <param name="passInValue">欲傳入副程式之物件</param>
        ///// <param name="auto">是否自動執行?  (True)自動執行  (False)手動執行</param>
        ///// <returns>因應此方法所建立之 <see cref="Task&lt;TResult&gt;"/></returns>
        ///// <remarks>
        ///// 此方法直接使用 .Net Framework 4.0 之 <see cref="Task&lt;TResult&gt;"/>，如專案為低於 4.0 之版本請移除或註解此段程式
        ///// </remarks>
        ///// <example><code>
        ///// private int tsk(object obj) {
        /////     return ((int)obj * 10);
        ///// }
        ///// 
        ///// Task mTask = CtThread.AddTask(tsk, 9);         //由 Task 執行 tsk 副程式，並傳入整數 9
        ///// MessageBox.Show(mTask.Result.ToString());      //可藉由 <see cref="Task&lt;TResult&gt;.Result"/> 取得回傳值
        ///// </code></example>
        //public static Task<TResult> AddTask<TResult>(Func<object, TResult> method, object passInValue, bool auto = true) {
        //    if (auto) return Task<TResult>.Factory.StartNew(method, passInValue);
        //    else return new Task<TResult>(method, passInValue);
        //}

        ///// <summary>
        ///// 透過 <see cref="Task"/> 喚醒休眠中的執行緒(<see cref="Thread"/>)執行副程式
        ///// <para>「不建議」帶入長效型副程式，如監控 I/O 等；「建議」用於計算、UI 等使用完即結束之方法</para>
        ///// </summary>
        ///// <param name="method">委派副程式位址，指向不需帶參數、無回傳值之副程式</param>
        ///// <param name="auto">是否自動執行?  (True)自動執行  (False)手動執行</param>
        ///// <param name="cancelToken">取消用物件。可透過此物件發出取消訊號，立即中斷並取消當前 Task 工作</param>
        ///// <returns>因應此方法所建立之 <see cref="Task"/></returns>
        ///// <remarks>
        ///// 此方法直接使用 .Net Framework 4.0 之 <see cref="Task"/>，如專案為低於 4.0 之版本請移除或註解此段程式
        ///// </remarks>
        ///// <example><code>
        ///// private void tsk() {
        /////     try {
        /////         while (true) cnTkSrc.Token.ThrowIfCancellationRequested();  //需於程式中加入此行，將會檢查 CancelToken 是否 IsCanceled == true
        /////     } catch (Exception ex) {
        /////         Console.WriteLine(ex.Message);  //IsCanceled == true 後會觸發 OperationCanceledException
        /////     }
        ///// }
        ///// 
        ///// CancellationTokenSource cnTkSrc = new CancellationTokenSource();
        ///// CtThread.AddTask(tsk, cnTkSrc); //由 Task 執行 tsk 副程式
        ///// cnTkSrc.Cancel();               //取消當前 Task 工作，會將 IsCanceled = true
        ///// </code></example>
        //public static Task AddTask(Action method, CancellationTokenSource cancelToken, bool auto = true) {
        //    if (auto) return Task.Factory.StartNew(method, cancelToken.Token);
        //    else return new Task(method, cancelToken.Token);
        //}

        ///// <summary>
        ///// 透過 <see cref="Task"/> 喚醒休眠中的執行緒(<see cref="Thread"/>)執行副程式
        ///// <para>「不建議」帶入長效型副程式，如監控 I/O 等；「建議」用於計算、UI 等使用完即結束之方法</para>
        ///// </summary>
        ///// <param name="method">委派副程式位址，指向帶有單一參數、無回傳值且型態為 <see cref="object"/> 之副程式</param>
        ///// <param name="passInValue">欲傳入副程式之物件</param>
        ///// <param name="cancelToken">取消用物件。可透過此物件發出取消訊號，立即中斷並取消當前 Task 工作</param>
        ///// <param name="auto">是否自動執行?  (True)自動執行  (False)手動執行</param>
        ///// <returns>因應此方法所建立之 <see cref="Task"/></returns>
        ///// <remarks>
        ///// 此方法直接使用 .Net Framework 4.0 之 <see cref="Task"/>，如專案為低於 4.0 之版本請移除或註解此段程式
        ///// </remarks>
        ///// <example><code>
        ///// private void tsk(object obj) {
        /////     try {
        /////         while (true) cnTkSrc.Token.ThrowIfCancellationRequested();  //需於程式中加入此行，將會檢查 CancelToken 是否 IsCanceled == true
        /////     } catch (Exception ex) {
        /////         Console.WriteLine(ex.Message);  //IsCanceled == true 後會觸發 OperationCanceledException
        /////     }
        ///// }
        ///// 
        ///// CancellationTokenSource cnTkSrc = new CancellationTokenSource();
        ///// CtThread.AddTask(tsk, "Parameter", cnTkSrc);    //由 Task 執行 tsk 副程式，並帶入引數 "Parameter" 字串
        ///// cnTkSrc.Cancel();                               //取消當前 Task 工作，會將 IsCanceled = true
        ///// </code></example>
        //public static Task AddTask(Action<object> method, object passInValue, CancellationTokenSource cancelToken, bool auto = true) {
        //    if (auto) return Task.Factory.StartNew(method, passInValue, cancelToken.Token);
        //    else return new Task(method, passInValue, cancelToken.Token);
        //}

        ///// <summary>
        ///// 透過 <see cref="Task"/> 喚醒休眠中的執行緒(<see cref="Thread"/>)執行帶有回傳值之副程式
        ///// <para>「不建議」帶入長效型副程式，如監控 I/O 等；「建議」用於計算、UI 等使用完即結束之方法</para>
        ///// </summary>
        ///// <param name="method">委派副程式位址，指向不需帶參數但「具有」回傳值之副程式</param>
        ///// <param name="cancelToken">取消用物件。可透過此物件發出取消訊號，立即中斷並取消當前 Task 工作</param>
        ///// <param name="auto">是否自動執行?  (True)自動執行  (False)手動執行</param>
        ///// <returns>因應此方法所建立之 <see cref="Task&lt;TResult&gt;"/></returns>
        ///// <remarks>
        ///// 此方法直接使用 .Net Framework 4.0 之 <see cref="Task&lt;TResult&gt;"/>，如專案為低於 4.0 之版本請移除或註解此段程式
        ///// </remarks>
        ///// <example><code>
        ///// private int tsk() {
        /////     try {
        /////         while (true) cnTkSrc.Token.ThrowIfCancellationRequested();  //需於程式中加入此行，將會檢查 CancelToken 是否 IsCanceled == true
        /////     } catch (Exception ex) {
        /////         Console.WriteLine(ex.Message);  //IsCanceled == true 後會觸發 OperationCanceledException
        /////     } finally {
        /////         return -1;
        /////     }
        ///// }
        ///// 
        ///// CancellationTokenSource cnTkSrc = new CancellationTokenSource();
        ///// Task mTask = CtThread.AddTask(tsk, cnTkSrc);    //由 Task 執行 tsk 副程式
        ///// cnTkSrc.Cancel();                               //取消當前 Task 工作，會將 IsCanceled = true
        ///// </code></example>
        //public static Task<TResult> AddTask<TResult>(Func<TResult> method, CancellationTokenSource cancelToken, bool auto = true) {
        //    if (auto) return Task<TResult>.Factory.StartNew(method, cancelToken.Token);
        //    else return new Task<TResult>(method, cancelToken.Token);
        //}

        ///// <summary>
        ///// 透過 <see cref="Task"/> 喚醒休眠中的執行緒(<see cref="Thread"/>)執行帶有回傳值之副程式
        ///// <para>「不建議」帶入長效型副程式，如監控 I/O 等；「建議」用於計算、UI 等使用完即結束之方法</para>
        ///// </summary>
        ///// <param name="method">委派副程式位址，指向「具有」引數與回傳值之副程式</param>
        ///// <param name="passInValue">欲傳入副程式之物件</param>
        ///// <param name="cancelToken">取消用物件。可透過此物件發出取消訊號，立即中斷並取消當前 Task 工作</param>
        ///// <param name="auto">是否自動執行?  (True)自動執行  (False)手動執行</param>
        ///// <returns>因應此方法所建立之 <see cref="Task&lt;TResult&gt;"/></returns>
        ///// <remarks>
        ///// 此方法直接使用 .Net Framework 4.0 之 <see cref="Task&lt;TResult&gt;"/>，如專案為低於 4.0 之版本請移除或註解此段程式
        ///// </remarks>
        ///// <example><code>
        ///// private int tsk(object obj) {
        /////     try {
        /////         while (true) cnTkSrc.Token.ThrowIfCancellationRequested();  //需於程式中加入此行，將會檢查 CancelToken 是否 IsCanceled == true
        /////     } catch (Exception ex) {
        /////         Console.WriteLine(ex.Message);  //IsCanceled == true 後會觸發 OperationCanceledException
        /////     } finally {
        /////         return ((int)obj * 10);
        /////     }
        ///// }
        ///// 
        ///// CancellationTokenSource cnTkSrc = new CancellationTokenSource();
        ///// Task mTask = CtThread.AddTask(tsk, 9, cnTkSrc); //由 Task 執行 tsk 副程式，並傳入整數 9
        ///// cnTkSrc.Cancel();                               //取消當前 Task 工作，會將 IsCanceled = true
        ///// </code></example>
        //public static Task<TResult> AddTask<TResult>(Func<object, TResult> method, object passInValue, CancellationTokenSource cancelToken, bool auto = true) {
        //    if (auto) return Task<TResult>.Factory.StartNew(method, passInValue, cancelToken.Token);
        //    else return new Task<TResult>(method, passInValue, cancelToken.Token);
        //}
        #endregion
    }
}
