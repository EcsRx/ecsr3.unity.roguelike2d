using EcsR3.Blueprints;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using EcsR3.Extensions;
using EcsR3.Plugins.Views.Components;
using Game.Components;
using Game.Enums;
using Random = UnityEngine.Random;

namespace Game.Blueprints
{
    public class EnemyBlueprint : IBlueprint
    {
        private EnemyTypes GetRandomEnemyType()
        {
            var enemyValue = Random.Range(0, 2); // Its exclusive on max, ask unity...
            return (EnemyTypes) enemyValue;
        }

        public void Apply(IEntityComponentAccessor entityComponentAccessor, Entity entity)
        {
            var enemyComponent = new EnemyComponent();
            enemyComponent.Health.Value = 3;
            enemyComponent.EnemyType = GetRandomEnemyType();
            enemyComponent.EnemyPower = enemyComponent.EnemyType == EnemyTypes.Regular ? 10 : 20;
            entityComponentAccessor.AddComponents(entity, enemyComponent, new ViewComponent(),
                new MovementComponent(), new RandomlyPlacedComponent());
        }
    }
}