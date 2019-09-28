﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

namespace MiniProj
{
    public class FixedRouteNpc : MonoBehaviour
    {
        private static float DiffX = 3.5f;
        private static float DiffZ = 5.0f;
        private MapPos m_playerPos;
        public MapPos[] m_routePosList =
        {
            new MapPos(0, 2), new MapPos(1, 2), new MapPos(2, 2), new MapPos(2, 3), new MapPos(2, 4),
            new MapPos(2 ,5), new MapPos(3, 5), new MapPos(4, 5),
        };

        public int m_stepPerRound = 2;
        public int m_curStep = 0;
        private int m_curRoundStep = 0;

        private void Awake()
        {
            EventManager.RegisterEvent(HLEventId.PLAYER_END_MOVE, this.GetHashCode(), FollowPlayer);
        }

        public void DestroyObj()
        {
            GameObject.Destroy(this.gameObject);
        }

        public void SetPosition(int row, int col)
        {
            m_playerPos.m_row = row;
            m_playerPos.m_col = col;
            transform.position = new Vector3(row * DiffX, 1f, col * DiffZ);
        }

        private void FollowPlayer(EventArgs args)
        {
            DoOneStep();
        }

        private void DoOneStep()
        {
            if(m_curStep == m_routePosList.Length)
            {
                m_curRoundStep = 0;
                EventManager.SendEvent(HLEventId.NPC_END_MOVE, null);
            }
            SceneModule _sceneModule = (SceneModule)GameManager.GameManagerObj.GetComponent<GameManager>().GetModuleByName("SceneModule");
            if (m_curRoundStep < m_stepPerRound - 1)
            {
                MapPos _playPos = new MapPos();
                _sceneModule.GetPlayerPos(ref _playPos);
                int _col = m_routePosList[m_curStep].m_col;
                int _row = m_routePosList[m_curStep].m_row;
                if((_col != _playPos.m_col || _row != _playPos.m_row) && _sceneModule.m_enemyList[_row][_col] == null)
                {
                    _sceneModule.m_npcList[_row][_col] = _sceneModule.m_npcList[m_playerPos.m_row][m_playerPos.m_col];
                    _sceneModule.m_npcList[m_playerPos.m_row][m_playerPos.m_col] = null;
                    m_playerPos.m_row = _row;
                    m_playerPos.m_col = _col;
                    Sequence _sequence = DOTween.Sequence();
                    _sequence.Append(transform.DOMove(new Vector3(_row * DiffX, 1f, _col * DiffZ), 2));
                    _sequence.onComplete += DoOneStep;
                    _sequence.SetAutoKill(true);
                    ++m_curRoundStep;
                    ++m_curStep;
                }
                else
                {
                    m_curRoundStep = 0;
                    EventManager.SendEvent(HLEventId.NPC_END_MOVE, null);                  
                }
                
            }
            else
            {
                MapPos _playPos = new MapPos();
                _sceneModule.GetPlayerPos(ref _playPos);
                int _col = m_routePosList[m_curStep].m_col;
                int _row = m_routePosList[m_curStep].m_row;
                if ((_col != _playPos.m_col || _row != _playPos.m_row) && _sceneModule.m_enemyList[_row][_col] == null)
                {
                    _sceneModule.m_npcList[_row][_col] = _sceneModule.m_npcList[m_playerPos.m_row][m_playerPos.m_col];
                    _sceneModule.m_npcList[m_playerPos.m_row][m_playerPos.m_col] = null;
                    m_playerPos.m_row = _row;
                    m_playerPos.m_col = _col;
                    m_curRoundStep = 0;
                    Sequence _sequence = DOTween.Sequence();
                    _sequence.Append(transform.DOMove(new Vector3(_row * DiffX, 1f, _col * DiffZ), 2));
                    _sequence.SetAutoKill(true);
                    _sequence.onComplete += NpcEndMoveCallBack;
                    ++m_curStep;
                }
                else
                {
                    m_curRoundStep = 0;
                    EventManager.SendEvent(HLEventId.NPC_END_MOVE, null);
                }
                
            }
            
        }

        private void NpcEndMoveCallBack()
        {
            EventManager.SendEvent(HLEventId.NPC_END_MOVE, null);
        }
    }

    

}
