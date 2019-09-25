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
		
	}

}


