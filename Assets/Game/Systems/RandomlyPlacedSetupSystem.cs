using System.Linq;
using EcsR3.Collections.Entities;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using EcsR3.Extensions;
using EcsR3.Groups;
using EcsR3.Systems.Reactive;
using EcsR3.Unity.Extensions;
using Game.Components;
using Game.Extensions;
using Game.Groups;
using SystemsR3.Attributes;

namespace Game.Systems
{
    [Priority(1)]
    public class RandomlyPlacedSetupSystem : ISetupSystem
    {
        private readonly IGroup _gameBoardGroup = new GameBoardGroup();
        public IGroup Group { get; } = new RandomlyPlacedGroup();
        
        public IEntityCollection EntityCollection { get; }
        
        public RandomlyPlacedSetupSystem(IEntityCollection entityCollection)
        {
            EntityCollection = entityCollection;
        }
        
        public void Setup(IEntityComponentAccessor entityComponentAccessor, Entity entity)
        {
            var gameBoardEntity = EntityCollection.GetEntitiesMatching(entityComponentAccessor, _gameBoardGroup).First();
            var gameBoardComponent = entityComponentAccessor.GetComponent<GameBoardComponent>(gameBoardEntity);

            var viewComponent = entityComponentAccessor.GetGameObject(entity);
            var randomlyPlacedComponent = entityComponentAccessor.GetComponent<RandomlyPlacedComponent>(entity);
            var randomPosition = gameBoardComponent.OpenTiles.TakeRandom();
            randomlyPlacedComponent.RandomPosition = randomPosition;
            viewComponent.transform.localPosition = randomPosition;
            gameBoardComponent.OpenTiles.Remove(randomPosition);
        }
    }
}