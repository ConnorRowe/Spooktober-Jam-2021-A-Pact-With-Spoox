using Godot;
using System.Collections.Generic;

namespace Spooktober
{
    public class PotionMaking : Node2D
    {
        private static Color darkGray = new Color("424242");
        private static Color sweetOrange = new Color("e64539");
        private static Color sourGreen = new Color("63ab3f");
        private static Color magicPurple = new Color("cc2f7b");
        private static Texture tickTex = GD.Load<Texture>("res://textures/tick.png");

        private Dictionary<SweetType, Button> sweetButtons = new Dictionary<SweetType, Button>();
        private Dictionary<SweetType, Label> sweetLabels = new Dictionary<SweetType, Label>();
        private Dictionary<SweetType, int> sweetCounts = new Dictionary<SweetType, int>()
        {
            { SweetType.Classic, 0 },
            { SweetType.Heart, 0 },
            { SweetType.Sour, 0 },
            { SweetType.Choc, 0 },
            { SweetType.Fizzer, 0 },
            { SweetType.Bonbons, 0 },
            { SweetType.KolaCubes, 0 },
        };

        private PanelContainer tooltip;
        private HBoxContainer sweetScore;
        private HBoxContainer sourScore;
        private HBoxContainer magicScore;
        private Tween tween;
        private AudioStreamPlayer pipPlayer;
        private List<Sweet> ingredients = new List<Sweet>();
        private PanelContainer[] ingredientBoxes = null;
        private TextureRect[] ingredientBoxTextures = null;
        private Sprite[] addToBrewSprites = null;
        private Button brewButton;
        private PotionDisplay[] potionDisplays;
        private Vector2[] potionEndPositions;
        private List<(int sweet, int sour, int magic)> potions = new List<(int sweet, int sour, int magic)>();
        private TextureRect sweetPotionCheck;
        private TextureRect sourPotionCheck;
        private TextureRect balancedPotionCheck;
        private int objectiveCount = 0;
        private Label sweetTotalLabel;
        private Label sourTotalLabel;
        private Label magicTotalLabel;
        private Button nextPhaseButton;
        private ColorRect transitionRect;

        private const float circleThird = Mathf.Tau / 3f;

        private PackedScene nextScene = null;

        public override void _Ready()
        {
            sweetButtons.Add(SweetType.Classic, GetNode<Button>("SweetButtons/Classic"));
            sweetButtons.Add(SweetType.Heart, GetNode<Button>("SweetButtons/Heart"));
            sweetButtons.Add(SweetType.Sour, GetNode<Button>("SweetButtons/Sour"));
            sweetButtons.Add(SweetType.Choc, GetNode<Button>("SweetButtons/Choc"));
            sweetButtons.Add(SweetType.Fizzer, GetNode<Button>("SweetButtons/Fizzer"));
            sweetButtons.Add(SweetType.Bonbons, GetNode<Button>("SweetButtons/Bonbons"));
            sweetButtons.Add(SweetType.KolaCubes, GetNode<Button>("SweetButtons/KolaCubes"));

            sweetLabels.Add(SweetType.Classic, GetNode<Label>("SweetButtons/Classic/CountLabel"));
            sweetLabels.Add(SweetType.Heart, GetNode<Label>("SweetButtons/Heart/CountLabel"));
            sweetLabels.Add(SweetType.Sour, GetNode<Label>("SweetButtons/Sour/CountLabel"));
            sweetLabels.Add(SweetType.Choc, GetNode<Label>("SweetButtons/Choc/CountLabel"));
            sweetLabels.Add(SweetType.Fizzer, GetNode<Label>("SweetButtons/Fizzer/CountLabel"));
            sweetLabels.Add(SweetType.Bonbons, GetNode<Label>("SweetButtons/Bonbons/CountLabel"));
            sweetLabels.Add(SweetType.KolaCubes, GetNode<Label>("SweetButtons/KolaCubes/CountLabel"));

            tooltip = GetNode<PanelContainer>("Tooltip"); tooltip.RectScale = new Vector2(0, 1);
            sweetScore = GetNode<HBoxContainer>("Tooltip/MarginContainer/VBoxContainer/SweetHBox");
            sourScore = GetNode<HBoxContainer>("Tooltip/MarginContainer/VBoxContainer/SourHBox");
            magicScore = GetNode<HBoxContainer>("Tooltip/MarginContainer/VBoxContainer/MagicHBox");
            tween = GetNode<Tween>("Tween");
            pipPlayer = GetNode<AudioStreamPlayer>("PipPlayer");
            ingredientBoxes = new PanelContainer[3] {
                 GetNode<PanelContainer>("HBoxContainer/IngredientContainer1"),
                 GetNode<PanelContainer>("HBoxContainer/IngredientContainer2"),
                 GetNode<PanelContainer>("HBoxContainer/IngredientContainer3")
            };
            ingredientBoxTextures = new TextureRect[3] {
                 ingredientBoxes[0].GetNode<TextureRect>("Center/TextureRect"),
                 ingredientBoxes[1].GetNode<TextureRect>("Center/TextureRect"),
                 ingredientBoxes[2].GetNode<TextureRect>("Center/TextureRect")
            };
            sweetCounts = GlobalNodes.SweetCounts;
            addToBrewSprites = new Sprite[3] {
                GetNode<Sprite>("AddToBrewSprite1"),
                GetNode<Sprite>("AddToBrewSprite2"),
                GetNode<Sprite>("AddToBrewSprite3")
            };
            potionDisplays = new PotionDisplay[3] {
                GetNode<PotionDisplay>("PotionDisplay1"),
                GetNode<PotionDisplay>("PotionDisplay2"),
                GetNode<PotionDisplay>("PotionDisplay3")
            };
            potionEndPositions = new Vector2[3] {
                potionDisplays[0].Position,
                potionDisplays[1].Position,
                potionDisplays[2].Position
            };
            potionDisplays[0].Visible = false; potionDisplays[1].Visible = false; potionDisplays[2].Visible = false;
            sweetPotionCheck = GetNode<TextureRect>("Instructions/Sweet/TextureRect");
            sourPotionCheck = GetNode<TextureRect>("Instructions/Sour/TextureRect");
            balancedPotionCheck = GetNode<TextureRect>("Instructions/Balanced/TextureRect");
            sweetTotalLabel = GetNode<Label>("SweetTotalLabel");
            sourTotalLabel = GetNode<Label>("SourTotalLabel");
            magicTotalLabel = GetNode<Label>("MagicTotalLabel");
            transitionRect = GetNode<ColorRect>("TransitionRect");

            foreach (SweetType sweetType in sweetButtons.Keys)
            {
                // TEST TEST TEST
                // sweetCounts[sweetType] += 3;

                UpdateSweetLabel(sweetType);

                sweetButtons[sweetType].Connect("mouse_entered", this, nameof(SweetMouseEntered), new Godot.Collections.Array(sweetType));
                sweetButtons[sweetType].Connect("mouse_exited", this, nameof(SweetMouseExited), new Godot.Collections.Array(sweetType));
                sweetButtons[sweetType].Connect("pressed", this, nameof(AddIngredient), new Godot.Collections.Array(sweetType));
            }

            brewButton = GetNode<Button>("BrewButton");
            brewButton.Connect("mouse_entered", this, nameof(ButtonOverlapSound));
            brewButton.Connect("pressed", this, nameof(BrewButtonPressed));

            GetNode<Button>("UndoButton").Connect("mouse_entered", this, nameof(ButtonOverlapSound));
            GetNode<Button>("UndoButton").Connect("pressed", this, nameof(UndoIngredient));

            nextPhaseButton = GetNode<Button>("NextPhaseButton");
            nextPhaseButton.Connect("mouse_entered", this, nameof(ButtonOverlapSound));
            nextPhaseButton.Connect("pressed", this, nameof(NextPhase));

            UpdateTotalLabels();
            CheckIfEnoughSweetsOrDone();

            transitionRect.Visible = true;
            ((ShaderMaterial)transitionRect.Material).SetShaderParam("percent", 1f);
            tween.InterpolateProperty(transitionRect.Material, "shader_param/percent", 1f, 0f, 1f, Tween.TransitionType.Cubic, Tween.EaseType.Out);
            tween.Start();
        }

        private void ButtonOverlapSound()
        {
            pipPlayer.Play();
        }

        private void SweetMouseEntered(SweetType sweetType)
        {
            ShowTooltip();
            tooltip.RectPosition = new Vector2(-41 + sweetButtons[sweetType].RectGlobalPosition.x, 200);

            var sweet = Sweet.AllSweets[sweetType.ToInt()];

            SetSweetStatScore(sweetScore, sweet.Sweetness);
            SetSweetStatScore(sourScore, sweet.Sourness);
            SetSweetStatScore(magicScore, sweet.Magic);

            ButtonOverlapSound();
        }

        private void SetSweetStatScore(HBoxContainer hBoxContainer, int val)
        {
            var one = hBoxContainer.GetNode<TextureRect>("blip1");
            var two = hBoxContainer.GetNode<TextureRect>("blip2");
            var three = hBoxContainer.GetNode<TextureRect>("blip3");

            one.Modulate = darkGray;
            two.Modulate = darkGray;
            three.Modulate = darkGray;

            if (val > 0)
                one.Modulate = Colors.White;
            if (val > 1)
                two.Modulate = Colors.White;
            if (val > 2)
                three.Modulate = Colors.White;
        }

        private void SweetMouseExited(SweetType sweetType)
        {
            HideTootip();
        }

        private void UpdateSweetLabel(SweetType sweetType)
        {
            int count = sweetCounts[sweetType];
            sweetLabels[sweetType].Text = $"{count}";

            sweetButtons[sweetType].Disabled = count == 0;
        }

        private void ShowTooltip()
        {
            tween.Stop(tooltip, "rect_scale");

            tween.InterpolateProperty(tooltip, "rect_scale", new Vector2(0, 1), Vector2.One, .33f, Tween.TransitionType.Back);

            tween.Start();
        }

        private void HideTootip()
        {
            tween.InterpolateProperty(tooltip, "rect_scale", Vector2.One, new Vector2(0, 1), .15f, Tween.TransitionType.Cubic);

            tween.Start();
        }

        private void AddIngredient(SweetType sweetType)
        {
            if (ingredients.Count >= 3)
                return;

            sweetCounts[sweetType]--;
            UpdateSweetLabel(sweetType);

            ingredients.Add(Sweet.AllSweets[sweetType.ToInt()]);

            SetLastIngredientTexture(sweetType.GetTexture());

            brewButton.Disabled = ingredients.Count < 3;

            UpdateTotalLabels();
        }

        private void UndoIngredient()
        {
            if (ingredients.Count > 0)
            {
                SetLastIngredientTexture(null);

                Sweet sweet = ingredients[ingredients.Count - 1];
                ingredients.RemoveAt(ingredients.Count - 1);

                sweetCounts[sweet.SweetType]++;
                UpdateSweetLabel(sweet.SweetType);

                brewButton.Disabled = ingredients.Count < 3;

                UpdateTotalLabels();
            }
        }

        private void SetLastIngredientTexture(Texture texture)
        {
            ingredientBoxTextures[ingredients.Count - 1].Texture = texture;
        }

        private Vector2 GetSwirlSpritePos(float percent, int spriteNum)
        {
            float angle = (percent * Mathf.Tau) + (spriteNum * circleThird);
            Vector2 dir = new Vector2(Mathf.Cos(-angle), Mathf.Sin(-angle)).Normalized();
            float dist = 112f * percent;

            return new Vector2(640 / 2, 360 / 2) + (dir * dist);
        }

        private void SendIngredientsToBrew()
        {
            Vector2[] startPositions = new Vector2[3] { GetSwirlSpritePos(1f, 0), GetSwirlSpritePos(1f, 1), GetSwirlSpritePos(1f, 2) };

            brewButton.Disabled = true;

            for (int i = 0; i < 3; i++)
            {
                ingredientBoxTextures[i].Texture = null;
                addToBrewSprites[i].ZIndex = 0;

                addToBrewSprites[i].Texture = ingredients[i].SweetType.GetTexture();
                addToBrewSprites[i].Visible = true;
                tween.InterpolateProperty(addToBrewSprites[i], "position", ingredientBoxTextures[i].RectGlobalPosition + new Vector2(26, 16), startPositions[i], 1.5f, Tween.TransitionType.Quad, Tween.EaseType.Out);
                tween.InterpolateProperty(addToBrewSprites[i], "z_index", 0, -1, .1f, delay: 1.5f + 2.75f);
            }

            tween.InterpolateMethod(this, nameof(SwirlBrewSprites), 1f, 0f, 3f, Tween.TransitionType.Quad, Tween.EaseType.In, delay: 1.5f);

            GetTree().CreateTimer(3f + 1.5f).Connect("timeout", this, nameof(HideBrewSprites));

            tween.Start();
        }

        private void SwirlBrewSprites(float percent)
        {
            for (int i = 0; i < 3; i++)
            {
                addToBrewSprites[i].Position = GetSwirlSpritePos(percent, i);
            }
        }

        private void HideBrewSprites()
        {
            foreach (var s in addToBrewSprites)
            {
                s.Visible = false;
            }

            GlobalNodes.MakeSmokePuff(this, new Vector2(640 / 2, 360 / 2));

            MakePotion();
        }

        private void MakePotion()
        {
            int sweetTotal = 0;
            int sourTotal = 0;
            int magicTotal = 0;

            foreach (var ingredient in ingredients)
            {
                sweetTotal += ingredient.Sweetness;
                sourTotal += ingredient.Sourness;
                magicTotal += ingredient.Magic;
            }

            potions.Add((sweetTotal, sourTotal, magicTotal));

            Color potionColour = Colors.Black;
            potionColour += (sweetOrange * sweetTotal);
            potionColour += (sourGreen * sourTotal);
            potionColour += (magicPurple * magicTotal);
            potionColour /= (sweetTotal + sourTotal + magicTotal);

            PotionDisplay potionDisplay = potionDisplays[potions.Count - 1];
            potionDisplay.SetPotionColour(potionColour);
            potionDisplay.Visible = true;

            tween.InterpolateProperty(potionDisplay, "position", new Vector2(640 / 2, 360 / 2), potionEndPositions[potions.Count - 1], 2f, Tween.TransitionType.Cubic);
            tween.Start();

            if (sweetTotal >= 7 && sweetPotionCheck.Texture != tickTex)
            {
                sweetPotionCheck.Texture = tickTex;
                objectiveCount++;
            }

            if (sourTotal >= 7 && sourPotionCheck.Texture != tickTex)
            {
                sourPotionCheck.Texture = tickTex;
                objectiveCount++;
            }

            float avg = ((float)(sweetTotal + sourTotal + magicTotal)) / 3f;

            if (Mathf.IsEqualApprox(sweetTotal, avg, 1f) && Mathf.IsEqualApprox(sourTotal, avg, 1f) && Mathf.IsEqualApprox(magicTotal, avg, 1f) && balancedPotionCheck.Texture != tickTex)
            {
                balancedPotionCheck.Texture = tickTex;
                objectiveCount++;
            }

            GD.Print($"New potion: sweet={sweetTotal}, sour={sourTotal}, magic={magicTotal}");

            ingredients.Clear();

            UpdateTotalLabels();

            if (potions.Count == 3)
            {
                foreach (var sweetType in sweetButtons.Keys)
                {
                    sweetButtons[sweetType].Disabled = true;
                }

                CheckIfEnoughSweetsOrDone();
            }
        }

        private void BrewButtonPressed()
        {
            if (ingredients.Count > 0)
                SendIngredientsToBrew();
        }

        private void UpdateTotalLabels()
        {
            int sweetTotal = 0;
            int sourTotal = 0;
            int magicTotal = 0;

            foreach (var ingredient in ingredients)
            {
                sweetTotal += ingredient.Sweetness;
                sourTotal += ingredient.Sourness;
                magicTotal += ingredient.Magic;
            }

            sweetTotalLabel.Text = "";
            sourTotalLabel.Text = "";
            magicTotalLabel.Text = "";

            if (sweetTotal > 0)
                sweetTotalLabel.Text = $"{sweetTotal}";
            if (sourTotal > 0)
                sourTotalLabel.Text = $"{sourTotal}";
            if (magicTotal > 0)
                magicTotalLabel.Text = $"{magicTotal}";
        }

        private void CheckIfEnoughSweetsOrDone()
        {
            int total = 0;
            if (potions.Count < 3)
            {
                foreach (var sweetType in sweetCounts.Keys)
                {
                    total += sweetCounts[sweetType];
                }
            }

            if (total < 3 || potions.Count == 3)
            {
                nextPhaseButton.Visible = true;
                tween.InterpolateProperty(nextPhaseButton, "rect_scale", Vector2.Right, Vector2.One, .5f, Tween.TransitionType.Back);

                tween.InterpolateProperty(GetNode("MusicPlayer"), "volume_db", 0f, -80f, 1f);
                tween.Start();
            }
        }

        private void NextPhase()
        {
            if (potions.Count <= 0)
            {
                // No Potions!? you fool
                nextScene = GD.Load<PackedScene>("res://scenes/EndingNoPotions.tscn");
            }
            else if (objectiveCount == 0)
            {
                // None of the potions I asked for. Pitiful.
                nextScene = GD.Load<PackedScene>("res://scenes/EndingNoObjectives.tscn");
            }

            if (objectiveCount == 1)
            {
                // One potions, ehh I suppose I won't curse you for eternity...
                nextScene = GD.Load<PackedScene>("res://scenes/EndingOneObjective.tscn");
            }

            if (objectiveCount == 2)
            {
                // Two potions, decent. You may have the power of foresight, every third wednesday of every fourth month of the year.
                nextScene = GD.Load<PackedScene>("res://scenes/EndingTwoObjectives.tscn");
            }

            if (objectiveCount == 3)
            {
                // All the potions I asked for?! You deserve a very strong power. I bless you with the power of Darkvision. So you will never be scared of the dark.
                nextScene = GD.Load<PackedScene>("res://scenes/EndingThreeObjectives.tscn");
            }

            tween.InterpolateProperty(transitionRect.Material, "shader_param/percent", 0f, .5f, 2f, Tween.TransitionType.Cubic, Tween.EaseType.In);
            tween.Start();
            GetTree().CreateTimer(2f).Connect("timeout", this, nameof(GotoNextScene));
        }

        private void GotoNextScene()
        {
            GetTree().ChangeSceneTo(nextScene);
            QueueFree();
        }
    }
}