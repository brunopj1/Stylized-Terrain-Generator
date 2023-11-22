#version 330 core
layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec2 aTexCoord;

uniform mat4 uPVMMatrix;

out vec2 texCoord;

void main()
{
    gl_Position = uPVMMatrix * vec4(aPosition, 1.0);
    texCoord = aTexCoord;
}