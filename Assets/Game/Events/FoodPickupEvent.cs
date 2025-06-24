using EcsR3.Entities;

namespace Game.Events
{
    public class FoodPickupEvent
    {
        public Entity Food { get; private set; } 
        public Entity Player { get; private set; }
        public bool IsSoda { get; private set; }

        public FoodPickupEvent(Entity food, Entity player, bool isSoda)
        {
            Food = food;
            Player = player;
            IsSoda = isSoda;
        }
    }
}