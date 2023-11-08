#version 440 core

uniform mat4 uPVMMatrix;
uniform mat4 uModelMatrix;
uniform mat4 uViewMatrix;
uniform mat4 uNormalMatrix;

uniform float uChunkLength;
uniform float uChunkHeight;
uniform ivec2 uChunkOffset;
uniform uint uChunkDivisions;

uniform float uTerrainFrequency;

layout (location = 0) in vec2 aPosition;
layout (location = 1) in vec2 aTexCoord;
layout (location = 2) in float aTriangleIdx;

out Data {
    vec3 viewPos;
    flat vec3 color;
} DataOut;

// this hash is not production ready, please replace this by something better
float hash(in ivec2 p) { int n = p.x*3 + p.y*113; n = (n << 13) ^ n; n = n * (n * n * 15731 + 789221) + 1376312589; return -1.0+2.0*float( n & 0x0fffffff)/float(0x0fffffff); }

float value_noise(in vec2 p) { ivec2 i = ivec2(floor( p )); vec2 f = fract( p ); vec2 u = f*f*f*(f*(f*6.0-15.0)+10.0); return mix(mix(hash(i + ivec2(0,0)), hash(i + ivec2(1,0)), u.x), mix(hash(i + ivec2(0,1)), hash(i + ivec2(1,1)), u.x), u.y); }

float getHeight(vec2 uv) {
    vec2 uvHeight = (uChunkOffset + uv) * uChunkLength * uTerrainFrequency;
    float heightNorm = value_noise(uvHeight) * 0.5 + 0.5;
    return heightNorm * uChunkHeight;
}

float getColor(vec2 uv) {
    vec2 uvColor = (uChunkOffset + uv + vec2(123.456, 789.123)) * uChunkLength * uTerrainFrequency;
    return value_noise(uvColor) * 0.5 + 0.5;
}

void main() {
    vec3 pos = vec3(aPosition.x, getHeight(aPosition), aPosition.y);
    DataOut.viewPos = vec3(uViewMatrix * uModelMatrix * vec4(pos, 1.0));
    gl_Position = uPVMMatrix * vec4(pos, 1.0);
    
    float colorF = getColor(aTexCoord);
    if (aTriangleIdx == 1.0) colorF -= 0.05;
    DataOut.color = vec3(colorF);
}