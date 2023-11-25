#version 460 core

#include "noise/perlin.glsl"
#include "noise/voronoi.glsl"

uniform float uTotalTime;
uniform ivec2 uWindowResolution;
uniform vec3 uCameraFront;
uniform vec3 uCameraRight;
uniform vec3 uCameraUp;

uniform vec3 uSkyColor0;
uniform vec3 uSkyColor1;
uniform float uSkyNoiseFactor;
uniform float uSkyNoiseFreq;

uniform float uCloudVoronoiNoiseFreq;
uniform float uCloudPerlinNoiseFreq;
uniform float uCloudThreshold;
uniform float uCloudExponent;
uniform float uCloudTimeFactor;
uniform vec3 uCloudDirection;

out vec4 FragColor;

void main() {
    vec2 uv = (gl_FragCoord.xy - 0.5 * uWindowResolution) / uWindowResolution.y;
    vec3 ray = normalize(uCameraFront + uv.x * uCameraRight + uv.y * uCameraUp);

    float skyNoise = voronoiNoise(85312, ray * uSkyNoiseFreq, VORONOI_DISTANCE_EUCLIDEAN_SQR, VORONOI_RETURN_CELL_VALUE) * 0.5 + 0.5;
    vec3 skyColor = mix(uSkyColor0, uSkyColor1, (ray.y * 0.5 + 0.5 + skyNoise * uSkyNoiseFactor) / (1 + uSkyNoiseFactor));
    
    vec3 voronoiCenter;
    float cloudVoronoiNoise = voronoiNoise(72301, ray * uCloudVoronoiNoiseFreq, VORONOI_DISTANCE_EUCLIDEAN_SQR, VORONOI_RETURN_CELL_VALUE, voronoiCenter);

    vec3 cloudTimeOffset = normalize(uCloudDirection) * uTotalTime * uCloudTimeFactor;
    float cloudPerlinNoise  = perlinNoise(91726, voronoiCenter * uCloudPerlinNoiseFreq + cloudTimeOffset) * 0.5 + 0.5;
    cloudPerlinNoise = pow(cloudPerlinNoise, uCloudExponent);

    FragColor = vec4(skyColor, 1);
    if (cloudPerlinNoise > uCloudThreshold) {
        float color = 0.8 + 0.2 * (cloudPerlinNoise - uCloudThreshold) / (1 - uCloudThreshold);
        FragColor = vec4(vec3(color), 1);
    }
}