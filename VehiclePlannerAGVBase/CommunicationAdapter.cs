using Geometry;
using GLCore;
using SerialCommunicationData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VehiclePlanner.Core;

namespace VehiclePlannerAGVBase
{
    /// <summary>
    /// 雷射要求回传
    /// </summary>
    public class RequestLaser: BaseRequeLaser
    {
        
        private List<IPair> mLaser = null;

        public RequestLaser(IProductPacket response)
        {
            mLaser = response.ToIRequestLaser().Product;
            Count = mLaser?.Count() ?? 0;
            Requited = mLaser != null;
        }

        public override void DrawLaser()
        {
            Database.AGVGM[1]?.LaserAPoints.DataList.Replace(mLaser);
        }
    }

    /// <summary>
    /// 马达激磁设定
    /// </summary>
    public class SetServoMode : BaseSetServoMode
    {
        public SetServoMode(IProductPacket response)
        {
            var v = response.ToISetServoMode();
            if (Requited = v != null)
            {
                Value = v.Product;
            }
        }
    }

    /// <summary>
    /// 手动移动速度设定
    /// </summary>
    public class SetWorkVelocity : BaseSetWorkVelocity
    {
        public SetWorkVelocity(IProductPacket response)
        {
            var v = response.ToISetWorkVelocity();
            if (Requited = v != null)
            {
                Value = v.Product;
            }
        }
    }

    /// <summary>
    /// 位置校正
    /// </summary>
    public class DoPositionComfirm : BaseDoPositionComfirm
    {
        public DoPositionComfirm(IProductPacket response)
        {
            var v = response.ToIDoPositionComfirm();
            if (Requited = v != null)
            {
                Similarity = v.Product;
            }
        }
    }

    public class GoTo : BaseGoTo
    {
        public GoTo(IProductPacket response)
        {
            Requited = false;
            if (response != null)
            {
                switch (response.Purpose)
                {
                    case AGVDefine.EPurpose.DoRuningByGoalName:
                        var v = response.ToIDoRuningByGoalName();
                        if (Requited = v != null)
                        {
                            Started = v.Product;
                        }
                        break;
                    case AGVDefine.EPurpose.DoCharging:
                        v = response.ToIDoCharging();
                        if (Requited = v != null)
                        {
                            Started = v.Product;
                        }
                        break;
                }
            }
        }
    }

    /// <summary>
    /// 要求Map档清单
    /// </summary>
    public class RequestMapList : BaseRequestMapList
    {
        public RequestMapList(IProductPacket response)
        {
            var v = response.ToIRequestMapList();
            if (Requited =  v != null)
            {
                var list = v.Product;
                if (list != null)
                {
                    ListString = string.Join(",", list);
                    Count = list.Count;
                }
            }
        }
    }

    /// <summary>
    /// 要求Map档
    /// </summary>
    public class RequestMapFile : BaseRequestMapFile
    {
        /// <summary>
        /// 档案物件
        /// </summary>
        private IDocument mDoc = null;

        public RequestMapFile(IProductPacket response)
        {
            var v = response.ToIRequestMapFile();
            if (Requited = v != null)
            {
                mDoc = v.Product;
                FileName = mDoc.Name;
            }
        }

        /// <summary>
        /// 存档
        /// </summary>
        /// <param name="path">储存路径</param>
        /// <returns>是否储存成功</returns>
        public override bool SaveAs(string path) => mDoc?.SaveAs(path) ?? false;

    }

    /// <summary>
    /// 要求到目标点的路径
    /// </summary>
    public class RequestPath : BaseRequestPath
    {
        public RequestPath(IProductPacket response)
        {
            var v = response.ToIRequestPath();
            if (Requited = v != null)
            {
                Count = v.Product.Count;
            }
        }
    }

    /// <summary>
    /// 要求Goal点清单
    /// </summary>
    public class RequestGoalList : BaseRequestGoalList
    {
        public RequestGoalList(IProductPacket response)
        {
            var v = response.ToIRequestGoalList();
            if (Requited = v != null)
            {
                List<string> list = v.Product;
                ListString = string.Join(",", list);
                Count = list.Count;
            }
        }
    }

    /// <summary>
    /// 開始手動控制
    /// </summary>
    public class StartManualControl : BaseStartManualControl
    {
        public StartManualControl(IProductPacket response)
        {
            var v = response.ToIStartManualControl();
            if (Requited = v != null)
            {
                Value = v.Product;
            }
        }
    }

    /// <summary>
    /// 設定地圖檔名
    /// </summary>
    /// <remarks>是否在掃描中</remarks>
    public class SetScanningOriFileName : BaseSetScanningOriFileName
    {
        public SetScanningOriFileName(IProductPacket response)
        {
            var v = response.ToISetScanningOriFileName();
            if (Requited = v!= null)
            {
                Value = v.Product;
            }
        }
    }

    /// <summary>
    /// 停止掃描地圖
    /// </summary>
    public class StopScanning : BaseStopScanning
    {
        public StopScanning(IProductPacket response)
        {
            var v = response.ToIStopScanning();
            if (Requited = v != null)
            {
                Value = v.Product;
            }
        }
    }
    
    /// <summary>
    /// 要求iTS載入指定的Map檔
    /// </summary>
    public class ChangeMap : BaseChangeMap
    {
        public ChangeMap(IProductPacket response)
        {
            var v = response.ToIChangeMap();
            if (Requited = v != null)
            {
                Value = v.Product;
            }
        }
    }
    
    /// <summary>
    /// 上傳Map檔
    /// </summary>
    public class UploadMapToAGV : BaseUploadMapToAGV
    {
        public UploadMapToAGV(IProductPacket response)
        {
            var v = response.ToIUploadMapToAGV();
            if (Requited = v != null)
            {
                Value = v.Product;
            }
        }
    }

    /// <summary>
    /// 設定手動控制移動速度(方向)
    /// </summary>
    public class SetManualVelocity : BaseSetManualVelocity
    {
        public SetManualVelocity(IProductPacket response)
        {
            var v = response.ToISetManualVelocity();
            if (Requited = v != null)
            {
                Value = v.Product;
            }
        }
    }
}
