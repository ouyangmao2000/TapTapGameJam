 using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;

public class PlayerController :SingletonMono<PlayerController>
{
    // ����˽�б���rb������ΪRigidbody2D�����ڴ洢�������������
    private Rigidbody2D rb;
    // ����˽�б���coll������ΪBoxCollider2D�����ڴ洢��ײ�����������
    private BoxCollider2D coll;
    // ����˽�б���anim������ΪAnimator�����ڴ洢�������������������
    private Animator anim;
    // ���幫������JumpSpeed������Ϊfloat�����ڴ洢��Ծ�ٶ�
    public float JumpSpeed = 8.0f;
    // ���幫������MoveSpeed������Ϊfloat�����ڴ洢�ƶ��ٶ�
    public float MoveSpeed = 0.0f;
    float MoveChangeSpeed = 1.0f;
    public float MinMoveSpeed = 2.0f;
    public float MaxMoveSpeed = 8.0f;
    // ���幫������Level������Ϊfloat�����ڴ洢ˮƽ����ֵ
    public float Level = 0f;
    // ���幫������Level������Ϊfloat�����ڴ洢ˮƽ����ֵ
    float Dir = 0f;

    // ����˽�б���JumpableGround������ΪLayerMask�����ڴ洢������Ծ�ĵ����
    [SerializeField] private LayerMask JumpableGround;

    // ����һ����ΪMoveState��ö�٣�����idle����ֹ����run���ܲ�����jump����Ծ����fall�����䣩�ĸ�״̬
    private enum MoveState { idle, run, jump, fall }

    // Start�����ڽű�ʵ�����󡢵�һ֡����ǰ������
    private void Start()
    {
        // ��ȡ����ֵ��ǰGameObject�ϵ�Rigidbody2D�����rb����
        rb = GetComponent<Rigidbody2D>();
        // ��ȡ����ֵ��ǰGameObject�ϵ�BoxCollider2D�����coll����
        coll = GetComponent<BoxCollider2D>();
        // ��ȡ����ֵ��ǰGameObject�ϵ�Animator�����anim����
        //anim = GetComponent<Animator>();
        MoveChangeSpeed = (MaxMoveSpeed - MinMoveSpeed) / (5.0f * 60.0f);
    }

    public float _time = 1.0f;

    float _MonsterCDTime = 5.0f;
    bool _CanUP = true;
    bool haveMonster = false;


    // Update����ÿ֡����һ��
    private void Update()
    {
        _time -= Time.deltaTime;
        _MonsterCDTime -= Time.deltaTime;

        if (_time < 0.0f)
        {
            _time = 0.5f;
        }
        if (_MonsterCDTime < 0.0f)
        {
            _CanUP = true;
            _MonsterCDTime = 5.0f;
        }

        // ��ȡˮƽ���������ֵ��������ƽ������
        Level = Input.GetAxisRaw("Horizontal");

        if (Level == 0)
        {
            //MoveSpeed = 0.0f;
            MoveSpeed  -= (MoveChangeSpeed * 1.20f);
            if (MoveSpeed < 0)
            {
                MoveSpeed = 0.0f;
            }
        }
        else 
        {
            Dir = Level;
            MoveSpeed += MoveChangeSpeed;
            MoveSpeed = Mathf.Clamp(MoveSpeed, MinMoveSpeed, MaxMoveSpeed);
        }


        // ���ý�ɫ��ˮƽ�ٶ�ΪLevel����MoveSpeed����ֱ�ٶȱ��ֲ���
        rb.velocity = new Vector2(Dir * (IsWall() ? 0 : MoveSpeed), rb.velocity.y);
        //rb.velocity = new Vector2(Dir * MoveSpeed, rb.velocity);

        // ���������Ծ��ť���ҽ�ɫ�ڵ�����
        if (Input.GetButtonDown("Jump") && IsGround())
        {
            // ���ý�ɫ�Ĵ�ֱ�ٶ�ΪJumpSpeed��ˮƽ�ٶȱ��ֲ���
            rb.velocity = new Vector2(rb.velocity.x, JumpSpeed);
        }
        bool fire = false;
        if (Input.GetKeyDown(KeyCode.E) && haveMonster)
        {
            haveMonster = false;
            fire = true;
            MonsterController.GetInstance().ThrowItOut(Dir * MoveSpeed, JumpSpeed);
        }
        // 
        if (Input.GetKeyDown(KeyCode.E) && _CanUP && !haveMonster && !fire)
        {
            _CanUP = false;
            haveMonster = true;
            MonsterController.GetInstance().PickUp(this.transform.GetChild(0));
        }
        // ����UpdateStates���������¶���״̬
        UpdateStates();
    }
    // ����һ��MoveState���͵ı���state
    MoveState state;

    // UpdateStates�������ڸ��½�ɫ�Ķ���״̬
    private void UpdateStates()
    {

        // ����ˮƽ����ֵ��ת��ɫ
        if (Level > 0f)
        {
            // �������ֵ����0������״̬Ϊrun���ܲ����������ý�ɫ����ת
            state = MoveState.run;
            this.gameObject.GetComponent<SpriteRenderer>().flipX = false;
        }
        else if (Level < 0f)
        {
            // �������ֵС��0������״̬Ϊrun���ܲ����������ý�ɫ��ת
            state = MoveState.run;
            this.gameObject.GetComponent<SpriteRenderer>().flipX = true;
        }
        else
        {
            // ���û��ˮƽ���룬����״̬Ϊidle����ֹ��
            state = MoveState.idle;
        }
        // ���ݽ�ɫ�Ĵ�ֱ�ٶ�������Ծ������״̬
        if (rb.velocity.y > .1f)
        {
            // �����ֱ�ٶȴ���0.1������״̬Ϊjump����Ծ��
            state = MoveState.jump;
        }
        else if (rb.velocity.y < -.1f)
        {
            // �����ֱ�ٶ�С��-0.1������״̬Ϊfall�����䣩
            state = MoveState.fall;
        }
        // ʹ��Animator���������Ϊ"state"������������ֵΪstate��ö������ֵ
        //anim.SetInteger("state", (int)state);
    }

    // IsGround�������ڼ���ɫ�Ƿ��ڵ�����
    private bool IsGround()
    {
        // ʹ��Physics2D.BoxCast������������Ͷ�䣬����ɫ�·��Ƿ��п�����Ծ�ĵ����
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .075f, JumpableGround);
    }

    // IsWall�������ڼ���ɫ�Ƿ���ǽ
    private bool IsWall()
    {
        var Center = coll.bounds.center;
        var Size = coll.bounds.size;
        var group = Physics2D.BoxCast(Center, Size, 0f, Dir * Vector2.right, .01f, JumpableGround);




        return group;
    }
}