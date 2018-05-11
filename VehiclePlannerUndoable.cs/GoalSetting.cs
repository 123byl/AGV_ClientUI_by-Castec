using CtBind;
using GLCore;
using GLUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VehiclePlanner.Module.Implement;
using WeifenLuo.WinFormsUI.Docking;

namespace VehiclePlannerUndoable.cs {

    /// <summary>
    /// 以UndoableMapGL實作之GoalSetting介面
    /// </summary>
    public partial class GoalSetting : BaseGoalSetting {

        #region Declaration - Fields

        /// <summary>
        /// MapGL控制項參考
        /// </summary>
        private GLUICtrl rMapGL = null;
        
        #endregion Declaration - Fields

        #region Declaration - Properties

        /// <summary>
        /// MapGL控制項參考
        /// </summary>
        public GLUICtrl RefMapGL {
            get {
                return rMapGL;
            }
            set {
                if (rMapGL != value && value != null) {
                    rMapGL = value;
                }
            }
        }

        #endregion Declaration - Properties

        #region Funciotn - Constructors

        /// <summary>
        /// 共用建構方法
        /// </summary>
        public GoalSetting(DockState defState = DockState.Float)
            : base(defState) {
            InitializeComponent();

            /*-- 由於無法於介面設計師新增控制項，只好用程式碼新增 --*/
            this.panel1.Controls.Remove(this.dgvGoalPoint);
            this.panel1.Controls.Add(this.cboSingleType);
            this.panel1.Controls.Add(dgvGoalPoint);
            cboSingleType.Items.Add(nameof(SinglePairInfo));
            cboSingleType.Items.Add(nameof(SingleTowardPairInfo));
            cboSingleType.Items.Add(nameof(SingleLineInfo));
            cboSingleType.Items.Add(nameof(SingleAreaInfo));
            cboSingleType.SelectedValueChanged += cboSingleType_SelectedValueChanged;
        }

        #endregion Funciton - Constructors
        
        #region Functino - Events

        /// <summary>
        /// 重新綁定資料來源
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cboSingleType_SelectedValueChanged(object sender, EventArgs e) {
            if (sender is ComboBox cbo) {
                BindingSource source = null;
                string info = cbo.Text;
                switch (info) {
                    case nameof(SinglePairInfo):
                        source = new BindingSource(GLCMD.CMD.SinglePairInfo, null);
                        break;
                    case nameof(SingleTowardPairInfo):
                        source = new BindingSource(GLCMD.CMD.SingleTowerPairInfo, null);
                        break;
                    case nameof(SingleLineInfo):
                        source = new BindingSource(GLCMD.CMD.SingleLineInfo, null);
                        break;
                    case nameof(SingleAreaInfo):
                        source = new BindingSource(GLCMD.CMD.SingleAreaInfo, null);
                        break;
                    default:
                        throw new Exception($"未定義{info}類型資訊");
                }
                dgvGoalPoint.DataSource = source;
            }            
        }

        /// <summary>
        /// 移動視角
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvGoalPoint_DoubleClick(object sender, EventArgs e) {
            if (sender is DataGridView dgv) {
                int rowIndex = dgv.CurrentRow?.Index ?? -1;
                if (rowIndex >= 0 && rowIndex < dgv.RowCount) {
                    /*-- 取得當前資料類型 --*/
                    string singleType = cboSingleType.Text;
                    int cX = 0, cY = 0, x = 0, y = 0;
                    /*-- 計算中心點 --*/
                    switch (singleType) {
                        case nameof(SinglePairInfo):
                            cX = (int)dgv[nameof(SinglePairInfo.X), rowIndex].Value;
                            cY = (int)dgv[nameof(SinglePairInfo.Y), rowIndex].Value;
                            break;
                        case nameof(SingleTowardPairInfo):
                            cX = (int)dgv[nameof(SingleTowardPairInfo.X), rowIndex].Value;
                            cY = (int)dgv[nameof(SingleTowardPairInfo.Y), rowIndex].Value;
                            break;
                        case nameof(SingleLineInfo):
                            x = (int)dgv[nameof(SingleLineInfo.X0), rowIndex].Value;
                            cX = (int)dgv[nameof(SingleLineInfo.X1), rowIndex].Value;
                            y = (int)dgv[nameof(SingleLineInfo.Y0), rowIndex].Value;
                            cY = (int)dgv[nameof(SingleLineInfo.Y1), rowIndex].Value;
                            cX = (x + cX) / 2;
                            cY = (y = cY) / 2;
                            break;
                        case nameof(SingleAreaInfo):
                            x = (int)dgv[nameof(SingleAreaInfo.MinX), rowIndex].Value;
                            cX = (int)dgv[nameof(SingleAreaInfo.MaxX), rowIndex].Value;
                            y = (int)dgv[nameof(SingleAreaInfo.MinY), rowIndex].Value;
                            cY = (int)dgv[nameof(SingleAreaInfo.MaxY), rowIndex].Value;
                            cX = (x + cX) / 2;
                            cY = (y = cY) / 2;
                            break;
                        default:
                            throw new Exception($"未定義{singleType}類型資訊");
                    }
                    /*-- 將畫面移至中心點 --*/
                    rMapGL?.Focus(cX, cY);
                }
            }
        }



        #endregion Function - Events

        private void dgvGoalPoint_CellValueChanged(object sender, DataGridViewCellEventArgs e) {
            //if (sender is DataGridView dgv) {
            //    if (e.RowIndex >=0 && e.RowIndex < dgv.RowCount &&
            //        e.ColumnIndex >= 0 && e.ColumnIndex < dgv.ColumnCount) {
            //        string columnName = dgv.Columns[e.ColumnIndex].Name;
            //        string newValue = dgv[e.ColumnIndex, e.RowIndex].Value.ToString();
            //        int id = GetTargetID(e.RowIndex)
            //        string singleType = cboSingleType.Text;
            //        switch (singleType) {
            //            case nameof(SinglePairInfo):

            //                break;
            //        }
            //    }
            //}
        }
    }

    //public abstract class BaseSingleInfo<T>: IGLCore {

    //    protected DataGridView rDgv = null;

    //    /// <summary>
    //    /// 中心點
    //    /// </summary>
    //    public abstract Point GetCenter(int rowIndex);
    //    public BaseSingleInfo(DataGridView dgv) { }

    //    public static BaseSingleInfo<T> GetSingleInfo(DataGridView dgv) {
    //        var type = typeof(T);
    //        BaseSingleInfo<T> singleInfo = null;
    //        if (typeof(ISinglePairInfo).IsAssignableFrom(type)) {

    //        } else if (typeof(ISingleTowardPairInfo).IsAssignableFrom(type)) {

    //        } else if (typeof(ISingleLineInfo).IsAssignableFrom(type)) {

    //        } else if (typeof(ISingleAreaInfo).IsAssignableFrom(type)) {

    //        } else {
    //            throw new Exception($"未定義{type.Name}類型資訊");
    //        }
    //    }
    //}

    //public class PairInfo : BaseSingleInfo<ISinglePairInfo> {
    //    public Point GetCenter(int rowIndex) {
    //        int cX = (int)rDgv[nameof(SinglePairInfo.X), rowIndex].Value;
    //        int cY = (int)rDgv[nameof(SinglePairInfo.Y), rowIndex].Value;
    //        var val = ("","");    
    //    }
    //}


}
