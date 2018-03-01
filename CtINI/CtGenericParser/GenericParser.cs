using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CtGenericParser
{

    /// <summary>
    /// 通用解析器
    /// </summary>
    /// <remarks>
    /// 透過Assignment<T>方法分配T類型TryParse方法委派後
    /// 之後便可透過GenericParser.TryParse<T>方法調用T類型TryParse方法
    /// 
    /// 若是沒有透過Assignment<T>方法分配T類型TryParse方法委派
    /// 則尋找T類型的靜態TryParse方法產生委派
    /// </remarks>
    public static class GenericParser {

        #region Function - Public Methods

        /// <summary>
        /// 將字串轉換為對等<see cref="T"/>變數
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryParse<T>(string s, out T result) {
            return  Parser<T>.TryParse.Invoke(s, out result);
        }

        /// <summary>
        /// 指定<see cref="T"/>TryParse方法委派
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="delTryParse"></param>
        public static void Assignment<T>(Parser<T>.DelTryParse delTryParse) {
            Parser<T>.mTryParse = delTryParse;
        }

        /// <summary>
        /// 清除<see cref="T"/>TryParse方法委派
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void RemoveDelegate<T>() {
            Parser<T>.mTryParse = null;
        }

        #endregion Function - Public Methods

        #region Support Class

        /// <summary>
        /// 解析器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class Parser<T> {

            #region Declaration - Delegates

            /// <summary>
            /// TryParse委派定義
            /// </summary>
            /// <param name="content"></param>
            /// <param name="val"></param>
            /// <returns></returns>
            public delegate bool DelTryParse(string content, out T val);

            #endregion Declaration - Delegates

            #region Declaration - Fields

            /// <summary>
            /// TryParse方法委派
            /// </summary>
            protected internal static DelTryParse mTryParse = null;

            #endregion Declaration - Fields

            #region Declaration - Properties
            
            /// <summary>
            /// TryParse方法委派
            /// </summary>
            protected internal static DelTryParse TryParse {
                get {
                    if (mTryParse == null) mTryParse = CreateTryParseDelegate(typeof(T));
                    return mTryParse;
                } set {
                    mTryParse = value;
                }
            }

            #endregion Declaration - Properties

            #region Function - Constructors

            private Parser() { }

            #endregion Function - Constructors

            #region Function - Private Methods

            /// <summary>
            /// 尋找<see cref="Type"/>的TryParse方法產生委派
            /// </summary>
            /// <param name="type"></param>
            /// <returns></returns>
            private static DelTryParse CreateTryParseDelegate(Type type) {
                MethodInfo methInfo = null;
                if (type.IsEnum) {
                    methInfo = type
                        .GetMethods(BindingFlags.Static | BindingFlags.FlattenHierarchy | BindingFlags.Public)
                        .First(x => x.Name == "TryParse" && x.GetParameters().Length == 2)
                        .MakeGenericMethod(type);
                } else if (!type.IsInterface) { 
                    methInfo = type.GetMethod("TryParse", BindingFlags.Public | BindingFlags.Static, Type.DefaultBinder,
                                                        new Type[] { typeof(string), type.MakeByRefType() },
                                                        new ParameterModifier[] { new ParameterModifier(2) });
                }

                if (methInfo == null) {
                    throw new Exception($"無{type.Name}TryParse方法委派");
                }
                return (DelTryParse)Delegate.CreateDelegate(typeof(DelTryParse), methInfo);
            }

            #endregion Function - Private Methdos

        }

        #endregion Support Class
    }

}
