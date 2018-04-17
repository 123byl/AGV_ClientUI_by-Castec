using AGV.Map.Common;
using System;

namespace AGV.Map.Core
{
    /// <summary>
    /// 禁止線
    /// </summary>
    [Serializable]
    internal class ForbiddenLine : SingleLine, IForbiddenLine
    {
        public ForbiddenLine(string name) : base(name)
        {
            GLSetting = new GLSetting(EType.ForbiddenLine);
        }

        public ForbiddenLine(int x0, int y0, int x1, int y1, string name) : base(x0, x1, y0, y1, name)
        {
            GLSetting = new GLSetting(EType.ForbiddenLine);
        }

        public ForbiddenLine(ILine line, string name) : base(line, name)
        {
            GLSetting = new GLSetting(EType.ForbiddenLine);
        }

        public ForbiddenLine(IPair beg, IPair end, string name) : base(beg, end, name)
        {
            GLSetting = new GLSetting(EType.ForbiddenLine);
        }
    }
}