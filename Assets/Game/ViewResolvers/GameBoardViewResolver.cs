using EcsR3.Collections.Entities;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using EcsR3.Extensions;
using EcsR3.Groups;
using EcsR3.Unity.Dependencies;
using EcsR3.Unity.Systems;
using Game.Components;
using Game.Extensions;
using Game.Groups;
using Game.SceneCollections;
using SystemsR3.Attributes;
using SystemsR3.Events;
using SystemsR3.Types;
using UnityEngine;

namespace Game.ViewResolvers
{
    [Priority(PriorityTypes.SuperHigh)]
    public class GameBoardViewResolver : DynamicViewResolverSystem
    {
        private readonly FloorTiles _floorTiles;
        private readonly OuterWallTiles _outerWallTiles;
        
        public IEntityComponentAccessor EntityComponentAccessor { get; }

        public override IGroup Group { get; } = new GameBoardGroup();

        public GameBoardViewResolver(IEventSystem eventSystem, IEntityCollection entityCollection, IUnityInstantiator instantiator, FloorTiles floorTiles, OuterWallTiles outerWallTiles, IEntityComponentAccessor entityComponentAccessor) : base(eventSystem, entityCollection, instantiator)
        {
            _floorTiles = floorTiles;
            _outerWallTiles = outerWallTiles;
            EntityComponentAccessor = entityComponentAccessor;
        }

        public override GameObject CreateView(Entity entity)
        {
            var rootView = new GameObject("Board");
            var boardComponent = EntityComponentAccessor.GetComponent<GameBoardComponent>(entity);
            CreateBoardTiles(rootView.transform, boardComponent.Width, boardComponent.Height);
            return rootView;
        }

        public override void DestroyView(Entity entity, GameObject view)
        { Object.Destroy(view); }

        private void CreateBoardTiles(Transform parentContainer, int width, int height)
        {
            var index = 0;
            for (var x = -1; x < width + 1; x++)
            {
                for (var y = -1; y < height + 1; y++)
                {
                    var tileToInstantiate = _floorTiles.AvailableTiles.TakeRandom();

                    if (x == -1 || x == width || y == -1 || y == height)
                    { tileToInstantiate = _outerWallTiles.AvailableTiles.TakeRandom(); }

                    var instance = Object.Instantiate(tileToInstantiate, new Vector3(x, y, 0f), Quaternion.identity);
                    instance.name = $"game-tile-{index}";
                    instance.transform.SetParent(parentContainer);
                    index++;
                }
            }
        }
    }
}