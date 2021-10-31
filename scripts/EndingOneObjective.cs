using System.Collections.Generic;

namespace Spooktober
{
    public class EndingOneObjective : SpeechScene
    {
        public override void _Ready()
        {
            base._Ready();

            List<(bool isPlayer, string speech)> convo = new List<(bool isPlayer, string speech)> {
                (false, "Hmm, one potion..."),
                (false, "This is disappointing, but it could be worse"),
                (false, "I suppose I won't curse you but I certainly won't be giving you any blessing!"),
                (true, "*You sigh*"),
            };

            SetConversation(convo);

        }
    }
}