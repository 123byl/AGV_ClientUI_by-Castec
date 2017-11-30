using System.Collections.Generic;
using System.Linq;
using MapProcessing;
namespace ClientUI
{

    /// <summary>
    /// 車子狀態資訊
    /// </summary>
    public class CarInfo : CartesianPosInfo
    {
        /// <summary>
        /// 雷射掃描資料
        /// </summary>
        public IEnumerable<int> LaserData;

        public CarInfo(double x, double y, double theta, string name, uint id) : base(x, y, theta, name, id)
        {
        }

        /// <summary>
        /// 電池電量百分比
        /// </summary>
        public int PowerPercent;

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
        public static bool TryParse(string src, ref CarInfo info)
        {
            IEnumerable<int> laserData = null;
            return TryParse(src, ref info, out laserData);
        }

        /// <summary>
        /// 嘗試將字串轉換為<see cref="CarInfo"/>
        /// </summary>
        /// <param name="src">來源字串，格式為"Get:Car:Name:X:Y:Toward:Power:LaserData1,2,3..,n:Status"</param>
        public static bool TryParse(string src, ref CarInfo info, out IEnumerable<int> laserData)
        {
            //info = new CarInfo(0,0,0,"",0);
            string[] strArray = src.Split(':');
            laserData = null;
            if (strArray.Length != 9) return false;
            if (strArray[0] != "Get") return false;
            if (strArray[1] != "Car") return false;

            info.name = strArray[2];
            
            double.TryParse(strArray[3], out info.x);
            double.TryParse(strArray[4], out info.y);
            double.TryParse(strArray[5], out info.theta);            
            int.TryParse(strArray[6], out info.PowerPercent);
            
            string[] sreRemoteLaser = strArray[7].Split(',');
            info.LaserData = sreRemoteLaser.Select(item => int.Parse(item));
            info.Status = strArray[8];
            laserData = info.LaserData;
            return true;
        }
    }
}