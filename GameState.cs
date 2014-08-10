//-----------------------------------------------------------------------------
// The main GameState Singleton. All actions that change the game state,
// as well as any global updates that happen during gameplay occur in here.
// Because of this, the file is relatively lengthy.
//
// __Defense Sample for Game Programming Algorithms and Techniques
// Copyright (C) Sanjay Madhav. All rights reserved.
//
// Released under the Microsoft Permissive License.
// See LICENSE.txt for full details.
//-----------------------------------------------------------------------------
using System;

using System.Collections;
using System.IO; 
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;

namespace itp380
{
	public enum eGameState
	{
		None = 0,
		MainMenu,
        SongSelection, 
        OptionsMenu, 
        HighScoreScreen,
		Gameplay,
        Edit
	}

    public enum ePermissions
    {
        None = 0,
        Normal,
        Dancepad,
        SongEdit,
        HighScores
    }

	public class GameState : itp380.Patterns.Singleton<GameState>
	{
		Game m_Game;
	    eGameState m_State;
        public ePermissions m_permissions; 
        public List<HighScorers> m_HighScorers = new List<HighScorers>();
        public List<SongList> m_SongList = new List<SongList>();
        private const int HIGHSCORE_COUNT = 5;


        public string m_currentSong;

		public eGameState State
		{
			get { return m_State; }
		}

		eGameState m_NextState;
		Stack<UI.UIScreen> m_UIStack;
		bool m_bPaused = false;
        public bool IsPaused
        {
            get { return m_bPaused; }
            set { m_bPaused = value; }
        }

		// Keeps track of all active game objects
		LinkedList<GameObject> m_GameObjects = new LinkedList<GameObject>();

		// Camera Information
		Camera m_Camera;
		public Camera Camera
		{
			get { return m_Camera; }
		}

		public Matrix CameraMatrix
		{
			get { return m_Camera.CameraMatrix; }
		}

		// Timer class for the global GameState
		Utils.Timer m_Timer = new Utils.Timer();

		UI.UIGameplay m_UIGameplay;
		
		public void Start(Game game)
		{
			m_Game = game;
			m_State = eGameState.None;
			m_UIStack = new Stack<UI.UIScreen>();

			m_Camera = new Camera(m_Game);
		}

		public void SetState(eGameState NewState)
		{
			m_NextState = NewState;
		}

		private void HandleStateChange()
		{
			if (m_NextState == m_State)
				return;

			switch (m_NextState)
			{
				case eGameState.MainMenu:
					m_UIStack.Clear();
					m_UIGameplay = null;
					m_Timer.RemoveAll();
					m_UIStack.Push(new UI.UIMainMenu(m_Game.Content));
					ClearGameObjects();
					break;
                case eGameState.OptionsMenu:
                    ShowOptionsMenu(); 
                    break; 
                case eGameState.HighScoreScreen:
                    ShowHighScores(); 
                    break; 
				case eGameState.Gameplay:
					SetupGameplay();
					break;
                case eGameState.SongSelection:
                    ShowSongs();
                    break; 
                case eGameState.Edit:
                    SetupEdit();
                    break;
			}

			m_State = m_NextState;
		}

		protected void ClearGameObjects()
		{
			// Clear out any and all game objects
			foreach (GameObject o in m_GameObjects)
			{
				RemoveGameObject(o, false);
			}
			m_GameObjects.Clear();
		}

        // Gameplay Variables
        Objects.Player m_Player;
        public Objects.Player PlayerCurrent
        {
            get
            {
                return m_Player;
            }
        }
        Objects.Song m_CurrentSong;
        float m_CurrentBeat = 0f;
        
        // Grid variables referenced by player, graphicsmanager, and beatplane
        float m_GridWidth = 22.0f;
        public float GridWidth
        {
            get
            {
                return m_GridWidth;
            }
        }
        float m_GridHeight = 18.0f;
        public float GridHeight
        {
            get
            {
                return m_GridHeight;
            }
        }

        Objects.BeatPlane.TunnelDirection m_TunnelDirection = Objects.BeatPlane.TunnelDirection.straight;

        // Song Data
        Song m_CurrentSongAudio = null;
        public Song CurrentSongAudio
        {
            get
            {
                return m_CurrentSongAudio;
            }
        }
        VisualizationData m_VisualData = null;
        public VisualizationData VisualData
        {
            get
            {
                return m_VisualData;
            }
        }

        //Lists
        List<Objects.Song> m_Songs = new List<Objects.Song>();
        Hashtable m_BeatPlanes = new Hashtable();
        List<Objects.Note> m_Notes = new List<Objects.Note>();

        string m_SongFile = null;
        public string SongFile
        {
            get
            {
                return m_SongFile;
            }
            set
            {
                m_SongFile = value;
            }
        }

        //These could be changed depending on how we want to balance the game
        int m_iComboReq = 16; //How many beats must you hit in succession to increment the multiplier?
        int m_iMultMax = 4; //What is the highest the multiplier can be?
        int m_iHealthMax = 50;
        float m_fTolerance = 1.0f;

        GameTime m_GameTime = new GameTime();

        bool m_bDanceMode = true;
        public bool DanceMode
        {
            get { return m_bDanceMode; }
            set { m_bDanceMode = value; }
        }

        int m_iCombo = 0;
        public int Combo
        {
            get { return m_iCombo; }
            set { m_iCombo = value; }
        }

        int m_iMultiplier = 1;
        public int Multiplier
        {
            get { return m_iMultiplier; }
            set { m_iMultiplier = value; }
        }

        int m_iScore = 0;
        public int Score
        {
            get { return m_iScore; }
            set { m_iScore = value; }
        }

        int m_iHitNotes;
        public int HitNotes
        {
            get { return m_iHitNotes; }
            set { m_iHitNotes = value; }
        }

        int m_iTotalNotes;
        public int TotalNotes
        {
            get { return m_iTotalNotes; }
            set { m_iTotalNotes = value; }
        }

        int m_iHealth;
        public int Health
        {
            get { return m_iHealth; }
            set { m_iHealth = value; }
        }

        int m_iMultMult = 1;
        public int MultMult
        {
            get { return m_iMultMult; }
            set { m_iMultMult = value; }
        }

        int m_iShield = 0;
        public int Shield
        {
            get { return m_iShield; }
            set { m_iShield = value; }
        }

        int m_iMissCount = 0;
        public int MissCount
        {
            get { return m_iMissCount; }
            set { m_iMissCount = value; }
        }

        public void SetupEdit()
        {
            ClearGameObjects();
            m_UIStack.Clear();
            m_UIStack.Push(new UI.UISongEdit(m_Game.Content, m_Game, m_SongFile));
        }

		public void SetupGameplay()
		{
			ClearGameObjects();
			m_UIStack.Clear();
			m_UIGameplay = new UI.UIGameplay(m_Game.Content);
			m_UIStack.Push(m_UIGameplay);

			m_bPaused = false;
			GraphicsManager.Get().ResetProjection();
			
			m_Timer.RemoveAll();

		    //Reset all gameplay variables
            Multiplier = 1;
            Combo = 0;
            Score = 0;
            HitNotes = 0;
            MultMult = 1;
            Shield = 0;
			
            

            // Set grid dimensions and player movement limit
            GraphicsManager.Get().SetGridDimensions(m_GridWidth, m_GridHeight);

            if (!DanceMode)
            {
                m_Player = new Objects.Player(m_Game);
                SpawnGameObject(m_Player);
                m_Camera.FollowObject = m_Player;
                m_Player.setPlayerMovementLimitations(m_GridWidth / 3.0f, m_GridHeight / 3.0f);
            }
            
            ParseFile(m_SongFile);


            foreach(Objects.BeatPlane b in m_BeatPlanes.Values) {
                RemoveBeatPlane(b);
            } 
            m_BeatPlanes.Clear();
            m_CurrentSong = m_Songs[0];

            Health = m_iHealthMax / 2;
            TotalNotes = 0;

            SetState(eGameState.Gameplay);
            createTestBeatPlanes();

            MediaPlayer.Stop();
            MediaPlayer.IsVisualizationEnabled = true;
            m_CurrentSongAudio = m_Game.Content.Load<Song>(m_SongFile.Replace(".txt", ""));
            
            m_VisualData = new VisualizationData();
            MediaPlayer.Play(m_CurrentSongAudio);

            m_CurrentBeat = 0.0f;

            // Get the beatplane at the beat
            Objects.BeatPlane beatplaneAtCurrentBeat = (Objects.BeatPlane)m_BeatPlanes[m_CurrentBeat];
            if (beatplaneAtCurrentBeat != null)
            {
                // Set it active
                beatplaneAtCurrentBeat.SetActive(m_CurrentSong, Objects.BeatPlane.TunnelDirection.straight);

            }

        }

        Objects.Song createTestSong()
        {
            Objects.Song returnSong = new Objects.Song();
            returnSong.BPM = 120f;

            Random randomNoteGenerator = new Random();
            // Create random notes
            for (float i = 1f; i < 600f; i += randomNoteGenerator.Next(1, 3))
            {
                Objects.Note newNote = new Objects.Note(m_Game);
                newNote.Location = new Point(randomNoteGenerator.Next(-1, 2), randomNoteGenerator.Next(-1, 2));
                returnSong.AddNote(i, newNote);
                SpawnGameObject(newNote);
                m_Notes.Add(newNote);
            }

            return returnSong;
        }

        void createTestBeatPlanes()
        {
            m_BeatPlanes.Clear();
            m_Notes.Clear();
            m_CurrentBeat = 0f;

            // Go through each beat in the song
            foreach (DictionaryEntry entry in m_CurrentSong.Notes)
            {
                // New beatplane
                Objects.BeatPlane newBeatPlane = new Objects.BeatPlane(m_Game, m_CurrentSong);
                // Populate the beatplane with notes
                newBeatPlane.addNotesFromSong(m_CurrentSong, (float)entry.Key);
                // Add the beatplane to the list of beatplanes for update
                m_BeatPlanes.Add(entry.Key, newBeatPlane);
                // Spawn the beatplane
                SpawnGameObject(newBeatPlane);
            }
        }

        void HitNote(int x, int y)
        {
            GraphicsManager.Get().DrawHitRect(new Point(x,y));

            bool noteExists = false;

            foreach (DictionaryEntry entry in m_BeatPlanes)
            {
                Objects.BeatPlane plane = (Objects.BeatPlane)entry.Value;
                if (plane.Enabled && (plane.Position.Z < m_fTolerance) && (plane.Position.Z > -1.0f * m_fTolerance))
                {
                    foreach (Objects.Note n in plane.ContainedNotes)
                    {
                        if (!n.Hit && !n.Miss && n.Location.X == x && n.Location.Y == y)
                        {
                            noteExists = true;
                            if (n.Bad)
                            {
                                if ((plane.Position.Z < m_fTolerance / 3.0f) && (plane.Position.Z > -1.0f * m_fTolerance / 8.0f))
                                {
                                    if (Shield > 0)
                                    {
                                        Shield--;
                                    }
                                    else
                                    {
                                        Score -= n.Value;
                                        Combo = 0;
                                        Multiplier = MultMult;
                                        Health -= 1;
                                        if (Health <= 0)
                                        {
                                            //FAILURE
                                            MediaPlayer.Stop();
                                            SetState(eGameState.MainMenu);
                                        }
                                    }
                                    n.Hit = true;
                                }
                            }
                            else
                            {
                                if (Health != m_iHealthMax)
                                {
                                    Health += 1;
                                    if (Health > 50)
                                    {
                                        Health = 50;
                                    }
                                }
                                Score += n.Value * Multiplier;
                                n.Value = n.Value * Multiplier;
                                Combo++;
                                if (n.NotePowerup == Objects.Note.Powerup.DoubleMult)
                                {
                                    MultMult = 2;
                                    Multiplier *= MultMult;
                                    m_Timer.AddTimer("DoubleMult Expire", 32.0f * 60.0f / m_CurrentSong.BPM, DoubleMultExpire, false);
                                }
                                if (n.NotePowerup == Objects.Note.Powerup.Shield)
                                {
                                    Shield = 3;
                                }
                                if ((Combo % m_iComboReq == 0) && (Multiplier < m_iMultMax * MultMult))
                                {
                                    Multiplier += MultMult;
                                }
                                n.Hit = true;
                                MissCount = 0;
                                HitNotes++;
                                TotalNotes++;
                            }
                        }
                    }
                }
            }

            if (!noteExists)
            {
                Combo = 0;
                Multiplier = MultMult;
                Health -= 1;
                if (Health <= 0)
                {
                    //FAILURE
                    MediaPlayer.Stop();
                    SetState(eGameState.MainMenu);
                }
            }
        }

		public void Update(float fDeltaTime)
		{
			HandleStateChange();

			switch (m_State)
			{
				case eGameState.MainMenu:
					UpdateMainMenu(fDeltaTime);
					break;
				case eGameState.Gameplay:
					UpdateGameplay(fDeltaTime);
					break;
			}

			foreach (UI.UIScreen u in m_UIStack)
			{
				u.Update(fDeltaTime);
			}
		}

		void UpdateMainMenu(float fDeltaTime)
		{

		}

		void UpdateGameplay(float fDeltaTime)
		{
			if (!IsPaused)
			{
				m_Camera.Update(fDeltaTime);

                if (MediaPlayer.PlayPosition.TotalSeconds + m_CurrentSong.AppearanceTime >= m_CurrentSong.LeadIn + m_CurrentBeat * 60 / m_CurrentSong.BPM)
                {
                    m_CurrentBeat += 0.25f;


                    if (((int)(m_CurrentBeat * 4)) % 32 == 0)
                    {
                        Array tunnelDirectionValues = Enum.GetValues(typeof(Objects.BeatPlane.TunnelDirection));
                        Random rng = new Random();
                        Objects.BeatPlane.TunnelDirection newTunnelDirection = 
                            (Objects.BeatPlane.TunnelDirection) tunnelDirectionValues.GetValue(rng.Next(0, tunnelDirectionValues.Length));
                        while (newTunnelDirection == m_TunnelDirection)
                        {
                            newTunnelDirection = (Objects.BeatPlane.TunnelDirection)tunnelDirectionValues.GetValue(rng.Next(0, tunnelDirectionValues.Length));
                        }
                        m_TunnelDirection = newTunnelDirection;
                    }
                    Objects.BeatPlane beatplaneAtCurrentBeat = (Objects.BeatPlane)m_BeatPlanes[m_CurrentBeat];
                    if (beatplaneAtCurrentBeat != null && !beatplaneAtCurrentBeat.Enabled)
                    {
                        // Set it active
                        beatplaneAtCurrentBeat.SetActive(m_CurrentSong, m_TunnelDirection);
                    }
                }

                
                if (m_NextState == eGameState.Gameplay && MediaPlayer.State == MediaState.Stopped)
                {
                    if (CheckNewHighScore())
                    {
                        IsPaused = true; 
                        ShowNewScoreMenu();
                    }
                    else
                    {
                        SetState(eGameState.MainMenu);
                    }
                }
                

				// Update objects in the world
				// We have to make a temp copy in case the objects list changes
				LinkedList<GameObject> temp = new LinkedList<GameObject>(m_GameObjects);
				foreach (GameObject o in temp)
				{
					if (o.Enabled)
					{
						o.Update(fDeltaTime);
					}
				}
				m_Timer.Update(fDeltaTime);

                if (MediaPlayer.State == MediaState.Playing)
                {
                    MediaPlayer.GetVisualizationData(m_VisualData);
                    GraphicsManager.Get().VisualData = m_VisualData;
                }

                if (!DanceMode)
                {
                    foreach (DictionaryEntry entry in m_BeatPlanes)
                    {
                        Objects.BeatPlane plane = (Objects.BeatPlane)entry.Value;
                        if (plane.Enabled && (plane.Position.Z < m_Player.Position.Z + m_fTolerance) && (plane.Position.Z > m_Player.Position.Z - m_fTolerance))
                        {
                            foreach (Objects.Note n in plane.ContainedNotes)
                            {
                                if (!n.Hit && !n.Miss && n.Location == m_Player.CurrentLocation)
                                {
                                    if (n.Bad)
                                    {
                                        if ((plane.Position.Z < m_Player.Position.Z + m_fTolerance / 3.0f) && (plane.Position.Z > m_Player.Position.Z))
                                        {
                                            if (Shield > 0)
                                            {
                                                Shield--;
                                            }
                                            else
                                            {
                                                Score -= n.Value;
                                                Combo = 0;
                                                Multiplier = MultMult;
                                                Health -= 1;
                                                if (Health <= 0)
                                                {
                                                    //FAILURE
                                                    MediaPlayer.Stop();
                                                    SetState(eGameState.MainMenu);
                                                }
                                            }
                                            n.Hit = true;
                                        }
                                    }
                                    else
                                    {
                                        if (Health != m_iHealthMax)
                                        {
                                            Health += 1;
                                            if (Health > 50)
                                            {
                                                Health = 50;
                                            }
                                        }
                                        Score += n.Value * Multiplier;
                                        n.Value = n.Value * Multiplier;
                                        Combo++;
                                        if (n.NotePowerup == Objects.Note.Powerup.DoubleMult)
                                        {
                                            MultMult = 2;
                                            Multiplier *= MultMult;
                                            m_Timer.AddTimer("DoubleMult Expire", 32.0f * 60.0f / m_CurrentSong.BPM, DoubleMultExpire, false);
                                        }
                                        if (n.NotePowerup == Objects.Note.Powerup.Shield)
                                        {
                                            Shield = 3;
                                        }
                                        if ((Combo % m_iComboReq == 0) && (Multiplier < m_iMultMax * MultMult))
                                        {
                                            Multiplier += MultMult;
                                        }
                                        n.Hit = true;
                                        MissCount = 0;
                                        HitNotes++;
                                        TotalNotes++;
                                    }
                                }
                            }
                        }
                    }
                }

			}
		}

        public void DoubleMultExpire()
        {
            Multiplier /= MultMult;
            MultMult = 1;
        }

        public void ParseFile(string filename)
        {
            Objects.Song song = new Objects.Song();
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Content", filename);
            string line;
            StreamReader sr = new StreamReader(path);
            bool gameElements = false;

            while ((line = sr.ReadLine()) != null)
            {
                if (gameElements)
                {
                    Objects.Note note = new Objects.Note(m_Game);
                    note.Beat = float.Parse(line.Remove(line.IndexOf(",")));
                    line = line.Substring(line.IndexOf(",") + 2);
                    string type = line.Remove(line.IndexOf(","));
                    if (type.Equals("Bad"))
                    {
                        note.SetBad(true);
                        note.NotePowerup = Objects.Note.Powerup.None;
                    }
                    else
                    {
                        note.SetBad(false);
                        if (type.Equals("Good"))
                        {
                            note.NotePowerup = Objects.Note.Powerup.None;
                        }
                        else
                        {
                            note.SetPowerup(type);
                        }
                    }
                    if (type.Equals("Good") || type.Equals("Bad")) {
                        line = line.Substring(line.IndexOf(",") + 2);
                        note.Value = int.Parse(line.Remove(line.IndexOf(",")));
                    }
                    else
                    {
                        note.Value = 0;
                    }
                    line = line.Substring(line.IndexOf(",") + 2);
                    note.IntToLocation(int.Parse(line));

                    song.AddNote(note.Beat, note);
                    SpawnGameObject(note);
                    m_Notes.Add(note);
                }
                
                if (line.StartsWith("#"))
                {
                    if (line.Contains("GameElements"))
                    {
                        gameElements = true;
                    }
                    continue;
                }
                else if (line.StartsWith("Name:"))
                {
                    song.Name = line.Remove(0, 6);
                }
                else if (line.StartsWith("Artist:"))
                {
                    song.Artist = line.Remove(0, 8);
                }
                else if (line.StartsWith("Mode:"))
                {
                    string mode = line.Remove(0, 6);
                    if (mode.Equals("Dance"))
                    {
                        song.DanceMode = true;
                    }
                    else
                    {
                        song.DanceMode = false;
                    }
                }
                else if (line.StartsWith("Difficulty:"))
                {
                    song.Difficulty = int.Parse(line.Remove(0, 12));
                }
                else if (line.StartsWith("BPM:"))
                {
                    song.BPM = float.Parse(line.Remove(0, 5));
                }
                else if (line.StartsWith("SongLength:"))
                {
                    song.Length = float.Parse(line.Remove(0, 12));
                }
                else if (line.StartsWith("LeadIn:"))
                {
                    song.LeadIn = float.Parse(line.Remove(0, 8));
                }
                else if (line.StartsWith("BeatsPerMeasure:"))
                {
                    song.BeatsPerMeasure = int.Parse(line.Remove(0, 17));
                }
                else if (line.StartsWith("AppearanceTime:"))
                {
                    song.AppearanceTime = float.Parse(line.Remove(0, 16));
                }
            }

            m_Songs.Clear();
            m_Songs.Add(song);
            sr.Close();
        }

		public void SpawnGameObject(GameObject o)
		{
			o.Load();
			m_GameObjects.AddLast(o);
			GraphicsManager.Get().AddGameObject(o);
		}

		public void RemoveGameObject(GameObject o, bool bRemoveFromList = true)
		{
			o.Enabled = false;
			o.Unload();
			GraphicsManager.Get().RemoveGameObject(o);
			if (bRemoveFromList)
			{
				m_GameObjects.Remove(o);
			}
		}

		public void MouseClick(Point Position)
		{
			if (m_State == eGameState.Gameplay && !IsPaused)
			{
				// TODO: Respond to mouse clicks here
			}
		}

		// I'm the last person to get keyboard input, so don't need to remove
		public void KeyboardInput(SortedList<eBindings, BindInfo> binds, float fDeltaTime)
		{
			if (m_State == eGameState.Gameplay && !IsPaused)
			{
                if (!DanceMode)
                {
                    // Player Movement
                    if (binds.ContainsKey(eBindings.Player_Up) || binds.ContainsKey(eBindings.Player_UpArrow))
                    {
                        m_Player.MoveUp();
                    }
                    if (binds.ContainsKey(eBindings.Player_Left) || binds.ContainsKey(eBindings.Player_LeftArrow))
                    {
                        m_Player.MoveLeft();
                    }
                    if (binds.ContainsKey(eBindings.Player_Down) || binds.ContainsKey(eBindings.Player_DownArrow))
                    {
                        m_Player.MoveDown();
                    }
                    if (binds.ContainsKey(eBindings.Player_Right) || binds.ContainsKey(eBindings.Player_RightArrow))
                    {
                        m_Player.MoveRight();
                    }
                }
                else
                {
                    if (binds.ContainsKey(eBindings.Num_1))
                    {
                        HitNote(-1, -1);
                    }
                    if (binds.ContainsKey(eBindings.Num_2))
                    {
                        HitNote(0, -1);
                    }
                    if (binds.ContainsKey(eBindings.Num_3))
                    {
                        HitNote(1, -1);
                    }
                    if (binds.ContainsKey(eBindings.Num_4))
                    {
                        HitNote(-1, 0);
                    }
                    if (binds.ContainsKey(eBindings.Num_5))
                    {
                        HitNote(0, 0);
                    }
                    if (binds.ContainsKey(eBindings.Num_6))
                    {
                        HitNote(1, 0);
                    }
                    if (binds.ContainsKey(eBindings.Num_7))
                    {
                        HitNote(-1, 1);
                    }
                    if (binds.ContainsKey(eBindings.Num_8))
                    {
                        HitNote(0, 1);
                    }
                    if (binds.ContainsKey(eBindings.Num_9))
                    {
                        HitNote(1, 1);
                    }
                }
			}
		}

		public UI.UIScreen GetCurrentUI()
		{
			return m_UIStack.Peek();
		}

		public int UICount
		{
			get { return m_UIStack.Count; }
		}

		// Has to be here because only this can access stack!
		public void DrawUI(float fDeltaTime, SpriteBatch batch)
		{
			// We draw in reverse so the items at the TOP of the stack are drawn after those on the bottom
			foreach (UI.UIScreen u in m_UIStack.Reverse())
			{
				u.Draw(fDeltaTime, batch);
			}
		}

		// Pops the current UI
		public void PopUI()
		{
			m_UIStack.Peek().OnExit();
			m_UIStack.Pop();
		}

        public void ShowSongs()
        {
            m_UIStack.Push(new UI.UISongSelectionMenu(m_Game.Content, m_SongList, m_permissions)); 
        }

		public void ShowPauseMenu()
		{
            MediaPlayer.Pause();
			IsPaused = true;
			m_UIStack.Push(new UI.UIPauseMenu(m_Game.Content));
		}

        public void ShowNewScoreMenu()
        {
            m_UIStack.Push(new UI.UINewHighScore(m_Game.Content)); 
        }

        public void ShowOptionsMenu()
        {
            m_UIStack.Push(new UI.UIOptionsMenu(m_Game.Content));
        }

        public void ShowHighScores()
        {
            m_UIStack.Push(new UI.UISongSelectionMenu(m_Game.Content, m_SongList, m_permissions));
        }

        public void GetHighScores()
        {

            string filePath = Directory.GetCurrentDirectory() + "\\Content\\HighScores.txt";
            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                int s = 0;
                int size = 0; 

                line = reader.ReadLine();
                line = line.Remove(0, 6);
                size = int.Parse(line);

                for (int i = 0; i < size; i++)
                {
                    while (!(line.StartsWith("Title: ")))
                    {
                        line = reader.ReadLine();
                    }
                    line = line.Remove(0, 7);
                    foreach (SongList sl in m_SongList)
                    {
                        if (sl.m_title == line)
                        {
                            string name = reader.ReadLine();
                            string score; 
                            while (!(name.StartsWith("#")))
                            {
                                score = reader.ReadLine();
                                s = int.Parse(score);
                                sl.m_HighScorers.Add(new HighScorers(name, s)); 
                                name = reader.ReadLine();
                                if (name == null)
                                {
                                    break; 
                                }
                            }
                        }
                    }
                    
                }

            }
        }

        public bool CheckNewHighScore()
        {
            foreach (SongList s in m_SongList)
            {
                if (s.m_title == m_currentSong)
                {
                    for (int index = 0; index < s.m_HighScorers.Count; index++)
                    {
                        if (s.m_HighScorers[index].score < Score)
                        {
                            return true;
                        }
                    }
                }
            }
            return false; 
        }

        public void AddNewHighScore(string name)
        {
            foreach (SongList s in m_SongList)
            {
                if (s.m_title == m_currentSong)
                {
                    for (int index = 0; index < s.m_HighScorers.Count; index++)
                    {
                        if (s.m_HighScorers[index].score < Score)
                        {
                            s.m_HighScorers.Insert(index, new HighScorers(name, Score));
                            if (s.m_HighScorers.Count > HIGHSCORE_COUNT)
                            {
                                s.m_HighScorers.RemoveAt(s.m_HighScorers.Count - 1);
                            }
                            break;
                        }
                    }
                    break; 
                }

            }
            WriteHighScores(); 
        }

        public void WriteHighScores()
        {

            string filePath = Directory.GetCurrentDirectory() + "\\Content\\HighScores.txt";
            StreamWriter writer = new StreamWriter(filePath);
                writer.WriteLine("Size: " + m_SongList.Count);
                writer.WriteLine(""); 
                foreach (SongList sl in m_SongList)
                {
                    writer.WriteLine("#Song");
                    writer.WriteLine("Title: " + sl.m_title);
                    for (int i = 0; i < sl.m_HighScorers.Count; i++)
                    {
                        writer.WriteLine(sl.m_HighScorers[i].name);
                        writer.WriteLine(sl.m_HighScorers[i].score.ToString());
                    }
                }
                writer.Close(); 
        }

        public void CreateSongList()
        {
            string sourceDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Content/Sounds/Songs"); 
            var path = Directory.EnumerateFiles(sourceDirectory, "*.txt"); 
            foreach (string file in path)
            {
                using (StreamReader reader = new StreamReader(file))
                {
                    string title = null;
                    string artist = null;
                    string line;

                    line = reader.ReadLine();
                    while (title == null && artist == null && line != null)
                    {
                        if (line.StartsWith("Name:"))
                        {
                            title = line.Remove(0, 6);
                            line = reader.ReadLine(); 
                            artist = line.Remove(0, 8);
                        }
                        line = reader.ReadLine(); 
                    }

                    if (title != null && artist != null)
                    {
                        m_SongList.Add(new SongList(title, artist));
                    }
                    else
                    {
                        //Error message here! 
                    }
                }
                   
            }
        }

		public void Exit()
		{
			m_Game.Exit();
		}

        void GameOver(bool victorious)
        {
            IsPaused = true;
            m_UIStack.Push(new UI.UIGameOver(m_Game.Content, victorious));

        }

        public void RemoveBeatPlane(Objects.BeatPlane planeToDelete)
        {
            foreach (Objects.Note n in planeToDelete.ContainedNotes)
            {
                if (!n.Hit && !n.Bad)
                {
                    //Miss a good note
                    Health -= 1;
                    if (Health <= 0)
                    {
                        //FAILURE
                        MediaPlayer.Stop();
                        SetState(eGameState.MainMenu);
                    }
                    Combo = 0;
                    n.Miss = true;
                    MissCount++;
                    if (MissCount == 3)
                    {
                        Multiplier = MultMult;
                    }
                    TotalNotes++;
                }
            }

            while(planeToDelete.ContainedNotes.Count > 0)
            {
                RemoveGameObject(planeToDelete.ContainedNotes[0]);
                planeToDelete.ContainedNotes.RemoveAt(0);
            }
            m_BeatPlanes.Remove(planeToDelete);
            RemoveGameObject(planeToDelete);
        }
	}

    public class HighScorers
    {
        public string name;
        public int score;

        public HighScorers(string n, int s)
        {
            name = n;
            score = s;
        }

    }

    public class SongList
    {
        public string m_title;
        public string m_artist;
        public List<HighScorers> m_HighScorers = new List<HighScorers>();

        public SongList(string t, string a)
        {
            m_title = t;
            m_artist = a; 
        }
    }
}
