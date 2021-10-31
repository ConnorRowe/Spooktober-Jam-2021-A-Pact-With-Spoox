using Godot;

namespace Spooktober
{
    public class Crow : AnimatedSprite
    {
        private const float flySpeed = 100f;

        public bool IsFlying { get { return isFlying; } set { isFlying = value; IsFlyingChanged(); } }
        private bool isFlying = false;
        private bool isLeft = true;

        private Timer timer;

        public override void _Ready()
        {
            timer = GetNode<Timer>("Timer");
            timer.Connect("timeout", this, nameof(Timeout));
			timer.Start();

            Timeout();
        }

        public override void _Process(float delta)
        {
            if (isFlying)
            {
                Position += new Vector2(isLeft ? -flySpeed : flySpeed, -flySpeed) * delta;
            }

            if (Position.y > 0f)
            {
                QueueFree();
            }
        }

        private void Timeout()
        {
            Frame = 0;
			Play();

            timer.WaitTime = GlobalNodes.RNG.RandfRange(2f, 6f);

            isLeft = GlobalNodes.RNG.Randf() > .75f;
            FlipH = !isLeft;
        }

        private void IsFlyingChanged()
        {
            if (isFlying)
            {
                timer.Stop();

                Animation = "flyaway";
            }
        }
    }
}