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
using SystemsR3.Types;
using UnityEngine;

namespace Game.ViewResolvers
{
    [Priority(PriorityTypes.Higher)]
    public class FoodViewResolver : DynamicViewResolverSystem
    {
        private readonly FoodTiles _foodTiles;
        
        public IEntityComponentAccessor EntityComponentAccessor { get; }
        
        public override IGroup Group { get; } = new Group(typeof(FoodComponent), typeof(ViewComponent));

        public FoodViewResolver(IEventSystem eventSystem, IEntityCollection entityCollection, IUnityInstantiator instantiator, FoodTiles foodTiles, IEntityComponentAccessor entityComponentAccessor) : base(eventSystem, entityCollection, instantiator)
        {
            _foodTiles = foodTiles;
            EntityComponentAccessor = entityComponentAccessor;
        }

        public override GameObject CreateView(Entity entity)
        {
            var foodComponent = EntityComponentAccessor.GetComponent<FoodComponent>(entity);
            var foodTileIndex = foodComponent.IsSoda ? 1 : 0;
            var tileChoice = _foodTiles.AvailableTiles.ElementAt(foodTileIndex);
            var gameObject = Object.Instantiate(tileChoice, Vector3.zero, Quaternion.identity);
            gameObject.name = $"food-{entity.Id}";
            return gameObject;
        }

        public override void DestroyView(Entity entity, GameObject view)
        { Object.Destroy(view); }
    }
}