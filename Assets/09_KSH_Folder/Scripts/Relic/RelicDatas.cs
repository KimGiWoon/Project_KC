using System.Collections.Generic;
using SDW;
using UnityEngine;

[CreateAssetMenu(fileName = "RelicData", menuName = "Relic/RelicData")]
public class RelicDatas : ScriptableObject
{
    public int relicID;
    public RelicEnName relicEnName;
    public string relicName;
    public RelicKind relicKind;
    public RelicGrade relicGrade;
    public List<string> relicDescription;
    public RelicTarget relicTarget;
    public bool relicIsPassive;
    public RelicType relicType;
    public RelicRole relicRole;
    public int chaAttack;
    public int chaArmor;
    public int chaHP;
    public int chaCritDmg;
    public int chaAtkSpeed;
    public int chaAccuracy;
    public int chaAvoid;
    public int chaMPRecovery;
    public int chaDrain;
    public int monArmor;
    public int monHP;
    public int monAttack;
    public int monAtkSpeed;
    public int addReward;
    public int storeDiscount;

    [Header("유물 이미지")] 
    public Sprite relicImage;

    public virtual void DataApply(RelicDataFileData relicData)
    {
         relicID = relicData.RelicID;
         relicEnName = relicData.RelicEnName;
         relicName = relicData.RelicName;
         relicKind = relicData.RelicKind;
         relicGrade = relicData.RelicGrade;;
         relicDescription = relicData.RelicDescription;
         relicTarget = relicData.RelicTarget;
         relicIsPassive = relicData.isPassive;
         relicType = relicData.RelicType;
         relicRole = relicData.RelicRole;
         chaAttack = relicData.ChaAttack;
         chaArmor = relicData.ChaArmor;
         chaHP = relicData.ChaHP;
         chaCritDmg = relicData.ChaCritDmg;
         chaAtkSpeed = relicData.ChaAtkSpeed;
         chaAccuracy = relicData.ChaAccuracy;
         chaAvoid = relicData.ChaAvoid;
         chaMPRecovery = relicData.ChaMPRecovery;
         chaDrain = relicData.ChaDrain;
         monArmor = relicData.MonArmor;
         monHP = relicData.MonHP;
         monAttack = relicData.MonAttack;
         monAtkSpeed = relicData.MonAtkSpeed;
         addReward = relicData.AddReward;
         storeDiscount = relicData.StoreDiscount;
    }
}
