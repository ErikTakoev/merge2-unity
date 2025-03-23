using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace BattleField
{
    public class BattleInstaller : LifetimeScope
    {
        [SerializeReference]
        BattleArrowController arrowController;
        [SerializeReference]
        BattleField field;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(arrowController);
            builder.RegisterComponent(field);
        }

        protected override void Awake()
        {
            base.Awake();
            BattleContainer.Initialize(Container);
		}
	}
}
