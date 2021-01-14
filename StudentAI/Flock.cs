using System.Collections.Generic;
using AI.SteeringBehaviors.Core;

namespace AI.SteeringBehaviors.StudentAI
{
    public class Flock
    {
        public float AlignmentStrength { get; set; }
        public float CohesionStrength { get; set; }
        public float SeparationStrength { get; set; }
        public List<MovingObject> Boids { get; protected set; }
        public Vector3 AveragePosition { get; set; }
        protected Vector3 AverageForward { get; set; }
        public float FlockRadius { get; set; }

        #region TODO
        //https://gamedevelopment.tutsplus.com/tutorials/3-simple-rules-of-flocking-behaviors-alignment-cohesion-and-separation--gamedev-3444
        //The above and Blanchard's lecture slides were helpful references for this exercise
        public Flock()
        {
            /*
            Boids = new List<MovingObject>();
            AveragePosition = new Vector3();
            AverageForward = new Vector3();
            AlignmentStrength = 1;
            CohesionStrength = 1;
            SeparationStrength = 1;
            FlockRadius = 10;
            */
        }
        public Vector3 getAverageForward()
        {
            Vector3 sum = new Vector3();
            foreach (MovingObject sibling in Boids)
            {
                sum = sum + sibling.Velocity;
            }
            return (sum/Boids.Count);
        }
        public Vector3 getAveragePosition()
        {
            Vector3 sum = new Vector3();
            foreach (MovingObject sibling in Boids)
            {
                sum = sum + sibling.Position;
            }
            return (sum/Boids.Count);
        }
        public virtual void Update(float deltaTime)
        {
            // Update goes here
            /*
             * public virtual void Update(float deltaTime)
                {
                    if (Velocity.LengthSquared > MaxSpeed * MaxSpeed)
                    {
                        Velocity = Vector3.Normalize(Velocity);
                        Velocity *= MaxSpeed;
                    }
                    Position += Velocity * deltaTime;
                }
             */
            AverageForward = getAverageForward();
            AveragePosition = getAveragePosition();
            foreach (MovingObject sibling in Boids)
            {
                Vector3 accel = calc_alignment_accel(sibling);
                accel = accel + calc_cohesion_accel(sibling);
                accel = accel + calc_separation_accel(sibling);
                accel = accel * sibling.MaxSpeed * deltaTime;
                sibling.Velocity = sibling.Velocity + accel;
                if (sibling.Velocity.Length > sibling.MaxSpeed)
                {
                    sibling.Velocity.Normalize();
                    sibling.Velocity = sibling.Velocity * sibling.MaxSpeed;
                }
                sibling.Update(deltaTime);   
            }
        }

        private Vector3 calc_alignment_accel(MovingObject boid)
        {
            Vector3 accel = AverageForward / boid.MaxSpeed;
            if(accel.Length > 1)
            {
                accel.Normalize();
            }
            return accel*AlignmentStrength;   
        }
        private Vector3 calc_cohesion_accel(MovingObject boid)
        {
            Vector3 accel = AveragePosition - boid.Position;
            float distance = accel.Length;
            accel.Normalize();
            if (distance<FlockRadius)
            {
                accel = accel * distance / FlockRadius;
            }
            return accel * CohesionStrength;
        }
        private Vector3 calc_separation_accel(MovingObject boid)
        {
            Vector3 sum = new Vector3();
            //int neightborCount = 0;
            foreach(MovingObject sibling in Boids)
            {
                if (boid == sibling)
                    continue;
                Vector3 accel = boid.Position - sibling.Position;
                float distance = accel.Length;
                float safeDistance = sibling.SafeRadius + sibling.SafeRadius;
                if (distance < safeDistance)
                {
                    accel.Normalize();
                    accel = accel * (safeDistance - distance) / safeDistance;
                    sum = sum + accel;
                }
            }
            if (sum.Length > 1.0)
                sum.Normalize();
            return sum * SeparationStrength;
        }

        #endregion
    }
}
