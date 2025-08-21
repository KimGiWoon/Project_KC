using SDW;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillData", menuName = "Skills/SkillData")]
public class FireBall : Test_SkillDataSO
{
    public override void DataApply(CharacterSkillFileData skillFileData)
    {
        base.DataApply(skillFileData);
    }

    public override void UseSkill(Transform caster, MonoBehaviour target)
    {

    }
}
