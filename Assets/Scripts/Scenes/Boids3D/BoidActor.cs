using Portfolio.Shared.Octree;
using System.Collections.Generic;
using UnityEngine;

namespace Portfolio.Boids3D
{
    public class BoidActor: ISpacialData
    {
        public class MeanForce
        {
            /// <summary>
            /// Direction of the force
            /// </summary>
            public Vector3 Value
            {
                get
                {
                    if (count == 0)
                    {
                        return force;
                    }

                    return force / count;
                }
            }

            /// <summary>
            /// Direction of the force
            /// </summary>
            private Vector3 force;

            /// <summary>
            /// Number of forces added
            /// </summary>
            private int count;

            public MeanForce()
            {
                force = Vector3.zero;
                count = 0;
            }

            /// <summary>
            /// Reset force
            /// </summary>
            public void ResetForce()
            {
                force = Vector3.zero;
                count = 0;
            }

            /// <summary>
            /// Add a new force
            /// </summary>
            /// <param name="force"></param>
            public void AddForce(Vector3 force)
            {
                this.force += force;
                this.count++;
            }
        }
        public class SeparationForce
        {
            /// <summary>
            /// Direction of the force
            /// </summary>
            public Vector3 Value => force;

            /// <summary>
            /// Direction of the force
            /// </summary>
            private Vector3 force;

            public SeparationForce()
            {
                force = Vector3.zero;
            }

            /// <summary>
            /// Reset force
            /// </summary>
            public void ResetForce()
            {
                force = Vector3.zero;
            }

            /// <summary>
            /// Add a new force
            /// </summary>
            /// <param name="force">Force to add</param>
            /// <param name="viewRadius">View Radius of the boid</param>
            public void AddForce(Vector3 force, float viewRadius)
            {
                Vector3 direction = force;
                float distance = direction.magnitude;

                if (distance < viewRadius)
                {
                    this.force += force.normalized / (distance * distance);
                }
            }
        }
        
        /// <summary>
        /// Boid subject transform reference
        /// </summary>
        public Transform Subject => subject;

        /// <summary>
        /// Boids settings
        /// </summary>
        private readonly BoidsSettings settings;

        /// <summary>
        /// Subject fish
        /// </summary>
        private Transform subject;

        /// <summary>
        /// Projection of points on a sphere
        /// </summary>
        private Vector3[] vectorSphericalProjection;

        /// <summary>
        /// Fishes alignment force
        /// </summary>
        private readonly MeanForce alignmentForce;

        /// <summary>
        /// Boids cohesion force
        /// </summary>
        private readonly MeanForce cohesionForce;

        /// <summary>
        /// Boids separation force
        /// </summary>
        private readonly SeparationForce separationForce;

        /// <summary>
        /// Boid speed
        /// </summary>
        private float speed;

        public BoidActor(Transform fishObject, Vector3[] vectorSphericalProjection, BoidsSettings settings)
        {
            this.settings = settings;
            this.subject = fishObject;
            this.vectorSphericalProjection = vectorSphericalProjection;
            this.speed = Random.Range(settings.speedRange.x, settings.speedRange.y);

            alignmentForce = new MeanForce();
            cohesionForce = new MeanForce();
            separationForce = new SeparationForce();
        }

        /// <summary>
        /// Update boid position
        /// </summary>
        /// <param name="localBoids">Local boids</param>
        public void Update(List<BoidActor> localBoids)
        {
            alignmentForce.ResetForce();
            cohesionForce.ResetForce();
            separationForce.ResetForce();

            foreach (BoidActor other in localBoids) 
            {
                if (subject.Equals(other.Subject))
                {
                    continue;
                }

                alignmentForce.AddForce(other.Subject.forward);
                cohesionForce.AddForce((other.Subject.position - subject.position).normalized);
                separationForce.AddForce((subject.position - other.Subject.position), settings.separationRadius);
            }

            Vector3 steerDirection = (subject.forward +
                                      separationForce.Value * settings.separationRatio +
                                      alignmentForce.Value * settings.allignmentRatio +
                                      cohesionForce.Value * settings.cohesionRatio +
                                      GetCollidersForce() * settings.colliderRatio).normalized;

            subject.forward = Vector3.Lerp(subject.forward, steerDirection, settings.steerSmoothRatio);
            subject.position += subject.forward * this.speed * Time.deltaTime;
        }

        /// <summary>
        /// Collider force
        /// </summary>
        private Vector3 GetCollidersForce()
        {
            float furthestUnobstructedDistance = 0f;
            Vector3 escapeDirection = subject.forward;
            RaycastHit hit;

            for (int i = 0; i < vectorSphericalProjection.Length; i++)
            {
                Vector3 rayDirection = subject.TransformDirection(vectorSphericalProjection[i]);

                if (Vector3.Angle(subject.forward, rayDirection) > settings.filterAngle)
                {
                    continue;
                }
                
                if (Physics.Raycast(subject.position, rayDirection, out hit, settings.colliderRadius))
                {
                    Debug.DrawRay(subject.position, rayDirection, Color.red);
                    if (hit.distance > furthestUnobstructedDistance)
                    {
                        escapeDirection = rayDirection;
                        furthestUnobstructedDistance = hit.distance;
                    }
                }
                else
                {
                    escapeDirection = rayDirection;
                    break;
                }
            }

            return escapeDirection;
        }

        public Vector3 GetLocation()
        {
            throw new System.NotImplementedException();
        }

        public Bounds GetBounds()
        {
            throw new System.NotImplementedException();
        }

        public float GetRadius()
        {
            throw new System.NotImplementedException();
        }
    }
}