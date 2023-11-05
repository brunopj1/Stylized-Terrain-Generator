#version 440 core

uniform mat4 uPVMMatrix;
uniform ivec2 uChunkOffset;

layout (location = 0) in vec2 aPosition;

out Data {
	vec2 uv;
} DataOut;

void main()
{
    DataOut.uv = aPosition + uChunkOffset;
    gl_Position = uPVMMatrix * vec4(aPosition.x, 0, aPosition.y, 1.0);
}