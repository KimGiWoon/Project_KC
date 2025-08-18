using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 유닛의 공통의기능 정보 추상 클래스
public abstract class UnitBaseData : MonoBehaviour
{
    public float _currentHp;     // 유닛의 현재 체력
    public bool _isAlive;        // 유닛의 생존 여부
    protected float _attackCoolTimer;     // 공격 쿨타임
    protected Vector3 _moveDir;     // 유닛의 이동 방향
    public BattleManager _battleManager;

    // 유닛 생성시 초기화
    protected virtual void Start()
    {
        Init();
        _battleManager = FindObjectOfType<BattleManager>();
    }

    // 유닛의 동작 관리
    protected virtual void Update()
    {
        Movement();
        Attack();
    }

    // 유닛의 초기화
    protected abstract void Init();

    // 유닛의 움직임
    protected abstract void Movement();

    // 유닛의 공격
    protected abstract void Attack();

    // 유닛의 데미지 받음
    public virtual void TakeDamage(float damage)
    {
        // 데미지를 받음, 방어력에 대한 것은??
        _currentHp -= damage;
        
        if (_currentHp <= 0)
        {
            // 유닛의 죽음
            Death();
        }
    }

    // 유닛의 사망
    protected virtual void Death()
    {
        // 유닛의 사망처리
        _isAlive = false;

        // 오브젝트 삭제
        Destroy(gameObject);
    }
}
