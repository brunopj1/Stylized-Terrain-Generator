//? #version 460 core

#ifndef __VORONOI_NOISE
#define __VORONOI_NOISE

#include "internal/constants.glsl"
#include "internal/helpers.glsl"

// Constants

#define VORONOI_JITTER 0.43701595

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

float voronoiNoise(int seed, vec2 p, int distanceFunction, int returnType, out ivec2 closestCenter)
{
    ivec2 pr = ivec2(round(p));

    float distance0 = 3.402823466e+38;
    float distance1 = 3.402823466e+38;
    int closestHash = 0;

    ivec2 primed = (pr - 1) * PRIME2;
    int yPrimedBase = primed.y;

    for (int xi = pr.x - 1; xi <= pr.x + 1; xi ++)
    {
        primed.y = yPrimedBase;

        for (int yi = pr.y - 1; yi <= pr.y + 1; yi++)
        {
            int hash = hash(seed, primed);
            int idx = hash & (255 << 1);

            vec2 v = vec2(xi, yi) - p + vec2(randVecs2D[idx], randVecs2D[idx | 1]) * VORONOI_JITTER;

            float newDistance;
            switch (distanceFunction)
            {
                case VORONOI_DISTANCE_EUCLIDEAN:
                case VORONOI_DISTANCE_EUCLIDEAN_SQR:
                    newDistance = v.x * v.x + v.y * v.y;
                    break;
                case VORONOI_DISTANCE_MANHATTAN:
                    newDistance = abs(v.x) + abs(v.y);
                    break;
                case VORONOI_DISTANCE_HYBRID:
                    newDistance = (abs(v.x) + abs(v.y)) + (v.x * v.x + v.y * v.y);
                    break;
                default:
                    newDistance = 0;
            }

            distance1 = max(min(distance1, newDistance), distance0);
            if (newDistance < distance0)
            {
                distance0 = newDistance;
                closestHash = hash;
                closestCenter = primed;
            }
            primed.y += PRIME_Y;
        }
        primed.x += PRIME_X;
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


float voronoiNoise(int seed, vec3 p, int distanceFunction, int returnType, out ivec3 closestCenter)
{
    ivec3 pr = ivec3(round(p));
    
    float distance0 = 3.402823466e+38;
    float distance1 = 3.402823466e+38;
    int closestHash = 0;

    ivec3 primed = (pr - 1) * PRIME3;
    int yPrimedBase = primed.y;
    int zPrimedBase = primed.z;

    for (int xi = pr.x - 1; xi <= pr.x + 1; xi++)
    {
        primed.y = yPrimedBase;

        for (int yi = pr.y - 1; yi <= pr.y + 1; yi++)
        {
            primed.z = zPrimedBase;

            for (int zi = pr.z - 1; zi <= pr.z + 1; zi++)
            {
                int hash = hash(seed, primed);
                int idx = hash & (255 << 2);
                
                vec3 v = vec3(xi, yi, zi) - p + vec3(randVecs3D[idx], randVecs3D[idx | 1], randVecs3D[idx | 2]) * VORONOI_JITTER;

                float newDistance;
                switch (distanceFunction)
                {
                    case VORONOI_DISTANCE_EUCLIDEAN:
                    case VORONOI_DISTANCE_EUCLIDEAN_SQR:
                        newDistance = v.x * v.x + v.y * v.y + v.z * v.z;
                        break;
                    case VORONOI_DISTANCE_MANHATTAN:
                        newDistance = abs(v.x) + abs(v.y) + abs(v.z);
                        break;
                    case VORONOI_DISTANCE_HYBRID:
                        newDistance = (abs(v.x) + abs(v.y) + abs(v.z)) + (v.x * v.x + v.y * v.y * v.z * v.z);
                        break;
                    default:
                    newDistance = 0;
                }

                distance1 = max(min(distance1, newDistance), distance0);
                if (newDistance < distance0)
                {
                    distance0 = newDistance;
                    closestHash = hash;
                    closestCenter = primed;
                }
                primed.z += PRIME_Z;
            }
            primed.y += PRIME_Y;
        }
        primed.x += PRIME_X;
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