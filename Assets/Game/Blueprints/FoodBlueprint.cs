using EcsR3.Blueprints;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using EcsR3.Extensions;
using EcsR3.Plugins.Views.Components;
using Game.Components;
using UnityEngine;

namespace Game.Blueprints
{
    public class FoodBlueprint : IBlueprint
    {
        private readonly int FoodValue = 10;
        private readonly int SodaValue = 20;

        private bool ShouldBeSoda()
        { return Random.Range(0, 2) == 1; }
    

        public void Apply(IEntityComponentAccessor entityComponentAccessor, Entity entity)
        {
            var foodComponent = new FoodComponent();
            var isSoda = ShouldBeSoda();
            foodComponent.IsSoda = isSoda;
            foodComponent.FoodAmount = isSoda ? SodaValue : FoodValue;
            entityComponentAccessor.AddComponents(entity, foodComponent, new ViewComponent(), new RandomlyPlacedComponent());
        }
    }
}