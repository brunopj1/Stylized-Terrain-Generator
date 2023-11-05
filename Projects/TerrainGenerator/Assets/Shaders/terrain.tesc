#version 440 core

const float inner = 16.0;
const float outer = 16.0;

in Data {
	vec2 uv;
} DataIn[];

out Data {
	vec2 uv;
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

    DataOut[gl_InvocationID].uv = DataIn[gl_InvocationID].uv;

    gl_out[gl_InvocationID].gl_Position = gl_in[gl_InvocationID].gl_Position;
}