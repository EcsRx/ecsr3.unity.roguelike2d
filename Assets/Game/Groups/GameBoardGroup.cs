using System;
using EcsR3.Groups;
using EcsR3.Plugins.Views.Components;
using Game.Components;

namespace Game.Groups
{
    public class GameBoardGroup : IGroup
    {
        public Type[] RequiredComponents { get; } = {typeof (ViewComponent), typeof (GameBoardComponent)};
        public Type[] ExcludedComponents { get; } = Array.Empty<Type>();
    }
}