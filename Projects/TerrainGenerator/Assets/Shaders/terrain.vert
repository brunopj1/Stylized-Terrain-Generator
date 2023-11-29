#version 440 core

uniform mat4 uPVMMatrix;
uniform mat4 uModelMatrix;
uniform mat4 uViewMatrix;
uniform mat4 uNormalMatrix;

uniform float uChunkHeight;
uniform uint uChunkDivisions;

uniform usampler2D uChunkTexture;

layout (location = 0) in vec2 aPosition;
layout (location = 1) in float aTriangleIdx;

out Data {
    vec3 viewPos;
    flat vec3 color;
    vec3 edge;
} DataOut;

vec3 hexToRgb(uint hex) {
    return vec3(
        ((hex >> 16) & 0xFF) / 255.0,
        ((hex >> 8) & 0xFF) / 255.0,
        (hex & 0xFF) / 255.0
    );
}

void main() {
    vec2 uv = (aPosition * uChunkDivisions + 0.5) / (uChunkDivisions + 1);
    uvec4 texValue = texture(uChunkTexture, uv);

    float height = (texValue.x / 4294967295.0) * uChunkHeight;

    vec4 pos = vec4(aPosition.x, height, aPosition.y, 1);
    DataOut.viewPos = vec3(uViewMatrix * uModelMatrix * pos);
    gl_Position = uPVMMatrix * pos;

    DataOut.color = aTriangleIdx == 0.0 ? hexToRgb(texValue.y) : hexToRgb(texValue.z);

    DataOut.edge = vec3((gl_VertexID % 3) == 0, (gl_VertexID % 3) == 1, (gl_VertexID % 3) == 2);
}