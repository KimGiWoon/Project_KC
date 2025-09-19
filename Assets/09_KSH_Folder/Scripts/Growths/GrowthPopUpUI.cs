using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class GrowthPopUpUI : MonoBehaviour
{
    [SerializeField] private GameObject growthPopUp; //팝업 창
    [SerializeField] private TextMeshProUGUI growthName; //영구성장 이름
    [SerializeField] private TextMeshProUGUI growthDescription; //영구성장 설명
    [SerializeField] private Button growthButton; //활성화 버튼
    [SerializeField] private TextMeshProUGUI point;
    
    //TODO : isUnlocked = false라면 활성하버튼 없고 (전 영구 성장 이름)필요라고 써져있어야함
    //TODO : isCompleted = true면 활성완료라고 떠야함

    public void ShowPopUp(GrowthDatas growth)
    {
        growthName.text = growth.nodeName;
        growthDescription.text = growth.nodeDescription;
        point.text = growth.nodeCurrency.ToString();
        growthPopUp.SetActive(true);
    }
}
