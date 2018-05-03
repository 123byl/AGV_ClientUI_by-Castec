using CtParamEditor.Comm;
using CtParamEditor.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INITesting {
    internal class Factory {
        private Factory() { }
        public static CtParamEditor.Core.Public.Factory Param { get; } = new CtParamEditor.Core.Public.Factory();
        
    }

}
