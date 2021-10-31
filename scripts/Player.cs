using Godot;
using System.Collections.Generic;

namespace Spooktober
{
    public class Player : KinematicBody2D
    {
        [Export]
        public bool IsFacingLeft { get; set; } = true;
        public Area2D DoorKnockArea { get { return doorKnockArea; } }

        private Vector2 inputDir = Vector2.Zero;
        private float acceleration = 500f;
        private float maxSpeed = 500f;
        private float movementDampening = 8f;
        private Vector2 velocity = Vector2.Zero;
        private Vector2 externalVelocity = Vector2.Zero;
        private bool canKnock = true;

        private AnimatedSprite charSprite;
        private AnimationPlayer animPlayer;
        private Area2D doorKnockArea = null;
        private AudioStreamPlayer footstepPlayer;
        private AudioStreamPlayer knockPlayer;
        private Label debugLabel;
        private World world;
        private Sprite shadow;

        private List<AudioStream> knockSamples = new List<AudioStream>();

        public override void _Ready()
        {
            base._Ready();

            GlobalNodes.LoadFromDirectory<AudioStream>("res://sound/Knocks/", knockSamples);

            world = (World)GetTree().CurrentScene;
            charSprite = GetNode<AnimatedSprite>("CharSprite");
            animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
            footstepPlayer = GetNode<AudioStreamPlayer>("FootstepPlayer");
            knockPlayer = GetNode<AudioStreamPlayer>("KnockPlayer");
            shadow = GetNode<Sprite>("Shadow");

            animPlayer.CurrentAnimation = "Flip";

            debugLabel = GetTree().CurrentScene.GetNode<Label>("CanvasLayer/debugLabel");

            GetNode<Area2D>("DoorInteraction").Connect("area_entered", this, nameof(AreaEntered));
            GetNode<Area2D>("DoorInteraction").Connect("area_exited", this, nameof(AreaExited));
            charSprite.Connect("frame_changed", this, nameof(FrameChanged));

            GetNode<Area2D>("SweetCollector").Connect("body_entered", this, nameof(SweetBodyEntered));
        }

        public override void _Process(float delta)
        {
            shadow.Scale = new Vector2(charSprite.Scale.x, 1f);
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            inputDir = GetInputDir();

            // Dampening
            velocity -= (velocity * movementDampening * delta);
            externalVelocity -= (externalVelocity * 4f * delta);

            // Acceleration
            velocity += inputDir * acceleration * delta;

            // Cap speed
            float velLength = velocity.Length();
            if (velLength > maxSpeed)
                velocity = velocity.Normalized() * maxSpeed;

            // Actually move
            MoveAndSlide(velocity + externalVelocity);

            if (velLength > 2f)
            {
                charSprite.SpeedScale = (velLength * delta) / 1.5f;

                if (velocity.x > 0 != IsFacingLeft && !animPlayer.IsPlaying())
                {
                    if (IsFacingLeft)
                        animPlayer.PlayBackwards();
                    else
                        animPlayer.Play();

                    IsFacingLeft = velocity.x > 0;
                }
            }
            else
                charSprite.Frame = 3;
        }

        public override void _Input(InputEvent evt)
        {
            if (evt.IsActionReleased("g_knock_door") && doorKnockArea != null)
            {
                KnockOnDoor();

                world.HideControls();
            }
        }

        private Vector2 GetInputDir()
        {
            var dir = Vector2.Zero;

            if (world.GameEnded)
                return dir;

            if (Input.IsActionPressed("g_move_up"))
                dir += new Vector2(0, -1);
            if (Input.IsActionPressed("g_move_down"))
                dir += new Vector2(0, 1);
            if (Input.IsActionPressed("g_move_left"))
                dir += new Vector2(-1, 0);
            if (Input.IsActionPressed("g_move_right"))
                dir += new Vector2(1, 0);

            return dir.Normalized();
        }

        public void ApplyKnockback(Vector2 knockback)
        {
            externalVelocity += knockback;
        }

        private void AreaEntered(Area2D area)
        {
            if (area.Owner is HouseBlock)
            {
                doorKnockArea = area;
            }
        }

        private void AreaExited(Area2D area)
        {
            if (area.Owner is HouseBlock)
            {
                doorKnockArea = null;
            }
        }

        private void FrameChanged()
        {
            if (charSprite.Frame == 0 || charSprite.Frame == 5)
            {
                footstepPlayer.Play();
            }
        }

        private void KnockOnDoor()
        {
            if (!knockPlayer.Playing && canKnock)
            {
                canKnock = false;

                ((AudioStreamRandomPitch)knockPlayer.Stream).AudioStream = knockSamples[GlobalNodes.RNG.RandiRange(0, knockSamples.Count - 1)];
                knockPlayer.Play();

                doorKnockArea.GetOwner<HouseBlock>().KnockOnDoor(doorKnockArea);

                GetTree().CreateTimer(3f).Connect("timeout", this, nameof(ResetCanKnock));
            }
        }

        private void ResetCanKnock()
        {
            canKnock = true;
        }

        private void SweetBodyEntered(Node body)
        {
            if (body is DroppedSweet droppedSweet)
            {
                world.CreateSweetScreenElement(droppedSweet.Sweet, new Vector2(640 / 2, world.Player.Position.y - 16f));
                droppedSweet.QueueFree();
            }
        }
    }
}