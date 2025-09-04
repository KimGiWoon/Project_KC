using System.Collections.Generic;
using UnityEngine;

namespace SDW
{
    public class CharacterDataManager : MonoBehaviour
    {
        //# ID(int) - CharacterBaseDataFileData
        private Dictionary<int, CharacterBaseDataFileData> _chaIdData = new Dictionary<int, CharacterBaseDataFileData>();
        public Dictionary<int, CharacterBaseDataFileData> ChaIdData => _chaIdData;

        //# CharacterEnName(enum) - CharacterBaseDataFileData
        private Dictionary<CharacterEnName, CharacterBaseDataFileData> _chaEnNameData =
            new Dictionary<CharacterEnName, CharacterBaseDataFileData>();
        public Dictionary<CharacterEnName, CharacterBaseDataFileData> ChaEnNameData => _chaEnNameData;

        //# CharacterRole(enum) - CharacterTypeFileData
        private Dictionary<CharacterRole, CharacterTypeFileData> _chaTypeData =
            new Dictionary<CharacterRole, CharacterTypeFileData>();
        public Dictionary<CharacterRole, CharacterTypeFileData> ChaTypeData => _chaTypeData;

        //# CharacterUpgrade(int) - CharacterUpgradeData
        private Dictionary<int, CharacterUpgradeFileData> _chaBeadsData = new Dictionary<int, CharacterUpgradeFileData>();
        public Dictionary<int, CharacterUpgradeFileData> ChaBeadsData => _chaBeadsData;

        //# CharacterLevel(int) - CharacterLevelUpStatData
        private Dictionary<int, CharacterLevelUpStatFileData> _chaLevelUpStatData =
            new Dictionary<int, CharacterLevelUpStatFileData>();
        public Dictionary<int, CharacterLevelUpStatFileData> ChaLevelUpStatData => _chaLevelUpStatData;

        //# SkillID(int) - CharacterSkillFileData
        private Dictionary<int, CharacterSkillFileData> _chaIdSkillData = new Dictionary<int, CharacterSkillFileData>();
        public Dictionary<int, CharacterSkillFileData> ChaIdSkillData => _chaIdSkillData;

        //# SkillEnName(enum) - CharacterSkillFileData
        private Dictionary<CharacterSkillEnName, CharacterSkillFileData> _chaEnNameSkillData =
            new Dictionary<CharacterSkillEnName, CharacterSkillFileData>();
        public Dictionary<CharacterSkillEnName, CharacterSkillFileData> ChaEnNameSkillData => _chaEnNameSkillData;

        /// <summary>
        /// 각 Data Table 데이터 연결
        /// </summary>
        private void Start()
        {
            LoadCharacterBase();
            LoadCharacterType();
            LoadCharacterUpgrade();
            LoadCharacterLevelUpStat();
            LoadCharacterSkill();
        }

        /// <summary>
        /// CharacterBase를 CSV 파일로부터 로드하고, 내부 딕셔너리에 저장
        /// </summary>
        private void LoadCharacterBase()
        {
            string[] fields = HandleCSV.LoadFromCsv("Character/CharacterBaseData");
            var chaBaseList = HandleCSV.ReadDataFromLines<CharacterBaseDataFileData>(fields);

            foreach (var chaBase in chaBaseList)
            {
                _chaIdData[chaBase.ChaID] = chaBase;
                _chaEnNameData[chaBase.ChaEnName] = chaBase;
            }
        }

        /// <summary>
        /// CharacterTypeFileData를 CSV 파일로부터 로드하고, 내부 딕셔너리에 저장
        /// </summary>
        private void LoadCharacterType()
        {
            string[] fields = HandleCSV.LoadFromCsv("Character/CharacterType");
            var chaRoleList = HandleCSV.ReadDataFromLines<CharacterTypeFileData>(fields);

            foreach (var chaRole in chaRoleList)
            {
                _chaTypeData[chaRole.ChaRole] = chaRole;
            }
        }

        /// <summary>
        /// CharacterUpgradeFileData를 CSV 파일로부터 로드하고, 내부 딕셔너리에 저장
        /// </summary>
        private void LoadCharacterUpgrade()
        {
            string[] fields = HandleCSV.LoadFromCsv("Character/CharacterUpgrade");
            var chaUpgradeList = HandleCSV.ReadDataFromLines<CharacterUpgradeFileData>(fields);

            foreach (var chaUpgrade in chaUpgradeList)
            {
                _chaBeadsData[chaUpgrade.ChaUpgrade] = chaUpgrade;
            }
        }

        /// <summary>
        /// CharacterLevelUpStatFileData를 CSV 파일로부터 로드하고, 내부 딕셔너리에 저장
        /// </summary>
        private void LoadCharacterLevelUpStat()
        {
            string[] fields = HandleCSV.LoadFromCsv("Character/CharacterLevelUpStat");
            var chaLevelUpStatList = HandleCSV.ReadDataFromLines<CharacterLevelUpStatFileData>(fields);

            foreach (var chaLevelUpStat in chaLevelUpStatList)
            {
                _chaLevelUpStatData[chaLevelUpStat.ChaLevel] = chaLevelUpStat;
            }
        }

        /// <summary>
        /// CharacterSkillFileData를 CSV 파일로부터 로드하고, 내부 딕셔너리에 저장
        /// </summary>
        private void LoadCharacterSkill()
        {
            string[] fields = HandleCSV.LoadFromCsv("Character/CharacterSkill");
            var chaSkillList = HandleCSV.ReadDataFromLines<CharacterSkillFileData>(fields);

            foreach (var chaSkill in chaSkillList)
            {
                _chaIdSkillData[chaSkill.ChaSkillID] = chaSkill;
                _chaEnNameSkillData[chaSkill.ChaSkillEnName] = chaSkill;
            }
        }
    }
}