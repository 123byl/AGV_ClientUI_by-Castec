using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Drawing;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Windows.Forms;

namespace CtLib.Library {

    /// <summary>提供常用的物件擴充方法</summary>
    public static class CtExtension {

        /// <summary>提供 <see cref="IEnumerable{T}"/> 之簡易反覆查看集合內容方法，無法於內部進行加入或移除集合項目</summary>
        /// <typeparam name="TObj">可套用於 IEnumerable 之 <see cref="Type"/></typeparam>
        /// <param name="enumObj">欲執行的集合</param>
        /// <param name="action">每項集合內容所需執行的內容</param>
        public static void ForEach<TObj>(this IEnumerable<TObj> enumObj, Action<TObj> action) {
            foreach (TObj item in enumObj) {
                action(item);
            }
        }

        /// <summary>提供 <see cref="Array"/> 之簡易反覆查看集合內容方法，無法於內部進行加入或移除集合項目</summary>
        /// <typeparam name="TObj">可套用於 <see cref="Array"/> 之 <see cref="Type"/></typeparam>
        /// <param name="ary">欲執行的集合</param>
        /// <param name="action">每項集合內容所需執行的內容</param>
        public static void ForEach<TObj>(this TObj[] ary, Action<TObj> action) {
            Array.ForEach(ary, action);
        }

        /// <summary>提供將某一個類型的陣列轉換成另一個類型的陣列</summary>
        /// <typeparam name="TSrc">來源陣列型態</typeparam>
        /// <typeparam name="TTar">欲轉換的陣列型態</typeparam>
        /// <param name="ary">欲執行的陣列</param>
        /// <param name="action">轉換執行內容</param>
        /// <returns>轉換後的陣列</returns>
        public static TTar[] ConvertAll<TSrc, TTar>(this TSrc[] ary, Func<TSrc, TTar> action) {
            return Array.ConvertAll(ary, new Converter<TSrc, TTar>(action));
        }

        /// <summary>提供將某一個類型的陣列轉換成另一個類型的 <see cref="List{T}"/></summary>
        /// <typeparam name="TSrc">來源陣列型態</typeparam>
        /// <typeparam name="TTar">欲轉換的 <see cref="List{T}"/> 型態</typeparam>
        /// <param name="ary">欲執行的陣列</param>
        /// <param name="action">轉換執行內容</param>
        /// <returns>轉換後的陣列</returns>
        /// <remarks>經測試，使用 Array.ConvertAll().ToList() > foreach > .Select().ToList()</remarks>
        public static List<TTar> ConvertToList<TSrc, TTar>(this TSrc[] ary, Converter<TSrc, TTar> action) {
            return Array.ConvertAll(ary, action).ToList();
        }

        /// <summary>提供 <see cref="ConcurrentQueue{T}"/> 之將內容全部清空之簡易擴充</summary>
        /// <typeparam name="TObj">適用於 ConcurrentQueue 之 <see cref="Type"/></typeparam>
        /// <param name="queue">欲清空內容的先進先出集合</param>
        public static void Clear<TObj>(this ConcurrentQueue<TObj> queue) {
            TObj tempObj;
            while (queue.Count > 0) queue.TryDequeue(out tempObj);
        }

        /// <summary>提供 <see cref="DataGridViewRowCollection"/> 可加入多個 <see cref="DataGridViewRow"/></summary>
        /// <param name="rowColl">欲加入新列的集合物件</param>
        /// <param name="rows">欲加入的新列</param>
        public static void AddRange(this DataGridViewRowCollection rowColl, IEnumerable<DataGridViewRow> rows) {
            foreach (var row in rows) {
                rowColl.Add(row);
            }
        }

        /// <summary>繪製由座標對、寬度和高度所指定的矩形</summary>
        /// <param name="graphic">欲繪製的 GDI+ 物件</param>
        /// <param name="pen"><see cref="Pen"/>，決定矩形的色彩、寬度和樣式</param>
        /// <param name="rect"><see cref="RectangleF"/> 結構，表示要繪製的矩形</param>
        public static void DrawRectangle(this Graphics graphic, Pen pen, RectangleF rect) {
            graphic.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
        }

        /// <summary>將 <see cref="XDocument"/> 儲存成含有縮排的 XML 文件</summary>
        /// <param name="doc">欲進行存檔的 <see cref="XDocument"/></param>
        /// <param name="path">欲儲存的檔案路徑，如 @"D:\file.xml"</param>
        public static void SaveWithIndent(this XDocument doc, string path) {
            XmlWriterSettings setting = new XmlWriterSettings() { Indent = true, IndentChars = "\t" };
            using (XmlWriter wr = XmlWriter.Create(path, setting)) {
                doc.Save(wr);
            }
        }

        /// <summary>取得 <see cref="Enum"/> 所附加的 <see cref="CultureKey"/></summary>
        /// <typeparam name="TEnum">列舉</typeparam>
        /// <param name="e">欲取得 <see cref="CultureKey"/> 的 Enum</param>
        /// <returns><see cref="CultureKey.KeyWord"/></returns>
        public static string GetCultureKey<TEnum>(this TEnum e) where TEnum : struct {
            var retStr = string.Empty;
            var type = typeof(TEnum);
            if (!type.IsEnum) throw new ArgumentException($"GetCultrueKey just for \"enum\" but got {type.ToString()}", "type");

            var membInfos = type.GetMember(e.ToString());
            if (membInfos != null && membInfos.Length > 0) {
                var attrs = membInfos[0].GetCustomAttributes(typeof(CultureKey), false);
                if (attrs != null && attrs.Length > 0) {
                    retStr = (attrs[0] as CultureKey).KeyWord;
                }
            }

            return retStr;
        }

        /// <summary>取得 <see cref="Enum"/> 的完整名稱。如 MyEnum.FIRST_ENUM</summary>
        /// <param name="e">欲取得名稱之列舉</param>
        /// <returns>列舉完整名稱</returns>
        public static string GetFullName<TEnum>(this TEnum e) where TEnum : struct {
            return $"{e.GetType().Name}.{e.ToString()}";
        }

        /// <summary>將 <see cref="string"/> 轉換為對應的列舉 <see cref="Enum"/></summary>
        /// <typeparam name="TEnum">欲轉換的目標列舉類型</typeparam>
        /// <param name="str">欲轉換的字串</param>
        /// <returns>目標列舉</returns>
        /// <example><code language="c#">
        /// enum Operation {
        ///		Add,
        ///		Product
        /// }
        /// 
        /// string enumStr = "Add";
        /// Operation op = enumStr.ToEnum&lt;Operation&gt;();
        /// </code></example>
        public static TEnum ToEnum<TEnum>(this string str) where TEnum : struct {
            return (TEnum)Enum.Parse(typeof(TEnum), str, true);
        }
    }
}
