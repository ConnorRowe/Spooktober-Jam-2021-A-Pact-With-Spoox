using Godot;

namespace Spooktober
{
	public class PotionDisplay : Sprite
	{
		public void SetPotionColour(Color colour)
		{
			((ShaderMaterial)Material).SetShaderParam("potion_colour", colour);
		}
	}
}