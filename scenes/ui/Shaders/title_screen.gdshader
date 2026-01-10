shader_type canvas_item;

uniform vec2 motion = vec2(1.0, 0.0);
uniform float speed = 0.1;

void fragment() {
    COLOR = texture(TEXTURE, UV + (motion * TIME * speed));
}