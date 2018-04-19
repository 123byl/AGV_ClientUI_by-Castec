using SerialCommunication;
using static FactoryMode;

namespace VehiclePlanner.Core {

    public static class VehiclePlanner_Extension {

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

        /// <summary>
        /// 建構VehiclePlanner物件
        /// </summary>
        /// <param name="factory"></param>
        /// <returns></returns>
        public static ICtVehiclePlanner CtVehiclePlanner(this IFactory factory) {
            return new CtVehiclePlanner();
        }
        
    }
}
