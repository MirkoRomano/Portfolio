using System;
using System.Collections.Generic;
using UnityEngine;

namespace Portfolio.Shared
{
    public static class PhysicsUtility
    {
        /// <summary>
        /// Generates an array of directions evenly distributed on a spherical surface, 
        /// transformed by the given <see cref="Transform"/>. The directions are calculated 
        /// by dividing the surface of a sphere into <paramref name="numberOfPoints"/> points, 
        /// and then converted to the world space using the provided transform.
        /// </summary>
        /// <param name="subject">The transform to apply to the directions to convert them into world space.</param>
        /// <param name="numberOfPoints">The number of directions (points) to generate around the sphere.</param>
        /// <param name="rayDirection">Direction of the vector to filter</param>
        /// <param name="angle">Angle for filter the vectors that are between rayDirection and angle</param>
        /// <returns>An array of <see cref="Vector3"/> representing the directions in world space.</returns>
        public static List<Vector3> CircleRaycastDirections(int numberOfPoints, Vector3 rayDirection, float angle)
        {
            List<Vector3> directions = new List<Vector3>();
            float angleInRadians = Mathf.Deg2Rad * angle;
            float cosAngle = Mathf.Cos(angleInRadians);

            for (int i = 0; i < numberOfPoints; i++)
            {
                float t = i / ((float)numberOfPoints - 1);
                float inclination = Mathf.Acos(1 - 2 * t);
                float azimuth = 2 * Mathf.PI * MathUtility.GOLDEN_RATIO * i;

                Vector3 direction = new Vector3(
                                    Mathf.Sin(inclination) * Mathf.Cos(azimuth),
                                    Mathf.Sin(inclination) * Mathf.Sin(azimuth),
                                    Mathf.Cos(inclination)
                                    );

                Vector3 transformedDirection = direction;
                float angleBetween = Vector3.Angle(rayDirection, transformedDirection);
                if(angleBetween <= angle)
                {
                    directions.Add(transformedDirection);
                }
            }

            return directions;
        }

        /// <summary>
        /// Generates an array of directions evenly distributed on a spherical surface, 
        /// transformed by the given <see cref="Transform"/>. The directions are calculated 
        /// by dividing the surface of a sphere into <paramref name="numberOfPoints"/> points, 
        /// and then converted to the world space using the provided transform.
        /// </summary>
        /// <param name="subject">The transform to apply to the directions to convert them into world space.</param>
        /// <param name="numberOfPoints">The number of directions (points) to generate around the sphere.</param>
        /// <returns>An array of <see cref="Vector3"/> representing the directions in world space.</returns>
        public static Vector3[] CircleRaycastDirections(int numberOfPoints)
        {
            Vector3[] directions = new Vector3[numberOfPoints];

            for (int i = 0; i < numberOfPoints; i++)
            {
                float t = i / ((float)numberOfPoints - 1);
                float inclination = Mathf.Acos(1 - 2 * t);
                float azimuth = 2 * Mathf.PI * MathUtility.GOLDEN_RATIO * i;

                Vector3 direction = new Vector3(
                                    Mathf.Sin(inclination) * Mathf.Cos(azimuth),
                                    Mathf.Sin(inclination) * Mathf.Sin(azimuth),
                                    Mathf.Cos(inclination)
                                    );

                directions[i] = direction;
            }

            return directions;
        }


    }
}