using System.Collections.Generic;
using UnityEngine;

namespace SDW
{
    public class MonsterDataManager : MonoBehaviour
    {
        //# ID(int) - MonsterDataFileData 
        private Dictionary<int, MonsterDataFileData> _monIdData = new Dictionary<int, MonsterDataFileData>();
        public Dictionary<int, MonsterDataFileData> MonIdData => _monIdData;

        //# MonsterEnName(enum) - MonsterDataFileData
        private Dictionary<MonsterEnName, MonsterDataFileData> _monEnNameData =
            new Dictionary<MonsterEnName, MonsterDataFileData>();
        public Dictionary<MonsterEnName, MonsterDataFileData> MonEnNameData => _monEnNameData;

        //# MonsterID - MonsterStatFileData
        private Dictionary<int, MonsterStatFileData> _monIdStatData =
            new Dictionary<int, MonsterStatFileData>();
        public Dictionary<int, MonsterStatFileData> MonIdStatData => _monIdStatData;

        //# MonsterEnName(enum) - MonsterStatFileData 
        private Dictionary<MonsterEnName, MonsterStatFileData> _monEnNameStatData =
            new Dictionary<MonsterEnName, MonsterStatFileData>();
        public Dictionary<MonsterEnName, MonsterStatFileData> MonEnNameStatData => _monEnNameStatData;

        //# SkillID(int) - CharacterSkillFileData
        private Dictionary<int, MonsterSkillFileData> _monIdSkillData = new Dictionary<int, MonsterSkillFileData>();
        public Dictionary<int, MonsterSkillFileData> MonIdSkillData => _monIdSkillData;

        //# SkillEnName(enum) - CharacterSkillFileData
        private Dictionary<MonsterSkillEnName, MonsterSkillFileData> _monEnNameSkillData =
            new Dictionary<MonsterSkillEnName, MonsterSkillFileData>();
        public Dictionary<MonsterSkillEnName, MonsterSkillFileData> MonEnNameSkillData => _monEnNameSkillData;

        /// <summary>
        /// 각 Data Table 데이터 연결
        /// </summary>
        private void Start()
        {
            LoadMonsterData();
            LoadMonsterStat();
            LoadMonsterSkill();
        }

        /// <summary>
        /// MonsterDataFileData를 CSV 파일로부터 로드하고, 내부 딕셔너리에 저장
        /// </summary>
        private void LoadMonsterData()
        {
            string[] fields = HandleCSV.LoadFromCsv("Monster/MonsterData");
            var monDataList = HandleCSV.ReadDataFromLines<MonsterDataFileData>(fields);

            foreach (var monStat in monDataList)
            {
                _monIdData[monStat.MonId] = monStat;
                _monEnNameData[monStat.MonEnName] = monStat;
            }
        }

        /// <summary>
        /// MonsterStatFileData를 CSV 파일로부터 로드하고, 내부 딕셔너리에 저장
        /// </summary>
        private void LoadMonsterStat()
        {
            string[] fields = HandleCSV.LoadFromCsv("Monster/MonsterStat");
            var monStatList = HandleCSV.ReadDataFromLines<MonsterStatFileData>(fields);

            foreach (var monStat in monStatList)
            {
                _monIdStatData[monStat.MomID] = monStat;
                _monEnNameStatData[monStat.MonEnName] = monStat;
            }
        }

        /// <summary>
        /// MonsterSkillFileData를 CSV 파일로부터 로드하고, 내부 딕셔너리에 저장
        /// </summary>
        private void LoadMonsterSkill()
        {
            string[] fields = HandleCSV.LoadFromCsv("Monster/MonsterSkill");
            var monSkillList = HandleCSV.ReadDataFromLines<MonsterSkillFileData>(fields);

            foreach (var monSkill in monSkillList)
            {
                _monIdSkillData[monSkill.MonSkillID] = monSkill;
                _monEnNameSkillData[monSkill.MonSkillEnName] = monSkill;
            }
        }
    }
}