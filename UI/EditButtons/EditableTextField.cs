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
using Microsoft.Xna.Framework.Input;

namespace itp380.UI
{
    public enum TextFieldType
    {
        Text,
        Float,
        Int
    }

    public class EditableTextField : Button
	{
        public EditableTextField(Point Position, string Text, SpriteFont Font, Color Default,
            Color MouseOver, int width, int height, TextFieldType TextType, Action Callback,
            string initialText = "", int maxCharacters = 0, bool editable = true,
            Texture2D DefaultTexture = null, Texture2D FocusTexture = null)
            :base(Position, Text, Font, Default, MouseOver, Callback, eButtonAlign.Left)
        {
            m_TexDefault = DefaultTexture;
            m_TexFocus = FocusTexture;

            m_Bounds.Width = width;
            m_Bounds.Height = height;

            m_TextFieldType = TextType;

            m_LastPressedKeys = new Keys[0];

            m_MaxCharacters = maxCharacters;
            m_Editable = editable;
            m_TextField = initialText;
            if (m_TextFieldType != TextFieldType.Text)
            {
                m_NumberValue = float.Parse(m_TextField);
            }
        }

		private bool m_IsSelected;
		public bool IsSelected
		{
            set { m_IsSelected = value; }
            get { return m_IsSelected; }
		}

        private TextFieldType m_TextFieldType;
        private float m_NumberValue = 0;
        private string m_TextField = "";

        //Key Press Variables
        private Keys[] m_LastPressedKeys;
        float m_DeleteTimer = 0;
        int m_MaxCharacters;
        bool m_Editable;
        
		public override void Draw(float fDeltaTime, SpriteBatch DrawBatch)
		{
            if(IsSelected && m_Editable)
                CaptureKeyboard(fDeltaTime);

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
            else//Draw blank rectangles yay
            {
                if (IsSelected)
                    GraphicsManager.Get().DrawFilled(DrawBatch, m_Bounds, Color.Black, 1.0f, Color.White);
                else
                    GraphicsManager.Get().DrawFilled(DrawBatch, m_Bounds, Color.Black, 1.0f, Color.CadetBlue);
            }

            if (m_TextFieldType == TextFieldType.Text)
            {
                if (HasFocus)
                {
                    if (m_TextField.Length > 0)
                        DrawBatch.DrawString(m_Font, m_TextField, new Vector2(m_Bounds.X, m_Bounds.Y + 5), m_ColorFocus);
                    else
                        DrawBatch.DrawString(m_Font, m_Text, new Vector2(m_Bounds.X, m_Bounds.Y + 5), m_ColorFocus);
                }
                else
                {
                    if (m_TextField.Length > 0)
                        DrawBatch.DrawString(m_Font, m_TextField, new Vector2(m_Bounds.X, m_Bounds.Y + 5), m_ColorDefault);
                    else
                        DrawBatch.DrawString(m_Font, m_Text, new Vector2(m_Bounds.X, m_Bounds.Y + 5), m_ColorDefault);

                    //DrawBatch.DrawString(m_Font, m_Text, new Vector2(m_Bounds.X, m_Bounds.Y), m_ColorDefault);
                    //DrawBatch.DrawString(m_Font, m_TextField, new Vector2(m_Bounds.X - m_Font.MeasureString(m_TextField).X, m_Bounds.Y), m_ColorDefault);
                }
            }
            else if(m_TextFieldType == TextFieldType.Float || m_TextFieldType == TextFieldType.Int)
            {
                if (HasFocus)
                {
                    DrawBatch.DrawString(m_Font, m_Text, new Vector2(m_Bounds.X, m_Bounds.Y + 5), m_ColorFocus);
                    //DrawBatch.DrawString(m_Font, m_TextField, new Vector2(m_Bounds.X - m_Font.MeasureString(m_TextField).X, m_Bounds.Y), m_ColorFocus);
                    DrawBatch.DrawString(m_Font, m_TextField, new Vector2(m_Bounds.X, m_Bounds.Y + m_Bounds.Height/2 + 5), m_ColorFocus);
                }
                else
                {
                    DrawBatch.DrawString(m_Font, m_Text, new Vector2(m_Bounds.X, m_Bounds.Y + 5), m_ColorDefault);
                    //DrawBatch.DrawString(m_Font, m_TextField, new Vector2(m_Bounds.X - m_Font.MeasureString(m_TextField).X, m_Bounds.Y), m_ColorDefault);
                    DrawBatch.DrawString(m_Font, m_TextField, new Vector2(m_Bounds.X, m_Bounds.Y + m_Bounds.Height / 2 + 5), m_ColorDefault);
                }
            }
		}

        private void CaptureKeyboard(float fDeltaTime)
        {
            KeyboardState kbState = Keyboard.GetState();
            Keys[] pressedKeys = kbState.GetPressedKeys();

            bool shift = (pressedKeys.Contains(Keys.LeftShift) || pressedKeys.Contains(Keys.RightShift));

            if (m_MaxCharacters == 0 || (m_MaxCharacters > 0 && m_TextField.Length < m_MaxCharacters))
            {
                foreach (Keys key in pressedKeys)
                {

                    if (!m_LastPressedKeys.Contains(key))
                        OnKeyDown(key, shift);
                }
            }

            if (pressedKeys.Contains(Keys.Back))
            {
                if (m_DeleteTimer == 0.00f)
                {
                    if (m_TextField.Length > 0)
                        m_TextField = m_TextField.Substring(0, m_TextField.Length - 1);
                }
                m_DeleteTimer += fDeltaTime;
                if (m_DeleteTimer >= 0.10f)
                    m_DeleteTimer = 0.00f;
            }
            else
            {
                m_DeleteTimer = 0.00f;
            }

            m_LastPressedKeys = pressedKeys;
        }

        public int ReturnInt()
        {
            if (m_TextFieldType == TextFieldType.Int)
            {
                return (int)m_NumberValue;
            }
            else
            {
                return 0;
            }
        }

        public float ReturnFloat()
        {
            if (m_TextFieldType == TextFieldType.Float)
            {
                return m_NumberValue;
            }
            else
            {
                return 0f;
            }
        }

        public string ReturnString()
        {
            if (m_TextFieldType == TextFieldType.Text)
            {
                return m_TextField;
            }
            else
            {
                return "";
            }
        }

        private void OnKeyDown(Keys key, bool shift)
        {
            if (m_TextFieldType == TextFieldType.Text)
            {
                switch (key)
                {
                    case Keys.A:
                        if (shift)
                            m_TextField = string.Concat(m_TextField, "A");
                        else
                            m_TextField = string.Concat(m_TextField, "a");
                        break;

                    case Keys.B:
                        if (shift)
                            m_TextField = string.Concat(m_TextField, "B");
                        else
                            m_TextField = string.Concat(m_TextField, "b");
                        break;

                    case Keys.C:
                        if (shift)
                            m_TextField = string.Concat(m_TextField, "C");
                        else
                            m_TextField = string.Concat(m_TextField, "c");
                        break;

                    case Keys.D:
                        if (shift)
                            m_TextField = string.Concat(m_TextField, "D");
                        else
                            m_TextField = string.Concat(m_TextField, "d");
                        break;

                    case Keys.E:
                        if (shift)
                            m_TextField = string.Concat(m_TextField, "E");
                        else
                            m_TextField = string.Concat(m_TextField, "e");
                        break;

                    case Keys.F:
                        if (shift)
                            m_TextField = string.Concat(m_TextField, "F");
                        else
                            m_TextField = string.Concat(m_TextField, "f");
                        break;

                    case Keys.G:
                        if (shift)
                            m_TextField = string.Concat(m_TextField, "G");
                        else
                            m_TextField = string.Concat(m_TextField, "g");
                        break;

                    case Keys.H:
                        if (shift)
                            m_TextField = string.Concat(m_TextField, "H");
                        else
                            m_TextField = string.Concat(m_TextField, "h");
                        break;

                    case Keys.I:
                        if (shift)
                            m_TextField = string.Concat(m_TextField, "I");
                        else
                            m_TextField = string.Concat(m_TextField, "i");
                        break;

                    case Keys.J:
                        if (shift)
                            m_TextField = string.Concat(m_TextField, "J");
                        else
                            m_TextField = string.Concat(m_TextField, "j");
                        break;

                    case Keys.K:
                        if (shift)
                            m_TextField = string.Concat(m_TextField, "K");
                        else
                            m_TextField = string.Concat(m_TextField, "k");
                        break;

                    case Keys.L:
                        if (shift)
                            m_TextField = string.Concat(m_TextField, "L");
                        else
                            m_TextField = string.Concat(m_TextField, "l");
                        break;

                    case Keys.M:
                        if (shift)
                            m_TextField = string.Concat(m_TextField, "M");
                        else
                            m_TextField = string.Concat(m_TextField, "m");
                        break;

                    case Keys.N:
                        if (shift)
                            m_TextField = string.Concat(m_TextField, "N");
                        else
                            m_TextField = string.Concat(m_TextField, "n");
                        break;

                    case Keys.O:
                        if (shift)
                            m_TextField = string.Concat(m_TextField, "O");
                        else
                            m_TextField = string.Concat(m_TextField, "o");
                        break;

                    case Keys.P:
                        if (shift)
                            m_TextField = string.Concat(m_TextField, "P");
                        else
                            m_TextField = string.Concat(m_TextField, "p");
                        break;

                    case Keys.Q:
                        if (shift)
                            m_TextField = string.Concat(m_TextField, "Q");
                        else
                            m_TextField = string.Concat(m_TextField, "q");
                        break;

                    case Keys.R:
                        if (shift)
                            m_TextField = string.Concat(m_TextField, "R");
                        else
                            m_TextField = string.Concat(m_TextField, "r");
                        break;

                    case Keys.S:
                        if (shift)
                            m_TextField = string.Concat(m_TextField, "S");
                        else
                            m_TextField = string.Concat(m_TextField, "s");
                        break;

                    case Keys.T:
                        if (shift)
                            m_TextField = string.Concat(m_TextField, "T");
                        else
                            m_TextField = string.Concat(m_TextField, "t");
                        break;

                    case Keys.U:
                        if (shift)
                            m_TextField = string.Concat(m_TextField, "U");
                        else
                            m_TextField = string.Concat(m_TextField, "u");
                        break;

                    case Keys.V:
                        if (shift)
                            m_TextField = string.Concat(m_TextField, "V");
                        else
                            m_TextField = string.Concat(m_TextField, "v");
                        break;

                    case Keys.W:
                        if (shift)
                            m_TextField = string.Concat(m_TextField, "W");
                        else
                            m_TextField = string.Concat(m_TextField, "w");
                        break;

                    case Keys.X:
                        if (shift)
                            m_TextField = string.Concat(m_TextField, "X");
                        else
                            m_TextField = string.Concat(m_TextField, "x");
                        break;

                    case Keys.Y:
                        if (shift)
                            m_TextField = string.Concat(m_TextField, "Y");
                        else
                            m_TextField = string.Concat(m_TextField, "y");
                        break;

                    case Keys.Z:
                        if (shift)
                            m_TextField = string.Concat(m_TextField, "Z");
                        else
                            m_TextField = string.Concat(m_TextField, "z");
                        break;

                    case Keys.D0:
                        if (shift)
                            m_TextField = string.Concat(m_TextField, ")");
                        else
                            m_TextField = string.Concat(m_TextField, "0");
                        break;

                    case Keys.D1:
                        if (shift)
                            m_TextField = string.Concat(m_TextField, "!");
                        else
                            m_TextField = string.Concat(m_TextField, "1");
                        break;

                    case Keys.D2:
                        if (shift)
                            m_TextField = string.Concat(m_TextField, "@");
                        else
                            m_TextField = string.Concat(m_TextField, "2");
                        break;

                    case Keys.D3:
                        if (shift)
                            m_TextField = string.Concat(m_TextField, "#");
                        else
                            m_TextField = string.Concat(m_TextField, "3");
                        break;

                    case Keys.D4:
                        if (shift)
                            m_TextField = string.Concat(m_TextField, "$");
                        else
                            m_TextField = string.Concat(m_TextField, "4");
                        break;

                    case Keys.D5:
                        if (shift)
                            m_TextField = string.Concat(m_TextField, "%");
                        else
                            m_TextField = string.Concat(m_TextField, "5");
                        break;

                    case Keys.D6:
                        if (shift)
                            m_TextField = string.Concat(m_TextField, "^");
                        else
                            m_TextField = string.Concat(m_TextField, "6");
                        break;

                    case Keys.D7:
                        if (shift)
                            m_TextField = string.Concat(m_TextField, "&");
                        else
                            m_TextField = string.Concat(m_TextField, "7");
                        break;

                    case Keys.D8:
                        if (shift)
                            m_TextField = string.Concat(m_TextField, "*");
                        else
                            m_TextField = string.Concat(m_TextField, "8");
                        break;

                    case Keys.D9:
                        if (shift)
                            m_TextField = string.Concat(m_TextField, "(");
                        else
                            m_TextField = string.Concat(m_TextField, "9");
                        break;

                    case Keys.OemPeriod:
                        if (shift)
                            m_TextField = string.Concat(m_TextField, ">");
                        else
                            m_TextField = string.Concat(m_TextField, ".");
                        break;

                    case Keys.OemComma:
                        if (shift)
                            m_TextField = string.Concat(m_TextField, "<");
                        else
                            m_TextField = string.Concat(m_TextField, ",");
                        break;

                    case Keys.OemQuotes:
                        if (shift)
                            m_TextField = string.Concat(m_TextField, "\"");
                        else
                            m_TextField = string.Concat(m_TextField, "'");
                        break;

                    case Keys.OemMinus:
                        if (shift)
                            m_TextField = string.Concat(m_TextField, "_");
                        else
                            m_TextField = string.Concat(m_TextField, "-");
                        break;
                    case Keys.Space:
                        m_TextField = string.Concat(m_TextField, " ");
                        break;
                }
            }
            else if (m_TextFieldType == TextFieldType.Float)
            {
                switch (key)
                {
                    case Keys.D0:
                        m_TextField = string.Concat(m_TextField, "0");
                        m_NumberValue = float.Parse(m_TextField);
                        break;

                    case Keys.D1:
                        m_TextField = string.Concat(m_TextField, "1");
                        m_NumberValue = float.Parse(m_TextField);
                        break;

                    case Keys.D2:
                        m_TextField = string.Concat(m_TextField, "2");
                        m_NumberValue = float.Parse(m_TextField);
                        break;

                    case Keys.D3:
                        m_TextField = string.Concat(m_TextField, "3");
                        m_NumberValue = float.Parse(m_TextField);
                        break;

                    case Keys.D4:
                        m_TextField = string.Concat(m_TextField, "4");
                        m_NumberValue = float.Parse(m_TextField);
                        break;

                    case Keys.D5:
                        m_TextField = string.Concat(m_TextField, "5");
                        m_NumberValue = float.Parse(m_TextField);
                        break;

                    case Keys.D6:
                        m_TextField = string.Concat(m_TextField, "6");
                        m_NumberValue = float.Parse(m_TextField);
                        break;

                    case Keys.D7:
                        m_TextField = string.Concat(m_TextField, "7");
                        m_NumberValue = float.Parse(m_TextField);
                        break;

                    case Keys.D8:
                        m_TextField = string.Concat(m_TextField, "8");
                        m_NumberValue = float.Parse(m_TextField);
                        break;

                    case Keys.D9:
                        m_TextField = string.Concat(m_TextField, "9");
                        m_NumberValue = float.Parse(m_TextField);
                        break;

                    case Keys.OemPeriod:
                        if (!m_TextField.Contains(".") && m_TextField.Length > 0)
                        {
                            m_TextField = string.Concat(m_TextField, ".");
                            m_NumberValue = float.Parse(m_TextField);
                        }
                        break;
                }
            }

            else if (m_TextFieldType == TextFieldType.Int)
            {
                switch (key)
                {
                    case Keys.D0:
                        m_TextField = string.Concat(m_TextField, "0");
                        m_NumberValue = float.Parse(m_TextField);
                        break;

                    case Keys.D1:
                        m_TextField = string.Concat(m_TextField, "1");
                        m_NumberValue = float.Parse(m_TextField);
                        break;

                    case Keys.D2:
                        m_TextField = string.Concat(m_TextField, "2");
                        m_NumberValue = float.Parse(m_TextField);
                        break;

                    case Keys.D3:
                        m_TextField = string.Concat(m_TextField, "3");
                        m_NumberValue = float.Parse(m_TextField);
                        break;

                    case Keys.D4:
                        m_TextField = string.Concat(m_TextField, "4");
                        m_NumberValue = float.Parse(m_TextField);
                        break;

                    case Keys.D5:
                        m_TextField = string.Concat(m_TextField, "5");
                        m_NumberValue = float.Parse(m_TextField);
                        break;

                    case Keys.D6:
                        m_TextField = string.Concat(m_TextField, "6");
                        m_NumberValue = float.Parse(m_TextField);
                        break;

                    case Keys.D7:
                        m_TextField = string.Concat(m_TextField, "7");
                        m_NumberValue = float.Parse(m_TextField);
                        break;

                    case Keys.D8:
                        m_TextField = string.Concat(m_TextField, "8");
                        m_NumberValue = float.Parse(m_TextField);
                        break;

                    case Keys.D9:
                        m_TextField = string.Concat(m_TextField, "9");
                        m_NumberValue = float.Parse(m_TextField);
                        break;
                }
            }
        }
	}
}
