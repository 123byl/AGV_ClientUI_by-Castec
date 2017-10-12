using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using CtLib.Library;

namespace CtLib.Module.PCB.ICT {
	/// <summary>適用於 Cadence Allegro 之 In-Circuit Test 檔案(*.val)</summary>
	public partial class ICTReader : Form {

		private IEnumerable<IctBase> mIctColl;
		private PointF mCenter;
		private DialogResult mFrmResult = DialogResult.Cancel;
		private string mFistId = string.Empty;

		/// <summary>建構 ICT 讀取器</summary>
		public ICTReader() {
			InitializeComponent();
		}

		private void ChangeControl(bool stt) {
			foreach (Control ctrl in this.Controls) {
				CtInvoke.ControlVisible(ctrl, stt);
			}
			CtInvoke.ControlVisible(btnOpenFile, !stt);
		}

		private PointF CalculateCenterPoint() {
			IEnumerable<IctSymbol> ictColl = dgvTarget.Rows.Cast<DataGridViewRow>().Select(row => row.Tag as IctSymbol);
			if (ictColl != null && ictColl.Any()) {
				double x = ictColl.Average(ict => ict.PinX);
				double y = ictColl.Average(ict => ict.PinY);
				return new PointF((float)x, (float)y);
			} else return PointF.Empty;
		}

		private PointF CalculateMilToMm(PointF p) {
			float x = p.X * 0.0254F;
			float y = p.Y * 0.0254F;
			return new PointF(x, y);
		}

		private void CalculateCurrentPoint() {
			PointF milPoint = CalculateCenterPoint();
			lbMilX.Text = milPoint.X.ToString("F3");
			lbMilY.Text = milPoint.Y.ToString("F3");

			PointF mmPoint = CalculateMilToMm(milPoint);
			lbMmX.Text = mmPoint.X.ToString("F3");
			lbMmY.Text = mmPoint.Y.ToString("F3");
		}

		private void SearchComponent(string id) {
			if (!string.IsNullOrEmpty(id) && mIctColl != null) {

				IEnumerable<IctSymbol> tar = mIctColl.Where(
					ict => {
						IctSymbol refDes = ict as IctSymbol;
						return refDes != null && refDes.RefDes == id;
					}
				).Cast<IctSymbol>();

				int idx = 0;
				DataGridViewRow[] rows = new DataGridViewRow[tar.Count()];
				foreach (var ict in tar) {
					DataGridViewRow row = new DataGridViewRow();
					row.CreateCells(dgvSearch, ict.RefDes, ict.SymbolName, ict.PinName);
					row.Tag = ict;
					rows[idx] = row;
					idx++;
				}

				dgvSearch.InvokeIfNecessary(
					() => {
						dgvSearch.Rows.Clear();
						dgvSearch.Rows.AddRange(rows);
					}
				);
			}
		}

		/// <summary>將表單顯示為強制回應對話方塊</summary>
		/// <param name="center">使用者選取的點位中心點</param>
		/// <returns>其中一個 <see cref="DialogResult"/> 值</returns>
		public DialogResult ShowDialog(out PointF center) {
			mCenter = PointF.Empty;
			this.ShowDialog();
			center = mCenter;
			return mFrmResult;
		}

		/// <summary>將表單顯示為強制回應對話方塊</summary>
		/// <param name="id">啟動後自動進入搜尋</param>
		/// <param name="center">使用者選取的點位中心點</param>
		/// <returns>其中一個 <see cref="DialogResult"/> 值</returns>
		public DialogResult ShowDialog(string id, out PointF center) {
			mCenter = PointF.Empty;

			txtPartNo.Text = id;
			mFistId = id;

			this.ShowDialog();
			center = mCenter;
			return mFrmResult;
		}

		private void btnOpenFile_Click(object sender, EventArgs e) {
			string file = string.Empty;
			using (OpenFileDialog dialog = new OpenFileDialog()) {
				dialog.Filter = "In-Circuit Test File | *.val";
				if (dialog.ShowDialog() == DialogResult.OK)
					file = dialog.FileName;
			}

			if (!string.IsNullOrEmpty(file)) {
				mIctColl = File.ReadLines(file).Select(str => IctBase.Factory(str));
				ChangeControl(true);
			}
		}

		private void btnSearch_Click(object sender, EventArgs e) {
			string partNo = txtPartNo.Text;
			SearchComponent(partNo);
		}

		private void btnAdd_Click(object sender, EventArgs e) {
			if (dgvSearch.SelectedRows.Count > 0) {
				IEnumerable<DataGridViewRow> srcRows = dgvSearch.SelectedRows.Cast<DataGridViewRow>();
				IEnumerable<DataGridViewRow> tarRows = dgvTarget.Rows.Cast<DataGridViewRow>();
				IEnumerable<bool> repStt = srcRows.Select(
					srcRow => {
						var repRow = tarRows.FirstOrDefault(
							tarRow =>
								srcRow.Cells["colSym"].Value.ToString() == tarRow.Cells["colTarSym"].Value.ToString()
								&& srcRow.Cells["colPin"].Value.ToString() == tarRow.Cells["colTarPin"].Value.ToString()
						);
						return repRow != null;
					}
				);
				List<DataGridViewRow> newRows = new List<DataGridViewRow>();
				for (int idx = 0; idx < srcRows.Count(); idx++) {
					if (!repStt.ElementAt(idx)) {
						var srcRow = srcRows.ElementAt(idx);
						var newRow = new DataGridViewRow();
						newRow.CreateCells(dgvTarget, srcRow.Cells["colSym"].Value, srcRow.Cells["colPin"].Value);
						newRow.Tag = srcRow.Tag;
						newRows.Add(newRow);
					}
				}
				if (newRows.Count > 0) {
					dgvTarget.BeginInvokeIfNecessary(() => dgvTarget.Rows.AddRange(newRows));
				}
			}
			CalculateCurrentPoint();
		}

		private void btnRemove_Click(object sender, EventArgs e) {
			if (dgvTarget.SelectedRows.Count > 0) {
				DataGridViewRow row = dgvTarget.SelectedRows[0];
				dgvTarget.Rows.Remove(row);
				row.Dispose();
			}
			CalculateCurrentPoint();
		}

		private void btnAddAll_Click(object sender, EventArgs e) {
			dgvSearch.SelectAll();
			btnAdd.PerformClick();
			CalculateCurrentPoint();
		}

		private void btnRemoveAll_Click(object sender, EventArgs e) {
			int rowCount = dgvTarget.RowCount;
			for (int idx = 0; idx < rowCount; idx++) {
				dgvTarget.Rows.RemoveAt(0);
			}
			CalculateCurrentPoint();
		}

		private void btnExit_Click(object sender, EventArgs e) {
			mFrmResult = DialogResult.Cancel;
			CtInvoke.FormClose(this);
		}

		private void btnSave_Click(object sender, EventArgs e) {
			mFrmResult = DialogResult.OK;
			PointF tempPoint = CalculateCenterPoint();
			mCenter = CalculateMilToMm(tempPoint);
			CtInvoke.FormClose(this);
		}

		private void ICTReader_Shown(object sender, EventArgs e) {
			if (!string.IsNullOrEmpty(mFistId)) {
				Application.DoEvents();
				btnOpenFile.PerformClick();
				SearchComponent(mFistId);
				mFistId = string.Empty;
			}
		}
	}
}
