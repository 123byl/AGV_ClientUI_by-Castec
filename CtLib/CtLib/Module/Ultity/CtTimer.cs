using System;
using System.Threading;
using System.Threading.Tasks;

using CtLib.Library;

namespace CtLib.Module.Ultity
{

    /// <summary>實作計時器、延遲或等待方法與類別</summary>
    public class CtTimer : ICtVersion
    {

        #region Version

        /// <summary>CtTimer 版本訊息</summary>
        /// <remarks><code>
        /// 1.0.0  Ahern [2015/08/11]
        ///     + 建立基礎模組，含 Timer、Delay、Timeout
        ///     
        /// </code></remarks>
        public CtVersion Version { get { return new CtVersion(1, 0, 0, "2015/08/11", "Ahern Kuo"); } }

        #endregion

        #region Threading Timers

        /// <summary>
        /// 輕量化的計時器，採用 <see cref="System.Threading.Timer"/> 實作
        /// <para>此為 non thread-safe，實作時請小心處理</para>
        /// </summary>
        /// <remarks>
        /// 簡易的三種 Timer 比較
        /// <code>
        /// 1. <see cref="System.Windows.Forms.Timer"/> 由 HWND (Handle of Window) 處理，即由介面的背景處理程序來計時。操作介面元件時不需使用 delegate 或 SynchronizingObject，可直接應用
        /// 
        /// 2. <see cref="System.Timers.Timer"/> 由 Thread Pool 拉出一條執行緒計時，故須使用 delegate 或 SynchronizingObject 來處理 UI
        ///    此為 <seealso cref="System.ComponentModel.Component"/>，包含了計時器以及其他額外功能。另此為 thread-safe。採用 <seealso cref="System.Timers.ElapsedEventHandler"/> 事件來回報觸發
        /// 
        /// 3. <seealso cref="System.Threading.Timer"/> 與 <seealso cref="System.Timers.Timer"/> 類似，均由 Thread Pool 拉出一條執行緒執行
        ///    但此僅含計時器部分，為輕量化的計時器。此為非 thread-safe (需自行掌握 Thread 狀態。如果時間過短而 callback 來不及處理的話，callback 可能同時觸發)
        ///    採用回授(Callback) <seealso cref="TimerCallback"/> 來處理觸發
        /// </code>
        /// </remarks>
        /// <example>
        /// 1. 由建構時直接啟動
        /// <code>
        /// CtTimer.Timer timer = new CtTimer.Timer(3000, tsk_TmrDoWork, true, true);   //每 3 秒一個 Tick，並直接開始計時
        /// 
        /// private void tsk_TmrDoWork(object param) {
        ///     CtTimer.Timer timer = param as CtTimer.Timer;
        ///     do {
        ///         //do something
        ///     } while (timer.IsEnabled);  //當 Timer 的 Enabled 關閉時，表示 Timer 壽命已盡
        /// }
        /// </code>
        /// 2. 手動啟動
        /// <code>
        /// CtTimer.Timer timer = new CtTimer.Timer(tsk_TmrDoWork);   //每 3 秒一個 Tick，並直接開始計時
        /// timer.Start(3000);  //使用 Start() 開始計時
        /// 
        /// private void tsk_TmrDoWork(object param) {
        ///     CtTimer.Timer timer = param as CtTimer.Timer;
        ///     do {
        ///         //do something
        ///     } while (timer.IsEnabled);  //當 Timer 的 Enabled 關閉時，表示 Timer 壽命已盡
        /// }
        /// </code>
        /// 3. 更改週期
        /// <code>
        /// CtTimer.Timer timer = new CtTimer.Timer(3000, tsk_TmrDoWork, true, true);   //每 3 秒一個 Tick，並直接開始計時
        /// 
        /// //等待某段時間後...
        /// timer.Start(1000);  //更改週期為 1s，不需 Stop() 即可
        /// 
        /// private void tsk_TmrDoWork(object param) {
        ///     CtTimer.Timer timer = param as CtTimer.Timer;
        ///     do {
        ///         //do something
        ///     } while (timer.IsEnabled);  //當 Timer 的 Enabled 關閉時，表示 Timer 壽命已盡
        /// }
        /// </code>
        /// </example>
        public class Timer : IDisposable
        {

            #region Declaration - Members

            /// <summary>計時器</summary>
            private System.Threading.Timer mTimer;
            /// <summary>[DueTime] 啟動後至第一次執行之時間。 0 表示立即觸發</summary>
            private TimeSpan mDueTime = TimeSpan.Zero;
            /// <summary>[Period] 第一次觸發後，接下來要持續觸發的間隔時間。 -1 或 Infinite 表示只跑第一次觸發，不重複執行</summary>
            private TimeSpan mPeriod = TimeSpan.Zero;

            #endregion

            #region Declaration - Properties

            /// <summary>取得當前計時器是否啟用</summary>
            /// <remarks>Enabled = true，計時器啟動... 僅能知道這個物件是 "活" 的，不一定正在 "工作" (觸發)</remarks>
            public bool IsEnabled { get; private set; }

            #endregion

            #region Function - Constructor

            /// <summary>
            /// 建立計時器但尚不決定時間與啟動，由使用者自行透過 <see cref="Start()"/> 觸發
            /// <para>於觸發的自訂方法，將以此 <see cref="CtTimer"/> 作為傳入值</para>
            /// </summary>
            /// <param name="callback">
            /// 時間觸發欲執行的方法
            /// <para>簽章必須符合 <see cref="Action{T}"/>，即不具回傳值，且含有單一 object 引數的方法</para>
            /// </param>
            public Timer(Action<object> callback)
            {
                mTimer = new System.Threading.Timer(new TimerCallback(callback), this, Timeout.Infinite, Timeout.Infinite);
            }

            /// <summary>
            /// 建立計時器但尚不決定時間與啟動，由使用者自行透過 <see cref="Start()"/> 觸發
            /// <para>於觸發的自訂方法，將以此建構方法引數 parameter 作為傳入值</para>
            /// </summary>
            /// <param name="callback">
            /// 時間觸發欲執行的方法
            /// <para>簽章必須符合 <see cref="Action{T}"/>，即不具回傳值，且含有單一 object 引數的方法</para>
            /// </param>
            /// <param name="parameter">欲於觸發時傳入自訂方法的物件</param>
            public Timer(Action<object> callback, object parameter)
            {
                mTimer = new System.Threading.Timer(new TimerCallback(callback), parameter, Timeout.Infinite, Timeout.Infinite);
            }

            /// <summary>
            /// 建立帶有預設計時時間的計時器
            /// <para>於觸發的自訂方法，將以此 <see cref="CtTimer"/> 作為傳入值</para>
            /// </summary>
            /// <param name="interval">計時間隔，單位為毫秒(Millisecond, ms)</param>
            /// <param name="callback">
            /// 時間觸發欲執行的方法
            /// <para>簽章必須符合 <see cref="Action{T}"/>，即不具回傳值，且含有單一 object 引數的方法</para>
            /// </param>
            /// <param name="repeat">是否持續觸發？  (True)固定間隔持續觸發  (False)僅觸發一次</param>
            /// <param name="autoStart">是否於此建構方法後啟動計時器？  (True)建構後立即啟動  (False)自行使用 <see cref="Start()"/> 啟動</param>
            public Timer(long interval, Action<object> callback, bool repeat = false, bool autoStart = true)
            {
                /*-- Create Object --*/
                mTimer = new System.Threading.Timer(new TimerCallback(callback), this, Timeout.Infinite, Timeout.Infinite);

                /*-- Time Setting --*/
                mDueTime = TimeSpan.FromMilliseconds(interval);
                mPeriod = repeat ? TimeSpan.FromMilliseconds(interval) : Timeout.InfiniteTimeSpan;
                if (autoStart) Start();
            }

            /// <summary>
            /// 建立帶有預設計時時間的計時器，但使用不同的計時間隔(第一次觸發 與 後續持續觸發)
            /// <para>於觸發的自訂方法，將以此 <see cref="CtTimer"/> 作為傳入值</para>
            /// </summary>
            /// <param name="dueTime">啟動後到第一次觸發的時間。單位為毫秒(Millisecond, ms)</param>
            /// <param name="period">第一次觸發後，後續持續觸發的時間間隔。單位為毫秒(Millisecond, ms)</param>
            /// <param name="callback">
            /// 時間觸發欲執行的方法
            /// <para>簽章必須符合 <see cref="Action{T}"/>，即不具回傳值，且含有單一 object 引數的方法</para>
            /// </param>
            /// <param name="autoStart">是否於此建構方法後啟動計時器？  (True)建構後立即啟動  (False)自行使用 <see cref="Start()"/> 啟動</param>
            public Timer(long dueTime, long period, Action<object> callback, bool autoStart = true)
            {
                /*-- Create Object --*/
                mTimer = new System.Threading.Timer(new TimerCallback(callback), this, Timeout.Infinite, Timeout.Infinite);

                /*-- Time Setting --*/
                mDueTime = TimeSpan.FromMilliseconds(dueTime);
                mPeriod = TimeSpan.FromMilliseconds(period);
                if (autoStart) Start();
            }

            /// <summary>
            /// 建立帶有預設計時時間的計時器
            /// <para>於觸發的自訂方法，將以此建構方法引數 parameter 作為傳入值</para>
            /// </summary>
            /// <param name="interval">計時間隔，單位為毫秒(Millisecond, ms)</param>
            /// <param name="callback">
            /// 時間觸發欲執行的方法
            /// <para>簽章必須符合 <see cref="Action{T}"/>，即不具回傳值，且含有單一 object 引數的方法</para>
            /// </param>
            /// <param name="parameter">欲於觸發時傳入自訂方法的物件</param>
            /// <param name="repeat">是否持續觸發？  (True)固定間隔持續觸發  (False)僅觸發一次</param>
            /// <param name="autoStart">是否於此建構方法後啟動計時器？  (True)建構後立即啟動  (False)自行使用 <see cref="Start()"/> 啟動</param>
            public Timer(long interval, Action<object> callback, object parameter, bool repeat = false, bool autoStart = true)
            {
                /*-- Create Object --*/
                mTimer = new System.Threading.Timer(new TimerCallback(callback), parameter, Timeout.Infinite, Timeout.Infinite);

                /*-- Time Setting --*/
                mDueTime = TimeSpan.FromMilliseconds(interval);
                mPeriod = repeat ? TimeSpan.FromMilliseconds(interval) : Timeout.InfiniteTimeSpan;
                if (autoStart) Start();
            }

            /// <summary>
            /// 建立帶有預設計時時間的計時器，但使用不同的計時間隔(第一次觸發 與 後續持續觸發)
            /// <para>於觸發的自訂方法，將以此建構方法引數 parameter 作為傳入值</para>
            /// </summary>
            /// <param name="dueTime">啟動後到第一次觸發的時間。單位為毫秒(Millisecond, ms)</param>
            /// <param name="period">第一次觸發後，後續持續觸發的時間間隔。單位為毫秒(Millisecond, ms)</param>
            /// <param name="callback">
            /// 時間觸發欲執行的方法
            /// <para>簽章必須符合 <see cref="Action{T}"/>，即不具回傳值，且含有單一 object 引數的方法</para>
            /// </param>
            /// <param name="parameter">欲於觸發時傳入自訂方法的物件</param>
            /// <param name="autoStart">是否於此建構方法後啟動計時器？  (True)建構後立即啟動  (False)自行使用 <see cref="Start()"/> 啟動</param>
            public Timer(long dueTime, long period, Action<object> callback, object parameter, bool autoStart = true)
            {
                /*-- Create Object --*/
                mTimer = new System.Threading.Timer(new TimerCallback(callback), parameter, Timeout.Infinite, Timeout.Infinite);

                /*-- Time Setting --*/
                mDueTime = TimeSpan.FromMilliseconds(dueTime);
                mPeriod = TimeSpan.FromMilliseconds(period);
                if (autoStart) Start();
            }

            /// <summary>
            /// 建立帶有預設計時時間的計時器
            /// <para>於觸發的自訂方法，將以此 <see cref="CtTimer"/> 作為傳入值</para>
            /// </summary>
            /// <param name="interval">計時間隔，單位為毫秒(Millisecond, ms)</param>
            /// <param name="callback">
            /// 時間觸發欲執行的方法
            /// <para>簽章必須符合 <see cref="Action{T}"/>，即不具回傳值，且含有單一 object 引數的方法</para>
            /// </param>
            /// <param name="repeat">是否持續觸發？  (True)固定間隔持續觸發  (False)僅觸發一次</param>
            /// <param name="autoStart">是否於此建構方法後啟動計時器？  (True)建構後立即啟動  (False)自行使用 <see cref="Start()"/> 啟動</param>
            public Timer(TimeSpan interval, Action<object> callback, bool repeat = false, bool autoStart = true)
            {
                /*-- Create Object --*/
                mTimer = new System.Threading.Timer(new TimerCallback(callback), this, Timeout.Infinite, Timeout.Infinite);

                /*-- Time Setting --*/
                mDueTime = interval;
                mPeriod = repeat ? interval : Timeout.InfiniteTimeSpan;
                if (autoStart) Start();
            }

            /// <summary>
            /// 建立帶有預設計時時間的計時器，但使用不同的計時間隔(第一次觸發 與 後續持續觸發)
            /// <para>於觸發的自訂方法，將以此 <see cref="CtTimer"/> 作為傳入值</para>
            /// </summary>
            /// <param name="dueTime">啟動後到第一次觸發的時間</param>
            /// <param name="period">第一次觸發後，後續持續觸發的時間間隔</param>
            /// <param name="callback">
            /// 時間觸發欲執行的方法
            /// <para>簽章必須符合 <see cref="Action{T}"/>，即不具回傳值，且含有單一 object 引數的方法</para>
            /// </param>
            /// <param name="autoStart">是否於此建構方法後啟動計時器？  (True)建構後立即啟動  (False)自行使用 <see cref="Start()"/> 啟動</param>
            public Timer(TimeSpan dueTime, TimeSpan period, Action<object> callback, bool autoStart = true)
            {
                /*-- Create Object --*/
                mTimer = new System.Threading.Timer(new TimerCallback(callback), this, Timeout.Infinite, Timeout.Infinite);

                /*-- Time Setting --*/
                mDueTime = dueTime;
                mPeriod = period;
                if (autoStart) Start();
            }

            /// <summary>
            /// 建立帶有預設計時時間的計時器
            /// <para>於觸發的自訂方法，將以此建構方法引數 parameter 作為傳入值</para>
            /// </summary>
            /// <param name="interval">計時間隔，單位為毫秒(Millisecond, ms)</param>
            /// <param name="callback">
            /// 時間觸發欲執行的方法
            /// <para>簽章必須符合 <see cref="Action{T}"/>，即不具回傳值，且含有單一 object 引數的方法</para>
            /// </param>
            /// <param name="parameter">欲於觸發時傳入自訂方法的物件</param>
            /// <param name="repeat">是否持續觸發？  (True)固定間隔持續觸發  (False)僅觸發一次</param>
            /// <param name="autoStart">是否於此建構方法後啟動計時器？  (True)建構後立即啟動  (False)自行使用 <see cref="Start()"/> 啟動</param>
            public Timer(TimeSpan interval, Action<object> callback, object parameter, bool repeat = false, bool autoStart = true)
            {
                /*-- Create Object --*/
                mTimer = new System.Threading.Timer(new TimerCallback(callback), parameter, Timeout.Infinite, Timeout.Infinite);

                /*-- Time Setting --*/
                mDueTime = interval;
                mPeriod = repeat ? interval : Timeout.InfiniteTimeSpan;
                if (autoStart) Start();
            }

            /// <summary>
            /// 建立帶有預設計時時間的計時器，但使用不同的計時間隔(第一次觸發 與 後續持續觸發)
            /// <para>於觸發的自訂方法，將以此建構方法引數 parameter 作為傳入值</para>
            /// </summary>
            /// <param name="dueTime">啟動後到第一次觸發的時間</param>
            /// <param name="period">第一次觸發後，後續持續觸發的時間間隔</param>
            /// <param name="callback">
            /// 時間觸發欲執行的方法
            /// <para>簽章必須符合 <see cref="Action{T}"/>，即不具回傳值，且含有單一 object 引數的方法</para>
            /// </param>
            /// <param name="parameter">欲於觸發時傳入自訂方法的物件</param>
            /// <param name="autoStart">是否於此建構方法後啟動計時器？  (True)建構後立即啟動  (False)自行使用 <see cref="Start()"/> 啟動</param>
            public Timer(TimeSpan dueTime, TimeSpan period, Action<object> callback, object parameter, bool autoStart = true)
            {
                /*-- Create Object --*/
                mTimer = new System.Threading.Timer(new TimerCallback(callback), parameter, Timeout.Infinite, Timeout.Infinite);

                /*-- Time Setting --*/
                mDueTime = dueTime;
                mPeriod = period;
                if (autoStart) Start();
            }


            #endregion

            #region Function - Disposable
            /// <summary>停止計時器並釋放相關資源。此動作會等待已觸發的自訂方法執行完畢才會離開</summary>
            public void Dispose()
            {
                try
                {
                    Dispose(true);
                    GC.SuppressFinalize(this);
                }
                catch (ObjectDisposedException ex)
                {
                    CtStatus.Report(Stat.ER_SYSTEM, ex);
                }
            }

            /// <summary>停止計時器並釋放相關資源。此動作會等待已觸發的自訂方法執行完畢才會離開</summary>
            /// <param name="isDisposing">是否為第一次釋放</param>
            protected virtual void Dispose(bool isDisposing)
            {
                try
                {
                    if (isDisposing && mTimer != null)
                    {
                        IsEnabled = false;
                        WaitHandle waitHdl = new AutoResetEvent(false);
                        mTimer.Dispose(waitHdl);
                        waitHdl.WaitOne();
                    }
                }
                catch (Exception ex)
                {
                    CtStatus.Report(Stat.ER_SYSTEM, ex);
                }
            }
            #endregion

            #region Function - Core

            #region Start
            /// <summary>啟動計時器。以已設定的計時間隔啟動</summary>
            /// <returns>是否啟動成功？ (True)啟動成功  (False)無法變更時間間隔</returns>
            public bool Start()
            {
                bool result = false;
                if (mTimer != null)
                {
                    result = mTimer.Change(mDueTime, mPeriod);
                    if (result) IsEnabled = true;
                }
                return result;
            }

            /// <summary>啟動計時器。以 interval 引數決定計時間隔</summary>
            /// <param name="interval">計時間隔，單位為毫秒(Millisecond, ms)</param>
            /// <param name="repeat">是否持續觸發？  (True)固定間隔持續觸發  (False)僅觸發一次</param>
            /// <returns>是否啟動成功？ (True)啟動成功  (False)無法變更時間間隔</returns>
            public bool Start(long interval, bool repeat = false)
            {
                mDueTime = TimeSpan.FromMilliseconds(interval);
                mPeriod = repeat ? TimeSpan.FromMilliseconds(interval) : Timeout.InfiniteTimeSpan;
                return Start();
            }

            /// <summary>啟動計時器。可調整第一次觸發時間與後續觸發時間間隔</summary>
            /// <param name="dueTime">啟動後到第一次觸發的時間</param>
            /// <param name="period">第一次觸發後，後續持續觸發的時間間隔</param>
            /// <returns>是否啟動成功？ (True)啟動成功  (False)無法變更時間間隔</returns>
            public bool Start(long dueTime, long period)
            {
                mDueTime = TimeSpan.FromMilliseconds(dueTime);
                mPeriod = TimeSpan.FromMilliseconds(period);
                return Start();
            }

            /// <summary>啟動計時器。以 interval 引數決定計時間隔</summary>
            /// <param name="interval">計時間隔，以時間間隔決定計時間隔</param>
            /// <param name="repeat">是否持續觸發？  (True)固定間隔持續觸發  (False)僅觸發一次</param>
            /// <returns>是否啟動成功？ (True)啟動成功  (False)無法變更時間間隔</returns>
            public bool Start(TimeSpan interval, bool repeat = false)
            {
                mDueTime = interval;
                mPeriod = repeat ? interval : Timeout.InfiniteTimeSpan;
                return Start();
            }

            /// <summary>啟動計時器。可調整第一次觸發時間與後續觸發時間間隔</summary>
            /// <param name="dueTime">啟動後到第一次觸發的時間</param>
            /// <param name="period">第一次觸發後，後續持續觸發的時間間隔</param>
            /// <returns>是否啟動成功？ (True)啟動成功  (False)無法變更時間間隔</returns>
            public bool Start(TimeSpan dueTime, TimeSpan period)
            {
                mDueTime = dueTime;
                mPeriod = period;
                return Start();
            }
            #endregion

            #region Stop
            /// <summary>停止計時器，但不會停止已觸發的自訂方法</summary>
            /// <returns>計時器是否成功停止？  (True)已停止  (False)無法將計時間隔設為無限</returns>
            public bool Stop()
            {
                bool result = false;
                if (mTimer != null)
                {
                    result = mTimer.Change(Timeout.Infinite, Timeout.Infinite);
                    if (result) IsEnabled = false;
                }

                return result;
            }
            #endregion

            #region Others
            /// <summary>立即觸發計時器</summary>
            /// <param name="repeat">觸發後是否持續觸發？  (True)固定間隔持續觸發  (False)僅觸發一次</param>
            /// <returns>是否觸發成功？ (True)觸發成功  (False)無法變更時間間隔</returns>
            /// <remarks>如果 repeat = true，但原本已設為不重複，則一樣不會重複！請再用 Start 自行重設吧！</remarks>
            public bool TriggerDirectly(bool repeat = false)
            {
                bool result = false;
                if (mTimer != null) result = mTimer.Change(0, repeat ? mPeriod.Milliseconds : Timeout.Infinite);
                return result;
            }
            #endregion

            #endregion
        }

        #endregion

        #region Delay

        /// <summary>進行中斷執行緒資源的延遲</summary>
        /// <param name="delayTime">欲延遲執行緒的時間，單位為毫秒(Millisecond, ms)</param>
        /// <example><code>
        /// private void example() {
        ///     bool value = false;
        ///     do {
        ///         mBeckhoff.GetValue("Main.pF_AutoRun", out value);
        ///         if (value) break;
        ///
        ///         CtTimer.Delay(100); //Delay 100ms
        ///     } while (true);
        /// }
        /// </code></example>
        public static void Delay(int delayTime)
        {
            /*-- Un-block this region if .Net 3.5 --*/
            //using (WaitHandle waitHdl = new ManualResetEvent(false)) {
            //    waitHdl.WaitOne(time); 
            //}

            /*-- Un-block this region if .Net 4+ --*/
            Task.Delay(delayTime).Wait();
        }

        /// <summary>進行中斷執行緒資源的延遲</summary>
        /// <param name="delayTime">欲延遲執行緒的時間</param>
        /// <example><code>
        /// private void example() {
        ///     bool value = false;
        ///     do {
        ///         mBeckhoff.GetValue("Main.pF_AutoRun", out value);
        ///         if (value) break;
        ///
        ///         CtTimer.Delay(TimeSpan.FromSeconds(5)); //Delay 5 second
        ///     } while (true);
        /// }
        /// </code></example>
        public static void Delay(TimeSpan delayTime)
        {
            /*-- Un-block this region if .Net 3.5 --*/
            //using (WaitHandle waitHdl = new ManualResetEvent(false)) {
            //    waitHdl.WaitOne(time);
            //}

            /*-- Un-block this region if .Net 4+ --*/
            Task.Delay(delayTime).Wait();
        }

        /// <summary>
        /// 進行中斷執行緒資源的延遲
        /// <para>跳脫條件為設定時間到達或 <see cref="CancellationTokenSource.Cancel()"/> 觸發</para>
        /// </summary>
        /// <param name="delayTime">欲延遲執行緒的時間，單位為毫秒(Millisecond, ms)</param>
        /// <param name="cancelToken">監控取消物件</param>
        /// <example><code>
        /// CancellationTokenSource cancelToken = new CancellationTokenSource();
        /// 
        /// private void example() {
        ///     CtThread.AddTask(tsk_AnotherTask);
        ///     CtTimer.Delay(100 * 1000, cancelToken); //等待 Beckhoff 抓到正確的數值，但是最多只等 100 秒。先到先離開
        /// }
        /// 
        /// private void tsk_AnotherTask() {
        ///     do {
        ///         mBeckhoff.GetValue("Main.pF_AutoRun", out value);
        ///         if (value) cancelToken.Cancel();
        ///     } while (!value);
        /// }
        /// </code></example>
        public static void Delay(int delayTime, CancellationTokenSource cancelToken)
        {
            try
            {
                /*-- Method 1 - Blocking current thread directly by WaitHandle --*/
                //using (ManualResetEventSlim waitHdl = new ManualResetEventSlim(false)) {
                //    waitHdl.Wait(time, cancelToken.Token);
                //}

                /*-- Method 2 - Waiting for request a thread pool thread to count delay --*/
                Task.Delay(delayTime).Wait(cancelToken.Token);
            }
            catch (OperationCanceledException)
            {
                /*-- 由 CancellationToken 取消作業，此屬正常情形 --*/
            }
        }

        /// <summary>
        /// 進行中斷執行緒資源的延遲
        /// <para>跳脫條件為設定時間到達或 <see cref="CancellationTokenSource.Cancel()"/> 觸發</para>
        /// </summary>
        /// <param name="delayTime">欲延遲執行緒的時間</param>
        /// <param name="cancelToken">監控取消物件</param>
        /// <example><code>
        /// CancellationTokenSource cancelToken = new CancellationTokenSource();
        /// 
        /// private void example() {
        ///     CtThread.AddTask(tsk_AnotherTask);
        ///     CtTimer.Delay(TimeSpan.FromSeconds(100), cancelToken); //等待 Beckhoff 抓到正確的數值，但是最多只等 100 秒。先到先離開
        /// }
        /// 
        /// private void tsk_AnotherTask() {
        ///     do {
        ///         mBeckhoff.GetValue("Main.pF_AutoRun", out value);
        ///         if (value) cancelToken.Cancel();
        ///     } while (!value);
        /// }
        /// </code></example>
        public static void Delay(TimeSpan delayTime, CancellationTokenSource cancelToken)
        {
            try
            {
                /*-- Method 1 - Blocking current thread directly by WaitHandle --*/
                //using (ManualResetEventSlim waitHdl = new ManualResetEventSlim(false)) {
                //    waitHdl.Wait(time, cancelToken.Token);
                //}

                /*-- Method 2 - Waiting for request a thread pool thread to count delay --*/
                Task.Delay(delayTime).Wait(cancelToken.Token);
            }
            catch (OperationCanceledException)
            {
                /*-- 由 CancellationToken 取消作業，此屬正常情形 --*/
            }
        }

        /// <summary>
        /// 進行中斷執行緒資源的延遲
        /// <para>跳脫條件為設定時間到達或 <see cref="CancellationTokenSource.Cancel()"/> 觸發</para>
        /// </summary>
        /// <param name="delayTime">欲延遲執行緒的時間，單位為毫秒(Millisecond, ms)</param>
        /// <param name="cancelToken">監控取消物件</param>
        /// <param name="autoReset">
        /// 是否自動重設 <see cref="CancellationTokenSource"/>
        /// <para>重設時採 CancellationTokenSource()，如欲使用其他方法請設為 false 並手動重設</para>
        /// </param>
        /// <example>
        /// 由於 CancellationTokenSource 並無 Reset 之功能，故如需重複使用請使用 ref 並將 autoReset 設為 true
        /// 但僅適用於 new CancellationTokenSource()，如是其他建構則不適合使用此方法
        /// <code>
        /// CancellationTokenSource cancelToken = new CancellationTokenSource();
        /// 
        /// private void example() {
        ///     CtThread.AddTask(tsk_AnotherTask);
        ///     do {
        ///         mBeckhoff.SetValue("Main.pF_AutoRun", false);
        ///         CtTimer.Delay(10 * 1000, ref cancelToken); //等待 Beckhoff 抓到正確的數值，但是最多只等 100 秒。先到先離開
        ///     } while (true);
        /// }
        /// 
        /// private void tsk_AnotherTask() {
        ///     do {
        ///         mBeckhoff.GetValue("Main.pF_AutoRun", out value);
        ///         if (value) cancelToken.Cancel();
        ///     } while (!value);
        /// }
        /// </code></example>
        public static void Delay(int delayTime, ref CancellationTokenSource cancelToken, bool autoReset = true)
        {
            try
            {
                /*-- Method 1 - Blocking current thread directly by WaitHandle --*/
                //using (ManualResetEventSlim waitHdl = new ManualResetEventSlim(false)) {
                //    waitHdl.Wait(time, cancelToken.Token);
                //}

                /*-- Method 2 - Waiting for request a thread pool thread to count delay --*/
                Task.Delay(delayTime).Wait(cancelToken.Token);
            }
            catch (OperationCanceledException)
            {
                /*-- 由 CancellationToken 取消作業，此屬正常情形 --*/
            }
            finally
            {
                if (autoReset) cancelToken = new CancellationTokenSource();
            }
        }

        /// <summary>
        /// 進行中斷執行緒資源的延遲
        /// <para>跳脫條件為設定時間到達或 <see cref="CancellationTokenSource.Cancel()"/> 觸發</para>
        /// </summary>
        /// <param name="delayTime">欲延遲執行緒的時間</param>
        /// <param name="cancelToken">監控取消物件</param>
        /// <param name="autoReset">
        /// 是否自動重設 <see cref="CancellationTokenSource"/>
        /// <para>重設時採 CancellationTokenSource()，如欲使用其他方法請設為 false 並手動重設</para>
        /// </param>
        /// <example>
        /// 由於 CancellationTokenSource 並無 Reset 之功能，故如需重複使用請使用 ref 並將 autoReset 設為 true
        /// 但僅適用於 new CancellationTokenSource()，如是其他建構則不適合使用此方法
        /// <code>
        /// CancellationTokenSource cancelToken = new CancellationTokenSource();
        /// 
        /// private void example() {
        ///     CtThread.AddTask(tsk_AnotherTask);
        ///     do {
        ///         mBeckhoff.SetValue("Main.pF_AutoRun", false);
        ///         CtTimer.Delay(TimeSpan.FromSeconds(100), ref cancelToken); //等待 Beckhoff 抓到正確的數值，但是最多只等 100 秒。先到先離開
        ///     } while (true);
        /// }
        /// 
        /// private void tsk_AnotherTask() {
        ///     do {
        ///         mBeckhoff.GetValue("Main.pF_AutoRun", out value);
        ///         if (value) cancelToken.Cancel();
        ///     } while (!value);
        /// }
        /// </code></example>
        public static void Delay(TimeSpan delayTime, ref CancellationTokenSource cancelToken, bool autoReset = true)
        {
            try
            {
                /*-- Method 1 - Blocking current thread directly by WaitHandle --*/
                //using (ManualResetEventSlim waitHdl = new ManualResetEventSlim(false)) {
                //    waitHdl.Wait(time, cancelToken.Token);
                //}

                /*-- Method 2 - Waiting for request a thread pool thread to count delay --*/
                Task.Delay(delayTime).Wait(cancelToken.Token);
            }
            catch (OperationCanceledException)
            {
                /*-- 由 CancellationToken 取消作業，此屬正常情形 --*/
            }
            finally
            {
                if (autoReset) cancelToken = new CancellationTokenSource();
            }
        }

        #endregion

        #region Timeout

        #region Support Class
        /// <summary>逾時旗標</summary>
        public class TimeoutToken : IDisposable
        {

            #region Declaration - Field
            /// <summary>等待計時用之 <see cref="WaitHandle"/></summary>
            private AutoResetEvent mWaitHdl = new AutoResetEvent(false);
            /// <summary>觀察是否取消之物件</summary>
            private CancellationTokenSource mCncSrc = new CancellationTokenSource();
            #endregion

            #region Declaration - Properties
            /// <summary>取得是否已完成工作</summary>
            public bool IsDone { get { return mCncSrc.IsCancellationRequested; } }
            #endregion

            #region Function - Disposable
            /// <summary>釋放相關資源</summary>
            public void Dispose()
            {
                try
                {
                    Dispose(true);
                    GC.SuppressFinalize(this);
                }
                catch (ObjectDisposedException ex)
                {
                    CtStatus.Report(Stat.ER_SYSTEM, ex);
                }
            }

            /// <summary>釋放相關資源</summary>
            /// <param name="isDisposing">是否為第一次釋放</param>
            protected virtual void Dispose(bool isDisposing)
            {
                try
                {
                    if (isDisposing)
                    {
                        if (mWaitHdl != null) mWaitHdl.Dispose();
                        if (mCncSrc != null) mCncSrc.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    CtStatus.Report(Stat.ER_SYSTEM, ex);
                }
            }
            #endregion

            #region Function - Core
            /// <summary>等待 <see cref="AutoResetEvent"/> 回傳訊號</summary>
            /// <param name="tmoTime">欲等待的逾時時間，時間單位為毫秒(Millisecond)</param>
            internal bool Wait(int tmoTime)
            {
                return mWaitHdl.WaitOne(tmoTime);
            }

            /// <summary>等待 <see cref="AutoResetEvent"/> 回傳訊號</summary>
            /// <param name="tmoTime">欲等待的逾時時間</param>
            internal bool Wait(TimeSpan tmoTime)
            {
                return mWaitHdl.WaitOne(tmoTime);
            }

            /// <summary>觸發工作完成訊號</summary>
            public void WorkDone()
            {
                mWaitHdl.Set();
                mCncSrc.Cancel();
            }
            #endregion
        }
        #endregion

        #region Core Functions
        /// <summary>執行特定程式，並觀察其是否逾時</summary>
        /// <param name="tmoTime">欲等待的逾時時間，時間單位為毫秒(Millisecond)</param>
        /// <param name="action">欲執行的特定程式，須符合 <see cref="Action{TimeoutToken}"/> 之簽章</param>
        /// <returns>是否逾時?  (True)逾時  (False)於時間內完成工作</returns>
        /// <example><code>
        /// private void example() {
        ///     if (CtTimer.WaitTimeout(3000, tsk_TmoTest)) //執行 tsk_TmoTest 副程式，如果超過 3 秒還沒完成表示逾時！
        ///         MessageBox.Show("Timeout!!");
        /// }
        /// 
        /// private void tsk_TmoTest(CtTimer.TimeoutToken tmoToken) {
        ///     do {
        ///         mBeckhoff.GetValue("Main.pF_AutoRun", out value);
        ///         if (value) tmoToken.WorkDone();
        ///     } while (!tmoToken.IsDone);
        /// }
        /// </code></example>
        public static bool WaitTimeout(int tmoTime, Action<TimeoutToken> action)
        {
            bool isTimeout = false;
            TimeoutToken tmoToken = new TimeoutToken();
            try
            {
                /*-- Using for .Net 3.5 --*/
                //CtThread.AddWorkItem(obj => action(tmoToken));
                //isTimeout = !tmoToken.Wait(tmoTime);

                /*-- Using for .Net 4+ --*/
                isTimeout = !Task.Run(() => action(tmoToken)).Wait(tmoTime);

            }
            finally
            {
                if (!tmoToken.IsDone) tmoToken.WorkDone();
            }
            return isTimeout;
        }

        /// <summary>執行可傳入參數之方法，並觀察其是否逾時</summary>
        /// <param name="tmoTime">欲等待的逾時時間，時間單位為毫秒(Millisecond)</param>
        /// <param name="action">欲執行的特定程式，須符合 <see cref="Action{TimeoutToken, Object}"/> 之簽章</param>
        /// <param name="param">欲傳入的參數組</param>
        /// <returns>是否逾時?  (True)逾時  (False)於時間內完成工作</returns>
        /// <example><code>
        /// private void example() {
        ///     bool b_IsTriggered = false;
        ///     int i_Count = 99;
        ///     if (CtTimer.WaitTimeout(3000, tsk_TmoTest, b_IsTriggered, i_Count)) //執行 tsk_TmoTest 副程式，如果超過 3 秒還沒完成表示逾時！同時帶入兩個參數至方法裡
        ///         MessageBox.Show("Timeout!!");
        /// }
        /// 
        /// private void tsk_TmoTest(CtTimer.TimeoutToken tmoToken, object[] obj) {
        ///     bool valBool = CtConvert.CBool(obj[0]); //從父程式傳進來的物件
        ///     int valInt = CtConvert.CInt(obj[1]);    //從父程式傳進來的物件
        /// 
        ///     do {
        ///         mBeckhoff.GetValue("Main.pF_AutoRun", out value);
        ///         if (value) tmoToken.WorkDone();
        ///     } while (!tmoToken.IsDone);
        /// }
        /// </code></example>
        public static bool WaitTimeout(int tmoTime, Action<TimeoutToken, object[]> action, params object[] param)
        {
            bool isTimeout = false;
            TimeoutToken tmoToken = new TimeoutToken();
            try
            {
                /*-- Using for .Net 3.5 --*/
                //CtThread.AddWorkItem(obj => action(tmoToken, param));
                //isTimeout = !tmoToken.Wait(tmoTime);

                /*-- Using for .Net 4+ --*/
                isTimeout = !Task.Run(() => action(tmoToken, param)).Wait(tmoTime);

            }
            finally
            {
                if (!tmoToken.IsDone) tmoToken.WorkDone();
            }
            return isTimeout;
        }

        /// <summary>執行特定程式，並觀察其是否逾時。當逾時發生，將以 <see cref="TimeoutException"/> 方式跳脫</summary>
        /// <param name="tmoTime">欲等待的逾時時間，時間單位為毫秒(Millisecond)</param>
        /// <param name="action">欲執行的特定程式，須符合 <see cref="Action{TimeoutToken}"/> 之簽章</param>
        /// <example><code>
        /// private void example() {
        ///     try {
        ///         CtTimer.WaitTimeout(3000, tsk_TmoTest); //執行 tsk_TmoTest 副程式，如果超過 3 秒還沒完成表示逾時！以 Exception 方式跳出
        ///     } catch (TimeoutException) {
        ///         MessageBox.Show("Timeout!!");
        ///     }
        /// }
        /// 
        /// private void tsk_TmoTest(CtTimer.TimeoutToken tmoToken) {
        ///     do {
        ///         mBeckhoff.GetValue("Main.pF_AutoRun", out value);
        ///         if (value) tmoToken.WorkDone();
        ///     } while (!tmoToken.IsDone);
        /// }
        /// </code></example>
        public static void WaitTimeoutEx(int tmoTime, Action<TimeoutToken> action)
        {

            TimeoutToken tmoToken = new TimeoutToken();

            /*-- Using for .Net 3.5 --*/
            //CtThread.AddWorkItem(obj => action(tmoToken));
            //tmoToken.Wait(tmoTime);

            /*-- Using for .Net 4+ --*/
            Task.Run(() => action(tmoToken)).Wait(tmoTime);

            if (!tmoToken.IsDone)
            {
                tmoToken.WorkDone();
                throw (new TimeoutException("Waiting program finished timeout."));
            }
        }

        /// <summary>執行可傳入參數之方法，並觀察其是否逾時。當逾時發生，將以 <see cref="TimeoutException"/> 方式跳脫</summary>
        /// <param name="tmoTime">欲等待的逾時時間，時間單位為毫秒(Millisecond)</param>
        /// <param name="action">欲執行的特定程式，須符合 <see cref="Action{TimeoutToken, Object}"/> 之簽章</param>
        /// <param name="param">欲傳入的參數組</param>
        /// <example><code>
        /// private void example() {
        ///     try {
        ///         bool b_IsTriggered = false;
        ///         int i_Count = 99;
        /// 
        ///         CtTimer.WaitTimeout(3000, tsk_TmoTest, b_IsTriggered, i_Count); //執行 tsk_TmoTest 副程式，如果超過 3 秒還沒完成表示逾時！以 Exception 方式跳出
        ///     } catch (TimeoutException) {
        ///         MessageBox.Show("Timeout!!");
        ///     }
        /// }
        /// 
        /// private void tsk_TmoTest(CtTimer.TimeoutToken tmoToken, object[] obj) {
        ///     bool valBool = CtConvert.CBool(obj[0]); //從父程式傳進來的物件
        ///     int valInt = CtConvert.CInt(obj[1]);    //從父程式傳進來的物件
        /// 
        ///     do {
        ///         mBeckhoff.GetValue("Main.pF_AutoRun", out value);
        ///         if (value) tmoToken.WorkDone();
        ///     } while (!tmoToken.IsDone);
        /// }
        /// </code></example>
        public static void WaitTimeoutEx(int tmoTime, Action<TimeoutToken, object[]> action, params object[] param)
        {

            TimeoutToken tmoToken = new TimeoutToken();

            /*-- Using for .Net 3.5 --*/
            //CtThread.AddWorkItem(obj => action(tmoToken, param));
            //tmoToken.Wait(tmoTime);

            /*-- Using for .Net 4+ --*/
            Task.Run(() => action(tmoToken, param)).Wait(tmoTime);

            if (!tmoToken.IsDone)
            {
                tmoToken.WorkDone();
                throw (new TimeoutException("Waiting program finished timeout."));
            }
        }

        /// <summary>執行特定程式，並觀察其是否逾時</summary>
        /// <param name="tmoTime">欲等待的逾時時間</param>
        /// <param name="action">欲執行的特定程式，須符合 <see cref="Action{TimeoutToken}"/> 之簽章</param>
        /// <returns>是否逾時?  (True)逾時  (False)於時間內完成工作</returns>
        /// <example><code>
        /// private void example() {
        ///     if (CtTimer.WaitTimeout(TimeSpan.FromSeconds(3), tsk_TmoTest)) //執行 tsk_TmoTest 副程式，如果超過 3 秒還沒完成表示逾時！
        ///         MessageBox.Show("Timeout!!");
        /// }
        /// 
        /// private void tsk_TmoTest(CtTimer.TimeoutToken tmoToken) {
        ///     do {
        ///         mBeckhoff.GetValue("Main.pF_AutoRun", out value);
        ///         if (value) tmoToken.WorkDone();
        ///     } while (!tmoToken.IsDone);
        /// }
        /// </code></example>
        public static bool WaitTimeout(TimeSpan tmoTime, Action<TimeoutToken> action)
        {
            return WaitTimeout(tmoTime.Milliseconds, action);
        }

        /// <summary>執行可傳入參數之方法，並觀察其是否逾時</summary>
        /// <param name="tmoTime">欲等待的逾時時間</param>
        /// <param name="action">欲執行的特定程式，須符合 <see cref="Action{TimeoutToken}"/> 之簽章</param>
        /// <param name="param">欲傳入的參數組</param>
        /// <returns>是否逾時?  (True)逾時  (False)於時間內完成工作</returns>
        /// <example><code>
        /// private void example() {
        ///     bool b_IsTriggered = false;
        ///     int i_Count = 99;
        ///     if (CtTimer.WaitTimeout(TimeSpan.FromSeconds(3), tsk_TmoTest, b_IsTriggered, i_Count)) //執行 tsk_TmoTest 副程式，如果超過 3 秒還沒完成表示逾時！同時帶入兩個參數至方法裡
        ///         MessageBox.Show("Timeout!!");
        /// }
        /// 
        /// private void tsk_TmoTest(CtTimer.TimeoutToken tmoToken, object[] obj) {
        ///     bool valBool = CtConvert.CBool(obj[0]); //從父程式傳進來的物件
        ///     int valInt = CtConvert.CInt(obj[1]);    //從父程式傳進來的物件
        /// 
        ///     do {
        ///         mBeckhoff.GetValue("Main.pF_AutoRun", out value);
        ///         if (value) tmoToken.WorkDone();
        ///     } while (!tmoToken.IsDone);
        /// }
        /// </code></example>
        public static bool WaitTimeout(TimeSpan tmoTime, Action<TimeoutToken, object[]> action, params object[] param)
        {
            return WaitTimeout(tmoTime.Milliseconds, action, param);
        }

        /// <summary>執行特定程式，並觀察其是否逾時。當逾時發生，將以 <see cref="TimeoutException"/> 方式跳脫</summary>
        /// <param name="tmoTime">欲等待的逾時時間</param>
        /// <param name="action">欲執行的特定程式，須符合 <see cref="Action{TimeoutToken}"/> 之簽章</param>
        /// <example><code>
        /// private void example() {
        ///     try {
        ///         CtTimer.WaitTimeout(TimeSpan.FromSeconds(3), tsk_TmoTest); //執行 tsk_TmoTest 副程式，如果超過 3 秒還沒完成表示逾時！以 Exception 方式跳出
        ///     } catch (TimeoutException) {
        ///         MessageBox.Show("Timeout!!");
        ///     }
        /// }
        /// 
        /// private void tsk_TmoTest(CtTimer.TimeoutToken tmoToken) {
        ///     do {
        ///         mBeckhoff.GetValue("Main.pF_AutoRun", out value);
        ///         if (value) tmoToken.WorkDone();
        ///     } while (!tmoToken.IsDone);
        /// }
        /// </code></example>
        public static void WaitTimeoutEx(TimeSpan tmoTime, Action<TimeoutToken> action)
        {
            WaitTimeoutEx(tmoTime.Milliseconds, action);
        }

        /// <summary>執行可傳入參數之方法，並觀察其是否逾時。當逾時發生，將以 <see cref="TimeoutException"/> 方式跳脫</summary>
        /// <param name="tmoTime">欲等待的逾時時間</param>
        /// <param name="action">欲執行的特定程式，須符合 <see cref="Action{TimeoutToken}"/> 之簽章</param>
        /// <param name="param">欲傳入的參數組</param>
        /// <example><code>
        /// private void example() {
        ///     try {
        ///         bool b_IsTriggered = false;
        ///         int i_Count = 99;
        /// 
        ///         CtTimer.WaitTimeout(TimeSpan.FromSeconds(3), tsk_TmoTest, b_IsTriggered, i_Count); //執行 tsk_TmoTest 副程式，如果超過 3 秒還沒完成表示逾時！以 Exception 方式跳出
        ///     } catch (TimeoutException) {
        ///         MessageBox.Show("Timeout!!");
        ///     }
        /// }
        /// 
        /// private void tsk_TmoTest(CtTimer.TimeoutToken tmoToken, object[] obj) {
        ///     bool valBool = CtConvert.CBool(obj[0]); //從父程式傳進來的物件
        ///     int valInt = CtConvert.CInt(obj[1]);    //從父程式傳進來的物件
        /// 
        ///     do {
        ///         mBeckhoff.GetValue("Main.pF_AutoRun", out value);
        ///         if (value) tmoToken.WorkDone();
        ///     } while (!tmoToken.IsDone);
        /// }
        /// </code></example>
        public static void WaitTimeoutEx(TimeSpan tmoTime, Action<TimeoutToken, object[]> action, params object[] param)
        {
            WaitTimeoutEx(tmoTime.Milliseconds, action, param);
        }

        #endregion

        #endregion

    }
}
