using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using CtLib.Library;

namespace CtLib.Forms.TestPlatform {

    /// <summary>系統相關資訊</summary>
    public partial class SystemInfo : Form {

        List<DriveInfo> mDiskInfo;
        List<LogicalDiskInfo> mLogicalInfo;
        List<PhysicalDiskInfo> mPhysicalInfo;

        /// <summary>建立系統相關資訊</summary>
        public SystemInfo() {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e) {
            string strTemp = "";

            CtApplication.GetLogicalDiskInfo(out mDiskInfo);
            CtApplication.GetLogicalDiskInfo(out mLogicalInfo);
            CtApplication.GetPhysicalDiskInfo(out mPhysicalInfo);

            CtInvoke.ListBoxClear(listBox4);
            BiosInfo bios;
            CtApplication.GetBIOSInfo(out bios);
            CtInvoke.ListBoxAdd(listBox4, "Name = " + bios.Name);
            CtInvoke.ListBoxAdd(listBox4, "Version = " + bios.Version);
            CtInvoke.ListBoxAdd(listBox4, "SerialNumber = " + bios.SerialNumber);

            CtInvoke.ListBoxClear(listBox5);
            CpuInfo cpu;
            CtApplication.GetCPUInfo(out cpu);
            CtInvoke.ListBoxAdd(listBox5, "ID = " + cpu.ID);
            CtInvoke.ListBoxAdd(listBox5, "Name = " + cpu.Name);
            CtInvoke.ListBoxAdd(listBox5, "Manufacturer = " + cpu.Manufacturer);
            CtInvoke.ListBoxAdd(listBox5, "SerialNumber = " + cpu.SerialNumber);
            CtInvoke.ListBoxAdd(listBox5, "Version = " + cpu.Version);

            CtInvoke.ListBoxClear(listBox6);
            MotherBoardInfo mb;
            CtApplication.GetMBInfo(out mb);
            CtInvoke.ListBoxAdd(listBox6, "Product = " + mb.Product);
            CtInvoke.ListBoxAdd(listBox6, "Manufacturer = " + mb.Manufacturer);
            CtInvoke.ListBoxAdd(listBox6, "SerialNumber = " + mb.SerialNumber);

            CtInvoke.ListBoxClear(listBox1);
            Dictionary<string, string> logicDisk;
            CtApplication.GetLogicalDiskSerial(out logicDisk);
            foreach (KeyValuePair<string, string> item in logicDisk) {
                strTemp = "[" + item.Key + "] " + item.Value;
                CtInvoke.ListBoxAdd(listBox1, strTemp);
            }

            CtInvoke.ListBoxClear(listBox2);
            Dictionary<string, string> physDisk;
            CtApplication.GetPhysicalDiskSerial(out physDisk);
            foreach (KeyValuePair<string, string> item in physDisk) {
                strTemp = "[" + item.Key + "] " + item.Value;
                CtInvoke.ListBoxAdd(listBox2, strTemp);
            }

            CtInvoke.ListBoxClear(listBox3);
            PerformanceInfo pmInfo;
            CtApplication.GetPerformance(out pmInfo);
            CtInvoke.ListBoxAdd(listBox3, "CommitLimit = " + pmInfo.CommitLimit.ToInt64().ToString());
            CtInvoke.ListBoxAdd(listBox3, "CommitPeak = " + pmInfo.CommitPeak.ToInt64().ToString());
            CtInvoke.ListBoxAdd(listBox3, "CommitTotal = " + pmInfo.CommitTotal.ToInt64().ToString());
            CtInvoke.ListBoxAdd(listBox3, "KernelNonPaged = " + pmInfo.KernelNonPaged.ToInt64().ToString());
            CtInvoke.ListBoxAdd(listBox3, "KernelPaged = " + pmInfo.KernelPaged.ToInt64().ToString());
            CtInvoke.ListBoxAdd(listBox3, "KernelTotal = " + pmInfo.KernelTotal.ToInt64().ToString());
            CtInvoke.ListBoxAdd(listBox3, "PageSize = " + pmInfo.PageSize.ToInt64().ToString());
            CtInvoke.ListBoxAdd(listBox3, "PhysicalAvailable = " + pmInfo.PhysicalAvailable.ToInt64().ToString());
            CtInvoke.ListBoxAdd(listBox3, "PhysicalTotal = " + pmInfo.PhysicalTotal.ToInt64().ToString());
            CtInvoke.ListBoxAdd(listBox3, "HandlesCount = " + pmInfo.HandlesCount.ToString());
            CtInvoke.ListBoxAdd(listBox3, "ProcessCount = " + pmInfo.ProcessCount.ToString());
            CtInvoke.ListBoxAdd(listBox3, "ThreadCount = " + pmInfo.ThreadCount.ToString());
            CtInvoke.ListBoxAdd(listBox3, "SystemCache = " + pmInfo.SystemCache.ToString());

            CtInvoke.ListBoxClear(listBox7);
            OsInfo osInfo;
            CtApplication.GetOSInfo(out osInfo);
            CtInvoke.ListBoxAdd(listBox7, "Caption = " + osInfo.Caption);
            CtInvoke.ListBoxAdd(listBox7, "Name = " + osInfo.Name);
            CtInvoke.ListBoxAdd(listBox7, "Manufacturer = " + osInfo.Manufacturer);
            CtInvoke.ListBoxAdd(listBox7, "Version = " + osInfo.Version);
            CtInvoke.ListBoxAdd(listBox7, "WindowsDirectory = " + osInfo.WindowsDirectory);
        }

        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e) {
            if (listBox1.SelectedIndex > -1) {
                /*-- 取得單一磁碟序號 --*/
                //string[] strSplit = listBox1.SelectedItem.ToString().Split(CtConst.CHR_BRACKET, StringSplitOptions.RemoveEmptyEntries );
                //string temp = CtApplication.GetLogicalDiskSerial(strSplit[0]);
                //MessageBox.Show(temp);

                /*-- 使用 System.IO.DriveInfo 讀取相關訊息 --*/
                //string[] strSplit = listBox1.SelectedItem.ToString().Split(CtConst.CHR_BRACKET, StringSplitOptions.RemoveEmptyEntries );
                //DriveInfo info = mDiskInfo.Find(disk=>disk.Name.Contains(strSplit[0]));
                //if (info != null) {
                //    string temp = "";
                //    if (info.IsReady) {
                //        temp += "FreeSpace = " + info.AvailableFreeSpace.ToString() + " bytes" + CtConst.NewLine;
                //        temp += "TotalSize = " + info.TotalSize.ToString() + " bytes" + CtConst.NewLine;

                //        temp += "Name = " + info.Name + CtConst.NewLine;
                //        temp += "Format = " + info.DriveFormat + CtConst.NewLine;
                //        temp += "Label = " + info.VolumeLabel + CtConst.NewLine;

                //        temp += "DriveType = " + info.DriveType.ToString() + CtConst.NewLine;

                //        temp += "Root = " + info.RootDirectory.Name;
                //    }
                //    MessageBox.Show(temp);
                //}

                /*-- 使用 CtApplication.LogicalInfo 讀取相關訊息 --*/
                string[] strSplit = listBox1.SelectedItem.ToString().Split(CtConst.CHR_BRACKET, StringSplitOptions.RemoveEmptyEntries);
                LogicalDiskInfo info = mLogicalInfo.Find(disk => disk.Name.Contains(strSplit[0]));

                string temp = "";
                temp += "Name = " + info.Name + CtConst.NewLine;
                temp += "SystemName = " + info.SystemName + CtConst.NewLine;
                temp += "VolumeName = " + info.VolumeName + CtConst.NewLine;
                temp += "VolumeSerialNumber = " + info.VolumeSerialNumber + CtConst.NewLine;
                temp += "Size = " + info.Size.ToString() + CtConst.NewLine;
                temp += "FreeSpace = " + info.FreeSpace.ToString() + CtConst.NewLine;
                CtMsgBox.Show("Logical Disk Information", temp);
            }
        }

        private void listBox2_MouseDoubleClick(object sender, MouseEventArgs e) {
            if (listBox2.SelectedIndex > -1) {
                /*-- 取得單一磁碟序號 --*/
                //string temp = CtApplication.GetPhysicalDiskSerial((byte)listBox2.SelectedIndex);
                //MessageBox.Show(temp);

                /*-- 使用 CtApplication.LogicalInfo 讀取相關訊息 --*/
                string[] strSplit = listBox2.SelectedItem.ToString().Split(CtConst.CHR_BRACKET, StringSplitOptions.RemoveEmptyEntries);
                PhysicalDiskInfo info = mPhysicalInfo.Find(disk => disk.Name.Contains(strSplit[0]));

                string temp = "";
                temp += "Name = " + info.Name + CtConst.NewLine;
                temp += "Model = " + info.Model + CtConst.NewLine;
                temp += "Manufacturer = " + info.Manufacturer + CtConst.NewLine;
                temp += "InterfaceType = " + info.InterfaceType + CtConst.NewLine;
                temp += "SerialNumber = " + info.SerialNumber + CtConst.NewLine;
                CtMsgBox.Show("Physical Disk Information", temp);
            }
        }
    }
}
