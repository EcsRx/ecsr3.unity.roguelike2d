using Game.Configuration;
using SystemsR3.Infrastructure.Dependencies;
using SystemsR3.Infrastructure.Extensions;

namespace Game.Modules
{
    public class GameModule : IDependencyModule
    {
        public void Setup(IDependencyRegistry registry)
        {
            registry.Bind<GameConfiguration>(x => x.AsSingleton());
        }
    }
}