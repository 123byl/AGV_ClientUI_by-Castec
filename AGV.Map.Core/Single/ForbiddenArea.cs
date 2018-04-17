using AGV.Map.Common;

namespace AGV.Map.Core
{
    /// <summary>
    /// 禁止面
    /// </summary>
    internal class ForbiddenArea : SingleArea, IForbiddenArea
    {
        public ForbiddenArea(string name) : base(name)
        {
            GLSetting = new GLSetting(EType.ForbiddenArea);
        }

        public ForbiddenArea(int x0, int y0, int x1, int y1, string name) : base(x0, x1, y0, y1, name)
        {
            GLSetting = new GLSetting(EType.ForbiddenArea);
        }

        public ForbiddenArea(IArea area, string name) : base(area, name)
        {
            GLSetting = new GLSetting(EType.ForbiddenArea);
        }

        public ForbiddenArea(IPair min, IPair max, string name) : base(min, max, name)
        {
            GLSetting = new GLSetting(EType.ForbiddenArea);
        }
    }
}