#version 330 core

uniform float uTotalTime;

in vec3 color;

out vec4 FragColor;

void main()
{
    FragColor = vec4(color * (sin(uTotalTime) * 0.3 + 0.7), 1.0);
}