//-----------------------------------------------------------------------------
// Camera Singleton that for now, doesn't do much.
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

namespace itp380
{
	public class Camera
	{
		Game m_Game;
		Vector3 m_vEye = new Vector3(0, 0f, 15f);
		Vector3 m_vTarget = Vector3.Zero;

        GameObject m_FollowObject = null;
        public GameObject FollowObject
        {
            get
            {
                return m_FollowObject;
            }
            set
            {
                m_FollowObject = value;
               // m_vTarget = m_FollowObject.Position;
            }
        }
		
		Matrix m_Camera;
		public Matrix CameraMatrix
		{
			get { return m_Camera; }
		}

		public Camera(Game game)
		{
			m_Game = game;
			ComputeMatrix();
		}

		public void Update(float fDeltaTime)
		{
            if (m_FollowObject != null)
            {
                m_vTarget = m_FollowObject.Position;
                ComputeMatrix();
            }
		}

		void ComputeMatrix()
		{
            Vector3 vTarget = new Vector3(m_vTarget.X * 0.3f, m_vTarget.Y * 0.3f, m_vTarget.Z);
            Vector3 vEye = vTarget + m_vEye;
			Vector3 vUp = Vector3.Cross(Vector3.Zero - vEye, Vector3.Left);
			m_Camera = Matrix.CreateLookAt(vEye, vTarget, Vector3.Up);
		}
	}
}
