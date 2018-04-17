using AGV.Map.Common;
using System;

namespace AGV.Map.Core
{
    /// <summary>
    /// 目標點
    /// </summary>
    [Serializable]
    internal class Goal : SingleTowardPair, IGoal
    {
        public Goal(string name) : base(name)
        {
            GLSetting = new GLSetting(EType.Goal);
        }

        public Goal(int x, int y, double toward, string name) : base(x, y, toward, name)
        {
            GLSetting = new GLSetting(EType.Goal);
        }

        public Goal(int x, int y, IAngle toward, string name) : base(x, y, toward, name)
        {
            GLSetting = new GLSetting(EType.Goal);
        }

        public Goal(ITowardPair towardPair, string name) : base(towardPair, name)
        {
            GLSetting = new GLSetting(EType.Goal);
        }
    }
}