using AGV.Map.Common;
using System;

namespace AGV.Map.Core
{
    /// <summary>
    /// 窄道
    /// </summary>
    [Serializable]
    internal class NarrowLine : SingleLine, INarrowLine
    {
        public NarrowLine(string name) : base(name)
        {
            GLSetting = new GLSetting(EType.NarrowLine);
        }

        public NarrowLine(int x0, int y0, int x1, int y1, string name) : base(x0, x1, y0, y1, name)
        {
            GLSetting = new GLSetting(EType.NarrowLine);
        }

        public NarrowLine(ILine line, string name) : base(line, name)
        {
            GLSetting = new GLSetting(EType.NarrowLine);
        }

        public NarrowLine(IPair beg, IPair end, string name) : base(beg, end, name)
        {
            GLSetting = new GLSetting(EType.NarrowLine);
        }
    }
}