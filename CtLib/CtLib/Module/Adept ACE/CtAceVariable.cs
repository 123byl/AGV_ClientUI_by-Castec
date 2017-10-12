using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using CtLib.Library;
using CtLib.Module.Adept.Extension;

using Ace.Adept.Server.Controls;
using Ace.Adept.Server.Desktop;
using Ace.Core.Server;
using Ace.Core.Server.Program;

namespace CtLib.Module.Adept {
	/// <summary>
	/// Adept ACE 之相關變數控制
	/// <para>包含 V+ (Global)、Numeric、String 等，提供讀取與寫入等相關方法</para>
	/// </summary>
	[Serializable]
	public sealed class CtAceVariable {

		#region Declaration - Fields
		/// <summary>AceServer，用於取得內部物件，如 Numeric、String 等物件</summary>
		private IAceServer mISrv;
		/// <summary>V+ 連結器，可能為 SmartController 或 iCobra 等物件</summary>
		private VpObjects mVpObj;
		#endregion

		#region Function - Constructor
		/// <summary>建立一與 Adept ACE Variable 相關控制關聯之子模組</summary>
		/// <param name="links">V+ 連結器</param>
		/// <param name="aceSrv">AceServer，用於取得內部物件，如 Numeric、String 等物件</param>
		internal CtAceVariable(IAceServer aceSrv, VpObjects links) {
			mISrv = aceSrv;
			mVpObj = links;
		}
		#endregion

		#region Function - Methods
		/// <summary>取得 <see cref="IAceServer"/> 內部物件</summary>
		/// <param name="path">欲取得物件之路徑，如 "Folder/Numeric Variable 1"</param>
		/// <returns>回傳之路徑</returns>
		public object FindObject(string path) {
			object objTemp = null;
			if (!string.IsNullOrEmpty(path) && mISrv != null) objTemp = mISrv.Root[path];
			return objTemp;
		}

		/// <summary>取得特定的 <see cref="IVPlusGlobalVariable"/>，需搭配 Adept ACE 3.4 (含)以上版本</summary>
		/// <param name="name">欲取得的變數名稱</param>
		/// <returns>相對應的 <see cref="IVPlusGlobalVariable"/></returns>
		private IVPlusGlobalVariable GetVpGlobVar(string name) {
			IVPlusGlobalVariable vpVar = null;
			if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("VpVarName", "錯誤或為空的 V+ 變數名稱");
			string vpName = string.Empty;
			int idx = 0;
			do {
				switch (idx) {
					case 0:
						vpName = name;
						break;
					case 1:
						vpName = string.Format("${0}", name);
						break;
					case 2:
						vpName = string.Format("#{0}", name);
						break;
				}
				vpVar = mVpObj.Var(obj => obj.GetVariableByName(vpName));
				idx++;
			} while (vpVar == null && idx < 3);
			return vpVar;
		}
		#endregion

		#region Function - Write Variable

		/// <summary>
		/// 寫入未知型態的 V+ 全域變數，會自動判斷該變數之型態並進行轉型
		/// <para>僅支援 Adept ACE 3.4 以上版本!</para>
		/// </summary>
		/// <param name="name">變數名稱</param>
		/// <param name="value">
		/// 帶入之引數數值，各類型所需引數型態如下
		/// <para>Location       - List(Of double)、double[]或string</para>
		/// <para>PrecisionPoint - List(Of double)、double[]或string</para>
		/// <para>Real           - float或string</para>
		/// <para>String         - string</para>
		/// </param>
		public void SetUndefVpValue(string name, object value) {
			if (string.IsNullOrEmpty(name)) throw (new Exception("錯誤的變數名稱"));

			IVPlusGlobalVariable vpGlobVar = GetVpGlobVar(name);
			if (vpGlobVar != null) {
				switch (vpGlobVar.VariableType) {
					case VPlusGlobalVariableType.Location:
					case VPlusGlobalVariableType.PrecisionPoint:
						if (value is string) {
							double[] valColl = value
												.ToString()
												.Split(CtConst.CHR_DELIMITERS, StringSplitOptions.RemoveEmptyEntries)
												.ConvertAll(val => double.Parse(val));
							SetValue(vpGlobVar.Name, valColl, (VPlusVariableType)vpGlobVar.VariableType);
						} else {
							IEnumerable<double> loc = value as IEnumerable<double>;
							if (loc == null) throw (new Exception("未知的引數型態"));
							SetValue(vpGlobVar.Name, loc, (VPlusVariableType)vpGlobVar.VariableType);
						}
						break;
					case VPlusGlobalVariableType.Real:
						float sngTemp = Convert.ToSingle(value);
						mVpObj.Link(link => link.SetR(vpGlobVar.Name, sngTemp));
						break;
					case VPlusGlobalVariableType.String:
						mVpObj.Link(link => link.SetS(vpGlobVar.Name, value.ToString()));
						break;
				}
			} else throw (new Exception("無法查詢到變數 " + name + "，請查明後再試"));
		}

		/// <summary>更改 V+ String 變數數值</summary>
		/// <param name="name">變數名稱，如 "alarm.msg"</param>
		/// <param name="value">欲更改之數值，如 "Taking picture failed"</param>
		public void SetValue(string name, string value) {
			mVpObj.Link(link => link.SetS(name, value));
		}

		/// <summary>更改 V+ Real 變數數值</summary>
		/// <param name="name">變數名稱，如 "alarm.code"</param>
		/// <param name="value">欲更改之實數數值，如 "-1000"</param>
		public void SetValue(string name, float value) {
			mVpObj.Link(link => link.SetR(name, value));
		}

		/// <summary>更改 V+ Location 或 PrecisionPoint 變數數值</summary>
		/// <param name="name">變數名稱，如 "loc.w"、"#loc.j"</param>
		/// <param name="value">欲更改之數值</param>
		/// <param name="vpVarType">變數類型，可為 Joints(PrecisionPoint) 或 Location(Transform3D)</param>
		public void SetValue(string name, IEnumerable<double> value, VPlusVariableType vpVarType = VPlusVariableType.Location) {
			switch (vpVarType) {
				case VPlusVariableType.PrecisionPoint:
					PrecisionPoint pp = value.ToPrecisionPoint();
					mVpObj.Link(link => link.SetPP(name, pp));
					break;

				case VPlusVariableType.Location:
					Transform3D loc = value.ToTransform3D();
					mVpObj.Link(link => link.SetL(name, loc));
					break;

				default:
					throw (new Exception("僅支援 PrecisionPoint 或 Location 之 V+ 變數"));
			}
		}

		/// <summary>寫入當前機器手臂位置至 V+ 全域變數</summary>
		/// <param name="varName">V+ 變數名稱，若為 Precision Point 請注意加上 "#" 字號。如 "wloc"、"#jloc"</param>
		/// <param name="robNum">機器手臂編號</param>
		public void SetHere(string varName, int robNum = 1) {
			if (varName.StartsWith("#")) {
				mVpObj.Link(
					link => {
						PrecisionPoint pp = link.WhereJoint(robNum);
						link.SetPP(varName, pp);
					}
				);
			} else {
				mVpObj.Link(
					link => {
						Transform3D loc = link.WhereWorld(robNum);
						link.SetL(varName, loc);
					}
				);
			}
		}

		/// <summary>寫入當前機器手臂位置至 V+ 全域變數，但將 Z 軸設定於特定高度</summary>
		/// <param name="varName">變數名稱，若為 Precision Point 請注意加上 "#" 字號。如 "wloc"、"#jloc"</param>
		/// <param name="z">欲指定的 Z 軸高度</param>
		/// <param name="robNum">機器手臂編號</param>
		public void SetHere(string varName, float z, int robNum = 1) {
			if (!varName.StartsWith("#")) {
				mVpObj.Link(
					link => {
						double[] decompose = link.WhereWorld(robNum).ToArray();
						decompose[2] = z;
						Transform3D loc = decompose.ToTransform3D();
						link.SetL(varName, loc);
					}
				);
			} else {
				mVpObj.Link(
					link => {
						PrecisionPoint pp = link.WhereJoint(robNum);
						pp.J3 = z;
						link.SetPP(varName, pp);
					}
				);
			}
		}

		/// <summary>寫入當前機器手臂位置至 V+ 全域變數，但可特別指定 Z 軸高度與 θ 數值</summary>
		/// <param name="varName">變數名稱，若為 Precision Point 請注意加上 "#" 字號。如 "wloc"、"#jloc"</param>
		/// <param name="z">欲指定的Z軸高度</param>
		/// <param name="degree">欲指定的角度</param>
		/// <param name="robNum">機器手臂編號</param>
		public void SetHere(string varName, float z, float degree, int robNum = 1) {
			if (!varName.StartsWith("#")) {
				mVpObj.Link(
					link => {
						double[] decompose = link.WhereWorld(robNum).ToArray();
						decompose[2] = z;
						decompose[5] = degree;
						Transform3D loc = decompose.ToTransform3D();
						link.SetL(varName, loc);
					}
				);
			} else {
				mVpObj.Link(
					link => {
						PrecisionPoint pp = link.WhereJoint(robNum);
						pp.SetJoint(2, z);  //[0..5]J1~J6
						pp.SetJoint(pp.Length - 1, degree);
						link.SetPP(varName, pp);
					}
				);
			}
		}

		/// <summary>更改 Numeric Variable 數值 (非 V+ 全域變數)</summary>
		/// <param name="path">物件路徑，如 "Program/Variables/Numeric Variable 3"</param>
		/// <param name="value">欲寫入的數值</param>
		public void SetNumericValue(string path, double value) {
			IVariableNumeric numVar = FindObject(path) as IVariableNumeric;
			if (numVar != null) numVar.CurrentValue = value;
			else throw new InvalidOperationException(string.Format("不正確的路徑 : \"{0}\"", path));
		}

		/// <summary>更改 String Variable 數值 (非 V+ 全域變數)</summary>
		/// <param name="path">物件路徑，如 "Program/Variables/String Variable 1"</param>
		/// <param name="value">欲寫入的數值</param>
		public void SetStringValue(string path, string value) {
			IVariableString strVar = FindObject(path) as IVariableString;
			if (strVar != null) strVar.CurrentValue = value;
			else throw new InvalidOperationException(string.Format("不正確的路徑 : \"{0}\"", path));
		}

		/// <summary>寫入未知路徑的 Nnumeric Variable，以搜尋到的第一個符合項目為準</summary>
		/// <param name="name">物件名稱，如 "Numeric Variable 3"</param>
		/// <param name="value">欲寫入的數值</param>
		public void SetUndefNumericValue(string name, double value) {
			IVariableNumeric numVar = mISrv
										.Root
										.FilterType(typeof(IVariableNumeric), true)
										.FirstOrDefault(obj => obj.Name == name)
										as IVariableNumeric;
			if (numVar != null) numVar.CurrentValue = value;
			else throw new ArgumentNullException("ObjectName", string.Format("無法找到對應的 Numeric Variable : \"{0}\"", name));
		}

		/// <summary>寫入未知路徑的 String Variable，以搜尋到的第一個符合項目為準</summary>
		/// <param name="name">物件名稱，如 "String Variable 1"</param>
		/// <param name="value">欲寫入的數值</param>
		public void SetUndefStringValue(string name, string value) {
			IVariableString strVar = mISrv
										.Root
										.FilterType(typeof(IVariableString), true)
										.FirstOrDefault(obj => obj.Name == name)
										as IVariableString;
			if (strVar != null) strVar.CurrentValue = value;
			else throw new ArgumentNullException("ObjectName", string.Format("無法找到對應的 String Variable : \"{0}\"", name));
		}
		#endregion

		#region Function - Read Variable

		/// <summary>取得特定 V+ 變數之型態</summary>
		/// <param name="name">欲取得之變數名稱</param>
		/// <param name="varType">回傳之變數型態</param>
		public void GetVariabeType(string name, out VPlusVariableType varType) {
			IVPlusGlobalVariable vpGlobVar = GetVpGlobVar(name);
			if (vpGlobVar != null) varType = (VPlusVariableType)vpGlobVar.VariableType;
			else throw new ArgumentException(string.Format("不正確的 V+ 變數名稱 : \"{0}\"", name));
		}

		/// <summary>取得特定 V+ 變數之型態</summary>
		/// <param name="name">欲取得之變數名稱</param>
		/// <returns>變數型態</returns>
		public VPlusVariableType GetVariabeType(string name) {
			IVPlusGlobalVariable vpGlobVar = GetVpGlobVar(name);
			if (vpGlobVar != null) return (VPlusVariableType)vpGlobVar.VariableType;
			else throw new ArgumentException(string.Format("不正確的 V+ 變數名稱 : \"{0}\"", name));
		}

		/// <summary>
		/// 讀取未知型態的 V+ 全域變數，會自動判斷該變數之型態並進行轉型
		/// <para>僅支援 Adept ACE 3.4 以上版本!</para>
		/// </summary>
		/// <param name="name">變數名稱</param>
		/// <param name="value">
		/// 當前數值，各類型所回傳型態如下
		/// <para>Location       - List(Of double)</para>
		/// <para>PrecisionPoint - List(Of double)</para>
		/// <para>Real           - float</para>
		/// <para>String         - string</para>
		/// </param>
		public void GetUndefVpValue(string name, out object value) {
			IVPlusGlobalVariable vpGlobVar = GetVpGlobVar(name);
			if (vpGlobVar != null) {
				switch (vpGlobVar.VariableType) {
					case VPlusGlobalVariableType.Real:
						float realVal = float.MinValue;
						mVpObj.Var(obj => obj.ReadValue(vpGlobVar, out realVal));
						value = realVal;
						break;
					case VPlusGlobalVariableType.String:
						string strVal = null;
						mVpObj.Var(obj => obj.ReadValue(vpGlobVar, out strVal));
						value = strVal;
						break;
					case VPlusGlobalVariableType.Location:
						Transform3D loc = null;
						mVpObj.Var(obj => obj.ReadValue(vpGlobVar, out loc));
						value = loc?.ToList();
						break;
					case VPlusGlobalVariableType.PrecisionPoint:
						double[] pp = null;
						mVpObj.Var(obj => obj.ReadValue(vpGlobVar, out pp));
						value = pp;
						break;
					default:
						throw new InvalidEnumArgumentException("VpGlobType", (int)vpGlobVar.VariableType, typeof(VPlusGlobalVariableType));
				}
			} else throw new ArgumentException(string.Format("不正確的 V+ 變數名稱 : \"{0}\"", name));
		}

		/// <summary>取得 V+ String 變數之數值</summary>
		/// <param name="name">變數名稱，如 "$alarm.msg"</param>
		/// <param name="value">當前數值</param>
		public void GetValue(string name, out string value) {
			if (!name.StartsWith("$")) value = mVpObj.Link(link => link.ListS("$" + name));
			else value = mVpObj.Link(link => link.ListS(name));
		}

		/// <summary>取得 V+ Real 變數之數值</summary>
		/// <param name="name">變數名稱，如 "alarm.code"</param>
		/// <param name="value">當前數值</param>
		public void GetValue(string name, out float value) {
			value = (float)mVpObj.Link(link => link.ListR(name));
		}

		/// <summary>取得 V+ Location 或 PrecisionPoint 變數之數值</summary>
		/// <param name="name">變數名稱，如 "loc_world", "#loc_joint"</param>
		/// <param name="value">當前數值</param>
		/// <param name="locType">變數類型，可為 Joints(PrecisionPoint) 或 Location(Transform3D)</param>
		public void GetValue(string name, out double[] value, VPlusVariableType locType = VPlusVariableType.Location) {
			double[] locAry = null;

			switch (locType) {
				case VPlusVariableType.PrecisionPoint:
					locAry = mVpObj.Link(link => link.ListPP(name)).ToArray();
					break;
				case VPlusVariableType.Location:
					locAry = mVpObj.Link(link => link.ListL(name)).ToArray();
					break;
				default:
					throw new InvalidEnumArgumentException("VpGlobType", (int)locType, typeof(VPlusVariableType));
			}

			value = locAry;
		}

		/// <summary>取得 V+ Location 或 PrecisionPoint 變數之數值</summary>
		/// <param name="name">變數名稱，如 "loc_world", "#loc_joint"</param>
		/// <param name="value">當前數值</param>
		/// <param name="locType">變數類型，可為 Joints(PrecisionPoint) 或 Location(Transform3D)</param>
		public void GetValue(string name, out List<double> value, VPlusVariableType locType = VPlusVariableType.Location) {
			List<double> locList = null;

			switch (locType) {
				case VPlusVariableType.PrecisionPoint:
					locList = mVpObj.Link(link => link.ListPP(name)).ToList();
					break;
				case VPlusVariableType.Location:
					locList = mVpObj.Link(link => link.ListL(name)).ToList();
					break;
				default:
					throw new InvalidEnumArgumentException("VpGlobType", (int)locType, typeof(VPlusVariableType));
			}

			value = locList;
		}

		/// <summary>取得 Numeric Variable 數值 (非 V+ 全域變數)</summary>
		/// <param name="path">物件路徑，如 "Program/Variables/Numeric Variable 3"</param>
		/// <param name="value">當前數值</param>
		public void GetNumericValue(string path, out double value) {
			IVariableNumeric numVar = GetVpGlobVar(path) as IVariableNumeric;
			if (numVar != null) value = numVar.CurrentValue;
			else throw new InvalidOperationException(string.Format("不正確的路徑 : \"{0}\"", path));
		}

		/// <summary>取得 Numeric Variable 數值 (非 V+ 全域變數)</summary>
		/// <param name="path">物件路徑，如 "Program/Variables/Numeric Variable 3"</param>
		/// <param name="value">當前數值</param>
		public void GetStringValue(string path, out string value) {
			IVariableString numVar = GetVpGlobVar(path) as IVariableString;
			if (numVar != null) value = numVar.CurrentValue;
			else throw new InvalidOperationException(string.Format("不正確的路徑 : \"{0}\"", path));
		}

		/// <summary>取得未知路徑的 Nnumeric Variable 當前數值，以搜尋到的第一個符合項目為準</summary>
		/// <param name="name">物件名稱，如 "Numeric Variable 3"</param>
		/// <param name="value">當前數值</param>
		public void GetUndefNumericValue(string name, out double value) {
			IVariableNumeric numVar = mISrv
										.Root
										.FilterType(typeof(IVariableNumeric), true)
										.FirstOrDefault(obj => obj.Name == name)
										as IVariableNumeric;
			if (numVar != null) value = numVar.CurrentValue;
			else throw new ArgumentNullException("ObjectName", string.Format("無法找到對應的 Numeric Variable : \"{0}\"", name));
		}

		/// <summary>取得未知路徑的 String Variable 當前數值，以搜尋到的第一個符合項目為準</summary>
		/// <param name="name">物件名稱，如 "String Variable 1"</param>
		/// <param name="value">當前數值</param>
		public void GetUndefStringValue(string name, out string value) {
			IVariableString strVar = mISrv
										.Root
										.FilterType(typeof(IVariableString), true)
										.FirstOrDefault(obj => obj.Name == name)
										as IVariableString;
			if (strVar != null) value = strVar.CurrentValue;
			else throw new ArgumentNullException("ObjectName", string.Format("無法找到對應的 String Variable : \"{0}\"", name));
		}

		/// <summary>取得 V+ 變數數值，並直接回傳</summary>
		/// <param name="name">變數名稱</param>
		/// <param name="varTyp">變數型態</param>
		/// <returns>
		/// 回傳之變數數值(Current Value)，各類型回傳如下
		/// <para>Location: 回傳List(Of Double)</para>
		/// <para>PrecisionPoint: 回傳List(Of Double)</para>
		/// <para>Real: 回傳float</para>
		/// <para>String: 回傳string</para>
		/// </returns>
		public object GetValue(string name, VPlusVariableType varTyp) {
			object objTemp = null;

			switch (varTyp) {
				case VPlusVariableType.Real:
					objTemp = (float)mVpObj.Link(link => link.ListR(name));
					break;
				case VPlusVariableType.String:
					objTemp = mVpObj.Link(link => link.ListS(name));
					break;
				case VPlusVariableType.PrecisionPoint:
					objTemp = mVpObj.Link(link => link.ListPP(name)).ToList();
					break;
				case VPlusVariableType.Location:
					objTemp = mVpObj.Link(link => link.ListL(name)).ToList();
					break;
				default:
					throw new InvalidEnumArgumentException("VpGlobType", (int)varTyp, typeof(VPlusVariableType));
			}

			return objTemp;
		}

		/// <summary>取得當前機器手臂座標</summary>
		/// <param name="robNum">手臂編號。預設為 1 開始</param>
		/// <param name="location">當前座標</param>
		/// <param name="varType">欲取得之型態，Location 或 PrecisionPoint</param>
		public void GetHere(int robNum, out List<double> location, VPlusVariableType varType = VPlusVariableType.Location) {
			List<double> locTemp = null;

			if (varType == VPlusVariableType.Location) {
				locTemp = mVpObj.Link(link => link.WhereWorld(robNum)).ToList();
			} else if (varType == VPlusVariableType.PrecisionPoint) {
				locTemp = mVpObj.Link(link => link.WhereJoint(robNum)).ToList();
			} else throw (new Exception("目前僅支援 Location 與 PrecisionPoint"));

			location = locTemp;
		}
		#endregion

	}
}
