using VContainer;

namespace BattleField
{
    public static class BattleContainer
    {
        static IObjectResolver resolver;

        public static IObjectResolver Resolver { get { return resolver; } }
        public static void Initialize(IObjectResolver resolver)
        {
            BattleContainer.resolver = resolver;
        }
    }
}
