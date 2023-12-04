using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace nara
{

    public class GameMgr : MonoBehaviour
    {
        static GameMgr _Ins;//�ν��Ͻ�
        public static GameMgr Instance { get { Init();return _Ins; } }



        InputMgr _Input = new InputMgr();
        public static InputMgr Input { get { return Instance._Input; } }

        //EffectMgr _Effect = new EffectMgr();
        //public static EffectMgr Effect { get { return Instance._Effect; } } 


        void Start()
        {
            Init();
        }


        void Update()
        {

            _Input.OnUpdate();
        }
        private void FixedUpdate()
        {
           
        }
        public void onRespawnPlat(int player)
        {
            
            if (player == 1)
            {
                GameObject obj = GameObject.Find("1PZone");
                if(obj != null)
                {
                    Debug.Log("dd");
                    obj.transform.Find("1P").gameObject.SetActive(true);
                }

            }
            else
            {

                GameObject obj = GameObject.Find("2PZone");
                if (obj != null)
                {
                    obj.transform.Find("2P").gameObject.SetActive(true);
                }

            }

        }
        public void offRespawnPlat(int player)
        {
            if (player == 1)
            {
                GameObject obj = GameObject.Find("1PZone");

                if (obj != null)
                {
                    obj.transform.Find("1P").gameObject.SetActive(false);
                }
            }
            else
            {

                GameObject obj = GameObject.Find("2PZone");
                if (obj != null)
                {
                    obj.transform.Find("2P").gameObject.SetActive(false);
                }
            }

        }

        static void Init()
        {
            if(_Ins == null )
            {
                GameObject gameobject = GameObject.Find("@GameMgr");
                if(gameobject == null )
                {
                    gameobject = new GameObject { name = "@GameMgr" };//������Ʈ ����
                    gameobject.AddComponent<GameMgr>();

                }
                DontDestroyOnLoad(gameobject);
                _Ins = gameobject.GetComponent<GameMgr>();
            }
        }
    }

}