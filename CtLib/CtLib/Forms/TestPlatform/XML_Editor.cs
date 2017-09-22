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

        List<CtIOData> mIOList = new List<CtIOData>();

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

        private CtIOData FindIO(List<CtIOData> ioData, string id) {
            return ioData.Find(data => {
                if (data.Device == Devices.ADEPT_ACE)
                    return data.IONum.ToString() == id;
                else
                    return data.ID == id;
            });
        }

        private void treeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e) {
            if (e.Node.Level == 1) {
                CtInput input = new CtInput();
                string strTemp = e.Node.Text;
                input.Start(CtInput.InputStyle.TEXT, "修改ID", "請輸入要修改的ID", out strTemp, strTemp);
                if (strTemp != "") {
                    CtIOData ioTemp = FindIO(mIOList, e.Node.Text);
                    if (ioTemp.Device == Devices.ADEPT_ACE) ioTemp.IONum = CtConvert.CInt(strTemp);
                    else ioTemp.ID = strTemp;
                    e.Node.Text = strTemp;
                }

            } else if (e.Node.Level == 2) {
                CtInput input = new CtInput();
                string strTemp = "";
                if (e.Node.Text.StartsWith("註解")) {
                    strTemp = e.Node.Text.Replace("註解: ", "").Trim();
                    input.Start(CtInput.InputStyle.TEXT, "修改註解", "請輸入要修改的註解", out strTemp, strTemp);
                    if (strTemp != "") {
                        e.Node.Text = "註解: " + strTemp;
                        FindIO(mIOList, e.Node.Parent.Text).Comment = strTemp;
                    }
                } else if (e.Node.Text.StartsWith("變數")) {
                    strTemp = e.Node.Text.Replace("變數: ", "").Trim();
                    input.Start(CtInput.InputStyle.TEXT, "修改變數", "請輸入要修改相對應於 Beckhoff Ads 系統之變數名稱", out strTemp, strTemp);
                    if (strTemp != "") {
                        e.Node.Text = "變數: " + strTemp;
                        FindIO(mIOList, e.Node.Parent.Text).Variable = strTemp;
                    }
                } else if (e.Node.Text.StartsWith("索引")) {
                    strTemp = e.Node.Text.Replace("索引: ", "").Trim();
                    input.Start(CtInput.InputStyle.TEXT, "修改索引", "請輸入要修改於陣列中之索引值\r\n\"-1\" 表示忽略", out strTemp, strTemp);
                    if (strTemp != "") {
                        e.Node.Text = "索引: " + strTemp;
                        FindIO(mIOList, e.Node.Parent.Text).Index = CtConvert.CInt(strTemp);
                    }
                } else if (e.Node.Text.StartsWith("編號")) {
                    strTemp = e.Node.Text.Replace("編號: ", "").Trim();
                    input.Start(CtInput.InputStyle.TEXT, "修改編號", "請輸入要修改於 FC1/FC2/FC5/FC15 之編號", out strTemp, strTemp);
                    if (strTemp != "") {
                        e.Node.Text = "編號: " + strTemp;
                        FindIO(mIOList, e.Node.Parent.Text).IONum = CtConvert.CInt(strTemp);
                    }
                } else if (e.Node.Text.StartsWith("暫存器(Bit)")) {
                    strTemp = e.Node.Text.Replace("暫存器(Bit): ", "").Trim();
                    input.Start(CtInput.InputStyle.TEXT, "修改暫存器(Bit)", "請輸入要修改於 Register 之位元索引數", out strTemp, strTemp);
                    if (strTemp != "") {
                        e.Node.Text = "暫存器(Bit): " + strTemp;
                        FindIO(mIOList, e.Node.Parent.Text).RegBit = CtConvert.CByte(strTemp);
                    }
                } else if (e.Node.Text.StartsWith("暫存器")) {
                    strTemp = e.Node.Text.Replace("暫存器: ", "").Trim();
                    input.Start(CtInput.InputStyle.TEXT, "修改暫存器", "請輸入要修改於 FC3/FC4/FC6/FC16 之 Register 編號", out strTemp, strTemp);
                    if (strTemp != "") {
                        e.Node.Text = "暫存器: " + strTemp;
                        FindIO(mIOList, e.Node.Parent.Text).RegNum = CtConvert.CUShort(strTemp);
                    }
                } else if (e.Node.Text.StartsWith("Enum")) {
                    strTemp = e.Node.Text.Replace("Enum: ", "").Trim();
                    input.Start(CtInput.InputStyle.TEXT, "修改Enum數值", "請輸入要修改相對於 Enum 裡之數值", out strTemp, strTemp);
                    if (strTemp != "") {
                        e.Node.Text = "Enum: " + strTemp;
                        FindIO(mIOList, e.Node.Parent.Text).EnumIdx = CtConvert.CUShort(strTemp);
                    }
                }
            }
        }

        private void button3_Click(object sender, EventArgs e) {

            /*-- 排序 --*/
            mIOList.Sort();

            if (checkBox1.Checked) {
                if (CtMsgBox.Show("Enumerations", "是否要重設所有 Enum?", MsgBoxButton.YES_NO, MsgBoxStyle.QUESTION) == MsgBoxButton.YES)
                    ResetEnumIdx(mIOList);
                else
                    SetEnumIdx(mIOList);
            }
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.DefaultExt = "xml";
            dialog.Filter = "XML|*.xml";
            dialog.InitialDirectory = CtDefaultPath.GetPath(SystemPath.CONFIG);
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
            CtInput input = new CtInput();

            /*-- 輸入 IO 編號 --*/
            string strIONum = "";
            stt = input.Start(CtInput.InputStyle.TEXT, "I/O 編號", "請輸入此 I/O 編號", out strIONum);
            if (stt == Stat.WN_SYS_USRCNC) return;

            /*-- 輸入註解 --*/
            string strComment = "";
            stt = input.Start(CtInput.InputStyle.TEXT, "I/O 註解", "請輸入註解", out strComment);
            if (stt == Stat.WN_SYS_USRCNC) return;

            /*-- 加到 TreeView --*/
            int ioNum;
            if (int.TryParse(strIONum, out ioNum)) {
                TreeNode ADEPT_ACE;
                if (ioNum > 1999) {
                    ADEPT_ACE = treeView.Nodes[2].Nodes.Add(strIONum);
                } else if (ioNum > 999) {
                    ADEPT_ACE = treeView.Nodes[0].Nodes.Add(strIONum);
                } else {
                    ADEPT_ACE = treeView.Nodes[1].Nodes.Add(strIONum);
                }
                ADEPT_ACE.Nodes.Add("註解: " + strComment);

                /*-- 加到 mIOLisy --*/
                mIOList.Add(new CtIOData(ioNum, strComment));
            } else CtMsgBox.Show("錯誤", "I/O 編號錯誤，請重新新增", MsgBoxButton.OK, MsgBoxStyle.ERROR);

            input.Dispose();
        }

        private void beckhoffToolStripMenuItem_Click(object sender, EventArgs e) {
            Stat stt = Stat.SUCCESS;
            CtInput input = new CtInput();

            /*-- 輸入 IO 類型 --*/
            string strType = "";
            stt = input.Start(CtInput.InputStyle.COMBOBOX_LIST, "I/O 類型", "請選擇類型", out strType, new List<string> { "Input", "Output" });
            if (stt == Stat.WN_SYS_USRCNC) return;

            /*-- 輸入 IO 名稱 --*/
            string strID = "";
            stt = input.Start(CtInput.InputStyle.TEXT, "I/O 代號", "請輸入此 I/O 代號。如 \"Y0013\"", out strID);
            if (stt == Stat.WN_SYS_USRCNC) return;

            /*-- 輸入 IO 變數 --*/
            string strVar = "";
            stt = input.Start(CtInput.InputStyle.TEXT, "變數", "請輸入此 I/O 相對應的變數。如 pQ_ClampOpen", out strVar);
            if (stt == Stat.WN_SYS_USRCNC) return;

            /*-- 輸入 IO 索引 --*/
            string strIdx = "";
            stt = input.Start(CtInput.InputStyle.TEXT, "陣列索引值", "請輸入此 I/O 相對應於陣列之索引\r\n\"-1\" 表示忽略", out strIdx, "-1");
            if (stt == Stat.WN_SYS_USRCNC) return;

            /*-- 輸入註解 --*/
            string strComment = "";
            stt = input.Start(CtInput.InputStyle.TEXT, "I/O 註解", "請輸入註解", out strComment);
            if (stt == Stat.WN_SYS_USRCNC) return;

            /*-- 加到 TreeView --*/
            TreeNode beckhoff;
            IOTypes ioType = IOTypes.INPUT;
            if (strType == "Input") {
                ioType = IOTypes.INPUT;
                beckhoff = treeView.Nodes[3].Nodes.Add(strID);
            } else {
                ioType = IOTypes.OUTPUT;
                beckhoff = treeView.Nodes[4].Nodes.Add(strID);
            }

            beckhoff.Nodes.Add("變數: " + strVar);
            beckhoff.Nodes.Add("索引: " + strIdx);
            beckhoff.Nodes.Add("註解: " + strComment);

            /*-- 加到 mIOList --*/
            mIOList.Add(new CtIOData(strID, ioType, strVar, strComment, 0, CtConvert.CInt(strIdx)));

            input.Dispose();
        }

        private void wagoToolStripMenuItem_Click(object sender, EventArgs e) {
            Stat stt = Stat.SUCCESS;
            CtInput input = new CtInput();

            /*-- 輸入 IO 類型 --*/
            string strType = "";
            stt = input.Start(CtInput.InputStyle.COMBOBOX_LIST, "I/O 類型", "請選擇類型", out strType, new List<string> { "Input", "Output" });
            if (stt == Stat.WN_SYS_USRCNC) return;

            /*-- 輸入 IO 名稱 --*/
            string strID = "";
            stt = input.Start(CtInput.InputStyle.TEXT, "I/O 代號", "請輸入此 I/O 代號。如 \"00103\"", out strID);
            if (stt == Stat.WN_SYS_USRCNC) return;

            /*-- 輸入 IO 編號 --*/
            string strIONum = "";
            stt = input.Start(CtInput.InputStyle.TEXT, "I/O 編號", "請輸入 FC1/FC2/FC5/FC15 編號", out strIONum);
            if (stt == Stat.WN_SYS_USRCNC) return;

            /*-- 輸入 IO Reg Num --*/
            string strRegNum = "";
            stt = input.Start(CtInput.InputStyle.TEXT, "Register 編號", "請輸入此 I/O 相對應的暫存器編號", out strRegNum);
            if (stt == Stat.WN_SYS_USRCNC) return;

            /*-- 輸入 IO Reg Bit --*/
            string strRegBit = "";
            stt = input.Start(CtInput.InputStyle.TEXT, "Register 索引值", "請輸入此 I/O 相對應於 Register 的位元索引", out strRegBit, "0");
            if (stt == Stat.WN_SYS_USRCNC) return;

            /*-- 輸入註解 --*/
            string strComment = "";
            stt = input.Start(CtInput.InputStyle.TEXT, "I/O 註解", "請輸入註解", out strComment);
            if (stt == Stat.WN_SYS_USRCNC) return;

            /*-- 加到 TreeView --*/
            TreeNode wago;
            IOTypes ioType = IOTypes.INPUT;
            if (strType == "Input") {
                ioType = IOTypes.INPUT;
                wago = treeView.Nodes[5].Nodes.Add(strID);
            } else {
                ioType = IOTypes.OUTPUT;
                wago = treeView.Nodes[6].Nodes.Add(strID);
            }

            wago.Nodes.Add("編號: " + strIONum);
            wago.Nodes.Add("暫存器: " + strRegNum);
            wago.Nodes.Add("暫存器(Bit): " + strRegBit);
            wago.Nodes.Add("註解: " + strComment);

            /*-- 加到 mIOLisy --*/
            mIOList.Add(new CtIOData(strID, ioType, CtConvert.CInt(strIONum), CtConvert.CUShort(strRegNum), CtConvert.CByte(strRegBit), strComment));

            input.Dispose();
        }

        private void button4_Click(object sender, EventArgs e) {
            TreeNode node = null;
            if (treeView.SelectedNode.Level == 1) {
                node = treeView.SelectedNode;
            } else if (treeView.SelectedNode.Level == 2) {
                node = treeView.SelectedNode.Parent;
            }

            if (node != null) {
                CtIOData ioTemp = FindIO(mIOList, node.Text);
                switch (ioTemp.Device) {
                    case Devices.ADEPT_ACE:
                        mIOList.Add(new CtIOData(ioTemp.IONum, ioTemp.Comment, ioTemp.EnumIdx, ioTemp.State));
                        break;
                    case Devices.BECKHOFF_PLC:
                        mIOList.Add(new CtIOData(ioTemp.ID, ioTemp.Type, ioTemp.Variable, ioTemp.Comment, ioTemp.EnumIdx, ioTemp.Index, ioTemp.State));
                        break;
                    case Devices.WAGO:
                        mIOList.Add(new CtIOData(ioTemp.ID, ioTemp.Type, ioTemp.IONum, ioTemp.RegNum, ioTemp.RegBit, ioTemp.Comment, ioTemp.EnumIdx, ioTemp.State));
                        break;
                }

                node.Parent.Nodes.Add(node.Clone() as TreeNode);

            }
        }

        private void iODataToolStripMenuItem_Click(object sender, EventArgs e) {
            mIOList.Clear();
            treeView.Nodes.Clear();

            treeView.Nodes.Add("ADEPT_ACE Digital Input");
            treeView.Nodes.Add("ADEPT_ACE Digital Output");
            treeView.Nodes.Add("ADEPT_ACE Software I/O");
            treeView.Nodes.Add("Beckhoff Input");
            treeView.Nodes.Add("Beckhoff Output");
            treeView.Nodes.Add("Wago Input");
            treeView.Nodes.Add("Wago Output");
        }

        private void iOXMLFileToolStripMenuItem_Click(object sender, EventArgs e) {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.DefaultExt = "xml";
            dialog.Filter = "XML|*.xml";
            dialog.InitialDirectory = CtDefaultPath.GetPath(SystemPath.CONFIG);
            if (dialog.ShowDialog() == DialogResult.OK) {
                treeView.Nodes.Clear();
                treeView.Nodes.Add("ADEPT_ACE Digital Input");
                treeView.Nodes.Add("ADEPT_ACE Digital Output");
                treeView.Nodes.Add("ADEPT_ACE Software I/O");
                treeView.Nodes.Add("Beckhoff Input");
                treeView.Nodes.Add("Beckhoff Output");
                treeView.Nodes.Add("Wago Input");
                treeView.Nodes.Add("Wago Output");
                treeView.Nodes.Add("Delta Input");
                treeView.Nodes.Add("Delta Output");

                mIOList.Clear();
                CtIO.LoadFromXML(dialog.FileName, out mIOList);

                if (mIOList != null) {
                    foreach (CtIOData item in mIOList) {
                        switch (item.Device) {
                            case Devices.ADEPT_ACE:
                                if (item.Type == IOTypes.INPUT) {
                                    TreeNode ADEPT_ACEIn = treeView.Nodes[0].Nodes.Add(item.IONum.ToString());
                                    ADEPT_ACEIn.Nodes.Add("註解: " + item.Comment);
                                    ADEPT_ACEIn.Nodes.Add("Enum: " + item.EnumIdx.ToString());
                                } else if (item.Type == IOTypes.OUTPUT) {
                                    TreeNode ADEPT_ACEOut = treeView.Nodes[1].Nodes.Add(item.IONum.ToString());
                                    ADEPT_ACEOut.Nodes.Add("註解: " + item.Comment);
                                    ADEPT_ACEOut.Nodes.Add("Enum: " + item.EnumIdx.ToString());
                                } else {
                                    TreeNode ADEPT_ACEOut = treeView.Nodes[2].Nodes.Add(item.IONum.ToString());
                                    ADEPT_ACEOut.Nodes.Add("註解: " + item.Comment);
                                    ADEPT_ACEOut.Nodes.Add("Enum: " + item.EnumIdx.ToString());
                                }
                                break;
                            case Devices.BECKHOFF_PLC:
                                if (item.Type == IOTypes.INPUT) {
                                    TreeNode beckhoffIn = treeView.Nodes[3].Nodes.Add(item.ID);
                                    beckhoffIn.Nodes.Add("變數: " + item.Variable);
                                    beckhoffIn.Nodes.Add("索引: " + item.Index.ToString());
                                    beckhoffIn.Nodes.Add("註解: " + item.Comment);
                                    beckhoffIn.Nodes.Add("Enum: " + item.EnumIdx.ToString());
                                } else {
                                    TreeNode beckhoffOut = treeView.Nodes[4].Nodes.Add(item.ID);
                                    beckhoffOut.Nodes.Add("變數: " + item.Variable);
                                    beckhoffOut.Nodes.Add("索引: " + item.Index.ToString());
                                    beckhoffOut.Nodes.Add("註解: " + item.Comment);
                                    beckhoffOut.Nodes.Add("Enum: " + item.EnumIdx.ToString());
                                }
                                break;
                            case Devices.DELTA:
                                if (item.Type == IOTypes.INPUT) {
                                    TreeNode beckhoffIn = treeView.Nodes[7].Nodes.Add(item.ID);
                                    beckhoffIn.Nodes.Add("變數: " + item.Variable);
                                    beckhoffIn.Nodes.Add("註解: " + item.Comment);
                                    beckhoffIn.Nodes.Add("Enum: " + item.EnumIdx.ToString());
                                } else {
                                    TreeNode beckhoffOut = treeView.Nodes[8].Nodes.Add(item.ID);
                                    beckhoffOut.Nodes.Add("變數: " + item.Variable);
                                    beckhoffOut.Nodes.Add("註解: " + item.Comment);
                                    beckhoffOut.Nodes.Add("Enum: " + item.EnumIdx.ToString());
                                }
                                break;
                            case Devices.WAGO:
                                if (item.Type == IOTypes.INPUT) {
                                    TreeNode wagoIn = treeView.Nodes[5].Nodes.Add(item.ID);
                                    wagoIn.Nodes.Add("編號: " + item.IONum);
                                    wagoIn.Nodes.Add("暫存器: " + item.RegNum.ToString());
                                    wagoIn.Nodes.Add("暫存器(Bit): " + item.RegBit.ToString());
                                    wagoIn.Nodes.Add("註解: " + item.Comment);
                                    wagoIn.Nodes.Add("Enum: " + item.EnumIdx.ToString());
                                } else {
                                    TreeNode wagoOut = treeView.Nodes[6].Nodes.Add(item.ID);
                                    wagoOut.Nodes.Add("編號: " + item.IONum);
                                    wagoOut.Nodes.Add("暫存器: " + item.RegNum.ToString());
                                    wagoOut.Nodes.Add("暫存器(Bit): " + item.RegBit.ToString());
                                    wagoOut.Nodes.Add("註解: " + item.Comment);
                                    wagoOut.Nodes.Add("Enum: " + item.EnumIdx.ToString());
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
                if (CtMsgBox.Show("Enumerations", "是否要重設所有 Enum?", MsgBoxButton.YES_NO, MsgBoxStyle.QUESTION) == MsgBoxButton.YES)
                    ResetEnumIdx(mIOList);
                else
                    SetEnumIdx(mIOList);
            }
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.InitialDirectory = CtDefaultPath.GetPath(SystemPath.CONFIG);
            if (dialog.ShowDialog() == DialogResult.OK) {
                List<string> strTemp = new List<string> { "enum Name : ushort {" };
                foreach (CtIOData item in mIOList) {
                    strTemp.Add("\t///<summary>" + item.Comment + "</summary>");
                    if (item.Device == Devices.ADEPT_ACE) {
                        strTemp.Add("\tACE_" + ((item.Type == IOTypes.INPUT) ? "DI_" : "DO_") + item.IONum.ToString("0000") + " = " + item.EnumIdx.ToString() + ",");
                    } else if (item.Device == Devices.BECKHOFF_PLC) {
                        strTemp.Add("\tBKF_" + ((item.Type == IOTypes.INPUT) ? "DI_" : "DO_") + item.ID + " = " + item.EnumIdx.ToString() + ",");
                    } else if (item.Device == Devices.WAGO) {
                        strTemp.Add("\tWGO_" + ((item.Type == IOTypes.INPUT) ? "DI_" : "DO_") + item.ID + " = " + item.EnumIdx.ToString() + ",");
                    } else if (item.Device == Devices.DELTA) {
                        strTemp.Add("\tDELTA_" + ((item.Type == IOTypes.INPUT) ? "DI_" : "DO_") + item.ID + " = " + item.EnumIdx.ToString() + ",");
                    }
                }
                strTemp[strTemp.Count - 1] = strTemp[strTemp.Count - 1].Remove(strTemp[strTemp.Count - 1].Length - 1);
                strTemp.Add("}");
                CtFile.WriteFile(dialog.FileName, strTemp);
            }
            dialog.Dispose();
        }

        private void SetEnumIdx(List<CtIOData> ioList) {
            ushort idx = 0;
            foreach (CtIOData item in ioList) {
                if (item.EnumIdx == 0) {
                    item.EnumIdx = idx;
                } else {
                    idx = item.EnumIdx; //從後面繼續加下去
                }
                idx++;
            }
        }

        private void ResetEnumIdx(List<CtIOData> ioList) {
            ushort idx = 0;
            List<CtIOData> aceIN = ioList.FindAll(io => io.Device == Devices.ADEPT_ACE && io.Type == IOTypes.INPUT);
            if (aceIN != null && aceIN.Count > 0) {
                aceIN.ForEach(
                    io => {
                        io.EnumIdx = idx;
                        idx++;
                    }
                );
            }

            List<CtIOData> aceOut = ioList.FindAll(io => io.Device == Devices.ADEPT_ACE && io.Type == IOTypes.OUTPUT);
            if (aceOut != null && aceOut.Count > 0) {
                aceOut.ForEach(
                    io => {
                        io.EnumIdx = idx;
                        idx++;
                    }
                );
            }

            List<CtIOData> bkfIN = ioList.FindAll(io => io.Device == Devices.BECKHOFF_PLC && io.Type == IOTypes.INPUT);
            if (bkfIN != null && bkfIN.Count > 0) {
                bkfIN.ForEach(
                    io => {
                        io.EnumIdx = idx;
                        idx++;
                    }
                );
            }

            List<CtIOData> bkfOut = ioList.FindAll(io => io.Device == Devices.BECKHOFF_PLC && io.Type == IOTypes.OUTPUT);
            if (bkfOut != null && bkfOut.Count > 0) {
                bkfOut.ForEach(
                    io => {
                        io.EnumIdx = idx;
                        idx++;
                    }
                );
            }

            List<CtIOData> wagoIN = ioList.FindAll(io => io.Device == Devices.WAGO && io.Type == IOTypes.INPUT);
            if (wagoIN != null && wagoIN.Count > 0) {
                wagoIN.ForEach(
                    io => {
                        io.EnumIdx = idx;
                        idx++;
                    }
                );
            }

            List<CtIOData> wagoOut = ioList.FindAll(io => io.Device == Devices.WAGO && io.Type == IOTypes.OUTPUT);
            if (wagoOut != null && wagoOut.Count > 0) {
                wagoOut.ForEach(
                    io => {
                        io.EnumIdx = idx;
                        idx++;
                    }
                );
            }

            List<CtIOData> deltaIN = ioList.FindAll(io => io.Device == Devices.DELTA && io.Type == IOTypes.INPUT);
            if (deltaIN != null && deltaIN.Count > 0) {
                deltaIN.ForEach(
                    io => {
                        io.EnumIdx = idx;
                        idx++;
                    }
                );
            }

            List<CtIOData> deltaOut = ioList.FindAll(io => io.Device == Devices.DELTA && io.Type == IOTypes.OUTPUT);
            if (deltaOut != null && deltaOut.Count > 0) {
                deltaOut.ForEach(
                    io => {
                        io.EnumIdx = idx;
                        idx++;
                    }
                );
            }
        }
    }
}
