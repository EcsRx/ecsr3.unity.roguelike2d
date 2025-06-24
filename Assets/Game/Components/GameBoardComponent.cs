using System.Collections.Generic;
using EcsR3.Components;
using UnityEngine;

namespace Game.Components
{
    public class GameBoardComponent : IComponent
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public IList<Vector3> OpenTiles { get; set; }

        public GameBoardComponent()
        {
            OpenTiles = new List<Vector3>();
        }
    }
}