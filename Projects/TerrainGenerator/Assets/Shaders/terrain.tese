#version 440 core

layout(triangles, equal_spacing, ccw) in;

uniform mat4 uPVMMatrix;
uniform float uChunkLength;
uniform float uChunkHeight;
uniform ivec2 uChunkOffset;

in Data {
	vec2 uv;
} DataIn[];

out Data {
	vec2 uv;
} DataOut;

void main()
{
    vec3 pos = 
        gl_TessCoord.x * gl_in[0].gl_Position.xyz +
        gl_TessCoord.y * gl_in[1].gl_Position.xyz +
        gl_TessCoord.z * gl_in[2].gl_Position.xyz;
   
    DataOut.uv = 
        gl_TessCoord.x * DataIn[0].uv +
        gl_TessCoord.y * DataIn[1].uv +
        gl_TessCoord.z * DataIn[2].uv;

    gl_Position = vec4(pos, 1.0);
}