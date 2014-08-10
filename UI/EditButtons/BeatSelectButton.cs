//-----------------------------------------------------------------------------
// Buttons are used by UIScreens -- they have a list of Buttons and decide
// whether a click hits a particular button.
// There's logic for both text and image-based buttons in here.
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

namespace itp380.UI
{
    public class BeatSelectButton : Button
	{
        public BeatSelectButton(Point Position, string Text, SpriteFont Font, Color Default,
            Color MouseOver, int width, int height, Action Callback,
            Texture2D DefaultTexture = null, Texture2D FocusTexture = null)
            :base(Position, Text, Font, Default, MouseOver, Callback, eButtonAlign.Left)
        {
            m_TexDefault = DefaultTexture;
            m_TexFocus = FocusTexture;

            m_Bounds.Width = width;
            m_Bounds.Height = height;
        }

		private bool m_IsSelected;
		public bool IsSelected
		{
            set { m_IsSelected = value; }
            get { return m_IsSelected; }
		}

		public override void Draw(float fDeltaTime, SpriteBatch DrawBatch)
		{
            if (m_TexDefault != null && m_TexFocus != null)
            {
                if (IsSelected)
                {
                    DrawBatch.Draw(m_TexFocus, m_Bounds, Color.White);
                }
                else
                {
                    DrawBatch.Draw(m_TexDefault, m_Bounds, Color.White);
                }
            }
            else
            {
                if (IsSelected)
                {
                    GraphicsManager.Get().DrawFilled(DrawBatch, m_Bounds, Color.Black, 2.0f, Color.DarkGoldenrod);
                }
                else
                {
                    GraphicsManager.Get().DrawFilled(DrawBatch, m_Bounds, Color.Black, 2.0f, Color.Cyan);
                }
            }
            Vector2 middleAlign = (new Vector2(m_Bounds.Width, m_Bounds.Height) - m_Font.MeasureString(m_Text)) * 0.5f;

            if (HasFocus)
            {
                DrawBatch.DrawString(m_Font, m_Text, new Vector2(m_Bounds.X + (int)middleAlign.X, m_Bounds.Y + (int)middleAlign.Y + 1), m_ColorFocus);
            }
            else
            {
                if(IsSelected)
                    DrawBatch.DrawString(m_Font, m_Text, new Vector2(m_Bounds.X + (int)middleAlign.X, m_Bounds.Y + (int)middleAlign.Y + 1), Color.DarkGoldenrod);
                else
                    DrawBatch.DrawString(m_Font, m_Text, new Vector2(m_Bounds.X + (int)middleAlign.X, m_Bounds.Y + (int)middleAlign.Y + 1), m_ColorDefault);
            }
		}
	}
}
