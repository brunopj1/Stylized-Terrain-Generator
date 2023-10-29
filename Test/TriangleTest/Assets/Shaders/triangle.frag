#version 330 core

uniform float uTime;

in vec3 color;

out vec4 FragColor;

void main()
{
    FragColor = vec4(color * sin(uTime), 1.0);
}