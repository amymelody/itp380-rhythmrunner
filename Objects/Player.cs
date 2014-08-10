using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace itp380.Objects
{
    public class Player : GameObject
    {
        public Player(Game game)
            : base(game)
        {
            m_ModelName = "SpaceShip";
            m_TargetLocations.AddLast(new Point(0, 0));
            // Rotate the model so he's pointing into the forward direction, but tilted slightly upwards
            Rotation = Quaternion.CreateFromAxisAngle(new Vector3(0, 1, 0), MathHelper.PiOver2);
            Rotation *= Quaternion.CreateFromAxisAngle(Vector3.Backward, MathHelper.PiOver2 / 4);
            // Position the player
            Position = new Vector3(0f, 0f, 0f);
            // Scale of player
            Scale = 0.4f;
        }

        float xBounds;
        float yBounds;

        float m_MoveTime = 0.07f;
        float m_CurrentMoveTime = 0.0f;

        Point m_CurrentLocation = Point.Zero;
        public Point CurrentLocation
        {
            get
            {
                return m_CurrentLocation;
            }
            set
            {
                m_CurrentLocation = value;
            }
        }

        // The first item in the list is the location it is currently one
        LinkedList<Point> m_TargetLocations = new LinkedList<Point>();

        public void MoveUp()
        {
            if (m_TargetLocations.Last().Y < 1)
            {
                m_TargetLocations.AddLast(new Point(m_TargetLocations.Last().X, m_TargetLocations.Last().Y + 1));
            }
        }

        public void MoveLeft()
        {
            if (m_TargetLocations.Last().X > -1)
            {
                m_TargetLocations.AddLast(new Point(m_TargetLocations.Last().X - 1, m_TargetLocations.Last().Y));
            }
        }

        public void MoveDown()
        {
            if (m_TargetLocations.Last().Y > -1)
            {
                m_TargetLocations.AddLast(new Point(m_TargetLocations.Last().X, m_TargetLocations.Last().Y - 1));
            }
        }

        public void MoveRight()
        {
            if (m_TargetLocations.Last().X < 1)
            {
                m_TargetLocations.AddLast(new Point(m_TargetLocations.Last().X + 1, m_TargetLocations.Last().Y));
            }
        }

        public override void Update(float fDeltaTime)
        {
            if (m_TargetLocations.Count > 1)
            {
                // Get the next target location
                Point currentTargetLocation = m_TargetLocations.ElementAt(1);

                // If the target x is different from the current x
                m_CurrentMoveTime += fDeltaTime;
                if (m_CurrentMoveTime > m_MoveTime)
                {
                    m_CurrentMoveTime = m_MoveTime;
                }
                // Calculate the position based on the lerp
                Position = Vector3.Lerp(new Vector3(m_CurrentLocation.X * xBounds, m_CurrentLocation.Y * yBounds, 0.0f),
                    new Vector3(currentTargetLocation.X * xBounds, currentTargetLocation.Y * yBounds, 0.0f),
                    m_CurrentMoveTime / m_MoveTime);
                if (m_CurrentMoveTime >= m_MoveTime)
                {
                    // Reset the move time
                    m_CurrentMoveTime = 0.0f;
                    // Update the currentlocation
                    m_CurrentLocation = new Point(currentTargetLocation.X, currentTargetLocation.Y);
                    // Update the target location list
                    if (m_TargetLocations.Count > 1)
                    {
                        m_TargetLocations.RemoveFirst();
                    }
                }
            }
            base.Update(fDeltaTime);
        }

        public void setPlayerMovementLimitations(float xLimit, float yLimit)
        {
            xBounds = xLimit;
            yBounds = yLimit;
        }
    }
}
