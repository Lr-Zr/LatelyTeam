using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.VisualScripting;
using UnityEngine.Playables;
using System.IO.IsolatedStorage;
using UnityEditor.Build;
using UnityEditor.Experimental.GraphView;
using JetBrains.Annotations;
using TMPro;

namespace nara
{

    public class PlayerCtrllerTauro : MonoBehaviour
    {

        [SerializeField]
        int playertype = 1;

        //�̵��ӵ�
        [SerializeField]
        float _MoveSpeed = 10.0f;

        //������
        [SerializeField]
        float _JumpingPower = 13.0f;

        //������
        [SerializeField]
        float _Gauge = 1.0f;

        //���
        [SerializeField]
        public int _Life = 3;

        //�̲������� �ӵ�
        [SerializeField]
        float _SlideSpeed = 20.0f;

        //�ϰ�Ű ������ ���ǵ�
        [SerializeField]
        float _DownSpeed = 5.0f;

        //���� ��  �ൿ����
        [SerializeField]
        float _JumpRestriction = 0.25f;

        //���� ���� �ð�
        [SerializeField]
        float _RunRestriction = 1.0f;


        //�����ð�
        [SerializeField]
        float _BreakingTime = 0.25f;
        //�Ŀ�
        [SerializeField]
        public Vector3 _Power = Vector3.zero;

        [SerializeField]
        public float Dmg = 0.0f;

        //�������� ���ǵ�
        [SerializeField]
        float RLspeed = 1000f;

        [SerializeField]
        float Upspeed = 20f;


        Rigidbody _Rigid;
        PlayerAnimation _Anim;
        PlayerEffectTauro _Eff;


        public PlayerState _State;
        //���� �¿� move�Լ��� �� ����Ʈ ���� ��ȯ
        public int dir;
        public int airdir;
        //time
        public float _RunTime = 0.0f;
        float _BreakTime = 0.0f;
        float _JumpTime = 0.3f;
        float _Floortime = 0.0f;//
        float _GodTime = 0.0f;

        //���� ���� �ð� �� ����
        float _AttackTime = 0.0f;
        bool _CantAttack;//

        public bool _IsAttack;
        public bool _IsSkill;
        bool _IsJump;
        bool _IsDJump;
        bool _IsOnesec;//���� ���� 
        public bool _IsRunning;


        float _Teemo;

        /* ���� ��ų ���� ���� */
        bool _IsRLMove;//�������� �̵�����
        bool _IsUpMove;//����ų �̵�����
        bool _IsAirAtk;//�߷�0����
        //keydown time;
        float _KDwTime = 0f;
        float _KUpTime = 0f;
        float _KQTime = 0f;//����Ű ������
        float _KWTime = 0f;//��ųŰ ������

        //keydown bool;
        bool _IsKeyDown;
        bool _IsKeyUp;
        bool _IsKeyQ;
        bool _IskeyE;
        //�Ÿ� ������ 
        Vector3 _MovePos;
        Vector3 _PrePos;



        void Start()
        {

            _Rigid = GetComponent<Rigidbody>();
            _Anim = GetComponent<PlayerAnimation>();
            _Eff = GetComponent<PlayerEffectTauro>();
            GameMgr.Input.KeyAction -= OnKeyboard;
            GameMgr.Input.KeyAction += OnKeyboard;
            _State = PlayerState.Idle;
            _IsJump = false;
            _IsDJump = false;
            _IsOnesec = false;
            _IsRunning = false;
            _IsKeyDown = false;
            _IsKeyUp = false;
            _IsAttack = false;
            _IsRLMove = false;
            _IsUpMove = false;
            _IsAirAtk = false;
            playertype = 0;
        }

        private void FixedUpdate()
        {


            //�޸��ٰ� ���ߴ� ����
            _Floortime += Time.deltaTime;
            //���� ����
            if (_CantAttack)
            {
                _AttackTime -= Time.deltaTime;
                if (_AttackTime < 0f)
                {
                    _IsAttack = false;
                    _CantAttack = false;
                    _IsSkill = false;
                    _Anim.SetIsAttack(_IsAttack);
                    _Anim.SetIsSkill(_IsSkill);
                }
            }



            if (_IsAirAtk)
                _Rigid.velocity = Vector3.zero;

            if (_IsRLMove)//�Ÿ��� ����̻�Ǹ� �����.
            {
                float x = Vector3.Distance(_PrePos, _MovePos);
                if (x < _Teemo)
                    _Rigid.AddForce(this.transform.forward * RLspeed, ForceMode.Force);
            }
            if (_IsUpMove)
            {
                _IsAirAtk = false;
                _Rigid.AddForce(this.transform.up * Upspeed, ForceMode.Impulse);
                _IsUpMove = false;
            }


            //Ŀ�ǵ� Ű�Է�
            _KUpTime -= Time.deltaTime;
            _KDwTime -= Time.deltaTime;
            _KQTime -= Time.deltaTime;
            _KWTime -= Time.deltaTime;
            if (_KUpTime < 0) _IsKeyUp = false;
            else _IsKeyUp = true;
            if (_KDwTime < 0) _IsKeyDown = false;
            else _IsKeyDown = true;
            _Anim.SetIsUpKey(_IsKeyUp);
            _Anim.SetIsDwKey(_IsKeyDown);

            //ü�� �ð�
            if (_IsJump)
                _JumpTime += Time.deltaTime;

            //�극��ŷ ���� ���� ���� RunTime;
            if (_RunTime > _RunRestriction && !_IsJump)
            {
                _RunTime = _RunRestriction;
                _IsOnesec = true;
                _Anim.SetIsOnesec(_IsOnesec);
                _BreakTime = _BreakingTime;
            }
            //�޸����� �޸��� �ʴ��� �Ǵ�.

            if (_RunTime > 0) _IsRunning = true;
            else _IsRunning = false;
            _Anim.SetIsRunning(_IsRunning);


            //Runtime �ʱ�ȭ;
            if (_State != PlayerState.Running && !_IsAttack && !_IsOnesec && !_IsSkill)
            {
                _RunTime = 0.0f;
            }


            //���ϻ��� 
            if (_Rigid.velocity.y < -0.05f && _IsJump && !_IsAttack && !_IsSkill)//����
            {
                SetState(PlayerState.Falling);

            }

        }
        void Update()
        {


            //�ٴڿ� �ִ��� üũ �Լ�;
            OnFloor();
            //�극��ŷ �κ�
            Breaking();

        }
        private void LateUpdate()
        {
            //_Eff.EffectPlay(Effect.Test);
        }
        void OnKeyboard()
        {
            if (playertype == 1)
            {
                if (Input.GetKey(KeyCode.S))//���ձ� �� �� �ϰ� �ӵ� ���
                {

                    _KUpTime = 0.0f;
                    _KDwTime = 0.2f;
                    if (_IsJump && _JumpTime > _JumpRestriction && !_IsAttack && !_IsSkill)
                        Move(0);
                }

                else if (Input.GetKey(KeyCode.W))//����Ű ��
                {
                    _KUpTime = 0.2f;
                    _KDwTime = 0.0f;


                }

                /* �̵� �Է� */
                if (Input.GetKey(KeyCode.A))//�� �̵�
                {
                    if (!_IsAttack && !_IsSkill)
                    {
                        dir = -1;
                        Move(dir);
                    }

                }
                if (Input.GetKey(KeyCode.D))//�� �̵�
                {
                    if (!_IsAttack && !_IsSkill)
                    {
                        dir = 1;
                        Move(dir);

                    }

                }


                if (Input.GetKeyDown(KeyCode.F))
                {
                    if (_KQTime < 0.0f && !_IsSkill)
                    {
                        _KQTime = 0.3f;
                        Attack();

                    }
                }
                //else if (Input.GetKey(KeyCode.Q)) //����
                //{

                //}

                else if (Input.GetKeyDown(KeyCode.G))//��ų
                {
                    if (_KWTime < 0.0f && !_IsAttack && !_IsSkill)
                    {
                        _KWTime = 0.3f;
                        Skill();
                    }
                }

                else if (Input.GetKey(KeyCode.H))//���
                {

                }

                /* ���� */
                if (Input.GetKey(KeyCode.Space))
                {
                    Jump();

                }
            }
            else
            {
                if (Input.GetKey(KeyCode.DownArrow))//���ձ� �� �� �ϰ� �ӵ� ���
                {

                    _KUpTime = 0.0f;
                    _KDwTime = 0.2f;
                    if (_IsJump && _JumpTime > _JumpRestriction && !_IsAttack && !_IsSkill)
                        Move(0);
                }

                else if (Input.GetKey(KeyCode.UpArrow))//����Ű ��
                {
                    _KUpTime = 0.2f;
                    _KDwTime = 0.0f;


                }

                /* �̵� �Է� */
                if (Input.GetKey(KeyCode.LeftArrow))//�� �̵�
                {
                    if (!_IsAttack && !_IsSkill)
                    {
                        dir = -1;
                        Move(dir);
                    }

                }
                if (Input.GetKey(KeyCode.RightArrow))//�� �̵�
                {
                    if (!_IsAttack && !_IsSkill)
                    {
                        dir = 1;
                        Move(dir);

                    }

                }


                if (Input.GetKeyDown(KeyCode.L))
                {
                    if (_KQTime < 0.0f && !_IsSkill)
                    {
                        _KQTime = 0.3f;
                        Attack();

                    }
                }
                //else if (Input.GetKey(KeyCode.Q)) //����
                //{

                //}

                else if (Input.GetKeyDown(KeyCode.Semicolon))//��ų
                {
                    if (_KWTime < 0.0f && !_IsAttack && !_IsSkill)
                    {
                        _KWTime = 0.3f;
                        Skill();
                    }
                }

                else if (Input.GetKey(KeyCode.Quote))//���
                {

                }

                /* ���� */
                if (Input.GetKey(KeyCode.RightShift))
                {
                    Jump();

                }
            }
            //1p//////////////////////////////////////////////////////////////////////////////
            /* ���� �Է� */

            //1p//////////////////////////////////////////////////////////////////////////////
        }




        void Jump()
        {

            if (_IsAttack || _IsSkill) return;
            _RunTime = 0;
            _IsOnesec = false;

            _Anim.SetIsOnesec(_IsOnesec);

            if (!_IsJump && _State != PlayerState.Falling)
            {

                _Rigid.velocity = Vector3.zero;
                _Rigid.AddForce(Vector3.up * _JumpingPower, ForceMode.Impulse);

                _JumpTime = 0;
                SetState(PlayerState.Jumping);
                _IsJump = true;
                _Anim.SetIsJump(_IsJump);

                if (dir > 0)
                    _Eff.EffectPlay(Effect.RJump);
                else
                    _Eff.EffectPlay(Effect.LJump);
            }
            else if (!_IsDJump && _JumpTime > 0.25)
            {
                _Rigid.velocity = Vector3.zero;
                _Rigid.AddForce(Vector3.up * _JumpingPower, ForceMode.Impulse);

                SetState(PlayerState.DoudbleJumping);
                _IsDJump = true;
                _Anim.SetIsDJump(_IsDJump);
                _Eff.EffectPlay(Effect.DJump);
            }

        }



        void Move(int dir)      /*�̵� �� ������ȯ*/
        {
            _IsRunning = true;
            _Anim.SetIsRunning(_IsRunning);
            _RunTime += Time.deltaTime;
            if (!_IsJump)
            {

                if (dir == 0) return;
                SetState(PlayerState.Running);

                this.transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.right * dir), 1f);
                this.transform.position += transform.forward * _MoveSpeed * Time.deltaTime;


                if (dir > 0)
                {

                    _Eff.EffectPlay(Effect.RRun);
                }
                else
                {
                    _Eff.EffectPlay(Effect.LRun);
                }
            }
            else
            {


                if (dir == 0)
                {
                    this.transform.position -= Vector3.up * _DownSpeed * Time.deltaTime;

                }
                else
                {
                    this.transform.position += Vector3.right * dir * _MoveSpeed / 2.0f * Time.deltaTime;
                }
            }
        }




        void Breaking()
        {
            //�޸��ٰ� ���߸� �̲�����
            if (_IsOnesec && !_IsJump)//ź�� ȿ��
            {
                if (_BreakTime > 0)
                {
                    //Debug.Log("�̵�");
                    _Rigid.AddForce(this.transform.forward * _SlideSpeed);
                    _BreakTime -= Time.deltaTime;
                }
                else
                {

                    _RunTime = 0.0f;
                    _IsOnesec = false;

                    _Anim.SetIsOnesec(_IsOnesec);
                    if (dir > 0)
                        _Eff.EffectPlay(Effect.RBreak);
                    else
                        _Eff.EffectPlay(Effect.LBreak);
                }

            }
            _PrePos = transform.position;
        }


        void SetState(PlayerState t)
        {
            _State = t;
            _Anim.SetAnim(_State);
        }

        void Attack()
        {

            if (_IsKeyDown)//�Ʒ�Ű�� ������
            {
                if (_IsJump)//���������϶�
                {
                    //���߾Ʒ����� 
                    if (!_IsAttack)
                    {
                        this.transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.right * dir), 1f);
                        SetState(PlayerState.AirDwAttack);

                    }
                }
                else//�������°� �ƴ� ��
                {
                    //�Ʒ����� X

                    SetState(PlayerState.DwAttack);
                }
            }
            else if (_IsKeyUp) //��Ű�� ��������
            {
                //������
                _IsAirAtk = true;//�̵��� ����
                SetState(PlayerState.UpAttack);//1.67


            }
            else //�Ʒ�, ��Ű�Է� ���� ����
            {

                if (_IsJump)//���� ����
                {
                    if (_IsRunning)//�޸��� ����
                    {

                        //������������
                        _Teemo = 3.0f;
                        _IsAirAtk = true;

                        this.transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.right * dir), 1f);
                        SetState(PlayerState.RLAttack);

                    }
                    else
                    {
                        //�����߸�����


                        this.transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.right * dir), 1f);
                        SetState(PlayerState.AirNormalAttack);




                    }
                }
                else//���� �ִ� ����
                {
                    if (_IsRunning)//�޸��� ����
                    {


                        if (_IsOnesec)//1����������
                        {
                            //�����ְ���-
                            SetState(PlayerState.RunAttack);
                            Debug.Log("������");
                            _Teemo = 2.0f;
                        }
                        else
                        {
                            //����������
                            SetState(PlayerState.RLAttack);
                            _Teemo = 3.0f;
                            //Debug.Log("��������");
                        }
                    }
                    else
                    {
                        //���߸�����

                        Debug.Log("�� �⺻ ����");
                        if (!_IsKeyQ && !_CantAttack)//Q�߸������� qŰ�� ������ ��
                        {
                            if (_State == PlayerState.NormalAttack)
                            {
                                SetState(PlayerState.NormalAttack2);

                                _IsKeyQ = true;
                            }
                            else if (_State == PlayerState.NormalAttack2)
                            {
                                SetState(PlayerState.NormalAttack3);
                                _IsKeyQ = true;
                            }
                        }
                        if (!_IsAttack)
                            SetState(PlayerState.NormalAttack);



                    }
                }
            }

            if (!_IsAttack)
            {

                _Anim.TriggerAtk();
                _IsAttack = true;
                _Anim.SetIsAttack(_IsAttack);

            }
        }



        /// <summary>
        /// ///////////////////////////////////////////////////////////////
        /// </summary>
        void Skill()
        {

            if (_IsKeyDown)//�Ʒ�Ű�� ������
            {
                if (_IsJump)//���������϶�
                {
                    //�Ʒ�����

                    _IsAirAtk = true;//�̵��� ����
                    SetState(PlayerState.DwSkill);

                }
                else//�������°� �ƴ� ��
                {
                    SetState(PlayerState.DwSkill);

                }
            }
            else if (_IsKeyUp) //��Ű�� ��������
            {
                //������
                _IsAirAtk = true;
                _IsJump = true;
                _Anim.SetIsJump(_IsJump);
                SetState(PlayerState.UpSkill);

            }
            else //�Ʒ�, ��Ű�Է� ���� ����
            {

                if (_IsJump)//���� ����
                {
                    if (_IsRunning)//�޸��� ����
                    {
                        //������������
                        _IsAirAtk = true;//�̵��� ����
                        this.transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.right * dir), 1f);

                        SetState(PlayerState.RLSkill);
                        _Teemo = 10f;
                    }
                    else
                    {
                        //���� �߸�����
                        _IsAirAtk = true;//�̵��� ����
                        this.transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.right * dir), 1f);
                        SetState(PlayerState.NormalSkill);

                    }
                }
                else//���� �ִ� ����
                {
                    if (_IsRunning)//�޸��� ����
                    {
                        //����������
                        SetState(PlayerState.RLSkill);
                        _Teemo = 10f;
                    }
                    else
                    {
                        //���߸�����
                        SetState(PlayerState.NormalSkill);

                    }
                }

            }


            if (!_IsSkill)
            {
                Debug.Log("ss");
                _Anim.TriggerSkill();
                _IsSkill = true;
                _Anim.SetIsSkill(_IsSkill);

            }
        }


        void OnFloor()
        {
            RaycastHit hit;
            Vector3 StartPos = transform.position;
            StartPos.y += 0.1f;
            Debug.DrawRay(StartPos, this.transform.up * -0.2f, Color.green);
            LayerMask mask = LayerMask.GetMask("Floor");
            if (Physics.Raycast(StartPos, this.transform.up * -1, out hit, 0.2f, mask))
            {
                //&& _Floortime > 0.05
                if (_State != PlayerState.Idle && _JumpTime > _JumpRestriction)
                {
                    if (_IsJump)
                        _Eff.EffectPlay(Effect.Land);
                    _Rigid.velocity = Vector3.zero;
                    _Floortime = 0.0f;
                    if (!_IsAttack && !_IsSkill)
                    {
                        _State = PlayerState.Idle;
                        _Anim.SetAnim(_State);

                        _IsJump = false;
                    }
                    _IsRunning = false;
                    _Anim.SetIsRunning(_IsRunning);
                    _IsDJump = false;
                    _Anim.SetIsJump(_IsJump);
                    _Anim.SetIsDJump(_IsDJump);


                    _IsAirAtk = false;

                }

            }
        }




        public void onRLMove()
        {
            _IsRLMove = true;
            _MovePos = this.transform.position;
        }
        public void onUpMove()
        {
            Debug.Log("upmove");
            _IsUpMove = true;
            _MovePos = this.transform.position;
        }

        public void setinit()//�ʱ�ȭ
        {
            if (_IsKeyQ)
            {
                _IsKeyQ = false;

            }
            else
            {
                _CantAttack = true;
                _AttackTime = 0.05f;

                //    _IsAttack = false;
                _IsRLMove = false;
                _IsUpMove = false;
                _IsAirAtk = false;

                _Anim.TriggerReset();
            }


        }

        public void TypeOfPlayer(int type)
        {
            if (type == 1)
            {
                dir = 1;

                this.transform.eulerAngles = new Vector3(0, -90f, 0);
                //��ġ �ֱ�
            }
            else
            {
                dir = -1;
                this.transform.eulerAngles = new Vector3(0, -90f, 0);
                //��ġ �ֱ�
            }
        }

    }





}



