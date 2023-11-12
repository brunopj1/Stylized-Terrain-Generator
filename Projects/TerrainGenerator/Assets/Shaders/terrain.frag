#version 440 core

uniform mat4 uViewMatrix;

in Data {
    vec3 viewPos;
    flat vec3 color;
} DataIn;

out vec4 FragColor;

void main()
{
    // TODO calculate the normal in the vertex shader and send it as a flat vector
    vec3 xTangent = dFdx(DataIn.viewPos);
    vec3 yTangent = dFdy(DataIn.viewPos);
    vec3 faceNormal = normalize(cross(xTangent, yTangent));

    vec3 lightDirWorld = normalize(vec3(1.0, 1.0, 1.0));
    vec3 lightDir = normalize(uViewMatrix * vec4(lightDirWorld, 0.0)).xyz;

    float diffuse = clamp(dot(faceNormal, lightDir), 0.2, 0.8) + 0.2;

    FragColor = vec4(diffuse * DataIn.color, 1);
}