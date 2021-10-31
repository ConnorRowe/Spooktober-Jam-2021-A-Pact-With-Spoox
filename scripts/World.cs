using Godot;

namespace Spooktober
{
    public class World : Node2D
    {
        private static AudioStreamOGGVorbis timeoutMusic = GD.Load<AudioStreamOGGVorbis>("res://sound/timeout_music.ogg");

        private const int houseBlockWidth = 640;
        private const float gameTime = 180f;

        public Player Player { get; set; }
        public bool GameEnded { get; set; } = false;
        public YSort YSort { get; set; }
        public SweetCounter SweetCounter { get; set; }

        private int leftHouseBlocks = 1;
        private int rightHouseBlocks = 1;
        private float timeRemaining = gameTime;
        private bool statedMusicFade = false;
        private bool startedTimeout = false;
        private float count = 0;

        private Label debugLabel;
        private CanvasLayer UILayer;
        private Tween tween;
        private ShaderMaterial timeOverlayShader;
        private AudioStreamPlayer musicPlayer;
        private ColorRect transitionRect;
        private Sprite controls;
        private bool pulseControls = true;

        private PackedScene houseBlockScene;

        public override void _Ready()
        {
            base._Ready();

            houseBlockScene = GD.Load<PackedScene>("res://scenes/HouseBlock.tscn");

            YSort = GetNode<YSort>("YSort");
            tween = GetNode<Tween>("Tween");
            debugLabel = GetNode<Label>("CanvasLayer/debugLabel");
            UILayer = GetNode<CanvasLayer>("CanvasLayer");
            SweetCounter = GetNode<SweetCounter>("CanvasLayer/SweetCounter");
            timeOverlayShader = (ShaderMaterial)GetNode<Sprite>("CanvasLayer/Clock/Overlay").Material;
            musicPlayer = GetNode<AudioStreamPlayer>("MusicPlayer");
            transitionRect = GetNode<ColorRect>("CanvasLayer/TransitionRect");
            controls = GetNode<Sprite>("CanvasLayer/Controls");

            Player = GetNode<Player>("YSort/Player");

            transitionRect.Visible = true;
            ((ShaderMaterial)transitionRect.Material).SetShaderParam("percent", 1f);
            tween.InterpolateProperty(transitionRect.Material, "shader_param/percent", .6f, 0f, 1.5f, Tween.TransitionType.Cubic, Tween.EaseType.In);
            tween.InterpolateProperty(transitionRect, "visible", true, false, 0.1f, delay: 1.5f);

            float quarterTime = gameTime / 4f;
            var lighting = GetNode<CanvasModulate>("Lighting");
            tween.InterpolateProperty(lighting, "color", Colors.White, new Color("e7cea2"), quarterTime);
            tween.InterpolateProperty(lighting, "color", new Color("e7cea2"), new Color("c67e7e"), quarterTime, delay: quarterTime);
            tween.InterpolateProperty(lighting, "color", new Color("c67e7e"), new Color("5c5d7e"), quarterTime, delay: quarterTime * 2);
            tween.InterpolateProperty(lighting, "color", new Color("5c5d7e"), new Color("1d1d2f"), quarterTime, delay: quarterTime * 3);

            tween.Start();
        }

        public override void _Process(float delta)
        {
            if (GameEnded)
                return;

            count += delta * .5f;
            if (count >= 1f)
                count--;

            if (pulseControls)
            {
                float scale = 1f + (Mathf.Sin(count * Mathf.Pi) * .2f);
                controls.Scale = new Vector2(scale, scale);
            }

            timeRemaining -= delta;

            timeOverlayShader.SetShaderParam("percent", timeRemaining / gameTime);

            debugLabel.Text = $"timeRemaining={timeRemaining}";

            if (timeRemaining <= 11f && !statedMusicFade && timeRemaining > 0f)
            {
                tween.InterpolateProperty(musicPlayer, "volume_db", 0f, -80f, 1f);
                tween.Start();
                statedMusicFade = true;
            }

            if (timeRemaining <= 10f && !startedTimeout && timeRemaining > 0f)
            {
                tween.Stop(musicPlayer, "volume_db");
                musicPlayer.VolumeDb = 0f;
                musicPlayer.Stream = timeoutMusic;
                musicPlayer.Play();
                startedTimeout = true;
            }

            if (timeRemaining <= 0f)
            {
                TrickOrTreatPhaseEnded();
            }

            float playerX = Player.Position.x;

            if (playerX > (rightHouseBlocks * houseBlockWidth) - (houseBlockWidth / 2))
            {
                GD.Print("Generate house to the right");
                SpawnHouseBlock(rightHouseBlocks * houseBlockWidth);
                rightHouseBlocks++;
            }
            else if (playerX < (-leftHouseBlocks * houseBlockWidth) + (houseBlockWidth / 2))
            {
                GD.Print("Generate house to the left");
                leftHouseBlocks++;
                SpawnHouseBlock(-leftHouseBlocks * houseBlockWidth);
            }
        }

        private void SpawnHouseBlock(float x)
        {
            HouseBlock newHouseBlock = houseBlockScene.Instance<HouseBlock>();

            YSort.AddChild(newHouseBlock);
            newHouseBlock.Position = new Vector2(x, 0);
        }

        public void CreateSweetScreenElement(Sweet sweet, Vector2 screenPos)
        {
            Vector2 endPos = new Vector2(8 + (52 / 2), 8 + (32 / 2));

            Sprite sprite = new Sprite();
            sprite.Texture = sweet.SweetType.GetTexture();
            UILayer.AddChild(sprite);
            sprite.ZIndex = -1;

            tween.InterpolateProperty(sprite, "position", screenPos, endPos, 1f, Tween.TransitionType.Sine, Tween.EaseType.In);

            tween.Start();

            GetTree().CreateTimer(1f).Connect("timeout", this, nameof(ScreenSweetFinished), new Godot.Collections.Array(sprite, sweet.SweetType.ToInt()));
        }

        private void ScreenSweetFinished(Sprite screenSweet, int sweetId)
        {
            screenSweet.QueueFree();

            SweetCounter.AddSweet(SweetTypeExtensions.FromInt(sweetId));
            SweetCounter.SetSweetTexture(screenSweet.Texture);
        }

        private void TrickOrTreatPhaseEnded()
        {
            GD.Print("Trick-or-treat phase ended.");

            GameEnded = true;
            transitionRect.Visible = true;
            tween.InterpolateProperty(transitionRect.Material, "shader_param/percent", 0f, 1f, 3f, Tween.TransitionType.Cubic, Tween.EaseType.Out);
            tween.Start();

            GetTree().CreateTimer(3f).Connect("timeout", this, nameof(GotoNextScene));
        }

        // For testing purposes only. vvv
        private T TestSpawnNodeAtMouse<T>(NodePath scenePath) where T : Node2D
        {
            T instance = GD.Load<PackedScene>(scenePath).Instance<T>();
            instance.Position = GetNode<Camera2D>("YSort/Player/Camera2D").GetGlobalMousePosition();
            YSort.AddChild(instance);

            return instance;
        }

        private void GotoNextScene()
        {
            GlobalNodes.SweetCounts = SweetCounter.SweetCounts;

            GetTree().ChangeSceneTo(GD.Load<PackedScene>("res://scenes/PrePotionsSpeechScene.tscn"));

            QueueFree();
        }

        public void HideControls()
        {
            if (pulseControls)
            {
                pulseControls = false;
                tween.InterpolateProperty(controls, "scale", controls.Scale, Vector2.Zero, 1f, Tween.TransitionType.Cubic);
                tween.Start();
            }
        }
    }
}