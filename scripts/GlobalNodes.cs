using Godot;
using System.Collections.Generic;

namespace Spooktober
{
    public class GlobalNodes : Node
    {
        public static RandomNumberGenerator RNG = new RandomNumberGenerator();

        private static PackedScene smokePuffScene = GD.Load<PackedScene>("res://scenes/SmokePuff.tscn");

        static GlobalNodes()
        {
            RNG.Randomize();
        }

        public AudioStreamPlayer RandomPitchPlayer { get; set; }
        public AudioStreamRandomPitch RandomSample { get; set; }

        public static Dictionary<SweetType, int> SweetCounts { get; set; } = new Dictionary<SweetType, int>()
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
            RandomPitchPlayer = GetNode<AudioStreamPlayer>("RandomPitchPlayer");
            RandomSample = (AudioStreamRandomPitch)RandomPitchPlayer.Stream;
        }

        public override void _Input(InputEvent evt)
        {
            base._Input(evt);

            if (evt is InputEventKey ek && !ek.Pressed && (ek.Scancode == (uint)KeyList.F11 || (ek.Scancode == (uint)KeyList.Enter && ek.Alt)))
            {
                OS.WindowFullscreen = !OS.WindowFullscreen;
            }
        }

        public static void LoadFromDirectory<T>(string fileDirectory, System.Collections.Generic.HashSet<T> objectSet) where T : Godot.Object
        {
            Directory directory = new Directory();
            directory.Open(fileDirectory);
            directory.ListDirBegin(skipNavigational: true);

            string file = directory.GetNext();
            do
            {
                // remove .import extension
                if (file.Substring(file.Length - 7) == ".import")
                    file = file.Substring(0, file.Length - 7);

                objectSet.Add(GD.Load<T>(fileDirectory + file));

                file = directory.GetNext();
            } while (!file.Empty());
        }

        public static void LoadFromDirectory<T>(string fileDirectory, System.Collections.Generic.List<T> objectList) where T : Godot.Object
        {
            System.Collections.Generic.HashSet<T> objects = new System.Collections.Generic.HashSet<T>();
            LoadFromDirectory<T>(fileDirectory, objects);
            foreach (T o in objects)
            {
                objectList.Add(o);
            }
        }

        public static void MakeSmokePuff(Node parent, Vector2 position)
        {
            SmokePuff smokePuff = smokePuffScene.Instance<SmokePuff>();
            parent.AddChild(smokePuff);
            smokePuff.Position = position;
            smokePuff.Start();
        }
    }
}