//? #version 460 core

#ifndef __NOISE_HELPERS
#define __NOISE_HELPERS

#include "constants.glsl"

#define INTERP_QUINTIC(x) x * x * x * (x * (x * 6 - 15) + 10)

int hash(int seed, ivec2 p)
{
    int hash = seed ^ p.x ^ p.y;

    hash *= 0x27d4eb2d;
    return hash;
}

int hash(int seed, ivec3 p)
{
    int hash = seed ^ p.x ^ p.y ^ p.z;

    hash *= 0x27d4eb2d;
    return hash;
}

float gradCoord(int seed, ivec2 p, vec2 d)
{
    int hash = hash(seed, p);
    hash ^= hash >> 15;
    hash &= 127 << 1;

    float xg = gradients2D[hash];
    float yg = gradients2D[hash | 1];

    return d.x * xg + d.y * yg;
}

float gradCoord(int seed, ivec3 p, vec3 d)
{
    int hash = hash(seed, p);
    hash ^= hash >> 15;
    hash &= 63 << 2;

    float xg = gradients3D[hash];
    float yg = gradients3D[hash | 1];
    float zg = gradients3D[hash | 2];

    return d.x * xg + d.y * yg + d.z * zg;
}

#endif