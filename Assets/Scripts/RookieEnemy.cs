﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniProj
{
    public class RookieEnemy : MonoBehaviour
    {
        private static float DiffX = 3.5f;
        private static float DiffZ = 5.0f;

        private int m_id;
        private MapPos m_playerPos;

        public void SetPosition(int row, int col)
        {
            m_playerPos.m_row = row;
            m_playerPos.m_col = col;
            transform.position = new Vector3(col * DiffX, 1f, row * DiffZ);
        }

        public void Execute()
        {
            SetPosition(m_playerPos.m_row - 1, m_playerPos.m_col);
        }
        public void SetId(int id)
        {
            m_id = id;
        }
    }

}