using Godot;
using System.Collections.Generic;

namespace Jump.Entities
{
    [Tool]
    public class CollectibleContainer : Node
    {
        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            RegenerateIds();
        }

        private void RegenerateIds()
        {
            int counter = 0;
            foreach (Collectible c in GetChildren())
            {
                counter++;
                c.Id = counter;
            }
        }
    }
}
