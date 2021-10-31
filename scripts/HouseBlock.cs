using Godot;
using System.Collections.Generic;

namespace Spooktober
{
    public class HouseBlock : YSort
    {
        private static readonly Color[] houseColours = new Color[8] { new Color("63ab3f"), new Color("4fa4b8"), new Color("f0b541"), new Color("e64539"), new Color("9c2a70"), new Color("ff5277"), new Color("404973"), new Color("8f4d57") };
        private static Texture darkDoorTex;
        private static Texture darkWindowTex;
        private static Texture lightDoorTex;
        private static Texture lightWindowTex;
        private static Texture openDoorTex;
        private static PackedScene murderScene; // lol
        private static AudioStreamSample lightSwitchSample;
        private static PackedScene droppedSweetScene = GD.Load<PackedScene>("res://scenes/DroppedSweet.tscn");
        private static PackedScene spookScene = GD.Load<PackedScene>("res://scenes/Spook.tscn");
        private static List<Texture> bigPumpkinVariants = new List<Texture>();
        private static List<Texture> bannerVariants = new List<Texture>();
        private static List<Texture> spiderwebVariants= new List<Texture>();
        private static List<AudioStreamSample> doorOpenSamples = new List<AudioStreamSample>();
        private static List<AudioStreamSample> doorCloseSamples = new List<AudioStreamSample>();

        static HouseBlock()
        {
            GlobalNodes.LoadFromDirectory<Texture>("res://textures/BigPumpkins/", bigPumpkinVariants);
            GlobalNodes.LoadFromDirectory<Texture>("res://textures/Banners/", bannerVariants);
            GlobalNodes.LoadFromDirectory<Texture>("res://textures/Spiderwebs/", spiderwebVariants);
            GlobalNodes.LoadFromDirectory<AudioStreamSample>("res://sound/DoorOpen/", doorOpenSamples);
            GlobalNodes.LoadFromDirectory<AudioStreamSample>("res://sound/DoorClose/", doorCloseSamples);
            darkDoorTex = GD.Load<Texture>("res://textures/door_dark.png");
            darkWindowTex = GD.Load<Texture>("res://textures/house_window_dark.png");
            lightDoorTex = GD.Load<Texture>("res://textures/door.png");
            lightWindowTex = GD.Load<Texture>("res://textures/house_window.png");
            openDoorTex = GD.Load<Texture>("res://textures/door_open.png");
            murderScene = GD.Load<PackedScene>("res://scenes/MurderOfCrows.tscn");
            lightSwitchSample = GD.Load<AudioStreamSample>("res://sound/light switch.wav");
        }

        private House house1;
        private House house2;

        private GlobalNodes globalNodes;
        private World world;
        private RandomNumberGenerator rng;

        public override void _Ready()
        {
            base._Ready();

            rng = GlobalNodes.RNG;
            globalNodes = GetNode<GlobalNodes>("/root/GlobalNodes");
            world = (World)GetTree().CurrentScene;

            house1 = new House(true, LowWeightedRandiRange(1, 5), rng.Randf() > .55f, rng.Randf() < .65f, GetNode<Sprite>("HouseWall1"), GetNode<Sprite>("Door1"), GetNode<Sprite>("Window1"), GetNode<Node2D>("House1Decs"));
            house2 = new House(false, LowWeightedRandiRange(1, 5), rng.Randf() > .55f, rng.Randf() < .65f, GetNode<Sprite>("HouseWall2"), GetNode<Sprite>("Door2"), GetNode<Sprite>("Window2"), GetNode<Node2D>("House2Decs"));

            GenerateHouse(house1);
            GenerateHouse(house2);

            // Remove walls
            if (rng.Randf() > .75f)
            {
                GetNode("VertWall1").QueueFree();
                GetNode("WallColliders/CollisionPolygon2D3").QueueFree();
            }
            if (rng.Randf() > .75f)
            {
                GetNode("VertWall2").QueueFree();
                GetNode("WallColliders/CollisionPolygon2D4").QueueFree();
            }
        }

        public void GenerateHouse(House house)
        {
            house.Wall.Modulate = houseColours[rng.RandiRange(0, houseColours.Length - 1)];

            if (!house.LightsOn || !house.AreIn)
            {
                house.Door.Texture = darkDoorTex;
                house.Window.Texture = darkWindowTex;

                if (house.SweetLevel <= 2)
                    house.Haunted = true;
                else if (rng.Randf() < .25f)
                    house.Haunted = true;

            }

            // Select random crow position out of the 3

            Vector2 crowPos = ((Node2D)house.CrowPositions.GetChild(rng.RandiRange(0, 2))).Position;
            MurderOfCrows murder = murderScene.Instance<MurderOfCrows>();
            AddChild(murder);
            murder.Position = crowPos;
            house.CrowPositions.GetParent().RemoveChild(house.CrowPositions);
            house.CrowPositions.QueueFree();

            List<Node2D> decorations = new List<Node2D>() { house.PumpByTheDoor, house.PumpOnWall, house.PumpOnWallMini, house.PumpUnderWindow, house.PumpWindowsillMini, house.Banner, house.Spiderweb, murder };
            int decsToRemove = decorations.Count;
            float decsShowPercent = ((float)house.SweetLevel - 1) * .25f; // 1/4
            GD.Print($"decsShowPercent={decsShowPercent}");
            decsToRemove = decsToRemove - Mathf.CeilToInt(decsShowPercent * (float)decsToRemove);

            GD.Print($"SweetLevel={house.SweetLevel}, decsToRemove = {decsToRemove}, decorations.Count={decorations.Count}");

            for (int i = 0; i < decsToRemove; i++)
            {
                if (decorations.Count > 0)
                {
                    int killIdx = rng.RandiRange(0, decorations.Count - 1);
                    Node2D dec = decorations[killIdx];
                    dec.QueueFree();
                    decorations.RemoveAt(killIdx);
                }
            }

            GD.Print($"Remaining decorations:{decorations.Count}");

            // Parse remaining decorations for special stuff
            foreach (Node2D dec in decorations)
            {
                if (dec.IsInGroup("BigPumpkin") && dec is Sprite bigPumpkin)
                {
                    bigPumpkin.Texture = bigPumpkinVariants[rng.RandiRange(0, bigPumpkinVariants.Count - 1)];
                }
                else if (dec.Name == "Banner" && dec is Sprite banner)
                {
                    banner.Texture = bannerVariants[rng.RandiRange(0, bannerVariants.Count - 1)];
                }
                else if (dec.Name == "Spiderweb" && dec is Sprite spiderweb)
                {
                    spiderweb.Texture = spiderwebVariants[rng.RandiRange(0, spiderwebVariants.Count - 1)];
                }
            }
        }

        private void KnockOnDoor(House house)
        {
            if (house.GivenOutSweets)
                return;

            house.KnockedOn = true;

            if (house.Haunted)
            {
                float delay = rng.RandfRange(.5f, 1f);

                GetTree().CreateTimer(delay).Connect("timeout", this, nameof(HauntPlayer), new Godot.Collections.Array(house));
            }

            if (house.AreIn && !house.GivenOutSweets && !house.Haunted)
            {
                float doorDelay = rng.RandfRange(1f, 2f);

                if (!house.LightsOn)
                {
                    float lightDelay = rng.RandfRange(.5f, 4f);

                    doorDelay += lightDelay;

                    TurnOnLightsDelay(house, lightDelay);
                }

                OpenDoorDelay(house, doorDelay);
            }
        }

        private void HauntPlayer(House house)
        {
            GD.Print("Haunt player!");

            house.GivenOutSweets = true;

            // say boo!
            // knock back player
            world.Player.ApplyKnockback(Vector2.Down * 200f);
            // spawn ghost - one main one, one transparant one behind that scales over time
            var spook = spookScene.Instance<Spook>();
            world.YSort.AddChild(spook);
            spook.Position = house.Door.GlobalPosition + new Vector2(34, 0);

            GlobalNodes.MakeSmokePuff(world.YSort, spook.Position);

            // drop some 1 - 3 sweets, can pick them back up but wastes time
            for (int i = rng.RandiRange(0, 2); i < 3; i++)
            {
                if (world.SweetCounter.SweetCount > 0)
                {
                    GD.Print("Dropped sweet");
                    SpawnDroppedSweet(world.SweetCounter.DropSweet());
                }
            }
        }

        private void TurnOnLightsDelay(House house, float delay)
        {
            GetTree().CreateTimer(delay).Connect("timeout", this, nameof(TurnOnLights), new Godot.Collections.Array(house));
        }

        private void TurnOnLights(House house)
        {
            house.Door.Texture = lightDoorTex;
            house.Window.Texture = lightWindowTex;
            house.LightsOn = true;

            globalNodes.RandomSample.AudioStream = lightSwitchSample;
            globalNodes.RandomPitchPlayer.Play();
        }

        private void OpenDoorDelay(House house, float delay)
        {
            GetTree().CreateTimer(delay).Connect("timeout", this, nameof(OpenDoor), new Godot.Collections.Array(house));
        }

        private void OpenDoor(House house)
        {
            GD.Print("Open door");

            house.Door.Texture = openDoorTex;
            house.Person.Visible = true;
            house.Person.Frame = 0;

            globalNodes.RandomSample.AudioStream = doorOpenSamples[rng.RandiRange(0, doorOpenSamples.Count - 1)];
            globalNodes.RandomPitchPlayer.Play();

            float delay = rng.RandfRange(.25f, .75f);
            GetTree().CreateTimer(delay).Connect("timeout", this, nameof(CheckPlayerNearDoor), new Godot.Collections.Array(house));
        }

        private void CloseDoor(House house)
        {
            house.Person.Visible = false;
            house.Door.Texture = lightDoorTex;

            globalNodes.RandomSample.AudioStream = doorCloseSamples[rng.RandiRange(0, doorCloseSamples.Count - 1)];
            globalNodes.RandomPitchPlayer.Play();
        }

        private void CheckPlayerNearDoor(House house)
        {
            if (house.GivenOutSweets)
            {
                CloseDoor(house);
                house.DoorCheckCount = 0;
                return;
            }

            bool playerNearDoor = IsPlayerNearDoor(house);

            switch (house.DoorCheckCount)
            {
                case 0:
                    if (!playerNearDoor)
                    {
                        house.Person.Frame = 2;

                        house.DoorCheckCount++;

                        GetTree().CreateTimer(1f).Connect("timeout", this, nameof(CheckPlayerNearDoor), new Godot.Collections.Array(house));
                    }
                    break;
                case 1:
                    if (!playerNearDoor)
                    {
                        house.Person.Frame = 3;

                        house.DoorCheckCount++;

                        GetTree().CreateTimer(1f).Connect("timeout", this, nameof(CheckPlayerNearDoor), new Godot.Collections.Array(house));
                    }
                    break;
                case 2:
                    if (!playerNearDoor)
                    {
                        CloseDoor(house);
                        house.DoorCheckCount = 0;
                    }
                    break;
                case 3:
                    house.Person.Frame = 0;
                    house.DoorCheckCount++;

                    GetTree().CreateTimer(1f).Connect("timeout", this, nameof(CheckPlayerNearDoor), new Godot.Collections.Array(house));
                    break;
                case 4:
                    CloseDoor(house);
                    house.DoorCheckCount = 0;
                    return;
            }

            if (playerNearDoor && house.DoorCheckCount < 3)
            {
                house.Person.Frame = 1;

                house.GivenOutSweets = true;

                SpawnSweets(house.SweetLevel, house);

                house.DoorCheckCount = 3;

                GetTree().CreateTimer(2f).Connect("timeout", this, nameof(CheckPlayerNearDoor), new Godot.Collections.Array(house));
            }
        }

        private void SpawnSweets(int count, House house)
        {
            Vector2 pos = new Vector2(640 / 2, world.Player.Position.y - 16f);

            world.CreateSweetScreenElement(Sweet.AllSweets[rng.RandiRange(0, Sweet.AllSweets.Length - 1)], pos);

            GD.Print(pos.ToString());

            var newCount = count - 1;

            if (newCount > 0)
                GetTree().CreateTimer(.25f).Connect("timeout", this, nameof(SpawnSweets), new Godot.Collections.Array(newCount, house));
        }

        private bool IsPlayerNearDoor(House house)
        {
            var area = world.Player.DoorKnockArea;
            if (area != null && area.IsInGroup("House1") && house == house1 && house.IsHouseOne)
            {
                return true;
            }
            else if (area != null && area.IsInGroup("House2") && house == house2 && !house.IsHouseOne)
            {
                return true;
            }

            return false;
        }

        public void KnockOnDoor(Area2D area)
        {
            if (area == GetNode<Area2D>("DoorKnockArea1"))
            {
                KnockOnDoor(house1);
            }
            else if (area == GetNode<Area2D>("DoorKnockArea2"))
            {
                KnockOnDoor(house2);
            }
        }

        private int LowWeightedRandiRange(int min, int max)
        {
            return Mathf.RoundToInt(min + (max - min) * Mathf.Pow(rng.Randf(), 1.5f));
        }

        private void SpawnDroppedSweet(Sweet sweet)
        {
            DroppedSweet newSweet = droppedSweetScene.Instance<DroppedSweet>();

            var angle = new Vector2(rng.Randf(), rng.Randf()).Normalized();
            newSweet.Position = world.Player.Position + (angle * 20f);

            world.YSort.AddChild(newSweet);
            newSweet.Start(rng.RandfRange(72f, 144), angle * rng.RandfRange(40f, 60f), sweet);
        }
    }
}