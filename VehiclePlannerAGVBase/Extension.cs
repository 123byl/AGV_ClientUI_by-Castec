using SerialCommunication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FactoryMode;

namespace VehiclePlannerAGVBase {

    /// <summary>
    /// 擴充方法
    /// </summary>
    public static class VehiclePlannerExtension {
        /// <summary>
        /// 建構序列傳輸物件
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="receiveDataEvent"></param>
        /// <param name="bypass"></param>
        /// <returns></returns>
        public static ISerialClient SerialClient(this IFactory factory, DelReceiveDataEvent receiveDataEvent, bool bypass) {
            if (bypass) {
                return new FakeSerialClient(receiveDataEvent);
            } else {
                return FactoryMode.Factory.SerialClient(receiveDataEvent);
            }
        }
    }

}
