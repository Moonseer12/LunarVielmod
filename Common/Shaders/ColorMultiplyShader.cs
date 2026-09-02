namespace Stellamod.Common.Shaders
{
    public class ColorMultiplyShader : BaseShader
    {
        private EffectParameter _intensityParam;
        private static ColorMultiplyShader _instance;
        public static ColorMultiplyShader Instance
        {
            get
            {
                _instance ??= new();
                _instance.SetDefaults();
                return _instance;
            }
        }

        public float Intensity
        {
            set
            {
                _intensityParam ??= Effect.Parameters["intensity"];
                _intensityParam.SetValue(value);
            }
        }


        public override void SetDefaults()
        {
            base.SetDefaults();
            Intensity = 25;
        }
    }
}
