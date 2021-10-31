using System.Collections.Generic;

namespace Spooktober
{
    public class EndingTwoObjectives : SpeechScene
    {
        public override void _Ready()
        {
            base._Ready();

            List<(bool isPlayer, string speech)> convo = new List<(bool isPlayer, string speech)> {
                (false, "Two potions eh? Almost everything I asked for, but not quite."),
                (false, "You were so close, I bet one more try and you would've got it."),
                (false, "A decent job. You may have the power of foresight, every third wednesday of every fourth month of the year."),
                (true, "*You feel confused yet relieved*"),
            };

            SetConversation(convo);

        }
    }
}