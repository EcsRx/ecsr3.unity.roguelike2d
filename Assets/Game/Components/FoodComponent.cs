using EcsR3.Components;

namespace Game.Components
{
    public class FoodComponent : IComponent
    {
        public bool IsSoda { get; set; }
        public int FoodAmount { get; set; }
    }
}