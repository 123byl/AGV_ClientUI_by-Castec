using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using CtLib.Library;

namespace CtLib.Forms {

    /// <summary>
    /// 錯誤之歷史紀錄，將自動搜尋Log資料夾
    /// </summary>
    public partial class CtErrorHistory : Form {

        #region Declaration - Enumeration
        /// <summary>記錄檔類型</summary>
        private enum LogType {
            /// <summary>Alarm Log</summary>
            ALARM,
            /// <summary>Trace Log</summary>
            TRACE,
            /// <summary>System Status Report</summary>
            SYSTEM
        }
        
        #endregion

        #region Declaration - Members

        /// <summary>顯示Alarm Log</summary>
        private bool mShowAlarmLog = true;
        /// <summary>顯示Trace Log</summary>
        private bool mShowTraceLog = true;
        /// <summary>顯示Status Report</summary>
        private bool mShowErrorLog = true;
        
        #endregion

        #region Function - Constructor

        /// <summary>建構歷史紀錄視窗，請帶入CtProject已取得系統路徑</summary>
        public CtErrorHistory() {
            InitializeComponent();

            /*-- 初始化DateTimePicker和ComboBox --*/
            CtInvoke.DateTimePickerValue(tpickStart, DateTime.Now);
            CtInvoke.DateTimePickerValue(tpickEnd, DateTime.Now);
            CtInvoke.RadioButtonChecked(rdbTime_Today, true);
            CtInvoke.ComboBoxSelectedIndex(cbType, 0);
            CtInvoke.ComboBoxSelectedIndex(cbLv, 0);
        }
        
        #endregion

        #region Function - Core

        /// <summary>搜尋Log資料夾裡時間相符之檔案路徑</summary>
        /// <param name="path">Log資料夾，預設為D:\CASTEC\Log</param>
        /// <param name="startTime">搜尋檔案之起始時間</param>
        /// <param name="endTime">搜尋檔案之結算時間</param>
        /// <returns>路徑與類型之Dictionary</returns>
        private Dictionary<string, LogType> SearchLog(string path, DateTime startTime, DateTime endTime) {
            Dictionary<string, LogType> pathCollection = new Dictionary<string, LogType>();
            DateTime fileTime;
            try {
                foreach (string file in Directory.GetFiles(path, "*.log", SearchOption.AllDirectories)) {
                    if ((file.Contains(CtConst.FILE_ALARMLOG)) && (mShowAlarmLog)) {
                        fileTime = File.GetLastWriteTime(file);
                        if ((startTime.Date <= fileTime.Date) && (fileTime.Date <= endTime.Date)) pathCollection.Add(file, LogType.ALARM);
                    } else if ((file.Contains(CtConst.FILE_REPORTLOG)) && (mShowErrorLog)) {
                        fileTime = File.GetLastWriteTime(file);
                        if ((startTime.Date <= fileTime.Date) && (fileTime.Date <= endTime.Date)) pathCollection.Add(file, LogType.SYSTEM);
                    } else if ((file.Contains(CtConst.FILE_TRACELOG)) && (mShowTraceLog)) {
                        fileTime = File.GetLastWriteTime(file);
                        if ((startTime.Date <= fileTime.Date) && (fileTime.Date <= endTime.Date)) pathCollection.Add(file, LogType.TRACE);
                    }
                }
            } catch (Exception ex) {
                CtStatus.Report(Stat.ER_SYSTEM, ex);
            }
            return pathCollection;
        }

        /// <summary>載入Log訊息</summary>
        /// <param name="kind">Log種類</param>
        /// <param name="path">路徑</param>
        /// <returns>Statuc Code</returns>
        private Stat LoadLog(LogType kind, string path) {
            Stat stt = Stat.SUCCESS;
            try {
                if (CtFile.IsFileExist(path)) {
                    List<string> strDocument = CtFile.ReadFile(path);
                    List<string> strDGV = new List<string>();
                    string[] strSplit;
                    foreach (string item in strDocument) {
                        switch (kind) {
                            case LogType.ALARM:
                                strSplit = item.Split(new char[] { '[', ']', '{', '}', '(', ')' }, StringSplitOptions.RemoveEmptyEntries);
                                strDGV.Clear();
                                strDGV.Add(File.GetCreationTime(path).ToString("yyyy/MM/dd") + " " + strSplit[0]);
                                strDGV.Add("Alarm");
                                strDGV.Add(strSplit[2]);
                                strDGV.Add(strSplit[6]);
                                strDGV.Add(strSplit[7].Replace("-", "").Trim());
                                break;
                            case LogType.TRACE:
                                strSplit = item.Split(CtConst.CHR_BRACKET, StringSplitOptions.RemoveEmptyEntries);
                                strDGV.Clear();
                                strDGV.Add(File.GetCreationTime(path).ToString("yyyy/MM/dd") + " " + strSplit[0]);
                                strDGV.Add("Trace");
                                strDGV.Add("");
                                strDGV.Add(strSplit[2]);
                                strDGV.Add(strSplit[3].Replace("-", "").Trim());
                                break;
                            case LogType.SYSTEM:
                                strSplit = item.Split(new char[] { '[', ']', '<', '>', '(', ')' }, StringSplitOptions.RemoveEmptyEntries);
                                strDGV.Clear();
                                strDGV.Add(File.GetCreationTime(path).ToString("yyyy/MM/dd") + " " + strSplit[0]);
                                strDGV.Add("System");
                                strDGV.Add(strSplit[2]);
                                strDGV.Add(strSplit[6]);
                                strDGV.Add(strSplit[7].Trim());
                                break;
                        }
                        CtInvoke.DataGridViewAddRow(dgvMsg, strDGV, false, false);
                    }
                }
            } catch (Exception ex) {
                stt = Stat.ER_SYSTEM;
                CtStatus.Report(stt, ex);
            }
            return stt;
        } 
        #endregion

        #region Function - Interface Event
        private void rdbTime_Today_CheckedChanged(object sender, EventArgs e) {
            if (rdbTime_Today.Checked) {
                /*-- 將其他的RadioButton消掉 --*/
                CtInvoke.RadioButtonChecked(rdbTime_Today, true);
                CtInvoke.RadioButtonChecked(rdbTime_Week, false);
                CtInvoke.RadioButtonChecked(rdbTime_Month, false);
                CtInvoke.RadioButtonChecked(rdbTime_Custom, false);

                /*-- 更改時間 --*/
                CtInvoke.DateTimePickerValue(tpickStart, DateTime.Now);
                CtInvoke.DateTimePickerValue(tpickEnd, DateTime.Now);

                /*-- 更改Enable --*/
                CtInvoke.DateTimePickerEnable(tpickStart, false);
                CtInvoke.DateTimePickerEnable(tpickEnd, false);
            }
        }

        private void rdbTime_Week_CheckedChanged(object sender, EventArgs e) {
            if (rdbTime_Week.Checked) {
                /*-- 將其他的RadioButton消掉 --*/
                CtInvoke.RadioButtonChecked(rdbTime_Today, false);
                CtInvoke.RadioButtonChecked(rdbTime_Week, true);
                CtInvoke.RadioButtonChecked(rdbTime_Month, false);
                CtInvoke.RadioButtonChecked(rdbTime_Custom, false);

                /*-- 更改時間 --*/
                CtInvoke.DateTimePickerValue(tpickStart, DateTime.Now.AddDays(-7));
                CtInvoke.DateTimePickerValue(tpickEnd, DateTime.Now);

                /*-- 更改Enable --*/
                CtInvoke.DateTimePickerEnable(tpickStart, false);
                CtInvoke.DateTimePickerEnable(tpickEnd, false);
            }
        }

        private void rdbTime_Month_CheckedChanged(object sender, EventArgs e) {
            if (rdbTime_Month.Checked) {
                /*-- 將其他的RadioButton消掉 --*/
                CtInvoke.RadioButtonChecked(rdbTime_Today, false);
                CtInvoke.RadioButtonChecked(rdbTime_Week, false);
                CtInvoke.RadioButtonChecked(rdbTime_Month, true);
                CtInvoke.RadioButtonChecked(rdbTime_Custom, false);

                /*-- 更改時間 --*/
                CtInvoke.DateTimePickerValue(tpickStart, DateTime.Now.AddMonths(-1));
                CtInvoke.DateTimePickerValue(tpickEnd, DateTime.Now);

                /*-- 更改Enable --*/
                CtInvoke.DateTimePickerEnable(tpickStart, false);
                CtInvoke.DateTimePickerEnable(tpickEnd, false);
            }
        }

        private void rdbTime_Custom_CheckedChanged(object sender, EventArgs e) {
            if (rdbTime_Custom.Checked) {
                /*-- 將其他的RadioButton消掉 --*/
                CtInvoke.RadioButtonChecked(rdbTime_Today, false);
                CtInvoke.RadioButtonChecked(rdbTime_Week, false);
                CtInvoke.RadioButtonChecked(rdbTime_Month, false);
                CtInvoke.RadioButtonChecked(rdbTime_Custom, true);

                /*-- 更改Enable --*/
                CtInvoke.DateTimePickerEnable(tpickStart, true);
                CtInvoke.DateTimePickerEnable(tpickEnd, true);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e) {
            CtInvoke.DataGridViewClear(dgvMsg);

            /*-- 搜尋Log --*/
            Dictionary<string, LogType> logs = SearchLog(CtDefaultPath.GetPath(SystemPath.LOG), tpickStart.Value, tpickEnd.Value);

            foreach (KeyValuePair<string, LogType> item in logs) {
                LoadLog(item.Value, item.Key);
            }
        }

        private void cbType_SelectedIndexChanged(object sender, EventArgs e) {
            switch (cbType.SelectedIndex) {
                case 0:
                    mShowAlarmLog = true;
                    mShowErrorLog = true;
                    mShowTraceLog = true;
                    break;
                case 1:
                    mShowAlarmLog = false;
                    mShowErrorLog = false;
                    mShowTraceLog = true;
                    break;
                case 2:
                    mShowAlarmLog = true;
                    mShowErrorLog = false;
                    mShowTraceLog = false;
                    break;
                case 3:
                    mShowAlarmLog = false;
                    mShowErrorLog = true;
                    mShowTraceLog = false;
                    break;
            }
        }

        #endregion

    }
}
