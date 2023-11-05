#version 440 core

const float inner = 8.0;
const float outer = 8.0;

in Data {
	vec2 uvLocal;
	vec2 uvWorld;
} DataIn[];

out Data {
	vec2 uvLocal;
	vec2 uvWorld;
} DataOut[];


layout(vertices = 3) out;

void main()
{
    if (gl_InvocationID == 0)
    {
        gl_TessLevelInner[0] = inner;

        gl_TessLevelOuter[0] = outer;
        gl_TessLevelOuter[1] = outer;
        gl_TessLevelOuter[2] = outer;
    }

    DataOut[gl_InvocationID].uvLocal = DataIn[gl_InvocationID].uvLocal;
    DataOut[gl_InvocationID].uvWorld = DataIn[gl_InvocationID].uvWorld;
}