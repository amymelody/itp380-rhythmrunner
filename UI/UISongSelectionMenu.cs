//-----------------------------------------------------------------------------
// Song Selection Menu

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

namespace itp380.UI
{

    public class UISongSelectionMenu : UIScreen
    {
        SpriteFont m_TitleFont;
        SpriteFont m_ButtonFont;
        SpriteFont m_ScoreFont; 
        ePermissions m_mode; 
        string m_OptionsText;
        string m_SongClicked;
        int m_listStart;
        int m_listEnd;

        private List<SongList> m_SongList = new List<SongList>();
        private List<HighScorers> m_HighScorers = new List<HighScorers>(); 

        public UISongSelectionMenu(ContentManager Content, List<SongList> sl, ePermissions p) :
            base(Content)
        {
            m_bCanExit = true;

            m_TitleFont = m_Content.Load<SpriteFont>("SpaceAge");
            m_ButtonFont = m_Content.Load<SpriteFont>("SquaredDisplay_bold");
            m_ScoreFont = m_Content.Load<SpriteFont>("imagine"); 

            m_mode = p;
            if (m_mode == ePermissions.SongEdit)
            {
                m_OptionsText = "Song Edit"; 
            }
            else if (m_mode == ePermissions.HighScores)
            {
                m_OptionsText = "High Scores"; 
            }
            else
            {
                m_OptionsText = "Song Selection";
            }
            
            // Create buttons
            Point vPos = new Point();
            //vPos.X = (int)(GraphicsManager.Get().Width / 5.0f);
            vPos.X = (int)(GraphicsManager.Get().Width/2.0f); 
            vPos.Y = (int)(GraphicsManager.Get().Height / 3.5f);
            
            vPos.Y += 400;
            m_Buttons.AddLast(new Button(vPos, "Back to Menu",
                m_ButtonFont, Color.White,
                Color.White, Quit, eButtonAlign.Center));

            if (m_mode == ePermissions.HighScores)
            {
                vPos.X = 150;
            }
            else
            {
                vPos.X = 350; 
            }
            vPos.Y -= 350;
            m_Buttons.AddLast(new Button(vPos, "Previous",
                m_ButtonFont, Color.DarkTurquoise,
                Color.White, ShowPreviousSongs, eButtonAlign.Center));

            vPos.Y += 300;
            if (m_mode == ePermissions.HighScores)
            {
                vPos.X = GraphicsManager.Get().Width - 550;
            }
            else
            {
                vPos.X = GraphicsManager.Get().Width - 350;
            }

            m_Buttons.AddLast(new Button(vPos, "Next",
                m_ButtonFont, Color.DarkTurquoise,
                Color.White, ShowNextSongs, eButtonAlign.Center));
             

            SoundManager.Get().PlaySoundCue("MenuClick");

            m_SongList = sl;
            if (m_mode == ePermissions.HighScores)
            {
                m_SongClicked = m_SongList[0].m_title; 
            }
            DrawHighScores(); 
            ShowNextSongs(); 
        }

        public void Quit()
        {
            GameState.Get().SetState(eGameState.MainMenu);
            SoundManager.Get().PlaySoundCue("MenuClick");
        }

        public void ShowNextSongs()
        {
            Point vPos = new Point();
            vPos.Y = (int)(GraphicsManager.Get().Height / 3.5f) + 50;
            vPos.X = GraphicsManager.Get().Width / 2;
            if (m_mode == ePermissions.HighScores)
            {
                vPos.X -= 200; 
            }

            Action<string> messageTarget = delegate(string s)
            { ClickedSong(s); };

            m_SongButtons.Clear();

            if (m_SongList.Count < 5)
            {
                m_listStart = 0;
                m_listEnd = m_SongList.Count - 1;
            }
            else
            {
                if (m_listStart + 5 >= m_SongList.Count)
                {
                    m_listStart = 0;
                    m_listEnd = m_SongList.Count - 1;
                }
                else
                {
                    if (m_listEnd + 5 >= m_SongList.Count)
                    {
                        int x = m_SongList.Count % 5;
                        m_listStart = m_SongList.Count - (x + 1);
                        m_listEnd = m_SongList.Count - 1; 
                    }
                    else
                    {
                        m_listStart += 5;
                        m_listEnd += 5; 
                    }
                }
            }

            for (int i = m_listStart; i <= m_listEnd; i++)
            {
                vPos.Y += 50;
                m_SongButtons.AddLast(new SongButton(vPos, m_SongList[i].m_title,
                    m_ButtonFont, Color.HotPink,
                    Color.White, messageTarget, m_SongList[i].m_title, eButtonAlign.Center));
            }
        }

        public void ShowPreviousSongs()
        {
            m_SongButtons.Clear();

            Point vPos = new Point();
            vPos.Y = (int)(GraphicsManager.Get().Height / 3.5f) + 50;
            vPos.X = GraphicsManager.Get().Width / 2;
            if (m_mode == ePermissions.HighScores)
            {
                vPos.X -= 200;
            }

            if (m_SongList.Count >= 5)
            {
                if (m_listStart - 5 == -5)
                {
                    int x = m_SongList.Count % 5;
                    if (x == 0)
                    {
                        m_listStart = m_SongList.Count - 6;
                        m_listEnd = m_SongList.Count - 1;
                    }
                    else
                    {
                        m_listStart = m_SongList.Count - (x + 1);
                        m_listEnd = m_SongList.Count - 1;
                    }
                }
                else
                {
                    m_listStart -= 5;
                    m_listEnd -= 5;
                }
            }
            else
            {
                m_listStart = 0;
                m_listEnd = m_SongList.Count - 1; 
            }

            Action<string> messageTarget = delegate(string s)
            { ClickedSong(s); };

            for (int i = m_listStart; i <= m_listEnd; i++)
            {
                vPos.Y += 50;
                m_SongButtons.AddLast(new SongButton(vPos, m_SongList[i].m_title,
                    m_ButtonFont, Color.HotPink,
                    Color.White, messageTarget, m_SongList[i].m_title, eButtonAlign.Center));
            }
        }

        public void ClickedSong(string name)
        {
            m_SongClicked = name;
            if (m_mode == ePermissions.Normal)
            {
                GameState.Get().SongFile = "Sounds/Songs/" + name + ".txt";
                GameState.Get().SetState(eGameState.Gameplay);
                SoundManager.Get().PlaySoundCue("MenuClick");
                GameState.Get().m_currentSong = m_SongClicked; 
            }
            else if (m_mode == ePermissions.SongEdit)
            {
                GameState.Get().SongFile = name + ".txt";
                GameState.Get().SetState(eGameState.Edit);
                SoundManager.Get().PlaySoundCue("MenuClick");
            }
            else if (m_mode == ePermissions.HighScores)
            {
                DrawHighScores();
            }

        }

        public override void Update(float fDeltaTime)
        {
            base.Update(fDeltaTime);
        }

        public override void Draw(float fDeltaTime, SpriteBatch DrawBatch)
        {
            // Draw background
            GraphicsManager g = GraphicsManager.Get();
            Rectangle rect = new Rectangle(0, 0,
                g.Width, g.Height);
            g.DrawFilled(DrawBatch, rect, Color.Black, 4.0f, Color.Black);

            Vector2 vOffset = Vector2.Zero;

            vOffset.Y -= GraphicsManager.Get().Height / 3.5f;
            //vOffset.X -= GraphicsManager.Get().Width / 5.0f;
            DrawCenteredString(DrawBatch, m_OptionsText, m_TitleFont, Color.White, vOffset);

            base.Draw(fDeltaTime, DrawBatch);
        }

        public void DrawHighScores()
        {
            m_ScoreButtons.Clear(); 
            foreach (SongList sl in m_SongList)
            {
                if(sl.m_title == m_SongClicked)
                {
                    Point vPos = new Point();
                    vPos.Y = GraphicsManager.Get().Height / 3 - 20;
                    vPos.X = (GraphicsManager.Get().Width / 2) + 80;

         
                    m_ScoreButtons.AddLast(new ScoreButton(vPos, m_SongClicked + " High Scores", m_ScoreFont, Color.HotPink, Color.HotPink, eButtonAlign.Left));
                    vPos.X -= 30; 
                    Point scorePos = vPos;
                    scorePos.X += 350; 

                    for (int i = 1; i <= sl.m_HighScorers.Count; i++)
                    {
                        vPos.Y += 50;
                        scorePos.Y += 50; 
                        m_ScoreButtons.AddLast(new ScoreButton(vPos, i + ". " + sl.m_HighScorers[i-1].name,
                            m_ScoreFont, Color.Aquamarine, Color.Aquamarine, eButtonAlign.Left));
                        m_ScoreButtons.AddLast(new ScoreButton(scorePos, "" + sl.m_HighScorers[i - 1].score, m_ScoreFont, Color.Aquamarine, Color.Aquamarine, eButtonAlign.Right)); 
                    }
                    break; 
                }
            }
        }

        public override void KeyboardInput(SortedList<eBindings, BindInfo> binds)
        {
            GameState g = GameState.Get();
            if (binds.ContainsKey(eBindings.UI_Exit))
            {
                GameState.Get().SetState(eGameState.MainMenu);
                binds.Remove(eBindings.UI_Exit);
            }

            // Handle any input before the gameplay screen can look at it

            base.KeyboardInput(binds);
        }


  
    }
}
