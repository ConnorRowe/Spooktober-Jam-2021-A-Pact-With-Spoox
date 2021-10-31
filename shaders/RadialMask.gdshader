shader_type canvas_item;

const float PI = 3.14159274;
uniform float percent: hint_range(0, 1) = 0.0;

// Couldn't get builtin atan to work the way I wanted lol
float atan2(vec2 v)
{
	if(v.x == 0.0 && v.y == 0.0)
	{
		return 0.0;
	}
	else
	{
		float offset = 0.0;
		if (v.x < 0.0)
		{
			offset = PI;
		}
		return atan(v.y / v.x) + offset;
	}
}

void fragment()
{
	vec4 base = texture(TEXTURE, UV);
	float mask = (atan2(UV*2.0-1.0) / PI) / 2.0 + 0.5;
	float stepVal = step(mask, (percent + 0.25));
	
	if(stepVal < 1.0)
	{
		discard;
	}
}