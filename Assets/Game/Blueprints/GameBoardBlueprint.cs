using EcsR3.Blueprints;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using EcsR3.Extensions;
using EcsR3.Plugins.Views.Components;
using Game.Components;

namespace Game.Blueprints
{
    public class GameBoardBlueprint : IBlueprint
    {
        private readonly int _width;
        private readonly int _height;

        public GameBoardBlueprint(int width = 8, int height = 8)
        {
            _width = width;
            _height = height;
        }

        public void Apply(IEntityComponentAccessor entityComponentAccessor, Entity entity)
        {
            var gameBoardComponent = new GameBoardComponent
            {
                Width = _width,
                Height = _height
            };

            entityComponentAccessor.AddComponents(entity, gameBoardComponent, new ViewComponent());
        }
    }
}