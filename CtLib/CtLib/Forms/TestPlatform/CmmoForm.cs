using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Threading;
using CtLib.Library;
using CtLib.Module.FESTO;
using System.Collections;


namespace CtLib.Forms.TestPlatform
{
    /// <summary>FESTO CMMO 測試介面</summary>
    public partial class Test_FestoCMMO : Form
    {
        #region Properties

        private delegate void UpdateUICallback();
        private delegate void UpdateUIInitCallback();
        private Thread newThread;
        private ushort RowIndexdt = 0;

        private CtFestoCMMO rCmmo = new CtFestoCMMO();

        #endregion

        #region FormActions

        /// <summary>建立測試介面</summary>
        public Test_FestoCMMO()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CtInvoke.TextBoxText(textBoxMasterControl, (CtFestoCMMO.ControlViaEthernet.GetHashCode().ToString()), false, false);
            CtInvoke.TextBoxText(textBoxIP, CtFestoCMMO.IP, false, false);
            CtInvoke.TextBoxText(textBoxPort, CtFestoCMMO.PORT, false, false);

            //Measurement Unit Combobox DataSource from Enum
            cbxMeasurementUnit.DataSource = Enum.GetValues(typeof(CtFestoCMMO.eUnitOfMeasurementConversionFactor));
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            CtInvoke.ButtonEnable(btnConnect, false);

            if (rCmmo.IsConnected)
            {
                rCmmo.CloseConnection();
                newThread.Join(1000);

                CtInvoke.ButtonText(btnConnect, "Connect");
                CtInvoke.ButtonEnable(btnEnable, false);
                CtInvoke.ButtonEnable(btnGetRecordTable, false);
                CtInvoke.ButtonEnable(btnSaveRecordTable, false);
                CtInvoke.TextBoxEnable(textBoxMasterControl, false);
                CtInvoke.LabelEnable(lbMasterControl, false);

                lbSWbit1.Image = Properties.Resources.Grey_Ball;
                lbSWbit2.Image = Properties.Resources.Grey_Ball;
                lbSWbit3.Image = Properties.Resources.Grey_Ball;
                lbSWbit7.Image = Properties.Resources.Grey_Ball;
                lbSWbit10.Image = Properties.Resources.Grey_Ball;
                lbSWbit15.Image = Properties.Resources.Grey_Ball;
                lbSTObit.Image = Properties.Resources.Grey_Ball;
            }
            else
            {
                try
                {
                    rCmmo.Connection(textBoxIP.Text, textBoxPort.Text);

                    if (rCmmo.IsConnected)
                    {
                        newThread = new Thread(new ThreadStart(ReadStatus));
                        newThread.SetApartmentState(ApartmentState.MTA);
                        newThread.IsBackground = true;
                        newThread.Start();
                    }
                }
                catch
                {
                    MessageBox.Show("Error during connect.");
                }
            }

            CtInvoke.ButtonEnable(btnConnect, true);
        }

        private void btnSet_Click(object sender, EventArgs e)
        {
            if (rCmmo.IsEnabled)
            {
                CtInvoke.ButtonText(btnEnable, "Enable");
                CtInvoke.ButtonEnable(btnStep0, false);
                CtInvoke.ButtonEnable(btnStep1, false);
                CtInvoke.ButtonEnable(btnJog0, false);
                CtInvoke.ButtonEnable(btnJog1, false);
                CtInvoke.ButtonEnable(btnHoming, false);
                CtInvoke.TextBoxText(textBoxIncrement, "", false, false);
                CtInvoke.TextBoxText(textBoxVelocity, "", false, false);
                CtInvoke.TextBoxEnable(textBoxIncrement, false);
                CtInvoke.TextBoxEnable(textBoxVelocity, false);
                CtInvoke.ComboBoxEnable(cbxMeasurementUnit, false);
                CtInvoke.LabelEnable(lbMeasurementUnit, false);

                rCmmo.Disable();

            }
            else
            {
                CtInvoke.ButtonText(btnEnable, "Disable");
                CtInvoke.ButtonEnable(btnHoming, true);
                CtInvoke.ButtonEnable(btnJog1, true);
                CtInvoke.ButtonEnable(btnJog0, true);
                CtInvoke.TextBoxText(textBoxIncrement, ((double)1).ToString("F3"), false, false);
                CtInvoke.TextBoxText(textBoxVelocity, rCmmo.DefaultVelocity.ToString("F3"), false, false);
                CtInvoke.TextBoxEnable(textBoxIncrement, true);
                CtInvoke.TextBoxEnable(textBoxVelocity, true);
                CtInvoke.ComboBoxEnable(cbxMeasurementUnit, true);
                CtInvoke.LabelEnable(lbMeasurementUnit, true);

                //CVE (Control Via Ethernet) , Set 2
                rCmmo.HigherOrderControl = (CtFestoCMMO.eHigherOrderControl)Enum.Parse(typeof(CtFestoCMMO.eHigherOrderControl),textBoxMasterControl.Text);

                rCmmo.Enable();
            }
        }

        private void btnStep0_Click(object sender, EventArgs e)
        {
            double Velocity = Convert.ToDouble(textBoxVelocity.Text);
            double Position = -Convert.ToDouble(textBoxIncrement.Text);
            rCmmo.SingleStep(Velocity, Position);
        }

        private void btnStep1_Click(object sender, EventArgs e)
        {
            double Velocity = Convert.ToDouble(textBoxVelocity.Text);
            double Position = Convert.ToDouble(textBoxIncrement.Text);
            rCmmo.SingleStep(Velocity, Position);
        }

        private void btnJog1_MouseDown(object sender, MouseEventArgs e)
        {
            rCmmo.JogStart(true);
        }

        private void btnJog1_MouseUp(object sender, MouseEventArgs e)
        {
            rCmmo.JogStop();
        }

        private void btnJog0_MouseDown(object sender, MouseEventArgs e)
        {
            rCmmo.JogStart(false);
        }

        private void btnJog0_MouseUp(object sender, MouseEventArgs e)
        {
            rCmmo.JogStop();
        }

        private void btnHoming_Click(object sender, EventArgs e)
        {
            rCmmo.Homing();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            rCmmo.ErrorReset();
        }

        private void btnStopMotion_Click(object sender, EventArgs e)
        {
            rCmmo.StopMotion();
        }

        private void dataGridViewRecordTable_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            RowIndexdt = Convert.ToUInt16(e.RowIndex + 1);
        }

        private void btnGetRecordTable_Click(object sender, EventArgs e)
        {
            CtInvoke.DataGridViewClear(dataGridViewRecordTable);

            for (ushort index = 1; index < 33; index++)
            {
                List<string> Row = rCmmo.GetRecordTableRow(index);
                CtInvoke.DataGridViewAddRow(dataGridViewRecordTable, Row, false, false);
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            //Select Record No
            //if (RowIndexdt != 0) // Debug
            if (RowIndexdt != 0 && RowIndexdt != 33)
            {
                rCmmo.StartRecordTableRow(RowIndexdt);
            }
            else
            {
                MessageBox.Show("Please Select Record No");
            }
        }

        private void btnSaveRecordTable_Click(object sender, EventArgs e)
        {
            ushort No;
            int Type = 0;
            double PositionX = 0;
            double PositionY = 0;
            double Velocity = 0;
            double Acceleration = 0;
            double Force = 0;

            //Store Data into Device (Disable status) , WriteRecord (CVE mode)
            if (rCmmo.IsEnabled)
            {
                CtInvoke.ButtonText(btnEnable, "Enable");
                CtInvoke.ButtonEnable(btnStep0, false);
                CtInvoke.ButtonEnable(btnStep1, false);
                CtInvoke.ButtonEnable(btnJog0, false);
                CtInvoke.ButtonEnable(btnJog1, false);
                CtInvoke.ButtonEnable(btnHoming, false);
                CtInvoke.TextBoxText(textBoxIncrement, "", false, false);
                CtInvoke.TextBoxText(textBoxVelocity, "", false, false);
                CtInvoke.TextBoxEnable(textBoxIncrement, false);
                CtInvoke.TextBoxEnable(textBoxVelocity, false);
                CtInvoke.ComboBoxEnable(cbxMeasurementUnit, false);
                CtInvoke.LabelEnable(lbMeasurementUnit, false);

                rCmmo.Disable();

            }
            else
            {
                //CVE (Control Via Ethernet) , Set 2
                rCmmo.HigherOrderControl = (CtFestoCMMO.eHigherOrderControl)Enum.Parse(typeof(CtFestoCMMO.eHigherOrderControl), textBoxMasterControl.Text);
            }

            //DataGridView No , Type , Position , Velocity , Acceleration
            for (int index = 0; index < dataGridViewRecordTable.RowCount; index++)
            {
                if (dataGridViewRecordTable[0, index].Value != null)
                {
                    No = Convert.ToUInt16(dataGridViewRecordTable[0, index].Value.ToString());

                    if (dataGridViewRecordTable[1, index].Value != null)
                    {
                        switch (dataGridViewRecordTable[1, index].Value.ToString())
                        {
                            case "PA":
                                Type = 16;
                                break;
                            case "PRN":
                                Type = 17;
                                break;
                            case "Inactive":
                            default:
                                Type = 0;
                                break;
                        }
                    }

                    if (dataGridViewRecordTable[2, index].Value != null)
                        PositionX = Convert.ToDouble(dataGridViewRecordTable[2, index].Value.ToString());

                    if (dataGridViewRecordTable[3, index].Value != null)
                        PositionY = Convert.ToDouble(dataGridViewRecordTable[3, index].Value.ToString());

                    if (dataGridViewRecordTable[4, index].Value != null)
                        Velocity = Convert.ToDouble(dataGridViewRecordTable[4, index].Value.ToString());

                    if (dataGridViewRecordTable[5, index].Value != null)
                        Acceleration = Convert.ToDouble(dataGridViewRecordTable[5, index].Value.ToString());

                    if (dataGridViewRecordTable[6, index].Value != null)
                        Force = Convert.ToDouble(dataGridViewRecordTable[6, index].Value.ToString());

                    rCmmo.SetRecordTableRow(No, Type, PositionX, PositionY, Velocity, Acceleration, Force);
                }
            }
            Thread.Sleep(500);
            btnGetRecordTable_Click(sender, e);

            rCmmo.StoreDataToDevice();
        }

        private void cbxMeasurementUnit_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox combobox = (ComboBox)sender;

            CtFestoCMMO.eUnitOfMeasurementConversionFactor MeasurementUnit = (CtFestoCMMO.eUnitOfMeasurementConversionFactor)Enum.Parse(typeof(CtFestoCMMO.eUnitOfMeasurementConversionFactor), combobox.SelectedValue.ToString());
            rCmmo.UnitOfMeasurementConversionFactor = MeasurementUnit;

            switch (MeasurementUnit)
            {
                case (CtFestoCMMO.eUnitOfMeasurementConversionFactor.METRE):
                    CtInvoke.LabelText(lbIncrementUnit, "mm");
                    CtInvoke.LabelText(lbVelocityUnit, "mm/s");
                    CtInvoke.LabelText(lbActualVelocityUnit, "mm/s");
                    CtInvoke.LabelText(lbActualPositionUnit, "mm");
                    CtInvoke.LabelText(lbTargetPositionUnit, "mm");
                    break;

                case (CtFestoCMMO.eUnitOfMeasurementConversionFactor.INCH):
                    CtInvoke.LabelText(lbIncrementUnit, "inch");
                    CtInvoke.LabelText(lbVelocityUnit, "inch/s");
                    CtInvoke.LabelText(lbActualVelocityUnit, "inch/s");
                    CtInvoke.LabelText(lbActualPositionUnit, "inch");
                    CtInvoke.LabelText(lbTargetPositionUnit, "inch");
                    break;

                case (CtFestoCMMO.eUnitOfMeasurementConversionFactor.REVOLUTIONS):
                    CtInvoke.LabelText(lbIncrementUnit, "r");
                    CtInvoke.LabelText(lbVelocityUnit, "rpm");
                    CtInvoke.LabelText(lbActualVelocityUnit, "rpm");
                    CtInvoke.LabelText(lbActualPositionUnit, "r");
                    CtInvoke.LabelText(lbTargetPositionUnit, "r");
                    break;

                case (CtFestoCMMO.eUnitOfMeasurementConversionFactor.DEGREE):
                    CtInvoke.LabelText(lbIncrementUnit, "∘");
                    CtInvoke.LabelText(lbVelocityUnit, "∘/s");
                    CtInvoke.LabelText(lbActualVelocityUnit, "∘/s");
                    CtInvoke.LabelText(lbActualPositionUnit, "∘");
                    CtInvoke.LabelText(lbTargetPositionUnit, "∘");
                    break;

                case (CtFestoCMMO.eUnitOfMeasurementConversionFactor.UNDEFINED):
                default:
                    CtInvoke.LabelText(lbIncrementUnit, "");
                    CtInvoke.LabelText(lbVelocityUnit, "");
                    CtInvoke.LabelText(lbActualVelocityUnit, "");
                    CtInvoke.LabelText(lbActualPositionUnit, "");
                    CtInvoke.LabelText(lbTargetPositionUnit, "");
                    break;
            }
        }

        private void btnStore_Click(object sender, EventArgs e)
        {
            rCmmo.StoreDataToDevice();
        }

        #endregion

        #region Functions

        private void ReadStatus()
        {
            UpdateUIInit();

            while (!rCmmo.IsTerminated)
            {
                try
                {
                    // --------------------------------------------
                    // Update UI
                    // --------------------------------------------
                    UpdateUI();

                    Thread.Sleep(100);
                }
                catch (System.IO.IOException)
                {
                    rCmmo.CloseConnection();
                    newThread.Join(1000);
                }
            }
        }

        private void UpdateUIInit()
        {
            if (this.InvokeRequired)
            {
                UpdateUIInitCallback d = new UpdateUIInitCallback(UpdateUIInit);
                try
                {
                    this.Invoke(d);
                }
                catch
                {
                }
            }
            else
            {
                CtInvoke.ButtonText(btnConnect, "Disconnect");
                CtInvoke.ButtonEnable(btnEnable, true);
                CtInvoke.ButtonEnable(btnGetRecordTable, true);
                CtInvoke.ButtonEnable(btnSaveRecordTable, true);
                CtInvoke.TextBoxEnable(textBoxMasterControl, true);
                CtInvoke.LabelEnable(lbMasterControl, true);

                CtInvoke.ComboBoxSelectedItem(cbxMeasurementUnit, rCmmo.UnitOfMeasurementConversionFactor);
            }
        }

        private void UpdateUI()
        {
            if (this.InvokeRequired)
            {
                UpdateUICallback d = new UpdateUICallback(UpdateUI);
                try
                {
                    this.Invoke(d);
                }
                catch
                {
                }
            }
            else
            {
                try
                {
                    long sw = rCmmo.StatusWord;
                    CtFestoCMMO.eNominalOperatingMode mode = rCmmo.NominalOperatingMode;

                    switch (mode)
                    {
                        case CtFestoCMMO.eNominalOperatingMode.POSITIONING_MODE:
                            CtInvoke.LabelText(lbOperationMode, "Positioning Mode");
                            break;
                        case CtFestoCMMO.eNominalOperatingMode.SPEED_MODE:
                            CtInvoke.LabelText(lbOperationMode, "Speed Mode");
                            break;
                        case CtFestoCMMO.eNominalOperatingMode.FORCE_MODE:
                            CtInvoke.LabelText(lbOperationMode, "Force Mode");
                            break;
                        case CtFestoCMMO.eNominalOperatingMode.HOMING_MODE:
                            CtInvoke.LabelText(lbOperationMode, "Homing Mode");
                            break;
                        case CtFestoCMMO.eNominalOperatingMode.JOG_POSITIVE:
                            CtInvoke.LabelText(lbOperationMode, "Jog positive");
                            break;
                        case CtFestoCMMO.eNominalOperatingMode.JOG_NEGATIVE:
                            CtInvoke.LabelText(lbOperationMode, "Jog negative");
                            break;
                        case 0:
                        default:
                            CtInvoke.LabelText(lbOperationMode, "");
                            break;
                    }

                    CtInvoke.LabelText(lbActualVelocity, rCmmo.ActualVelocity.ToString("F3"));
                    CtInvoke.LabelText(lbActualCurrent, rCmmo.ActualCurrent.ToString("F3"));
                    CtInvoke.LabelText(lbActualTorque, rCmmo.ActualForce.ToString("F1"));
                    CtInvoke.LabelText(lbTargetPosition, rCmmo.SetPointPosition.ToString("F3"));
                    CtInvoke.LabelText(lbActualPosition, rCmmo.ActualPosition.ToString("F3"));

                    //Ready
                    if ((sw & 0x02) > 0) { lbSWbit1.Image = Properties.Resources.Yellow_Ball; }
                    else { lbSWbit1.Image = Properties.Resources.Grey_Ball; }

                    //Enable
                    if ((sw & 0x04) > 0)
                    {
                        CtInvoke.ButtonEnable(btnStopMotion, true);
                        lbSWbit2.Image = Properties.Resources.Green_Ball;
                    }
                    else
                    {
                        CtInvoke.ButtonEnable(btnStopMotion, false);
                        lbSWbit2.Image = Properties.Resources.Grey_Ball;
                    }

                    //Error
                    if ((sw & 0x08) > 0)
                    {
                        CtInvoke.ButtonEnable(btnReset, true);
                        lbSWbit3.Image = Properties.Resources.Red_Ball;
                    }
                    else
                    {
                        CtInvoke.ButtonEnable(btnReset, false);
                        lbSWbit3.Image = Properties.Resources.Grey_Ball;
                    }

                    //Warning
                    if ((sw & 0x80) > 0) { lbSWbit7.Image = Properties.Resources.Red_Ball; }
                    else { lbSWbit7.Image = Properties.Resources.Grey_Ball; }

                    //MC
                    if ((sw & 0x400) > 0)
                    {
                        //Enable = 1 and MC = 1 , button enable
                        if (rCmmo.IsEnabled)
                        {
                            CtInvoke.ButtonEnable(btnJog0, true);
                            CtInvoke.ButtonEnable(btnJog1, true);
                            //Homing Valid = 1
                            if ((sw & 0x8000) > 0)
                            {
                                CtInvoke.ButtonEnable(btnStep0, true);
                                CtInvoke.ButtonEnable(btnStep1, true);
                            }
                            CtInvoke.ButtonEnable(btnHoming, true);
                            CtInvoke.ButtonEnable(btnStart, true);
                        }
                        else
                        {
                            CtInvoke.ButtonEnable(btnJog0, false);
                            CtInvoke.ButtonEnable(btnJog1, false);
                            CtInvoke.ButtonEnable(btnStep0, false);
                            CtInvoke.ButtonEnable(btnStep1, false);
                            CtInvoke.ButtonEnable(btnHoming, false);
                            CtInvoke.ButtonEnable(btnStart, false);
                        }

                        lbSWbit10.Image = Properties.Resources.Yellow_Ball;
                    }
                    else
                    {
                        //!Jog positive and !Jog negative and MC = 0 , Jog button disable
                        if (mode != CtFestoCMMO.eNominalOperatingMode.JOG_POSITIVE && mode != CtFestoCMMO.eNominalOperatingMode.JOG_NEGATIVE)
                        {
                            CtInvoke.ButtonEnable(btnJog0, false);
                            CtInvoke.ButtonEnable(btnJog1, false);
                        }
                        CtInvoke.ButtonEnable(btnStep0, false);
                        CtInvoke.ButtonEnable(btnStep1, false);
                        CtInvoke.ButtonEnable(btnHoming, false);
                        CtInvoke.ButtonEnable(btnStart, false);

                        lbSWbit10.Image = Properties.Resources.Grey_Ball;
                    }

                    //Homing Valid
                    if ((sw & 0x8000) > 0)
                    {
                        lbSWbit15.Image = Properties.Resources.Yellow_Ball;
                    }
                    else
                    {
                        lbSWbit15.Image = Properties.Resources.Grey_Ball;
                    }

                    //STO
                    if ((rCmmo.EnableStatusBitField & 0x01) > 0) { lbSTObit.Image = Properties.Resources.Green_Ball; }
                    else { lbSTObit.Image = Properties.Resources.Grey_Ball; }

                    //Error Message
                    if (rCmmo.ErrorWithTopPriorty != 0xFFFF)
                    {
                        lbErrorMsg.Text = String.Format("0x{0:X}", rCmmo.ErrorWithTopPriorty);
                    }
                    else
                    {
                        lbErrorMsg.Text = "";
                    }

                    //Warning Message
                    if (rCmmo.WarningWithTopPriorty != 0xFFFF)
                    {
                        lbWarningMsg.Text = String.Format("0x{0:X}", rCmmo.WarningWithTopPriorty);
                    }
                    else
                    {
                        lbWarningMsg.Text = "";
                    }
                }
                catch
                {
                }
            }
        }

        #endregion

    }
}
