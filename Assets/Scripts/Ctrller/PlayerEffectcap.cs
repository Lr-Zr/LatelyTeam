using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace nara
{


    public class PlayerEffectcap : MonoBehaviour
    {

        [SerializeField]
        GameObject[] _Effects;
        GameObject[] go;



        [SerializeField]
        GameObject[] _AtkEffects;
        GameObject[] atkgo;

        //�� �� 
        Transform _TipOfSword;

        Vector3 _Pos;
        Vector3 _Rot;


        PlayerCtrllercap _playerCtrller;
        [SerializeField]
        GameObject _hit;
        GameObject _hitgo;
        void Start()
        {
            go = new GameObject[(int)Effect.End];
            atkgo = new GameObject[(int)AtkEffect1.End];
            SearchInChildren(this.transform);
            _playerCtrller = GetComponent<PlayerCtrllercap>();
        }

        public void EffectPlay(Effect effect, float time = 0.5f)
        {
            if (go[(int)effect] != null) return;
            _Pos = this.transform.position + _Effects[(int)effect].transform.position;

            go[(int)effect] = Instantiate(_Effects[(int)effect], _Pos, Quaternion.identity);
            Destroy(go[(int)effect], time);

        }

        //���� ���� ����Ʈ
        public void EffectOn(int type)
        {
            //������ ���ݾ� �׷� �����ð� ����Ʈ

            //Debug.Log(_AtkPos);
            if (type != 16)
            {

                if (atkgo[type] != null) return;
            }
            _Pos = _TipOfSword.position;
            switch (type)
            {
                case 0://�⺻����1
                    if (_playerCtrller.dir > 0.0f)//����Ʈ ���� ��ȯ
                    {
                        _AtkEffects[type].transform.localScale = new Vector3(1, 1, 1);

                    }
                    else if (_playerCtrller.dir < 0.0f)
                    {
                        _AtkEffects[type].transform.localScale = new Vector3(-1, 1, 1); ;

                    }
                    _Pos += _AtkEffects[type].transform.position;
                    break;
                case 1://�⺻����2
                    if (_playerCtrller.dir > 0.0f)//����Ʈ ���� ��ȯ
                    {
                        _AtkEffects[type].transform.localScale = new Vector3(1, 1, 1);

                        _AtkEffects[type].transform.localEulerAngles = new Vector3(0, 0, -45f);
                    }
                    else if (_playerCtrller.dir < 0.0f)
                    {
                        _AtkEffects[type].transform.localEulerAngles = new Vector3(0, 0, 45f);
                        _AtkEffects[type].transform.localScale = new Vector3(-1, 1, 1); ;
                    }
                    _Pos += _AtkEffects[type].transform.position;
                    break;

                case 2://�⺻����3
                    if (_playerCtrller.dir > 0.0f)//����Ʈ ���� ��ȯ
                    {
                        _AtkEffects[type].transform.localScale = new Vector3(1, 1, 1);

                    }
                    else if (_playerCtrller.dir < 0.0f)
                    {
                        _AtkEffects[type].transform.localScale = new Vector3(-1, 1, 1); ;

                    }
                    _Pos += _AtkEffects[type].transform.position;

                    break;


                case 3://������
                    
                    break;

                case 4://�Ʒ�����

                    _AtkEffects[type].transform.position = new Vector3(_playerCtrller.dir, 2, 1);

                    _Pos = this.transform.position + _AtkEffects[type].transform.position;
                    break;




                case 5://�¿����
                    break;

                case 6://���ְ���
                    break;




                case 7://���߾Ʒ�����

                    if (_playerCtrller.dir > 0.0f)//����Ʈ ���� ��ȯ
                    {
                        _AtkEffects[type].transform.localScale = new Vector3(-1, -1, 1);
                        _AtkEffects[type].transform.localEulerAngles = new Vector3(0, 0, 45f);
                    }
                    else if (_playerCtrller.dir < 0.0f)
                    {
                        _AtkEffects[type].transform.localScale = new Vector3(1, -1, 1); ;
                        _AtkEffects[type].transform.localEulerAngles = new Vector3(0, 0, -45f);
                    }
                    _Pos = this.transform.position + _AtkEffects[type].transform.position;
                    break;




                case 8://�����߸�����
                    if (_playerCtrller.dir > 0.0f)//����Ʈ ���� ��ȯ
                    {
                       
                        _AtkEffects[type].transform.localEulerAngles = new Vector3(-90f, 0, 0f);
                    }
                    else if (_playerCtrller.dir < 0.0f)
                    {
                        
                        _AtkEffects[type].transform.localEulerAngles = new Vector3(-90f, 0, 180f);
                    }
                    _Pos = this.transform.position + _AtkEffects[type].transform.position;
                    break;



                case 9://��ų�߸�
                    _AtkEffects[type].transform.position = new Vector3(-7.5f*_playerCtrller.dir, -1.0f, 0f);

                    _AtkEffects[type].transform.localEulerAngles = new Vector3(0f, 90f * _playerCtrller.dir, 10f *_playerCtrller.dir) ;
                    _Pos +=  _AtkEffects[type].transform.position;
                    break;




                case 10://����ų

                    _Pos = this.transform.position + _AtkEffects[type].transform.position;
                    break;


                case 11://�Ʒ���ų1

                    _AtkEffects[type].transform.position = new Vector3(1 * _playerCtrller.dir, 0.6f, 0f);
                    _AtkEffects[type].transform.localEulerAngles = new Vector3(0f, -90* _playerCtrller.dir, 0f);//�¿�
                    _AtkEffects[type].transform.localScale = new Vector3( -1, 1, 1);
                    _Pos = this.transform.position + _AtkEffects[type].transform.position;

                    break;


                case 12://�Ʒ���ų2

                    _AtkEffects[type].transform.localScale = new Vector3( 1, 1, 1);
                    _Pos = this.transform.position + _AtkEffects[type].transform.position;

                    break;

                case 13://�Ʒ���ų3
                    _AtkEffects[type].transform.position = new Vector3(1.6f * _playerCtrller.dir, 0.7f, 0f);
                    _Pos = this.transform.position + _AtkEffects[type].transform.position;
                    break;


                case 14://�¿콺ų
                    _AtkEffects[type].transform.position = new Vector3(-1 * _playerCtrller.dir, 0.7f, 0f);
                    if (_playerCtrller.dir > 0.0f)//����Ʈ ���� ��ȯ
                    {

                        _AtkEffects[type].transform.localEulerAngles = new Vector3(0f, 0, 0f);
                    }
                    else if (_playerCtrller.dir < 0.0f)
                    {

                        _AtkEffects[type].transform.localEulerAngles = new Vector3(0f, 180f, 0f);
                    }
                    _Pos = this.transform.position + _AtkEffects[type].transform.position;
                    break;

                case 15://���
                    _Pos = this.transform.position;
                    break;
                case 16://���
                    _Pos = this.transform.position;
                    break;

            }

            atkgo[type] = Instantiate(_AtkEffects[type], _Pos, _AtkEffects[type].transform.rotation);
        }
        public void EffectOff(int type)
        {
            Destroy(atkgo[type]);
        }
        public void EffDestroyWithTime(int type, float time)
        {
            Destroy(atkgo[type], time);
        }
        void SearchInChildren(Transform parent)
        {
            foreach (Transform child in parent)
            {
                // ������ �����ϴ��� ���θ�   Debug.Log(playertype + "P " + "�ݸ���");�˻�
                if (child.name == "TipOfSword")
                {
                    _TipOfSword = child;

                }

                // �ڽ� ������Ʈ�� �ڽĵ��� �˻��ϱ� ���� ��� ȣ��
                SearchInChildren(child);
            }
        }

        private void Update()
        {
            if (atkgo[6] != null)
                atkgo[6].transform.position = this.transform.position + _AtkEffects[6].transform.position; ;
            if (atkgo[8] != null)
                atkgo[8].transform.position = this.transform.position + _AtkEffects[8].transform.position; ;
            if (atkgo[9] != null)
                atkgo[9].transform.position = _TipOfSword.position + _AtkEffects[9].transform.position; ;
            if (atkgo[10] != null)
                atkgo[10].transform.position = this.transform.position+ _AtkEffects[10].transform.position; ;
            if (atkgo[14] != null)
                atkgo[14].transform.position = this.transform.position + _AtkEffects[14].transform.position; ;

        }

        public void Hitted(Vector3 pos, float time)
        {
          
            _hitgo = Instantiate(_hit, pos, _hit.transform.rotation);
            Destroy(_hitgo, time);
        }
    }

}