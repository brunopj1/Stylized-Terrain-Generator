#version 440 core

uniform mat4 uViewMatrix;
uniform float uEdgeDistance;

in Data {
    vec3 viewPos;
    flat vec3 color;
    vec3 edge;
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
    brightness = mix(max(brightness, 0), brightness * 0.5 + 0.5, 0.7);

    FragColor = vec4(brightness * DataIn.color, 1);

    if (DataIn.edge.x < uEdgeDistance || DataIn.edge.y < uEdgeDistance || DataIn.edge.z < uEdgeDistance) {
        FragColor = vec4(0.0, 0.0, 0.0, 1.0);
    }
}