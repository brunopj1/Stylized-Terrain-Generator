#version 440 core

uniform uint uChunkRadius;
uniform ivec2 uChunkOffsetLocal;
uniform float uChunkTesselation;

in Data {
	vec2 uvLocal;
	vec2 uvWorld;
} DataIn[];

out Data {
	vec2 uvLocal;
	vec2 uvWorld;
} DataOut[];


layout(vertices = 3) out;

void main() {
    if (gl_InvocationID == 0)
    {
        gl_TessLevelInner[0] = uChunkTesselation;

        gl_TessLevelOuter[0] = uChunkTesselation;
        gl_TessLevelOuter[1] = uChunkTesselation;
        gl_TessLevelOuter[2] = uChunkTesselation;
    }

    DataOut[gl_InvocationID].uvLocal = DataIn[gl_InvocationID].uvLocal;
    DataOut[gl_InvocationID].uvWorld = DataIn[gl_InvocationID].uvWorld;
}