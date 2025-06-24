using EcsR3.Entities;

namespace Game.Events
{
    public class PlayerKilledEvent
    {
        public Entity Player { get; private set; }

        public PlayerKilledEvent(Entity player)
        {
            Player = player;
        }
    }
}