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
using UnityEditor.Compilation;
using System.Runtime.CompilerServices;

namespace nara
{

    public class PlayerCtrller : MonoBehaviour
    {








        [SerializeField]
        string _Name = "Aries";

        [SerializeField]
        public int playertype = 1;

        //이동속도
        [SerializeField]
        float _MoveSpeed = 10.0f;

        //점프력
        [SerializeField]
        float _JumpingPower = 13.0f;

        //게이지
        [SerializeField]
        public float _Gauge = 1.0f;

        //목숨
        [SerializeField]
        public int _Life = 3;

        //미끄러지는 속도
        [SerializeField]
        float _SlideSpeed = 20.0f;

        //하강키 누를때 스피드
        [SerializeField]
        float _DownSpeed = 5.0f;

        //점프 후  행동제약
        [SerializeField]
        float _JumpRestriction = 0.25f;

        //관성 시작 시간
        [SerializeField]
        float _RunRestriction = 1.0f;


        //관성시간
        [SerializeField]
        float _BreakingTime = 0.25f;
        //파워
        [SerializeField]
        public Vector3 _Power = Vector3.zero;

        [SerializeField]
        public float Dmg = 0.0f;

        //전진공격 스피드
        [SerializeField]
        float RLspeed = 1000f;

        [SerializeField]
        float Upspeed = 20f;


        Rigidbody _Rigid;
        PlayerAnimation _Anim;
        PlayerEffect _Eff;
        PlayerColliders _Cols;

        GameObject _objgauge;

        public PlayerState _State;
        //방향 좌우 move함수용 및 이펙트 방향 변환
        public int dir;
        //time
        public float _RunTime = 0.0f;
        float _BreakTime = 0.0f;
        float _JumpTime = 0.3f;
        float _Floortime = 0.0f;//
        float _GodTime = 0.0f;

        //공격 지연 시간 및 조건
        float _AttackTime = 0.0f;
        bool _CantAttack;//

        public bool _IsAttack;
        public bool _IsSkill;
        bool _IsJump;
        bool _IsDJump;
        bool _IsOnesec;//질주 공격 
        public bool _IsRunning;
        bool _IsKnockOut;
        bool _IsShield;
        public float _ECooldown = 0f;//방어키 딜레이
        bool _IsRespawn;


        float _Teemo;

        /* 공격 스킬 관련 변수 */
        bool _IsRLMove;//전진공격 이동시작
        bool _IsUpMove;//위스킬 이동시작
        bool _IsDwMove;
        bool _IsAirAtk;//중력0으로
        //keydown time;
        float _KDwTime = 0f;
        float _KUpTime = 0f;
        float _KQTime = 0f;//공격키 딜레이
        float _KWTime = 0f;//스킬키 딜레이

        //keydown bool;
        bool _IsKeyDown;
        bool _IsKeyUp;
        bool _IsKeyQ;
        bool _IskeyE;
        //거리 측정용 
        Vector3 _MovePos;
        Vector3 _PrePos;





        //
        float stuntime = 0.1f;//경직시간
        float outtime;//넉아웃 시간//
        float startime;//넉아웃 이펙트 생성시간;




        //게이지에 따른 넉아웃 시간 측정용
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
        //죽을 때 
        float RespawnTime;
        //리스폰 포지션
        Vector3 P1ResPos;
        Vector3 P2ResPos;


        int _BodyTouchWay;

        public bool god;


        void Start()
        {

            _Rigid = GetComponent<Rigidbody>();
            _Anim = GetComponent<PlayerAnimation>();
            _Eff = GetComponent<PlayerEffect>();
            _Cols = GetComponent<PlayerColliders>();
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
            playertype = 1;
            P1ResPos = new Vector3(-4.5f, 8.8f, 0f);
            P2ResPos = new Vector3(4.5f, 8.8f, 0f);
            god = false;
            TypeOfPlayer(1);
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
            _ECooldown -= Time.deltaTime;//쿨타임6초
            if (_IsShield)//1초동안 무적역할
            {
                if (_ECooldown < 5.0f)
                {

                    Debug.Log("test1");
                    _IsShield = false;
                    god = false;
                    _Eff.EffectOff(21);
                }

            }

            //넉아웃 상태에서 이동
            if (_IsKnockOut)
            {
                startime += Time.deltaTime;
                if (startime > 0.05f)
                {
                    //넉아웃 이펙트 생성
                    _Eff.EffectOn(22);
                    _Eff.EffDestroyWithTime(22, 1);

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







            //달리다가 멈추는 조건
            _Floortime += Time.deltaTime;
            //공격 지연
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




            if (_IsRLMove)//거리가 어느이상되면 멈춘다.
            {
                float x = Vector3.Distance(_PrePos, _MovePos);
                if (x < _Teemo)
                {
                    Debug.Log("dddd");
                    _Rigid.AddForce(this.transform.forward * RLspeed, ForceMode.Force);
                    Debug.Log("arries " + RLspeed);


                }
            }
            if (_IsUpMove)
            {
                Debug.Log("위");
                _IsAirAtk = false;
                _Rigid.AddForce(this.transform.up * Upspeed, ForceMode.Impulse);
                _IsUpMove = false;

                Upspeed = 20.0f;
            }
            if (_IsDwMove)
            {
                _Rigid.AddForce(this.transform.up * -10, ForceMode.Impulse);
                _IsDwMove = false;
            }
            if (_IsAirAtk)
            {
                _Rigid.velocity = Vector3.zero;
                Debug.Log("111111");
            }


            //커맨드 키입력
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

            //체공 시간
            if (_IsJump)
                _JumpTime += Time.deltaTime;

            //브레이킹 질주 공격 조건 RunTime;
            if (_RunTime > _RunRestriction && !_IsJump)
            {
                _RunTime = _RunRestriction;
                _IsOnesec = true;
                _Anim.SetIsOnesec(_IsOnesec);
                _BreakTime = _BreakingTime;
            }
            //달리는지 달리지 않는지 판단.

            if (_RunTime > 0) _IsRunning = true;
            else _IsRunning = false;
            _Anim.SetIsRunning(_IsRunning);


            //Runtime 초기화;
            if (_State != PlayerState.Running && !_IsAttack && !_IsOnesec && !_IsSkill)
            {
                _RunTime = 0.0f;
            }


            //낙하상태 
            if (_Rigid.velocity.y < -0.05f && !_IsAttack && !_IsSkill && stuntime < 0f && !_IsKnockOut)//낙하
            {
                _IsOnesec = false;
                _Anim.SetIsOnesec(_IsOnesec);
                SetState(PlayerState.Falling);
                //_IsJump = true;
            }
            Breaking();
        }
        void Update()
        {
            if (_objgauge)//null이 아닐때 발동
            {
                _objgauge.GetComponent<PlayerGauge>()._gauge = _Gauge;
                _objgauge.transform.position = this.transform.position;

            }
            //바닥에 있는지 체크 함수;
            OnFloor();
            //브레이킹 부분


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
                if (Input.GetKey(KeyCode.S))//조합기 하 및 하강 속도 향상
                {

                    _KUpTime = 0.0f;
                    _KDwTime = 0.2f;
                    if (_IsJump && _JumpTime > _JumpRestriction && !_IsAttack && !_IsSkill)
                        Move(0);
                }

                else if (Input.GetKey(KeyCode.W))//조합키 상
                {
                    _KUpTime = 0.2f;
                    _KDwTime = 0.0f;


                }

                /* 이동 입력 */
                if (Input.GetKey(KeyCode.A))//좌 이동
                {
                    if (!_IsAttack && !_IsSkill)
                    {
                        dir = -1;
                        Move(dir);
                    }

                }
                if (Input.GetKey(KeyCode.D))//우 이동
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
                //else if (Input.GetKey(KeyCode.Q)) //공격
                //{

                //}

                else if (Input.GetKeyDown(KeyCode.G))//스킬
                {
                    if (_KWTime < 0.0f && !_IsAttack && !_IsSkill)
                    {
                        _KWTime = 0.3f;
                        Skill();
                    }
                }

                else if (Input.GetKey(KeyCode.H))//방어
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

                /* 점프 */
                if (Input.GetKey(KeyCode.Space))
                {
                    Jump();

                }
            }
            else
            {
                if (Input.GetKey(KeyCode.DownArrow))//조합기 하 및 하강 속도 향상
                {

                    _KUpTime = 0.0f;
                    _KDwTime = 0.2f;
                    if (_IsJump && _JumpTime > _JumpRestriction && !_IsAttack && !_IsSkill)
                        Move(0);
                }

                else if (Input.GetKey(KeyCode.UpArrow))//조합키 상
                {
                    _KUpTime = 0.2f;
                    _KDwTime = 0.0f;


                }

                /* 이동 입력 */
                if (Input.GetKey(KeyCode.LeftArrow))//좌 이동
                {
                    if (!_IsAttack && !_IsSkill)
                    {
                        dir = -1;
                        Move(dir);
                    }

                }
                if (Input.GetKey(KeyCode.RightArrow))//우 이동
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
                //else if (Input.GetKey(KeyCode.Q)) //공격
                //{

                //}

                else if (Input.GetKeyDown(KeyCode.Semicolon))//스킬
                {
                    if (_KWTime < 0.0f && !_IsAttack && !_IsSkill)
                    {
                        _KWTime = 0.3f;
                        Skill();
                    }

                }

                else if (Input.GetKey(KeyCode.Quote))//방어
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

                /* 점프 */
                if (Input.GetKey(KeyCode.RightShift))
                {
                    Jump();

                }
            }
            //1p//////////////////////////////////////////////////////////////////////////////
            /* 상하 입력 */

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



        void Move(int dir)      /*이동 및 방향전환*/
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
                    Debug.Log("touch");
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
                        Debug.Log("touch");
                        this.transform.position += Vector3.right * dir * _MoveSpeed / 2.0f * Time.deltaTime;
                    }
                }
            }
        }




        void Breaking()
        {
            //달리다가 멈추면 미끄러짐
            if (_IsOnesec && !_IsJump && _State == PlayerState.Idle && !_IsAttack && !_IsSkill)//탄성 효과
            {
                if (_BreakTime > 0)
                {
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

            if (_IsKeyDown)//아래키를 누르고
            {
                if (_IsJump)//점프상태일때
                {
                    //공중아래공격 
                    if (!_IsAttack)
                    {

                        _IsAirAtk = true;//이동이 멈춤
                        this.transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.right * dir), 1f);
                        SetState(PlayerState.AirDwAttack);

                    }
                }
                else//점프상태가 아닐 때
                {
                    //아래공격 X

                    SetState(PlayerState.DwAttack);
                }
            }
            else if (_IsKeyUp) //위키를 누른상태
            {
                //위공격
                _IsAirAtk = true;//이동이 멈춤
                SetState(PlayerState.UpAttack);//1.67


            }
            else //아래, 위키입력 없는 상태
            {

                if (_IsJump)//점프 상태
                {
                    if (_IsRunning)//달리는 상태
                    {

                        //공중전진공격
                        _Teemo = 3.0f;
                        _IsAirAtk = true;

                        this.transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.right * dir), 1f);
                        SetState(PlayerState.RLAttack);

                    }
                    else
                    {
                        //공중중립공격


                        this.transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.right * dir), 1f);
                        SetState(PlayerState.AirNormalAttack);





                    }
                }
                else//땅에 있는 상태
                {
                    if (_IsRunning)//달리는 상태
                    {


                        if (_IsOnesec)//1초지난상태
                        {
                            //땅질주공격-
                            SetState(PlayerState.RunAttack);

                            _Teemo = 2.0f;
                        }
                        else
                        {
                            //땅전진공격
                            SetState(PlayerState.RLAttack);
                            _Teemo = 3.0f;
                            //Debug.Log("전진공격");
                        }
                    }
                    else
                    {
                        //땅중립공격

                        if (!_IsKeyQ && !_CantAttack)//Q중립공격중 q키가 눌렸을 때
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

            if (_IsKeyDown)//아래키를 누르고
            {
                if (_IsJump)//점프상태일때
                {
                    //아래공격
                    Upspeed = 12f;
                    SetState(PlayerState.DwSkill);

                }
                else//점프상태가 아닐 때
                {
                    Upspeed = 12f;
                    SetState(PlayerState.DwSkill);

                }
            }
            else if (_IsKeyUp) //위키를 누른상태
            {
                //위공격
                _IsAirAtk = true;
                _IsJump = true;
                _Anim.SetIsJump(_IsJump);
                SetState(PlayerState.UpSkill);

            }
            else //아래, 위키입력 없는 상태
            {

                if (_IsJump)//점프 상태
                {
                    if (_IsRunning)//달리는 상태
                    {
                        //공중전진공격
                        _Teemo = 10f;
                        _IsAirAtk = true;//이동이 멈춤
                        this.transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.right * dir), 1f);

                        SetState(PlayerState.RLSkill);
                    }
                    else
                    {
                        //공중 중립공격
                        this.transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.right * dir), 1f);
                        SetState(PlayerState.NormalSkill);

                    }
                }
                else//땅에 있는 상태
                {
                    if (_IsRunning)//달리는 상태
                    {
                        //땅전진공격
                        _Teemo = 10f;
                        _IsAirAtk = true;
                        SetState(PlayerState.RLSkill);
                    }
                    else
                    {
                        //땅중립공격
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
        public void onDwMove()
        {
            if (_IsRespawn) return;
            _IsDwMove = true;
        }

        public void setinit()//초기화
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
        private void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.tag == "Floor")
            {
                _IsOnesec = false;
                _IsJump = true;
                _Anim.SetIsJump(_IsJump);
                _Anim.SetIsOnesec(_IsOnesec);


            }
            if (collision.gameObject.tag == "1P" || collision.gameObject.tag == "2P")
            {
                _BodyTouchWay = 0;
                Debug.Log("뭐지");
            }
        }



        private void OnTriggerEnter(Collider other)
        {
            //공격 이나 스킬 제외하고선 안밀리게
            if (RespawnTime > 0) return;


            if (other.gameObject.tag == "DeadLine")
            {
                Debug.Log("데드라인 터치");
                god = true;
                setinit();
                RespawnTime = 6.0f;
                _IsRespawn = true;
                _Eff.Dead(this.transform.position);//이펙트 생성


            }

            if (_IsKnockOut || _IsShield) return;


            //방어스킬 쓸때는 하지말것.
            if (playertype == 1)
            {


                if (other.gameObject.tag == "2PA")
                {
                    Debug.Log("dd");
                    PlayerCtrllercap pc = other.transform.root.GetComponent<PlayerCtrllercap>();
                    this.dir = -pc.dir;

                    this.transform.localEulerAngles = new Vector3(0, dir * 90, 0);
                    if (pc._Power.x == 0 && pc._Power.y == 0) //경직

                    {

                        setinit();
                        stuntime = 0.2f;
                        _Gauge += pc.Dmg;
                        _RunTime = 0;
                        _IsOnesec = false;


                    }
                    else//넉아웃
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
                    Debug.Log("이거 여기로 드러와야하는데?");
                    PlayerCtrllercap pc = other.transform.root.GetComponent<PlayerCtrllercap>();
                    dir = -pc.dir;
                    pc.transform.localEulerAngles = new Vector3(0, dir * 90, 0);
                    if (pc._Power.x == 0 && pc._Power.y == 0) //경직
                    {

                        setinit();
                        stuntime = 0.2f;
                        _Gauge += pc.Dmg;
                        _RunTime = 0;
                        _IsOnesec = false;

                    }
                    else//넉아웃
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






        ///이동관련
        private void OnCollisionStay(Collision other)
        {

            if (_IsRunning)//이동해서 만날때만
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
                this.transform.position = new Vector3(-10f, 1f, 0f);
                //위치 넣기


                this.tag = "1P";
                _Cols.SetTag("1PA");


                _objgauge = GameObject.Find("1pGauge");
                _objgauge.transform.position = this.transform.position;
                _objgauge.GetComponent<PlayerGauge>()._pType = playertype;
                _objgauge.GetComponent<PlayerGauge>()._gauge = _Gauge;



            }
            else
            {
                dir = -1;
                this.transform.eulerAngles = new Vector3(0, -90f, 0);
                //위치 넣기
                this.tag = "2P";
                this.transform.position = new Vector3(10f, 1f, 0f);
                _Cols.SetTag("2PA");
                _objgauge = GameObject.Find("2pGauge");
                _objgauge.transform.position = this.transform.position;

                _objgauge.GetComponent<PlayerGauge>()._pType = playertype;
                _objgauge.GetComponent<PlayerGauge>()._gauge = _Gauge;

            }
        }

        //넉아웃 시간 설정 
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
            // 알아서 찾으셈 
            //Vector3 v = new Vector3(-dir*hittedPower.x, hittedPower.y, 0);
            //_Rigid.AddForce(v * 0.01f, ForceMode.Force);


            Vector3 v = new Vector3(-dir * hittedPower.x * 30, hittedPower.y * 5, 0);
            _Rigid.AddForce(v * 0.01f, ForceMode.Force);
        }
    }



}



