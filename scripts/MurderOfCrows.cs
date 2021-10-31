using Godot;

namespace Spooktober
{
    public class MurderOfCrows : Area2D
    {
        private static System.Collections.Generic.List<AudioStream> flapSounds = new System.Collections.Generic.List<AudioStream>();

        static MurderOfCrows()
        {
            GlobalNodes.LoadFromDirectory<AudioStream>("res://sound/Flap/", flapSounds);
        }

        private const int maxCrows = 5;
        private Crow[] crows = new Crow[5];
        private bool activated = false;
        private GlobalNodes globalNodes;
        private RandomNumberGenerator rng;

        public override void _Ready()
        {
            globalNodes = GetNode<GlobalNodes>("/root/GlobalNodes");
            rng = GlobalNodes.RNG;
            int crowCount = rng.RandiRange(1, 5);

            var crowScene = GD.Load<PackedScene>("res://scenes/Crow.tscn");
            var ySort = GetNode<YSort>("YSort");

            for (int i = 0; i < crowCount; i++)
            {
                var crow = crowScene.Instance<Crow>();
                ySort.AddChild(crow);
                crow.Position = new Vector2(rng.RandfRange(-50f, 50f), rng.RandfRange(-18f, 18f));
                crows[i] = crow;
            }

            Connect("body_entered", this, nameof(BodyEntered));
        }

        private void BodyEntered(Node body)
        {
            if (!activated && body is Player)
            {
                activated = true;
                foreach (Crow crow in crows)
                {
                    if (IsInstanceValid(crow))
                    {
                        crow.IsFlying = true;
                    }
                }

                if (rng.Randf() <= .25f)
                    GetNode<AudioStreamPlayer>("CawPlayer").Play();

                globalNodes.RandomSample.AudioStream = flapSounds[rng.RandiRange(0, flapSounds.Count-1)];
                globalNodes.RandomPitchPlayer.Play();
            }
        }
    }
}