//? #version 460 core

#ifndef __PERLIN_NOISE
#define __PERLIN_NOISE

#include "Internal/constants.glsl"
#include "Internal/helpers.glsl"

float perlinNoise(int seed, vec2 p)
{
    ivec2 p0 = ivec2(floor(p));

    vec2 pd0 = p - p0;

    vec2 pd1 = pd0 - 1;

    vec2 ps = INTERP_QUINTIC(pd0);

    p0 *= PRIME2;

    ivec2 p1 = p0 + PRIME2;


    float xf0 = mix(gradCoord(seed, p0, pd0), gradCoord(seed, ivec2(p1.x, p0.y), vec2(pd1.x, pd0.y)), ps.x);
    float xf1 = mix(gradCoord(seed, ivec2(p0.x, p1.y), vec2(pd0.x, pd1.y)), gradCoord(seed, p1, pd1), ps.x);

    return mix(xf0, xf1, ps.y) * 1.4247691104677813;
}

float perlinNoise(int seed, vec3 p)
{
    ivec3 p0 = ivec3(floor(p));

    vec3 pd0 = p - p0;

    vec3 pd1 = pd0 - 1;

    vec3 ps = INTERP_QUINTIC(pd0);
    
    p0 *= PRIME3;

    ivec3 p1 = p0 + PRIME3;

    float xf00 = mix(gradCoord(seed, p0, pd0), gradCoord(seed, ivec3(p1.x, p0.y, p0.z), vec3(pd1.x, pd0.y, pd0.z)), ps.x);
    float xf10 = mix(gradCoord(seed, ivec3(p0.x, p1.y, p0.z), vec3(pd0.x, pd1.y, pd0.z)), gradCoord(seed, ivec3(p1.x, p1.y, p0.z), vec3(pd1.x, pd1.y, pd0.z)), ps.x);
    float xf01 = mix(gradCoord(seed, ivec3(p0.x, p0.y, p1.z), vec3(pd0.x, pd0.y, pd1.z)), gradCoord(seed, ivec3(p1.x, p0.y, p1.z), vec3(pd1.x, pd0.y, pd1.z)), ps.x);
    float xf11 = mix(gradCoord(seed, ivec3(p0.x, p1.y, p1.z), vec3(pd0.x, pd1.y, pd1.z)), gradCoord(seed, p1, pd1), ps.x);

    float yf0 = mix(xf00, xf10, ps.y);
    float yf1 = mix(xf01, xf11, ps.y);

    return mix(yf0, yf1, ps.z) * 0.964921414852142333984375f;
}

#endif