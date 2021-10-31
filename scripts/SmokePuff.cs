using Godot;

namespace Spooktober
{
    public class SmokePuff : Particles2D
    {
        public void Start()
        {
            Emitting = true;
            GetTree().CreateTimer(1.2f).Connect("timeout", this, nameof(Die));
            GetNode<AudioStreamPlayer>("AudioStreamPlayer").Play();
        }

        private void Die()
        {
            QueueFree();
        }
    }
}