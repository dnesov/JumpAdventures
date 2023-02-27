using Godot;

namespace Jump.Utils
{
    public class ParticleSpawner : Node
    {
        public void SpawnParticlesAt(Particles2D particles, Vector2 at)
        {
            AddChild(particles);
            particles.GlobalPosition = at;
            particles.Restart();
        }
    }
}