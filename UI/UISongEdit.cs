//-----------------------------------------------------------------------------
// UIMainMenu is the main menu UI.
//
// __Defense Sample for Game Programming Algorithms and Techniques
// Copyright (C) Sanjay Madhav. All rights reserved.
//
// Released under the Microsoft Permissive License.
// See LICENSE.txt for full details.
//-----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.IO;
using Microsoft.Xna.Framework.Input;

namespace itp380.UI
{
    public class UISongEdit : UIScreen
    {
        Objects.Song m_Song;
        Game m_Game;
        string m_FileName;
        float m_CurrentTime;

        SpriteFont m_TitleFont;
        SpriteFont m_ButtonFont;
        SpriteFont m_SubTitleFont;

        BeatSelectButton[] m_BeatTypeButtons;//Array for clickable buttons to determine type of beat pressed
        BeatSelectButton[] m_BeatSnapButtons;
        private int beatButtonWidth = 120;
        private int beatButtonHeight = 75;

        //Pixel dimensions of different areas on screen
        private int TotalTimeHeight;
        private int LocalTimeHeight;
        private int NoteSelectArea;
        private int LeftPanelWidth;
        private int TopPanelHeight;

        //Float percentages of different areas
        private float topBarFraction = 0.1f;
        //private float middleSection = 0.7f;
        private float localTimeFraction = 0.075f;
        private float totalTimeFraction = 0.05f;

        private float leftBarFraction = 0.2f;
        private float middleAreaFraction = 0.5f;
        //private float rightAreaFraction = 0.3f;

        //Buttons for access in writing out parsing file
        EditableTextField m_SongTitle, m_SongArtist, m_Appearancetime, m_BeatsPerMeasure, m_LeadIn, m_BeatsPerMinute, m_SongLength;
        EditableTextField[] m_TextFields;

        Rectangle NoteAreaRect, LocalTimeRect, TotalTimeRect, TopBar;
        Rectangle CurrentTimeSelected, LocalTimeSelected;

        int screenHeight;
        int screenWidth;

        private float m_CurrentBeat;
        private float m_BeatSnap;
        private string m_OriginalName;

        EditableTextField currentlyFocused = null;
        Button GTButton;

        Keys[] m_LastPressedKeys;

        private enum BeatSelectType
        {
            Good,
            GoodVariable,
            Bad,
            Shield,
            DoublePoints,
            Unselected
        }
        private BeatSelectType beatType;

        public UISongEdit(ContentManager Content, Game game, string filename = "") :
            base(Content)
        {
            m_Game = game;
            m_FileName = filename;
            //if (filename == "")
            //    m_FileName = "voyager.txt";

            if (m_FileName != "")
            {
                m_Song = ParseFile(m_FileName);
                m_OriginalName = m_FileName;
            }
            else
            {
                m_Song = new Objects.Song();
                m_Song.Name = "";
                m_Song.Artist = "";
                m_Song.AppearanceTime = 0.00f;
                m_Song.BeatsPerMeasure = 0;
                m_Song.BPM = 0.00f;
                m_Song.Difficulty = 10;
                m_Song.LeadIn = 0.00f;
                m_Song.Length = 0.00f;
            }

            KeyboardState kbState = Keyboard.GetState();
            m_LastPressedKeys = kbState.GetPressedKeys(); 

            RedoConstructor();
        }

        private void RedoConstructor()
        {
            m_Buttons.Clear();
            m_CurrentTime = 0.00f;
            m_CurrentBeat = 0.00f;

            m_Buttons.Clear();

            screenHeight = GraphicsManager.Get().Height;
            screenWidth = GraphicsManager.Get().Width;

            TotalTimeHeight = (int)(screenHeight * totalTimeFraction);
            LocalTimeHeight = (int)(screenHeight * localTimeFraction);
            NoteSelectArea = (int)(screenWidth * middleAreaFraction);
            LeftPanelWidth = (int)(screenWidth * leftBarFraction);
            TopPanelHeight = (int)(screenHeight * topBarFraction);

            //System.Diagnostics.Debug.WriteLine(screenHeight + ", " + screenWidth);

            //RECTANGLES
            NoteAreaRect = new Rectangle(LeftPanelWidth, TopPanelHeight, NoteSelectArea, NoteSelectArea);
            LocalTimeRect = new Rectangle(0, screenHeight - TotalTimeHeight - LocalTimeHeight, screenWidth, LocalTimeHeight);
            TotalTimeRect = new Rectangle(0, screenHeight - TotalTimeHeight, screenWidth, TotalTimeHeight);
            TopBar = new Rectangle(0, 0, screenWidth, TopPanelHeight);

            //Displayable time bar. Assumes That 8 measures are visible at all times
            //Console.WriteLine("Length: " + m_Song.Length + ", BPM: " + m_Song.BPM + ", BPMe: " + m_Song.BeatsPerMeasure);
            if (m_Song.Length != 0)
            {
                float barWidth = GraphicsManager.Get().Width;
                barWidth *= ((m_Song.BeatsPerMeasure * 8) / (m_Song.BPM / 60)) / m_Song.Length;
                //Console.WriteLine("Bar Width: " + (int)barWidth);
                CurrentTimeSelected = new Rectangle(0, TotalTimeRect.Y - 1, (int)barWidth, TotalTimeHeight);
            }
            else
            {
                CurrentTimeSelected = new Rectangle(0, TopPanelHeight + NoteSelectArea + LocalTimeHeight, GraphicsManager.Get().Width, TotalTimeHeight);
            }

            LocalTimeSelected = new Rectangle(0, screenHeight - TotalTimeHeight - LocalTimeHeight, 0, LocalTimeHeight - 1);

            m_TitleFont = m_Content.Load<SpriteFont>("SquaredDisplay");
            m_ButtonFont = m_Content.Load<SpriteFont>("SquaredDisplay_bold");
            m_SubTitleFont = m_Content.Load<SpriteFont>("imagine_bold");

            beatType = BeatSelectType.Unselected;

            // Create buttons
            Point buttonLocation = new Point(LeftPanelWidth + NoteSelectArea, 0);

            //TODO: Create Save and Reload buttons


            buttonLocation = new Point(1, 1);

            m_TextFields = new EditableTextField[7];
            //Song Title
            m_SongTitle = new EditableTextField(buttonLocation,
                "Title", m_TitleFont, Color.White, Color.Blue,
                LeftPanelWidth + NoteSelectArea, (int)TopPanelHeight / 2 - 1, TextFieldType.Text,
                FocusTitle, m_Song.Name);
            m_Buttons.AddLast(m_SongTitle);
            m_TextFields[0] = m_SongTitle;

            //Song Artist
            buttonLocation.Y += TopPanelHeight / 2;
            m_SongArtist = new EditableTextField(buttonLocation,
                "Artist", m_TitleFont, Color.White, Color.Blue,
                LeftPanelWidth + NoteSelectArea, (int)TopPanelHeight / 2 - 1, TextFieldType.Text,
                FocusArtist, m_Song.Artist);
            m_Buttons.AddLast(m_SongArtist);
            m_TextFields[1] = m_SongArtist;


            //Song BPM
            buttonLocation.Y = TopPanelHeight + 1;
            m_BeatsPerMinute = new EditableTextField(buttonLocation,
                "BPM", m_TitleFont, Color.White, Color.Blue,
                LeftPanelWidth, NoteSelectArea / 5 - 1, TextFieldType.Float,
                FocusBPM, m_Song.BPM.ToString(), 6);
            m_Buttons.AddLast(m_BeatsPerMinute);
            m_TextFields[2] = m_BeatsPerMinute;

            //Beats Per Measure
            buttonLocation.Y = TopPanelHeight + (NoteSelectArea / 5) + 1;
            m_BeatsPerMeasure = new EditableTextField(buttonLocation,
                "BPMeasure", m_TitleFont, Color.White, Color.Blue,
                LeftPanelWidth, NoteSelectArea / 5 - 1, TextFieldType.Int,
                FocusBPMe, m_Song.BeatsPerMeasure.ToString(), 2);
            m_Buttons.AddLast(m_BeatsPerMeasure);
            m_TextFields[3] = m_BeatsPerMeasure;

            //Song Length
            buttonLocation.Y = TopPanelHeight + (2 * NoteSelectArea / 5) + 1;
            m_SongLength = new EditableTextField(buttonLocation,
                "Song Length", m_TitleFont, Color.White, Color.Blue,
                LeftPanelWidth, NoteSelectArea / 5 - 1, TextFieldType.Float,
                FocusSongLength, m_Song.Length.ToString(), 8);
            m_Buttons.AddLast(m_SongLength);
            m_TextFields[4] = m_SongLength;

            //Song LeadIn
            buttonLocation.Y = TopPanelHeight + (3 * NoteSelectArea / 5) + 1;
            m_LeadIn = new EditableTextField(buttonLocation,
                "Song LeadIn", m_TitleFont, Color.White, Color.Blue,
                LeftPanelWidth, NoteSelectArea / 5 - 1, TextFieldType.Float,
                FocusLeadIn, m_Song.LeadIn.ToString(), 8);
            m_Buttons.AddLast(m_LeadIn);
            m_TextFields[5] = m_LeadIn;

            //Appearance Time
            buttonLocation.Y = TopPanelHeight + (4 * NoteSelectArea / 5) + 1;
            m_Appearancetime = new EditableTextField(buttonLocation,
                "Note LeadIn", m_TitleFont, Color.White, Color.Blue,
                LeftPanelWidth, NoteSelectArea / 5 - 1, TextFieldType.Float,
                FocusAT, m_Song.AppearanceTime.ToString(), 8);
            m_Buttons.AddLast(m_Appearancetime);
            m_TextFields[6] = m_Appearancetime;


            buttonLocation.X = LeftPanelWidth + NoteSelectArea + ((screenWidth - NoteSelectArea - LeftPanelWidth - beatButtonWidth * 2) - 30) / 2 + 10;
            buttonLocation.Y = TopPanelHeight + 240;
            //Beat select buttons
            m_BeatTypeButtons = new BeatSelectButton[4];
            //Generic positive button
            m_BeatTypeButtons[0] = new BeatSelectButton(buttonLocation, "Good", m_ButtonFont, Color.White, Color.Blue,
                beatButtonWidth, beatButtonHeight, ChangeBeatTypeGood);
            //Bad Button
            buttonLocation.X += beatButtonWidth + 10;
            m_BeatTypeButtons[1] = new BeatSelectButton(buttonLocation, "Bad", m_ButtonFont, Color.White, Color.Blue,
                beatButtonWidth, beatButtonHeight, ChangeBeatTypeBad);
            //Shield Powerup
            buttonLocation.X = LeftPanelWidth + NoteSelectArea + (screenWidth - NoteSelectArea - LeftPanelWidth - (int)(beatButtonWidth * 1.5)) / 2;
            buttonLocation.Y += 10 + beatButtonHeight;
            m_BeatTypeButtons[2] = new BeatSelectButton(buttonLocation, "Shield", m_ButtonFont, Color.White, Color.Blue,
               (int)(beatButtonWidth * 1.5), beatButtonHeight, ChangeBeatTypeShield);
            //Double Points Powerup
            buttonLocation.Y += beatButtonHeight + 10;
            m_BeatTypeButtons[3] = new BeatSelectButton(buttonLocation, "x2 Points", m_ButtonFont, Color.White, Color.Blue,
               (int)(beatButtonWidth * 1.5), beatButtonHeight, ChangeBeatTypeDoublePoints);
            //Entry Stuff for Number

            foreach (BeatSelectButton button in m_BeatTypeButtons)
            {
                m_Buttons.AddLast(button);
            }

            buttonLocation.X = LeftPanelWidth - 10;
            buttonLocation.Y = TopPanelHeight + NoteSelectArea + 5;

            m_BeatSnapButtons = new BeatSelectButton[5];
            //Whole Note
            m_BeatSnapButtons[0] = new BeatSelectButton(buttonLocation, "Whole", m_ButtonFont, Color.White, Color.Blue,
                beatButtonWidth, beatButtonHeight, ChangeSnapWhole);
            //Half Note
            buttonLocation.X += beatButtonWidth + 10;
            m_BeatSnapButtons[1] = new BeatSelectButton(buttonLocation, "Half", m_ButtonFont, Color.White, Color.Blue,
                beatButtonWidth, beatButtonHeight, ChangeSnapHalf);
            //Quarter
            buttonLocation.X += beatButtonWidth + 10;
            m_BeatSnapButtons[2] = new BeatSelectButton(buttonLocation, "Quarter", m_ButtonFont, Color.White, Color.Blue,
                (int)(beatButtonWidth * 1.5), beatButtonHeight, ChangeSnapQuarter);
            //Eighth
            buttonLocation.X += (int)(beatButtonWidth * 1.5) + 10;
            m_BeatSnapButtons[3] = new BeatSelectButton(buttonLocation, "Eighth", m_ButtonFont, Color.White, Color.Blue,
                (int)(beatButtonWidth * 1.5), beatButtonHeight, ChangeSnapEighth);
            //Sixteenth
            buttonLocation.X += (int)(beatButtonWidth * 1.5) + 10;
            m_BeatSnapButtons[4] = new BeatSelectButton(buttonLocation, "Sixteenth", m_ButtonFont, Color.White, Color.Blue,
               (int)(beatButtonWidth * 1.5), beatButtonHeight, ChangeSnapSixteenth);

            foreach (BeatSelectButton b in m_BeatSnapButtons)
            {
                m_Buttons.AddLast(b);
            }


            //Save and Exit Button
            buttonLocation.X = LeftPanelWidth + NoteSelectArea + ((screenWidth - LeftPanelWidth - NoteSelectArea - (int)m_SubTitleFont.MeasureString("Save/Exit").X) / 2);
            buttonLocation.Y = 10;
            Button SEButton = new Button(buttonLocation, "Save/Exit", m_SubTitleFont, Color.White, Color.Blue, SaveAndExit, eButtonAlign.Left);
            m_Buttons.AddLast(SEButton);

            //Save Button
            buttonLocation.X = LeftPanelWidth + NoteSelectArea + ((screenWidth - LeftPanelWidth - NoteSelectArea - (int)m_SubTitleFont.MeasureString("Save").X) / 2);
            buttonLocation.Y = 50;
            Button SButton = new Button(buttonLocation, "Save", m_SubTitleFont, Color.White, Color.Blue, Save, eButtonAlign.Left);
            m_Buttons.AddLast(SButton);

            //Load Button
            buttonLocation.X = LeftPanelWidth + NoteSelectArea + ((screenWidth - LeftPanelWidth - NoteSelectArea - (int)m_SubTitleFont.MeasureString("Reload").X) / 2);
            buttonLocation.Y = 90;
            Button LButton = new Button(buttonLocation, "Reload", m_SubTitleFont, Color.White, Color.Blue, Reload, eButtonAlign.Left);
            m_Buttons.AddLast(LButton);

            //GameType Button
            buttonLocation.X = LeftPanelWidth + NoteSelectArea + ((screenWidth - LeftPanelWidth - NoteSelectArea) / 2);
            buttonLocation.Y = 210;
            if (m_Song.DanceMode)
                GTButton = new Button(buttonLocation, "Dance Pad", m_SubTitleFont, Color.HotPink, Color.Cyan, ChangeGameType);
            else
                GTButton = new Button(buttonLocation, "Runner", m_SubTitleFont, Color.Lime, Color.Cyan, ChangeGameType);
            m_Buttons.AddLast(GTButton);

            ChangeSnapSixteenth();
            MoveLocalSlider(0);
        }

        public void ChangeGameType()
        {
            m_Song.DanceMode = !m_Song.DanceMode;
            if (m_Song.DanceMode)
            {
                GTButton.m_Text = "Dance Pad";
                GTButton.m_ColorDefault = Color.HotPink;
            }
            else
            {
                GTButton.m_Text = "Runner";
                GTButton.m_ColorDefault = Color.Lime;
            }
            
        }

        public void NewGame()
        {
        }

        public void Options()
        {
        }

        public override bool MouseClick(Point Position)
        {
            Position.Y = Position.Y * 768 / 750;
            bool bReturn = false;
            foreach (Button b in m_Buttons)
            {
                if (b.Enabled && b.m_Bounds.Contains(Position))
                {
                    b.Click();
                    bReturn = true;
                    break;
                }
            }

            if (NoteAreaRect.Contains(Position))
            {
                bReturn = true;
                ChangeFocus(null);
                //Call code for adding note
                AddNote(MouseLocationToNotePoint(Position), m_CurrentBeat, beatType);

            }
            else if(LocalTimeRect.Contains(Position))
            {
                bReturn = true;
                ChangeFocus(null);
                //Change the current beat
                MoveLocalSlider(Position.X);
            }
            else if (TotalTimeRect.Contains(Position))
            {
                bReturn = true;
                ChangeFocus(null);
                //Change the local time rect position
                MoveTimeSlider(Position.X);
            }
            return bReturn;
        }

        public override bool MouseClickRight(Point Position)
        {
            Position.Y = Position.Y * 768 / 750;
            if (NoteAreaRect.Contains(Position))
            {
                //Call code for remove note
                RemoveNote(MouseLocationToNotePoint(Position), m_CurrentBeat);
                return true;
            }
            return false;
        }

        public override bool MouseHold(Point Position)
        {
            Position.Y = Position.Y * 768 / 750;
            if (TotalTimeRect.Contains(Position))
            {
                ChangeFocus(null);
                //Change the local time rect position
                MoveTimeSlider(Position.X);
                return true;
            }
            else if (LocalTimeRect.Contains(Position))
            {
                ChangeFocus(null);
                //Change the local time rect position
                MoveLocalSlider(Position.X);
                return true;
            }
            return false;
        }

        public override bool MouseScroll(int Scroll)
        {
            MoveSlider(Scroll);
            return true;
        }

        public void Exit()
        {
            SoundManager.Get().PlaySoundCue("MenuClick");
            GameState.Get().Exit();
        }

        public override void Update(float fDeltaTime)
        {
            base.Update(fDeltaTime);
        }

        public override void Draw(float fDeltaTime, SpriteBatch DrawBatch)
        {
            CaptureKeyboard();

            //Draw background 

            //GraphicsManager.Get().DrawFilled(DrawBatch, TopBar, Color.AliceBlue, 1.0f, Color.Blue);
            GraphicsManager.Get().DrawFilled(DrawBatch, new Rectangle(0, 0, 1024, 768), Color.Black, 1.0f, Color.Black);

            DrawBatch.DrawString(m_SubTitleFont, "Beat Type", new Vector2(LeftPanelWidth + NoteSelectArea + 
                ((screenWidth - LeftPanelWidth - NoteSelectArea - m_SubTitleFont.MeasureString("Beat Type").X) / 2),
                TopPanelHeight + 180), Color.Cyan);

            DrawBatch.DrawString(m_SubTitleFont, "Snap", new Vector2(10, TopPanelHeight + NoteSelectArea + 20), Color.Cyan);
            DrawBatch.DrawString(m_SubTitleFont, "Time", new Vector2(80, TopPanelHeight + NoteSelectArea + 50), Color.Cyan);

            DrawBatch.DrawString(m_SubTitleFont, "Mode:", new Vector2(LeftPanelWidth + NoteSelectArea + 10, 167), Color.Cyan);

            //Local Time Bar
            GraphicsManager.Get().DrawFilled(DrawBatch, LocalTimeRect, Color.DarkBlue, 0.0f, Color.Black);
            //Total Time Bar
            GraphicsManager.Get().DrawFilled(DrawBatch, TotalTimeRect, Color.Blue, 0.0f, Color.Black);
            //Time Selection Bar
            GraphicsManager.Get().DrawFilled(DrawBatch, CurrentTimeSelected, Color.Red, 2.0f, Color.Black);

            //Test call for drawing sizes
            //GraphicsManager.Get().DrawFilled(DrawBatch, new Rectangle(400, 480, 48, 48), Color.AliceBlue, 0.00f, Color.Black);

            //Draw Grid
            GraphicsManager.Get().DrawLine(DrawBatch, 3.0f, Color.White,
                new Vector2(LeftPanelWidth, (NoteSelectArea / 3) + TopPanelHeight),
                new Vector2(NoteSelectArea + LeftPanelWidth, (NoteSelectArea / 3) + TopPanelHeight));
            GraphicsManager.Get().DrawLine(DrawBatch, 3.0f, Color.White,
                new Vector2(LeftPanelWidth, (2 * NoteSelectArea / 3) + TopPanelHeight),
                new Vector2(NoteAreaRect.Width + LeftPanelWidth, (2 * NoteSelectArea / 3) + TopPanelHeight));
            GraphicsManager.Get().DrawLine(DrawBatch, 3.0f, Color.White,
                new Vector2((NoteSelectArea / 3) + LeftPanelWidth, TopPanelHeight),
                new Vector2((NoteSelectArea / 3) + LeftPanelWidth, NoteSelectArea + TopPanelHeight));
            GraphicsManager.Get().DrawLine(DrawBatch, 3.0f, Color.White,
                new Vector2((2 * NoteSelectArea / 3) + LeftPanelWidth, TopPanelHeight),
                new Vector2((2 * NoteSelectArea / 3) + LeftPanelWidth, NoteSelectArea + TopPanelHeight));

            //Draw the lines for the local time bar
            //Find nearest beat
            if (m_Song.BeatsPerMeasure != 0 && m_Song.BPM != 0)
            {
                float TimePerSixteenth = (60 / m_Song.BPM) / 4;
                float offset = m_CurrentTime % TimePerSixteenth;
                float firstBeatIndex = ((float)((int)(m_CurrentTime / TimePerSixteenth))) / 4;
                float widthBetweenBeat = screenWidth / (m_Song.BeatsPerMeasure * 8);
                offset *= widthBetweenBeat;

                Color beatColor = Color.DarkViolet;
                Color noBeatColor = Color.Azure;

                //Draw local time slider
                GraphicsManager.Get().DrawFilled(DrawBatch, LocalTimeSelected, Color.Cyan, 1.0f, Color.Black);

                //Draw beat lines
                for (float i = 0; i < 8 * m_Song.BeatsPerMeasure; i += 0.25f)
                {
                    //Decide on the type of beat from most important down
                    if ((firstBeatIndex + i) % (m_Song.BeatsPerMeasure) == 0)//It's the beginning of a measure
                    {
                        if (m_Song.Notes.ContainsKey(firstBeatIndex + i))//Draw measure line with beat color
                        {
                            GraphicsManager.Get().DrawLine(DrawBatch, 2.0f, beatColor,
                               new Vector2((int)(offset + i * widthBetweenBeat), screenHeight - TotalTimeHeight - 1),
                               new Vector2((int)(offset + i * widthBetweenBeat), screenHeight - TotalTimeHeight - 37));
                        }
                        else //Draw it the no beat color
                        {
                            GraphicsManager.Get().DrawLine(DrawBatch, 2.0f, noBeatColor,
                               new Vector2((int)(offset + i * widthBetweenBeat), screenHeight - TotalTimeHeight - 1),
                               new Vector2((int)(offset + i * widthBetweenBeat), screenHeight - TotalTimeHeight - 37));
                        }
                    }
                    else if ((firstBeatIndex + i) % 1 == 0)//It's the beginning of a beat
                    {
                        if (m_Song.Notes.ContainsKey(firstBeatIndex + i))//Draw beat line with beat color
                        {
                            GraphicsManager.Get().DrawLine(DrawBatch, 2.0f, beatColor,
                               new Vector2((int)(offset + i * widthBetweenBeat), screenHeight - TotalTimeHeight - 1),
                               new Vector2((int)(offset + i * widthBetweenBeat), screenHeight - TotalTimeHeight - 25));
                        }
                        else //Draw it the no beat color
                        {
                            GraphicsManager.Get().DrawLine(DrawBatch, 2.0f, noBeatColor,
                               new Vector2((int)(offset + i * widthBetweenBeat), screenHeight - TotalTimeHeight - 1),
                               new Vector2((int)(offset + i * widthBetweenBeat), screenHeight - TotalTimeHeight - 25));
                        }
                    }
                    else if ((firstBeatIndex + i) % 0.50f == 0)//It's an eighth note
                    {
                        if (m_Song.Notes.ContainsKey(firstBeatIndex + i))//Draw beat line with beat color
                        {
                            GraphicsManager.Get().DrawLine(DrawBatch, 2.0f, beatColor,
                               new Vector2((int)(offset + i * widthBetweenBeat), screenHeight - TotalTimeHeight - 1),
                               new Vector2((int)(offset + i * widthBetweenBeat), screenHeight - TotalTimeHeight - 13));
                        }
                        else //Draw it the no beat color
                        {
                            GraphicsManager.Get().DrawLine(DrawBatch, 2.0f, noBeatColor,
                               new Vector2((int)(offset + i * widthBetweenBeat), screenHeight - TotalTimeHeight - 1),
                               new Vector2((int)(offset + i * widthBetweenBeat), screenHeight - TotalTimeHeight - 13));
                        }
                    }
                    else if ((firstBeatIndex + i) % 0.25f == 0)//It's a sixteenth note
                    {
                        if (m_Song.Notes.ContainsKey(firstBeatIndex + i))//Draw beat line with beat color
                        {
                            GraphicsManager.Get().DrawLine(DrawBatch, 2.0f, beatColor,
                               new Vector2((int)(offset + i * widthBetweenBeat), screenHeight - TotalTimeHeight - 1),
                               new Vector2((int)(offset + i * widthBetweenBeat), screenHeight - TotalTimeHeight - 7));
                        }
                        else //Draw it the no beat color
                        {
                            GraphicsManager.Get().DrawLine(DrawBatch, 2.0f, noBeatColor,
                               new Vector2((int)(offset + i * widthBetweenBeat), screenHeight - TotalTimeHeight - 1),
                               new Vector2((int)(offset + i * widthBetweenBeat), screenHeight - TotalTimeHeight - 7));
                        }
                    }
                }

                //Draw the beats on the current time
                if(m_Song.Notes.Contains(m_CurrentBeat))
                {
                    if (m_Song.Notes[m_CurrentBeat] is List<Objects.Note>)
                    {
                        List<Objects.Note> notes = (List<Objects.Note>)m_Song.Notes[m_CurrentBeat];
                        //Console.WriteLine(notes.Count);
                        foreach (Objects.Note note in notes)
                        {
                            Rectangle noteRect = new Rectangle((note.Location.X + 1) * (NoteSelectArea / 3) + LeftPanelWidth + 10,
                                (note.Location.Y * -1 + 1) * (NoteSelectArea / 3) + TopPanelHeight + 10,
                                NoteSelectArea / 3 - 20, NoteSelectArea / 3 - 20);
                            //Select the color
                            Color noteColor = new Color();
                            if (note.Bad)
                                noteColor = Color.Red;
                            else
                            {
                                switch (note.NotePowerup)
                                {
                                    case Objects.Note.Powerup.None:
                                        noteColor = Color.White;
                                        break;

                                    case Objects.Note.Powerup.DoubleMult:
                                        noteColor = Color.Yellow;
                                        break;

                                    case Objects.Note.Powerup.Shield:
                                        noteColor = Color.Blue;
                                        break;
                                }
                            }

                            //Draw the notes
                            GraphicsManager.Get().DrawFilled(DrawBatch, noteRect, Color.Black, 5.0f, noteColor);
                        }
                    }
                }
            }

            base.Draw(fDeltaTime, DrawBatch);
        }

        private void ChangeBeatTypeGood()
        {
            beatType = BeatSelectType.Good;
            foreach (BeatSelectButton b in m_BeatTypeButtons)
            {
                b.IsSelected = false;
            }
            m_BeatTypeButtons[0].IsSelected = true;
        }
        private void ChangeBeatTypeBad()
        {
            beatType = BeatSelectType.Bad;
            foreach (BeatSelectButton b in m_BeatTypeButtons)
            {
                b.IsSelected = false;
            }
            m_BeatTypeButtons[1].IsSelected = true;
        }
        private void ChangeBeatTypeShield()
        {
            beatType = BeatSelectType.Shield;
            foreach (BeatSelectButton b in m_BeatTypeButtons)
            {
                b.IsSelected = false;
            }
            m_BeatTypeButtons[2].IsSelected = true;
        }
        private void ChangeBeatTypeDoublePoints()
        {
            beatType = BeatSelectType.DoublePoints;
            foreach (BeatSelectButton b in m_BeatTypeButtons)
            {
                b.IsSelected = false;
            }
            m_BeatTypeButtons[3].IsSelected = true;
        }

        private void CaptureKeyboard()
        {
            if (currentlyFocused != null)
                return;
            KeyboardState kbState = Keyboard.GetState();
            Keys[] pressedKeys = kbState.GetPressedKeys();

            bool shift = (pressedKeys.Contains(Keys.LeftShift) || pressedKeys.Contains(Keys.RightShift));

            foreach (Keys key in pressedKeys)
            {
                if (!m_LastPressedKeys.Contains(key))
                    OnKeyDown(key);
            }
            m_LastPressedKeys = pressedKeys;
        }

        private void OnKeyDown(Keys key)
        {
            switch (key)
            {
                case Keys.Q:
                    MoveSlider(120);
                    break;

                case Keys.E:
                    MoveSlider(-120);
                    break;

                case Keys.A:
                    ChangeBeatTypeGood();
                    break;

                case Keys.S:
                    ChangeBeatTypeBad();
                    break;

                case Keys.D:
                    ChangeBeatTypeDoublePoints();
                    break;

                case Keys.F:
                    ChangeBeatTypeShield();
                    break;

                case Keys.D1:
                    ChangeSnapWhole();
                    break;

                case Keys.D2:
                    ChangeSnapHalf();
                    break;

                case Keys.D3:
                    ChangeSnapQuarter();
                    break;

                case Keys.D4:
                    ChangeSnapEighth();
                    break;

                case Keys.D5:
                    ChangeSnapSixteenth();
                    break;
            }
        }

        private void ChangeSnapWhole()
        {
            m_BeatSnap = m_Song.BeatsPerMeasure;
            MoveLocalSlider(LocalTimeSelected.X);
            foreach (BeatSelectButton b in m_BeatSnapButtons)
            {
                b.IsSelected = false;
            }
            m_BeatSnapButtons[0].IsSelected = true;
        }
        private void ChangeSnapHalf()
        {
            m_BeatSnap = 2;
            MoveLocalSlider(LocalTimeSelected.X);
            foreach (BeatSelectButton b in m_BeatSnapButtons)
            {
                b.IsSelected = false;
            }
            m_BeatSnapButtons[1].IsSelected = true;
        }
        private void ChangeSnapQuarter()
        {
            m_BeatSnap = 1;
            MoveLocalSlider(LocalTimeSelected.X);
            foreach (BeatSelectButton b in m_BeatSnapButtons)
            {
                b.IsSelected = false;
            }
            m_BeatSnapButtons[2].IsSelected = true;
        }
        private void ChangeSnapEighth()
        {
            m_BeatSnap = 0.50f;
            MoveLocalSlider(LocalTimeSelected.X);
            foreach (BeatSelectButton b in m_BeatSnapButtons)
            {
                b.IsSelected = false;
            }
            m_BeatSnapButtons[3].IsSelected = true;
        }
        private void ChangeSnapSixteenth()
        {
            m_BeatSnap = 0.25f;
            MoveLocalSlider(LocalTimeSelected.X+1);
            foreach (BeatSelectButton b in m_BeatSnapButtons)
            {
                b.IsSelected = false;
            }
            m_BeatSnapButtons[4].IsSelected = true;
        }


        private void FocusTitle() { ChangeFocus(m_SongTitle); }
        private void FocusArtist() { ChangeFocus(m_SongArtist); }
        private void FocusBPM() { ChangeFocus(m_BeatsPerMinute); }
        private void FocusBPMe() { ChangeFocus(m_BeatsPerMeasure); }
        private void FocusLeadIn() { ChangeFocus(m_LeadIn); }
        private void FocusAT() { ChangeFocus(m_Appearancetime); }
        private void FocusSongLength() { ChangeFocus(m_SongLength); }

        private void ChangeFocus(EditableTextField textField)
        {
            foreach (EditableTextField TextField in m_TextFields)
            {
                TextField.IsSelected = false;
            }
            if(textField != null)
                textField.IsSelected = true;

            currentlyFocused = textField;

            ResetTimeSliders();
        }

        private void ResetTimeSliders()
        {
            m_Song.Length = m_SongLength.ReturnFloat();
            m_Song.BPM = m_BeatsPerMinute.ReturnFloat();
            m_Song.BeatsPerMeasure = m_BeatsPerMeasure.ReturnInt();
            m_Song.AppearanceTime = m_Appearancetime.ReturnFloat();
            m_Song.LeadIn = m_LeadIn.ReturnFloat();
            //Console.WriteLine("Length: " + m_Song.Length + ", BPM: " + m_Song.BPM + ", BPMe: " + m_Song.BeatsPerMeasure);
            /*
            if (m_Song.Length != 0)
            {
                float barWidth = GraphicsManager.Get().Width;
                barWidth *= ((m_Song.BeatsPerMeasure * 8) / (m_Song.BPM / 60)) / m_Song.Length;
                //Console.WriteLine("Bar Width: " + (int)barWidth);
                CurrentTimeSelected = new Rectangle(0, TotalTimeRect.Y, (int)barWidth, TotalTimeHeight);
            }
            else
            {
                CurrentTimeSelected = new Rectangle(m_CurrentTime, TopPanelHeight + NoteSelectArea + LocalTimeHeight, GraphicsManager.Get().Width, TotalTimeHeight);
            }
            */
        }

        private void MoveTimeSlider(int Location)
        {
            CurrentTimeSelected.X = Location - CurrentTimeSelected.Width / 2;
            if (CurrentTimeSelected.X < 0)
                CurrentTimeSelected.X = 0;
            if (CurrentTimeSelected.X > GraphicsManager.Get().Width - CurrentTimeSelected.Width)
                CurrentTimeSelected.X = GraphicsManager.Get().Width - CurrentTimeSelected.Width;

            //TODO: Update local time bar
            m_CurrentTime = ((float)CurrentTimeSelected.X / (float)GraphicsManager.Get().Width) * m_Song.Length;
            MoveLocalSlider(0);
        }

        private void MoveLocalSlider(int Location)
        {
            float beatFinder = m_CurrentTime;
            float displayedTime = (float)(m_Song.BeatsPerMeasure * 8) / (m_Song.BPM / 60);
            beatFinder += ((float)Location / (float)screenWidth) * displayedTime;//Get exact time of click
            //Console.WriteLine("Current Time: " + beatFinder);
            beatFinder *= (m_Song.BPM / 60.000f);//Find the exact beat clicked
            //Shenanigans to round to the nearest 0.5 of a beat
            //Console.WriteLine("Before Shenanigans: " + beatFinder);
            beatFinder = ((float)((int)(beatFinder / m_BeatSnap))) * m_BeatSnap;
            m_CurrentBeat = beatFinder;
            //Console.WriteLine("Current beat: " + m_CurrentBeat);

            float TimePerSixteenth = (60 / m_Song.BPM) / 4;
            float offset = m_CurrentTime % TimePerSixteenth;
            float firstBeatIndex = ((float)((int)(m_CurrentTime / TimePerSixteenth))) / 4;
            float widthBetweenBeat = screenWidth / (m_Song.BeatsPerMeasure * 8);
            offset *= widthBetweenBeat;

            int i = 0;
            if (m_BeatSnap != 0.25f)
            {
                float sixteenthRounder = m_CurrentTime;
                sixteenthRounder *= (m_Song.BPM / 60.000f);

                if (sixteenthRounder % 0.25f != 0)
                {
                    sixteenthRounder = ((float)((int)(sixteenthRounder / 0.25f))) * 0.25f;
                }

                //Console.WriteLine("beat: " + sixteenthRounder);
                //Console.WriteLine("beat snap: " + m_BeatSnap);
                while (sixteenthRounder % m_BeatSnap != 0)
                {
                    sixteenthRounder += 0.25f;
                    i++;
                }
                //Console.WriteLine(i);
            }

            int beatOffset = (int)(((int)(((float)Location - offset) / (widthBetweenBeat * m_BeatSnap))) * (widthBetweenBeat * m_BeatSnap));
            beatOffset += (int)(i * widthBetweenBeat / 4);

            LocalTimeSelected = new Rectangle((int)(offset + beatOffset), LocalTimeSelected.Y,
                (int)(widthBetweenBeat * m_BeatSnap), LocalTimeHeight - 1);
            //Console.WriteLine(offset + beatOffset);
        }

        private void MoveSlider(int dir)
        {
            if(m_Song.BPM != 0 && m_Song.Length != 0)
            {
                dir /= -120;
                m_CurrentBeat += dir * m_BeatSnap;
                if (m_CurrentBeat < 0)
                {
                    m_CurrentBeat -= dir * m_BeatSnap;
                    return;
                }

                if (m_CurrentBeat > m_Song.Length * (m_Song.BPM / 60))
                {
                    m_CurrentBeat -= dir * m_BeatSnap;
                    return;
                }

                float TimePerSixteenth = (60 / m_Song.BPM) / 4;
                float offset = m_CurrentTime % TimePerSixteenth;
                float firstBeatIndex = ((float)((int)(m_CurrentTime / TimePerSixteenth))) / 4;
                float widthBetweenBeat = screenWidth / (m_Song.BeatsPerMeasure * 8);
                offset *= widthBetweenBeat;

                LocalTimeSelected.X += (int)(widthBetweenBeat * m_BeatSnap * dir);
                
                if (LocalTimeSelected.X < 0)
                {
                    m_CurrentTime -= m_BeatSnap * (60 / m_Song.BPM);
                    CurrentTimeSelected.X -= (int)(m_BeatSnap / m_Song.Length);
                    LocalTimeSelected.X -= (int)(widthBetweenBeat * m_BeatSnap * dir);
                }
                else if (LocalTimeSelected.X > screenWidth - LocalTimeSelected.Width)
                {
                    m_CurrentTime += m_BeatSnap * (60 / m_Song.BPM);
                    CurrentTimeSelected.X += (int)(m_BeatSnap / m_Song.Length);
                    LocalTimeSelected.X -= (int)(widthBetweenBeat * m_BeatSnap * dir);
                }
            }
        }

        private Point MouseLocationToNotePoint(Point MouseLoc)
        {
            Point NotePoint = MouseLoc;
            NotePoint.X -= LeftPanelWidth;
            NotePoint.Y -= TopPanelHeight;
            NotePoint.X /= (NoteSelectArea / 3);
            NotePoint.Y /= (NoteSelectArea / 3);
            NotePoint.X -= 1;
            NotePoint.Y -= 1;
            NotePoint.Y *= -1;
            return NotePoint;
        }

        private void AddNote(Point location, float beat, BeatSelectType beatType)
        {
            if (beatType == BeatSelectType.Unselected)
                return;
            if (m_Song.Notes.Contains(m_CurrentBeat))
            {
                if (m_Song.Notes[m_CurrentBeat] is List<Objects.Note>)
                {
                    List<Objects.Note> notes = (List<Objects.Note>)m_Song.Notes[m_CurrentBeat];
                    foreach (Objects.Note note in notes)
                    {
                        if (note.Location.X == location.X && note.Location.Y == location.Y)
                        {
                            return;
                        }
                    }
                    //If made it through the for loop, we can add a note to the list
                    Objects.Note newNote = new Objects.Note(m_Game);
                    newNote.Location = location;
                    newNote.Beat = m_CurrentBeat;
                    switch (beatType)
                    {
                        case BeatSelectType.Good:
                            newNote.Bad = false;
                            newNote.NotePowerup = Objects.Note.Powerup.None;
                            newNote.Value = 10;
                            break;

                        case BeatSelectType.Bad:
                            newNote.Bad = true;
                            newNote.NotePowerup = Objects.Note.Powerup.None;
                            newNote.Value = 10;
                            break;

                        case BeatSelectType.DoublePoints:
                            newNote.Bad = false;
                            newNote.NotePowerup = Objects.Note.Powerup.DoubleMult;
                            newNote.Value = 0;
                            break;

                        case BeatSelectType.Shield:
                            newNote.Bad = false;
                            newNote.NotePowerup = Objects.Note.Powerup.Shield;
                            newNote.Value = 0;
                            break;
                    }
                    notes.Add(newNote);
                }
            }
            else//No entry at this key. Make one
            {
                List<Objects.Note> newNotes = new List<Objects.Note>();
                Objects.Note newNote = new Objects.Note(m_Game);
                newNote.Location = location;
                newNote.Beat = m_CurrentBeat;
                switch (beatType)
                {
                    case BeatSelectType.Good:
                        newNote.Bad = false;
                        newNote.NotePowerup = Objects.Note.Powerup.None;
                        newNote.Value = 10;
                        break;

                    case BeatSelectType.Bad:
                        newNote.Bad = true;
                        newNote.NotePowerup = Objects.Note.Powerup.None;
                        newNote.Value = 10;
                        break;

                    case BeatSelectType.DoublePoints:
                        newNote.Bad = false;
                        newNote.NotePowerup = Objects.Note.Powerup.DoubleMult;
                        newNote.Value = 0;
                        break;

                    case BeatSelectType.Shield:
                        newNote.Bad = false;
                        newNote.NotePowerup = Objects.Note.Powerup.Shield;
                        newNote.Value = 0;
                        break;
                }
                newNotes.Add(newNote);
                m_Song.Notes.Add(m_CurrentBeat, newNotes);
            }
        }

        private void RemoveNote(Point location, float beat)
        {
            if (m_Song.Notes.Contains(m_CurrentBeat))
            {
                if (m_Song.Notes[m_CurrentBeat] is List<Objects.Note>)
                {
                    List<Objects.Note> notes = (List<Objects.Note>)m_Song.Notes[m_CurrentBeat];
                    foreach (Objects.Note note in notes)
                    {
                        if (note.Location.X == location.X && note.Location.Y == location.Y)
                        {
                            notes.Remove(note);
                            if (notes.Count == 0)
                            {
                                m_Song.Notes.Remove(m_CurrentBeat);
                            }
                            break;
                        }
                    }
                }
            }
        }

        private void SaveAndExit()
        {
            ChangeFocus(null);
            WriteFile(m_Song);
            GameState.Get().SetState(eGameState.MainMenu);
            SoundManager.Get().PlaySoundCue("MenuClick");
        }

        private void Save()
        {
            ChangeFocus(null);
            WriteFile(m_Song);
        }

        private void Reload()
        {
            ChangeFocus(null);
            if (m_OriginalName != "")
            {
                m_Song = ParseFile(m_OriginalName);
            }
            else
            {
                m_Song = new Objects.Song();
                m_Song.Name = "";
                m_Song.Artist = "";
                m_Song.AppearanceTime = 0.00f;
                m_Song.BeatsPerMeasure = 0;
                m_Song.BPM = 0.00f;
                m_Song.Difficulty = 10;
                m_Song.LeadIn = 0.00f;
                m_Song.Length = 0.00f;
            }

            RedoConstructor();
        }

        private void WriteFile (Objects.Song song)
        {
            if (song.Name == "" || song.BPM == 0 || song.BeatsPerMeasure == 0 || song.Length == 0)
                return;

            Console.WriteLine("Writing out: " + song.Name);
            string filePath = Directory.GetCurrentDirectory() +  "\\Content\\Sounds\\Songs\\" + song.Name + ".txt";
            Console.WriteLine(filePath);
            StreamWriter writer = new StreamWriter(filePath);

            //General Info
            writer.WriteLine("#General");
            writer.WriteLine("Name: " + song.Name);
            writer.WriteLine("Artist: " + song.Artist);
            if (song.DanceMode)
                writer.WriteLine("Mode: Dance");
            else
                writer.WriteLine("Mode: Runner");
            writer.WriteLine("Difficulty: " + song.Difficulty);
            writer.WriteLine("");

            //Song Info
            writer.WriteLine("BPM: " + song.BPM.ToString());
            writer.WriteLine("SongLength: " + song.Length.ToString());   
            writer.WriteLine("LeadIn: " + song.LeadIn.ToString());
            writer.WriteLine("BeatsPerMeasure: " + song.BeatsPerMeasure.ToString());
            writer.WriteLine("AppearanceTime: " + song.AppearanceTime.ToString());
            writer.WriteLine("");

            //Game Elements
            writer.WriteLine("#GameElements");
            foreach(Object objectList in song.Notes.Values)
            {
                List<Objects.Note> noteList = (List<Objects.Note>)objectList;
                foreach (Objects.Note note in noteList)
                {
                    string noteInfo = note.Beat.ToString() + ", ";
                    if (note.Bad)
                    {
                        noteInfo += "Bad, 10, ";
                    }
                    else
                    {
                        if (note.NotePowerup == Objects.Note.Powerup.DoubleMult)
                            noteInfo += "DoubleMult, ";
                        else if (note.NotePowerup == Objects.Note.Powerup.Shield)
                            noteInfo += "Shield, ";
                        else if (note.NotePowerup == Objects.Note.Powerup.None)
                        {
                            noteInfo += "Good, 10, ";
                        }
                    }
                    noteInfo += PointToPositionString(note.Location);
                    writer.WriteLine(noteInfo);
                }
            }
            writer.Close();
        }

        private string PointToPositionString(Point location)
        {
            string PositionString = "";
            int positionInt = 1;
            positionInt += (location.Y + 1) * 3;
            positionInt += (location.X + 1);
            PositionString = positionInt.ToString();
            return PositionString;
        }

        private Objects.Song ParseFile(string filename)//Parser taken from GameState with some minor changes
        {
            //Console.WriteLine(filename);
            Objects.Song song = new Objects.Song();
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Content\\Sounds\\Songs", filename);
            //Console.WriteLine(path);
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
                        note.Bad = true;
                        note.NotePowerup = Objects.Note.Powerup.None;
                    }
                    else
                    {
                        note.Bad = false;
                        if (type.Equals("DoubleMult"))
                        {
                            note.NotePowerup = Objects.Note.Powerup.DoubleMult;
                        }
                        else if (type.Equals("Shield"))
                        {
                            note.NotePowerup = Objects.Note.Powerup.Shield;
                        }
                        else
                        {
                            note.NotePowerup = Objects.Note.Powerup.None;
                        }
                    }
                    if (type.Equals("Good") || type.Equals("Bad"))
                    {
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
            sr.Close();
            return song;
        }
    }
}
