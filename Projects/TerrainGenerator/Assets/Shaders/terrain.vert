#version 440 core

uniform mat4 uPVMMatrix;
uniform mat4 uModelMatrix;
uniform mat4 uViewMatrix;
uniform mat4 uNormalMatrix;

uniform float uChunkHeight;
uniform uint uChunkDivisions;

uniform sampler2D uChunkHeightmap;
uniform sampler2D uChunkColormap;

layout (location = 0) in vec2 aPosition;
layout (location = 1) in float aTriangleIdx;

out Data {
    vec3 viewPos;
    flat vec3 color;
} DataOut;

void main() {
    vec2 uvHeight = (aPosition * uChunkDivisions + 0.5) / (uChunkDivisions + 1);
    float height = texture(uChunkHeightmap, uvHeight).x * uChunkHeight;

    vec4 pos = vec4(aPosition.x, height, aPosition.y, 1);
    DataOut.viewPos = vec3(uViewMatrix * uModelMatrix * pos);
    gl_Position = uPVMMatrix * pos;

    vec2 colormapSize = vec2(2, 1) * uChunkDivisions;
    vec2 uvColor = (aPosition * colormapSize + vec2(aTriangleIdx, 0) + 0.5) / colormapSize;
    DataOut.color = texture(uChunkColormap, uvColor).xyz;
}