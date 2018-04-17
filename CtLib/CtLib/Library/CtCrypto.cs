using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.Net.NetworkInformation;

using CtLib.Forms;
using CtLib.Module.Utility;

namespace CtLib.Library {

	/// <summary>提供文字加解密、網卡取得與認證、時間鎖等</summary>
	/// <example>
	/// 此 CtCrypto 可分為三部分: 加解密、網卡MAC、授權檔
	/// 
	/// 1. 加解密
	/// <code language="C#">
	/// Stat stt = Stat.SUCCESS;
	/// string oriStr = "這句話要加密";
	/// string encStr = "";
	/// 
	/// stt = CtCrypto.Encrypt(CryptoMode.AES256, oriStr, out encStr);  //使用 AES256 進行加密
	/// stt = CtCrypto.Decrypt(CryptoMode.AES256, encStr, out oriStr);  //將加密後的字串解密
	/// 
	/// </code>
	/// 
	/// 2. MAC
	/// <code language="C#">
	/// List&lt;string&gt; mac = GetMacAddress();   //會將執行此程式之電腦的所有網卡MAC丟到裡面來
	/// 
	/// bool isMac = IsMacAddress("28F3478B2582");  //檢查此 Mac 是否存為此電腦的網卡之一
	/// if (isMac) MessageBox.Show("此 MAC 為此電腦之網卡");
	/// else MessageBox.Show("非法的 MAC !!");
	/// </code>
	/// 
	/// 3. 授權檔，可用於時間鎖或炸彈
	/// <code language="C#">
	/// Stat stt = CreateTimeLicense(DateTime.Parse("2099/12/31")); //如是 Win7 (含)以上系統，請先建立空檔案(有系統權限問題)！ 時間鎖設於 2099 年 12 月 31 日 凌晨 00:00 爆炸!!
	///
	/// bool bombStt = CheckTimeLincense(true);                     //檢查時間鎖，並檢查 Mac 是否相符
	/// if (bombStt) MessageBox.Show("時間尚未超過時間鎖！可正常使用");
	/// else MessageBox.Show("時間已超過！請立即停止任何操作，程式箱自動關閉");
	/// </code>
	/// </example>
	public static class CtCrypto {

		#region Version

		/// <summary>CtCrypto 版本訊息</summary>
		/// <remarks><code language="C#">
		/// 1.0.0  Ahern [2014/09/13]
		///     + 從舊版CtLib搬移
		///     
		/// 1.0.1  Ahern [2015/02/27]
		///     \ IsMacAddress 加入判斷搜尋到的 MAC 是否可用
		/// </code></remarks>
		public static CtVersion Version { get { return new CtVersion(1, 0, 1, "2015/02/27", "Ahern Kuo"); } }

		#endregion

		#region Declaration - Definitions

		/// <summary>
		/// 授權檔案存放位置，用於判斷時間鎖或者炸彈
		/// <para>此檔案如是放在系統區，可能要先手動將檔案複製進去(空的也可)，否則有可能無法建立新檔案</para>
		/// </summary>
		private static readonly string LICENSE_PATH = @"C:\Windows\System32\dDbtUFbll.dll";

		/// <summary>AES 加解密使用之「秘密金鑰」</summary>
		private static readonly string CRYPTO_AES_KEY = @"Software Department of CASTEC International Corp. / 友上科技股份有限公司 軟體部";
		/// <summary>AES 加解密使用之「初始化向量(Initialization Vector)」</summary>
		private static readonly string CRYPTO_AES_IV = @"Copyright © 2005~2014 CASTEC, Inc.";

		#endregion

		#region Function - Method

		/// <summary>計算湊雜碼</summary>
		/// <param name="hashMode">湊雜碼模式</param>
		/// <param name="oriString">欲計算之字串</param>
		/// <returns>湊雜碼位組元</returns>
		private static byte[] ComputeHash(HashMode hashMode, string oriString) {
			byte[] hashByte = null;

			switch (hashMode) {
				case HashMode.MD5:
					MD5CryptoServiceProvider MD5 = new MD5CryptoServiceProvider();
					hashByte = MD5.ComputeHash(Encoding.Unicode.GetBytes(oriString));
					break;
				case HashMode.SHA1:
					SHA1Managed SHA = new SHA1Managed();
					hashByte = SHA.ComputeHash(Encoding.Unicode.GetBytes(oriString));
					break;
				case HashMode.SHA2_256:
					SHA256Managed SHA256 = new SHA256Managed();
					hashByte = SHA256.ComputeHash(Encoding.Unicode.GetBytes(oriString));
					break;
				case HashMode.SHA2_384:
					SHA384Managed SHA384 = new SHA384Managed();
					hashByte = SHA384.ComputeHash(Encoding.Unicode.GetBytes(oriString));
					break;
				case HashMode.SHA2_512:
					SHA512Managed SHA512 = new SHA512Managed();
					hashByte = SHA512.ComputeHash(Encoding.Unicode.GetBytes(oriString));
					break;
			}

			return hashByte;
		}

		/// <summary>計算湊雜碼文字</summary>
		/// <param name="hashMode">湊雜碼模式</param>
		/// <param name="oriString">欲計算之字串</param>
		/// <returns>湊雜碼字串</returns>
		private static string ComputeHashString(HashMode hashMode, string oriString) {
			string hashStr = "";

			byte[] hashByte = ComputeHash(hashMode, oriString);
			hashStr = Convert.ToBase64String(hashByte);

			return hashStr;
		}

		#endregion

		#region Function - Core

		#region Encrypt / Decrypt

		/// <summary>使用 Base64/AES 加密文字，並直接回傳</summary>
		/// <param name="cryMode">加密方法</param>
		/// <param name="oriString">要加密的字串</param>
		public static void Encrypt(CryptoMode cryMode, ref string oriString) {
			byte[] encodeBytes;
			string encodeStr = "";
			switch (cryMode) {
				case CryptoMode.AES256:
					RijndaelManaged AES = new RijndaelManaged();                                        //AES使用密碼學中的Rijndael加密法
					MD5CryptoServiceProvider MD5 = new MD5CryptoServiceProvider();                      //用於計算湊雜碼
					encodeBytes = Encoding.Unicode.GetBytes(oriString);                                 //將要加密文字轉成Byte
					byte[] keyData = MD5.ComputeHash(Encoding.Unicode.GetBytes(CRYPTO_AES_KEY));        //取得Key的湊雜碼
					byte[] IVData = MD5.ComputeHash(Encoding.Unicode.GetBytes(CRYPTO_AES_IV));          //取得IV的湊雜碼
					ICryptoTransform cryTrans = AES.CreateEncryptor(keyData, IVData);                   //建立加密子(需要帶入Key與IV)
					byte[] bytTemp = cryTrans.TransformFinalBlock(encodeBytes, 0, encodeBytes.Length);  //轉換
					encodeStr = Convert.ToBase64String(bytTemp);                                        //將轉換完成的Byte轉回String
					oriString = encodeStr;                                                              //回傳
					break;

				case CryptoMode.Base64:
					encodeBytes = Encoding.Unicode.GetBytes(oriString);
					encodeStr = Convert.ToBase64String(encodeBytes);
					oriString = encodeStr;
					break;
			}
		}

		/// <summary>使用 Base64/AES 加密文字，並透過out回傳</summary>
		/// <param name="cryMode">加密方法</param>
		/// <param name="oriString">要加密的字串</param>
		/// <param name="outString">接收回傳之字串</param>
		public static void Encrypt(CryptoMode cryMode, string oriString, out string outString) {
			string encodeStr = "";

			byte[] encodeBytes;
			switch (cryMode) {
				case CryptoMode.AES256:
					RijndaelManaged AES = new RijndaelManaged();                                        //AES使用密碼學中的Rijndael加密法
					MD5CryptoServiceProvider MD5 = new MD5CryptoServiceProvider();                      //用於計算湊雜碼
					encodeBytes = Encoding.Unicode.GetBytes(oriString);                                 //將要加密文字轉成Byte
					byte[] keyData = MD5.ComputeHash(Encoding.Unicode.GetBytes(CRYPTO_AES_KEY));        //取得Key的湊雜碼
					byte[] IVData = MD5.ComputeHash(Encoding.Unicode.GetBytes(CRYPTO_AES_IV));          //取得IV的湊雜碼
					ICryptoTransform cryTrans = AES.CreateEncryptor(keyData, IVData);                   //建立加密子(需要帶入Key與IV)
					byte[] bytTemp = cryTrans.TransformFinalBlock(encodeBytes, 0, encodeBytes.Length);  //轉換
					encodeStr = Convert.ToBase64String(bytTemp);                                        //將轉換完成的Byte轉回String
					break;

				case CryptoMode.Base64:
					encodeBytes = Encoding.Unicode.GetBytes(oriString);
					encodeStr = Convert.ToBase64String(encodeBytes);
					break;
			}

			outString = encodeStr;
		}

		/// <summary>解密 Base64/AES 字串，並直接回傳</summary>
		/// <param name="cryMode">解密方法</param>
		/// <param name="oriString">要解密的字串</param>
		public static void Decrypt(CryptoMode cryMode, ref string oriString) {
			byte[] decodeBytes;
			string decodeStr = "";
			switch (cryMode) {
				case CryptoMode.AES256:
					RijndaelManaged AES = new RijndaelManaged();                                        //AES使用密碼學中的Rijndael加密法
					MD5CryptoServiceProvider MD5 = new MD5CryptoServiceProvider();                      //用於計算湊雜碼
					decodeBytes = Convert.FromBase64String(oriString);                                  //將要加密文字轉成Byte
					byte[] keyData = MD5.ComputeHash(Encoding.Unicode.GetBytes(CRYPTO_AES_KEY));        //取得Key的湊雜碼
					byte[] IVData = MD5.ComputeHash(Encoding.Unicode.GetBytes(CRYPTO_AES_IV));          //取得IV的湊雜碼
					ICryptoTransform cryTrans = AES.CreateDecryptor(keyData, IVData);                   //建立解密子(需要帶入Key與IV)
					byte[] bytTemp = cryTrans.TransformFinalBlock(decodeBytes, 0, decodeBytes.Length);  //轉換
					decodeStr = Encoding.Unicode.GetString(bytTemp);                                    //將轉換完成的Byte轉回String
					oriString = decodeStr;                                                              //回傳
					break;

				case CryptoMode.Base64:
					decodeBytes = Convert.FromBase64String(oriString);
					decodeStr = Encoding.Default.GetString(decodeBytes);
					oriString = decodeStr;
					break;
			}
		}

		/// <summary>解密 Base64/AES 字串，並透過out回傳</summary>
		/// <param name="cryMode">解密方法</param>
		/// <param name="oriString">要解密的字串</param>
		/// <param name="outString">解密完成之字串</param>
		public static void Decrypt(CryptoMode cryMode, string oriString, out string outString) {
			string decodeStr = "";

			byte[] decodeBytes;
			switch (cryMode) {
				case CryptoMode.AES256:
					RijndaelManaged AES = new RijndaelManaged();                                        //AES使用密碼學中的Rijndael加密法
					MD5CryptoServiceProvider MD5 = new MD5CryptoServiceProvider();                      //用於計算湊雜碼
					decodeBytes = Convert.FromBase64String(oriString);                                  //將要加密文字轉成Byte
					byte[] keyData = MD5.ComputeHash(Encoding.Unicode.GetBytes(CRYPTO_AES_KEY));        //取得Key的湊雜碼
					byte[] IVData = MD5.ComputeHash(Encoding.Unicode.GetBytes(CRYPTO_AES_IV));          //取得IV的湊雜碼
					ICryptoTransform cryTrans = AES.CreateDecryptor(keyData, IVData);                   //建立解密子(需要帶入Key與IV)
					byte[] bytTemp = cryTrans.TransformFinalBlock(decodeBytes, 0, decodeBytes.Length);  //轉換
					decodeStr = Encoding.Unicode.GetString(bytTemp);                                    //將轉換完成的Byte轉回String
					break;

				case CryptoMode.Base64:
					decodeBytes = Convert.FromBase64String(oriString);
					decodeStr = Encoding.Default.GetString(decodeBytes);
					break;
			}

			outString = decodeStr;
		}

		#endregion

		#region MAC

		/// <summary>取得網路卡實體位置</summary>
		/// <returns>Mac字串集合</returns>
		/// <remarks>此方法是取得所有電腦內的Mac，沒有判斷類型</remarks>
		public static List<string> GetMacAddress() {
			string strTemp = "";
			List<string> macString = new List<string>();
			IPGlobalProperties pcProperty = IPGlobalProperties.GetIPGlobalProperties();

			foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces()) {
				strTemp = adapter.GetPhysicalAddress().ToString();
				if ((strTemp != "") && (!strTemp.Contains("00000000000000"))) {
					macString.Add(strTemp);
				}
			}
			return macString;
		}

		/// <summary>判斷字串是否是本機網路卡實體位置之一</summary>
		/// <param name="address">欲判斷之字串</param>
		/// <returns>是(<see langword="true"/>) / 否(<see langword="false"/>)</returns>
		public static bool IsMacAddress(string address) {
			bool bolTemp = false;
			string strTemp = "";
			List<string> macString = new List<string>();

			IPGlobalProperties pcProperty = IPGlobalProperties.GetIPGlobalProperties();

			foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces()) {
				strTemp = adapter.GetPhysicalAddress().ToString();
				if ((strTemp != "") && (!strTemp.Contains("00000000000000")) && (strTemp == address)) {
					bolTemp = true;
					break;
				}
			}

			return bolTemp;
		}

		/// <summary>驗證輸入的 AES256 加密字串是否為本機的 MAC 之一</summary>
		/// <param name="oriMac">加密的 AES256 字串</param>
		/// <param name="retAddr">回傳 MAC 加密後的字串，可供 Opcode 寫入使用。如果非重設驗證，則會回傳相同加密字串</param>
		/// <returns>(<see langword="true"/>)驗證成功  (<see langword="false"/>)驗證失敗</returns>
		public static bool MacVerify(string oriMac, out string retAddr) {

			bool retVerify = false;

			/*-- 比對輸入的認證 --*/
			string strEncrypt;
			Encrypt(CryptoMode.AES256, oriMac, out strEncrypt);

			/*-- 如果為初始化密碼，進行初始化動作 --*/
			if (strEncrypt == "UtWAz4xcdPTnUuJj526qHg==") {
				/* 確認驗證碼 */
				string pwd, pwsEnc;
				CtInput.Text(out pwd, "認證", "請輸入驗證碼");
				Encrypt(CryptoMode.AES256, pwd, out pwsEnc);
				if (pwsEnc != "td1+CY1spGpYHBsye0VoEty8UA0rw7g9bQERA7XaKLM=") throw (new Exception("恢復預設，但驗證輸入錯誤: " + pwd));

				/* 驗證成功則回傳 */
				Encrypt(CryptoMode.AES256, GetMacAddress()[0], out strEncrypt);
				retAddr = strEncrypt;
				retVerify = true;

				/*-- 非初始化密碼，比對 MacAddress --*/
			} else {
				string strDecrypt;
				Decrypt(CryptoMode.AES256, oriMac, out strDecrypt);
				retAddr = oriMac;
				retVerify = IsMacAddress(strDecrypt);
				if (!retVerify) {
					CtMsgBox.Show("系統錯誤", "此電腦非 CASTEC 認證的電腦", MsgBoxBtn.OK, MsgBoxStyle.Error);
					throw (new Exception("此電腦非 CASTEC 認證的電腦。 輸入授權: " + oriMac));
				}
			}

			return retVerify;
		}
		#endregion

		#region License

		/// <summary>隨機產生亂數字串</summary>
		/// <param name="length">欲產生之字串長度</param>
		/// <param name="mode">模式 (0)僅數字 (1)僅英文_小寫 (2)僅英文_大寫 (3)數字+英文大小寫</param>
		/// <returns>產生後的字串</returns>
		public static string RandomString(int length, byte mode = 3) {
			string tempStr = "";
			int tempMode = 0;
			int tempChar = 0;

			for (int i = 0; i < length; i++) {
				tempMode = (mode > 2) ? CtConst.RandomInt(3) : mode;
				switch (tempMode) {
					case 0:
						tempChar = CtConst.RandomInt(9) + 48;
						tempStr += CtConvert.ASCII(tempChar);
						break;
					case 1:
						tempChar = CtConst.RandomInt(26) + 97;
						tempStr += CtConvert.ASCII(tempChar);
						break;
					default:
						tempChar = CtConst.RandomInt(26) + 65;
						tempStr += CtConvert.ASCII(tempChar);
						break;
				}
			}

			return tempStr;
		}

		/// <summary>建立時間授權</summary>
		/// <param name="targetTime">截止時間。如設2014/03/31，則在2014/03/31凌晨失效</param>
		public static void CreateTimeLicense(DateTime targetTime) {
			string[] tempStr = new string[100];
			byte rndIdx = 0;
			int rndTemp = 0;
			int[] rndVal = new int[3] { -1, -1, -1 };
			string rndStr = "";

			do {
				/*-- 隨機取一數字 --*/
				rndTemp = CtConst.RandomInt(100);

				/*-- 如果該數字沒有重複，且非儲存行數資料的行號，則記錄下來直至取得3組 --*/
				if ((rndTemp != rndVal[0]) && (rndTemp != rndVal[1]) && (rndTemp != rndVal[2]) && (rndTemp != 3)) {
					rndStr += "," + rndTemp.ToString("00");
					rndVal[rndIdx] = rndTemp;
					rndIdx++;
				}
			} while (rndIdx < 3);

			/*-- 建立相關字串，並放置相對應行數裡 (前面或後面會補一些字，讓他在文件裡看起來字數都一樣，讀取後用逗號分割) --*/
			Encrypt(
				CryptoMode.AES256,
				"License Location " + rndStr,
				out tempStr[3]
			);

			Encrypt(
				CryptoMode.AES256,
				"CASTEC International Corp.",
				out tempStr[rndVal[0]]
			);

			Encrypt(
				CryptoMode.AES256,
				targetTime.ToString("Time,yyyy/MM/dd HH:mm:ss"),
				out tempStr[rndVal[1]]
			);

			Encrypt(
				CryptoMode.AES256,
				"MAC Address," + GetMacAddress()[1],
				out tempStr[rndVal[2]]
			);

			/*-- 如果該位置並不是儲存資料區，隨機產生26字的亂數字串並塞進去 --*/
			for (int i = 0; i < tempStr.Length; i++) {
				if ((tempStr[i] == "") || (tempStr[i] == null)) {
					Encrypt(
						CryptoMode.AES256,
						RandomString(26),
						out tempStr[i]
					);
				}
			}

			/*-- 儲存檔案 --*/
			CtFile.WriteFile(LICENSE_PATH, tempStr);
		}

		/// <summary>檢查時間授權，如在時間內則回傳True，反之為False</summary>
		/// <param name="mac">是否檢查MAC</param>
		/// <returns>是否允許使用 (<see langword="false"/>)已失效 (<see langword="true"/>)時間內可使用</returns>
		public static bool CheckTimeLincense(bool mac = false) {
			Stat stt = Stat.SUCCESS;
			bool bolPass = false;
			string[] strSplit;
			string strDecode = "";
			int[] intLoc = new int[3] { -1, -1, -1 };
			try {
				if (CtFile.IsFileExist(LICENSE_PATH)) {
					/*-- 讀檔 --*/
					List<string> strTemp = CtFile.ReadFile(LICENSE_PATH);
					if (strTemp.Count < 99) {
						stt = Stat.ER_SYS_FILACC;
						throw (new Exception("授權檔內容錯誤"));
					}

					/*-- 取得資料位置 --*/
					Decrypt(
						CryptoMode.AES256,
						strTemp[3],
						out strDecode
					);
					strSplit = strDecode.Split(CtConst.CHR_COMMA, StringSplitOptions.RemoveEmptyEntries);
					if (strSplit.Length != 4) {
						stt = Stat.ER_SYS_FILACC;
						throw (new Exception("授權檔索引內容錯誤"));
					}
					intLoc[0] = CtConvert.CInt(strSplit[1]);    //公司索引
					intLoc[1] = CtConvert.CInt(strSplit[2]);    //截止時間索引
					intLoc[2] = CtConvert.CInt(strSplit[3]);    //MAC索引

					/*-- 檢查公司 --*/
					strDecode = "";
					Decrypt(
						CryptoMode.AES256,
						strTemp[intLoc[0]],
						out strDecode
					);
					if (strDecode != "CASTEC International Corp.") {
						stt = Stat.ER_SYS_INVSET;
						throw (new Exception("授權檔比對(1)錯誤"));
					}

					/*-- 檢查時間 --*/
					strDecode = "";
					Decrypt(
						CryptoMode.AES256,
						strTemp[intLoc[1]],
						out strDecode
					);
					strSplit = strDecode.Split(CtConst.CHR_COMMA, StringSplitOptions.RemoveEmptyEntries);
					if (DateTime.Now > DateTime.Parse(strSplit[1])) {
						stt = Stat.ER_SYS_INVSET;
						throw (new Exception("時間授權已過"));
					}

					/*-- 取得是否是在本台電腦建立的。如不需檢查則直接回傳成功 --*/
					if (mac) {
						strDecode = "";
						Decrypt(
							CryptoMode.AES256,
							strTemp[intLoc[2]],
							out strDecode
						);
						strSplit = strDecode.Split(CtConst.CHR_COMMA, StringSplitOptions.RemoveEmptyEntries);
						bolPass = IsMacAddress(strSplit[1]);
					} else bolPass = true;
				}
			} catch (Exception ex) {
				if (stt == Stat.SUCCESS) stt = Stat.ER_SYSTEM;
				CtStatus.Report(stt, ex);
			}
			return bolPass;
		}

		/// <summary>取得授權檔案內所設定的時間</summary>
		/// <param name="time">從檔案取得的設定時間</param>
		public static void GetTimeLincense(out DateTime time) {
			DateTime tempTime = DateTime.Now;
			string[] strSplit;
			string strDecode = "";
			int intLoc = -1;

			if (CtFile.IsFileExist(LICENSE_PATH)) {
				/*-- 讀檔 --*/
				List<string> strTemp = CtFile.ReadFile(LICENSE_PATH);
				if (strTemp.Count < 99) throw (new Exception("授權檔內容錯誤"));

				/*-- 取得資料位置 --*/
				Decrypt(
					CryptoMode.AES256,
					strTemp[3],
					out strDecode
				);
				strSplit = strDecode.Split(CtConst.CHR_COMMA, StringSplitOptions.RemoveEmptyEntries);
				if (strSplit.Length != 4) throw (new Exception("授權檔索引內容錯誤"));

				intLoc = CtConvert.CInt(strSplit[2]);    //截止時間索引

				/*-- 檢查時間 --*/
				strDecode = "";
				Decrypt(
					CryptoMode.AES256,
					strTemp[intLoc],
					out strDecode
				);
				strSplit = strDecode.Split(CtConst.CHR_COMMA, StringSplitOptions.RemoveEmptyEntries);
				tempTime = DateTime.Parse(strSplit[1]);
			}
			time = tempTime;
		}
		#endregion

		#endregion
	}
}
