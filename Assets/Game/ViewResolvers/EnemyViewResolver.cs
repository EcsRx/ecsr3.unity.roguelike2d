using System.Linq;
using EcsR3.Collections.Entities;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using EcsR3.Extensions;
using EcsR3.Groups;
using EcsR3.Plugins.Views.Components;
using EcsR3.Unity.Dependencies;
using EcsR3.Unity.Systems;
using Game.Components;
using Game.SceneCollections;
using SystemsR3.Attributes;
using SystemsR3.Events;
using UnityEngine;

namespace Game.ViewResolvers
{
    [Priority(100)]
    public class EnemyViewResolver : DynamicViewResolverSystem
    {
        private readonly EnemyTiles _enemyTiles;
        
        public IEntityComponentAccessor EntityComponentAccessor { get; }

        public override IGroup Group { get; } = new Group(typeof(EnemyComponent), typeof(ViewComponent));

        public EnemyViewResolver(IEventSystem eventSystem, IEntityCollection entityCollection, IUnityInstantiator instantiator, EnemyTiles enemyTiles, IEntityComponentAccessor entityComponentAccessor) : base(eventSystem, entityCollection, instantiator)
        {
            _enemyTiles = enemyTiles;
            EntityComponentAccessor = entityComponentAccessor;
        }

        public override GameObject CreateView(Entity entity)
        {
            var enemyComponent = EntityComponentAccessor.GetComponent<EnemyComponent>(entity);
            var enemyType = (int)enemyComponent.EnemyType;
            var tileChoice = _enemyTiles.AvailableTiles.ElementAt(enemyType);
            var gameObject = Object.Instantiate(tileChoice, Vector3.zero, Quaternion.identity);
            gameObject.name = $"enemy-{entity.Id}";
            return gameObject;
        }

        public override void DestroyView(Entity entity, GameObject view)
        { Object.Destroy(view); }
    }
}