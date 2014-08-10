using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace itp380.Objects
{
    public class Song
    {
        Hashtable m_Notes = new Hashtable();
        public Hashtable Notes
        {
            get { return m_Notes; }
        }

        public void AddNote(float beat, Note note)
        {
            if (m_Notes.ContainsKey(beat))
            {
                if (m_Notes[beat] is List<Note>)
                {
                    List<Note> notes = (List<Note>)m_Notes[beat];
                    notes.Add(note);
                    m_Notes[beat] = notes;
                }
            }
            else
            {
                List<Note> notes = new List<Note>();
                notes.Add(note);
                m_Notes.Add(beat, notes);
            }
        }

        string m_sName;
        public string Name
        {
            get { return m_sName; }
            set { m_sName = value; }
        }

        string m_sArtist;
        public string Artist
        {
            get { return m_sArtist; }
            set { m_sArtist = value; }
        }

        bool m_bDanceMode;
        public bool DanceMode
        {
            get { return m_bDanceMode; }
            set { m_bDanceMode = value; }
        }

        int m_iDifficulty;
        public int Difficulty
        {
            get { return m_iDifficulty; }
            set { m_iDifficulty = value; }
        }

        float m_fBPM;
        public float BPM
        {
            get { return m_fBPM; }
            set { m_fBPM = value; }
        }

        float m_fLength;
        public float Length
        {
            get { return m_fLength; }
            set { m_fLength = value; }
        }

        float m_fLeadIn;
        public float LeadIn
        {
            get { return m_fLeadIn; }
            set { m_fLeadIn = value; }
        }

        float m_fAppearanceTime;
        public float AppearanceTime
        {
            get { return m_fAppearanceTime; }
            set { m_fAppearanceTime = value; }
        }

        int m_iBeatsPerMeasure;
        public int BeatsPerMeasure
        {
            get { return m_iBeatsPerMeasure; }
            set { m_iBeatsPerMeasure = value; }
        }

        public Song()
        {
        }

    }
}
