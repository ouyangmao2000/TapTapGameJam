using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MonsterController : SingletonMono<MonsterController>
{

    // ����˽�б���rb������ΪRigidbody2D�����ڴ洢�������������
    private Rigidbody2D rb;
    // ����˽�б���coll������ΪBoxCollider2D�����ڴ洢��ײ�����������
    private BoxCollider2D coll;


    // Start�����ڽű�ʵ�����󡢵�һ֡����ǰ������
    private void Start()
    {
        // ��ȡ����ֵ��ǰGameObject�ϵ�Rigidbody2D�����rb����
        rb = GetComponent<Rigidbody2D>();
        // ��ȡ����ֵ��ǰGameObject�ϵ�BoxCollider2D�����coll����
        coll = GetComponent<BoxCollider2D>();
        // ��ȡ����ֵ��ǰGameObject�ϵ�Animator�����anim����
        //anim = GetComponent<Animator>();
    }

    public void PickUp( Transform transform)
    {
        this.transform.parent = transform;
        rb.simulated = false;

        this.transform.localPosition = Vector3.zero;
    }


    public void ThrowItOut(float HSpeed,float VSpeed)
    {
        Debug.LogError($"NowSpeed:{HSpeed}");
        rb.simulated = true;
        rb.velocity = new Vector2(HSpeed, VSpeed);

    }




}
