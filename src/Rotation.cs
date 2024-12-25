#if BEPINEX
using BepInEx.Configuration;

#elif MELONLOADER
using MelonLoader;

#endif

namespace BetterRoutingFlag {
    public class Rotation {

#if BEPINEX
        private ConfigEntry<float> _rotY;
        private ConfigEntry<float> _rotW;
        private ConfigEntry<float> _rotationY;

        public Rotation(
            ConfigEntry<float> rotY,
            ConfigEntry<float> rotW,
            ConfigEntry<float> rotationY
        ) {
            this._rotY = rotY;
            this._rotW = rotW;
            this._rotationY = rotationY;
        }

#elif MELONLOADER
        private MelonPreferences_Entry<float> _rotY;
        private MelonPreferences_Entry<float> _rotW;
        private MelonPreferences_Entry<float> _rotationY;

        public Rotation(
            MelonPreferences_Entry<float> rotY,
            MelonPreferences_Entry<float> rotW,
            MelonPreferences_Entry<float> rotationY
        ) {
            this._rotY = rotY;
            this._rotW = rotW;
            this._rotationY = rotationY;
        }
#endif

        public float rotY {
            get => _rotY.Value;
            set {
                _rotY.Value = value;
            }
        }

        public float rotW {
            get => _rotW.Value;
            set {
                _rotW.Value = value;
            }
        }

        public float rotationY {
            get => _rotationY.Value;
            set => _rotationY.Value = value;
        }
    }
}
