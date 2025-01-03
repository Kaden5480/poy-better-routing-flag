using UnityEngine;

namespace BetterRoutingFlag.Patches {
    public class Flag {
        public static bool IsUsingFlag() {
            RoutingFlag[] flags = GameObject.FindObjectsOfType<RoutingFlag>();
            RoutingFlag flag;

            if (flags.Length < 1) {
                return false;
            }

            flag = flags[0];

            return flag.currentlyUsingFlag;
        }
    }
}
