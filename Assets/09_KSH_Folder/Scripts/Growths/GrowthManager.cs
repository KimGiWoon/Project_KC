using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SDW;
using System;
using System.Collections;

public class GrowthManager : MonoBehaviour
{
    [SerializeField] private List<GrowthDatas> _growthsDatas;
    [SerializeField] private Transform nodeContent; //노드들 들어있는 부모 오브젝트

    [SerializeField] private PermanentGrowthUI _permanentGrowthUI;

    private CharacterDataManager _charData;

    //노드 데이터 저장 딕셔너리
    private Dictionary<int, GrowthDatas> growthDataDic = new Dictionary<int, GrowthDatas>();
    public Dictionary<int, GrowthDatas> GrowthDataDic => growthDataDic;

    //노드 UI 저장 딕셔너리
    private Dictionary<int, GameObject> growthUIDic = new Dictionary<int, GameObject>();

    //해금된 노드 ID 중복없이 리스트에 저장 
    private GameManager _gameManager;
    private bool _isLoaded;
    private static bool _hasFaster = false;

    private void Awake()
    {
        _gameManager = GameManager.Instance;
        _charData = _gameManager.CharacterData;
        if (!_hasFaster)
            GameManager.Instance.CanFaster = false;
    }

    private void Update()
    {
        if (!_gameManager.CompleteDownload || !_gameManager.ImageSpriteConnected ||
            !_gameManager.PrefabAndSoConnected ||
            _isLoaded) return;
        LoadGrowthSO();
        ConnectUIAndData();
        if (!_gameManager.GrowthUnlockNodes.Contains(80201))
            UnlockNode(80201); //처음 노드만 활성화
        UpdateAllNode();
        StartCoroutine(DelayedSetNewCharacter());
        _isLoaded = true;
    }

    private IEnumerator DelayedSetNewCharacter()
    {
        yield return new WaitForSeconds(0.5f);

        foreach (var cha in _charData.AllOwnedCharacters)
        {
            SetNewCharacter(cha);
        }
    }

    private void OnEnable()
    {
        GameManager.Instance.Reward.OnNewCharacterAdded += SetNewCharacter;
    }

    private void OnDisable()
    {
        GameManager.Instance.Reward.OnNewCharacterAdded -= SetNewCharacter;
    }

    private void LoadGrowthSO() //스크립터블오브젝트 자동으로 딕셔너리에 넣어주는 기능
    {
        foreach (var growthData in _growthsDatas)
        {
            if (growthDataDic.ContainsKey(growthData.nodeID)) //딕셔너리에 중복된 아이디가 있다면
                continue;
            growthDataDic[growthData.nodeID] = growthData; //없다면 넣기
        }

        // Debug.Log($"{growthDataDic.Count}개의 노드를 로드");
    }

    private void ConnectUIAndData() //성장 데이터와 UI 연결시켜주는 기능
    {
        foreach (var growthNodeUI in nodeContent.GetComponentsInChildren<GrowthNodeUI>())
        {
            if (int.TryParse(growthNodeUI.name, out int growthNodeID) &&
                growthDataDic.TryGetValue(growthNodeID, out var growthData))
            {
                growthNodeUI.Init(growthData);
                growthUIDic[growthNodeID] = growthNodeUI.gameObject;
                _permanentGrowthUI.AddNode(growthNodeUI);
            }
        }
    }

    private bool IsCompleteNode(int nodeId) //해금 노드 Bool
        // if (growthDatas.nodeActiveCondition == -1) return true;
        => _gameManager.GrowthCompleteNodes.Contains(nodeId);

    //# unlock - 현재 노드가 활성화 가능해진 노드(Complete에 있으면 이미 해금)
    //# 하나가 complete가 되면, 다음 노드가 unlock 되어야 함
    //# 문제점 1. activate를 한 node가 계속 activate가 가능하다고 뜨고 있음
    //# 문제점 2. 다음 노드가 unlock되지 않음

    public void UpdateAllNode() //모든 노드 업데이트 
    {
        foreach (var growth in growthUIDic)
        {
            int nodeId = growth.Key; //해당 노드ID
            var nodeGO = growth.Value; //해당 노드 UI

            //노드 데이터가 있으면 True
            if (!growthDataDic.TryGetValue(nodeId, out var growthData)) continue;

            var growthNodeUI = nodeGO.GetComponent<GrowthNodeUI>(); //오브젝트에서 GrowthNodeUi 가져옴
            if (growthNodeUI == null) continue;

            bool canActivate;
            int activeCondition = growthData.nodeActiveCondition;

            bool isUnlocked;
            if (activeCondition == -1)
            {
                //todo 첫 번째 노드인 경우
                canActivate = !IsCompleteNode(nodeId);
                isUnlocked = _gameManager.GrowthUnlockNodes.Contains(nodeId);
                //can / unlocked
            }
            else
            {
                //# 이전꺼가 unlock이면서 canActivate가 false
                //false, true -> 다음 노드 활성화할 차례
                //true , true -> 해당 노드가 활성화할 차례
                canActivate = !IsCompleteNode(nodeId);
                isUnlocked = _gameManager.GrowthUnlockNodes.Contains(activeCondition) &&
                             IsCompleteNode(growthDataDic[activeCondition].nodeID);

                if (canActivate && isUnlocked) UnlockNode(nodeId);
            }

            // 노드 1 true true
            // 노드 2 -> 이 경우 반응하면 안됨
            // 노드 1 false true
            // 노드 2 -> 아 내 차례다

            growthNodeUI.NodeUIUpdate(isUnlocked, canActivate);
        }
    }

    private void UnlockNode(int nodeId)
    {
        if (_gameManager.GrowthUnlockNodes.Contains(nodeId)) return;

        _gameManager.AddGrowthUnlockNode(nodeId); //해금딕셔너리에 추가
    }

    private void AddGrowthStat(ref float modified, float original, GrowthDatas growthDatas) //더하기, 곱하기 계산 기능
    {
        if (growthDatas.nodeAbilityValuePlus != 0 && growthDatas.nodeAbilityValueMult <= 1)
        {
            modified = original + growthDatas.nodeAbilityValuePlus;
        }
        else if (growthDatas.nodeAbilityValueMult != 0 && growthDatas.nodeAbilityValuePlus <= 0)
        {
            if (growthDatas.nodeAbility == NodeAbility.chaAtkSpeed)
            {
                modified = original * (2 - growthDatas.nodeAbilityValueMult);
            }
            else
            {
                modified = original * growthDatas.nodeAbilityValueMult;    
            }
        }
    }

    public void ApplyGrowthStat(GrowthDatas growthDatas, CharacterState modified, CharacterState original) //단일 스탯 적용
    {
        switch (growthDatas.nodeAbility)
         {
            case NodeAbility.chaAttack:
                AddGrowthStat(ref modified._chaAttack, original._chaAttack, growthDatas);
                Debug.Log($"{original._chaName}{modified._chaName} 공격력 증가: {original._chaAttack} -> {modified._chaAttack}");
                break;
            case NodeAbility.chaAtkSpeed:
                AddGrowthStat(ref modified._chaAtkSpeed, original._chaAtkSpeed, growthDatas);
                Debug.Log($"{original._chaName}{modified._chaName} 공격속도 증가: {original._chaAtkSpeed} -> {modified._chaAtkSpeed}");
                break;
            case NodeAbility.chaArmor:
                AddGrowthStat(ref modified._chaArmor, original._chaArmor, growthDatas);
                Debug.Log($"{original._chaName}{modified._chaName} 방어력 증가: {original._chaArmor} -> {modified._chaArmor}");
                break;
            case NodeAbility.chaAvoid:
                AddGrowthStat(ref modified._chaAvoid, original._chaAvoid, growthDatas);
                Debug.Log($"{original._chaName}{modified._chaName} 회피율 증가: {original._chaAvoid} -> {modified._chaAvoid}");
                break;
            case NodeAbility.chaCrit:
                AddGrowthStat(ref modified._chaCrit, original._chaCrit, growthDatas);
                Debug.Log($"{original._chaName}{modified._chaName} 치명타 증가: {original._chaCrit} -> {modified._chaCrit}");
                break;
            case NodeAbility.chaCritDmg:
                AddGrowthStat(ref modified._chaCritDmg, original._chaCritDmg, growthDatas);
                Debug.Log($"{original._chaName}{modified._chaName} 치명타데미지 증가: {original._chaCritDmg} -> {modified._chaCritDmg}");
                break;
            case NodeAbility.chaMPRecovery:
                AddGrowthStat(ref modified._chaMPRecovery, original._chaMPRecovery, growthDatas);
                Debug.Log($"{original._chaName}{modified._chaName} 마나회복 증가: {original._chaMPRecovery} -> {modified._chaMPRecovery}");
                break;
            case NodeAbility.chaMP:
                modified._chaMaxMP = original._chaMaxMP;
                modified._chaCurrentMP = modified._chaMaxMP * growthDatas.nodeAbilityValueMult;
                Debug.Log($"{original._chaName}{modified._chaName} 현재 MP 증가: {modified._chaCurrentMP} ({growthDatas.nodeAbilityValueMult * 100}%)");
                break;
            case NodeAbility.None:
                if (growthDatas.nodeID == 80002) //배속 기능 활성화
                {
                    if (!GameManager.Instance.CanFaster)
                        GameManager.Instance.CanFaster = true;
                    // Debug.Log("배속 기능 활성화!");
                }
                else if (growthDatas.nodeID == 80001) //가챠 기능 활성화
                {
                    GameManager.Instance.CanGacha = true;
                    // Debug.Log("뽑기 기능 활성화");
                }
                break;
        }
    }

    public void AllApplyGrowth(GrowthDatas growthDatas) //전체 스탯 적용
    {
        //todo CharacterDataSO의 GetOriginalCharacterState()를 가져와서
        //todo 변경된 스탯은 GetModifiedCharacterState() => 여기꺼에 적용
        foreach (var cha in _charData.AllOwnedCharacters)
        {
            CharacterState original = cha.GetOriginalCharacterState();
            CharacterState modified = cha.GetModifiedCharacterState();
            ApplyGrowthStat(growthDatas, modified, original);
        }
    }

    private void SetNewCharacter(CharacterDataSO cha) //새로 뽑힌 캐릭터에 기존 스탯 적용
    {
        CharacterState original = cha.GetOriginalCharacterState();
        CharacterState modified = cha.GetModifiedCharacterState();
        
        foreach (int nodeID in GameManager.Instance.GrowthCompleteNodes)
        {
            if (GrowthDataDic.TryGetValue(nodeID, out var growthDatas))
            {
                ApplyGrowthStat(growthDatas, modified, original);
            }
        }
    }
}