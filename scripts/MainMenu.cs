using Godot;

namespace Spooktober
{
    public class MainMenu : Node2D
    {
        private static PackedScene spookScene = GD.Load<PackedScene>("res://scenes/Spook.tscn");

        private Tween tween;
        private ColorRect transitionRect;

        public override void _Ready()
        {
            tween = GetNode<Tween>("Tween");
            transitionRect = GetNode<ColorRect>("TransitionRect");

            GetNode("MainMenuStuff/PlayButton").Connect("pressed", this, nameof(PlayPressed));
            GetNode("MainMenuStuff/Settings").Connect("pressed", this, nameof(Transition), new Godot.Collections.Array(true));
            GetNode("SettingsStuff/BackButton").Connect("pressed", this, nameof(Transition), new Godot.Collections.Array(false));
            ((ShaderMaterial)transitionRect.Material).SetShaderParam("percent", 1f);
            tween.InterpolateProperty(transitionRect.Material, "shader_param/percent", .5f, 0f, .75f, Tween.TransitionType.Cubic);
            tween.Start();
        }

        private void PlayPressed()
        {
            var spook = spookScene.Instance<Spook>();
            AddChild(spook);
            spook.Position = new Vector2(640 / 2, (360 / 2) + 112f);
            spook.Scale = new Vector2(4, 4);

            tween.InterpolateProperty(transitionRect.Material, "shader_param/percent", 0f, .5f, .75f, Tween.TransitionType.Cubic, delay: 3.6f - .75f);
            tween.Start();

            GetTree().CreateTimer(3.6f).Connect("timeout", this, nameof(StartGame));
        }

        private void StartGame()
        {
            GetTree().ChangeSceneTo(GD.Load<PackedScene>("res://scenes/IntroSpeechScene.tscn"));
        }

        private void Transition(bool toSettings)
        {
            tween.InterpolateProperty(transitionRect.Material, "shader_param/percent", 0f, .5f, .75f, Tween.TransitionType.Cubic);
            tween.InterpolateProperty(transitionRect.Material, "shader_param/percent", .5f, 0f, .75f, Tween.TransitionType.Cubic, delay: .75f);
            tween.InterpolateProperty(toSettings ? GetNode("SettingsStuff") : GetNode("MainMenuStuff"), "visible", false, true, 0f, delay: .75f);
            tween.InterpolateProperty(toSettings ? GetNode("MainMenuStuff") : GetNode("SettingsStuff"), "visible", true, false, 0f, delay: .75f);

            tween.Start();
        }
    }
}