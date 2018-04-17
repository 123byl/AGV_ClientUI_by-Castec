using CtINI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CtINI.Testing {
    public partial class CtrlIni : Form {

        private OpenFileDialog mOdlg = new OpenFileDialog();

        private SaveFileDialog mSdlg = new SaveFileDialog();

        private List<IniStruct> mInis = null;

        private TreeNode mSelectNode = null;

        private string mIniPath = null;

        public CtrlIni() {
            InitializeComponent();
        }

        private void btnOpen_Click(object sender, EventArgs e) {
            mOdlg.Filter = "Ini File|*.ini";
            mOdlg.Title = "Select a Ini File";
            if (mOdlg.ShowDialog() == DialogResult.OK) {
                mIniPath = mOdlg.FileName;
                lbFilePath.Text = "INI Path:" + mIniPath;
                mInis = CtINI.ReadValues(mOdlg.FileName);
                trvIni.Nodes.Clear();
                foreach (IniStruct item in mInis) {
                    TreeNode node = trvIni.Nodes.Add(item.Section);
                    IEnumerator Enumerator = item.GetEnumerator();
                    while (Enumerator.MoveNext()) {
                        KeyValuePair<string, string> kvp = (KeyValuePair<string, string>)Enumerator.Current;
                        TreeNode keyNode = node.Nodes.Add($"{kvp.Key}={kvp.Value}");
                        keyNode.Tag = kvp;
                        keyNode.ToolTipText = item.GetComment(kvp.Key);
                    }
                }
            }
        }
        
        private void trvIni_MouseUp(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Right && !cmsMain.Visible) {
                cmsMain.Show(trvIni, e.Location);
                if (miAddKey.Visible) miAddKey.Visible = false;
                if (!miAddSection.Visible) miAddSection.Visible = true;
                if (miEdit.Visible) miEdit.Visible = false;
                if (miDelete.Visible) miDelete.Visible = false;
            }
        }

        private void trvIni_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e) {
            if (e.Button == MouseButtons.Right) {
                cmsMain.Show(trvIni, e.Location);
                mSelectNode = e.Node;
                switch (e.Node.Level) {
                    case 0://Section
                        if (!miAddKey.Visible) miAddKey.Visible = true;
                        if (miAddSection.Visible) miAddSection.Visible = false;
                        if (!miEdit.Visible) miEdit.Visible = true;
                        if (!miDelete.Visible) miDelete.Visible = true;
                        break;
                    case 1://Key
                        if (miAddKey.Visible) miAddKey.Visible = false;
                        if (miAddSection.Visible) miAddSection.Visible = false;
                        if (!miEdit.Visible) miEdit.Visible = true;
                        if (!miDelete.Visible) miDelete.Visible = true;
                        break;
                    default:
                        mSelectNode = null;
                        throw new Exception("未知的資料階層");
                }
            }
        }

        private void miAddSection_Click(object sender, EventArgs e) {
            string sectionName = string.Empty;
            if (DialogResult.OK == InputBox.Show("Section Name", "Section Name", ref sectionName)) {
                trvIni.Nodes.Add(sectionName);
            }
        }

        private void miAddKey_Click(object sender, EventArgs e) {
            string key = string.Empty;
            string val = string.Empty;
            if (DialogResult.OK == InputBox.Show("Key", "Key", ref key)) {
                if (DialogResult.OK == InputBox.Show("Value", "Value", ref val)) {
                    KeyValuePair<string, string> kvp = new KeyValuePair<string, string>(key,val);
                    TreeNode keyNode =  mSelectNode.Nodes.Add($"{kvp.Key} = {kvp.Value}");
                    keyNode.Tag = kvp;
                }
            }
        }

        private void miDelete_Click(object sender, EventArgs e) {
            if (mSelectNode.Parent != null) {
                mSelectNode.Parent.Nodes.Remove(mSelectNode);
            } else {
                mSelectNode.TreeView.Nodes.Remove(mSelectNode);
            }
        }

        private void miEdit_Click(object sender, EventArgs e) {
            switch (mSelectNode.Level) {
                case 0:
                    string section = mSelectNode.Text;
                    if (DialogResult.OK == InputBox.Show("Section","Section",ref section)) {
                        mSelectNode.Text = section;
                    }
                    break;
                case 1:
                    KeyValuePair<string, string> kvp = (KeyValuePair<string,string>)mSelectNode.Tag;
                    string key = kvp.Key;
                    string val = kvp.Value;

                    InputBox.Show("Key", "Key", ref key);
                    InputBox.Show("Value", "Value", ref val);
                    kvp = new KeyValuePair<string, string>(key, val);
                    mSelectNode.Text = $"{kvp.Key} = {kvp.Value}";
                    mSelectNode.Tag = kvp;
                    break;
                default:
                    throw new Exception("未知資料階層");
            }
        }

        private void btnNew_Click(object sender, EventArgs e) {
            trvIni.Nodes.Clear();
            mIniPath = null;
            lbFilePath.Text = "INI Path:";
        }

        private void btnSave_Click(object sender, EventArgs e) {
            if (!File.Exists(mIniPath)) {
                mSdlg.Filter = "INI|*.ini";
                mSdlg.Title = "Save an INI File";
                if (DialogResult.OK == mSdlg.ShowDialog()) {
                    mIniPath = mSdlg.FileName;
                    File.Create(mIniPath).Close();
                }
            }
            foreach(TreeNode main in trvIni.Nodes) {
                string section = main.Text;
                CtINI.Write(mIniPath, section, null, null, null);
                foreach(TreeNode sub in main.Nodes) {
                    KeyValuePair<string, string> kvp = (KeyValuePair<string, string>)sub.Tag;
                    string key = kvp.Key;
                    string valu = kvp.Value;
                    CtINI.Write(mIniPath, section, key, valu, null);
                }
            }
        }
    }

    public static class InputBox {
        public static DialogResult Show(string title, string promptText, ref string value) {
            Form form = new Form();
            Label label = new Label();
            TextBox textBox = new TextBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();

            form.Text = title;
            label.Text = promptText;
            textBox.Text = value;

            buttonOk.Text = "OK";
            buttonCancel.Text = "Cancel";
            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

            label.SetBounds(9, 20, 372, 13);
            textBox.SetBounds(12, 36, 372, 20);
            buttonOk.SetBounds(228, 72, 75, 23);
            buttonCancel.SetBounds(309, 72, 75, 23);

            label.AutoSize = true;
            textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new Size(396, 107);
            form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
            form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            DialogResult dialogResult = form.ShowDialog();
            if (dialogResult == DialogResult.OK) value = textBox.Text;
            return dialogResult;
        }
    }
}
