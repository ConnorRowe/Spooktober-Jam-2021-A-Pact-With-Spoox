using System.Collections.Generic;

namespace Spooktober
{
    public class IntroSpeechScene : SpeechScene
    {
        public override void _Ready()
        {
            base._Ready();

            List<(bool isPlayer, string speech)> convo = new List<(bool isPlayer, string speech)> {
                (false, "[wave amp=50 freq=2]Boo! Hahaha[/wave]"),
                (true, "*You scream!*"),
                (false, "Ha! Got you. My name is Spoox and I'm here to trick or treat your halloween!"),
                (true, "What does that even mean?"),
                (false, "Well, stupid, idiot, oafish, mortal child. I demand that you [shake rate=10 level=8][color=#ae0000]sacrifice[/color][/shake] your hard-earned sweets this halloween."),
                (false, "In return, I will bestow up on you a tremendous blessing! [font=res://font/tiny_dynamicfont.tres]Or a curse...[/font]"),
                (true, "Hmm... Sounds alright to me, a ghost blessing could be pretty useful."),
                (false, "[font=res://font/tiny_dynamicfont.tres]And so begins a pact... Ha haha ha[/font] haha HAHA HA!"),
                (true, "You're weird."),
                (false, "[font=res://font/tiny_dynamicfont.tres]I don't get out very often.[/font]"),
                (true, "Guess I'll start sweet hunting now. Cya."),
                (false, "[shake rate=10 level=8]Wait![/shake] I offer you 3 pieces of advice."),
                (false, "1. Houses with more decorations will give more sweets. But that doesn't mean they won't be haunted."),
                (false, "2. Houses with the lights off might still be giving out sweets!"),
                (false, "3. If you drop your sweets, don't forget to pick them back up!"),
                (false, "Don't think I'll make this easy for you child. You have 3 minutes before sundown. Now go!"),
            };

            SetConversation(convo);
        }
    }
}