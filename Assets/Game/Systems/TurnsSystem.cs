using System;
using System.Linq;
using System.Threading.Tasks;
using EcsR3.Computeds.Entities;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using EcsR3.Extensions;
using EcsR3.Plugins.GroupBinding.Attributes;
using EcsR3.Unity.Extensions;
using Game.Components;
using Game.Configuration;
using Game.Events;
using R3;
using SystemsR3.Events;
using SystemsR3.Extensions;
using SystemsR3.Systems.Conventional;

namespace Game.Systems
{
    public class TurnsSystem : IManualSystem
    {
        public GameConfiguration GameConfiguration { get; }
        public IEventSystem EventSystem { get; }
        public IEntityComponentAccessor EntityComponentAccessor { get; }

        private IDisposable _updateSubscription;
        private bool _isProcessing;
        
        [FromComponents(typeof (LevelComponent))]
        public IComputedEntityGroup LevelAccessor;
        
        [FromComponents(typeof(EnemyComponent))]
        public IComputedEntityGroup EnemyAccessor;
        
        private Entity _level;

        public TurnsSystem(GameConfiguration gameConfiguration, IEventSystem eventSystem, IEntityComponentAccessor entityComponentAccessor)
        {
            GameConfiguration = gameConfiguration;
            EventSystem = eventSystem;
            EntityComponentAccessor = entityComponentAccessor;
        }
        
        private async void CarryOutTurns()
        {
            if (_isProcessing) { return; }
            
            _isProcessing = true;
            await Task.Delay((int)GameConfiguration.TurnDelay);

            if(!EnemyAccessor.Any())
            { await Task.Delay((int)GameConfiguration.TurnDelay); }

            foreach (var enemy in EnemyAccessor)
            {
                EventSystem.Publish(new EnemyTurnEvent(enemy));
                await Task.Delay((int)GameConfiguration.TurnDelay);
            }

            EventSystem
                .Receive<PlayerTurnOverEvent>()
                .SubscribeOnce(x => _isProcessing = false);
            
            EventSystem.Publish(new PlayerTurnEvent());
        }

        private bool IsLevelLoaded()
        {
            var levelComponent = EntityComponentAccessor.GetComponent<LevelComponent>(_level);
            return levelComponent != null && levelComponent.HasLoaded.Value;
        }

        public void StartSystem()
        {
            this.WaitForScene().Subscribe(x => _level = LevelAccessor.First());
            
            _updateSubscription = Observable.EveryUpdate()
                .Where(x => IsLevelLoaded())
                .Subscribe(x => {
                    CarryOutTurns();
                });
        }

        public void StopSystem()
        { _updateSubscription.Dispose(); }
    }
}