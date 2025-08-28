using SDW;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillData", menuName = "Skills/SkillData")]
public class Explosion : SkillDataSO
{
    public override void UseSkill(Transform caster, MonoBehaviour target)
    {
        // 스킬 레벨에 대한 계산 추가 예정

        // 스킬 사용 이미지 생성
        GameObject explosion = Instantiate(base._skillPrefab, caster.position, caster.rotation);
        ExplosionController explosionController = explosion.GetComponent<ExplosionController>();

        // 스킬의 데미지, 사거리 데이터 전달
        explosionController.Init(base._skillDamage, base._skillRange);
    }
}
