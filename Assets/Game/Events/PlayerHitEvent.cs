using EcsR3.Entities;

namespace Game.Events
{
    public class PlayerHitEvent
    {
        public Entity Enemy { get; private set; }
        public Entity Player { get; private set; }

        public PlayerHitEvent(Entity player, Entity enemy)
        {
            Enemy = enemy;
            Player = player;
        }
    }
}