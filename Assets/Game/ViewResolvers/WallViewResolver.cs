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
    public class WallViewResolver : DynamicViewResolverSystem
    {
        private readonly WallTiles _wallTiles;
        
        public override IGroup Group { get; } = new Group(typeof(WallComponent), typeof(ViewComponent));

        public WallViewResolver(IEventSystem eventSystem, IEntityCollection entityCollection, IUnityInstantiator instantiator, WallTiles wallTiles) : base(eventSystem, entityCollection, instantiator)
        {
            _wallTiles = wallTiles;
        }

        public override GameObject CreateView(Entity entity)
        {
            var tileChoice = _wallTiles.AvailableTiles.TakeRandom();
            var gameObject = Object.Instantiate(tileChoice, Vector3.zero, Quaternion.identity);
            gameObject.name = $"wall-{entity.Id}";
            return gameObject;
        }

        public override void DestroyView(Entity entity, GameObject view)
        { Object.Destroy(view); }
    }
}