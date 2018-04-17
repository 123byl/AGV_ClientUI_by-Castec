using System.Collections.Generic;
using System.Linq;

namespace AGVMap
{
    /// <summary>
    /// 車子狀態資訊
    /// </summary>
    public class CarInfo : Info
    {
        /// <summary>
        /// 雷射掃描資料
        /// </summary>
        public IEnumerable<int> LaserData;

        public CarInfo(int id, string name, int x, int y, Angle toward) : base(id, name, x, y, toward)
        {
        }

        public CarInfo() : base()
        {
        }

        /// <summary>
        /// 電池電量百分比
        /// </summary>
        public int PowerPercent { get; set; }

        /// <summary>
        /// 當前訊息
        /// </summary>
        public string Status { get; set; }


        /// <summary>
        /// 嘗試將字串轉換為<see cref="CarInfo"/>
        /// </summary>
        /// <param name="src">來源字串</param>
        /// <param name="info">轉換後的<see cref="CarInfo"/></param>
        /// <returns>是否轉換成功</returns>
        public static bool TryParse(string src, out CarInfo info)
        {
            IEnumerable<int> laserData = null;
            return TryParse(src, out info, out laserData);
        }

        /// <summary>
        /// 嘗試將字串轉換為<see cref="CarInfo"/>
        /// </summary>
        /// <param name="src">來源字串，格式為"Get:Car:Name:X:Y:Toward:Power:LaserData1,2,3..,n:Status"</param>
        public static bool TryParse(string src, out CarInfo info, out IEnumerable<int> laserData)
        {
            info = new CarInfo();
            string[] strArray = src.Split(':');
            laserData = null;
            if (strArray.Length != 9) return false;
            if (strArray[0] != "Get") return false;
            if (strArray[1] != "Car") return false;

            info.Name = strArray[2];
            int x; int.TryParse(strArray[3], out x);
            int y; int.TryParse(strArray[4], out y);
            double toward; double.TryParse(strArray[5], out toward);
            int powerPercent; int.TryParse(strArray[6], out powerPercent);
            info.X = x;
            info.Y = y;
            info.Toward = toward;
            info.PowerPercent = powerPercent;
            string[] sreRemoteLaser = strArray[7].Split(',');
            info.LaserData = sreRemoteLaser.Select(item => int.Parse(item));
            info.Status = strArray[8];
            laserData = info.LaserData;
            return true;
        }
    }

    public interface IInfo: IToward, IPair, INameable, IID
    {

    }

    /// <summary>
    /// 點狀態資訊
    /// </summary>
    public class Info : TowardPos, IInfo
    {
        public Info()
        {
            ID = Factory.CreatID.NewID;
        }

        public Info(int id, string name, TowardPos towardPos) : base(towardPos)
        {
            ID = id;
            Name = name;
        }

        public Info(int id, string name, int x, int y, Angle toward) : base(x, y, toward)
        {
            ID = id;
            Name = name;
        }

        /// <summary>
        /// 唯一識別碼
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 名稱
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 顯示完整字串
        /// </summary>
        public override string ToString()
        {
            return string.Format("{0}:{1}({2},{3},{4:F2})", ID, Name, X, Y, Toward);
        }
    }

    /// <summary>
    /// 具有方向性的座標點
    /// </summary>
    public class TowardPos : IToward, IPair
    {
        public TowardPos()
        {
        }

        public TowardPos(TowardPos towardPos)
        {
            X = towardPos.X;
            Y = towardPos.Y;
            Toward = towardPos.Toward;
        }

        public TowardPos(int x, int y, Angle toward)
        {
            X = x;
            Y = y;
            Toward = toward;
        }

        /// <summary>
        /// 座標點
        /// </summary>
        public Pair Pos { get; set; } = new Pair();

        /// <summary>
        /// 方向，提供一個介於 [0,360) 之間的角度
        /// </summary>
        public Angle Toward { get; set; } = new Angle();

        /// <summary>
        /// X 座標
        /// </summary>
        public int X { get { return Pos.X; } set { Pos.X = value; } }

        /// <summary>
        /// Y 座標
        /// </summary>
        public int Y { get { return Pos.Y; } set { Pos.Y = value; } }

        /// <summary>
        /// 設定位置
        /// </summary>
        public void SetPos<T>(T pos) where T : IPair
        {
            X = pos.X;
            Y = pos.Y;
        }

        /// <summary>
        /// 設定位置和首向
        /// </summary>
        public void SetPosAndToward<T>(T pos) where T : IPair, IToward
        {
            X = pos.X;
            Y = pos.Y;
            Toward = pos.Toward;
        }

        /// <summary>
        /// 設定位置
        /// </summary>
        public void SetPosition(int x, int y, Angle toward)
        {
            X = x;
            Y = y;
            Toward = toward;
        }

        #region 運算子

        public static bool operator !=(TowardPos p1, TowardPos p2)
        {
            return !(p1 == p2);
        }

        public static bool operator ==(TowardPos lhs, TowardPos rhs)
        {
            return lhs.Pos == rhs.Pos && lhs.Toward == rhs.Toward;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is TowardPos)) return false;
            return this == (obj as TowardPos);
        }

        public override int GetHashCode()
        {
            return Pos.GetHashCode() ^ Toward.GetHashCode();
        }

        #endregion 運算子
    }
}