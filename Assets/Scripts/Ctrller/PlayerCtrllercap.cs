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

    public class PlayerCtrllercap : MonoBehaviour
    {


        [SerializeField]
        string _Name = "Cap";

        [SerializeField]
        public int playertype = 2;

        //�̵��ӵ�
        [SerializeField]
        float _MoveSpeed = 10.0f;

        //������
        [SerializeField]
        float _JumpingPower = 13.0f;

        //������
        [SerializeField]
        public float _Gauge = 1.0f;

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
        PlayerEffectcap _Eff;
        PlayerColliderscap _Cols;
        GameObject _objgauge;

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
        bool _IsKnockOut;
        bool _IsShield;
        public float _ECooldown = 0f;//���Ű ������
        bool _IsRespawn;


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

        float stuntime = 0.1f;//�����ð�
        float outtime;//�˾ƿ� �ð�//
        float startime;//�˾ƿ� ����Ʈ �����ð�;


        //test
        [SerializeField]
        float kt1;
        [SerializeField]
        float kt2;
        [SerializeField]
        float kt3;
        [SerializeField]
        float kt4;
        [SerializeField]
        float kt5;

        Vector3 hittedPower;
        //���� �� 
        float RespawnTime;
        //������ ������
        Vector3 P1ResPos;
        Vector3 P2ResPos;
        int _BodyTouchWay;

        public bool god;
        void Start()
        {

            _Rigid = GetComponent<Rigidbody>();
            _Anim = GetComponent<PlayerAnimation>();
            _Eff = GetComponent<PlayerEffectcap>();
            _Cols = GetComponent<PlayerColliderscap>();
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
            playertype = 2;
            P1ResPos = new Vector3(-4.5f, 9f, 0f);
            P2ResPos = new Vector3(4.5f, 9f, 0f);
            god = false;
            TypeOfPlayer(2);
        }

        private void FixedUpdate()
        {
            if (god)
            {
                this.transform.position = _PrePos;
            }
            if (RespawnTime > 0f)
            {
                RespawnTime -= Time.deltaTime;
                if (_IsRespawn == true)
                {

                    if (RespawnTime < 4.0f)
                    {
                        GameMgr.Instance.onRespawnPlat(playertype);
                        _Gauge = 0;
                        if (playertype == 1)
                        {
                            this.transform.position = P1ResPos;
                        }
                        else
                        {
                            this.transform.position = P2ResPos;

                        }

                        this.transform.eulerAngles = new Vector3(0, 180, 0);
                        _IsRespawn = false;
                    }
                    else
                    {
                        _Rigid.velocity = Vector3.zero;
                    }

                }
                if (RespawnTime < 0f)
                {
                    god = false;
                    GameMgr.Instance.offRespawnPlat(playertype);
                    _IsJump = true;
                    _Anim.SetIsJump(_IsJump);
                }
            }
            //���Ű ��Ÿ�� 
            _ECooldown -= Time.deltaTime;

            if (_IsShield)
            {
                if (_ECooldown < 5.0f)
                {
                    Debug.Log("test1");
                    _IsShield = false;
                    god = false;
                    _Eff.EffectOff(15);
                }

            }
            //�˾ƿ� ���¿��� �̵�
            if (_IsKnockOut)
            {
                startime += Time.deltaTime;
                if (startime > 0.05f)
                {
                    //����Ʈ ����
                    _Eff.EffectOn(16);
                    _Eff.EffDestroyWithTime(16, 1);
                    startime = 0;
                }
                if (outtime < 0)
                {
                    _IsKnockOut = false;
                    _Rigid.velocity = Vector3.zero;
                    startime = 0;

                }
                KnockOutMove();

                outtime -= Time.deltaTime;
            }
            stuntime -= Time.deltaTime;


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
                {
                    _Rigid.AddForce(this.transform.forward * RLspeed, ForceMode.Force);
                    Debug.Log("cap " + RLspeed);
                }
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
            if (_Rigid.velocity.y < -0.05f && !_IsAttack && !_IsSkill && stuntime < 0f && !_IsKnockOut)//����
            {
                _IsOnesec = false;
                _Anim.SetIsOnesec(_IsOnesec);
                SetState(PlayerState.Falling);

            }
            Breaking();
        }
        void Update()
        {
            if (_objgauge)
            {
                _objgauge.GetComponent<PlayerGauge>()._gauge = _Gauge;
                _objgauge.transform.position = this.transform.position;

            }

            //�ٴڿ� �ִ��� üũ �Լ�;
            OnFloor();
            //�극��ŷ �κ�


        }
        private void LateUpdate()
        {
            //_Eff.EffectPlay(Effect.Test);
        }
        void OnKeyboard()
        {

            if ((_IsKnockOut || _IsShield) || stuntime >= 0f || RespawnTime > 0f) return;
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
                    if ((_State == PlayerState.Idle || _State == PlayerState.Running) && _ECooldown < 0f)
                    {
                        SetState(PlayerState.Sheild);
                        _IsShield = true;
                        god = true;
                        _ECooldown = 6.0f;
                        _Anim.TriggerShield();

                    }
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
                    if ((_State == PlayerState.Idle || _State == PlayerState.Running) && _ECooldown < 0f)
                    {
                        SetState(PlayerState.Sheild);
                        _IsShield = true;
                        god = true;
                        _ECooldown = 6.0f;
                        _Anim.TriggerShield();

                    }
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
                if (_BodyTouchWay != dir)
                {
                    Debug.Log("touchCAP");
                    this.transform.position += transform.forward * _MoveSpeed * Time.deltaTime;
                }

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
                    if (_BodyTouchWay != dir)
                    {
                        Debug.Log("touchCAP");
                        this.transform.position += Vector3.right * dir * _MoveSpeed / 2.0f * Time.deltaTime;
                    }
                }
            }
        }




        void Breaking()
        {
            //�޸��ٰ� ���߸� �̲�����
            if (_IsOnesec && !_IsJump && _State == PlayerState.Idle && _State == PlayerState.Idle && !_IsAttack && !_IsSkill)//ź�� ȿ��
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
            _State = t;//�ڵ峻�� ����
            _Anim.SetAnim(_State);//�ִϸ������ǻ���
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
                            _Teemo = 2.0f;
                            SetState(PlayerState.RunAttack);
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
                    this.transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.right * dir), 1f);

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
                        _Teemo = 10f;
                        _IsAirAtk = true;//�̵��� ����
                        this.transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.right * dir), 1f);

                        SetState(PlayerState.RLSkill);
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
                        _Teemo = 10f;
                        //����������
                        _IsAirAtk = true;
                        SetState(PlayerState.RLSkill);
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
            Debug.DrawRay(StartPos, this.transform.up * -0.15f, Color.green);
            LayerMask mask = LayerMask.GetMask("Floor");
            if (Physics.Raycast(StartPos, this.transform.up * -1, out hit, 0.15f, mask))
            {
                //&& _Floortime > 0.05
                if (_State != PlayerState.Idle && _JumpTime > _JumpRestriction)
                {
                    if (_IsJump)
                        _Eff.EffectPlay(Effect.Land);
                    _Rigid.velocity = Vector3.zero;
                    _Floortime = 0.0f;
                    if (!_IsAttack && !_IsSkill && !_IsShield && stuntime < 0f && !_IsKnockOut)
                    {
                        _State = PlayerState.Idle;
                        _Anim.SetAnim(_State);

                        _IsJump = false;
                        _Anim.SetIsJump(_IsJump);
                    }
                    _IsRunning = false;
                    _Anim.SetIsRunning(_IsRunning);
                    _IsDJump = false;
                    _Anim.SetIsDJump(_IsDJump);



                }

            }
        }




        public void onRLMove()
        {
            if (_IsRespawn) return;

            _IsRLMove = true;
            _MovePos = this.transform.position;
        }
        public void onUpMove()
        {
            if (_IsRespawn) return;

            _IsUpMove = true;
        }

        public void setinit()//�ʱ�ȭ
        {
            if (_IsKeyQ)
            {
                _IsKeyQ = false;

            }
            else
            {
                _IsOnesec = false;

                _Anim.SetIsOnesec(_IsOnesec);
                _Rigid.velocity = Vector3.zero;
                _CantAttack = true;
                _AttackTime = 0.05f;

                //    _IsAttack = false;
                _IsRLMove = false;
                _IsUpMove = false;
                _IsAirAtk = false;

                _Cols.offhitted();
                _Anim.TriggerReset();
            }


        }

        private void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.tag == "Floor")
            {
                _IsOnesec = false;
                _IsJump = true;
                _Anim.SetIsJump(_IsJump);
                _Anim.SetIsOnesec(_IsOnesec);
                if (_IsAttack || _IsSkill)
                {
                    _RunTime = 0;
                    if (!_IsKeyUp)

                        _IsAirAtk = true;

                }
            }
            if (collision.gameObject.tag == "1P" || collision.gameObject.tag == "2P")
                _BodyTouchWay = 0;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (RespawnTime > 0) return;


            if (other.gameObject.tag == "DeadLine")
            {
                Debug.Log("������� ��ġ");
                god = true;
                setinit();

                RespawnTime = 6.0f;
                _IsRespawn = true;
                _Eff.Dead(this.transform.position);//����Ʈ ����

            }

            if (_IsKnockOut || _IsShield) return;


            //��ų ������ ��������.
            if (playertype == 1)
            {
                if (other.gameObject.tag == "2PA")
                {

                    PlayerCtrller pc = other.transform.root.GetComponent<PlayerCtrller>();
                    this.dir = -pc.dir;
                    pc.transform.localEulerAngles = new Vector3(0, dir * 90, 0);
                    if (pc._Power.x == 0 && pc._Power.y == 0) //����
                    {
                        setinit();
                        stuntime = 0.2f;
                        _Gauge += pc.Dmg;
                        _RunTime = 0;
                        _IsOnesec = false;
                    }
                    else//�˾ƿ�
                    {

                        setinit();
                        _Gauge += pc.Dmg;
                        hittedPower = pc._Power * _Gauge;
                        _IsKnockOut = true;
                        setOutTime();
                        SetState(PlayerState.KnockOut);
                        _Anim.TriggerKnockOut();
                    }



                    _Eff.Hitted(other.transform.position, 1);

                }

            }
            else
            {
                if (other.gameObject.tag == "1PA")
                {

                    Debug.Log("kk");
                    PlayerCtrller pc = other.transform.root.GetComponent<PlayerCtrller>();
                    dir = -pc.dir;
                    this.transform.localEulerAngles = new Vector3(0, dir * 90, 0);
                    if (pc._Power.x == 0 && pc._Power.y == 0) //����
                    {
                        setinit();
                        stuntime = 0.2f;
                        _Gauge += pc.Dmg;
                        _RunTime = 0;
                        _IsOnesec = false;
                    }
                    else//�˾ƿ�
                    {
                        setinit();
                        _Gauge += pc.Dmg;
                        hittedPower = pc._Power * _Gauge;
                        _IsKnockOut = true;

                        setOutTime();
                        SetState(PlayerState.KnockOut);
                        _Anim.TriggerKnockOut();

                    }

                    _Eff.Hitted(other.transform.position, 1);
                }
            }


        }
        private void OnCollisionEnter(Collision other)
        {
            if (_IsSkill || _IsAttack)
            {
                if (other.gameObject.name == "N_Aries")
                {
                    PlayerCtrller pc = other.gameObject.GetComponent<PlayerCtrller>();

                    if (playertype == 1)
                    {
                        if (other.gameObject.tag == "2P")
                        {

                            if (pc.god)
                            {

                                _IsRLMove = false;
                            }
                        }
                    }
                    else
                    {
                        if (other.gameObject.tag == "1P")
                        {

                            if (pc.god)
                            {

                                _IsRLMove = false;
                            }
                        }
                    }
                }
                else
                {

                    PlayerCtrllercap pc = other.gameObject.GetComponent<PlayerCtrllercap>();

                    if (playertype == 1)
                    {
                        if (other.gameObject.tag == "2P")
                        {
                            if (pc.god)
                            {

                                _IsRLMove = false;
                            }

                        }
                    }
                    else
                    {
                        if (other.gameObject.tag == "1P")
                        {
                            if (pc.god)
                            {

                                _IsRLMove = false;
                            }

                        }
                    }
                }


            }
        }

            ///�̵�����
            private void OnCollisionStay(Collision other)
        {
            //�ƴ����� Ŀ����Ʈ���� Ȯ���Ѵ����� �����;���

            if (_IsSkill || _IsAttack)
            {
                if (other.gameObject.name == "N_Aries")
                {
                    PlayerCtrller pc = other.gameObject.GetComponent<PlayerCtrller>();
                }
                else
                {

                    PlayerCtrller pc = other.gameObject.GetComponent<PlayerCtrller>();
                }

                if (playertype == 1)
                {
                    if (other.gameObject.tag == "2P")
                    {


                    }
                }
                else
                {
                    if (other.gameObject.tag == "1P")
                    {


                    }
                }

            }


            if (_IsRunning)//�̵��ؼ� ��������
            {
                if (playertype == 1)
                {
                    if (other.gameObject.tag == "2P")
                    {
                        if (Mathf.Abs(other.gameObject.transform.position.y - this.transform.position.y) < 0.8f)
                        {

                            Debug.Log("2pppp");
                            if (other.gameObject.transform.position.x > this.transform.position.x)
                            {
                                _BodyTouchWay = 1;
                            }
                            else
                            {
                                _BodyTouchWay = -1;
                            }
                        }

                    }
                }
                else
                {
                    if (other.gameObject.tag == "1P")
                    {
                        if (Mathf.Abs(other.gameObject.transform.position.y - this.transform.position.y) < 0.8f)
                        {

                            if (other.gameObject.transform.position.x > this.transform.position.x)
                            {
                                _BodyTouchWay = 1;
                            }
                            else
                            {
                                _BodyTouchWay = -1;
                            }
                        }

                    }
                }

            }

        }


        public void TypeOfPlayer(int type)
        {
            if (type == 1)
            {
                dir = 1;

                this.transform.eulerAngles = new Vector3(0, -90f, 0);
                //��ġ �ֱ�

                //�±׼���
                this.tag = "1P";
                _Cols.SetTag("1PA");

                //������ ã��
                _objgauge = GameObject.Find("1pGauge");
                _objgauge.transform.position = this.transform.position;
                _objgauge.GetComponent<PlayerGauge>()._pType = playertype;
                _objgauge.GetComponent<PlayerGauge>()._gauge = _Gauge;



            }
            else
            {
                dir = -1;
                this.transform.eulerAngles = new Vector3(0, -90f, 0);
                //��ġ �ֱ�
                this.tag = "2P";

                _Cols.SetTag("2PA");
                _objgauge = GameObject.Find("2pGauge");
                _objgauge.transform.position = this.transform.position;

                _objgauge.GetComponent<PlayerGauge>()._pType = playertype;
                _objgauge.GetComponent<PlayerGauge>()._gauge = _Gauge;

            }
        }
        void setOutTime()
        {
            if (_Gauge <= 25f) outtime = kt1;
            else if (_Gauge <= 50f) outtime = kt2;
            else if (_Gauge <= 90f) outtime = kt3;
            else if (_Gauge <= 150f) outtime = kt4;
            else outtime = kt5;
            //if (_Gauge <= 25f) outtime = 0.1f;
            //else if (_Gauge <= 50f) outtime = 0.2f;
            //else if (_Gauge <= 90f) outtime = 0.3f;
            //else if (_Gauge <= 150f) outtime = 0.4f;
            //else outtime = 0.5f;
        }


        void KnockOutMove()
        {
            // �˾Ƽ� ã���� 
            //Vector3 v = new Vector3(-dir*hittedPower.x, hittedPower.y, 0);
            //_Rigid.AddForce(v * 0.01f, ForceMode.Force);


            Vector3 v = new Vector3(-dir * hittedPower.x * 30, hittedPower.y * 5, 0);
            _Rigid.AddForce(v * 0.01f, ForceMode.Force);


        }
    }





}



