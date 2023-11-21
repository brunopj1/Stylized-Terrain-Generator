//? #version 460 core

#ifndef __VORONOI_NOISE
#define __VORONOI_NOISE

#include "internal/constants.glsl"
#include "internal/helpers.glsl"

// Enums

#define VORONOI_DISTANCE_EUCLIDEAN 0
#define VORONOI_DISTANCE_EUCLIDEAN_SQR 1
#define VORONOI_DISTANCE_MANHATTAN 2
#define VORONOI_DISTANCE_HYBRID 3

#define VORONOI_RETURN_CELL_VALUE 0
#define VORONOI_RETURN_DISTANCE 1
#define VORONOI_RETURN_DISTANCE_2 2
#define VORONOI_RETURN_DISTANCE_2_ADD 3
#define VORONOI_RETURN_DISTANCE_2_SUB 4
#define VORONOI_RETURN_DISTANCE_2_MUL 5
#define VORONOI_RETURN_DISTANCE_2_DIV 6

// Functions

float voronoiNoise(int seed, vec2 uv, int distanceFunction, int returnType)
{
    int xr = int(round(uv.x));
    int yr = int(round(uv.y));

    float distance0 = 3.402823466e+38;
    float distance1 = 3.402823466e+38;
    int closestHash = 0;

    int xPrimed = (xr - 1) * PRIME_X;
    int yPrimedBase = (yr - 1) * PRIME_Y;

    switch (distanceFunction)
    {
        default:
        case VORONOI_DISTANCE_EUCLIDEAN:
        case VORONOI_DISTANCE_EUCLIDEAN_SQR:
            for (int xi = xr - 1; xi <= xr + 1; xi++)
            {
                int yPrimed = yPrimedBase;

                for (int yi = yr - 1; yi <= yr + 1; yi++)
                {
                    int hash = hash(seed, xPrimed, yPrimed);
                    int idx = hash & (255 << 1);

                    float vecX = float(xi - uv.x) + randVecs2D[idx] * VORONOI_JITTER;
                    float vecY = float(yi - uv.y) + randVecs2D[idx | 1] * VORONOI_JITTER;

                    float newDistance = vecX * vecX + vecY * vecY;

                    distance1 = max(min(distance1, newDistance), distance0);
                    if (newDistance < distance0)
                    {
                        distance0 = newDistance;
                        closestHash = hash;
                    }
                    yPrimed += PRIME_Y;
                }
                xPrimed += PRIME_X;
            }
            break;
        case VORONOI_DISTANCE_MANHATTAN:
            for (int xi = xr - 1; xi <= xr + 1; xi++)
            {
                int yPrimed = yPrimedBase;

                for (int yi = yr - 1; yi <= yr + 1; yi++)
                {
                    int hash = hash(seed, xPrimed, yPrimed);
                    int idx = hash & (255 << 1);

                    float vecX = float(xi - uv.x) + randVecs2D[idx] * VORONOI_JITTER;
                    float vecY = float(yi - uv.y) + randVecs2D[idx | 1] * VORONOI_JITTER;

                    float newDistance = abs(vecX) + abs(vecY);

                    distance1 = max(min(distance1, newDistance), distance0);
                    if (newDistance < distance0)
                    {
                        distance0 = newDistance;
                        closestHash = hash;
                    }
                    yPrimed += PRIME_Y;
                }
                xPrimed += PRIME_X;
            }
            break;
        case VORONOI_DISTANCE_HYBRID:
            for (int xi = xr - 1; xi <= xr + 1; xi++)
            {
                int yPrimed = yPrimedBase;

                for (int yi = yr - 1; yi <= yr + 1; yi++)
                {
                    int hash = hash(seed, xPrimed, yPrimed);
                    int idx = hash & (255 << 1);

                    float vecX = float(xi - uv.x) + randVecs2D[idx] * VORONOI_JITTER;
                    float vecY = float(yi - uv.y) + randVecs2D[idx | 1] * VORONOI_JITTER;

                    float newDistance = (abs(vecX) + abs(vecY)) + (vecX * vecX + vecY * vecY);

                    distance1 = max(min(distance1, newDistance), distance0);
                    if (newDistance < distance0)
                    {
                        distance0 = newDistance;
                        closestHash = hash;
                    }
                    yPrimed += PRIME_Y;
                }
                xPrimed += PRIME_X;
            }
            break;
    }

    if (distanceFunction == VORONOI_DISTANCE_EUCLIDEAN && returnType >= VORONOI_RETURN_DISTANCE)
    {
        distance0 = sqrt(distance0);

        if (returnType >= VORONOI_RETURN_DISTANCE_2)
        {
            distance1 = sqrt(distance1);
        }
    }

    switch (returnType)
    {
        case VORONOI_RETURN_CELL_VALUE:
            return closestHash * (1 / 2147483648.0);
        case VORONOI_RETURN_DISTANCE:
            return distance0 - 1;
        case VORONOI_RETURN_DISTANCE_2:
            return distance1 - 1;
        case VORONOI_RETURN_DISTANCE_2_ADD:
            return (distance1 + distance0) * 0.5 - 1;
        case VORONOI_RETURN_DISTANCE_2_SUB:
            return distance1 - distance0 - 1;
        case VORONOI_RETURN_DISTANCE_2_MUL:
            return distance1 * distance0 * 0.5 - 1;
        case VORONOI_RETURN_DISTANCE_2_DIV:
            return distance0 / distance1 - 1;
        default:
            return 0;
    }
}

#endif