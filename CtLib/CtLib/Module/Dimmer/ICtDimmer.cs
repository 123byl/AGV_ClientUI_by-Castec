using System;
using System.Collections.Generic;

using CtLib.Library;

namespace CtLib.Module.Dimmer {

	/// <summary>調光器介面</summary>
	public interface ICtDimmer : IDisposable {

		#region Connections

		/// <summary>以使用者介面方式供人員手動連線</summary>
		/// <returns>Status Code</returns>
		Stat Connect();

		/// <summary>已指定的串列埠編號及預設參數進行連線</summary>
		/// <param name="comPort">串列埠編號，如 "COM3"</param>
		/// <returns>Status Code</returns>
		Stat Connect(string comPort);

		/// <summary>中斷連線</summary>
		/// <returns>Status Code</returns>
		Stat Disconnect();

		#endregion

		#region Set Lights

		/// <summary>設定單一組別之電流數值</summary>
		/// <param name="channel">調光器組別(通道、頻道)</param>
		/// <param name="lightValue">電流數值，單位為 毫安培(mA)</param>
		/// <returns>(<see langword="true"/>)設定成功 (<see langword="false"/>)設定失敗</returns>
		bool SetLight(Channels channel, int lightValue);

		/// <summary>設定所有組別之電流數值</summary>
		/// <param name="ch1Value">第 1 組數值，單位為 毫安培(mA)</param>
		/// <param name="ch2Value">第 2 組數值，單位為 毫安培(mA)</param>
		/// <param name="ch3Value">第 3 組數值，單位為 毫安培(mA)</param>
		/// <param name="ch4Value">第 4 組數值，單位為 毫安培(mA)</param>
		/// <returns>(<see langword="true"/>)設定成功 (<see langword="false"/>)設定失敗</returns>
		bool SetLight(int ch1Value, int ch2Value, int ch3Value, int ch4Value);

		/// <summary>設定所有組別之電流數值</summary>
		/// <param name="ch1Value">第 1 組數值，單位為 毫安培(mA)</param>
		/// <param name="ch2Value">第 2 組數值，單位為 毫安培(mA)</param>
		/// <param name="ch3Value">第 3 組數值，單位為 毫安培(mA)</param>
		/// <param name="ch4Value">第 4 組數值，單位為 毫安培(mA)</param>
		/// <param name="ch5Value">第 5 組數值，單位為 毫安培(mA)</param>
		/// <returns>(<see langword="true"/>)設定成功 (<see langword="false"/>)設定失敗</returns>
		bool SetLight(int ch1Value, int ch2Value, int ch3Value, int ch4Value, int ch5Value);

		/// <summary>設定所有組別之電流數值</summary>
		/// <param name="values">數值集合</param>
		/// <returns>(<see langword="true"/>)設定成功 (<see langword="false"/>)設定失敗</returns>
		bool SetLight(IEnumerable<int> values);
		#endregion

		#region Get Lights

		/// <summary>取得當前特定組別之電流數值</summary>
		/// <param name="channel">調光器組別(通道、頻道)</param>
		/// <param name="value">當前數值</param>
		/// <returns>Status Code</returns>
		Stat GetLight(Channels channel, out LightValue value);

		/// <summary>取得當前所有的電流數值</summary>
		/// <param name="value">當前四組電流數值</param>
		/// <returns>Status Code</returns>
		Stat GetLight(out List<LightValue> value);

		#endregion
	}

	/// <summary>提供建立相關 <see cref="ICtDimmer"/> 之方法</summary>
	public abstract class CtDimmerBase {

		#region Public Operations
		/// <summary>依照 <see cref="DimmerBrand"/> 產生相對應的 <see cref="ICtDimmer"/></summary>
		/// <param name="brand">欲產生的調光器品牌</param>
		/// <returns>調光器物件</returns>
		public static ICtDimmer Factory(DimmerBrand brand) {
			ICtDimmer dimmer = null;
			switch (brand) {
				case DimmerBrand.QUAN_DA:
					dimmer = new CtDimmer();
					break;
				case DimmerBrand.HJ_CH5I700RS232:
					dimmer = new CtDimmer_CH5I700RS232();
					break;
				case DimmerBrand.HJ_CH4I700RS485:
					dimmer = new CtDimmer_CH4I700RS485();
					break;
				default:
					throw new System.ComponentModel.InvalidEnumArgumentException("Brand", (int)brand, typeof(DimmerBrand));
			}
			return dimmer;
		}
		#endregion
	}
}
