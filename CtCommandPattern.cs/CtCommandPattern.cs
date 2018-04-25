using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtCommandPattern.cs
{

    /// <summary>
    /// 可撤銷接口
    /// </summary>
    public interface IUndoable {
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
        /// 執行
        /// </summary>
        void Execute();
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
        public abstract void Execute();

        /// <summary>
        /// 復原
        /// </summary>
        public abstract void Undo();
    }

    /// <summary>
    /// 命令管理器
    /// </summary>
    public class CommandManager : IUndoable {

        /// <summary>
        /// 撤銷堆疊
        /// </summary>
        private List<ICommand> mUndoList = new List<ICommand>();
        /// <summary>
        /// 重做堆疊
        /// </summary>
        private List<ICommand> mRedoList = new List<ICommand>();

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
        public void ExecutCmd(ICommand cmd) {
            cmd.Execute();
            mUndoList.Add(cmd);
            if (UndoLimit != -1 && mUndoList.Count > UndoLimit) {
                mUndoList.RemoveAt(0);
            }
            mRedoList.Clear();
        }

        /// <summary>
        /// 撤銷命令
        /// </summary>
        public void Undo() {
            if (mUndoList.Any()) {
                var cmd = mUndoList.Last();
                cmd.Undo();
                mUndoList.Remove(cmd);
                mRedoList.Add(cmd);
            }
        }

        /// <summary>
        /// 重做命令
        /// </summary>
        public void Redo() {
            if (mRedoList.Any()) {
                var cmd = mRedoList.Last();
                cmd.Execute();
                mRedoList.Remove(cmd);
                mUndoList.Add(cmd);
            }
        }

        /// <summary>
        /// 清除命令記錄
        /// </summary>
        public void Clear() {
            mUndoList.Clear();
            mRedoList.Clear();
        }

    }

}
