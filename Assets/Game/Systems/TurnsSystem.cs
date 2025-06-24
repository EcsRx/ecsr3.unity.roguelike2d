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
            _isProcessing = true;
            await Task.Delay((int)GameConfiguration.TurnDelay);

            if(!EnemyAccessor.Any())
            { await Task.Delay((int)GameConfiguration.TurnDelay); }

            foreach (var enemy in EnemyAccessor)
            {
                EventSystem.Publish(new EnemyTurnEvent(enemy));
                await Task.Delay((int)GameConfiguration.TurnDelay);
            }

            EventSystem.Publish(new PlayerTurnEvent());

            _isProcessing = false;
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
                    if (_isProcessing) { return; }
                    CarryOutTurns();
                });
        }

        public void StopSystem()
        { _updateSubscription.Dispose(); }
    }
}