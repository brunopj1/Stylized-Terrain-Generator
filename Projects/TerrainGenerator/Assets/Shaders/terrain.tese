#version 440 core

#pragma include "Util/noise.glsl"

layout(triangles, equal_spacing, ccw) in;

uniform mat4 uPVMMatrix;
uniform mat4 uViewMatrix;
uniform mat4 uModelMatrix;

uniform float uChunkLength;
uniform float uChunkHeight;
uniform ivec2 uChunkOffset;
uniform float uTerrainFrequency;

in Data {
	vec2 uvLocal;
	vec2 uvWorld;
} DataIn[];

out Data {
    vec3 viewPos;
} DataOut;

float hash( in ivec2 p ) { // this hash is not production ready, please replace this by something better
    int n = p.x*3 + p.y*113;
	n = (n << 13) ^ n;
    n = n * (n * n * 15731 + 789221) + 1376312589;
    return -1.0+2.0*float( n & 0x0fffffff)/float(0x0fffffff);
}

float value_noise( in vec2 p ) {
    ivec2 i = ivec2(floor( p ));
    vec2 f = fract( p );
    vec2 u = f*f*f*(f*(f*6.0-15.0)+10.0);

    return mix(mix(hash(i + ivec2(0,0)),
                   hash(i + ivec2(1,0)), u.x),
               mix(hash(i + ivec2(0,1)), 
                   hash(i + ivec2(1,1)), u.x), u.y);
}

void main() {
    vec2 uvLocal = 
		DataIn[0].uvLocal * gl_TessCoord.x +
		DataIn[1].uvLocal * gl_TessCoord.y +
		DataIn[2].uvLocal * gl_TessCoord.z;

    vec2 uvWorld = 
	    DataIn[0].uvWorld * gl_TessCoord.x +
	    DataIn[1].uvWorld * gl_TessCoord.y +
	    DataIn[2].uvWorld * gl_TessCoord.z;

    vec2 uv = uvWorld * uChunkLength * uTerrainFrequency;
    
    float noise = (value_noise(uv) + 1.) * 0.5;
    float height = noise * uChunkHeight;

    vec4 pos = vec4(uvLocal.x, height, uvLocal.y, 1.0);
    DataOut.viewPos = (uViewMatrix * uModelMatrix * pos).xyz;

    gl_Position = uPVMMatrix * pos;
}