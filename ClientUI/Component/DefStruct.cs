using System.Collections.Generic;
using System.Linq;
using MapProcessing;
using Geometry;

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
        public IEnumerable<IPair> LaserData;

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
            IEnumerable<IPair> laserData = null;
            return TryParse(src, ref info, out laserData);
        }

        /// <summary>
        /// 嘗試將字串轉換為<see cref="CarInfo"/>
        /// </summary>
        /// <param name="src">來源字串，格式為"Get:Car:Name:X:Y:Toward:Power:LaserData1,2,3..,n:Status"</param>
        public static bool TryParse(string src, ref CarInfo info, out IEnumerable<IPair> laserData)
        {
            //info = new CarInfo(0,0,0,"",0);
            string[] strArray = src.Split(':');
            laserData = null;
            if (strArray.Length != 6) return false;
            if (strArray[0] != "Get") return false;
            if (strArray[1] != "Car") return false;

            info.name = strArray[2];
            
       
            int.TryParse(strArray[3], out info.PowerPercent);
            string[] sreRemoteLaser = strArray[4].Split(',');
            double.TryParse(sreRemoteLaser[0], out info.x);
            double.TryParse(sreRemoteLaser[1], out info.y);
            double.TryParse(sreRemoteLaser[2], out info.theta);
            int pointCount = (sreRemoteLaser.Count() - 3);
            string[] temp = new string[pointCount];
            System.Array.Copy(sreRemoteLaser,3, temp,0, pointCount);
            List<IPair> laser = new List<IPair>();
            for (int i = 0; i < temp.Count()-1; i += 2) {
                laser.Add(FactoryMode.Factory.Pair(int.Parse(temp[i]), int.Parse(temp[i + 1])));
            }
            info.LaserData = laser;
            info.Status = strArray[5];
            laserData = info.LaserData;
            return true;
        }
    }
}