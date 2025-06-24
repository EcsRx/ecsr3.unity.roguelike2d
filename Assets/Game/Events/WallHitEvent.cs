using EcsR3.Entities;

namespace Game.Events
{
    public class WallHitEvent
    {
        public Entity Wall { get; private set; } 
        public Entity Player { get; private set; }

        public WallHitEvent(Entity wall, Entity player)
        {
            Wall = wall;
            Player = player;
        }
    }
}