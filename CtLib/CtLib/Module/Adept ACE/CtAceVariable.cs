using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CtLib.Library;

using Ace.Adept.Server.Controls;
using Ace.Adept.Server.Desktop;
using Ace.Adept.Server.Desktop.Connection;
using Ace.Core.Server;
using Ace.Core.Server.Program;

namespace CtLib.Module.Adept {
    /// <summary>
    /// Adept ACE 之相關變數控制
    /// <para>包含 V+ (Global)、Numeric、String 等，提供讀取與寫入等相關方法</para>
    /// </summary>
    public class CtAceVariable {

        #region Declaration - Members
        /// <summary>V+ 相關連結，用於讀寫 V+ Global 變數</summary>
        private IVpLink mVpLink;
        /// <summary>SmartController物件，含有控制器的情況下可使用特定方法直接存取控制器內部之變數</summary>
        private IAdeptController mICtrl;
        /// <summary>AceServer，用於取得內部物件，如 Numeric、String 等物件</summary>
        private IAceServer mISrv;
        #endregion

        #region Function - Constructor
        /// <summary>建立一與 Adept ACE Variable 相關控制關聯之子模組</summary>
        /// <param name="vpLink">V+ 相關連結，用於讀寫 V+ Global 變數</param>
        /// <param name="aceCtrl">SmartController物件，含有控制器的情況下可使用特定方法直接存取控制器內部之變數</param>
        /// <param name="aceSrv">AceServer，用於取得內部物件，如 Numeric、String 等物件</param>
        public CtAceVariable(IVpLink vpLink, IAdeptController aceCtrl, IAceServer aceSrv) {
            mVpLink = vpLink;
            mICtrl = aceCtrl;
            mISrv = aceSrv;
        }
        #endregion

        #region Function - Methods
        /// <summary>取得ACE內部物件(IAceServer.Root)</summary>
        /// <param name="path">欲取得物件之路徑</param>
        /// <returns>回傳之路徑</returns>
        public object FindObject(string path) {
            object objTemp = null;
            if ((path != "") && (mISrv != null)) objTemp = mISrv.Root[path];
            return objTemp;
        }
        #endregion

        #region Function - Write Variable

        /// <summary>
        /// 設定特定變數名稱或路徑之數值，V+ Variable 可不指定型態直接寫入數值
        /// <para>此部分需要含有 SmartController</para>
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
        /// <param name="varType">變數類型。請參考 <see cref="CtAce.VariableType"/></param>
        /// <remarks>此部分僅支援 Adept ACE 3.4 以上版本!</remarks>
        public void SetValue(string name, object value, CtAce.VariableType varType = CtAce.VariableType.VPLUS_VARIABLE) {
            byte count = 0;
            string strName = name;
            if (name == "") throw (new Exception("錯誤的變數名稱"));

            switch (varType) {
                case CtAce.VariableType.VPLUS_VARIABLE:
                    IVPlusGlobalVariable var = null;
                    do {
                        switch (count) {
                            case 1:
                                strName = "$" + name;
                                break;
                            case 2:
                                strName = "#" + name;
                                break;
                        }
                       // var = mICtrl.Memory.Variables.GetVariableByName(strName);
                        count++;
                    } while ((var == null) && (count < 3));

                    if (var != null) {
                        switch (var.VariableType) {
                            case VPlusGlobalVariableType.Location:
                                if (value.GetType() == typeof(string)) {
                                    List<string> strSplit = value.ToString().Split(CtConst.CHR_DELIMITERS, StringSplitOptions.RemoveEmptyEntries).ToList();
                                    SetValue(strName, strSplit.ConvertAll(new Converter<string, double>(data => CtConvert.CDbl(data))));
                                } else if (value.GetType().IsGenericType) {
                                    SetValue(strName, value as List<double>);
                                } else if (value.GetType().IsArray) {
                                    SetValue(strName, value as double[]);
                                } else throw (new Exception("未知的引數型態"));
                                break;
                            case VPlusGlobalVariableType.PrecisionPoint:
                                if (value.GetType() == typeof(string)) {
                                    List<string> strSplit = value.ToString().Split(CtConst.CHR_DELIMITERS, StringSplitOptions.RemoveEmptyEntries).ToList();
                                    SetValue(strName, strSplit.ConvertAll(new Converter<string, double>(data => CtConvert.CDbl(data))), CtAce.VPlusVariableType.PRECISION_POINT);
                                } else if (value.GetType().IsGenericType) {
                                    SetValue(strName, value as List<double>, CtAce.VPlusVariableType.PRECISION_POINT);
                                } else if (value.GetType().IsArray) {
                                    SetValue(strName, value as double[], CtAce.VPlusVariableType.PRECISION_POINT);
                                } else throw (new Exception("未知的引數型態"));
                                break;
                            case VPlusGlobalVariableType.Real:
                                float sngTemp = -9999.99F;
                                if (float.TryParse(value.ToString(), out sngTemp)) SetValue(strName, sngTemp);
                                else throw (new Exception("未知的引數型態"));
                                break;
                            case VPlusGlobalVariableType.String:
                                SetValue(strName, value.ToString());
                                break;
                        }
                    } else throw (new Exception("無法查詢到變數 " + name + "，請查明後再試"));

                    break;

                case CtAce.VariableType.NUMERIC_VARIABLE:
                    IVariableNumeric numeric = FindObject(name) as IVariableNumeric;
                    numeric.CurrentValue = CtConvert.CDbl(value);
                    break;

                case CtAce.VariableType.STRING_VARIABLE:
                    IVariableString str = FindObject(name) as IVariableString;
                    str.CurrentValue = value.ToString();
                    break;
            }

        }

        /// <summary>更改變數數值，型態為 String</summary>
        /// <param name="name">
        /// 變數名稱或路徑
        /// <para>V+ Variable - 請帶入名稱，如 "robot.alarm"</para>
        /// <para>Numeric/String Variable - 請帶入路徑，如 @"/Vairble/Delete Image"</para>
        /// </param>
        /// <param name="value">欲更改之數值</param>
        /// <param name="varType">變數種類，如 V+ Variable、Numeric Variable、String Variable</param>
        public void SetValue(string name, string value, CtAce.VariableType varType = CtAce.VariableType.VPLUS_VARIABLE) {
            switch (varType) {
                case CtAce.VariableType.VPLUS_VARIABLE:
                    lock (mVpLink) {
                        mVpLink.SetS(name, value);
                    }
                    break;

                case CtAce.VariableType.NUMERIC_VARIABLE:
                    IVariableNumeric numeric = FindObject(name) as IVariableNumeric;
                    numeric.CurrentValue = CtConvert.CDbl(value);
                    break;

                case CtAce.VariableType.STRING_VARIABLE:
                    IVariableString str = FindObject(name) as IVariableString;
                    str.CurrentValue = value;
                    break;
            }
        }

        /// <summary>更改變數數值，型態為 Real(Double/Float) </summary>
        /// <param name="name">
        /// 變數名稱或路徑
        /// <para>V+ Variable - 請帶入名稱，如 "robot.alarm"</para>
        /// <para>Numeric/String Variable - 請帶入路徑，如 @"/Vairble/Delete Image"</para>
        /// </param>
        /// <param name="value">欲更改之數值</param>
        /// <param name="varType">變數種類，如 V+ Variable、Numeric Variable、String Variable</param>
        public void SetValue(string name, float value, CtAce.VariableType varType = CtAce.VariableType.VPLUS_VARIABLE) {
            switch (varType) {
                case CtAce.VariableType.VPLUS_VARIABLE:
                    lock (mVpLink) {
                        mVpLink.SetR(name, value);
                    }
                    break;
                case CtAce.VariableType.NUMERIC_VARIABLE:
                    IVariableNumeric numeric = FindObject(name) as IVariableNumeric;
                    numeric.CurrentValue = value;
                    break;

                case CtAce.VariableType.STRING_VARIABLE:
                    IVariableString str = FindObject(name) as IVariableString;
                    str.CurrentValue = value.ToString();
                    break;
            }
        }

        /// <summary>更改變數數值，型態為 Joint/Location (PrecisionPoint/Transform3D)</summary>
        /// <param name="name">變數名稱</param>
        /// <param name="value">欲更改之數值</param>
        /// <param name="locTyp">變數類型，可為Joints(PrecisionPoint)或Location(Transform3D)</param>
        public void SetValue(string name, double[] value, CtAce.VPlusVariableType locTyp = CtAce.VPlusVariableType.LOCATION) {
            /*-- 雖Transform3D有dx/dy/dz的多載，但目前為了不要混淆，還是以常見的6座標系建立Transform3D (X Y Z rX/Yaw rY/Pitch rZ/Roll/Theta) 
                 Joint (PrecisionPoint) 部分因有四軸/六軸，故多增加判斷長度等於4的部分(6與Transform3D數量相同) --*/
            if ((value.Length != 6) && (value.Length != 4)) {
                throw (new Exception("陣列長度錯誤，僅支援轉為PrecisionPoint或Transform3D，請勿帶入其他數值"));
            } else {
                switch (locTyp) {
                    case CtAce.VPlusVariableType.PRECISION_POINT:
                        lock (mVpLink) {
                            PrecisionPoint loc = new PrecisionPoint(value);
                            mVpLink.SetPP(name, loc);
                        }
                        break;

                    case CtAce.VPlusVariableType.LOCATION:
                        lock (mVpLink) {
                            Transform3D loc = new Transform3D(value[0], value[1], value[2], value[3], value[4], value[5]);
                            mVpLink.SetL(name, loc);
                        }
                        break;

                    default:
                        throw (new Exception("SetValue僅支援寫入PrecisionPoint或Transform3D，請勿帶入其他數值"));
                }
            }
        }

        /// <summary>更改變數數值，型態為 Joint/Location (PrecisionPoint/Transform3D)</summary>
        /// <param name="name">變數名稱</param>
        /// <param name="value">欲更改之數值</param>
        /// <param name="locTyp">變數類型，可為Joints(PrecisionPoint)或Location(Transform3D)</param>
        public void SetValue(string name, List<double> value, CtAce.VPlusVariableType locTyp = CtAce.VPlusVariableType.LOCATION) {
            /*-- 雖Transform3D有dx/dy/dz的多載，但目前為了不要混淆，還是以常見的6座標系建立Transform3D (X Y Z rX/Yaw rY/Pitch rZ/Roll/Theta) 
                 Joint (PrecisionPoint) 部分因有四軸/六軸，故多增加判斷長度等於4的部分(6與Transform3D數量相同) --*/
            if ((value.Count != 6) && (value.Count != 4)) {
                throw (new Exception("陣列長度錯誤，僅支援轉為PrecisionPoint或Transform3D，請勿帶入其他數值"));
            } else {

                switch (locTyp) {
                    case CtAce.VPlusVariableType.PRECISION_POINT:
                        lock (mVpLink) {
                            PrecisionPoint loc = new PrecisionPoint(value.ToArray());
                            mVpLink.SetPP(name, loc);
                        }
                        break;
                    case CtAce.VPlusVariableType.LOCATION:
                        lock (mVpLink) {
                            Transform3D loc = new Transform3D(value[0], value[1], value[2], value[3], value[4], value[5]);
                            mVpLink.SetL(name, loc);
                        }
                        break;
                    default:
                        throw (new Exception("SetValue僅支援寫入PrecisionPoint或Transform3D，請勿帶入其他數值"));
                }
            }
        }

        /// <summary>將變數寫入當前Robot位置</summary>
        /// <param name="varName">變數名稱</param>
        /// <param name="robotNum">Robot編號</param>
        /// <param name="locTyp">變數型態，僅接受Locaroin/PrecisionPoint</param>
        public void SetHere(string varName, int robotNum = 1, CtAce.VPlusVariableType locTyp = CtAce.VPlusVariableType.LOCATION) {
            if (locTyp == CtAce.VPlusVariableType.LOCATION) {
                Transform3D loc = mVpLink.WhereWorld(robotNum);
                mVpLink.SetL(varName, loc);
            } else if (locTyp == CtAce.VPlusVariableType.PRECISION_POINT) {
                PrecisionPoint pp = mVpLink.WhereJoint(robotNum);
                mVpLink.SetPP(varName, pp);
            }
        }

        /// <summary>將變數寫入當前Robot位置，但將Z軸設定於特定高度</summary>
        /// <param name="varName">變數名稱</param>
        /// <param name="z">欲指定的Z軸高度</param>
        /// <param name="robotNum">Robot編號</param>
        /// <param name="locTyp">變數型態，僅接受Locaroin/PrecisionPoint</param>
        public void SetHere(string varName, float z, int robotNum = 1, CtAce.VPlusVariableType locTyp = CtAce.VPlusVariableType.LOCATION) {
            if (locTyp == CtAce.VPlusVariableType.LOCATION) {
                double[] decompose = mVpLink.WhereWorld(robotNum).ToArray();
                decompose[2] = z;
                mVpLink.SetL(varName, new Transform3D(decompose[0], decompose[1], decompose[2], decompose[3], decompose[4], decompose[5]));
            } else if (locTyp == CtAce.VPlusVariableType.PRECISION_POINT) {
                PrecisionPoint pp = mVpLink.WhereJoint(robotNum);
                pp.J3 = z;
                mVpLink.SetPP(varName, pp);
            }
        }

        /// <summary>將變數寫入當前Robot位置，但可特別指定Z軸高度與Theta角</summary>
        /// <param name="varName">變數名稱</param>
        /// <param name="z">欲指定的Z軸高度</param>
        /// <param name="degree">欲指定的角度</param>
        /// <param name="robotNum">Robot編號</param>
        /// <param name="locTyp">變數型態，僅接受Locaroin/PrecisionPoint</param>
        public void SetHere(string varName, float z, float degree, int robotNum = 1, CtAce.VPlusVariableType locTyp = CtAce.VPlusVariableType.LOCATION) {
            if (locTyp == CtAce.VPlusVariableType.LOCATION) {
                double[] decompose = mVpLink.WhereWorld(robotNum).ToArray();
                decompose[2] = z;
                decompose[5] = degree;
                mVpLink.SetL(varName, new Transform3D(decompose[0], decompose[1], decompose[2], decompose[3], decompose[4], decompose[5]));
            } else if (locTyp == CtAce.VPlusVariableType.PRECISION_POINT) {
                PrecisionPoint pp = mVpLink.WhereJoint(robotNum);
                pp.J3 = z;
                pp.J4 = degree;
                mVpLink.SetPP(varName, pp);
            }
        }
        #endregion

        #region Function - Read Variable

        /// <summary>取得特定 V+ 變數之型態。Numeric/String Variable 請勿使用此方法</summary>
        /// <param name="name">欲取得之變數名稱</param>
        /// <param name="varType">回傳之變數型態</param>
        public void GetVariabeType(string name, out CtAce.VPlusVariableType varType) {
            CtAce.VPlusVariableType typeTemp = CtAce.VPlusVariableType.REAL;
            string strName = name;
            byte count = 0;

            if (name == "") throw (new Exception("錯誤的變數名稱"));

            IVPlusGlobalVariable var = null;
            do {
                switch (count) {
                    case 1:
                        strName = "$" + name;
                        break;
                    case 2:
                        strName = "#" + name;
                        break;
                }
                //var = mICtrl.Memory.Variables.GetVariableByName(strName);
                count++;
            } while ((var == null) && (count < 3));

            if (var != null) {
                typeTemp = (CtAce.VPlusVariableType)var.VariableType;
            } else throw (new Exception("無法查詢到變數 " + name + "，請查明後再試"));

            varType = typeTemp;

        }

        /// <summary>取得特定 V+ 變數之型態。Numeric/String Variable 請勿使用此方法</summary>
        /// <param name="name">欲取得之變數名稱</param>
        /// <returns>變數型態</returns>
        public CtAce.VPlusVariableType GetVariabeType(string name) {
            CtAce.VPlusVariableType typeTemp = CtAce.VPlusVariableType.REAL;
            string strName = name;
            byte count = 0;

            if (name == "") throw (new Exception("錯誤的變數名稱"));

            IVPlusGlobalVariable var = null;
            do {
                switch (count) {
                    case 1:
                        strName = "$" + name;
                        break;
                    case 2:
                        strName = "#" + name;
                        break;
                }
                //var = mICtrl.Memory.Variables.GetVariableByName(strName);
                count++;
            } while ((var == null) && (count < 3));

            if (var != null) {
                typeTemp = (CtAce.VPlusVariableType)var.VariableType;
            } else throw (new Exception("無法查詢到變數 " + name + "，請查明後再試"));

            return typeTemp;

        }

        /// <summary>取得特定變數名稱之數值，V+ Variable 不指定型態並直接回傳該數值</summary>
        /// <param name="name">
        /// 變數名稱或路徑
        /// <para>V+ Variable - 請帶入名稱，如 "robot.alarm"</para>
        /// <para>Numeric/String Variable - 請帶入路徑，如 @"/Vairble/Delete Image"</para>
        /// </param>
        /// <param name="value">
        /// V+ 回傳之變數數值(Current Value)，各類型回傳如下
        /// <para>Location       - List(Of Double)</para>
        /// <para>PrecisionPoint - List(Of Double)</para>
        /// <para>Real           - float</para>
        /// <para>String         - string</para>
        /// </param>
        /// <param name="varType">此數值之型態。請參考 <see cref="CtAce.VariableType"/></param>
        public void GetValue(string name, out object value, CtAce.VariableType varType = CtAce.VariableType.VPLUS_VARIABLE) {
            object objTemp = null;
            string strName = name;
            byte count = 0;

            if (name == "") throw (new Exception("錯誤的變數名稱"));

            switch (varType) {
                case CtAce.VariableType.VPLUS_VARIABLE:

                    IVPlusGlobalVariable var = null;
                    do {
                        switch (count) {
                            case 1:
                                strName = "$" + name;
                                break;
                            case 2:
                                strName = "#" + name;
                                break;
                        }
                        //var = mICtrl.Memory.Variables.GetVariableByName(strName);
                        count++;
                    } while ((var == null) && (count < 3));

                    if (var != null) {
                        switch (var.VariableType) {
                            case VPlusGlobalVariableType.Location:
                                objTemp = GetValue(strName, CtAce.VPlusVariableType.LOCATION);
                                break;
                            case VPlusGlobalVariableType.PrecisionPoint:
                                objTemp = GetValue(strName, CtAce.VPlusVariableType.PRECISION_POINT);
                                break;
                            case VPlusGlobalVariableType.Real:
                                objTemp = GetValue(strName, CtAce.VPlusVariableType.REAL);
                                break;
                            case VPlusGlobalVariableType.String:
                                objTemp = GetValue(strName, CtAce.VPlusVariableType.STRING);
                                break;
                        }
                    } else throw (new Exception("無法查詢到變數 " + name + "，請查明後再試"));

                    break;

                case CtAce.VariableType.NUMERIC_VARIABLE:
                    IVariableNumeric numeric = FindObject(name) as IVariableNumeric;
                    objTemp = numeric.CurrentValue;
                    break;

                case CtAce.VariableType.STRING_VARIABLE:
                    IVariableString str = FindObject(name) as IVariableString;
                    objTemp = str.CurrentValue;
                    break;
            }

            value = objTemp;
        }

        /// <summary>取得當前 String 變數之數值</summary>
        /// <param name="name">
        /// 變數名稱或路徑
        /// <para>V+ Variable - 請帶入名稱，如 "$robot.alarm"</para>
        /// <para>Numeric/String Variable - 請帶入路徑，如 @"/Vairble/Delete Image"</para>
        /// </param>
        /// <param name="value">當前數值</param>
        /// <param name="varType">此數值之型態。請參考 <see cref="CtAce.VariableType"/></param>
        public void GetValue(string name, out string value, CtAce.VariableType varType = CtAce.VariableType.VPLUS_VARIABLE) {
            string strTemp = "";

            switch (varType) {
                case CtAce.VariableType.VPLUS_VARIABLE:
                    lock (mVpLink) {
                        if (!name.StartsWith("$")) strTemp = mVpLink.ListS("$" + name);
                        else strTemp = mVpLink.ListS(name);
                    }
                    break;

                case CtAce.VariableType.NUMERIC_VARIABLE:
                    IVariableNumeric numeric = FindObject(name) as IVariableNumeric;
                    strTemp = numeric.CurrentValue.ToString();
                    break;

                case CtAce.VariableType.STRING_VARIABLE:
                    IVariableString str = FindObject(name) as IVariableString;
                    strTemp = str.CurrentValue;
                    break;
            }

            value = strTemp;

        }

        /// <summary>取得當前 Real (Double/Float) 變數之數值</summary>
        /// <param name="name">
        /// 變數名稱或路徑
        /// <para>V+ Variable - 請帶入名稱，如 "robot.alarm"</para>
        /// <para>Numeric/String Variable - 請帶入路徑，如 @"/Vairble/Delete Image"</para>
        /// </param>
        /// <param name="value">當前數值</param>
        /// <param name="varType">此數值之型態。請參考 <see cref="CtAce.VariableType"/></param>
        public void GetValue(string name, out float value, CtAce.VariableType varType = CtAce.VariableType.VPLUS_VARIABLE) {
            float dblTemp = -999.99F;

            switch (varType) {
                case CtAce.VariableType.VPLUS_VARIABLE:
                    lock (mVpLink) {
                        dblTemp = (float)mVpLink.ListR(name);
                    }
                    break;

                case CtAce.VariableType.NUMERIC_VARIABLE:
                    IVariableNumeric numeric = FindObject(name) as IVariableNumeric;
                    dblTemp = (float)numeric.CurrentValue;
                    break;

                case CtAce.VariableType.STRING_VARIABLE:
                    IVariableString str = FindObject(name) as IVariableString;
                    dblTemp = CtConvert.CFloat(str.CurrentValue);
                    break;
            }

            value = dblTemp;
        }

        /// <summary>取得當前 Joint/Location 變數之數值</summary>
        /// <param name="name">變數名稱</param>
        /// <param name="value">當前數值</param>
        /// <param name="locType">變數類型，可為Joints(PrecisionPoint)或Location(Transform3D)</param>
        public void GetValue(string name, out double[] value, CtAce.VPlusVariableType locType = CtAce.VPlusVariableType.LOCATION) {
            double[] dblTemp = null;

            switch (locType) {
                case CtAce.VPlusVariableType.PRECISION_POINT:
                    lock (mVpLink) {
                        List<double> lstTemp = new List<double>();
                        PrecisionPoint loc = mVpLink.ListPP(name);
                        for (int joint = 0; joint < loc.Length; joint++) {
                            lstTemp.Add(loc.GetJoint(joint));
                        }
                        dblTemp = lstTemp.ToArray();
                    }
                    break;
                case CtAce.VPlusVariableType.LOCATION:
                    lock (mVpLink) {
                        dblTemp = mVpLink.ListL(name).ToArray();
                    }
                    break;
                default:
                    throw (new Exception("GetValue僅支援讀取PrecisionPoint或Transform3D，請勿帶入其他數值"));
            }

            value = dblTemp;
        }

        /// <summary>取得當前 Joint/Location 變數之數值</summary>
        /// <param name="name">變數名稱</param>
        /// <param name="value">當前數值</param>
        /// <param name="locType">變數類型，可為Joints(PrecisionPoint)或Location(Transform3D)</param>
        public void GetValue(string name, out List<double> value, CtAce.VPlusVariableType locType = CtAce.VPlusVariableType.LOCATION) {
            List<double> dblTemp = new List<double>();

            switch (locType) {
                case CtAce.VPlusVariableType.PRECISION_POINT:
                    lock (mVpLink) {
                        PrecisionPoint loc = mVpLink.ListPP(name);
                        for (int joint = 0; joint < loc.Length; joint++) {
                            dblTemp.Add(loc.GetJoint(joint));
                        }
                    }
                    break;
                case CtAce.VPlusVariableType.LOCATION:
                    lock (mVpLink) {
                        dblTemp = mVpLink.ListL(name).ToArray().ToList();
                    }
                    break;
                default:
                    throw (new Exception("GetValue僅支援讀取PrecisionPoint或Transform3D，請勿帶入其他數值"));
            }

            value = dblTemp;

        }

        /// <summary>取得當前變數數值，並直接回傳(Return)該數值</summary>
        /// <param name="name">變數名稱</param>
        /// <param name="varTyp">變數型態</param>
        /// <returns>
        /// 回傳之變數數值(Current Value)，各類型回傳如下
        /// <para>Location: 回傳List(Of Double)</para>
        /// <para>PrecisionPoint: 回傳List(Of Double)</para>
        /// <para>Real: 回傳float</para>
        /// <para>String: 回傳string</para>
        /// </returns>
        public object GetValue(string name, CtAce.VPlusVariableType varTyp) {
            object objTemp = null;
            try {
                lock (mVpLink) {
                    switch (varTyp) {
                        case CtAce.VPlusVariableType.REAL:
                            objTemp = (float)mVpLink.ListR(name);
                            break;
                        case CtAce.VPlusVariableType.STRING:
                            objTemp = mVpLink.ListS(name);
                            break;
                        case CtAce.VPlusVariableType.PRECISION_POINT:
                            List<double> lstTemp = new List<double>();
                            PrecisionPoint loc = mVpLink.ListPP(name);
                            for (int joint = 0; joint < loc.Length; joint++) {
                                lstTemp.Add(loc.GetJoint(joint));
                            }
                            objTemp = lstTemp;
                            break;
                        case CtAce.VPlusVariableType.LOCATION:
                            objTemp = mVpLink.ListL(name).ToArray().ToList();
                            break;
                        default:
                            throw (new Exception("請勿帶入其他變數類型"));
                    }
                }
            } catch (Exception ex) {
                CtStatus.Report(Stat.ER_SYSTEM, "GetValue", ex.Message);
            }
            return objTemp;
        }

        /// <summary>取得當前特定Robot之座標</summary>
        /// <param name="robotNum">手臂編號。預設為 1 開始</param>
        /// <param name="location">回傳之當前座標</param>
        /// <param name="varType">欲取得之型態，Location或PrecisionPoint</param>
        public void GetHere(int robotNum, out List<double> location, CtAce.VPlusVariableType varType = CtAce.VPlusVariableType.LOCATION) {
            List<double> locTemp = new List<double>();

            if (varType == CtAce.VPlusVariableType.LOCATION) {
                locTemp = mVpLink.WhereWorld(robotNum).ToArray().ToList();
            } else if (varType == CtAce.VPlusVariableType.PRECISION_POINT) {
                PrecisionPoint pLoc = mVpLink.WhereJoint(robotNum);
                for (byte idx = 0; idx < pLoc.Length; idx++) {
                    locTemp.Add(pLoc.GetJoint(idx));
                }
            } else throw (new Exception("目前僅支援Location與PrecisionPoint，請勿帶入其他型態"));

            location = locTemp;
        }
        #endregion

    }
}
