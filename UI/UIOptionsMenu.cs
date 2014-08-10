//-----------------------------------------------------------------------------
// Options Menu

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
	public class UIOptionsMenu : UIScreen
	{
		SpriteFont m_TitleFont;
		SpriteFont m_ButtonFont;
		string m_OptionsText;

		public UIOptionsMenu(ContentManager Content) :
			base(Content)
		{
			m_bCanExit = true;

            m_TitleFont = m_Content.Load<SpriteFont>("SpaceAge");
            m_ButtonFont = m_Content.Load<SpriteFont>("SquaredDisplay");

			m_OptionsText = "Options";
			// Create buttons
            Point vPos = new Point();
            vPos.X = (int)(GraphicsManager.Get().Width / 2.0f);
			//vPos.X = (int) (GraphicsManager.Get().Width / 5.0f);
			vPos.Y = (int)(GraphicsManager.Get().Height / 3.5f);

			m_Buttons.AddLast(new Button(vPos, "Option 1",
                m_ButtonFont, Color.HotPink, 
				Color.White, Option1, eButtonAlign.Center));

			vPos.Y += 50;
            m_Buttons.AddLast(new Button(vPos, "Option 2",
            m_ButtonFont, Color.HotPink,
            Color.White, Option2, eButtonAlign.Center));

            vPos.Y += 50;
            m_Buttons.AddLast(new Button(vPos, "Option 3",
            m_ButtonFont, Color.HotPink,
            Color.White, Option3, eButtonAlign.Center));

            vPos.Y += 50;
			m_Buttons.AddLast(new Button(vPos, "Back to Menu",
                m_ButtonFont, Color.Turquoise,
				Color.White, Quit, eButtonAlign.Center));

			SoundManager.Get().PlaySoundCue("MenuClick");
		}

        public void Option1()
        {
        }

        public void Option2()
        {
        }

        public void Option3()
        {
        }

		public void Quit()
		{
			GameState.Get().SetState(eGameState.MainMenu);
			SoundManager.Get().PlaySoundCue("MenuClick");
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
            //vOffset.X -= GraphicsManager.Get().Width / 3.3f;
			DrawCenteredString(DrawBatch, m_OptionsText, m_TitleFont, Color.White, vOffset);

			base.Draw(fDeltaTime, DrawBatch);
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
