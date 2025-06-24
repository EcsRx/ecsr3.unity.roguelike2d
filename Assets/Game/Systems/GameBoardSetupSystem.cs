using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using EcsR3.Extensions;
using EcsR3.Groups;
using EcsR3.Systems.Reactive;
using Game.Components;
using Game.Groups;
using SystemsR3.Attributes;
using SystemsR3.Types;
using UnityEngine;

namespace Game.Systems
{
    [Priority(PriorityTypes.High)]
    public class GameBoardSetupSystem : ISetupSystem
    {
        public IGroup Group { get; } = new GameBoardGroup();

        public void Setup(IEntityComponentAccessor entityComponentAccessor, Entity entity)
        {
            var gameBoardComponent = entityComponentAccessor.GetComponent<GameBoardComponent>(entity);

            for (var x = 1; x < gameBoardComponent.Width - 1; x++)
            {
                for (var y = 1; y < gameBoardComponent.Height - 1; y++)
                {
                    gameBoardComponent.OpenTiles.Add(new Vector3(x, y, 0f));
                }
            }
        }
    }
}