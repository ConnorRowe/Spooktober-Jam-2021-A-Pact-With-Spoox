using System.Collections.Generic;

namespace Spooktober
{
    public class EndingThreeObjectives : SpeechScene
    {
        public override void _Ready()
        {
            base._Ready();

            List<(bool isPlayer, string speech)> convo = new List<(bool isPlayer, string speech)> {
                (false, "Congratulations mortal, you've done as I asked!"),
                (false, "Three wonderful potions! Did you have trouble with the balanced one?"),
                (true, "A little bit..."),
                (false, "You deserve a very strong power. I bestow upon you the power of Darkvision. So you will never be scared of the dark."),
                (true, "Wow! Thanks Spoox! Will I ever see you again?"),
                (false, "Maybe next Halloween."),
                (false, "*Spoox winx*"),
            };

            SetConversation(convo);

        }
    }
}