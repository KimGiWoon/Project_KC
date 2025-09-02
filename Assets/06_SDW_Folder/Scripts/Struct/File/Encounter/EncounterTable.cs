using System;
using System.Collections.Generic;

namespace SDW
{
    public struct EncounterTable
    {
        public int EncounterID;
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
        public int ResultChoice;
        public List<string> EncounterExitText;

        public EncounterTable(string[] fields)
        {
            EncounterID = int.Parse(fields[0]);
            EncounterStage = int.Parse(fields[1]);
            ResultType = (EncounterResultType)Enum.Parse(typeof(EncounterResultType), fields[2]);
            EncounterText = fields[3];
            EnCounterType = (EncounterType)Enum.Parse(typeof(EncounterType), fields[4]);
            ChoiceCount = int.Parse(fields[5]);
            ChoiceTexts = new List<string>();

            string[] choiceTexts = fields[6].Split('`');
            foreach (string part in choiceTexts)
            {
                if (part == "null") break;
                ChoiceTexts.Add(part);
            }

            RandomType = (RandomType)Enum.Parse(typeof(RandomType), fields[7]);
            ResultOwned = bool.Parse(fields[8]);

            string[] resultId = fields[9].Split('`');
            CanGetMoney = bool.Parse(resultId[0]);
            StartID = int.Parse(resultId[1]);
            EndID = int.Parse(resultId[2]);

            ResultMinCount = int.Parse(fields[10]);
            ResultMaxCount = int.Parse(fields[11]);

            string[] resultMount = fields[12].Split('`');
            ResultMoney = int.Parse(resultMount[0]);
            ResultNumber = int.Parse(resultMount[1]);

            ResultChoice = int.Parse(fields[13]);

            EncounterExitText = new List<string>();
            string[] resultExit = fields[14].Split('`');

            foreach (string text in resultExit)
            {
                EncounterExitText.Add(text);
            }
        }
    }
}