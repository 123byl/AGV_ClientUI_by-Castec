using CtBind;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.CompilerServices;

namespace CtCommandPattern.cs
{

    /// <summary>
    /// 可撤銷接口
    /// </summary>
    public interface IUndoable:IDataSource{
        /// <summary>
        /// 可撤銷限制次數
        /// </summary>
        int UndoLimit { get; set; }

        /// <summary>
        /// 可撤銷次數
        /// </summary>
        int UndoCount { get; }

        /// <summary>
        /// 可重做次數
        /// </summary>
        int RedoCount { get; }

        /// <summary>
        /// 撤銷
        /// </summary>
        void Undo();

        /// <summary>
        /// 重做
        /// </summary>
        void Redo();

        /// <summary>
        /// 清除命令記錄
        /// </summary>
        void Clear();
    }

    /// <summary>
    /// 命令定義接口
    /// </summary>
    public interface ICommand {
        /// <summary>
        /// 執行/重做
        /// </summary>
        /// <returns>是否執行成功</returns>
        bool Execute();
        /// <summary>
        /// 撤銷
        /// </summary>
        void Undo();
    }

    /// <summary>
    /// 命令基類
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseCommand<T> : ICommand  where T:IUndoable{
        protected T mReceiver = default(T);
        public BaseCommand(T receiver) {
            mReceiver = receiver;
        }

        /// <summary>
        /// 執行/重做
        /// </summary>
        public abstract bool Execute();

        /// <summary>
        /// 復原
        /// </summary>
        public abstract void Undo();
    }



    /// <summary>
    /// 命令管理器 - 私有變數隔離
    /// </summary>
    public abstract class BaseCommandManager : IUndoable {

        #region Declaration - Declaration - Fields

        /// <summary>
        /// 撤銷堆疊
        /// </summary>
        private List<ICommand> mUndoList = new List<ICommand>();
        /// <summary>
        /// 重做堆疊
        /// </summary>
        private List<ICommand> mRedoList = new List<ICommand>();

        #endregion Declaration - Fields

        #region Declaration - PropertyChanged

        protected IReadOnlyList<ICommand> UndoList => mUndoList as IReadOnlyList<ICommand>;

        protected IReadOnlyList<ICommand> RedoList => mRedoList as IReadOnlyList<ICommand>;

        #endregion Declaration - PropertyChagned

        #region Implement - IUndoable

        /// <summary>
        /// 可撤銷次數限制,-1表無限
        /// </summary>
        public int UndoLimit { get; set; } = -1;

        /// <summary>
        /// 可撤銷次數
        /// </summary>
        public int UndoCount => mUndoList.Count;

        /// <summary>
        /// 可重做次數
        /// </summary>
        public int RedoCount => mRedoList.Count;

        /// <summary>
        /// 執行命令
        /// </summary>
        /// <param name="cmd"></param>
        public abstract void ExecutCmd(ICommand cmd);

        /// <summary>
        /// 撤銷命令
        /// </summary>
        public abstract void Undo();

        /// <summary>
        /// 重做命令
        /// </summary>
        public abstract void Redo();

        /// <summary>
        /// 清除命令記錄
        /// </summary>
        public abstract void Clear();

        #endregion Implement - IUndoable

        #region Implement - IDataSource

        /// <summary>
        /// 屬性變更事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Invoke方法委派
        /// </summary>
        public Action<MethodInvoker> DelInvoke { get; set; } = null;

        /// <summary>
        /// 屬性變更事件發報
        /// </summary>
        /// <param name="propertyName"></param>
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = "") {
            MethodInvoker method = () => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            if (DelInvoke != null) {
                DelInvoke(method);
            } else {
                method();
            }
        }

        #endregion Implement - IDataSource

        #region Function - Private Methods
        
        /// <summary>
        /// 撤銷堆疊操作
        /// </summary>
        /// <param name="act"></param>
        protected void UndoOperate(Action<List<ICommand>> act) {
            act(mUndoList);
            OnPropertyChanged(nameof(UndoCount));
        }

        /// <summary>
        /// 重做堆疊操作
        /// </summary>
        /// <param name="act"></param>
        protected void RedoOperate(Action<List<ICommand>> act) {
            act(mRedoList);
            OnPropertyChanged(nameof(RedoCount));
        }

        #endregion Function = Private Methods

    }

    /// <summary>
    /// 命令管理器
    /// </summary>
    public class CommandManager : BaseCommandManager {
        
        /// <summary>
        /// 執行命令
        /// </summary>
        /// <param name="cmd"></param>
        public override void ExecutCmd(ICommand cmd) {
            bool success = cmd.Execute();
            if (success) {
                UndoOperate(undo => undo.Add(cmd));
                if (UndoLimit != -1 && UndoCount > UndoLimit) {
                    UndoOperate(undo => undo.RemoveAt(0));
                }
                RedoOperate(redo => redo.Clear());
            }
        }

        /// <summary>
        /// 撤銷命令
        /// </summary>
        public override void Undo() {
            if (UndoList.Any()) {
                var cmd = UndoList.Last();
                cmd.Undo();
                UndoOperate(undo => undo.Remove(cmd));
                RedoOperate(redo => redo.Add(cmd));
            }
        }

        /// <summary>
        /// 重做命令
        /// </summary>
        public override void Redo() {
            if (RedoList.Any()) {
                var cmd = RedoList.Last();
                cmd.Execute();
                RedoOperate(redo => redo.Remove(cmd));
                UndoOperate(undo => undo.Add(cmd));
            }
        }

        /// <summary>
        /// 清除命令記錄
        /// </summary>
        public override void Clear() {
            UndoOperate(undo => undo.Clear());
            RedoOperate(redo => redo.Clear());
        }

    }

}
