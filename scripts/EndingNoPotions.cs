using System.Collections.Generic;

namespace Spooktober
{
    public class EndingNoPotions : SpeechScene
    {
        public override void _Ready()
        {
            base._Ready();

            List<(bool isPlayer, string speech)> convo = new List<(bool isPlayer, string speech)> {
                (false, "This was quite disappointing. Not even a single potion?!"),
                (false, "I shall curse you for this!"),
                (false, "You will never be able to taste sweets again!"),
                (true, "*You sob*"),
            };

            SetConversation(convo);

        }
    }
}