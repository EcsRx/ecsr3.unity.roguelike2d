using EcsR3.Blueprints;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using EcsR3.Extensions;
using EcsR3.Plugins.Views.Components;
using Game.Components;

namespace Game.Blueprints
{
    public class ExitBlueprint : IBlueprint
    {
        public void Apply(IEntityComponentAccessor entityComponentAccessor, Entity entity)
        {
            entityComponentAccessor.AddComponents(entity, new ExitComponent(), new ViewComponent(), new RandomlyPlacedComponent());
        }
    }
}