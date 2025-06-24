using EcsR3.Entities.Accessors;
using EcsR3.Infrastructure.Extensions;
using Game.Components;
using Game.Computeds;
using SystemsR3.Infrastructure.Dependencies;
using SystemsR3.Infrastructure.Extensions;

namespace Game.Modules
{
    public class ComputedModule : IDependencyModule
    {
        public void Setup(IDependencyRegistry registry)
        {
            registry.Bind<IComputedPlayerPosition>(c =>
            {
                c.ToMethod(x =>
                {
                    var entityComponentAccessor = x.Resolve<IEntityComponentAccessor>();
                    var computedEntityGroup = x.ResolveComputedEntityGroup(typeof(PlayerComponent));
                    return new ComputedPlayerPosition(computedEntityGroup, entityComponentAccessor);
                });
            });
        }
    }
}