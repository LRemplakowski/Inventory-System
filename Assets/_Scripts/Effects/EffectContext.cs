using SunsetSystems.VisualEffects;

namespace SunsetSystems.Effects
{
    public struct EffectContext
    {
        public readonly IEffectHandler EffectHandler;
        public readonly IVisualEffectHandler VisualEffectHandler;
        public readonly IVisualEffectSource VisualEffectSource;

        public EffectContext(IEffectHandler effectHandler, IVisualEffectHandler visualsHandler, IVisualEffectSource visualEffectSource)
        {
            this.EffectHandler = effectHandler;
            this.VisualEffectHandler = visualsHandler;
            this.VisualEffectSource = visualEffectSource;
        }
    }
}
