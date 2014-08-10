//-----------------------------------------------------------------------------
// UINewHighScore comes up if you achieve a new high score. 
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
using Microsoft.Xna.Framework.Media;

namespace itp380.UI
{
	public class UINewHighScore : UIScreen
	{
		SpriteFont m_TitleFont;
		SpriteFont m_ButtonFont;
		string m_NewScoreText;
        EditableTextField m_SongTitle;
        string name = " "; 

        string m_name;

		public UINewHighScore(ContentManager Content) :
			base(Content)
		{
            m_bCanExit = false;

			m_TitleFont = m_Content.Load<SpriteFont>("SquaredDisplay");
			m_ButtonFont = m_Content.Load<SpriteFont>("imagine");

            m_NewScoreText = "New High Score";
			// Create buttons
			Point vPos = new Point();
			vPos.X = (int) (GraphicsManager.Get().Width / 2.0f);
			vPos.Y = (int)(GraphicsManager.Get().Height / 2.0f) + 50;

			m_Buttons.AddLast(new Button(vPos, "Enter Name",
				m_ButtonFont, Color.Aquamarine, 
				Color.White, AddName, eButtonAlign.Center));

			vPos.Y += 50;
			m_Buttons.AddLast(new Button(vPos, "Quit",
				m_ButtonFont, Color.Aquamarine,
				Color.White, Quit, eButtonAlign.Center));

			SoundManager.Get().PlaySoundCue("MenuClick");

            EnterNewScore(); 
		}

		public void EnterNewScore()
		{
            Point buttonPos = new Point();
            buttonPos.X = 350;
            buttonPos.Y = 335; 

            m_SongTitle = new EditableTextField(buttonPos,
            "", m_ButtonFont, Color.White, Color.HotPink,
            310, 30, TextFieldType.Text,
            AddNewHighScore, name);

            m_SongTitle.IsSelected = true;
            m_Buttons.AddLast(m_SongTitle);
		}

		public void Quit()
		{
			GameState.Get().SetState(eGameState.MainMenu);
			SoundManager.Get().PlaySoundCue("MenuClick");
		}

        public void AddName()
        {
            m_name = m_SongTitle.ReturnString(); 
            GameState.Get().AddNewHighScore(m_name); 
            GameState.Get().SetState(eGameState.MainMenu);
            SoundManager.Get().PlaySoundCue("MenuClick");
        }

        public void AddNewHighScore()
        {
            m_name = m_SongTitle.ReturnString();
        }

		public override void Update(float fDeltaTime)
		{
			base.Update(fDeltaTime);
		}

		public override void Draw(float fDeltaTime, SpriteBatch DrawBatch)
		{
			// Draw background
			GraphicsManager g = GraphicsManager.Get();
			Rectangle rect = new Rectangle(g.Width / 2 - 200, g.Height / 2 - 115,
				400, 250);
			g.DrawFilled(DrawBatch, rect, Color.Black, 4.0f, Color.HotPink);

			Vector2 vOffset = Vector2.Zero;
			vOffset.Y -= 75;
            DrawCenteredString(DrawBatch, m_NewScoreText, m_TitleFont, Color.White, vOffset);
						
			base.Draw(fDeltaTime, DrawBatch);
		}

		public override void OnExit()
		{
            MediaPlayer.Resume();
			GameState.Get().IsPaused = false;
		}
	}
}
