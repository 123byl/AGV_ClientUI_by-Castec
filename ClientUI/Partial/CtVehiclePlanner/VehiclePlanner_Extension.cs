using SerialCommunication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FactoryMode;

namespace VehiclePlanner.Partial.CtVehiclePlanner {

    internal static class VehiclePlanner_Extension {
        public static ISerialClient SerialClient(this IFactory factory, DelReceiveDataEvent receiveDataEvent, bool bypass) {
            if (bypass) {
                return new FakeSerialClient(receiveDataEvent);
            } else {
                return FactoryMode.Factory.SerialClient(receiveDataEvent);
            }
        }
    }
}
