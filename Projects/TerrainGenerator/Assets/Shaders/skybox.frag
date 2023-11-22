#version 460 core

#include "noise/perlin.glsl"
#include "noise/voronoi.glsl"

uniform ivec2 uWindowResolution;
uniform mat4 uNormalMatrix;

uniform vec3 uCameraFront;
uniform vec3 uCameraRight;
uniform vec3 uCameraUp;

out vec4 FragColor;

void main() {
    vec2 uv = (gl_FragCoord.xy - 0.5 * uWindowResolution) / uWindowResolution.y;
    vec3 ray = normalize(uCameraFront + uv.x * uCameraRight + uv.y * uCameraUp);

    //float noise = perlinNoise(85312, ray * 20);
    ivec3 ignore;
    float noise = voronoiNoise(85312, ray * 8, VORONOI_DISTANCE_EUCLIDEAN_SQR, VORONOI_RETURN_CELL_VALUE, ignore) * 0.5 + 0.5;
    float noiseFactor = 0.2;

    vec4 color0 = vec4(0.23, 0.43, 0.71, 1.0);
    vec4 color1 = vec4(0.08, 0.20, 0.48, 1.0);

    FragColor = mix(color0, color1, (ray.y * 0.5 + 0.5 + noise * noiseFactor) / (1 + noiseFactor));
}