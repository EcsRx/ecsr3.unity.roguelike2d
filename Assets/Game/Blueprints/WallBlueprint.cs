using EcsR3.Blueprints;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using EcsR3.Extensions;
using EcsR3.Plugins.Views.Components;
using Game.Components;

namespace Game.Blueprints
{
    public class WallBlueprint : IBlueprint
    {
        private readonly int DefaultWallHealth = 3;

        public void Apply(IEntityComponentAccessor entityComponentAccessor, Entity entity)
        {
            var wallComponent = new WallComponent();
            wallComponent.Health.Value = DefaultWallHealth;
            entityComponentAccessor.AddComponents(entity, wallComponent, new ViewComponent(), new RandomlyPlacedComponent());
        }
    }
}