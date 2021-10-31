using Godot;

namespace Spooktober
{
    public class House : Godot.Reference
    {
        public bool IsHouseOne { get; set; }
        public int SweetLevel { get; set; }
        public bool LightsOn { get; set; }
        public bool AreIn { get; set; }
        public bool KnockedOn { get; set; } = false;
        public Sprite Wall { get; set; }
        public Sprite Door { get; set; }
        public AnimatedSprite Person { get; set; }
        public Sprite Window { get; set; }
        public Sprite Banner { get; set; }
        public Sprite Spiderweb { get; set; }
        public Sprite PumpByTheDoor { get; set; }
        public Sprite PumpUnderWindow { get; set; }
        public Sprite PumpOnWall { get; set; }
        public Sprite PumpOnWallMini { get; set; }
        public Sprite PumpWindowsillMini { get; set; }
        public Node2D CrowPositions { get; set; }
        public int DoorCheckCount { get; set; } = 0;
        public bool GivenOutSweets { get; set; } = false;
        public bool Haunted { get; set; } = false;

        public House(bool isHouseOne, int sweetLevel, bool lightsOn, bool areIn, Sprite wall, Sprite door, Sprite window, Node2D decs)
        {
            IsHouseOne = isHouseOne;
            SweetLevel = sweetLevel;
            LightsOn = lightsOn;
            AreIn = areIn;
            Wall = wall;
            Door = door;
            Person = door.GetNode<AnimatedSprite>("DoorPerson");
            Window = window;
            Banner = window.GetNode<Sprite>("Banner");
            Spiderweb = window.GetNode<Sprite>("Spiderweb");
            PumpByTheDoor = decs.GetNode<Sprite>("ByTheDoor");
            PumpUnderWindow = decs.GetNode<Sprite>("UnderTheWindow");
            PumpOnWall = decs.GetNode<Sprite>("OnTheWall");
            PumpOnWallMini = decs.GetNode<Sprite>("OnTheWallMini");
            PumpWindowsillMini = decs.GetNode<Sprite>("WindowsillMini");
            CrowPositions = decs.GetNode<Node2D>("PossibleCrows");
        }

        public House() { }
    }
}