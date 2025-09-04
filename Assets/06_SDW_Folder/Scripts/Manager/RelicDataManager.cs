using System.Collections.Generic;
using UnityEngine;

namespace SDW
{
    public class RelicDataManager : MonoBehaviour
    {
        //# RelicID - RelicDataFileData
        private Dictionary<int, RelicDataFileData> _relicIDData = new Dictionary<int, RelicDataFileData>();
        public Dictionary<int, RelicDataFileData> RelicIDData => _relicIDData;

        //# CookID - CookDataFileData
        private Dictionary<int, CookDataFileData> _cookIDData = new Dictionary<int, CookDataFileData>();
        public Dictionary<int, CookDataFileData> CookIDData => _cookIDData;

        //# CookEnName - CookDataFileData
        private Dictionary<CookEnName, CookDataFileData> _cookEnNameData = new Dictionary<CookEnName, CookDataFileData>();
        public Dictionary<CookEnName, CookDataFileData> CookEnNameData => _cookEnNameData;

        //# IngredientID - IngredientDataFileData
        private Dictionary<int, IngredientDataFileData> _ingredientIDData = new Dictionary<int, IngredientDataFileData>();
        public Dictionary<int, IngredientDataFileData> IngredientIDData => _ingredientIDData;

        //# IngredientEnName - IngredientDataFileData
        private Dictionary<IngredientEnName, IngredientDataFileData> _ingredientEnNameData =
            new Dictionary<IngredientEnName, IngredientDataFileData>();
        public Dictionary<IngredientEnName, IngredientDataFileData> IngredientEnNameData => _ingredientEnNameData;

        private void Start()
        {
            LoadRelicDataFileData();
            LoadCookDataFileData();
            LoadIngredientDataFileData();
        }

        private void LoadRelicDataFileData()
        {
            string[] fields = HandleCSV.LoadFromCsv("Relic/RelicData");
            var relicDataList = HandleCSV.ReadDataFromLines<RelicDataFileData>(fields);

            foreach (var relicData in relicDataList)
            {
                _relicIDData[relicData.RelicID] = relicData;
            }
        }

        private void LoadCookDataFileData()
        {
            string[] fields = HandleCSV.LoadFromCsv("Relic/CookData");
            var cookDataList = HandleCSV.ReadDataFromLines<CookDataFileData>(fields);

            foreach (var cookData in cookDataList)
            {
                _cookIDData[cookData.CookID] = cookData;
                _cookEnNameData[cookData.CookEnName] = cookData;
            }
        }

        private void LoadIngredientDataFileData()
        {
            string[] fields = HandleCSV.LoadFromCsv("Relic/IngredientData");
            var ingredientDataList = HandleCSV.ReadDataFromLines<IngredientDataFileData>(fields);

            foreach (var ingredientData in ingredientDataList)
            {
                _ingredientIDData[ingredientData.IngredientID] = ingredientData;
                _ingredientEnNameData[ingredientData.IngredientEnName] = ingredientData;
            }
        }
    }
}