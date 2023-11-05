#version 440 core

in Data {
	vec2 uv;
    float noise;
} DataIn;

out vec4 FragColor;

void main() {
    FragColor = vec4(vec3(DataIn.noise), 1);
}