#version 440 core

in Data {
	vec2 uv;
} DataIn;

out vec4 FragColor;

void main()
{
    FragColor = vec4(DataIn.uv.x, 0, DataIn.uv.y, 1);
}