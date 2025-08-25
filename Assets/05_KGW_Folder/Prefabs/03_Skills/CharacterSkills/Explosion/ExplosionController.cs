using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionController : MonoBehaviour
{
    float _explosionDamage; // 스킬 데미지
    float _explosionAttackRange; // 스킬 사거리

    // 데이터 받기
    public void Init(float damage, float range)
    {
        _explosionDamage = damage;
        _explosionAttackRange = range;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 닿은 물체가 몬스터면 데미지 주기
        if (collision.gameObject.layer == LayerMask.NameToLayer("Monster"))
        {
            MonsterController monster = collision.GetComponent<MonsterController>();

            // 닿은 물체와 거리 비교
            float skillAttackDistance = Vector3.Distance(transform.position, collision.transform.position);

            Debug.Log($"몬스터의 거리 : {skillAttackDistance}");

            // 몬스터와 닿은 거리가 스킬의 사거리에 해당하면
            if(skillAttackDistance <= _explosionAttackRange)
            {
                Debug.Log("몬스터가 사거리 안에 있어 데미지를 주기");
                // 몬스터에게 데미지 주기
                monster.TakeDamage(_explosionDamage);
            }
            else
            {
                Debug.Log("몬스터가 사거리 밖에 있다");
            }
        }

        // 0.5초 후 사라짐
        Destroy(gameObject, 0.5f);
    }
}
