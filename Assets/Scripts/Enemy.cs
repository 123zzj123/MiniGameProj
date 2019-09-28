using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;


namespace MiniProj
{
	public class Enemy : MonoBehaviour
	{
		//[SerializeField]
		public int EnemyType;     //���˵�����
		public int PosIsChange;   //λ���ڱ����Ƿ��Ѿ��ı����
		public MapPos m_EnemyPosOld;   //�ϵ�λ��
		public MapPos m_EnemyPosNew;	//�µ�λ��

		private static float DiffX = 3.5f;
        private static float DiffZ = 5.0f;

		private void Awake()
		{
		}

		public void SetType(int iType)
		{
			EnemyType = iType;
		}

        public void DestroyObj()
        {
            GameObject.Destroy(this.gameObject);
        }

        public void SetStartPos(int row, int col)
		{
			m_EnemyPosOld.m_row = row;
			m_EnemyPosOld.m_col = col;

			m_EnemyPosNew.m_row = row;
			m_EnemyPosNew.m_col = col;

			transform.position = new Vector3(col * DiffX, 1.6f, row * DiffZ);
		}

		public void MovePos(int row, int col)
		{
			m_EnemyPosOld.m_row = m_EnemyPosNew.m_row;
			m_EnemyPosOld.m_col = m_EnemyPosNew.m_col;

			m_EnemyPosNew.m_row = row;
			m_EnemyPosNew.m_col = col;
		}

		public void Update()
		{
			transform.position = new Vector3(m_EnemyPosNew.m_col * DiffX, 1.6f, m_EnemyPosNew.m_row * DiffZ);
		}

		//�����ȣ������õ�
		private bool PosExitChess(int row, int col, bool YuJiExist)
        {
            SceneModule _sceneModule = (SceneModule)GameManager.GameManagerObj.GetComponent<GameManager>().GetModuleByName("SceneModule");
			if(_sceneModule.m_enemyList[row][col] != null )
			{
				return true;
			}

            MapPos _pos = new MapPos();
            _sceneModule.GetPlayerPos(ref _pos);

            if (row == _pos.m_row && col == _pos.m_col)
			{
				return true;
			}

            if (YuJiExist && _sceneModule.YuJiPos.m_row == row && _sceneModule.YuJiPos.m_col == col)
            {
                return true;
            }

			return false;
		}

        //�����ܷ��ߵ�ʱ������ж��ݼ��Ƿ�ס��
        //���˳Ե�ʱ�򣬳��ݼ��������ȼ�һ���ߣ����ǿ����ݼ�
        //1 �����ӣ� 2�Ը��ӣ� 0��û����
		public int GetEnemyNextPos(bool YuJiExist)
        {
        	SceneModule _sceneModule = (SceneModule)GameManager.GameManagerObj.GetComponent<GameManager>().GetModuleByName("SceneModule");
			//�ж�,�Ƿ��ǿ��ߵĵ�
            int _mapRow = _sceneModule.m_config.SceneConfigList[GameManager.SceneConfigId].MapRow;
            int _mapCol = _sceneModule.m_config.SceneConfigList[GameManager.SceneConfigId].MapCol;
			
			int DistancePlayer = -1;
			//��ǰ��������ĵ�
			int minrow = 0;
			int mincol = 0;

            //����Ҫ�ߵ�λ��
			int temprow;
			int tempcol;

            MapPos _pos = new MapPos();
            _sceneModule.GetPlayerPos(ref _pos);

            //������
            int PRow = -1;
            int PCol = -1;

            //�����ӣ����ݼ�ʱ�������
            int SecondRow = -1;
            int SecondCol = -1;

            int RetValue = 0;

            if (YuJiExist)
            {
                PRow = _sceneModule.YuJiPos.m_row;
                PCol = _sceneModule.YuJiPos.m_col;

                SecondRow = _pos.m_row;
                SecondCol = _pos.m_col;
            }
            else
            {
                //��player��λ��
                PRow = _pos.m_row;
			    PCol = _pos.m_col;
            }


			//enemy��ǰ��λ��
			int playerRow = m_EnemyPosNew.m_row;
			int playerCol = m_EnemyPosNew.m_col;
			
            switch (EnemyType)
            {
            	//��
                case 1:
                    if(playerRow >= 1)
                    {
                        if(playerCol >= 2)
                        {
                            if(_sceneModule.m_mapData[playerRow - 1][playerCol - 2] != MapDataType.NONE
                                && (_sceneModule.m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (_sceneModule.m_mapData[playerRow][playerCol - 1] != MapDataType.GAOTAI)))
                            {
                            	if(PosExitChess(playerRow,playerCol - 1, YuJiExist) == false)
                            	{
                            		temprow = playerRow - 1;
									tempcol = playerCol - 2;

                                    //�ܷ�Ը���
                                    if (YuJiExist && temprow == SecondRow && tempcol == SecondCol)
                                    {
                                        RetValue = 2;
                                        DistancePlayer = (PRow - temprow) * (PRow - temprow) + (tempcol - PCol) * (tempcol - PCol);
                                        minrow = temprow;
                                        mincol = tempcol;
                                        break;
                                    }
									else if(DistancePlayer == -1)
									{
                                        //��һ������ĵ�
										DistancePlayer = (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol);
										minrow = temprow;
										mincol = tempcol;
									}
									else if(DistancePlayer > (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol))
									{
										DistancePlayer = (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol);
										minrow = temprow;
										mincol = tempcol;
									}
								}
                            	
                            }
                        }
                        if(playerCol + 2 < _mapCol)
                        {
                            if(_sceneModule.m_mapData[playerRow - 1][playerCol + 2] != MapDataType.NONE
                                && (_sceneModule.m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (_sceneModule.m_mapData[playerRow][playerCol + 1] != MapDataType.GAOTAI)))
                            {
                                if(PosExitChess(playerRow,playerCol + 1, YuJiExist) == false)
								{
									temprow = playerRow - 1;
									tempcol = playerCol + 2;
                                    //��һ������ĵ�
                                    //�ܷ�Ը���
                                    if (YuJiExist && temprow == SecondRow && tempcol == SecondCol)
                                    {
                                        RetValue = 2;
                                        DistancePlayer = (PRow - temprow) * (PRow - temprow) + (tempcol - PCol) * (tempcol - PCol);
                                        minrow = temprow;
                                        mincol = tempcol;
                                        break;
                                    }
                                    else if (DistancePlayer == -1)
									{
										DistancePlayer = (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol);
										minrow = temprow;
										mincol = tempcol;
									}
									else if(DistancePlayer > (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol))
									{
										DistancePlayer = (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol);
										minrow = temprow;
										mincol = tempcol;
									}
								}
                            }
                        }
                    }
                    if(playerRow >= 2)
                    {
                        if (playerCol >= 1)
                        {
                            if (_sceneModule.m_mapData[playerRow - 2][playerCol - 1] != MapDataType.NONE
                                && (_sceneModule.m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (_sceneModule.m_mapData[playerRow - 1][playerCol] != MapDataType.GAOTAI)))
                            {
                                if(PosExitChess(playerRow - 1,playerCol, YuJiExist) == false)
								{
									temprow = playerRow - 2;
									tempcol = playerCol - 1;
                                    //��һ������ĵ�
                                    //�ܷ�Ը���
                                    if (YuJiExist && temprow == SecondRow && tempcol == SecondCol)
                                    {
                                        RetValue = 2;
                                        DistancePlayer = (PRow - temprow) * (PRow - temprow) + (tempcol - PCol) * (tempcol - PCol);
                                        minrow = temprow;
                                        mincol = tempcol;
                                        break;
                                    }
                                    else if (DistancePlayer == -1)
									{
										DistancePlayer = (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol);
										minrow = temprow;
										mincol = tempcol;
									}
									else if(DistancePlayer > (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol))
									{
										DistancePlayer = (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol);
										minrow = temprow;
										mincol = tempcol;
									}
								}
                            }
                        }
                        if (playerCol + 1 < _mapCol)
                        {
                            if (_sceneModule.m_mapData[playerRow - 2][playerCol + 1] != MapDataType.NONE
                                && (_sceneModule.m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (_sceneModule.m_mapData[playerRow - 1][playerCol] != MapDataType.GAOTAI)))
                            {
								if(PosExitChess(playerRow - 1,playerCol, YuJiExist) == false)
								{
									temprow = playerRow - 2;
									tempcol = playerCol + 1;
                                    //��һ������ĵ�
                                    //�ܷ�Ը���
                                    if (YuJiExist && temprow == SecondRow && tempcol == SecondCol)
                                    {
                                        RetValue = 2;
                                        DistancePlayer = (PRow - temprow) * (PRow - temprow) + (tempcol - PCol) * (tempcol - PCol);
                                        minrow = temprow;
                                        mincol = tempcol;
                                        break;
                                    }
                                    else if (DistancePlayer == -1)
									{
										DistancePlayer = (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol);
										minrow = temprow;
										mincol = tempcol;
									}
									else if(DistancePlayer > (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol))
									{
										DistancePlayer = (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol);
										minrow = temprow;
										mincol = tempcol;
									}
								}
                            }
                        }
                    }
                    if(playerRow + 1 < _mapRow)
                    {
                        if(playerCol >= 2)
                        {
                            if (_sceneModule.m_mapData[playerRow + 1][playerCol - 2] != MapDataType.NONE
                                && (_sceneModule.m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (_sceneModule.m_mapData[playerRow][playerCol - 1] != MapDataType.GAOTAI)))
                            {
                                if(PosExitChess(playerRow,playerCol - 1, YuJiExist) == false)
								{
									temprow = playerRow + 1;
									tempcol = playerCol - 2;
                                    //��һ������ĵ�
                                    //�ܷ�Ը���
                                    if (YuJiExist && temprow == SecondRow && tempcol == SecondCol)
                                    {
                                        RetValue = 2;
                                        DistancePlayer = (PRow - temprow) * (PRow - temprow) + (tempcol - PCol) * (tempcol - PCol);
                                        minrow = temprow;
                                        mincol = tempcol;
                                        break;
                                    }
                                    else if (DistancePlayer == -1)
									{
										DistancePlayer = (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol);
										minrow = temprow;
										mincol = tempcol;
									}
									else if(DistancePlayer > (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol))
									{
										DistancePlayer = (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol);
										minrow = temprow;
										mincol = tempcol;
									}
								}
                            }
                        }
                        if (playerCol + 1 < _mapCol)
                        {
                            if (_sceneModule.m_mapData[playerRow + 1][playerCol + 2] != MapDataType.NONE
                                && (_sceneModule.m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (_sceneModule.m_mapData[playerRow][playerCol + 1] != MapDataType.GAOTAI)))
                            {
                                if(PosExitChess(playerRow,playerCol + 1, YuJiExist) == false)
								{
									temprow = playerRow + 1;
									tempcol = playerCol + 2;
                                    //��һ������ĵ�
                                    //�ܷ�Ը���
                                    if (YuJiExist && temprow == SecondRow && tempcol == SecondCol)
                                    {
                                        RetValue = 2;
                                        DistancePlayer = (PRow - temprow) * (PRow - temprow) + (tempcol - PCol) * (tempcol - PCol);
                                        minrow = temprow;
                                        mincol = tempcol;
                                        break;
                                    }
                                    else if (DistancePlayer == -1)
									{
										DistancePlayer = (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol);
										minrow = temprow;
										mincol = tempcol;
									}
									else if(DistancePlayer > (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol))
									{
										DistancePlayer = (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol);
										minrow = temprow;
										mincol = tempcol;
									}
								}
                            }
                        }
                    }
                    if(playerRow + 2 < _mapRow)
                    {
                        if (playerCol >= 1)
                        {
                            if (_sceneModule.m_mapData[playerRow + 2][playerCol - 1] != MapDataType.NONE
                                && (_sceneModule.m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (_sceneModule.m_mapData[playerRow + 1][playerCol] != MapDataType.GAOTAI)))
                            {
                                if(PosExitChess(playerRow + 1,playerCol, YuJiExist) == false)
								{
									temprow = playerRow + 2;
									tempcol = playerCol - 1;
                                    //��һ������ĵ�
                                    //�ܷ�Ը���
                                    if (YuJiExist && temprow == SecondRow && tempcol == SecondCol)
                                    {
                                        RetValue = 2;
                                        DistancePlayer = (PRow - temprow) * (PRow - temprow) + (tempcol - PCol) * (tempcol - PCol);
                                        minrow = temprow;
                                        mincol = tempcol;
                                        break;
                                    }
                                    else if (DistancePlayer == -1)
									{
										DistancePlayer = (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol);
										minrow = temprow;
										mincol = tempcol;
									}
									else if(DistancePlayer > (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol))
									{
										DistancePlayer = (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol);
										minrow = temprow;
										mincol = tempcol;
									}
								}
                            }
                        }
                        if (playerCol + 1 < _mapCol)
                        {
                            if (_sceneModule.m_mapData[playerRow + 2][playerCol + 1] != MapDataType.NONE
                                && (_sceneModule.m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (_sceneModule.m_mapData[playerRow + 1][playerCol] != MapDataType.GAOTAI)))
                            {
                                if(PosExitChess(playerRow + 1,playerCol, YuJiExist) == false)
								{
									temprow = playerRow + 2;
									tempcol = playerCol + 1;
                                    //��һ������ĵ�
                                    //�ܷ�Ը���
                                    if (YuJiExist && temprow == SecondRow && tempcol == SecondCol)
                                    {
                                        RetValue = 2;
                                        DistancePlayer = (PRow - temprow) * (PRow - temprow) + (tempcol - PCol) * (tempcol - PCol);
                                        minrow = temprow;
                                        mincol = tempcol;
                                        break;
                                    }
                                    else if (DistancePlayer == -1)
									{
										DistancePlayer = (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol);
										minrow = temprow;
										mincol = tempcol;
									}
									else if(DistancePlayer > (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol))
									{
										DistancePlayer = (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol);
										minrow = temprow;
										mincol = tempcol;
									}
								}
                            }
                        }
                    }
                    break;
                case 2:
                    if(playerRow >= 2)
                    {
                        if(playerCol >= 2)
                        {
                            if (_sceneModule.m_mapData[playerRow - 2][playerCol - 2] != MapDataType.NONE
                                && (_sceneModule.m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (_sceneModule.m_mapData[playerRow - 1][playerCol - 1] != MapDataType.GAOTAI)))
                            {
                                if(PosExitChess(playerRow - 1,playerCol -1, YuJiExist) == false)
								{
									temprow = playerRow - 2;
									tempcol = playerCol - 2;
                                    //��һ������ĵ�
                                    //�ܷ�Ը���
                                    if (YuJiExist && temprow == SecondRow && tempcol == SecondCol)
                                    {
                                        RetValue = 2;
                                        DistancePlayer = (PRow - temprow) * (PRow - temprow) + (tempcol - PCol) * (tempcol - PCol);
                                        minrow = temprow;
                                        mincol = tempcol;
                                        break;
                                    }
                                    else if (DistancePlayer == -1)
									{
										DistancePlayer = (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol);
										minrow = temprow;
										mincol = tempcol;
									}
									else if(DistancePlayer > (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol))
									{
										DistancePlayer = (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol);
										minrow = temprow;
										mincol = tempcol;
									}
								}
                            }
                        }
                        if(playerCol + 2 < _mapCol)
                        {
                            if (_sceneModule.m_mapData[playerRow - 2][playerCol + 2] != MapDataType.NONE
                                && (_sceneModule.m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (_sceneModule.m_mapData[playerRow - 1][playerCol + 1] != MapDataType.GAOTAI)))
                            {
								if(PosExitChess(playerRow - 1,playerCol + 1, YuJiExist) == false)
								{
									temprow = playerRow - 2;
									tempcol = playerCol + 2;
                                    //��һ������ĵ�
                                    //�ܷ�Ը���
                                    if (YuJiExist && temprow == SecondRow && tempcol == SecondCol)
                                    {
                                        RetValue = 2;
                                        DistancePlayer = (PRow - temprow) * (PRow - temprow) + (tempcol - PCol) * (tempcol - PCol);
                                        minrow = temprow;
                                        mincol = tempcol;
                                        break;
                                    }
                                    else if (DistancePlayer == -1)
									{
										DistancePlayer = (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol);
										minrow = temprow;
										mincol = tempcol;
									}
									else if(DistancePlayer > (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol))
									{
										DistancePlayer = (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol);
										minrow = temprow;
										mincol = tempcol;
									}
								}

                            }
                        }
                    }
                    if(playerRow + 2 < _mapCol)
                    {
                        if (playerCol >= 2)
                        {
                            if (_sceneModule.m_mapData[playerRow + 2][playerCol - 2] != MapDataType.NONE
                                && (_sceneModule.m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (_sceneModule.m_mapData[playerRow + 1][playerCol - 1] != MapDataType.GAOTAI)))
                            {
								if(PosExitChess(playerRow + 1,playerCol -1, YuJiExist) == false)
								{
									temprow = playerRow + 2;
									tempcol = playerCol - 2;
                                    //��һ������ĵ�
                                    //�ܷ�Ը���
                                    if (YuJiExist && temprow == SecondRow && tempcol == SecondCol)
                                    {
                                        RetValue = 2;
                                        DistancePlayer = (PRow - temprow) * (PRow - temprow) + (tempcol - PCol) * (tempcol - PCol);
                                        minrow = temprow;
                                        mincol = tempcol;
                                        break;
                                    }
                                    else if (DistancePlayer == -1)
									{
										DistancePlayer = (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol);
										minrow = temprow;
										mincol = tempcol;
									}
									else if(DistancePlayer > (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol))
									{
										DistancePlayer = (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol);
										minrow = temprow;
										mincol = tempcol;
									}
								}

                            }
                        }
                        if (playerCol + 2 < _mapCol)
                        {
                            if (_sceneModule.m_mapData[playerRow + 2][playerCol + 2] != MapDataType.NONE
                                && (_sceneModule.m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (_sceneModule.m_mapData[playerRow + 1][playerCol + 1] != MapDataType.GAOTAI)))
                            {
								if(PosExitChess(playerRow + 1,playerCol + 1, YuJiExist) == false)
								{
									temprow = playerRow + 2;
									tempcol = playerCol + 2;
                                    //��һ������ĵ�
                                    //�ܷ�Ը���
                                    if (YuJiExist && temprow == SecondRow && tempcol == SecondCol)
                                    {
                                        RetValue = 2;
                                        DistancePlayer = (PRow - temprow) * (PRow - temprow) + (tempcol - PCol) * (tempcol - PCol);
                                        minrow = temprow;
                                        mincol = tempcol;
                                        break;
                                    }
                                    else if (DistancePlayer == -1)
									{
										DistancePlayer = (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol);
										minrow = temprow;
										mincol = tempcol;
									}
									else if(DistancePlayer > (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol))
									{
										DistancePlayer = (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol);
										minrow = temprow;
										mincol = tempcol;
									}
								}

                            }
                        }
                    }
                    break;
                case 3:
						if(playerCol >= 1 && _sceneModule.m_mapData[playerRow][playerCol - 1] != MapDataType.GAOTAI && _sceneModule.m_mapData[playerRow][playerCol - 1] != MapDataType.NONE && _sceneModule.m_enemyList[playerRow][playerCol - 1] == null)
						{
							temprow = playerRow;
							tempcol = playerCol - 1;
                        //��һ������ĵ�
                        //�ܷ�Ը���
                            if (YuJiExist && temprow == SecondRow && tempcol == SecondCol)
                            {
                                RetValue = 2;
                                DistancePlayer = (PRow - temprow) * (PRow - temprow) + (tempcol - PCol) * (tempcol - PCol);
                                minrow = temprow;
                                mincol = tempcol;
                                break;
                            }
                            else if (DistancePlayer == -1)
							{
								DistancePlayer = (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol);
								minrow = temprow;
								mincol = tempcol;
							}
							else if(DistancePlayer > (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol))
							{
								DistancePlayer = (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol);
								minrow = temprow;
								mincol = tempcol;
							}
						}
						
						if(playerCol + 1 < _mapCol && _sceneModule.m_mapData[playerRow][playerCol + 1] != MapDataType.GAOTAI && _sceneModule.m_mapData[playerRow][playerCol + 1] != MapDataType.NONE && _sceneModule.m_enemyList[playerRow][playerCol + 1] == null)
						{
							temprow = playerRow;
							tempcol = playerCol + 1;
                        //��һ������ĵ�
                        //�ܷ�Ը���
                            if (YuJiExist && temprow == SecondRow && tempcol == SecondCol)
                            {
                                RetValue = 2;
                                DistancePlayer = (PRow - temprow) * (PRow - temprow) + (tempcol - PCol) * (tempcol - PCol);
                                minrow = temprow;
                                mincol = tempcol;
                                break;
                            }
                            else if (DistancePlayer == -1)
							{
								DistancePlayer = (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol);
								minrow = temprow;
								mincol = tempcol;
							}
							else if(DistancePlayer > (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol))
							{
								DistancePlayer = (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol);
								minrow = temprow;
								mincol = tempcol;
							}

						}

						
						if(playerRow >= 1 && _sceneModule.m_mapData[playerRow - 1][playerCol] != MapDataType.GAOTAI && _sceneModule.m_mapData[playerRow - 1][playerCol] != MapDataType.NONE && _sceneModule.m_enemyList[playerRow - 1][playerCol] == null)
						{
							temprow = playerRow - 1;
							tempcol = playerCol;
                        //��һ������ĵ�
                        //�ܷ�Ը���
                            if (YuJiExist && temprow == SecondRow && tempcol == SecondCol)
                            {
                                RetValue = 2;
                                DistancePlayer = (PRow - temprow) * (PRow - temprow) + (tempcol - PCol) * (tempcol - PCol);
                                minrow = temprow;
                                mincol = tempcol;
                                break;
                            }
                            else if (DistancePlayer == -1)
							{
								DistancePlayer = (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol);
								minrow = temprow;
								mincol = tempcol;
							}
							else if(DistancePlayer > (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol))
							{
								DistancePlayer = (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol);
								minrow = temprow;
								mincol = tempcol;
							}

						}

						if(playerRow + 1 < _mapRow && _sceneModule.m_mapData[playerRow + 1][playerCol] != MapDataType.GAOTAI && _sceneModule.m_mapData[playerRow + 1][playerCol] != MapDataType.NONE && _sceneModule.m_enemyList[playerRow + 1][playerCol] == null)
						{
							temprow = playerRow + 1;
							tempcol = playerCol;
                            //��һ������ĵ�
                            //�ܷ�Ը���
                            if (YuJiExist && temprow == SecondRow && tempcol == SecondCol)
                            {
                                RetValue = 2;
                                DistancePlayer = (PRow - temprow) * (PRow - temprow) + (tempcol - PCol) * (tempcol - PCol);
                                minrow = temprow;
                                mincol = tempcol;
                                break;
                            }
                            else if (DistancePlayer == -1)
							{
								DistancePlayer = (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol);
								minrow = temprow;
								mincol = tempcol;
							}
							else if(DistancePlayer > (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol))
							{
								DistancePlayer = (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol);
								minrow = temprow;
								mincol = tempcol;
							}

						}
						
                    break;
            }

            //Debug.Log(string.Format("minx:{0}, miny:{1}",minrow, mincol));
            if (DistancePlayer == -1)
            {
                //û�е������
                MovePos(m_EnemyPosNew.m_row, m_EnemyPosNew.m_col);
            }
            else
            {
                _sceneModule.m_enemyList[minrow][mincol] = _sceneModule.m_enemyList[m_EnemyPosNew.m_row][m_EnemyPosNew.m_col];
                _sceneModule.m_enemyList[m_EnemyPosNew.m_row][m_EnemyPosNew.m_col] = null;
                MovePos(minrow, mincol);
                if (RetValue == 2)
                {
                    return 2;
                }
                else if (DistancePlayer == 0)
                {
                    return 1;
                }
            }

            return 0;

        }
		
	}

}


