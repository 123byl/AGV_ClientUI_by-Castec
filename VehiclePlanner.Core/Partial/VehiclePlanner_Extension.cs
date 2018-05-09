using static FactoryMode;

namespace VehiclePlanner.Core {

    public static class BaseVehiclePlanner_Extension {
        
        /// <summary>
        /// 建構VehiclePlanner物件
        /// </summary>
        /// <param name="factory"></param>
        /// <returns></returns>
        public static IBaseVehiclePlanner CtVehiclePlanner(this IFactory factory) {
            return new BaseVehiclePlanner();
        }
        
    }
}
