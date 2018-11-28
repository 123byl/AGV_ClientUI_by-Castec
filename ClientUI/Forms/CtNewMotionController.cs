using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CtBind;
using CtLib.Library;
using VehiclePlanner.Core;
using VehiclePlanner.Partial.VehiclePlannerUI;
using static VehiclePlanner.Partial.VehiclePlannerUI.Events.TestingEvents;

namespace VehiclePlanner.Forms
{
	public partial class CtNewMotionController : Form, IDataDisplay<IBaseITSController>
	{
		#region Declaration - Field
		private MotionDirection mCurrentDir = MotionDirection.None;

		private MotionDirection mTmpDir = MotionDirection.None;

		private Keys mCurrentKey = Keys.None;
		/// <summary>
		/// 控制器參考物件
		/// </summary>
		private IBaseITSController rController = null;

		/// <summary>
		/// 主介面物件參考
		/// </summary>
		private BaseVehiclePlanner_Ctrl rUI = null;

		public event Events.TestingEvents.DelMotion_Down MotionDown;

		public event Events.TestingEvents.DelMotion_Up MotionUp;
		#endregion

		#region Function - Constructors
		public CtNewMotionController(BaseVehiclePlanner_Ctrl ctrl, IBaseITSController controller)
		{
			InitializeComponent();
			btnForward.MouseDown += Motion_MouseDown;
			btnForward.MouseUp += Motion_MouseUp;
			btnBack.MouseDown += Motion_MouseDown;
			btnBack.MouseUp += Motion_MouseUp;
			btnCW.MouseDown += Motion_MouseDown;
			btnCW.MouseUp += Motion_MouseUp;
			btnCCW.MouseDown += Motion_MouseDown;
			btnCCW.MouseUp += Motion_MouseUp;
			this.KeyUp += Motion_KeyUp;
			btnForward.KeyUp += Motion_KeyUp;
			btnBack.KeyUp += Motion_KeyUp;
			btnCW.KeyUp += Motion_KeyUp;
			btnCCW.KeyUp += Motion_KeyUp;
			rUI = ctrl;
			Bindings(controller);

		}
		#endregion

		#region Function - Private
		private void Motion(MotionDirection dir)
		{
			switch (dir)
			{
				case MotionDirection.Forward:
					this.InvokeIfNecessary(() => btnForward.BackColor = Color.LightBlue);
					break;
				case MotionDirection.Backward:
					this.InvokeIfNecessary(() => btnBack.BackColor = Color.LightBlue);
					break;
				case MotionDirection.RightTurn:
					this.InvokeIfNecessary(() => btnCW.BackColor = Color.LightBlue);
					break;
				case MotionDirection.LeftTrun:
					this.InvokeIfNecessary(() => btnCCW.BackColor = Color.LightBlue);
					break;
			}

			MotionDown?.BeginInvoke(dir, MotionDownCallBack, MotionDown);
		}
		private void MotionDownCallBack(IAsyncResult ar)
		{
			DelMotion_Down del = ar.AsyncState as DelMotion_Down;
			MotionDirection result = del.EndInvoke(ar);
			mCurrentDir = result;
		}

		private void Stop()
		{
			MotionUp?.BeginInvoke(MotionUpCallBack, MotionUp);
			this.InvokeIfNecessary(() => { btnForward.BackColor = SystemColors.Control; btnBack.BackColor = SystemColors.Control; btnCW.BackColor = SystemColors.Control; btnCCW.BackColor = SystemColors.Control; });
		}

		private void MotionUpCallBack(IAsyncResult ar)
		{
			DelMotion_Up del = ar.AsyncState as DelMotion_Up;
			del.EndInvoke(ar);
			mTmpDir = MotionDirection.None;
			mCurrentDir = MotionDirection.None;
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if (!txtVelocity.Focused && mCurrentKey != keyData)
			{
				Console.WriteLine(keyData.ToString() + " ↓");
				mCurrentKey = keyData;
				MotionDirection dir = MotionDirection.None;
				switch (keyData)
				{
					case Keys.Up:
					case Keys.W:
						btnForward.Focus();
						dir = MotionDirection.Forward;
						break;
					case Keys.Down:
					case Keys.S:
						btnBack.Focus();
						dir = MotionDirection.Backward;
						break;
					case Keys.Right:
					case Keys.D:
						btnCW.Focus();
						dir = MotionDirection.RightTurn;
						break;
					case Keys.Left:
					case Keys.A:
						btnCCW.Focus();
						dir = MotionDirection.LeftTrun;
						break;
					case Keys.Space:
						btnStop.Focus();
						dir = MotionDirection.Stop;
						break;
				}

				if (dir != MotionDirection.None)
				{
					if (mCurrentDir != dir)
					{
						if (dir != MotionDirection.Stop)
						{
							if (mTmpDir == MotionDirection.None && mCurrentDir == MotionDirection.None)
							{
								//mCurrentKey = keyData;
								mTmpDir = dir;
								Motion(dir);
							}
							else
							{
								Stop();
								mCurrentKey = Keys.None;
							}
						}
						else if (dir == MotionDirection.Stop && (mTmpDir != MotionDirection.None || mCurrentDir != MotionDirection.None))
						{
							Stop();
							mCurrentKey = Keys.None;
						}
					}
					return true;
				}
			}
			else if (!txtVelocity.Focused && mCurrentKey == keyData)
			{
				return true;
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}

		#endregion

		#region Function - Events
		private void Motion_MouseDown(object sender, EventArgs e)
		{
			var btn = sender as Button;
			MotionDirection dir = MotionDirection.None;
			switch (btn.Name)
			{
				case nameof(btnForward):
					dir = MotionDirection.Forward;
					break;
				case nameof(btnBack):
					dir = MotionDirection.Backward;
					break;
				case nameof(btnCW):
					dir = MotionDirection.RightTurn;
					break;
				case nameof(btnCCW):
					dir = MotionDirection.LeftTrun;
					break;
			}

			if (mCurrentDir == MotionDirection.None)
			{
				Motion(dir);
			}
			else
			{
				Stop();
			}
		}

		private void Motion_MouseUp(object sender, EventArgs e)
		{
			if (mCurrentDir != MotionDirection.None)
			{
				Stop();
				this.InvokeIfNecessary(() => { btnForward.BackColor = SystemColors.Control; btnBack.BackColor = SystemColors.Control; btnCW.BackColor = SystemColors.Control; btnCCW.BackColor = SystemColors.Control; });
			}
		}

		private void Motion_KeyUp(object sender, KeyEventArgs e)
		{
			if (!txtVelocity.Focused)
			{
				Console.WriteLine(e.KeyData.ToString() + " ↑");
				switch (e.KeyData)
				{
					case Keys.Up:
					case Keys.W:
					case Keys.Down:
					case Keys.S:
					case Keys.Right:
					case Keys.D:
					case Keys.Left:
					case Keys.A:
						this.Focus();
						mCurrentKey = Keys.None;
						if (mCurrentDir != MotionDirection.None) Stop();

						break;
				}
			}
		}
		#endregion

		public void Bindings(IBaseITSController source)
		{
			if (source == null) return;
			if (source.DelInvoke == null) source.DelInvoke = invk => this.InvokeIfNecessary(invk);

			/*馬達激磁狀態*/
			string dataMember = nameof(source.IsMotorServoOn);
			btnServo.DataBindings.ExAdd(nameof(btnServo.Text), source, dataMember, (sneder, e) =>
			{
				e.Value = (bool)e.Value ? "Servo ON" : "Servo OFF";
			});
			btnServo.DataBindings.ExAdd(nameof(btnServo.BackColor), source, dataMember, (sneder, e) =>
			{
				e.Value = (bool)e.Value ? Color.LightGreen : Color.Red;
			});
			/*-- 手動控制移動速度 --*/
			txtVelocity.DataBindings.ExAdd(nameof(txtVelocity.Text), source, nameof(source.Velocity), (sender, e) =>
			{
				e.Value = e.Value.ToString();
			}, source.Velocity.ToString());
		}

		private void btnServo_Click(object sender, EventArgs e)
		{
			rUI.MotorServoOn();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			Stop();
		}
	}
}
