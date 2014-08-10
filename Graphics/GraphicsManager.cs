//-----------------------------------------------------------------------------
// The GraphicsManager handles all lower-level aspects of rendering.
//
// __Defense Sample for Game Programming Algorithms and Techniques
// Copyright (C) Sanjay Madhav. All rights reserved.
//
// Released under the Microsoft Permissive License.
// See LICENSE.txt for full details.
//-----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace itp380
{
	public enum eDrawOrder
	{
		Default,
		Background,
		Foreground
	}

	public class GraphicsManager : itp380.Patterns.Singleton<GraphicsManager>
	{

		GraphicsDeviceManager m_Graphics;
		Game m_Game;
		SpriteBatch m_SpriteBatch;
		Texture2D m_Blank;

		SpriteFont m_FPSFont;

        VisualizationData m_VisualData = null;
        public VisualizationData VisualData
        {
            get
            {
                return m_VisualData;
            }
            set
            {
                m_VisualData = value;
            }
        }

		LinkedList<GameObject> m_DefaultObjects = new LinkedList<GameObject>();
		LinkedList<GameObject> m_BGObjects = new LinkedList<GameObject>();
		LinkedList<GameObject> m_FGObjects = new LinkedList<GameObject>();

		public Matrix Projection;
		
		public bool IsFullScreen
		{
			get { return m_Graphics.IsFullScreen; }
			set { m_Graphics.IsFullScreen = value; }
		}

		public bool IsVSync
		{
			get { return m_Graphics.SynchronizeWithVerticalRetrace; }
			set { m_Graphics.SynchronizeWithVerticalRetrace = value; }
		}

		public int Width
		{
			get { return m_Graphics.PreferredBackBufferWidth; }
		}

		public int Height
		{
			get { return m_Graphics.PreferredBackBufferHeight; }
		}

		public GraphicsDevice GraphicsDevice
		{
			get { return m_Graphics.GraphicsDevice; }
		}

		float m_fZoom = GlobalDefines.fCameraZoom;

        float m_GridWidth;
        float m_GridHeight;

        Queue<Point> m_Points = new Queue<Point>();
        Utils.Timer m_Timer = new Utils.Timer();
		
        SpriteFont m_FixedSmall;

		public void Start(Game game)
		{
			m_Graphics = new GraphicsDeviceManager(game);
			m_Game = game;
			IsVSync = GlobalDefines.bVSync;
			
			// TODO: Set resolution to what's saved in the INI, or default full screen
			if (!GlobalDefines.bFullScreen)
			{
				SetResolution(GlobalDefines.WindowedWidth, GlobalDefines.WindowHeight);
			}
			else
			{
				SetResolutionToCurrent();
				ToggleFullScreen();
			}
		}

		public void LoadContent()
		{
			//InitializeRenderTargets();

			m_SpriteBatch = new SpriteBatch(m_Graphics.GraphicsDevice);
						
			// Load FPS font
			m_FPSFont = m_Game.Content.Load<SpriteFont>("Fonts/FixedText");
            m_FixedSmall = m_Game.Content.Load<SpriteFont>("imagine");

			// Debug stuff for line drawing
			m_Blank = new Texture2D(m_Graphics.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
			m_Blank.SetData(new[] { Color.White });
		}

		public void SetResolutionToCurrent()
		{
			m_Graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
			m_Graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

			m_fZoom = GlobalDefines.fCameraZoom;
			SetProjection((float)Width / Height);

			if (m_Graphics.GraphicsDevice != null)
			{
				m_Graphics.ApplyChanges();
			}
		}

		public void SetResolution(int Width, int Height)
		{
			m_Graphics.PreferredBackBufferWidth = Width;
			m_Graphics.PreferredBackBufferHeight = Height;

			m_fZoom = GlobalDefines.fCameraZoom;
			SetProjection((float)Width / Height);

			if (m_Graphics.GraphicsDevice != null)
			{
				m_Graphics.ApplyChanges();
			}
		}

		public void SetProjection(float fAspectRatio)
		{
            Projection = Matrix.CreatePerspective(m_fZoom, m_fZoom / fAspectRatio, 10.0f, 40.0f);
            //Projection = Matrix.CreateOrthographic(m_fZoom, m_fZoom / fAspectRatio, 2.0f, 10.0f);
		}

		public void ResetProjection()
		{
			m_fZoom = GlobalDefines.fCameraZoom;
			SetProjection((float)Width / Height);
		}

		public void ToggleFullScreen()
		{
			m_Graphics.ToggleFullScreen();
		}

		public void Draw(float fDeltaTime)
		{
            m_Timer.Update(fDeltaTime);

			// Clear back buffer
			m_Graphics.GraphicsDevice.Clear(Color.Black);

			// First draw all 3D components
			m_Game.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
			// For background objects, disabled Z-Buffer
			m_Game.GraphicsDevice.DepthStencilState = DepthStencilState.None;
			foreach (GameObject o in m_BGObjects)
			{
				if (o.Enabled)
				{
					o.Draw(fDeltaTime);
				}
			}

			m_Game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
			foreach (GameObject o in m_DefaultObjects)
			{
				if (o.Enabled)
				{
					o.Draw(fDeltaTime);
				}
			}

			// Also disabled Z-Buffer for background objects
			m_Game.GraphicsDevice.DepthStencilState = DepthStencilState.None;
			foreach (GameObject o in m_FGObjects)
			{
				if (o.Enabled)
				{
					o.Draw(fDeltaTime);
				}
			}

			// Now draw all 2D components
			m_SpriteBatch.Begin();

            // Draw the grid
            if (GameState.Get().State == eGameState.Gameplay)
            {
                DrawGrid(-m_GridWidth / 2.0f, -m_GridHeight / 2.0f, m_GridWidth / 3.0f, m_GridHeight / 3.0f, 0.0f, 3, 3, m_SpriteBatch, 1, Color.White);
            }

            // Draw the hit rectangles for dance mode
            float xBounds = GameState.Get().GridWidth / 3.0f;
            float yBounds = GameState.Get().GridHeight / 3.0f;
            foreach (Point point in m_Points)
            {
                float xPos = point.X * xBounds;
                float yPos = point.Y * yBounds;
                DrawRect3D(m_SpriteBatch,
                    new Vector3(xPos - (xBounds / 2.0f), yPos - (yBounds / 2.0f), 0.0f),
                    new Vector3(xPos - (xBounds / 2.0f), yPos + (yBounds / 2.0f), 0.0f),
                    new Vector3(xPos + (xBounds / 2.0f), yPos - (yBounds / 2.0f), 0.0f),
                    new Vector3(xPos + (xBounds / 2.0f), yPos + (yBounds / 2.0f), 0.0f),
                    0.0f, Color.HotPink, 8.0f);
            }

            // Draw the song visualization
            GameState currentGameState = GameState.Get(); 
            if (m_VisualData != null && currentGameState.State == eGameState.Gameplay)
            {
                Color visualizationColor = new Color(30, 50 * currentGameState.Multiplier, 250 - 50 * currentGameState.Multiplier);
                DrawSongVisualizationVertical(m_GridWidth, 0.0f, m_GridHeight / 2.0f, m_GridHeight / 2.0f + (float)currentGameState.Multiplier / 2.0f, m_SpriteBatch, 1.0f, visualizationColor);
                DrawSongVisualizationVertical(m_GridWidth, 0.0f, -m_GridHeight / 2.0f, -m_GridHeight / 2.0f - (float)currentGameState.Multiplier / 2.0f, m_SpriteBatch, 1.0f, visualizationColor);
                DrawSongVisualizationHorizontal(m_GridHeight, 0.0f, m_GridWidth / 2.0f, m_GridWidth / 2.0f + (float)currentGameState.Multiplier / 2.0f, m_SpriteBatch, 1.0f, visualizationColor);
                DrawSongVisualizationHorizontal(m_GridHeight, 0.0f, -m_GridWidth / 2.0f, -m_GridWidth / 2.0f - (float)currentGameState.Multiplier / 2.0f, m_SpriteBatch, 1.0f, visualizationColor);
                if (!currentGameState.DanceMode)
                {
                    DrawSongVisualizationCircle(currentGameState.PlayerCurrent.Position.X, currentGameState.PlayerCurrent.Position.Y, 1.5f, 2.0f + (float)currentGameState.Multiplier / 8f, m_SpriteBatch, 1.0f, visualizationColor);
                }
            }

			// Draw the UI screens
			GameState.Get().DrawUI(fDeltaTime, m_SpriteBatch);

			// Draw FPS counter
			Vector2 vFPSPos = Vector2.Zero;
			if (DebugDefines.bShowBuildString)
			{
				m_SpriteBatch.DrawString(m_FPSFont, DebugDefines.DebugName, vFPSPos, Color.White);
				vFPSPos.Y += 25.0f;
			}
			if (DebugDefines.bShowFPS)
			{
				string sFPS = String.Format("FPS: {0}", (int)(1 / fDeltaTime));
				m_SpriteBatch.DrawString(m_FPSFont, sFPS, vFPSPos, Color.White);
			}

			m_SpriteBatch.End();
		}

        public void DrawHitRect(Point position)
        {
            m_Points.Enqueue(position);
            string timerName = "Remove point" + position.X + position.Y;
            while (!m_Timer.AddTimer(timerName, 0.2f, RemovePoint, false))
            {
                timerName = timerName + "a";
            }
        }

        void RemovePoint()
        {
            m_Points.Dequeue();
        }

		public void AddGameObject(GameObject o)
		{
			if (o.DrawOrder == eDrawOrder.Background)
			{
				m_BGObjects.AddLast(o);
			}
			else if (o.DrawOrder == eDrawOrder.Default)
			{
				m_DefaultObjects.AddLast(o);
			}
			else
			{
				m_FGObjects.AddLast(o);
			}
		}

		public void RemoveGameObject(GameObject o)
		{
			if (o.DrawOrder == eDrawOrder.Background)
			{
				m_BGObjects.Remove(o);
			}
			else if (o.DrawOrder == eDrawOrder.Default)
			{
				m_DefaultObjects.Remove(o);
			}
			else
			{
				m_FGObjects.Remove(o);
			}
		}

		public void ClearAllObjects()
		{
			m_BGObjects.Clear();
			m_DefaultObjects.Clear();
			m_FGObjects.Clear();
		}

		// Draws a line
		public void DrawLine(SpriteBatch batch, float width, Color color, 
			Vector2 point1, Vector2 point2)
		{
			float angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
			float length = Vector2.Distance(point1, point2);

			batch.Draw(m_Blank, point1, null, color,
					   angle, Vector2.Zero, new Vector2(length, width),
					   SpriteEffects.None, 0);
		}

		public void DrawLine3D(SpriteBatch batch, float width, Color color, Vector3 point1, Vector3 point2)
		{
			// Convert the 3D points into screen space points
			Vector3 point1_screen = GraphicsDevice.Viewport.Project(point1, Projection, 
				GameState.Get().CameraMatrix, Matrix.Identity);
			Vector3 point2_screen = GraphicsDevice.Viewport.Project(point2, Projection,
				GameState.Get().CameraMatrix, Matrix.Identity);

			// Now draw a 2D line with the appropriate points
			DrawLine(batch, width, color, new Vector2(point1_screen.X, point1_screen.Y),
				new Vector2(point2_screen.X, point2_screen.Y));
		}

		public void DrawFilled(SpriteBatch batch, Rectangle rect, Color color, float outWidth, Color outColor)
		{
			// Draw the background
			batch.Draw(m_Blank, rect, color);

			// Draw the outline
			DrawLine(batch, outWidth, outColor, new Vector2(rect.Left, rect.Top),
				new Vector2(rect.Right, rect.Top));
			DrawLine(batch, outWidth, outColor, new Vector2(rect.Left, rect.Top),
				new Vector2(rect.Left, rect.Bottom + (int)outWidth));
			DrawLine(batch, outWidth, outColor, new Vector2(rect.Left, rect.Bottom),
				new Vector2(rect.Right, rect.Bottom));
			DrawLine(batch, outWidth, outColor, new Vector2(rect.Right, rect.Top),
				new Vector2(rect.Right, rect.Bottom));
		}

        public void DrawRect3D(SpriteBatch batch, Vector3 topLeft, Vector3 topRight, Vector3 bottomLeft, Vector3 bottomRight, float zPos, Color color, float lineWidth)
        {
            // Draw the outline
            DrawLine3D(batch, lineWidth, color, topLeft, topRight);
            DrawLine3D(batch, lineWidth, color, topRight, bottomRight);
            DrawLine3D(batch, lineWidth, color, bottomRight, bottomLeft);
            DrawLine3D(batch, lineWidth, color, bottomLeft, topLeft);
        }

        public void DrawGrid(float startingX, float startingY, float xSize, float ySize, float zPos, int rows, int columns,
            SpriteBatch batch, float lineWidth, Color color)
        {
            for (int i = 0; i <= rows; i++)
            {
                DrawLine3D(batch, lineWidth, color, new Vector3(startingX, startingY + ySize * i, zPos),
                    new Vector3(startingX + xSize *  columns, startingY + ySize * i, zPos));
            }
            for (int i = 0; i <= columns; i++)
            {
                DrawLine3D(batch, lineWidth, color, new Vector3(startingX + xSize * i, startingY, zPos),
                    new Vector3(startingX + xSize * i, startingY + ySize * rows, zPos));
            }
        }

        public void SetGridDimensions(float width, float height)
        {
            m_GridWidth = width;
            m_GridHeight = height;
        }

        public void DrawSongVisualizationVertical(float width, float xCenter, float yStart, float yEnd, SpriteBatch batch, float lineWidth, Color color)
        {
            float maxFrequency = m_VisualData.Samples.Max();
            float minFrequency = m_VisualData.Samples.Min();
            for (int i = 0; i < m_VisualData.Samples.Count; i+=2)
            {
                float normalizedFrequency = (m_VisualData.Samples[i] - minFrequency) / (maxFrequency - minFrequency);
                DrawLine3D(m_SpriteBatch, lineWidth, color,
                    new Vector3(-(width / 2) + (width / (float)m_VisualData.Samples.Count) * i + xCenter, yStart, 0.0f),
                    new Vector3(-(width / 2) + (width / (float)m_VisualData.Samples.Count) * i + xCenter, yStart + normalizedFrequency * (yEnd - yStart), 0.0f));
            }
        }

        public void DrawSongVisualizationHorizontal(float height, float yCenter, float xStart, float xEnd, SpriteBatch batch, float lineWidth, Color color)
        {
            float maxFrequency = m_VisualData.Samples.Max();
            float minFrequency = m_VisualData.Samples.Min();
            for (int i = 0; i < m_VisualData.Samples.Count; i+=2)
            {
                float normalizedFrequency = (m_VisualData.Samples[i] - minFrequency) / (maxFrequency - minFrequency);
                DrawLine3D(m_SpriteBatch, lineWidth, color,
                    new Vector3(xStart, -(height / 2) + (height / (float)m_VisualData.Samples.Count) * i + yCenter, 0.0f),
                    new Vector3(xStart + normalizedFrequency * (xEnd - xStart), -(height / 2) + (height / (float)m_VisualData.Samples.Count) * i + yCenter, 0.0f));
            }
        }

        public void DrawSongVisualizationCircle(float xCenter, float yCenter, float innerRadius, float outerRadius, SpriteBatch batch, float lineWidth, Color color)
        {
            float maxFrequency = m_VisualData.Samples.Max();
            float minFrequency = m_VisualData.Samples.Min();

            for (int i = 0; i < m_VisualData.Samples.Count; i += 8)
            {
                double currentAngle = (double)i / (double)m_VisualData.Samples.Count * MathHelper.TwoPi;
                float normalizedFrequency = (m_VisualData.Samples[i] - minFrequency) / (maxFrequency - minFrequency);
                DrawLine3D(m_SpriteBatch, lineWidth, color,
                    new Vector3(innerRadius * (float)Math.Cos(currentAngle) + xCenter, innerRadius * (float)Math.Sin(currentAngle) + yCenter, 0.0f),
                    new Vector3((innerRadius + (outerRadius - innerRadius) * normalizedFrequency) * (float)Math.Cos(currentAngle) + xCenter, (innerRadius + (outerRadius - innerRadius) * normalizedFrequency) * (float)Math.Sin(currentAngle) + yCenter, 0.0f));
            }
        }

        public void DrawNoteHitLabel(SpriteBatch batch, Vector3 position, string label, float scale, Color color)
        {
            Vector3 screenPosition = GraphicsDevice.Viewport.Project(position, Projection, 
				GameState.Get().CameraMatrix, Matrix.Identity);

            Vector2 origin = m_FixedSmall.MeasureString(label) / 2.0f;
            batch.DrawString(m_FixedSmall, label, new Vector2(screenPosition.X, screenPosition.Y), color, 0.0f, origin, scale, SpriteEffects.None, 0);
        }
	}
}
