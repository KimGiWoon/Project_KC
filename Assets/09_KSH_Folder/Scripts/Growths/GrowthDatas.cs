using System.Collections.Generic;
using SDW;
using UnityEngine;


[CreateAssetMenu(fileName = "GrowthData", menuName = "Growth/GrowthData")]
public class GrowthDatas : ScriptableObject
{
    public int nodeID;
    public NodeGrade nodeGrade;
    public string nodeName;
    public NodeEnName nodeNameEn;
    public string nodeDescription;
    public int nodeCurrency;
    public int nodeActiveCondition;
    public NodeAbility nodeAbility;
    public float nodeAbilityValuePlus;
    public float nodeAbilityValueMult;
    
    public virtual void DataApply(ChefsEchoDataFileData growthData)
    {
        nodeID = growthData.NodeID;
        nodeGrade = growthData.Grade;
        nodeName = growthData.NodeName;
        nodeNameEn = growthData.EnName;
        nodeDescription = growthData.NodeDescription;
        nodeCurrency = growthData.NodeCurrency;
        nodeActiveCondition = growthData.NodeActiveCondition;
        nodeAbility = growthData.Ability;
        nodeAbilityValuePlus = growthData.NodeAbilityValuePlus;
        nodeAbilityValueMult = growthData.NodeAbilityValueMult;
    }
}
