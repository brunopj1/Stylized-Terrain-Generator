//? #version 460 core

#ifndef __PERLIN_NOISE
#define __PERLIN_NOISE

#include "internal/constants.glsl"
#include "internal/helpers.glsl"

float perlinNoise(int seed, vec2 uv)
{
    int x0 = int(floor(uv.x));
    int y0 = int(floor(uv.y));

    float xd0 = float(uv.x - x0);
    float yd0 = float(uv.y - y0);
    float xd1 = xd0 - 1;
    float yd1 = yd0 - 1;

    float xs = INTERP_QUINTIC(xd0);
    float ys = INTERP_QUINTIC(yd0);

    x0 *= PRIME_X;
    y0 *= PRIME_Y;
    int x1 = x0 + PRIME_X;
    int y1 = y0 + PRIME_Y;

    float xf0 = mix(gradCoord(seed, x0, y0, xd0, yd0), gradCoord(seed, x1, y0, xd1, yd0), xs);
    float xf1 = mix(gradCoord(seed, x0, y1, xd0, yd1), gradCoord(seed, x1, y1, xd1, yd1), xs);

    return mix(xf0, xf1, ys) * 1.4247691104677813;
}

#endif