using System;
using System.Collections.Generic;

namespace SDW
{
    [Serializable]
    public struct EncounterTable
    {
        public int EncounterID;
        public EncounterSentiment Type;
        public int EncounterStage;
        public EncounterResultType ResultType;
        public string EncounterText;
        public EncounterType EnCounterType;
        public int ChoiceCount;
        public List<string> ChoiceTexts;
        public RandomType RandomType;
        public bool ResultOwned;
        public bool CanGetMoney;
        public int StartID;
        public int EndID;
        public int ResultMinCount;
        public int ResultMaxCount;
        public int ResultMoney;
        public int ResultNumber;
        public int ResultChoiceCount;
        public List<string> EncounterExitText;

        /// <summary>
        /// EncounterTable 초기화
        /// </summary>
        /// <param name="fields">초기화 데이터</param>
        public EncounterTable(string[] fields)
        {
            EncounterID = int.Parse(fields[0]);
            Type = (EncounterSentiment)Enum.Parse(typeof(EncounterSentiment), fields[1]);
            EncounterStage = int.Parse(fields[2]);
            ResultType = (EncounterResultType)Enum.Parse(typeof(EncounterResultType), fields[3]);
            EncounterText = fields[4];
            EnCounterType = (EncounterType)Enum.Parse(typeof(EncounterType), fields[5]);
            ChoiceCount = int.Parse(fields[6]);
            ChoiceTexts = new List<string>();

            string[] choiceTexts = fields[7].Split('`');
            foreach (string part in choiceTexts)
            {
                if (part == "null") break;
                ChoiceTexts.Add(part);
            }

            RandomType = (RandomType)Enum.Parse(typeof(RandomType), fields[8]);
            ResultOwned = bool.Parse(fields[9]);

            string[] resultId = fields[10].Split('`');
            CanGetMoney = bool.Parse(resultId[0]);
            StartID = int.Parse(resultId[1]);
            EndID = int.Parse(resultId[2]);

            ResultMinCount = int.Parse(fields[11]);
            ResultMaxCount = int.Parse(fields[12]);

            string[] resultMount = fields[13].Split('`');
            ResultMoney = int.Parse(resultMount[0]);
            ResultNumber = int.Parse(resultMount[1]);

            ResultChoiceCount = int.Parse(fields[14]);

            EncounterExitText = new List<string>();
            string[] resultExit = fields[15].Split('`');

            foreach (string text in resultExit)
            {
                EncounterExitText.Add(text);
            }
        }
    }
}