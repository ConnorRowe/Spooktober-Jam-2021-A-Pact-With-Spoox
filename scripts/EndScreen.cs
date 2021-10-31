using Godot;

namespace Spooktober
{
    public class EndScreen : Node2D
    {
        private Tween tween;
        private ColorRect transitionRect;

        public override void _Ready()
        {
            tween = GetNode<Tween>("Tween");
            transitionRect = GetNode<ColorRect>("TransitionRect");

            GetNode("RestartButton").Connect("pressed", this, nameof(RestartPressed));

            ((ShaderMaterial)transitionRect.Material).SetShaderParam("percent", 1f);
            tween.InterpolateProperty(transitionRect.Material, "shader_param/percent", 1f, 0f, 1f, Tween.TransitionType.Cubic, Tween.EaseType.Out);
            tween.Start();
        }

        private void RestartPressed()
        {
            tween.InterpolateProperty(transitionRect.Material, "shader_param/percent", 0f, .5f, 2f, Tween.TransitionType.Cubic, Tween.EaseType.In);
            tween.Start();
            GetTree().CreateTimer(2f).Connect("timeout", this, nameof(GotoMainMenu));
        }

        private void GotoMainMenu()
        {
            GetTree().ChangeSceneTo(GD.Load<PackedScene>("res://scenes/MainMenu.tscn"));
            QueueFree();
        }
    }
}