using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using SDW;
using TMPro;

public class GrowthNodeUI : MonoBehaviour
{
    [SerializeField] private Button growthButton; //노드 버튼
    [SerializeField] private Image growthImage; //노드 색
    [SerializeField] private GameObject growthEffect; //노드 해금 테두리
    [SerializeField] private TextMeshProUGUI currencyText; //노드 필요 재화
    [SerializeField] private TextMeshProUGUI growthDescription; //노드 설명
    private GrowthDatas growthDatas;
    public NodeGrade Grade;
    public bool CanActivate;
    public bool Unlocked;

    public void Init(GrowthDatas growth)
    {
        growthDatas = growth;
        growthEffect.SetActive(false);

        if (growthDescription != null && !string.IsNullOrEmpty(growthDatas.nodeDescription))
        {
            if(!string.IsNullOrEmpty(growthDescription.text))
                growthDescription.text += "\n" + growthDatas.nodeDescription.Replace("\\n", "\n");
            else
                growthDescription.text = growthDatas.nodeDescription.Replace("\\n", "\n");
        }
        
        if (growthDatas.nodeGrade == NodeGrade.Contents)
            return;

        if (currencyText != null)
            currencyText.text = $"조각난 기억 {growthDatas.nodeCurrency} 소모";
    }

    public void NodeUIUpdate(bool isUnlocked, bool isCanActivate) //노드 UI 업데이트 기능
    {
        if (growthImage != null)
        {
            string hexColor = "#989898"; //기본 회색

            switch (growthDatas.nodeGrade)
            {
                case NodeGrade.Contents: //노드 등급이 Contents
                    hexColor = isUnlocked ? "#69CBFF" : "#989898"; //True면 파란색 False면 회색
                    break;
                case NodeGrade.Main: //노드 등급이 Main
                    hexColor = isUnlocked ? "#FFE500" : "#989898"; //True면 노란색 False면 회색
                    break;
                case NodeGrade.Sub: //노드 등급이 Sub
                    hexColor = isUnlocked ? "#FFFFFF" : "#989898"; //True면 하얀색 False면 회색
                    break;
            }
            Unlocked = isUnlocked;
            CanActivate = isCanActivate;

            if (ColorUtility.TryParseHtmlString(hexColor, out var color)) //Hex 문자열을 색으로 변환
                growthImage.color = color; //색 적용
        }

        growthEffect.SetActive(Unlocked && CanActivate); //Bool 값에 따른 이펙트 활성화
        //TODO : 만약 해금된다면 이펙트 비활성화
    }

    public string GetDescription() => growthDatas.nodeDescription;

    public NodeGrade GetNodeGrade() => growthDatas.nodeGrade;

    public int GetCurrency() => growthDatas.nodeCurrency;

    public int GetNodeId() => growthDatas.nodeID;
    
    public string GetNodeName() => growthDatas.nodeName;
}