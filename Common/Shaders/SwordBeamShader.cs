namespace Stellamod.Common.Shaders
{
    public class SwordBeamShader : BaseShader
    {
        private EffectParameter _innerColorParam;
        private EffectParameter _outerColorParam;
        private static SwordBeamShader _instance;
        public static SwordBeamShader Instance
        {
            get
            {
                _instance ??= new();
                _instance.SetDefaults();
                return _instance;
            }
        }


   
        public Color InnerColor
        {
            set
            {
                _innerColorParam ??= Effect.Parameters["innerColor"];
                _innerColorParam.SetValue(value.ToVector3());
            }
        }

        public Color OuterColor
        {
            set
            {
                _outerColorParam ??= Effect.Parameters["outerColor"];
                _outerColorParam.SetValue(value.ToVector3());
            }
        }


        public override void SetDefaults()
        {
            base.SetDefaults();
            InnerColor = Color.White;
            OuterColor = Color.Red;
            BlendState = BlendState.AlphaBlend;
        }
    }
}
