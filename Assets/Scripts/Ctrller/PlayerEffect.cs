using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

namespace nara
{


    public class PlayerEffect : MonoBehaviour
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


        PlayerCtrller _playerCtrller;
        [SerializeField]
        GameObject _hit;
        GameObject _hitgo;
        [SerializeField]
        GameObject _out;
        GameObject _outgo;

        void Start()
        {
            go = new GameObject[(int)Effect.End];
            atkgo = new GameObject[(int)AtkEffect1.End];
            SearchInChildren(this.transform);
            _playerCtrller = GetComponent<PlayerCtrller>();
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
            if (type != 22)
            {

                if (atkgo[type] != null) return;
            }

            _Pos = _TipOfSword.position;
            switch (type)
            {
                case 0://Up1
                    _Pos = this.transform.position + _AtkEffects[type].transform.position; //ĳ������ġ + ����Ʈ�� ������ �ִ� ������

                    break;
                case 1://Up2
                    break;
                case 2://RL1
                    _Pos = this.transform.position + _AtkEffects[type].transform.position; //ĳ������ġ + ����Ʈ�� ������ �ִ� ������
                    break;
                case 3://RL2
                    if (_playerCtrller.dir > 0.0f)//����Ʈ ���� ��ȯ
                    {
                        _AtkEffects[type].transform.localScale = new Vector3(1, 1, 1);
                    }
                    else if (_playerCtrller.dir < 0.0f)
                    {
                        _AtkEffects[type].transform.localScale = new Vector3(-1, 1, 1);
                    }

                    break;
                case 4://Dw1
                    _Pos = this.transform.position + _AtkEffects[type].transform.position;
                    break;
                case 5://Dw2
                    break;
                case 6://�⺻����1
                    if (_playerCtrller.dir > 0.0f)//����Ʈ ���� ��ȯ
                    {
                        _AtkEffects[type].transform.localScale = new Vector3(1, 1, 1);
                    }
                    else if (_playerCtrller.dir < 0.0f)
                    {
                        _AtkEffects[type].transform.localScale = new Vector3(-1, 1, 1); ;
                    }
                    _Pos = this.transform.position + _AtkEffects[type].transform.position;
                    break;
                case 7://�⺻����2
                    if (_playerCtrller.dir > 0.0f)//����Ʈ ���� ��ȯ
                    {
                        _AtkEffects[type].transform.localScale = new Vector3(1, -1, 1);
                        _AtkEffects[type].transform.localEulerAngles = new Vector3(-90f, 0, 0);
                    }
                    else if (_playerCtrller.dir < 0.0f)
                    {
                        _AtkEffects[type].transform.localScale = new Vector3(-1, -1, 1); ;
                        _AtkEffects[type].transform.localEulerAngles = new Vector3(90f, 0, 0);
                    }
                    _Pos = this.transform.position + _AtkEffects[type].transform.position;
                    break;
                case 8://�⺻����3
                    if (_playerCtrller.dir > 0.0f)//����Ʈ ���� ��ȯ
                    {
                        _AtkEffects[type].transform.localScale = new Vector3(1, 1, 1);
                        _AtkEffects[type].transform.localPosition = new Vector3(1f, 0.6f, 0f);
                    }
                    else if (_playerCtrller.dir < 0.0f)
                    {
                        _AtkEffects[type].transform.localScale = new Vector3(-1, 1, 1);
                        _AtkEffects[type].transform.localPosition = new Vector3(-1f, 0.6f, 0f);
                    }
                    _Pos = this.transform.position + _AtkEffects[type].transform.position;
                    break;
                case 9:
                    _Pos = this.transform.position + _AtkEffects[type].transform.position;

                    break;
                case 10://���߾Ʒ�����
                    if (_playerCtrller.dir > 0.0f)//����Ʈ ���� ��ȯ
                    {
                        _AtkEffects[type].transform.localScale = new Vector3(1, 1, 1);
                    }
                    else if (_playerCtrller.dir < 0.0f)
                    {
                        _AtkEffects[type].transform.localScale = new Vector3(-1, 1, 1);
                    }
                    _Pos = this.transform.position + _AtkEffects[type].transform.position;

                    break;
                case 11://���߱⺻����
                    if (_playerCtrller.dir > 0.0f)//����Ʈ ���� ��ȯ
                    {
                        _AtkEffects[type].transform.localScale = new Vector3(1, 1, 1);
                    }
                    else if (_playerCtrller.dir < 0.0f)
                    {
                        _AtkEffects[type].transform.localScale = new Vector3(-1, 1, 1);
                    }
                    _Pos += _AtkEffects[type].transform.position;
                    break;
                case 12://����ų1
                    break;
                case 13://����ų2
                    break;
                case 14://����ų3
                    break;
                case 15://RL��ų1
                    if (_playerCtrller.dir > 0.0f)//����Ʈ ���� ��ȯ
                    {

                        _AtkEffects[type].transform.localEulerAngles = new Vector3(0, 0, 0);
                    }
                    else if (_playerCtrller.dir < 0.0f)
                    {
                        _AtkEffects[type].transform.localEulerAngles = new Vector3(0, 180f, 0);
                    }
                    _Pos = this.transform.position + _AtkEffects[type].transform.position;
                    break;
                case 16://RL��ų2
                    if (_playerCtrller.dir > 0.0f)//����Ʈ ���� ��ȯ
                    {

                        _AtkEffects[type].transform.localPosition = new Vector3(-4f, 0.6f, 0f);
                        _AtkEffects[type].transform.localEulerAngles = new Vector3(0, 0, 0);
                    }
                    else if (_playerCtrller.dir < 0.0f)
                    {

                        _AtkEffects[type].transform.localPosition = new Vector3(4f, 0.6f, 0f);
                        _AtkEffects[type].transform.localEulerAngles = new Vector3(0, 180f, 0);
                    }
                    _Pos = this.transform.position + _AtkEffects[type].transform.position;
                    break;
                case 17://Dw��ų1
                    _Pos = this.transform.position + _AtkEffects[type].transform.position;
                    break;
                case 18://Dw��ų2
                    break;
                case 19://�븻��ų1

                    _Pos = this.transform.position + _AtkEffects[type].transform.position;
                    break;
                case 20://�븻��ų2
                    if (_playerCtrller.dir > 0.0f)//����Ʈ ���� ��ȯ
                    {

                        _AtkEffects[type].transform.localEulerAngles = new Vector3(90f, 90f, 0);
                    }
                    else if (_playerCtrller.dir < 0.0f)
                    {

                        _AtkEffects[type].transform.localEulerAngles = new Vector3(-90f, 90f, 0);
                    }
                    _Pos = this.transform.position + _AtkEffects[type].transform.position;
                    break;
                case 21://���
                    _Pos = this.transform.position;
                    break;
                case 22://�˾ƿ�
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
                // ������ �����ϴ��� ���θ� �˻�
                if (child.name == "TipofSword")
                {
                    _TipOfSword = child;

                }

                // �ڽ� ������Ʈ�� �ڽĵ��� �˻��ϱ� ���� ��� ȣ��
                SearchInChildren(child);
            }
        }

        private void Update()
        {
            if (atkgo[10] != null)
                atkgo[10].transform.position = this.transform.position + _AtkEffects[10].transform.position; ;
            if (atkgo[11] != null)
                atkgo[11].transform.position = _TipOfSword.position + _AtkEffects[11].transform.position; ;

            if (atkgo[17] != null)
                atkgo[17].transform.position = this.transform.position + _AtkEffects[17].transform.position; ;
        }


        public void Hitted(Vector3 pos, float time)
        {

            _hitgo = Instantiate(_hit, pos, _hit.transform.rotation);
            Destroy(_hitgo, time);
        }

        public void Dead(Vector3 pos)
        {
            pos += new Vector3(0, 0, -2f);

            float dx = pos.x;
            float dy = pos.y - 5;
            float degree = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg;
            degree = (degree + 180) % 360;
            _out.transform.localEulerAngles = new Vector3(0, 0, degree);
            _outgo = Instantiate(_out, pos, _out.transform.rotation);


            Destroy(_outgo, 2);
        }
    }

}