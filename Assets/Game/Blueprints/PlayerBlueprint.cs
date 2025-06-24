using EcsR3.Blueprints;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using EcsR3.Extensions;
using EcsR3.Plugins.Views.Components;
using Game.Components;

namespace Game.Blueprints
{
    public class PlayerBlueprint : IBlueprint
    {
        private readonly int _playerFood;

        public PlayerBlueprint(int playerFood)
        {
            _playerFood = playerFood;
        }

        public void Apply(IEntityComponentAccessor entityComponentAccessor, Entity entity)
        {
            var playerComponent = new PlayerComponent();
            playerComponent.Food.Value = _playerFood;
            entityComponentAccessor.AddComponents(entity, playerComponent, new ViewComponent(), new MovementComponent());

#if UNITY_STANDALONE || UNITY_WEBPLAYER
            entityComponentAccessor.CreateComponent<StandardInputComponent>(entity);
#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
            entityComponentAccessor.CreateComponent<TouchInputComponent>(entity);
#endif
        }
    }
}