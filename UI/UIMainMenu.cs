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

namespace itp380.UI
{
	public class UIMainMenu : UIScreen
	{
		SpriteFont m_TitleFont;
		SpriteFont m_ButtonFont;
		string m_Title;

		public UIMainMenu(ContentManager Content) :
			base(Content)
        {
			m_TitleFont = m_Content.Load<SpriteFont>("SpaceAge");
			m_ButtonFont = m_Content.Load<SpriteFont>("SquaredDisplay_Bold");

			// Create buttons
			Point vPos = new Point();
			//vPos.X = (int) (GraphicsManager.Get().Width / 1.3f);

            vPos.X = (int)(GraphicsManager.Get().Width / 2.0f);
			vPos.Y = (int)(GraphicsManager.Get().Height / 2.5f);

			m_Title = "Rhythm Runner";

			m_Buttons.AddLast(new Button(vPos, "Runner Mode", 
				m_ButtonFont, Color.Aquamarine, 
				Color.White, SongSelection, eButtonAlign.Center));

            vPos.Y += 80;
            m_Buttons.AddLast(new Button(vPos, "Dance Pad Mode",
            m_ButtonFont, Color.Aquamarine,
            Color.White, DancePadMode, eButtonAlign.Center));

            vPos.Y += 80;
            m_Buttons.AddLast(new Button(vPos, "Edit Song",
                m_ButtonFont, Color.Aquamarine,
                Color.White, EditSong, eButtonAlign.Center)); 

            /*
            vPos.Y += 80; 
            m_Buttons.AddLast(new Button(vPos, "Options",
                m_ButtonFont, Color.Aquamarine,
                Color.White, Options, eButtonAlign.Center));
             */

            vPos.Y += 80;
            m_Buttons.AddLast(new Button(vPos, "High Scores",
                m_ButtonFont, Color.Aquamarine,
                Color.White, HighScore, eButtonAlign.Center));

			vPos.Y += 80;
			m_Buttons.AddLast(new Button(vPos, "Exit",
				m_ButtonFont, Color.Aquamarine,
				Color.White, Exit, eButtonAlign.Center));
		}

        public void SongSelection()
        {
            GameState.Get().SetState(eGameState.SongSelection);
            GameState.Get().m_permissions = ePermissions.Normal;

            GameState.Get().DanceMode = false; 
        }

        public void DancePadMode()
        {
            GameState.Get().SetState(eGameState.SongSelection);
            GameState.Get().m_permissions = ePermissions.Normal;
            GameState.Get().DanceMode = true; 
        }

        public void EditSong()
        {
            GameState.Get().SetState(eGameState.SongSelection);
            GameState.Get().m_permissions = ePermissions.SongEdit; 
            //GameState.Get().SetState(eGameState.Edit); 
            //TODO: Change permisions for this menu
        }

        /**
		public void NewGame()
		{
			SoundManager.Get().PlaySoundCue("MenuClick");
			GameState.Get().SetState(eGameState.Gameplay);
		}
         */

		public void Options()
		{
            GameState.Get().SetState(eGameState.OptionsMenu); 
		}

		public void Exit()
		{
			SoundManager.Get().PlaySoundCue("MenuClick");
			GameState.Get().Exit();
		}

        public void HighScore()
        {
            GameState.Get().SetState(eGameState.HighScoreScreen);
            GameState.Get().m_permissions = ePermissions.HighScores; 
        }

		public override void Update(float fDeltaTime)
		{
			base.Update(fDeltaTime);
		}

		public override void Draw(float fDeltaTime, SpriteBatch DrawBatch)
		{
			Vector2 vOffset = Vector2.Zero;
			vOffset.Y = -1.0f * GraphicsManager.Get().Height / 4.0f;
            //vOffset.X = GraphicsManager.Get().Width / 5.0f; 
			DrawCenteredString(DrawBatch, m_Title, m_TitleFont, Color.White, vOffset);

			base.Draw(fDeltaTime, DrawBatch);
		}
	}
}
