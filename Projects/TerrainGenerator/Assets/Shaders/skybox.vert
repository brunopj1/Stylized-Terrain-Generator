#version 460 core

layout (location = 0) in vec2 aPosition;

void main() {
    gl_Position = vec4(aPosition.xy, 0.99999, 1.0);
}