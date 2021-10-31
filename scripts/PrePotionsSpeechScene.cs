using System.Collections.Generic;

namespace Spooktober
{
    public class PrePotionsSpeechScene : SpeechScene
    {
        public override void _Ready()
        {
            base._Ready();

            List<(bool isPlayer, string speech)> convo = new List<(bool isPlayer, string speech)> {
                (false, "Interesting tactics..."),
                (true, "Uh, thank you?"),
                (false, "We'll see how well you do in the next phase."),
                (false, "You can brew up to 3 potions, each including 3 sweets."),
                (false, "You must brew me something sickeningly sweet, superbly sour, and brilliantly balanced!"),
                (false, "In return, I will bestow up on you a tremendous blessing! [font=res://font/tiny_dynamicfont.tres]Or a curse...[/font]"),
                (true, "You've already said that."),
                (false, "No matter. Go now! Brew me 3 powerful potions!"),
                (true, "This'll be easy."),
            };

            SetConversation(convo);
        }
    }
}