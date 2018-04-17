using CtLib.Library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VehiclePlanner.Core;

namespace VehiclePlanner {
    public partial class Test : Form,IDataDisplay<AGVController> {
        private AGVController mVehiclePlanner = new AGVController();// FactoryMode.Factory.CtVehiclePlanner();
        public Test() {
            InitializeComponent();
            //mVehiclePlanner.Initial();
            Bindings(mVehiclePlanner);
        }

        public void Bindings(AGVController source) {
            if (source.DelInvoke == null) source.DelInvoke = invk => this.InvokeIfNecessary(invk);
            bool isConnect = source.IsConnected;
            btnTest.DataBindings.Add("Text", source, "IsConnected").Format += (sender, e) => e.Value = (bool)e.Value ? "Connected" : "Disconnected";
            btnTest.DataBindings.Add("BackColor", source, "IsConnected").Format += (sneder, e) => e.Value = (bool)e.Value ? Color.Green : Color.Red;
        }
        private void btnTest_Click(object sender, EventArgs e) {
            //mVehiclePlanner.IsBypassSocket = true;
            Task.Run(() => {
                mVehiclePlanner.ConnectToITS(true, "127.0.0.1");
            });
        }
    }

    public class AGV:IDataSource {
        private bool mIsConnected = false;
        public bool IsConnected {
            get => mIsConnected;
            set {
                if (mIsConnected != value) {
                    mIsConnected = value;
                }
                OnPropertyChanged();
            }
        }
        public Action<MethodInvoker> DelInvoke { get; set; } = null;

        public event PropertyChangedEventHandler PropertyChanged;
        
        public void ConnectToITS(bool cnn,string ip) {
            IsConnected = !IsConnected;
        }

        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = "") {
            DelInvoke(() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)));
        }
    }

    public class AGVController :IDataSource{
        private AGV mAGV = new AGV();
        private event PropertyChangedEventHandler mPropertyChanged = null;
        public event PropertyChangedEventHandler PropertyChanged {
            add {
                mAGV.PropertyChanged += value;
                mPropertyChanged += value;
            }
            remove {
                mAGV.PropertyChanged -= value;
                mPropertyChanged -= value;
            }
        }
        private bool mIsConnected = false;
        public bool IsConnected {
            get => mIsConnected;
            set {
                if (mIsConnected != value) {
                    mIsConnected = value;
                }
                OnPropertyChanged();
            }
        }
        public Action<MethodInvoker> DelInvoke {
            get => mAGV.DelInvoke;
            set {
                mAGV.DelInvoke = value;
            }

        }
        public void ConnectToITS(bool cnn, string ip) {
            IsConnected = !IsConnected;
        }
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = "") {
            DelInvoke(() => mPropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)));
        }
    }
}
