using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace itp380.Objects
{
    public class Note : GameObject
    {
        public enum Powerup
        {
            None,
            DoubleMult,
            Shield
        }

        float m_fBeat;
        public float Beat
        {
            get { return m_fBeat; }
            set { m_fBeat = value; }
        }

        bool m_bHit = false;
        public bool Hit
        {
            get { return m_bHit; }
            set { m_bHit = value; }
        }

        bool m_bMiss = false;
        public bool Miss
        {
            get { return m_bMiss; }
            set { m_bMiss = value; }
        }

        bool m_bBad = false;
        public bool Bad
        {
            get { return m_bBad; }
            set { m_bBad = value; }
        }

        int m_iValue;
        public int Value
        {
            get { return m_iValue; }
            set { m_iValue = value; }
        }

        Powerup m_NotePowerup;
        public Powerup NotePowerup
        {
            get { return m_NotePowerup; }
            set { m_NotePowerup = value; }
        }

        Point m_Location = Point.Zero;
        public Point Location
        {
            get
            {
                return m_Location;
            }
            set
            {
                m_Location = value;
            }
        }

        // Used to draw the point value
        bool m_MissedNote = false;
        public bool MissedNote
        {
            get
            {
                return m_MissedNote;
            }
            set
            {
                m_MissedNote = value;
            }
        }

        public void IntToLocation(int loc)
        {
            switch(loc)
            {
                case 1:
                    m_Location.X = -1;
                    m_Location.Y = -1;
                    break;
                case 2:
                    m_Location.X = 0;
                    m_Location.Y = -1;
                    break;
                case 3:
                    m_Location.X = 1;
                    m_Location.Y = -1;
                    break;
                case 4:
                    m_Location.X = -1;
                    m_Location.Y = 0;
                    break;
                case 5:
                    m_Location.X = 0;
                    m_Location.Y = 0;
                    break;
                case 6:
                    m_Location.X = 1;
                    m_Location.Y = 0;
                    break;
                case 7:
                    m_Location.X = -1;
                    m_Location.Y = 1;
                    break;
                case 8:
                    m_Location.X = 0;
                    m_Location.Y = 1;
                    break;
                case 9:
                    m_Location.X = 1;
                    m_Location.Y = 1;
                    break;
                default:
                    break;
            }
        }

        public void SetBad(bool bad)
        {
            Bad = bad;
            if (Bad)
            {
                //m_ModelName = "Asteroid";
            }
        }

        public void SetPowerup(string powerup)
        {

            if (powerup.Equals("DoubleMult"))
            {
                NotePowerup = Objects.Note.Powerup.DoubleMult;
                //m_ModelName = "light_tower";
            }
            else if (powerup.Equals("Shield"))
            {
                NotePowerup = Objects.Note.Powerup.Shield;
                //m_ModelName = "slow_tower";
            }
        }

        public Note(Game game)
            : base(game)
        {
           //m_ModelName = "Projectiles/Sphere";
            Scale = 0.8f;
        }
    }
}
