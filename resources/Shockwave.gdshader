shader_type canvas_item;

uniform float force;
uniform float inner_radius = 0.2;
uniform float thickness = 0.1;

void fragment()
{
	vec2 centered_uv = UV - vec2(0.5, 0.5);

	float mask = (1.0 - smoothstep(inner_radius, inner_radius + thickness, length(centered_uv))) * 
		-smoothstep(inner_radius, inner_radius + thickness, length(centered_uv));

	vec2 displacement = normalize(centered_uv) * force * mask;
	displacement.x *= -1.0;

	COLOR = texture(SCREEN_TEXTURE, SCREEN_UV + displacement);
}