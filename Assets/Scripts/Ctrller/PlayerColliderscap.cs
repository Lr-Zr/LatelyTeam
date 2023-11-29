using nara;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nara
{

    public class PlayerColliderscap : MonoBehaviour
    {

        [SerializeField]
        public GameObject[] _Collider = new GameObject[11];

        PlayerCtrllercap _playerCtrller;
        void Start()
        {
            for (int i = 0; i < _Collider.Length; i++)
            {
                if (_Collider[i] != null)
                    _Collider[i].SetActive(false);
            }
            _playerCtrller = this.GetComponent<PlayerCtrllercap>();

        }


       
        /* �������� �浹������Ʈ �¿���*/
        public void ActiveOn(int atk)
        {
            switch (atk)
            {
                case 0://�븻����1,2
                    _playerCtrller.Dmg = 3;
                    _playerCtrller._Power = new Vector3(0, 0, 0);
                    break;
                case 1://�븻����3
                    _playerCtrller._Power = new Vector3(40, 30, 0);
                    _playerCtrller.Dmg = 6;
                    break;



                case 2://������
                    _playerCtrller._Power = new Vector3(0, 40, 0);
                    _playerCtrller.Dmg = 8;
                    break;

                    
                case 3://�Ʒ�����
                    _playerCtrller._Power = new Vector3(40, 10, 0);
                    _playerCtrller.Dmg = 6;
                    break;



                case 4://�¿����
                    _playerCtrller._Power = new Vector3(40, 20, 0);
                    _playerCtrller.Dmg = 15;
                    break;



                case 5://���ְ���
                    _playerCtrller._Power = new Vector3(50, 10, 0);
                    _playerCtrller.Dmg = 13;
                    break;


                case 6://���߾Ʒ�����
                    _playerCtrller._Power = new Vector3(10, -30, 0);
                    _playerCtrller.Dmg = 12;
                    //hit�� 3
                    break;
                //
                case 7://�����߸�����
                    _playerCtrller._Power = new Vector3(5, 40, 0);
                    _playerCtrller.Dmg = 4;
                    break;


                case 8://��ų �븻
                    _playerCtrller._Power = new Vector3(30, 20, 0);
                    _playerCtrller.Dmg = 10;

                    break;
                case 9://��ų ��
                    _playerCtrller._Power = new Vector3(30, 20, 0);
                    _playerCtrller.Dmg = 10;
                    break;


                case 10://��ų�Ʒ�1
                    _playerCtrller._Power = new Vector3(0, 0, 0);
                    _playerCtrller.Dmg = 6;
                    break;
                case 11://��ų�Ʒ�2
                    _playerCtrller._Power = new Vector3(0, 0, 0);
                    _playerCtrller.Dmg = 10;
                    break;
                case 12://��ų�Ʒ�3
                    _playerCtrller._Power = new Vector3(50, -50, 0);
                    _playerCtrller.Dmg = 16;
                    break;
                case 13://��ų �¿�
                    _playerCtrller._Power = new Vector3(50, 20, 0);
                    _playerCtrller.Dmg = 13;
                    break;
            }
            
                    _Collider[atk].SetActive(true);
        }
        public void ActiveOff(int atk)
        {
            _Collider[atk].SetActive(false);
       
        }

        public void SetTag(string tag)
        {
            for (int i = 0; i < _Collider.Length; i++)
            {
                if (_Collider[i] != null)
                    _Collider[i].tag = tag;

            }
        }
    }

}