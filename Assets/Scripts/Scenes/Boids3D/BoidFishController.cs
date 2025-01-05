using Portfolio.Shared;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Portfolio.Boids3D
{
    [System.Serializable]
    public sealed class BoidsSettings
    {
        [SerializeField, MinMax(3.5f, 5f)]
        public Vector2 speedRange;

        [SerializeField, Range(0f, 90f)]
        public float filterAngle;

        [SerializeField, Range(0f, 10f)]
        public float separationRadius;

        [SerializeField, Range(0f, 10f)]
        public float colliderRadius;

        [SerializeField, Range(0f, 1f)]
        public float steerSmoothRatio;

        [SerializeField, Range(0f, 1f)]
        public float separationRatio;

        [SerializeField, Range(0f, 1f)]
        public float allignmentRatio;

        [SerializeField, Range(0f, 1f)]
        public float cohesionRatio;

        [SerializeField, Range(0f, 1f)]
        public float colliderRatio;
    }

    public sealed class BoidFishController : MonoBehaviour
    {
        [SerializeField]
        private Transform boidsParent;

        [SerializeField]
        private GameAreaSphere gameArea;

        [SerializeField]
        private int circleRaycastNumPoints;

        [SerializeField]
        private BoidsSettings boidsSettings;

        private BoidActor[] boids;

        private Vector3[] vectorSphericalProjection;

        private readonly List<BoidActor> fishToCheck = new List<BoidActor>();

        private void Awake()
        {
            InitializeBoids();
        }

        private void Start()
        {
            for (int fishIndex = 0; fishIndex < boids.Length; fishIndex++)
            {
                boids[fishIndex].Subject.forward = GetRandomDirection();
            }
        }

        void Update()
        {
            for (int fishIndex = 0; fishIndex < boids.Length; fishIndex++) 
            {
                fishToCheck.Clear();

                var fish = boids[fishIndex];

                for (int otherIndex = 0; otherIndex < boids.Length; otherIndex++) 
                {
                    var other = boids[otherIndex];

                    if (fish == other)
                    {
                        continue;
                    }

                    if (Vector3.Distance(fish.Subject.position, other.Subject.position) < boidsSettings.separationRadius)
                    {
                        fishToCheck.Add(other);
                    }
                }

                fish.Update(fishToCheck);
            }
        }

        /// <summary>
        /// Initialize every fish in the game
        /// </summary>
        private void InitializeBoids()
        {
            vectorSphericalProjection = PhysicsUtility.CircleRaycastDirections(circleRaycastNumPoints);

            boids = new BoidActor[boidsParent.childCount];
            for (int i = 0; i < boidsParent.childCount; i++)
            {
                boids[i] = new BoidActor(boidsParent.GetChild(i), vectorSphericalProjection, boidsSettings);
            }
        }

        /// <summary>
        /// Get a random normalized vector
        /// </summary>
        private Vector3 GetRandomDirection()
        {
            return new Vector3(UnityEngine.Random.Range(-1f, 1f),
                               UnityEngine.Random.Range(-1f, 1f),
                               UnityEngine.Random.Range(-1f, 1f));
        }

    }
}