using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VehiclePlanner.Core
{

    /// <summary>
    /// 回传适配器
    /// </summary>
    public abstract class BaseAdapter
    {
        /// <summary>
        /// 是否有回传
        /// </summary>
        public bool Requited { get; protected set; } = false;
    }

    /// <summary>
    /// 雷射要求回传
    /// </summary>
    public abstract class BaseRequeLaser :BaseAdapter
    {
        /// <summary>
        /// 雷射点数量
        /// </summary>
        public int Count { get; protected set; } = 0;
        /// <summary>
        /// 绘制雷射
        /// </summary>
        public abstract void DrawLaser();
    }

    /// <summary>
    /// 位置校正
    /// </summary>
    public abstract class BaseDoPositionComfirm : BaseAdapter
    {
        /// <summary>
        /// 地图匹配度
        /// </summary>
        public double Similarity { get; protected set; } = 0;
    }

    /// <summary>
    /// 移动到目标点
    /// </summary>
    public abstract class BaseGoTo : BaseAdapter
    {
        /// <summary>
        /// 是否開始移動
        /// </summary>
        public bool Started { get; protected set; } = false;
        /// <summary>
        /// 目标点名称
        /// </summary>
        public string GoalName { get; protected set; } = null;

    }
    
    
    /// <summary>
    /// 要求到目标点的路径
    /// </summary>
    public abstract class BaseRequestPath:BaseAdapter
    {
        /// <summary>
        /// 路径点数量
        /// </summary>
        public int Count { get; protected set; } = 0;
    }
    
    #region BoolReturn

    /// <summary>
    /// 布林資料回傳
    /// </summary>
    public abstract class BaseBoolReturn :BaseAdapter
    {
        /// <summary>
        /// 回傳值
        /// </summary>
        public bool Value { get; protected set; }
    }
    
    /// <summary>
    /// 马达激磁设定
    /// </summary>
    public abstract class BaseSetServoMode : BaseBoolReturn { }

    /// <summary>
    /// 手动移动速度设定
    /// </summary>
    public abstract class BaseSetWorkVelocity : BaseBoolReturn { }

    /// <summary>
    /// 開始手動控制
    /// </summary>
    public abstract class BaseStartManualControl : BaseBoolReturn { }

    /// <summary>
    /// 設定地圖檔名
    /// </summary>
    /// <remarks>是否在掃描中</remarks>
    public abstract class BaseSetScanningOriFileName : BaseBoolReturn { }

    /// <summary>
    /// 停止掃描地圖
    /// </summary>
    public abstract class BaseStopScanning : BaseBoolReturn { }

    /// <summary>
    /// 要求iTS載入指定的Map檔
    /// </summary>
    public abstract class BaseChangeMap : BaseBoolReturn { }

    /// <summary>
    /// 上傳Map檔
    /// </summary>
    public abstract class BaseUploadMapToAGV : BaseBoolReturn { }
    
    /// <summary>
    /// 設定手動控制移動速度(方向)
    /// </summary>
    public abstract class BaseSetManualVelocity : BaseBoolReturn { }

	/// <summary>
	/// 停止運動
	/// </summary>
	public abstract class BaseStop : BaseBoolReturn { }

	/// <summary>
	/// 停止充電
	/// </summary>
	public abstract class BaseUncharge : BaseBoolReturn { }

    #endregion BoolReturn

    #region ListReturn

    /// <summary>
    /// 清單資料回傳
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseListReturn : BaseAdapter
    {
        /// <summary>
        /// 清單元素數量
        /// </summary>
        public int Count { get; protected set; }

        /// <summary>
        /// 將所有元素用","符號連接成一個字串
        /// </summary>
        public string ListString { get; protected set; }

    }
    
    /// <summary>
    /// 要求Goal点清单
    /// </summary>
    public abstract class BaseRequestGoalList : BaseListReturn { }

    /// <summary>
    /// 要求Map档清单
    /// </summary>
    public abstract class BaseRequestMapList : BaseListReturn { }

    #endregion ListReturn

    #region FileReturn

    /// <summary>
    /// 檔案回傳
    /// </summary>
    public abstract class BaseFileReturn : BaseAdapter
    {
        /// <summary>
        /// 档名
        /// </summary>
        public string FileName { get; protected set; }

        /// <summary>
        /// 存档
        /// </summary>
        /// <param name="path">储存路径</param>
        /// <returns>是否储存成功</returns>
        public abstract bool SaveAs(string path);

    }

    /// <summary>
    /// 要求Map档
    /// </summary>
    public abstract class BaseRequestMapFile : BaseFileReturn { }

	public abstract class BaseRequestOriFile : BaseFileReturn { }
    
    #endregion FileReturn


}
