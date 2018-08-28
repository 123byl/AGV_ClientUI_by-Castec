using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace CtExtendLib
{
	public partial class LoadingForm : Form
	{
		public LoadingForm()
		{
			InitializeComponent();
		}
		public void Start(string title, int seconds)
		{
			DateTime t = DateTime.Now;
			Text = title;
			this.Show();
			do
			{
				pgbLoad.Value = (int)(DateTime.Now.Subtract(t).TotalSeconds*100 / seconds) ;
				Application.DoEvents();
				lblLoad.Text = $"讀取剩餘 {seconds - (int)DateTime.Now.Subtract(t).TotalSeconds}秒";
				Application.DoEvents();
				Thread.Sleep(100);
			} while (DateTime.Now.Subtract(t).TotalSeconds<=seconds);
			this.Close();
			this.Dispose();
		}
	}
}
