using Godot;
using System.Collections.Generic;

namespace Spooktober
{
    public class SpeechScene : Node2D
    {
        [Export]
        private PackedScene nextScene;

        private RichTextLabel playerSpeech;
        private RichTextLabel otherSpeech;
        private AudioStreamPlayer pipPlayer;
        private AnimationPlayer animPlayer;

        private List<(bool isPlayer, string speech)> conversation = new List<(bool isPlayer, string speech)>();
        private bool isPlayerSpeaking = false;
        private float maxCharacters;
        private float currentCharacters = 0;

        public override void _Ready()
        {
            playerSpeech = GetNode<RichTextLabel>("PanelContainer/PlayerSpeech");
            otherSpeech = GetNode<RichTextLabel>("PanelContainer2/OtherSpeech");
            pipPlayer = GetNode<AudioStreamPlayer>("PipPlayer");
            animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
            var skip = GetNode<Button>("SkipButton");
            skip.Connect("pressed", this, nameof(SkipPressed));

            // List<(bool isPlayer, string speech)> testConvo = new List<(bool isPlayer, string speech)> {
            //     (false, " HI, I AM NOT THE PLAYER"),
            //     (true, "HI GUY I AM THE PLAYER"),
            //     (false, "COOL, IM NOT THE PLAYER THO"),
            //     (true, "YEAH BUT I AM THE PLAYER"),
            //     (true, "IM STILL THE PLAYER :D")
            // };

            // SetConversation(testConvo);

            ((ShaderMaterial)GetNode<ColorRect>("TransitionRect").Material).SetShaderParam("percent", 1f);
            var tween = GetNode<Tween>("Tween");
            tween.InterpolateProperty(GetNode<ColorRect>("TransitionRect").Material, "shader_param/percent", 1f, 0f, 1f, Tween.TransitionType.Cubic);
            tween.Start();
        }

        public override void _Process(float delta)
        {
            var label = GetLabel();

            int startChars = label.VisibleCharacters;

            if (currentCharacters < maxCharacters)
            {
                currentCharacters += delta * 20.0f;
            }

            if (currentCharacters > maxCharacters)
            {
                currentCharacters = maxCharacters;
            }

            label.VisibleCharacters = Mathf.RoundToInt(currentCharacters);

            if (label.VisibleCharacters > startChars)
            {
                pipPlayer.Play();
            }
        }

        public override void _Input(InputEvent evt)
        {
            base._Input(evt);

            if (evt.IsActionReleased("g_next_speech"))
            {
                if (currentCharacters < maxCharacters)
                {
                    currentCharacters = maxCharacters;
                    GetLabel().VisibleCharacters = Mathf.RoundToInt(currentCharacters);
                }
                else
                {
                    NextSpeech();
                }
            }
        }

        private void SkipPressed()
        {
            GD.Print("Skip Pressed!");
            ConvoFinished();
        }

        public void SetConversation(List<(bool isPlayer, string speech)> convo)
        {
            conversation = convo;

            NextSpeech();
        }

        public void NextSpeech()
        {
            if (conversation.Count > 0)
            {
                isPlayerSpeaking = conversation[0].isPlayer;
                var label = GetLabel();
                label.BbcodeText = conversation[0].speech;

                label.VisibleCharacters = 0;
                currentCharacters = 0;
                CallDeferred(nameof(UpdateMaxCharacters));

                GetLabel(true).VisibleCharacters = 0;

                animPlayer.Stop();
                animPlayer.Play(isPlayerSpeaking ? "PlayerTalk" : "OtherTalk");

                conversation.RemoveAt(0);
            }
            else
            {
                ConvoFinished();
            }
        }

        private void UpdateMaxCharacters()
        {
            maxCharacters = GetLabel().GetTotalCharacterCount();
        }

        private RichTextLabel GetLabel(bool inverse = false)
        {
            if (!inverse)
                return isPlayerSpeaking ? playerSpeech : otherSpeech;
            else
                return !isPlayerSpeaking ? playerSpeech : otherSpeech;
        }

        private void ConvoFinished()
        {
            GD.Print("Convo finished.");

            var tween = GetNode<Tween>("Tween");
            tween.InterpolateProperty(GetNode<ColorRect>("TransitionRect").Material, "shader_param/percent", 0f, 1f, 1f, Tween.TransitionType.Cubic);

            tween.InterpolateProperty(GetNode("MusicPlayer"), "volume_db", 0f, -80f, 1f);
            tween.Start();

            GetTree().CreateTimer(1f).Connect("timeout", this, nameof(GotoNextScene));
        }

        private void GotoNextScene()
        {
            if (IsInstanceValid(nextScene))
            {
                GetTree().ChangeSceneTo(nextScene);

                QueueFree();
            }
        }
    }
}