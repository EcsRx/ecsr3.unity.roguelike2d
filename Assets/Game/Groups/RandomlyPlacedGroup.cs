using System;
using EcsR3.Groups;
using EcsR3.Plugins.Views.Components;
using Game.Components;

namespace Game.Groups
{
    public class RandomlyPlacedGroup : IGroup
    {
        public Type[] RequiredComponents { get; } = { typeof(ViewComponent), typeof(RandomlyPlacedComponent) };
        public Type[] ExcludedComponents { get; } = Array.Empty<Type>();
    }
}