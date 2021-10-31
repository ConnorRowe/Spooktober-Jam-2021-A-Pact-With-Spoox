using Godot;
using System.Collections.Generic;

namespace Spooktober
{
    public class Spook : Node2D
    {
        private static List<AudioStreamSample> spookySamples = new List<AudioStreamSample>();

        static Spook()
        {
            GlobalNodes.LoadFromDirectory<AudioStreamSample>("res://sound/Spooks/", spookySamples);
        }

        public override void _Ready()
        {
            var spookPlayer = GetNode<AudioStreamPlayer>("SpookPlayer");

            spookPlayer.Stream = spookySamples[GlobalNodes.RNG.RandiRange(0, spookySamples.Count - 1)];
            spookPlayer.Play();

            GetNode<AnimationPlayer>("AnimationPlayer").Play("Spook");

            GetTree().CreateTimer(4f).Connect("timeout", this, nameof(KillSelf));
        }

        private void KillSelf()
        {
            QueueFree();
        }
    }
}