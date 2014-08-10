//-----------------------------------------------------------------------------
// UIGameplay is UI while in the main game state.
// Because there are so many aspects to the UI, this class is relatively large.
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
	public class UIGameplay : UIScreen
	{
		SpriteFont m_FixedSmall;
		SpriteFont m_StatusFont;


		public UIGameplay(ContentManager Content) :
			base(Content)
		{
			//m_FixedFont = Content.Load<SpriteFont>("Fonts/FixedText");
			m_FixedSmall = Content.Load<SpriteFont>("imagine");
			m_StatusFont = Content.Load<SpriteFont>("imagine_bold");
		}

		public override void Update(float fDeltaTime)
		{
			base.Update(fDeltaTime);
		}

		public override void Draw(float fDeltaTime, SpriteBatch DrawBatch)
		{	
			base.Draw(fDeltaTime, DrawBatch);

            Color MultColor = Color.White;
            Color HealthColor = Color.Yellow;
            if (GameState.Get().Health <= 10)
            {
                HealthColor = Color.Red;
            }
            else if (GameState.Get().Health >= 35)
            {
                HealthColor = Color.LimeGreen;
            }

            if (GameState.Get().MultMult == 2)
            {
                //MultColor = Color.Yellow;
                for (float i = 0.0f; i < 100.0f; i += 2)
                {
                    GraphicsManager.Get().DrawLine(DrawBatch, 2.0f, Color.Cyan * (1.0f - i / 100.0f),
                        new Vector2(0.0f + i, 0.0f + i),
                        new Vector2(GraphicsManager.Get().Width - i, 0.0f + i));
                    GraphicsManager.Get().DrawLine(DrawBatch, 2.0f, Color.Cyan * (1.0f - i / 100.0f),
                        new Vector2(1.0f + i, 0.0f + i),
                        new Vector2(1.0f + i, GraphicsManager.Get().Height - i));
                    GraphicsManager.Get().DrawLine(DrawBatch, 2.0f, Color.Cyan * (1.0f - i / 100.0f),
                        new Vector2(GraphicsManager.Get().Width - i, 0.0f + i),
                        new Vector2(GraphicsManager.Get().Width - i, GraphicsManager.Get().Height - i));
                    GraphicsManager.Get().DrawLine(DrawBatch, 2.0f, Color.Cyan * (1.0f - i / 100.0f),
                        new Vector2(0.0f + i, GraphicsManager.Get().Height - 1.0f - i),
                        new Vector2(GraphicsManager.Get().Width - i, GraphicsManager.Get().Height - 1.0f - i));
                }
            }
            else if (GameState.Get().Health <= 10)
            {
                for (float i = 0.0f; i < 100.0f; i += 2)
                {
                    GraphicsManager.Get().DrawLine(DrawBatch, 2.0f, Color.Red * (1.0f - i / 100.0f),
                        new Vector2(0.0f + i, 0.0f + i),
                        new Vector2(GraphicsManager.Get().Width - i, 0.0f + i));
                    GraphicsManager.Get().DrawLine(DrawBatch, 2.0f, Color.Red * (1.0f - i / 100.0f),
                        new Vector2(1.0f + i, 0.0f + i),
                        new Vector2(1.0f + i, GraphicsManager.Get().Height - i));
                    GraphicsManager.Get().DrawLine(DrawBatch, 2.0f, Color.Red * (1.0f - i / 100.0f),
                        new Vector2(GraphicsManager.Get().Width - i, 0.0f + i),
                        new Vector2(GraphicsManager.Get().Width - i, GraphicsManager.Get().Height - i));
                    GraphicsManager.Get().DrawLine(DrawBatch, 2.0f, Color.Red * (1.0f - i / 100.0f),
                        new Vector2(0.0f + i, GraphicsManager.Get().Height - 1.0f - i),
                        new Vector2(GraphicsManager.Get().Width - i, GraphicsManager.Get().Height - 1.0f - i));
                }
            }

            //Draw the health bar
            GraphicsManager.Get().DrawFilled(DrawBatch, new Rectangle(8, 280, 76, 204), Color.Black, 2.0f, Color.White); 
            for (int i = 0; i < GameState.Get().Health; i++)
            {
                GraphicsManager.Get().DrawLine(DrawBatch, 2.0f, HealthColor,
                new Vector2(10.0f, 480.0f - 4*i), new Vector2(80.0f, 480.0f - 4*i));
            }

            DrawBatch.DrawString(m_StatusFont, "Score: " + GameState.Get().Score.ToString(), new Vector2(670, 5), Color.White);
            int accuracy = 100;
            if (GameState.Get().TotalNotes > 0)
            {
                accuracy = (GameState.Get().HitNotes * 100 ) / GameState.Get().TotalNotes;
            }
            DrawBatch.DrawString(m_StatusFont, "Accuracy: " + accuracy + "%", new Vector2(670, 40), Color.White);
            DrawBatch.DrawString(m_StatusFont, "Multiplier: " + GameState.Get().Multiplier.ToString(), new Vector2(650, 700), MultColor);
            if (GameState.Get().Shield > 0)
            {
                DrawBatch.DrawString(m_StatusFont, "Shield Strength: " + GameState.Get().Shield.ToString(), new Vector2(50, 700), Color.White);
            }
		}

		public override void KeyboardInput(SortedList<eBindings, BindInfo> binds)
		{
			GameState g = GameState.Get();
			if (binds.ContainsKey(eBindings.UI_Exit))
			{
				g.ShowPauseMenu(); 
				binds.Remove(eBindings.UI_Exit);
			}

			// Handle any input before the gameplay screen can look at it

			base.KeyboardInput(binds);
		}
	}
}
