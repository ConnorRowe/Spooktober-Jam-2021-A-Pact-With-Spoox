using Godot;

namespace Spooktober
{
    public enum SweetType
    {
        Classic,
        Heart,
        Sour,
        Choc,
        Fizzer,
        Bonbons,
        KolaCubes
    }

    public struct Sweet
    {
        public string Name { get; }
        public SweetType @SweetType { get; }
        public int Sweetness { get; }
        public int Sourness { get; }
        public int Magic { get; }

        public Sweet(SweetType sweetType, int sweetness, int sourness, int magic)
        {
            Name = sweetType.GetName();
            SweetType = sweetType;
            Sweetness = sweetness;
            Sourness = sourness;
            Magic = magic;
        }

        public static Sweet Classic { get; } = new Sweet(SweetType.Classic, 3, 0, 1);
        public static Sweet Heart { get; } = new Sweet(SweetType.Heart, 2, 1, 2);
        public static Sweet Sour { get; } = new Sweet(SweetType.Sour, 0, 3, 1);
        public static Sweet Choc { get; } = new Sweet(SweetType.Choc, 2, 0, 2);
        public static Sweet Fizzer { get; } = new Sweet(SweetType.Fizzer, 1, 2, 2);
        public static Sweet Bonbons { get; } = new Sweet(SweetType.Bonbons, 2, 0, 0);
        public static Sweet KolaCubes { get; } = new Sweet(SweetType.KolaCubes, 2, 2, 2);

        public static Sweet[] AllSweets { get; } = new Sweet[7] { Classic, Heart, Sour, Choc, Fizzer, Bonbons, KolaCubes };
    }

    public static class SweetTypeExtensions
    {
        public static Texture classicTex = GD.Load<Texture>("res://textures/Sweets/sweet_happy.png");
        public static Texture heartTex = GD.Load<Texture>("res://textures/Sweets/sweet_heart.png");
        public static Texture sourTex = GD.Load<Texture>("res://textures/Sweets/sweet_sour.png");
        public static Texture chocoTex = GD.Load<Texture>("res://textures/Sweets/sweet_choc.png");
        public static Texture fizzTex = GD.Load<Texture>("res://textures/Sweets/sweet_fizzer.png");
        public static Texture bonbonTex = GD.Load<Texture>("res://textures/Sweets/sweet_bonbons.png");
        public static Texture kolaTex = GD.Load<Texture>("res://textures/Sweets/sweet_kolacubes.png");

        public static string GetName(this SweetType s)
        {
            switch (s)
            {
                case SweetType.Classic:
                    return "Classic sweet";
                case SweetType.Heart:
                    return "Pink heart";
                case SweetType.Sour:
                    return "Super Sour";
                case SweetType.Choc:
                    return "Choco Bar";
                case SweetType.Fizzer:
                    return "Fizzers";
                case SweetType.Bonbons:
                    return "Bonbons";
                case SweetType.KolaCubes:
                    return "Kola Cubes";
            }

            return "ERROR";
        }

        public static Texture GetTexture(this SweetType s)
        {
            switch (s)
            {
                case SweetType.Classic:
                    return classicTex;
                case SweetType.Heart:
                    return heartTex;
                case SweetType.Sour:
                    return sourTex;
                case SweetType.Choc:
                    return chocoTex;
                case SweetType.Fizzer:
                    return fizzTex;
                case SweetType.Bonbons:
                    return bonbonTex;
                case SweetType.KolaCubes:
                    return kolaTex;
            }

            return null;
        }

        public static int ToInt(this SweetType s)
        {
            switch (s)
            {
                case SweetType.Classic:
                    return 0;
                case SweetType.Heart:
                    return 1;
                case SweetType.Sour:
                    return 2;
                case SweetType.Choc:
                    return 3;
                case SweetType.Fizzer:
                    return 4;
                case SweetType.Bonbons:
                    return 5;
                case SweetType.KolaCubes:
                    return 6;
            }

            return -1;
        }

        public static SweetType FromInt(int id)
        {
            switch (id)
            {
                case 0:
                    return SweetType.Classic;
                case 1:
                    return SweetType.Heart;
                case 2:
                    return SweetType.Sour;
                case 3:
                    return SweetType.Choc;
                case 4:
                    return SweetType.Fizzer;
                case 5:
                    return SweetType.Bonbons;
                case 6:
                    return SweetType.KolaCubes;
            }

            return SweetType.Classic;
        }
    }
}