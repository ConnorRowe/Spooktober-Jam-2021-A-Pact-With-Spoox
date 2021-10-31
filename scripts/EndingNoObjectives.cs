using System.Collections.Generic;

namespace Spooktober
{
    public class EndingNoObjectives : SpeechScene
    {
        public override void _Ready()
        {
            base._Ready();

            List<(bool isPlayer, string speech)> convo = new List<(bool isPlayer, string speech)> {
                (false, "Oh my. I'm not sure how you've performed quite so terribly."),
                (false, "Not even a single potion I asked for?! You utter fool. As punishment this sordid performance, I shall curse you."),
                (false, "Forever, you will be unable to leave the house on halloween. No more trick-or-treating for you!"),
                (true, "*You cry*"),
            };

            SetConversation(convo);

        }
    }
}