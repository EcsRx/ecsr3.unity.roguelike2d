using EcsR3.Collections.Entities;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using EcsR3.Groups;
using EcsR3.Plugins.Views.Components;
using EcsR3.Unity.Dependencies;
using EcsR3.Unity.Systems;
using Game.Components;
using SystemsR3.Events;
using UnityEngine;

namespace Game.ViewResolvers
{
    public class PlayerViewResolver : PrefabViewResolverSystem
    {
        public override IGroup Group { get; } = new Group(typeof(PlayerComponent), typeof(ViewComponent));
        
        protected override GameObject PrefabTemplate { get; } = Resources.Load<GameObject>("Prefabs/Player");

        public PlayerViewResolver(IEntityCollection entityCollection, IEventSystem eventSystem, IUnityInstantiator instantiator) : base(entityCollection, eventSystem, instantiator)
        {}

        protected override void OnViewCreated(IEntityComponentAccessor accessor, Entity entity, GameObject view)
        { view.name = "Player"; }
    }
}