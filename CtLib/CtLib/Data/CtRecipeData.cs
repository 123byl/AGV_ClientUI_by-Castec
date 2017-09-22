using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using CtLib.Library;
using CtLib.Module.Beckhoff;
using CtLib.Module.Adept;
using CtLib.Module.Ultity;

namespace CtLib.Library {

    /// <summary>
    /// Recipe 相關資訊
    /// <para>目前如果是要儲存 Adept ACE 相關路徑，如 Locator Model 等路徑，請將類別設為 <see cref="Devices.CAMPro"/> 而非 <seealso cref="Devices.ADEPT_ACE"/></para>
    /// </summary>
    public class CtRecipeData {

        #region Version

        /// <summary>CtRecipeData 版本訊息</summary>
        /// <remarks><code>
        /// 1.0.0  Ahern [2015/02/04]
        ///     + 從 CtNexcom_Recipe 獨立至此
        /// </code></remarks>
        public static readonly CtVersion @Version = new CtVersion(1, 0, 0, "2015/02/04", "Ahern Kuo");

        #endregion

        #region Declaration - Members
        /// <summary>來源設備</summary>
        public Devices Source { get; internal set; }
        /// <summary>變數名稱</summary>
        public string Name { get; internal set; }
        /// <summary>註解</summary>
        public string Comment { get; internal set; }
        /// <summary>數值</summary>
        public object Value { get; internal set; }
        /// <summary>多重來源設備時，此 Recipe 為第幾個設備之項目</summary>
        public int DeviceIndex { get; internal set; }
        /// <summary>[ACE] 變數型態</summary>
        public CtAce.VPlusVariableType AceVarType { get; internal set; }
        /// <summary>[BECKHOFF] 變數型態</summary>
        public CtBeckhoff.SymbolType BkfVarType { get; internal set; }
        #endregion

        #region Function - Constructors
        /// <summary>建立全空不帶預設值之RecipeData</summary>
        public CtRecipeData() { }

        /// <summary>建立帶有預設值之RecipeData</summary>
        /// <param name="source">來源設備</param>
        /// <param name="name">變數名稱</param>
        /// <param name="value">數值</param>
        /// <param name="comment">註解</param>
        /// <param name="srcIdx">多重設備之索引。如有三台 Beckhoff PLC，則此 Recipe 於第 2 台之項目，則此處填入 "1" (從 0 開始)</param>
        public CtRecipeData(Devices source, string name, object value, string comment, int srcIdx = 0) {
            Source = source;
            Name = name;
            Value = value;
            Comment = comment;
            DeviceIndex = srcIdx;
        }

        /// <summary>建立帶有預設值之RecipeData</summary>
        /// <param name="source">來源設備</param>
        /// <param name="name">變數名稱</param>
        /// <param name="value">數值</param>
        /// <param name="comment">註解</param>
        /// <param name="varType">[ACE專用] 變數型態</param>
        /// <param name="srcIdx">多重設備之索引。如有三台 Beckhoff PLC，則此 Recipe 於第 2 台之項目，則此處填入 "1" (從 0 開始)</param>
        public CtRecipeData(Devices source, string name, object value, CtAce.VPlusVariableType varType, string comment, int srcIdx = 0) {
            Source = source;
            Name = name;
            Value = value;
            Comment = comment;
            AceVarType = varType;
            DeviceIndex = srcIdx;
        }

        /// <summary>建立帶有預設值之RecipeData</summary>
        /// <param name="source">來源設備</param>
        /// <param name="name">變數名稱</param>
        /// <param name="value">數值</param>
        /// <param name="comment">註解</param>
        /// <param name="varType">[Beckhoff專用] 變數型態</param>
        /// <param name="srcIdx">多重設備之索引。如有三台 Beckhoff PLC，則此 Recipe 於第 2 台之項目，則此處填入 "1" (從 0 開始)</param>
        public CtRecipeData(Devices source, string name, object value, CtBeckhoff.SymbolType varType, string comment, int srcIdx = 0) {
            Source = source;
            Name = name;
            Value = value;
            Comment = comment;
            BkfVarType = varType;
            DeviceIndex = srcIdx;
        }
        #endregion

    }
}
