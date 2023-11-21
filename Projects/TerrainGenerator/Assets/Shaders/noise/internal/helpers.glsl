//? #version 460 core

#ifndef __NOISE_HELPERS
#define __NOISE_HELPERS

#include "constants.glsl"


#define INTERP_QUINTIC(x) x * x * x * (x * (x * 6 - 15) + 10)

int hash(int seed, int xPrimed, int yPrimed)
{
    int hash = seed ^ xPrimed ^ yPrimed;

    hash *= 0x27d4eb2d;
    return hash;
}

float gradCoord(int seed, int xPrimed, int yPrimed, float xd, float yd)
{
    int hash = hash(seed, xPrimed, yPrimed);
    hash ^= hash >> 15;
    hash &= 127 << 1;

    float xg = gradients2D[hash];
    float yg = gradients2D[hash | 1];

    return xd * xg + yd * yg;
}

#endif