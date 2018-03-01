using CtParamEditor.Comm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtParamEditor.Core.Public {

    /// <summary>
    /// 參數編輯器工廠
    /// </summary>
    public class Factory {
    
        /// <summary>
        /// 參數編輯器
        /// </summary>
        /// <returns></returns>
        public IParamEditor Editor() {
            return new ParamEditor();
        }

    }
}
