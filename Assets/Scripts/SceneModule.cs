﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;


namespace MiniProj
{
    public enum ChessType
    {
		MA = 1,
		XIANG = 2,
		SHI = 3,
		JU = 4,
		PAO = 5,
    }

    //人
    public enum PlayerType
    {
        XIANGYU = 1,
        YUJI = 2,
        ENEMY = 3,
    }
    public class SceneModule : Module
    {
        /*
			MA = 1,
			XIANG = 2,
			SHI = 3,
			JU = 4,
			PAO = 5,
		*/
        private int m_SceneStep;
        private int m_waitCount;
        private int m_npcCount;
        private GameObject m_sceneObj;
        private GameObject m_skillPanelObj;
        private GameObject m_sceneTargetObj;
        private GameObject m_sceneMenuObject;

        public SceneConfig m_config;
        public Player m_player;
        private List<List<Transform>> m_tsfMapList;
        private List<Material> m_matList;
        private List<Color> m_originColorList;
        private List<SkillBtn> m_skillBtnList;
        private bool m_sceneWin = false;

        public SceneModule() : base("SceneModule")
        {
        }

        //1 马   2 象  3 士
        private static string[] EnemyPrefabName =
        {
            "null",
            "diHorse",
            "diXiang",
            "diBing",
            "diChe",
            "Enemy",
        };

        private const string MapPrefabPath = "Prefabs/Map";
        private const string PlayerPrefabPath = "Prefabs/Player";
        private const string BcakGroundPath = "Prefabs/BackGround";

        public MapPos YuJiPos;
        public List<List<Enemy>> m_enemyList;
        public Rock m_Rock;
        private List<Arrow> m_ArrowList;
        public List<List<FixedRouteNpc>> m_npcList;
        public List<List<MapDataType>> m_mapData;
        public List<List<MapDataType>> Data
        {
            get { return m_mapData; }
        }

        private void Awake()
        {
            
            
        }

        private void OnEnable()
        {
            m_SceneStep = 0;
            m_sceneWin = false;
            LoadMap();
            LoadPlayer();
            LoadSkillBtn();
            LoadSceneTarget();
            LoadSceneMenu();
            LoadNpc();
            LoadArrow();
            LoadEnemy();
            LoadBackground();
            //LoadRookieModule();

            EventManager.RegisterEvent(HLEventId.NPC_END_MOVE, this.GetHashCode(), NpcComplete);
            EventManager.RegisterEvent(HLEventId.ROUND_FAIL, this.GetHashCode(), RoundFail);
        }

        private void OnDisable()
        {
            m_player.DestroyObj();
            int _row = m_config.SceneConfigList[GameManager.SceneConfigId].MapRow;
            int _col = m_config.SceneConfigList[GameManager.SceneConfigId].MapCol;
            for (int _i = 0; _i < _row; ++_i)
            {
                for (int _j = 0; _j < _col; ++_j)
                {
                    if(m_npcList[_i][_j] != null)
                    m_npcList[_i][_j].DestroyObj();
                }
            }
            for (int _i = 0; _i < _row; ++_i)
            {
                for (int _j = 0; _j < _col; ++_j)
                {
                    if(m_enemyList[_i][_j] != null)
                    {
                        m_enemyList[_i][_j].DestroyObj();
                    }
                }
            }
            GameObject.Destroy(m_sceneObj);
            GameObject.Destroy(m_skillPanelObj);
            GameObject.Destroy(m_sceneTargetObj);
            GameObject.Destroy(m_sceneMenuObject);
            EventManager.UnregisterEvent(HLEventId.NPC_END_MOVE, this.GetHashCode());
            EventManager.UnregisterEvent(HLEventId.ROUND_FAIL, this.GetHashCode());
        }

        public bool isPlayerReady()
        {
            return m_player.IsReady();
        }

        public void GetPlayerPos(ref MapPos pos)
        {
            pos = m_player.Pos;
        }

        //重玩关卡
        private void ReplayScene()
        {
            if (GameManager.SceneConfigId == 0)
            {
                GameManager.GameManagerObj.GetComponent<GameManager>().UnloadModule("RookieModule");
            }
            m_player.DestroyObj();
            GameObject.Destroy(m_sceneObj);
            GameObject.Destroy(m_skillPanelObj);
            GameObject.Destroy(m_sceneTargetObj);
            GameObject.Destroy(m_sceneMenuObject);
            int _row = m_config.SceneConfigList[GameManager.SceneConfigId].MapRow;
            int _col = m_config.SceneConfigList[GameManager.SceneConfigId].MapCol;
            for (int _i = 0; _i < _row; ++_i)
            {
                for (int _j = 0; _j < _col; ++_j)
                {
                    if(m_enemyList[_i][_j] != null)
                    {
                        m_enemyList[_i][_j].DestroyObj();
                    }
                    else if(m_npcList[_i][_j] != null)
                    {
                        m_npcList[_i][_j].DestroyObj();
                    }
                }
            }
            //LoadBackground();
            m_SceneStep = 0;
            LoadMap();
            LoadPlayer();
            LoadSkillBtn();
            LoadSceneTarget();
            LoadSceneMenu();
            LoadNpc();
            LoadEnemy();

            LoadBackground();
            //LoadRookieModule();

        }

        //进入主菜单
        private void GotoMainMenu()
        {
            if(GameManager.SceneConfigId == 0)
            {
                GameManager.GameManagerObj.GetComponent<GameManager>().UnloadModule("RookieModule");
            }
            GameManager.GameManagerObj.GetComponent<GameManager>().UnloadModule("SceneModule");
            SceneManager.LoadScene(0);
            GameManager.GameManagerObj.GetComponent<GameManager>().LoadModule("MainMenuModule");
            //SceneManager.LoadScene(0);
        }

        public void GotoNextScene()
        {
            if (GameManager.SceneConfigId == 0)
            {
                GameManager.GameManagerObj.GetComponent<GameManager>().UnloadModule("RookieModule");
            }
            GameManager.GameManagerObj.GetComponent<GameManager>().UnloadModule("SceneModule");
            ++GameManager.SceneConfigId;
            SceneManager.LoadScene(GameManager.SceneConfigId + 1);
            GameManager.GameManagerObj.GetComponent<GameManager>().LoadModule("SceneModule");
        }

        private void LoadBackground()
        {
            GameObject _obj = (GameObject)GameManager.ResManager.LoadPrefabSync(BcakGroundPath, m_config.SceneConfigList[GameManager.SceneConfigId].BackGroundName, typeof(GameObject));
            _obj.transform.SetParent(GameManager.GameManagerObj.GetComponent<GameManager>().UILayer, false);
        }

        private void LoadNpc()
        {
            ClearNpcData();
            InitialNpcData();
            m_npcCount = m_config.SceneConfigList[GameManager.SceneConfigId].NpcPosData.Count;
            m_waitCount = 0;
            for (int _j = 0, _max = m_npcCount; _j  < _max; ++_j)
            {
                int _r = m_config.SceneConfigList[GameManager.SceneConfigId].NpcPosData[0].m_npcPosData[0].m_row;
                int _c = m_config.SceneConfigList[GameManager.SceneConfigId].NpcPosData[0].m_npcPosData[0].m_col;

                GameObject _obj = (GameObject)GameManager.ResManager.LoadPrefabSync(PlayerPrefabPath, "yuji", typeof(GameObject));
                _obj.transform.SetParent(GameManager.GameManagerObj.GetComponent<GameManager>().SceneLayer);
                m_npcList[_r][_c] = _obj.GetComponent<FixedRouteNpc>();
                if (m_npcList[_r][_c] != null)
                {
                    m_npcList[_r][_c].SetPosition(_r, _c);
                    m_npcList[_r][_c].m_routePosList = m_config.SceneConfigList[GameManager.SceneConfigId].NpcPosData[_j].m_npcPosData;
                }
                else
                {
                    Debug.Log("SceneModule | LoadEnemy Error");
                }
            }
            
        }

        private void LoadRock()
        {
            m_Rock = new Rock();
            m_Rock.m_RockPos.m_row = m_config.SceneConfigList[GameManager.SceneConfigId].RockData.m_RockPos.m_row;
            m_Rock.m_RockPos.m_col = m_config.SceneConfigList[GameManager.SceneConfigId].RockData.m_RockPos.m_col;

            m_Rock.m_dir = m_config.SceneConfigList[GameManager.SceneConfigId].RockData.dir;

            for(int _i = 0; _i < m_config.SceneConfigList[GameManager.SceneConfigId].RockData.m_Trigger.Count; _i++)
            {
                m_Rock.m_Trigger.Add(m_config.SceneConfigList[GameManager.SceneConfigId].RockData.m_Trigger[_i]);
            }
        }

        private void RockTrigger()
        {            
            //动画结束，清除路线上的敌人结束
            m_Rock.Trigger();

            if(m_Rock.m_IsEnd == true)
            {
                m_Rock.DestroyObj();
            }
        }

        private void LoadArrow()
        {
            m_ArrowList = new List<Arrow>();
            Arrow arrow = new Arrow();

            for (int _i = 0; _i < m_config.SceneConfigList[GameManager.SceneConfigId].ArrowData.Count; _i++)
            {
                //加载攻击区域
                
                for (int _j = 0; _j < m_config.SceneConfigList[GameManager.SceneConfigId].ArrowData[_i].m_AttackArea.Count; _j++)
                {
                    arrow.m_AttackArea.Add(new MapPos(m_config.SceneConfigList[GameManager.SceneConfigId].ArrowData[_i].m_AttackArea[_j].m_row, m_config.SceneConfigList[GameManager.SceneConfigId].ArrowData[_i].m_AttackArea[_j].m_col));
                    //arrow.m_AttackArea.Add(new MapPos(6, 0));
                }
                //加载触发区域
                for (int _j = 0; _j < m_config.SceneConfigList[GameManager.SceneConfigId].ArrowData[_i].m_TriggerArea.Count; _j++)
                {
                    arrow.m_TriggerArea.Add(new MapPos(m_config.SceneConfigList[GameManager.SceneConfigId].ArrowData[_i].m_TriggerArea[_j].m_row, m_config.SceneConfigList[GameManager.SceneConfigId].ArrowData[_i].m_TriggerArea[_j].m_col));
                }

                //加载触发角色
                for (int _j = 0; _j < m_config.SceneConfigList[GameManager.SceneConfigId].ArrowData[_i].m_Trigger.Count; _j++)
                {
                    arrow.m_Trigger.Add(m_config.SceneConfigList[GameManager.SceneConfigId].ArrowData[_i].m_Trigger[_j]);
                }

                m_ArrowList.Add(arrow);
            }

        }

        private void ArrowTrigger()
        {
            //所有角色移动后调用触发函数打开开关
            for (int _i = 0; _i < m_ArrowList.Count; _i++)
            {
                if (m_ArrowList[_i].ArrowTrigger())
                {

                    //这个弓箭提示，下回合开始攻击

                    //攻击区域
                    //m_ArrowList[_i].m_AttackArea

                }
            }

        }

        private void ArrowAttack()
        {
            for (int _i = 0; _i < m_ArrowList.Count; _i++)
            {
                PlayerD Ret = m_ArrowList[_i].ArrowAttack();
                //删掉arrow


                //是否删掉项羽虞姬，游戏是否结束
                if (Ret == PlayerD.NONE)
                {
                    //游戏继续
                }
                else if (Ret == PlayerD.ALL)
                {
                    m_player.DestroyObj();
                    //游戏结束
                }
                else if (Ret == PlayerD.YUJI)
                {
                    m_npcList[YuJiPos.m_row][YuJiPos.m_col].DestroyObj();
                    //游戏结束
                }
                else if(Ret == PlayerD.XIANGYU)
                {
                    m_player.DestroyObj();
                    m_npcList[YuJiPos.m_row][YuJiPos.m_col].DestroyObj();
                    //游戏结束
                }

            }
        }

        private void InitialNpcData()
        {
            int _row = m_config.SceneConfigList[GameManager.SceneConfigId].MapRow;
            int _col = m_config.SceneConfigList[GameManager.SceneConfigId].MapCol;
            m_npcList = new List<List<FixedRouteNpc>>(_row);
            for (int _i = 0; _i < _row; ++_i)
            {
                List<FixedRouteNpc> _lst = new List<FixedRouteNpc>(_col);
                for (int _j = 0; _j < _col; ++_j)
                {
                    _lst.Add(null);
                }
                m_npcList.Add(_lst);
            }
        }

        private void LoadRookieModule()
        {
            if (GameManager.SceneConfigId == 0)
            {
                GameManager.GameManagerObj.GetComponent<GameManager>().LoadModule("RookieModule");
                SetPlayerCanMove(false);
            }
        }

        private void LoadSkillBtn()
        {
            ClearSkillBtnData();
            GameObject _skillPanel = (GameObject)GameManager.ResManager.LoadPrefabSync(MapPrefabPath, "SKillPanel", typeof(GameObject));
            _skillPanel.transform.SetParent(GameManager.GameManagerObj.GetComponent<GameManager>().UILayer, false);
            m_skillPanelObj = _skillPanel;
            InitialSkillBtnData();

            for (int _i = 0; _i < m_config.SceneConfigList[GameManager.SceneConfigId].SkillData.Count; ++_i)
            {
                GameObject _skillBtnObj = (GameObject)GameManager.ResManager.LoadPrefabSync(MapPrefabPath, "SkillBtn", typeof(GameObject));
                _skillBtnObj.transform.SetParent(_skillPanel.transform, false);
                SkillBtn _skillBtn = _skillBtnObj.GetComponent<SkillBtn>();
                _skillBtn.Initial(
                    m_config.SceneConfigList[GameManager.SceneConfigId].SkillData[_i].Id,
                    m_config.SceneConfigList[GameManager.SceneConfigId].SkillData[_i].Count);
                m_skillBtnList.Add(_skillBtn);
            }

        }

        private void LoadSceneTarget()
        {
            GameObject _sceneTargetObj = (GameObject)GameManager.ResManager.LoadPrefabSync(MapPrefabPath, "SceneTarget", typeof(GameObject));
            _sceneTargetObj.transform.SetParent(GameManager.GameManagerObj.GetComponent<GameManager>().UILayer, false);
            m_sceneTargetObj = _sceneTargetObj;
        }

        private void LoadSceneMenu()
        {
            GameObject _sceneMenuObj = (GameObject)GameManager.ResManager.LoadPrefabSync(MapPrefabPath, "SceneMenu", typeof(GameObject));
            _sceneMenuObj.transform.SetParent(GameManager.GameManagerObj.GetComponent<GameManager>().UILayer, false);
            m_sceneMenuObject = _sceneMenuObj;
            m_sceneMenuObject.transform.Find("Button").GetComponent<Button>().onClick.AddListener(ReplayScene);
            m_sceneMenuObject.transform.Find("Button1").GetComponent<Button>().onClick.AddListener(GotoMainMenu);
            m_sceneMenuObject.transform.Find("Button2").GetComponent<Button>().onClick.AddListener(GotoNextScene);
        }

        private void InitialSkillBtnData()
        {
            m_skillBtnList = new List<SkillBtn>();
        }

        private void LoadPlayer()
        {
            m_player = null;
            string _name = "xiangyu";
            GameObject _playerPrefab = (GameObject)GameManager.ResManager.LoadPrefabSync(PlayerPrefabPath, _name, typeof(GameObject));
            _playerPrefab.transform.SetParent(GameManager.GameManagerObj.GetComponent<GameManager>().SceneLayer);
            m_player = _playerPrefab.GetComponent<Player>();
            if (m_player != null)
            {
                m_player.SetStartPosition(
                m_config.SceneConfigList[GameManager.SceneConfigId].PlayerStartRow,
                m_config.SceneConfigList[GameManager.SceneConfigId].PlayerStartCol);
            }
            else
            {
                Debug.Log("SceneModule | LoadPlayer Error");
            }
        }

        //加载地图数据
        private void LoadMap()
        {
            ClearMapData();
            m_config = Resources.Load<SceneConfig>("SceneConfig");
            string _name = m_config.SceneConfigList[GameManager.SceneConfigId].PrefabName;
            GameObject _mapPrefab = (GameObject)GameManager.ResManager.LoadPrefabSync(MapPrefabPath, _name, typeof(GameObject));
            _mapPrefab.transform.SetParent(GameManager.GameManagerObj.GetComponent<GameManager>().SceneLayer, false);
            m_sceneObj = _mapPrefab;

            Transform _tsfMapDataRoot = _mapPrefab.transform;
            InitialMapData();
            for (int _i = 0; _i < _tsfMapDataRoot.childCount; ++_i)
            {
                Transform _child = _tsfMapDataRoot.GetChild(_i);
                MapData _mapData = _child.GetComponent<MapData>();
                
                if (_mapData != null)
                {
                    int _r = _mapData.Pos.m_row;
                    int _c = _mapData.Pos.m_col;
                    m_mapData[_r][_c] = _mapData.Data;
                    m_tsfMapList[_r][_c] = _child;
                }
            }
        }

        private void InitialMapData()
        {
            m_tsfMapList = new List<List<Transform>>();
            m_matList = new List<Material>();
            m_originColorList = new List<Color>();

            int _row = m_config.SceneConfigList[GameManager.SceneConfigId].MapRow;
            int _col = m_config.SceneConfigList[GameManager.SceneConfigId].MapCol;
            m_mapData = new List<List<MapDataType>>(_row);
            for (int _i = 0; _i < _row; ++_i)
            {
                List<MapDataType> _list = new List<MapDataType>(_col);
                for (int _j = 0; _j < _col; ++_j)
                {
                    _list.Add(MapDataType.NONE);
                }
                m_mapData.Add(_list);
            }
            for (int _i = 0; _i < _row; ++_i)
            {
                List<Transform> _list = new List<Transform>(_col);
                for (int _j = 0; _j < _col; ++_j)
                {
                    _list.Add(null);
                }
                m_tsfMapList.Add(_list);
            }
        }




        public bool WaitNpc()
        {
            //到达终点
            if(m_sceneWin)
            {
                GotoNextScene();
                return false;
            }

            m_waitCount = m_npcCount;
            if(m_npcCount == 0)
            {
                NpcComplete(null);
                if (GameManager.SceneConfigId == 0) return true;
                else return false;
            }
            else
            {
                return true;
            }
        }

        public void NpcComplete(EventArgs args)
        {
            if(m_waitCount > 0)
            {
                --m_waitCount;
            }
            if (m_waitCount == 0)
            {
                m_player.SetCanMove(true);
                EnemyListUpdate();
                CheckSkillCount();
                m_SceneStep++;
                //map2在项羽走了两步以后在9,0，出现一个马
                if (GameManager.SceneConfigId == 2 && 3 == m_SceneStep)
                {
                    int _type = 4;
                    int row = 7;
                    int col = 5;
                    GameObject _obj = (GameObject)GameManager.ResManager.LoadPrefabSync(PlayerPrefabPath, EnemyPrefabName[_type], typeof(GameObject));
                    _obj.transform.SetParent(GameManager.GameManagerObj.GetComponent<GameManager>().SceneLayer);
                    m_enemyList[row][col] = _obj.GetComponent<Enemy>();
                    m_enemyList[row][col].SetType(_type);
                    m_enemyList[row][col].SetStartPos(row, col);
                }
                ArrowAttack();
                ArrowTrigger();
                
            }
        }

        //返回0 没有虞姬 ，返回1 有虞姬，已经更新位置
        public bool UpdateYuJiPos()
        {
            int _row = m_config.SceneConfigList[GameManager.SceneConfigId].MapRow;
            int _col = m_config.SceneConfigList[GameManager.SceneConfigId].MapCol;

            for (int _i = 0; _i < _row; ++_i)
            {

                for (int _j = 0; _j < _col; ++_j)
                {
                    if (m_npcList[_i][_j] != null)
                    {
                        YuJiPos.m_row = _i;
                        YuJiPos.m_col = _j;
                        return true;
                    }
                }
                
            }
            return false;
        }


		private void CheckSkillCount()
        {
            if(GameManager.SceneConfigId == 0)
            {
                return;
            }
            bool _ret = false;
            for (int _i = 0; _i < m_skillBtnList.Count; ++_i)
            {
                if (m_skillBtnList[_i].Count != 0)
                {
                    _ret = true;
                    break;
                }
            }
            if(!_ret)
            {
                RoundFail(null);
            }
        }

        public void RoundFail(EventArgs args)
        {
            ReplayScene();
        }

        public void EnemyListUpdate()
        {
            bool YuJiExist = UpdateYuJiPos();
            //清空所有enemy change 标记
            for (int _i = 0; _i < m_enemyList.Count; _i++)
            {
                for (int _j = 0; _j < m_enemyList[_i].Count; _j++)
                {
                    if (m_enemyList[_i][_j] != null)
                    {
                        m_enemyList[_i][_j].PosIsChange = 0;
                        m_enemyList[_i][_j].m_EnemyIsMove = false;
                    }
                }
            }

            int GameOver = 0;
            //找一个enemy
            for (int _i = 0; _i < m_enemyList.Count; _i++)
            {
                for (int _j = 0; _j < m_enemyList[_i].Count; _j++)
                {
                    if (m_enemyList[_i][_j] != null && m_enemyList[_i][_j].PosIsChange == 0)
                    {
                        //找出离player最近的可走的点为最后的结果
                        if ((GameOver = m_enemyList[_i][_j].GetEnemyNextPos(YuJiExist)) != 0)
                        {
                            //吃子特效写在这，1副子， 2主子**
                        }
                    }
                }
            }

            //遍历所有enemy,播位置变化的动画,update
            for (int _i = 0; _i < m_enemyList.Count; _i++)
            {
                for (int _j = 0; _j < m_enemyList[_i].Count; _j++)
                {
                    if (m_enemyList[_i][_j] != null && !m_enemyList[_i][_j].m_EnemyIsMove)
                    {
                        m_enemyList[_i][_j].EnemyMove(_i, _j);
                       // m_enemyList[_i][_j].Update();
                    }
                }
            }

            //触发本局游戏结束
            if (GameOver != 0)
            {
                if (YuJiExist && GameOver == 1)
                {
                    //虞姬死了
                }
                //项羽死了

            }
        }

        private void LoadEnemy()
        {
            ClearEnemyData();
            InitialEnemy();
            for(int _j = 0; _j < m_config.SceneConfigList[GameManager.SceneConfigId].EnemyData.Count; ++_j)
            {
                int _r = m_config.SceneConfigList[GameManager.SceneConfigId].EnemyData[_j].Pos.m_row;
                int _c = m_config.SceneConfigList[GameManager.SceneConfigId].EnemyData[_j].Pos.m_col;
                int _type = m_config.SceneConfigList[GameManager.SceneConfigId].EnemyData[_j].Type;
                GameObject _obj = (GameObject)GameManager.ResManager.LoadPrefabSync(PlayerPrefabPath, EnemyPrefabName[_type], typeof(GameObject));
                _obj.transform.SetParent(GameManager.GameManagerObj.GetComponent<GameManager>().SceneLayer);
                m_enemyList[_r][_c] = _obj.GetComponent<Enemy>();
                if (m_enemyList[_r][_c] != null)
                {
                    m_enemyList[_r][_c].SetType(_type);
                    m_enemyList[_r][_c].SetStartPos(_r, _c);
                }
                else
                {
                    Debug.Log("SceneModule | LoadEnemy Error");
                }
            }

        }

        private void InitialEnemy()
        {
            int _row = m_config.SceneConfigList[GameManager.SceneConfigId].MapRow;
            int _col = m_config.SceneConfigList[GameManager.SceneConfigId].MapCol;
            m_enemyList = new List<List<Enemy>>(_row);
            for (int _i = 0; _i < _row; ++_i)
            {
                List<Enemy> _lst = new List<Enemy>(_col);
                for (int _j = 0; _j < _col; ++_j)
                {
                    _lst.Add(null);
                }
                m_enemyList.Add(_lst);
            }
        }

        private void OnDestroy()
        {
            //Debug.Log("SceneModule | OnDestroy");
            //Resources.UnloadAsset(m_config);
            m_config = null;
        }

        private void ClearEnemyData()
        {
            if (m_enemyList != null)
            {
                for (int _i = 0, _max = m_enemyList.Count; _i < _max; _i++)
                {
                    m_enemyList[_i].Clear();
                }
                m_enemyList.Clear();
            }

        }

        private void ClearNpcData()
        {
            if(m_npcList != null)
            {
                for (int _i = 0, _max = m_npcList.Count; _i < _max; _i++)
                {
                    m_npcList[_i].Clear();
                }
                m_npcList.Clear();
            }
            m_npcList = null;
        }

        private void ClearMapData()
        {
            if(m_sceneObj != null)
            {
                GameObject.Destroy(m_sceneObj);
            }
            m_sceneObj = null;
            if (m_mapData != null)
            {
                for (int _i = 0, _max = m_mapData.Count; _i < _max; _i++)
                {
                    m_mapData[_i].Clear();
                }
                m_mapData.Clear();
            }
            if (m_tsfMapList != null)
            {
                for(int _i = 0, _max = m_mapData.Count; _i < _max; ++_i)
                {
                    m_tsfMapList[_i].Clear();
                }
                m_tsfMapList.Clear();
            }
            if (m_matList != null)
            {
                m_matList.Clear();
            }
            if (m_originColorList != null)
            {
                m_originColorList.Clear();
            }
            m_mapData = null;
            m_tsfMapList = null;
            m_matList = null;
            m_originColorList = null;
        }

        private void ClearSkillBtnData()
        {
            if(m_skillPanelObj != null)
            {
                GameObject.Destroy(m_skillPanelObj);
            }
            m_skillPanelObj = null;
            if (m_skillBtnList != null)
            {
                m_skillBtnList.Clear();
            }
            m_skillBtnList = null;
        }

        public int getMapDataRow()
        {
            return m_config.SceneConfigList[GameManager.SceneConfigId].MapRow;
        }

        public int getMapDataCol()
        {
            return m_config.SceneConfigList[GameManager.SceneConfigId].MapCol;
        }

        private bool PosExitChess(int row, int col)
        {
            SceneModule _sceneModule = (SceneModule)GameManager.GameManagerObj.GetComponent<GameManager>().GetModuleByName("SceneModule");
            if (_sceneModule.m_enemyList[row][col] != null)
            {
                return true;
            }
            if(_sceneModule.m_npcList[row][col] != null)
            {
                return true;
            }
            return false;
        }

        public void ChangeMap(SkillId id, int playerRow, int playerCol)
        {
            if(m_waitCount > 0)
            {
                return;
            }
            int _mapRow = m_config.SceneConfigList[GameManager.SceneConfigId].MapRow;
            int _mapCol = m_config.SceneConfigList[GameManager.SceneConfigId].MapCol;
            RefreshMap();
            switch (id)
            {
                case SkillId.JU:
                    for (int _i = playerRow + 1; _i < _mapRow; ++_i)
                    {
                        if (m_mapData[_i][playerCol] != MapDataType.GAOTAI && m_mapData[_i][playerCol] != MapDataType.NONE
                            && m_npcList[_i][playerCol] == null)
                        {
                            Material _material = m_tsfMapList[_i][playerCol].GetComponent<MeshRenderer>().material;
                            m_originColorList.Add(_material.GetColor("_Color"));
                            _material.SetColor("_Color", Color.red);
                            m_matList.Add(_material);
                            if(m_enemyList[_i][playerCol] != null)
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    for (int _i = playerRow - 1; _i >= 0; --_i)
                    {
                        if (m_mapData[_i][playerCol] != MapDataType.GAOTAI && m_mapData[_i][playerCol] != MapDataType.NONE
                              && m_npcList[_i][playerCol] == null)
                        {
                            Material _material = m_tsfMapList[_i][playerCol].GetComponent<MeshRenderer>().material;
                            m_originColorList.Add(_material.GetColor("_Color"));
                            _material.SetColor("_Color", Color.red);
                            m_matList.Add(_material);
                            if(m_enemyList[_i][playerCol] != null)
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    for (int _j = playerCol + 1; _j < _mapCol; ++_j)
                    {
                        if (m_mapData[playerRow][_j] != MapDataType.GAOTAI && m_mapData[playerRow][_j] != MapDataType.NONE
                              && m_npcList[playerRow][_j] == null)
                        {
                            Material _material = m_tsfMapList[playerRow][_j].GetComponent<MeshRenderer>().material;
                            m_originColorList.Add(_material.GetColor("_Color"));
                            _material.SetColor("_Color", Color.red);
                            m_matList.Add(_material);
                            if(m_enemyList[playerRow][_j] != null)
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    for (int _j = playerCol - 1; _j >= 0; --_j)
                    {
                        if (m_mapData[playerRow][_j] != MapDataType.GAOTAI && m_mapData[playerRow][_j] != MapDataType.NONE
                              && m_npcList[playerRow][_j] == null)
                        {
                            Material _material = m_tsfMapList[playerRow][_j].GetComponent<MeshRenderer>().material;
                            m_originColorList.Add(_material.GetColor("_Color"));
                            _material.SetColor("_Color", Color.red);
                            m_matList.Add(_material);
                            if(m_enemyList[playerRow][_j] != null)
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    break;
                case SkillId.MA:
                    if (playerRow >= 1)
                    {
                        if (playerCol >= 2)
                        {
                            if (m_mapData[playerRow - 1][playerCol - 2] != MapDataType.NONE 
                                && m_npcList[playerRow - 1][playerCol - 2] == null
                                && (m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (m_mapData[playerRow][playerCol - 1] != MapDataType.GAOTAI)))
                            {
                                if(!PosExitChess(playerRow, playerCol - 1))
                                {
                                    Material _material = m_tsfMapList[(playerRow - 1)][playerCol - 2].GetComponent<MeshRenderer>().material;
                                    m_originColorList.Add(_material.GetColor("_Color"));
                                    _material.SetColor("_Color", Color.red);
                                    m_matList.Add(_material);
                                }
                                
                            }
                        }
                        if (playerCol + 2 < _mapCol)
                        {
                            if (m_mapData[playerRow - 1][playerCol + 2] != MapDataType.NONE
                                && m_npcList[playerRow - 1][playerCol + 2] == null
                                && (m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (m_mapData[playerRow][playerCol + 1] != MapDataType.GAOTAI)))
                            {
                                if (!PosExitChess(playerRow, playerCol + 1))
                                {
                                    Material _material = m_tsfMapList[playerRow - 1][ playerCol + 2].GetComponent<MeshRenderer>().material;
                                    m_originColorList.Add(_material.GetColor("_Color"));
                                    _material.SetColor("_Color", Color.red);
                                    m_matList.Add(_material);
                                }                                 
                            }
                        }
                    }
                    if (playerRow >= 2)
                    {
                        if (playerCol >= 1)
                        {
                            if (m_mapData[playerRow - 2][playerCol - 1] != MapDataType.NONE
                                && m_npcList[playerRow - 2][playerCol - 1] == null
                                && (m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (m_mapData[playerRow - 1][playerCol] != MapDataType.GAOTAI)))
                            {
                                if(!PosExitChess(playerRow - 1, playerCol))
                                {
                                    Material _material = m_tsfMapList[playerRow - 2][playerCol - 1].GetComponent<MeshRenderer>().material;
                                    m_originColorList.Add(_material.GetColor("_Color"));
                                    _material.SetColor("_Color", Color.red);
                                    m_matList.Add(_material);
                                }
                            }
                        }
                        if (playerCol + 1 < _mapCol)
                        {
                            if (m_mapData[playerRow - 2][playerCol + 1] != MapDataType.NONE
                                && m_npcList[playerRow - 2][playerCol + 1] == null
                                && (m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (m_mapData[playerRow - 1][playerCol] != MapDataType.GAOTAI)))
                            {
                                if(!PosExitChess(playerRow - 1, playerCol))
                                {
                                    Material _material = m_tsfMapList[playerRow - 2][playerCol + 1].GetComponent<MeshRenderer>().material;
                                    m_originColorList.Add(_material.GetColor("_Color"));
                                    _material.SetColor("_Color", Color.red);
                                    m_matList.Add(_material);
                                }
                                
                            }
                        }
                    }
                    if (playerRow + 1 < _mapRow)
                    {
                        if (playerCol >= 2)
                        {
                            if (m_mapData[playerRow + 1][playerCol - 2] != MapDataType.NONE
                                && m_npcList[playerRow + 1][playerCol - 2] == null
                                && (m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (m_mapData[playerRow][playerCol - 1] != MapDataType.GAOTAI)))
                            {
                                if(!PosExitChess(playerRow, playerCol - 1))
                                {
                                    Material _material = m_tsfMapList[playerRow + 1][playerCol - 2].GetComponent<MeshRenderer>().material;
                                    m_originColorList.Add(_material.GetColor("_Color"));
                                    _material.SetColor("_Color", Color.red);
                                    m_matList.Add(_material);
                                }
                                
                            }
                        }
                        if (playerCol + 2 < _mapCol)
                        {
                            if (m_mapData[playerRow + 1][playerCol + 2] != MapDataType.NONE
                                && m_npcList[playerRow + 1][playerCol + 2] == null
                                && (m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (m_mapData[playerRow][playerCol + 1] != MapDataType.GAOTAI)))
                            {
                                if(!PosExitChess(playerRow, playerCol + 1))
                                {
                                    Material _material = m_tsfMapList[playerRow + 1][playerCol + 2].GetComponent<MeshRenderer>().material;
                                    m_originColorList.Add(_material.GetColor("_Color"));
                                    _material.SetColor("_Color", Color.red);
                                    m_matList.Add(_material);
                                }
                            }
                        }
                    }
                    if (playerRow + 2 < _mapRow)
                    {
                        if (playerCol >= 1)
                        {
                            if (m_mapData[playerRow + 2][playerCol - 1] != MapDataType.NONE
                                && m_npcList[playerRow + 2][playerCol - 1] == null
                                && (m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (m_mapData[playerRow + 1][playerCol] != MapDataType.GAOTAI)))
                            {
                                if(!PosExitChess(playerRow + 1, playerCol))
                                {
                                    Material _material = m_tsfMapList[playerRow + 2][playerCol - 1].GetComponent<MeshRenderer>().material;
                                    m_originColorList.Add(_material.GetColor("_Color"));
                                    _material.SetColor("_Color", Color.red);
                                    m_matList.Add(_material);
                                }
                                
                            }
                        }
                        if (playerCol + 1 < _mapCol)
                        {
                            if (m_mapData[playerRow + 2][playerCol + 1] != MapDataType.NONE
                                && m_npcList[playerRow + 2][playerCol + 1] == null
                                && (m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (m_mapData[playerRow + 1][playerCol] != MapDataType.GAOTAI)))
                            {
                                if(!PosExitChess(playerRow + 1, playerCol))
                                {
                                    Material _material = m_tsfMapList[playerRow + 2][playerCol + 1].GetComponent<MeshRenderer>().material;
                                    m_originColorList.Add(_material.GetColor("_Color"));
                                    _material.SetColor("_Color", Color.red);
                                    m_matList.Add(_material);
                                }
                            }
                        }
                    }
                    break;
                case SkillId.PAO:
                    break;
                case SkillId.XIANG:
                    if (playerRow >= 2)
                    {
                        if (playerCol >= 2)
                        {
                            if (m_mapData[playerRow - 2][playerCol - 2] != MapDataType.NONE
                                && m_npcList[playerRow - 2][playerCol - 2] == null
                                && (m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (m_mapData[playerRow - 1][playerCol - 1] != MapDataType.GAOTAI)))
                            {
                                if(!PosExitChess(playerRow-1, playerCol -1))
                                {
                                    Material _material = m_tsfMapList[playerRow - 2][playerCol - 2].GetComponent<MeshRenderer>().material;
                                    m_originColorList.Add(_material.GetColor("_Color"));
                                    _material.SetColor("_Color", Color.red);
                                    m_matList.Add(_material);
                                }
                                
                            }
                        }
                        if (playerCol + 2 < _mapCol)
                        {
                            if (m_mapData[playerRow - 2][playerCol + 2] != MapDataType.NONE
                                && m_npcList[playerRow - 2][playerCol + 2] == null
                                && (m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (m_mapData[playerRow - 1][playerCol + 1] != MapDataType.GAOTAI)))
                            {
                                if(!PosExitChess(playerRow - 1, playerCol + 1))
                                {
                                    Material _material = m_tsfMapList[playerRow - 2][playerCol + 2].GetComponent<MeshRenderer>().material;
                                    m_originColorList.Add(_material.GetColor("_Color"));
                                    _material.SetColor("_Color", Color.red);
                                    m_matList.Add(_material);
                                }
                                
                            }
                        }
                    }
                    if (playerRow + 2 < _mapRow)
                    {
                        if (playerCol >= 2)
                        {
                            if (m_mapData[playerRow + 2][playerCol - 2] != MapDataType.NONE
                                && m_npcList[playerRow + 2][playerCol - 2] == null
                                && (m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (m_mapData[playerRow + 1][playerCol - 1] != MapDataType.GAOTAI)))
                            {
                                if(!PosExitChess(playerRow + 1, playerCol - 1))
                                {
                                    Material _material = m_tsfMapList[playerRow + 2][playerCol - 2].GetComponent<MeshRenderer>().material;
                                    m_originColorList.Add(_material.GetColor("_Color"));
                                    _material.SetColor("_Color", Color.red);
                                    m_matList.Add(_material);
                                }
                                
                            }
                        }
                        if (playerCol + 2 < _mapCol)
                        {
                            if (m_mapData[playerRow + 2][playerCol + 2] != MapDataType.NONE
                                && m_npcList[playerRow + 2][playerCol + 2] == null
                                && (m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (m_mapData[playerRow + 1][playerCol + 1] != MapDataType.GAOTAI)))
                            {
                                if(!PosExitChess(playerRow + 1, playerCol + 1))
                                {
                                    Material _material = m_tsfMapList[playerRow + 2][playerCol + 2].GetComponent<MeshRenderer>().material;
                                    m_originColorList.Add(_material.GetColor("_Color"));
                                    _material.SetColor("_Color", Color.red);
                                    m_matList.Add(_material);
                                }
                            }
                        }
                    }
                    break;
                case SkillId.SHI:
                    break;
                case SkillId.BING:
                    break;
            }
        }

        public void RefreshMap()
        {
            if (m_matList != null)
            {
                for (int _i = 0; _i < m_matList.Count; ++_i)
                {
                    m_matList[_i].SetColor("_Color", m_originColorList[_i]);
                }
                m_matList.Clear();
                m_originColorList.Clear();
            }
        }

        public Transform GetTsfMapData(int row, int col)
        {
            int _mapRow = m_config.SceneConfigList[GameManager.SceneConfigId].MapRow;
            int _mapCol = m_config.SceneConfigList[GameManager.SceneConfigId].MapCol;
            if(row > _mapRow || row < 0 || col > _mapCol || col < 0)
            {
                return null;
            }
            return m_tsfMapList[row][col];
        }

        public void SetPlayerCanMove(bool bState)
        {
            m_player.SetCanMove(bState);
        }

        public void ArriveSceneFinal()
        {
            m_sceneWin = true;
        }
    }
}

