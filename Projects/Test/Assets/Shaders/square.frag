#version 330 core

uniform sampler2D texture0;
uniform sampler2D texture1;

in vec2 texCoord;

out vec4 FragColor;

void main()
{
    vec4 color0 = texture(texture0, texCoord);
    vec4 color1 = texture(texture1 , texCoord);

    FragColor = mix(color0, color1, color1.a);
}