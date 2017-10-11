using AGV.Map.Common;
using System;

namespace AGV.Map.Core
{
    /// <summary>
    /// 窄道暫時停車區
    /// </summary>
    [Serializable]
    internal class Parking : SingleTowardPair, IParking
    {
        public Parking(string name) : base(name)
        {
            GLSetting = new GLSetting(EType.Parking);
        }

        public Parking(int x, int y, double toward, string name) : base(x, y, toward, name)
        {
            GLSetting = new GLSetting(EType.Parking);
        }

        public Parking(int x, int y, IAngle toward, string name) : base(x, y, toward, name)
        {
            GLSetting = new GLSetting(EType.Parking);
        }

        public Parking(ITowardPair towardPair, string name) : base(towardPair, name)
        {
            GLSetting = new GLSetting(EType.Parking);
        }
    }
}