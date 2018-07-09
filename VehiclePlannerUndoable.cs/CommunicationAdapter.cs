using Geometry;
using GLCore;
using Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VehiclePlanner.Core;
using SerialData;

namespace VehiclePlannerUndoable.cs
{

    public enum EmID
    {
        ITS = 0,
        Laser = 1

    }

    public class ConvertRequestLaser : BaseRequeLaser
    {
        

        private List<IPair> mLaser = null;

        public ConvertRequestLaser(RequestLaser response)
        {

			var v = response;
			if (Requited = v != null)
			{
				foreach (Point2D point in response.Response)
				{
					if (mLaser == null) mLaser = new List<IPair>();
					mLaser.Add(new Pair(point.X, point.X));
				}
				Count = mLaser.Count;
			}

		}

        public override void DrawLaser()
        {
            GLCMD.CMD.SaftyEditMultiGeometry<IPair>((int)EmID.Laser, true, laser =>
            {
                laser.Clear();
                laser.AddRange(mLaser);
            });
        }
    }


	/// <summary>
	/// 要求Map档清单
	/// </summary>
	public class ConvertRequestMapList : BaseRequestMapList
	{
		public ConvertRequestMapList(RequestMapList response)
		{
			var v = response;
			if (Requited = v != null)
			{
				var list = v.Response;
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
		private FileInfo mDoc = null;

		public RequestMapFile(GetMap response)
		{
			var v = response;
			if (Requited = v != null)
			{
				mDoc = v.Response;
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

	public class ConvertDoPositionComfirm : BaseDoPositionComfirm
	{
		public ConvertDoPositionComfirm(DoPositionComfirm response)
		{
			var v = response;
			if (Requited = v != null)
			{
				Similarity = v.Response.Value;
			}
		}
	}

	/// <summary>
	/// 上傳Map檔
	/// </summary>
	public class ConvertUploadMapToAGV : BaseUploadMapToAGV
	{
		public ConvertUploadMapToAGV(UploadMapToAGV response)
		{
			var v = response;
			if (Requited = v != null)
			{
				Value = v.Response;
			}
		}
	}

	/// <summary>
	/// 要求iTS載入指定的Map檔
	/// </summary>
	public class ConvertChangeMap : BaseChangeMap
	{
		public ConvertChangeMap(ChangeMap response)
		{
			var v = response;
			if (Requited = v != null)
			{
				Value = v.Response;
			}
		}
	}

	/// <summary>
	/// 設定地圖檔名
	/// </summary>
	/// <remarks>是否在掃描中</remarks>
	public class ConvertSetScanningOriFileName : BaseSetScanningOriFileName
	{
		public ConvertSetScanningOriFileName(SetScanningOriFileName response)
		{
			var v = response;
			if (Requited = v != null)
			{
				Value = v.Response;
			}
		}
	}

	/// <summary>
	/// 停止掃描地圖
	/// </summary>
	public class ConvertStopScanning : BaseStopScanning
	{
		public ConvertStopScanning(StopScanning response)
		{
			var v = response;
			if (Requited = v != null)
			{
				Value = v.Response;
			}
		}
	}

	/// <summary>
	/// 開始手動控制
	/// </summary>
	public class ConvertStartManualControl : BaseStartManualControl
	{
		public ConvertStartManualControl(StartManualControl response)
		{
			var v = response;
			if (Requited = v != null)
			{
				Value = v.Response;
			}
		}
	}

	/// <summary>
	/// 設定手動控制移動速度(方向)
	/// </summary>
	public class ConvertSetManualVelocity : BaseSetManualVelocity
	{
		public ConvertSetManualVelocity(SetManualVelocity response)
		{
			var v = response;
			if (Requited = v != null)
			{
				Value = v.Response;
			}
		}
	}

	/// <summary>
	/// 要求到目标点的路径
	/// </summary>
	public class ConvertRequestPath : BaseRequestPath
	{
		public ConvertRequestPath(RequestPath response)
		{
			var v = response;
			if (Requited = v != null)
			{
				Count = v.Response.Count;
			}
		}
	}

	public class ConvertGoTo : BaseGoTo
	{
		public ConvertGoTo(GoTo response)
		{
			Requited = false;
			if (response != null)
			{
				var v = response;
				Started = v.Response;
			}
		}
	}
}
