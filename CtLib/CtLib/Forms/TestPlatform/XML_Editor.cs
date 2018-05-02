using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using CtLib.Library;

namespace CtLib.Forms.TestPlatform {
	/// <summary>XML 修改器</summary>
	public partial class XML_Editor : Form {

		List<ICtIO> mIOList = new List<ICtIO>();

		/// <summary>建構元</summary>
		public XML_Editor() {
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e) {
			ctxmAdd.Show(button1, button1.Width, 0);
		}

		private TreeNode FindNodes(TreeNodeCollection nodes, string keyWord) {
			return nodes.Find(keyWord, true)[0];
		}

		private ICtIO FindIO(List<ICtIO> ioData, string id) {
			return ioData.Find(data => {
				if (data.Device == Devices.AdeptACE)
					return (data as AceIO).IoNum.ToString() == id;
				else
					return (data as BeckhoffIO).IoName == id;
			});
		}

		private void treeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e) {
			if (e.Node.Level == 2) {
				string strTemp = "";
				if (e.Node.Text.StartsWith("註解")) {
					strTemp = e.Node.Text.Replace("註解: ", "").Trim();
					CtInput.Text(out strTemp, "修改註解", "請輸入要修改的註解", strTemp);
					if (strTemp != "") {
						e.Node.Text = "註解: " + strTemp;
						FindIO(mIOList, e.Node.Parent.Text).SetComment(strTemp);
					}
				} else if (e.Node.Text.StartsWith("變數")) {
					strTemp = e.Node.Text.Replace("變數: ", "").Trim();
					CtInput.Text(out strTemp, "修改變數", "請輸入要修改相對應於 Beckhoff Ads 系統之變數名稱", strTemp);
					if (strTemp != "") {
						e.Node.Text = "變數: " + strTemp;
						(FindIO(mIOList, e.Node.Parent.Text) as BeckhoffIO).SetVariable(strTemp);
					}
				} else if (e.Node.Text.StartsWith("索引")) {
					strTemp = e.Node.Text.Replace("索引: ", "").Trim();
					CtInput.Text(out strTemp, "修改索引", "請輸入要修改於陣列中之索引值\r\n\"-1\" 表示忽略", strTemp);
					if (strTemp != "") {
						e.Node.Text = "索引: " + strTemp;
						(FindIO(mIOList, e.Node.Parent.Text) as BeckhoffIO).SetArrayIndex(CtConvert.CInt(strTemp));
					}
				} else if (e.Node.Text.StartsWith("編號")) {
					strTemp = e.Node.Text.Replace("編號: ", "").Trim();
					CtInput.Text(out strTemp, "修改編號", "請輸入要修改於 FC1/FC2/FC5/FC15 之編號", strTemp);
					if (strTemp != "") {
						e.Node.Text = "編號: " + strTemp;
						(FindIO(mIOList, e.Node.Parent.Text) as WagoIO).SetDiscreteBit(CtConvert.CInt(strTemp));
					}
				} else if (e.Node.Text.StartsWith("暫存器(Bit)")) {
					strTemp = e.Node.Text.Replace("暫存器(Bit): ", "").Trim();
					CtInput.Text(out strTemp, "修改暫存器(Bit)", "請輸入要修改於 Register 之位元索引數", strTemp);
					if (strTemp != "") {
						e.Node.Text = "暫存器(Bit): " + strTemp;
						(FindIO(mIOList, e.Node.Parent.Text) as WagoIO).SetRegisterNo(CtConvert.CByte(strTemp));
					}
				} else if (e.Node.Text.StartsWith("暫存器")) {
					strTemp = e.Node.Text.Replace("暫存器: ", "").Trim();
					CtInput.Text(out strTemp, "修改暫存器", "請輸入要修改於 FC3/FC4/FC6/FC16 之 Register 編號", strTemp);
					if (strTemp != "") {
						e.Node.Text = "暫存器: " + strTemp;
						(FindIO(mIOList, e.Node.Parent.Text) as WagoIO).SetRegisterBit(CtConvert.CByte(strTemp));
					}
				} else if (e.Node.Text.StartsWith("Enum")) {
					strTemp = e.Node.Text.Replace("Enum: ", "").Trim();
					CtInput.Text(out strTemp, "修改Enum數值", "請輸入要修改相對於 Enum 裡之數值", strTemp);
					if (strTemp != "") {
						e.Node.Text = "Enum: " + strTemp;
						FindIO(mIOList, e.Node.Parent.Text).SetEnumIndex(CtConvert.CUShort(strTemp));
					}
				}
			}
		}

		private void button3_Click(object sender, EventArgs e) {

			/*-- 排序 --*/
			mIOList.Sort();

			if (checkBox1.Checked) {
				if (CtMsgBox.Show("Enumerations", "是否要重設所有 Enum?", MsgBoxBtn.YesNo, MsgBoxStyle.Question) == MsgBoxBtn.Yes)
					ResetEnumIdx(mIOList);
				else
					SetEnumIdx(mIOList);
			}
            SaveFileDialog dialog = new SaveFileDialog() {
                DefaultExt = "xml",
                Filter = "XML|*.xml",
                InitialDirectory = CtDefaultPath.GetPath(SystemPath.Configuration)
            };
            if (dialog.ShowDialog() == DialogResult.OK) {
				CtIO.SaveToXML(dialog.FileName, mIOList);
			}
		}

		private void button2_Click(object sender, EventArgs e) {
			TreeNode node = null;
			if (treeView.SelectedNode.Level == 1) {
				node = treeView.SelectedNode;
			} else if (treeView.SelectedNode.Level == 2) {
				node = treeView.SelectedNode.Parent;
			}

			if (node != null) {
				mIOList.Remove(FindIO(mIOList, node.Text));
				treeView.Nodes.Remove(node);
			}
		}

		private void adeptToolStripMenuItem_Click(object sender, EventArgs e) {
			Stat stt = Stat.SUCCESS;

            /*-- 輸入 IO 編號 --*/
            stt = CtInput.Text(out string strIONum, "I/O 編號", "請輸入此 I/O 編號");
            if (stt == Stat.WN_SYS_USRCNC) return;

            /*-- 輸入註解 --*/
            stt = CtInput.Text(out string strComment, "I/O 註解", "請輸入註解");
            if (stt == Stat.WN_SYS_USRCNC) return;

            /*-- 加到 TreeView --*/
            if (int.TryParse(strIONum, out int ioNum)) {
                TreeNode AdeptACE;
                if (ioNum > 1999) {
                    AdeptACE = treeView.Nodes[2].Nodes.Add(strIONum);
                } else if (ioNum > 999) {
                    AdeptACE = treeView.Nodes[0].Nodes.Add(strIONum);
                } else {
                    AdeptACE = treeView.Nodes[1].Nodes.Add(strIONum);
                }
                AdeptACE.Nodes.Add("註解: " + strComment);

                /*-- 加到 mIOLisy --*/
                mIOList.Add(new AceIO(int.Parse(strIONum), strComment));
            } else CtMsgBox.Show("錯誤", "I/O 編號錯誤，請重新新增", MsgBoxBtn.OK, MsgBoxStyle.Error);
        }

		private void beckhoffToolStripMenuItem_Click(object sender, EventArgs e) {
			Stat stt = Stat.SUCCESS;

			/*-- 輸入 IO 類型 --*/
			string strType = "";
			stt = CtInput.ComboBoxList(out strType, "I/O 類型", "請選擇類型", new List<string> { "Input", "Output" });
			if (stt == Stat.WN_SYS_USRCNC) return;

			/*-- 輸入 IO 名稱 --*/
			string strID = "";
			stt = CtInput.Text(out strID, "I/O 代號", "請輸入此 I/O 代號。如 \"Y0013\"");
			if (stt == Stat.WN_SYS_USRCNC) return;

			/*-- 輸入 IO 變數 --*/
			string strVar = "";
			stt = CtInput.Text(out strVar, "變數", "請輸入此 I/O 相對應的變數。如 pQ_ClampOpen");
			if (stt == Stat.WN_SYS_USRCNC) return;

			/*-- 輸入 IO 索引 --*/
			string strIdx = "";
			stt = CtInput.Text(out strID, "陣列索引值", "請輸入此 I/O 相對應於陣列之索引\r\n\"-1\" 表示忽略", "-1");
			if (stt == Stat.WN_SYS_USRCNC) return;

			/*-- 輸入註解 --*/
			string strComment = "";
			stt = CtInput.Text(out strComment, "I/O 註解", "請輸入註解");
			if (stt == Stat.WN_SYS_USRCNC) return;

			/*-- 加到 TreeView --*/
			TreeNode beckhoff;
			IOTypes ioType = IOTypes.Input;
			if (strType == "Input") {
				ioType = IOTypes.Input;
				beckhoff = treeView.Nodes[3].Nodes.Add(strID);
			} else {
				ioType = IOTypes.Output;
				beckhoff = treeView.Nodes[4].Nodes.Add(strID);
			}

			beckhoff.Nodes.Add("變數: " + strVar);
			beckhoff.Nodes.Add("索引: " + strIdx);
			beckhoff.Nodes.Add("註解: " + strComment);

			/*-- 加到 mIOList --*/
			mIOList.Add(new BeckhoffIO(strID, ioType, strVar, strComment, int.Parse(strIdx)));
		}

		private void wagoToolStripMenuItem_Click(object sender, EventArgs e) {
			Stat stt = Stat.SUCCESS;

			/*-- 輸入 IO 類型 --*/
			string strType = "";
			stt = CtInput.ComboBoxList(out strType, "I/O 類型", "請選擇類型", new List<string> { "Input", "Output" });
			if (stt == Stat.WN_SYS_USRCNC) return;

			/*-- 輸入 IO 編號 --*/
			string strIONum = "";
			stt = CtInput.Text(out strIONum, "I/O 編號", "請輸入 FC1/FC2/FC5/FC15 編號");
			if (stt == Stat.WN_SYS_USRCNC) return;

			/*-- 輸入 IO Reg Num --*/
			string strRegNum = "";
			stt = CtInput.Text(out strRegNum, "Register 編號", "請輸入此 I/O 相對應的暫存器編號");
			if (stt == Stat.WN_SYS_USRCNC) return;

			/*-- 輸入 IO Reg Bit --*/
			string strRegBit = "";
			stt = CtInput.Text(out strRegBit, "Register 索引值", "請輸入此 I/O 相對應於 Register 的位元索引", "0");
			if (stt == Stat.WN_SYS_USRCNC) return;

			/*-- 輸入註解 --*/
			string strComment = "";
			stt = CtInput.Text(out strComment, "I/O 註解", "請輸入註解");
			if (stt == Stat.WN_SYS_USRCNC) return;

			/*-- 加到 TreeView --*/
			TreeNode wago;
			IOTypes ioType = IOTypes.Input;
			if (strType == "Input") {
				ioType = IOTypes.Input;
				wago = treeView.Nodes[5].Nodes.Add(strIONum);
			} else {
				ioType = IOTypes.Output;
				wago = treeView.Nodes[6].Nodes.Add(strIONum);
			}

			wago.Nodes.Add("編號: " + strIONum);
			wago.Nodes.Add("暫存器: " + strRegNum);
			wago.Nodes.Add("暫存器(Bit): " + strRegBit);
			wago.Nodes.Add("註解: " + strComment);

			/*-- 加到 mIOLisy --*/
			mIOList.Add(new WagoIO(int.Parse(strIONum), ushort.Parse(strRegNum), byte.Parse(strRegBit), ioType, strComment));
		}

		private void button4_Click(object sender, EventArgs e) {
			TreeNode node = null;
			if (treeView.SelectedNode.Level == 1) {
				node = treeView.SelectedNode;
			} else if (treeView.SelectedNode.Level == 2) {
				node = treeView.SelectedNode.Parent;
			}

			if (node != null) {
				ICtIO ioTemp = FindIO(mIOList, node.Text);
				mIOList.Add(ioTemp.Clone());

				node.Parent.Nodes.Add(node.Clone() as TreeNode);

			}
		}

		private void iODataToolStripMenuItem_Click(object sender, EventArgs e) {
			mIOList.Clear();
			treeView.Nodes.Clear();

			treeView.Nodes.Add("AdeptACE Digital Input");
			treeView.Nodes.Add("AdeptACE Digital Output");
			treeView.Nodes.Add("AdeptACE Software I/O");
			treeView.Nodes.Add("Beckhoff Input");
			treeView.Nodes.Add("Beckhoff Output");
			treeView.Nodes.Add("Wago Input");
			treeView.Nodes.Add("Wago Output");
		}

		private void iOXMLFileToolStripMenuItem_Click(object sender, EventArgs e) {
			OpenFileDialog dialog = new OpenFileDialog();
			dialog.DefaultExt = "xml";
			dialog.Filter = "XML|*.xml";
			dialog.InitialDirectory = CtDefaultPath.GetPath(SystemPath.Configuration);
			if (dialog.ShowDialog() == DialogResult.OK) {
				treeView.Nodes.Clear();
				treeView.Nodes.Add("AdeptACE Digital Input");
				treeView.Nodes.Add("AdeptACE Digital Output");
				treeView.Nodes.Add("AdeptACE Software I/O");
				treeView.Nodes.Add("Beckhoff Input");
				treeView.Nodes.Add("Beckhoff Output");
				treeView.Nodes.Add("Beckhoff Flag");
				treeView.Nodes.Add("Wago Input");
				treeView.Nodes.Add("Wago Output");
				treeView.Nodes.Add("Delta Input");
				treeView.Nodes.Add("Delta Output");

				mIOList.Clear();
				CtIO.LoadFromXML(dialog.FileName, out mIOList);

				if (mIOList != null) {
					foreach (ICtIO item in mIOList) {
						switch (item.Device) {
							case Devices.AdeptACE:
								if (item.IoType == IOTypes.Input) {
									TreeNode AdeptACEIn = treeView.Nodes[0].Nodes.Add((item as AceIO).IoNum.ToString());
									AdeptACEIn.Nodes.Add("註解: " + item.Comment);
									AdeptACEIn.Nodes.Add("Enum: " + item.EnumIndex.ToString());
								} else if (item.IoType == IOTypes.Output) {
									TreeNode AdeptACEOut = treeView.Nodes[1].Nodes.Add((item as AceIO).IoNum.ToString());
									AdeptACEOut.Nodes.Add("註解: " + item.Comment);
									AdeptACEOut.Nodes.Add("Enum: " + item.EnumIndex.ToString());
								} else {
									TreeNode AdeptACEOut = treeView.Nodes[2].Nodes.Add((item as AceIO).IoNum.ToString());
									AdeptACEOut.Nodes.Add("註解: " + item.Comment);
									AdeptACEOut.Nodes.Add("Enum: " + item.EnumIndex.ToString());
								}
								break;
							case Devices.Beckhoff:
								if (item.IoType == IOTypes.Input) {
									TreeNode beckhoffIn = treeView.Nodes[3].Nodes.Add((item as BeckhoffIO).IoName);
									beckhoffIn.Nodes.Add("變數: " + (item as BeckhoffIO).Variable);
									beckhoffIn.Nodes.Add("索引: " + (item as BeckhoffIO).ArrayIndex.ToString());
									beckhoffIn.Nodes.Add("註解: " + item.Comment);
									beckhoffIn.Nodes.Add("Enum: " + item.EnumIndex.ToString());
								} else if (item.IoType == IOTypes.Output) {
									TreeNode beckhoffOut = treeView.Nodes[4].Nodes.Add((item as BeckhoffIO).IoName);
									beckhoffOut.Nodes.Add("變數: " + (item as BeckhoffIO).Variable);
									beckhoffOut.Nodes.Add("索引: " + (item as BeckhoffIO).ArrayIndex.ToString());
									beckhoffOut.Nodes.Add("註解: " + item.Comment);
									beckhoffOut.Nodes.Add("Enum: " + item.EnumIndex.ToString());
								} else {
									TreeNode beckhoffOut = treeView.Nodes[5].Nodes.Add((item as BeckhoffIO).IoName);
									beckhoffOut.Nodes.Add("變數: " + (item as BeckhoffIO).Variable);
									beckhoffOut.Nodes.Add("索引: " + (item as BeckhoffIO).ArrayIndex.ToString());
									beckhoffOut.Nodes.Add("註解: " + item.Comment);
									beckhoffOut.Nodes.Add("Enum: " + item.EnumIndex.ToString());
								}
								break;
							case Devices.DELTA:
								if (item.IoType == IOTypes.Input) {
									TreeNode beckhoffIn = treeView.Nodes[8].Nodes.Add((item as DeltaIO).IoName);
									beckhoffIn.Nodes.Add("變數: " + (item as DeltaIO).Variable);
									beckhoffIn.Nodes.Add("註解: " + item.Comment);
									beckhoffIn.Nodes.Add("Enum: " + item.EnumIndex.ToString());
								} else {
									TreeNode beckhoffOut = treeView.Nodes[9].Nodes.Add((item as DeltaIO).IoName);
									beckhoffOut.Nodes.Add("變數: " + (item as DeltaIO).Variable);
									beckhoffOut.Nodes.Add("註解: " + item.Comment);
									beckhoffOut.Nodes.Add("Enum: " + item.EnumIndex.ToString());
								}
								break;
							case Devices.WAGO:
								if (item.IoType == IOTypes.Input) {
									TreeNode wagoIn = treeView.Nodes[6].Nodes.Add((item as WagoIO).DiscreteBit.ToString());
									wagoIn.Nodes.Add("編號: " + (item as WagoIO).DiscreteBit.ToString());
									wagoIn.Nodes.Add("暫存器: " + (item as WagoIO).RegisterNo.ToString());
									wagoIn.Nodes.Add("暫存器(Bit): " + (item as WagoIO).RegisterBit.ToString());
									wagoIn.Nodes.Add("註解: " + item.Comment);
									wagoIn.Nodes.Add("Enum: " + item.EnumIndex.ToString());
								} else {
									TreeNode wagoOut = treeView.Nodes[7].Nodes.Add((item as WagoIO).DiscreteBit.ToString());
									wagoOut.Nodes.Add("編號: " + (item as WagoIO).DiscreteBit.ToString());
									wagoOut.Nodes.Add("暫存器: " + (item as WagoIO).RegisterNo.ToString());
									wagoOut.Nodes.Add("暫存器(Bit): " + (item as WagoIO).RegisterBit.ToString());
									wagoOut.Nodes.Add("註解: " + item.Comment);
									wagoOut.Nodes.Add("Enum: " + item.EnumIndex.ToString());
								}
								break;
							default:
								break;
						}
					}
				}
			}
			dialog.Dispose();
		}

		private void button5_Click(object sender, EventArgs e) {

			mIOList.Sort();

			if (checkBox1.Checked) {
				if (CtMsgBox.Show("Enumerations", "是否要重設所有 Enum?", MsgBoxBtn.YesNo, MsgBoxStyle.Question) == MsgBoxBtn.Yes)
					ResetEnumIdx(mIOList);
				else
					SetEnumIdx(mIOList);
			}
			SaveFileDialog dialog = new SaveFileDialog();
			dialog.InitialDirectory = CtDefaultPath.GetPath(SystemPath.Configuration);
			if (dialog.ShowDialog() == DialogResult.OK) {
				List<string> strTemp = new List<string> { "enum Name : ushort {" };
				foreach (ICtIO item in mIOList) {
					strTemp.Add("\t///<summary>" + item.Comment + "</summary>");
					if (item.Device == Devices.AdeptACE) {
						strTemp.Add("\tACE_" + ((item.IoType == IOTypes.Input) ? "DI_" : (item.IoType == IOTypes.Output ? "DO_" : "SIO_")) + (item as AceIO).IoNum.ToString("0000") + " = " + item.EnumIndex.ToString() + ",");
					} else if (item.Device == Devices.Beckhoff) {
						strTemp.Add("\tBKF_" + ((item.IoType == IOTypes.Input) ? "DI_" : (item.IoType == IOTypes.Output ? "DO_" : "FLAG_")) + (item as BeckhoffIO).IoName + " = " + item.EnumIndex.ToString() + ",");
					} else if (item.Device == Devices.WAGO) {
						strTemp.Add("\tWGO_" + ((item.IoType == IOTypes.Input) ? "DI_" : "DO_") + (item as WagoIO).DiscreteBit + " = " + item.EnumIndex.ToString() + ",");
					} else if (item.Device == Devices.DELTA) {
						strTemp.Add("\tDELTA_" + ((item.IoType == IOTypes.Input) ? "DI_" : "DO_") + (item as DeltaIO).IoName + " = " + item.EnumIndex.ToString() + ",");
					}
				}
				strTemp[strTemp.Count - 1] = strTemp[strTemp.Count - 1].Remove(strTemp[strTemp.Count - 1].Length - 1);
				strTemp.Add("}");
				CtFile.WriteFile(dialog.FileName, strTemp);
			}
			dialog.Dispose();
		}

		private void SetEnumIdx(List<ICtIO> ioList) {
			ushort idx = 0;
			foreach (ICtIO item in ioList) {
				if (item.EnumIndex == 0) {
					item.SetEnumIndex(idx);
				} else {
					idx = item.EnumIndex; //從後面繼續加下去
				}
				idx++;
			}
		}

		private void ResetEnumIdx(List<ICtIO> ioList) {
			ushort idx = 0;
			List<ICtIO> aceIN = ioList.FindAll(io => io.Device == Devices.AdeptACE && io.IoType == IOTypes.Input);
			if (aceIN != null && aceIN.Count > 0) {
				aceIN.ForEach(
					io => {
						io.SetEnumIndex(idx);
						idx++;
					}
				);
			}

			List<ICtIO> aceOut = ioList.FindAll(io => io.Device == Devices.AdeptACE && io.IoType == IOTypes.Output);
			if (aceOut != null && aceOut.Count > 0) {
				aceOut.ForEach(
					io => {
						io.SetEnumIndex(idx);
						idx++;
					}
				);
			}

			List<ICtIO> bkfIN = ioList.FindAll(io => io.Device == Devices.Beckhoff && io.IoType == IOTypes.Input);
			if (bkfIN != null && bkfIN.Count > 0) {
				bkfIN.ForEach(
					io => {
						io.SetEnumIndex(idx);
						idx++;
					}
				);
			}

			List<ICtIO> bkfOut = ioList.FindAll(io => io.Device == Devices.Beckhoff && io.IoType == IOTypes.Output);
			if (bkfOut != null && bkfOut.Count > 0) {
				bkfOut.ForEach(
					io => {
						io.SetEnumIndex(idx);
						idx++;
					}
				);
			}

			List<ICtIO> bkfInOut = ioList.FindAll(io => io.Device == Devices.Beckhoff && io.IoType == IOTypes.InOut);
			if (bkfInOut != null && bkfInOut.Count > 0) {
				bkfInOut.ForEach(
					io => {
						io.SetEnumIndex(idx);
						idx++;
					}
				);
			}

			List<ICtIO> wagoIN = ioList.FindAll(io => io.Device == Devices.WAGO && io.IoType == IOTypes.Input);
			if (wagoIN != null && wagoIN.Count > 0) {
				wagoIN.ForEach(
					io => {
						io.SetEnumIndex(idx);
						idx++;
					}
				);
			}

			List<ICtIO> wagoOut = ioList.FindAll(io => io.Device == Devices.WAGO && io.IoType == IOTypes.Output);
			if (wagoOut != null && wagoOut.Count > 0) {
				wagoOut.ForEach(
					io => {
						io.SetEnumIndex(idx);
						idx++;
					}
				);
			}

			List<ICtIO> deltaIN = ioList.FindAll(io => io.Device == Devices.DELTA && io.IoType == IOTypes.Input);
			if (deltaIN != null && deltaIN.Count > 0) {
				deltaIN.ForEach(
					io => {
						io.SetEnumIndex(idx);
						idx++;
					}
				);
			}

			List<ICtIO> deltaOut = ioList.FindAll(io => io.Device == Devices.DELTA && io.IoType == IOTypes.Output);
			if (deltaOut != null && deltaOut.Count > 0) {
				deltaOut.ForEach(
					io => {
						io.SetEnumIndex(idx);
						idx++;
					}
				);
			}
		}
	}
}
