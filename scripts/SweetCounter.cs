using Godot;
using System.Collections.Generic;

namespace Spooktober
{
    public class SweetCounter : Node2D
    {
        public int SweetCount => sweetCount;
        public Dictionary<SweetType, int> SweetCounts => sweetCounts;
        private int sweetCount = 0;
        private AnimationPlayer animPlayer;
        private AudioStreamPlayer popPlayer;

        private Label label;
        private Sprite sprite;

        private Dictionary<SweetType, int> sweetCounts = new Dictionary<SweetType, int>()
        {
            { SweetType.Classic, 0 },
            { SweetType.Heart, 0 },
            { SweetType.Sour, 0 },
            { SweetType.Choc, 0 },
            { SweetType.Fizzer, 0 },
            { SweetType.Bonbons, 0 },
            { SweetType.KolaCubes, 0 },
        };

        public override void _Ready()
        {
            label = GetNode<Label>("Label");
            animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
            sprite = GetNode<Sprite>("Sprite");
            popPlayer = GetNode<AudioStreamPlayer>("PopPlayer");
        }

        public void AddSweet(SweetType sweetType)
        {
            sweetCounts[sweetType]++;
            sweetCount++;

            UpdateCounter();
        }

        private void UpdateCounter()
        {
            string text = "";

            if (sweetCount < 10)
            {
                text = $"0{sweetCount}";
            }
            else
            {
                text = $"{sweetCount}";
            }

            label.Text = text;

            animPlayer.Play("Pulse");
            popPlayer.Play();
        }

        public void SetSweetTexture(Texture texture)
        {
            sprite.Texture = texture;
        }

        public Sweet DropSweet()
        {
            Sweet sweet = Sweet.Classic;

            if (sweetCount <= 0)
                return sweet;

            bool foundSweet = false;

            while (!foundSweet)
            {
                var typeCheck = Sweet.AllSweets[GlobalNodes.RNG.RandiRange(0, 6)];
                if (sweetCounts[typeCheck.SweetType] > 0)
                {
                    sweetCounts[typeCheck.SweetType]--;
                    sweetCount--;
                    sweet = typeCheck;

                    foundSweet = true;
                }
            }

            UpdateCounter();

            return sweet;
        }
    }
}