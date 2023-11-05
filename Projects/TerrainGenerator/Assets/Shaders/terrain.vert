#version 440 core

uniform mat4 uPVMMatrix;
uniform ivec2 uChunkOffset;

layout (location = 0) in vec2 aPosition;

out Data {
    vec2 uvLocal;
	vec2 uvWorld;
} DataOut;

void main()
{
    DataOut.uvLocal = aPosition;
    DataOut.uvWorld = aPosition + uChunkOffset;
}