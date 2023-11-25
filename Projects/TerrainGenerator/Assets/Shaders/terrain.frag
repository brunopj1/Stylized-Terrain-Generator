#version 440 core

uniform mat4 uViewMatrix;

in Data {
    vec3 viewPos;
    flat vec3 color;
} DataIn;

out vec4 FragColor;

void main()
{
    vec3 xTangent = dFdx(DataIn.viewPos);
    vec3 yTangent = dFdy(DataIn.viewPos);
    vec3 faceNormal = normalize(cross(xTangent, yTangent));

    vec3 lightDirWorld = normalize(vec3(1.0, 1.0, 1.0));
    vec3 lightDir = normalize(uViewMatrix * vec4(lightDirWorld, 0.0)).xyz;
    float brightness = dot(lightDir, faceNormal);
    brightness = mix(max(brightness, 0), brightness * 0.5 + 0.5, 0.5);

    FragColor = vec4(brightness * DataIn.color, 1);
}