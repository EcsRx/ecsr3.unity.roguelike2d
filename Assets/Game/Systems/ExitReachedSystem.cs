using System.Linq;
using EcsR3.Computeds.Entities;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using EcsR3.Extensions;
using EcsR3.Groups;
using EcsR3.Plugins.GroupBinding.Attributes;
using EcsR3.Systems;
using EcsR3.Unity.Extensions;
using Game.Blueprints;
using Game.Components;
using Game.Events;
using R3;
using SystemsR3.Extensions;
using SystemsR3.Systems.Conventional;

namespace Game.Systems
{
    public class ExitReachedSystem : IReactToEventSystem<ExitReachedEvent>, IManualSystem, IGroupSystem
    {
        public IGroup Group { get; } = new Group(typeof(LevelComponent));

        public IEntityComponentAccessor EntityComponentAccessor { get; }
        
        [FromGroup]
        public IComputedEntityGroup LevelEntityGroup { get; set; }

        private Entity _level;

        public ExitReachedSystem(IEntityComponentAccessor entityComponentAccessor)
        {
            EntityComponentAccessor = entityComponentAccessor;
        }

        public void StartSystem()
        { this.WaitForScene().SubscribeOnce(x => _level = LevelEntityGroup.First()); }

        public void StopSystem()
        {}

        public Observable<ExitReachedEvent> ObserveOn(Observable<ExitReachedEvent> observable)
        { return observable.ObserveOnMainThread(); }

        public void Process(ExitReachedEvent eventData)
        {
            var movementComponent = EntityComponentAccessor.GetComponent<MovementComponent>(eventData.Player);
            movementComponent.StopMovement = true;

            var levelComponent = EntityComponentAccessor.GetComponent<LevelComponent>(_level);
            var currentLevel = levelComponent.Level.Value;
            var levelBlueprint = new LevelBlueprint();
            levelBlueprint.UpdateLevel(levelComponent, currentLevel + 1);
        }
    }
}