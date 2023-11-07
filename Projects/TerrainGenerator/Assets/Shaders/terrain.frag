#version 440 core

uniform mat4 uViewMatrix;

in Data {
    vec3 viewPos;
} DataIn;

out vec4 FragColor;

void main() {
    // TODO find better way to get the face normal
    vec3 xTangent = dFdx( DataIn.viewPos );
    vec3 yTangent = dFdy( DataIn.viewPos );
    vec3 faceNormal = normalize( cross( xTangent, yTangent ) );

    vec3 lightDirWorld = normalize(vec3(1.0, 1.0, 1.0));
    vec3 lightDir = normalize(uViewMatrix * vec4(lightDirWorld, 0.0)).xyz;

    float diffuse = max(0.1, dot(faceNormal, lightDir));

    FragColor = vec4(vec3(diffuse), 1);
}