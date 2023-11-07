#version 440 core

uniform uint uChunkRadius;
uniform ivec2 uChunkOffsetLocal;
uniform float uChunkTesselation;
uniform float uChunkTesselationPosX;
uniform float uChunkTesselationNegX;
uniform float uChunkTesselationPosZ;
uniform float uChunkTesselationNegZ;

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
        // Vertices:
        //
        //  2---1   1
        //  |T1/   /|
		//  | /   / |
		//  |/   /T0|
		//  0   2---0

        // Triangle 0
        if (DataIn[0].uvLocal.x == 1) {
            gl_TessLevelOuter[0] = uChunkTesselation;
            gl_TessLevelOuter[1] = uChunkTesselationPosZ;
            gl_TessLevelOuter[2] = uChunkTesselationPosX;
        }
        // Triangle 1
        else {
            gl_TessLevelOuter[0] = uChunkTesselationNegZ;
            gl_TessLevelOuter[1] = uChunkTesselationNegX;
            gl_TessLevelOuter[2] = uChunkTesselation;
        }

        gl_TessLevelInner[0] = uChunkTesselation;
    }

    DataOut[gl_InvocationID].uvLocal = DataIn[gl_InvocationID].uvLocal;
    DataOut[gl_InvocationID].uvWorld = DataIn[gl_InvocationID].uvWorld;
}