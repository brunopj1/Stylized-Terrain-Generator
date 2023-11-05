#version 440 core

layout(triangles, equal_spacing, ccw) in;

uniform mat4 uPVMMatrix;
uniform float uChunkLength;
uniform float uChunkHeight;
uniform ivec2 uChunkOffset;

in Data {
	vec2 uvLocal;
	vec2 uvWorld;
} DataIn[];

out Data {
	vec2 uv;
} DataOut;

void main()
{
    vec2 uvLocal = 
		DataIn[0].uvLocal * gl_TessCoord.x +
		DataIn[1].uvLocal * gl_TessCoord.y +
		DataIn[2].uvLocal * gl_TessCoord.z;

    vec2 uvWorld = 
	    DataIn[0].uvWorld * gl_TessCoord.x +
	    DataIn[1].uvWorld * gl_TessCoord.y +
	    DataIn[2].uvWorld * gl_TessCoord.z;

    DataOut.uv = uvWorld;

    float height = (sin(uvWorld.x + uvWorld.y) + 1) * uChunkHeight * 0.3;
    gl_Position = uPVMMatrix * vec4(uvLocal.x, height, uvLocal.y, 1.0);
}