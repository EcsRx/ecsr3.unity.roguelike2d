using EcsR3.Collections.Entities;
using EcsR3.Entities;
using EcsR3.Groups;
using EcsR3.Plugins.Views.Components;
using EcsR3.Unity.Dependencies;
using EcsR3.Unity.Systems;
using Game.Components;
using Game.Extensions;
using Game.SceneCollections;
using SystemsR3.Attributes;
using SystemsR3.Events;
using UnityEngine;

namespace Game.ViewResolvers
{
    [Priority(100)]
    public class ExitViewResolver : DynamicViewResolverSystem
    {
        private readonly ExitTiles _exitTiles;
        
        public ExitViewResolver(IEventSystem eventSystem, IEntityCollection entityCollection, IUnityInstantiator instantiator, ExitTiles exitTiles) : base(eventSystem, entityCollection, instantiator)
        {
            _exitTiles = exitTiles;
        }

        public override IGroup Group { get; } = new Group(typeof(ExitComponent), typeof(ViewComponent));
        
        public override GameObject CreateView(Entity entity)
        {
            var tileChoice = _exitTiles.AvailableTiles.TakeRandom();
            var gameObject = Object.Instantiate(tileChoice, Vector3.zero, Quaternion.identity);
            gameObject.name = $"exit-{entity.Id}";
            return gameObject;
        }

        public override void DestroyView(Entity entity, GameObject view)
        { Object.Destroy(view); }
    }
}