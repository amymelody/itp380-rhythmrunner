using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace itp380.Objects
{
    public class BeatPlane : GameObject
    {
        public BeatPlane(Game game, Song song)
            : base(game)
        {
            Enabled = false;
            Position = new Vector3(0, 0, -20);
            m_Velocity = new Vector3(0, 0, 20 / song.AppearanceTime);
            m_ModelName = null;
        }

        // Should sync up with player's x and y bounds
        float xBounds = 3.0f;
        float yBounds = 2.0f;

        public enum TunnelDirection
        {
            straight = 0,
            up,
            right,
            down,
            left,
        }
        TunnelDirection m_TunnelDirection = TunnelDirection.straight;

        List<Note> m_ContainedNotes = new List<Note>();
        public List<Note> ContainedNotes
        {
            get
            {
                return m_ContainedNotes;
            }
        }
        Vector3 m_Velocity = new Vector3(0, 0, 0);
        float m_Beat = 0f;

        static SpriteBatch m_SpriteBatch = null;

        public override void Load()
        {
            m_SpriteBatch = new SpriteBatch(GraphicsManager.Get().GraphicsDevice);
        }

        public void addNotesFromSong(Song song, float beat)
        {
            // Set the x and y bounds from the gamestate
            xBounds = GameState.Get().GridWidth / 3.0f;
            yBounds = GameState.Get().GridHeight / 3.0f;
            // Set the beat
            m_Beat = beat;
            // Get the list of beats at the current beat in the song
            List<Note> currentBeatNotes = (List<Note>)(song.Notes[beat]);
            // Iterate through the list of beats and add the notes
            foreach (Note n in currentBeatNotes)
            {
                AddNote(n.Location.X, n.Location.Y, n);
            }
        }

        public void AddNote(int x, int y, Note newNote)
        {
            // Check if x is in bounds
            if (x < -1 || x > 1)
            {
                throw new IndexOutOfRangeException("X value out of range from -1 to 1, x is " + x + " for note: " + newNote);
            }
            // Check if y is in bounds
            if (y < -1 || y > 1)
            {
                throw new IndexOutOfRangeException("Y value out of range from -1 to 1, y is " + y + " for note: " + newNote);
            }
            // Check if the note would overwrite another note
            if (isNoteAt(x, y))
            {
                throw new ArgumentException("Note " + newNote.Beat + " exists at " + x + ", " + y);
            }
            // Add the note
            m_ContainedNotes.Add(newNote);
            // Set its position, and disable it
            newNote.Position = new Vector3(newNote.Location.X * xBounds, newNote.Location.Y * yBounds, Position.Z);
            newNote.Enabled = false;
        }

        // Used for error checking so we don't overwrite a note
        bool isNoteAt(int x, int y)
        {
            foreach (Note n in m_ContainedNotes)
            {
                if (n.Location.X == x && n.Location.Y == y)
                {
                    return true;
                }
            }
            return false;
        }

        public void RemoveNote(int x, int y)
        {
            // Check if x is in bounds
            if (x < -1 || x > 1)
            {
                throw new IndexOutOfRangeException("X value out of range from -1 to 1, x is " + x);
            }
            // Check if y is in bounds
            if (y < -1 || y > 1)
            {
                throw new IndexOutOfRangeException("Y value out of range from -1 to 1, y is " + y);
            }
            // Find the note in the list and delete it
            foreach (Note n in m_ContainedNotes)
            {
                if (n.Location.X == x && n.Location.Y == y)
                {
                    m_ContainedNotes.Remove(n);
                    return;
                }
            }
        }

        public void SetActive(Song song, TunnelDirection direction)
        {
            m_Velocity = new Vector3(0, 0, 20.0f / (song.LeadIn + m_Beat * 60 / song.BPM - (float)MediaPlayer.PlayPosition.TotalSeconds));
            m_TunnelDirection = direction;

            // Set self enabled
            Enabled = true;
            // Set each note enabled
            foreach (Note n in m_ContainedNotes)
            {
                n.Enabled = true;
            }
        }

        public override void Update(float fDeltaTime)
        {
            if (Enabled)
            {
                Position += m_Velocity * fDeltaTime;
                // Update each note's position
                foreach (Note n in m_ContainedNotes)
                {
                    n.Position = new Vector3(n.Location.X * xBounds, n.Location.Y * yBounds, Position.Z);
                }
                // Delete the beatplane and its notes if it goes out of bounds
                if (Position.Z > 3)
                {
                    GameState.Get().RemoveBeatPlane(this);
                    return;
                }

                base.Update(fDeltaTime);
            }
        }

        public override void Draw(float fDeltaTime)
        {
            if (Enabled)
            {
                foreach (Note n in m_ContainedNotes)
                {
                    Color noteColor = Color.White;
                    string labelString = "";
                    float width = 2.0f;

                    m_SpriteBatch.Begin();
                    // Set note color
                    if (n.Bad)
                    {
                        noteColor = Color.Red;
                        if (n.Hit)
                        {
                            labelString = "-10";
                        }
                    }
                    else
                    {
                        if (n.NotePowerup == Note.Powerup.DoubleMult)
                        {
                            noteColor = Color.Yellow;
                            width = 4;
                            if (n.Hit)
                            {
                                labelString = "x2 Points";
                            }
                        }
                        else if (n.NotePowerup == Note.Powerup.Shield)
                        {
                            noteColor = Color.Blue;
                            width = 8;
                            if (n.Hit)
                            {
                                labelString = "+Shield";
                            }
                        }
                        else
                        {
                            noteColor = Color.White;
                            if (n.Hit)
                            {
                                labelString = "+" + n.Value;
                            }
                            else
                            {
                                //labelString = "Miss :(";
                            }
                        }
                    }
                    if (Position.Z <= 1)
                    {
                        float xRotationFactor = 0.0f;
                        float yRotationFactor = 0.0f;
                        switch (m_TunnelDirection)
                        {
                            case TunnelDirection.right:
                                xRotationFactor = Math.Max(0.0f, MathHelper.Lerp(2.0f, 0.0f, (n.Position.Z + 20.0f) / 20.0f));
                                yRotationFactor = Math.Max(0.0f, MathHelper.Lerp(0.1f, 0.0f, (n.Position.Z + 20.0f) / 20.0f));
                                GraphicsManager.Get().DrawRect3D(m_SpriteBatch,
                                    new Vector3(n.Position.X - (xBounds / 2.0f) * Math.Min(1.0f, ((n.Position.Z + 20.0f) / 20.0f)) + xRotationFactor,
                                        n.Position.Y - (yBounds / 2.0f) * Math.Min(1.0f, ((n.Position.Z + 20.0f) / 20.0f)) + yRotationFactor, 0.0f),
                                    new Vector3(n.Position.X - (xBounds / 2.0f) * Math.Min(1.0f, ((n.Position.Z + 20.0f) / 20.0f)) + xRotationFactor,
                                        n.Position.Y + (yBounds / 2.0f) * Math.Min(1.0f, ((n.Position.Z + 20.0f) / 20.0f)) - yRotationFactor, 0.0f),
                                    new Vector3(n.Position.X + (xBounds / 2.0f) * Math.Min(1.0f, ((n.Position.Z + 20.0f) / 20.0f)) + xRotationFactor,
                                        n.Position.Y - (yBounds / 2.0f) * Math.Min(1.0f, ((n.Position.Z + 20.0f) / 20.0f)) - yRotationFactor, 0.0f),
                                    new Vector3(n.Position.X + (xBounds / 2.0f) * Math.Min(1.0f, ((n.Position.Z + 20.0f) / 20.0f)) + xRotationFactor,
                                        n.Position.Y + (yBounds / 2.0f) * Math.Min(1.0f, ((n.Position.Z + 20.0f) / 20.0f)) + yRotationFactor, 0.0f),
                                    0.0f, noteColor, width);
                                break;
                            case TunnelDirection.up:
                                xRotationFactor = Math.Max(0.0f, MathHelper.Lerp(0.5f, 0.0f, (n.Position.Z + 20.0f) / 20.0f));
                                yRotationFactor = Math.Max(0.0f, MathHelper.Lerp(2.0f, 0.0f, (n.Position.Z + 20.0f) / 20.0f));
                                GraphicsManager.Get().DrawRect3D(m_SpriteBatch,
                                    new Vector3(n.Position.X - (xBounds / 2.0f) * Math.Min(1.0f, ((n.Position.Z + 20.0f) / 20.0f)) - xRotationFactor / 2.0f,
                                        n.Position.Y - (yBounds / 2.0f) * Math.Min(1.0f, ((n.Position.Z + 20.0f) / 20.0f)) + yRotationFactor, 0.0f),
                                    new Vector3(n.Position.X - (xBounds / 2.0f) * Math.Min(1.0f, ((n.Position.Z + 20.0f) / 20.0f)) - xRotationFactor,
                                        n.Position.Y + (yBounds / 2.0f) * Math.Min(1.0f, ((n.Position.Z + 20.0f) / 20.0f)) + yRotationFactor, 0.0f),
                                    new Vector3(n.Position.X + (xBounds / 2.0f) * Math.Min(1.0f, ((n.Position.Z + 20.0f) / 20.0f)) + xRotationFactor / 2.0f,
                                        n.Position.Y - (yBounds / 2.0f) * Math.Min(1.0f, ((n.Position.Z + 20.0f) / 20.0f)) + yRotationFactor, 0.0f),
                                    new Vector3(n.Position.X + (xBounds / 2.0f) * Math.Min(1.0f, ((n.Position.Z + 20.0f) / 20.0f)) + xRotationFactor,
                                        n.Position.Y + (yBounds / 2.0f) * Math.Min(1.0f, ((n.Position.Z + 20.0f) / 20.0f)) + yRotationFactor, 0.0f),
                                    0.0f, noteColor, width);
                                break;
                            case TunnelDirection.left:
                                xRotationFactor = Math.Max(0.0f, MathHelper.Lerp(2.0f, 0.0f, (n.Position.Z + 20.0f) / 20.0f));
                                yRotationFactor = Math.Max(0.0f, MathHelper.Lerp(0.1f, 0.0f, (n.Position.Z + 20.0f) / 20.0f));
                                GraphicsManager.Get().DrawRect3D(m_SpriteBatch,
                                    new Vector3(n.Position.X - (xBounds / 2.0f) * Math.Min(1.0f, ((n.Position.Z + 20.0f) / 20.0f)) - xRotationFactor,
                                        n.Position.Y - (yBounds / 2.0f) * Math.Min(1.0f, ((n.Position.Z + 20.0f) / 20.0f)) - yRotationFactor, 0.0f),
                                    new Vector3(n.Position.X - (xBounds / 2.0f) * Math.Min(1.0f, ((n.Position.Z + 20.0f) / 20.0f)) - xRotationFactor,
                                        n.Position.Y + (yBounds / 2.0f) * Math.Min(1.0f, ((n.Position.Z + 20.0f) / 20.0f)) + yRotationFactor, 0.0f),
                                    new Vector3(n.Position.X + (xBounds / 2.0f) * Math.Min(1.0f, ((n.Position.Z + 20.0f) / 20.0f)) - xRotationFactor,
                                        n.Position.Y - (yBounds / 2.0f) * Math.Min(1.0f, ((n.Position.Z + 20.0f) / 20.0f)) + yRotationFactor, 0.0f),
                                    new Vector3(n.Position.X + (xBounds / 2.0f) * Math.Min(1.0f, ((n.Position.Z + 20.0f) / 20.0f)) - xRotationFactor,
                                        n.Position.Y + (yBounds / 2.0f) * Math.Min(1.0f, ((n.Position.Z + 20.0f) / 20.0f)) - yRotationFactor, 0.0f),
                                    0.0f, noteColor, width);
                                break;
                            case TunnelDirection.down:
                                xRotationFactor = Math.Max(0.0f, MathHelper.Lerp(0.5f, 0.0f, (n.Position.Z + 20.0f) / 20.0f));
                                yRotationFactor = Math.Max(0.0f, MathHelper.Lerp(2.0f, 0.0f, (n.Position.Z + 20.0f) / 20.0f));
                                GraphicsManager.Get().DrawRect3D(m_SpriteBatch,
                                    new Vector3(n.Position.X - (xBounds / 2.0f) * Math.Min(1.0f, ((n.Position.Z + 20.0f) / 20.0f)) - xRotationFactor,
                                        n.Position.Y - (yBounds / 2.0f) * Math.Min(1.0f, ((n.Position.Z + 20.0f) / 20.0f)) - yRotationFactor, 0.0f),
                                    new Vector3(n.Position.X - (xBounds / 2.0f) * Math.Min(1.0f, ((n.Position.Z + 20.0f) / 20.0f)) - xRotationFactor / 2.0f,
                                        n.Position.Y + (yBounds / 2.0f) * Math.Min(1.0f, ((n.Position.Z + 20.0f) / 20.0f)) - yRotationFactor, 0.0f),
                                    new Vector3(n.Position.X + (xBounds / 2.0f) * Math.Min(1.0f, ((n.Position.Z + 20.0f) / 20.0f)) + xRotationFactor,
                                        n.Position.Y - (yBounds / 2.0f) * Math.Min(1.0f, ((n.Position.Z + 20.0f) / 20.0f)) - yRotationFactor, 0.0f),
                                    new Vector3(n.Position.X + (xBounds / 2.0f) * Math.Min(1.0f, ((n.Position.Z + 20.0f) / 20.0f)) + xRotationFactor / 2.0f,
                                        n.Position.Y + (yBounds / 2.0f) * Math.Min(1.0f, ((n.Position.Z + 20.0f) / 20.0f)) - yRotationFactor, 0.0f),
                                    0.0f, noteColor, width);
                                break;
                            default: // Also used for straight
                                GraphicsManager.Get().DrawRect3D(m_SpriteBatch,
                                    new Vector3(n.Position.X - (xBounds / 2.0f) * Math.Min(1.0f, ((n.Position.Z + 20.0f) / 20.0f)),
                                        n.Position.Y - (yBounds / 2.0f) * Math.Min(1.0f, ((n.Position.Z + 20.0f) / 20.0f)), 0.0f),
                                    new Vector3(n.Position.X - (xBounds / 2.0f) * Math.Min(1.0f, ((n.Position.Z + 20.0f) / 20.0f)),
                                        n.Position.Y + (yBounds / 2.0f) * Math.Min(1.0f, ((n.Position.Z + 20.0f) / 20.0f)), 0.0f),
                                    new Vector3(n.Position.X + (xBounds / 2.0f) * Math.Min(1.0f, ((n.Position.Z + 20.0f) / 20.0f)),
                                        n.Position.Y - (yBounds / 2.0f) * Math.Min(1.0f, ((n.Position.Z + 20.0f) / 20.0f)), 0.0f),
                                    new Vector3(n.Position.X + (xBounds / 2.0f) * Math.Min(1.0f, ((n.Position.Z + 20.0f) / 20.0f)),
                                        n.Position.Y + (yBounds / 2.0f) * Math.Min(1.0f, ((n.Position.Z + 20.0f) / 20.0f)), 0.0f),
                                    0.0f, noteColor, width);
                                break;
                        }
                    }
                    else if (Position.Z >= 0 && n.Hit)
                    {
                        Vector3 labelPosition = n.Position;
                        labelPosition = new Vector3(n.Position.X, n.Position.Y + n.Position.Z / 3 + 0.7f, n.Position.Z);
                        GraphicsManager.Get().DrawNoteHitLabel(m_SpriteBatch, labelPosition, labelString, 1.5f - Position.Z / 10, noteColor);
                    }
                    m_SpriteBatch.End();
                }
            }
            base.Draw(fDeltaTime);
        }
    }
}
