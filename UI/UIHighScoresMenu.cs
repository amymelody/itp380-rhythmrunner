//-----------------------------------------------------------------------------
// High Score Menu

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
    public class UIHighScoresMenu : UIScreen
    {
        SpriteFont m_TitleFont;
        SpriteFont m_ButtonFont;
        string m_OptionsText;
        private List<HighScorers> m_HighScorers = new List<HighScorers>(); 

        public UIHighScoresMenu(ContentManager Content, List<HighScorers> hs) :
            base(Content)
        {
            m_bCanExit = true;

            m_TitleFont = m_Content.Load<SpriteFont>("SpaceAge");
            m_ButtonFont = m_Content.Load<SpriteFont>("SquaredDisplay");

            m_OptionsText = "High Scores";
            // Create strings 
            Point vPos = new Point();
            vPos.X = (int)(GraphicsManager.Get().Width / 2.0f);
            vPos.Y = (int)(GraphicsManager.Get().Height / 3.5f);


            vPos.Y = (int)(GraphicsManager.Get().Height / 1.1f);
            m_Buttons.AddLast(new Button(vPos, "Back to Menu",
                m_ButtonFont, new Color(0, 0, 200),
                Color.White, Quit, eButtonAlign.Center));

            SoundManager.Get().PlaySoundCue("MenuClick");
            m_HighScorers = hs; 
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

            DrawCenteredString(DrawBatch, m_OptionsText, m_TitleFont, Color.White, vOffset);

            vOffset.X -= 100;
            vOffset.Y += 50;
            Vector2 scoreOffset = vOffset;
            scoreOffset.X += 200;
            for (int i = 1; i <= m_HighScorers.Count; i++)
            {
                vOffset.Y += 50;
                scoreOffset.Y += 50;
                DrawCenteredString(DrawBatch, i.ToString() + ". " + m_HighScorers[i - 1].name, m_ButtonFont, Color.White, vOffset);
                DrawCenteredString(DrawBatch, m_HighScorers[i - 1].score.ToString(), m_ButtonFont, Color.White, scoreOffset);
            };
             
            base.Draw(fDeltaTime, DrawBatch);
        }
    }
}
