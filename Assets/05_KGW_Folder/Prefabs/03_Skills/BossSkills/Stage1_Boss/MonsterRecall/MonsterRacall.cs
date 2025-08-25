using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillData", menuName = "Skills/BossSkillData")]
public class MonsterRacall : SkillDataSO
{
    [Header("Monster Recall Setting")]
    [SerializeField] MonsterDataSO[] _recallMonsterList;

    Transform[] _recallPoint;
    Vector2 _basicPos;

    public override void UseSkill(Transform caster, MonoBehaviour target, Transform[] point)
    {
        _basicPos = caster.transform.position;
        float createPos = 2f;
        _recallPoint = point;

        // 스킬 사용 이미지 생성
        GameObject monsterRecall = Instantiate(base._skillPrefab, _basicPos + Vector2.up * createPos, caster.rotation);
        MonsterRacallController monsterRacallController = monsterRecall.GetComponent<MonsterRacallController>();

        // 스킬의 데이터 전달
        monsterRacallController.Init(_recallMonsterList, _recallPoint);
    }
}
