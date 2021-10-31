using Godot;

namespace Spooktober
{
    public class DroppedSweet : RigidBody2D
    {
        private float height = 0;
        private float bounceVelocity = 0f;
        private Sprite sprite;
        private Sprite shadow;
        public Sweet Sweet { get; set; }

        public override void _Ready()
        {
            sprite = GetNode<Sprite>("Sprite");
            shadow = GetNode<Sprite>("Shadow");

            GetTree().CreateTimer(2f).Connect("timeout", this, nameof(EnableSweetPickup));
        }

        private void EnableSweetPickup()
        {
            CollisionLayer = 1 + 2 + 4;
        }

        public void Start(float height, Vector2 impulse, Sweet sweet)
        {
            this.height = height;
            ApplyCentralImpulse(impulse);
            GetNode<Sprite>("Sprite").Texture = sweet.SweetType.GetTexture();
            Sweet = sweet;
        }

        public override void _Process(float delta)
        {
            height += bounceVelocity * delta;

            if (height <= 0.0f)
            {
                height = 0.0f;
                bounceVelocity *= -0.5f;
            }
            else
                bounceVelocity -= 9.8f * 30f * delta;

            sprite.Offset = new Vector2(0, Mathf.Min(-6 - height, -6));

            shadow.Frame = Mathf.Min(Mathf.RoundToInt(height / 32f * 4f), 3);
        }
    }
}