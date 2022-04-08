shader_type canvas_item;

uniform vec4 base_color : hint_color = vec4(0.4, 0.6, 0.9, 1.0);
uniform vec4 border_color : hint_color = vec4(1.0);
uniform vec2 sprite_size = vec2(20.0, 20.0);
uniform float border_width = 2.0;
uniform bool draw_top_border = true;
uniform bool draw_bottom_border = true;
uniform bool draw_left_border = true;
uniform bool draw_right_border = true;

void vertex() {
	if (VERTEX.x > 0.0) VERTEX.x = sprite_size.x;
	if (VERTEX.y > 0.0) VERTEX.y = sprite_size.y;
}

void fragment() {
	vec2 pixel_coord = vec2(UV.x * sprite_size.x, UV.y * sprite_size.y);
	if ((draw_top_border && pixel_coord.y < border_width) ||
		(draw_bottom_border && pixel_coord.y > sprite_size.y - border_width) ||
		(draw_left_border && pixel_coord.x < border_width) ||
		(draw_right_border && pixel_coord.x > sprite_size.x - border_width)) COLOR = border_color;
	else {
		COLOR = base_color;
	}
}