//-----------------------------------------------------------------------------
// Base UIScreen class that all other UIScreens inherit from.
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
	public class UIScreen
	{
		protected LinkedList<Button> m_Buttons = new LinkedList<Button>();
        protected LinkedList<SongButton> m_SongButtons = new LinkedList<SongButton>();
        protected LinkedList<ScoreButton> m_ScoreButtons = new LinkedList<ScoreButton>();
		protected ContentManager m_Content;
		protected float m_fLiveTime = 0.0f;
		// Determines whether or not you can press ESC to leave the screen
		protected bool m_bCanExit = false;
		protected Utils.Timer m_Timer = new Utils.Timer();

		public UIScreen(ContentManager Content)
		{
			m_Content = Content;
		}

		public virtual void Update(float fDeltaTime)
		{
			m_fLiveTime += fDeltaTime;
			foreach (Button b in m_Buttons)
			{
				// If the button is enabled, the mouse is pointing to it, and the UI is the top one
				if (b.Enabled && b.m_Bounds.Contains(InputManager.Get().MousePosition) &&
					GameState.Get().GetCurrentUI() == this)
				{
					b.HasFocus = true;
				}
				else
				{
					b.HasFocus = false;
				}
			}

            foreach (SongButton sb in m_SongButtons)
            {
                // If the button is enabled, the mouse is pointing to it, and the UI is the top one
                if (sb.Enabled && sb.m_Bounds.Contains(InputManager.Get().MousePosition) &&
                    GameState.Get().GetCurrentUI() == this)
                {
                    sb.HasFocus = true;
                }
                else
                {
                    sb.HasFocus = false;
                }
            }

			m_Timer.Update(fDeltaTime);
		}

		public virtual void Draw(float fDeltaTime, SpriteBatch DrawBatch)
		{
			DrawButtons(fDeltaTime, DrawBatch);
		}

		public virtual bool MouseClick(Point Position)
		{
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

            foreach (SongButton sb in m_SongButtons)
            {
                if (sb.Enabled && sb.m_Bounds.Contains(Position))
                {
                    sb.Click();
                    bReturn = true;
                    break;
                }
            }

			return bReturn;
		}

        public virtual bool MouseClickRight(Point Position)
        {//Only necessary for song edit
            return false;
        }

        public virtual bool MouseHold(Point Position)
        {//Only necessary for song edit
            return false;
        }

        public virtual bool MouseScroll(int Scroll)
        {//Only necessary for song edit
            return false;
        }

		protected void DrawButtons(float fDeltaTime, SpriteBatch DrawBatch)
		{
			foreach (Button b in m_Buttons)
			{
				if (b.Enabled)
				{
					b.Draw(fDeltaTime, DrawBatch);
				}
			}

            foreach (SongButton sb in m_SongButtons)
            {
                if (sb.Enabled)
                {
                    sb.Draw(fDeltaTime, DrawBatch);
                }
            }

            foreach (ScoreButton sb in m_ScoreButtons)
            {
                if (sb.Enabled)
                {
                    sb.Draw(fDeltaTime, DrawBatch); 
                }
            }

		}

		public void DrawCenteredString(SpriteBatch DrawBatch, string sText, 
			SpriteFont font, Color color, Vector2 vOffset)
		{
			Vector2 pos = new Vector2(GraphicsManager.Get().Width / 2.0f, GraphicsManager.Get().Height / 2.0f);
			pos -= font.MeasureString(sText) / 2.0f;
			pos += vOffset;
			DrawBatch.DrawString(font, sText, pos, color);
		}

		public virtual void KeyboardInput(SortedList<eBindings, BindInfo> binds)
		{
			if (binds.ContainsKey(eBindings.UI_Exit))
			{
				if (m_bCanExit)
				{
					GameState.Get().PopUI();
				}

				binds.Remove(eBindings.UI_Exit);
			}
		}

		public virtual void OnExit()
		{

		}
	}
}
