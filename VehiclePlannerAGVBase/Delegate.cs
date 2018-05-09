using GLCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VehiclePlannerAGVBase {

    /// <summary>
    /// 按照順序移動全部
    /// </summary>
    public delegate void DelRunLoop(IEnumerable<IGoal> goal);

    /// <summary>
    /// 更新 Goal 點
    /// </summary>
    public delegate void DelUpdateGoal(IGoal newGoal);


}
