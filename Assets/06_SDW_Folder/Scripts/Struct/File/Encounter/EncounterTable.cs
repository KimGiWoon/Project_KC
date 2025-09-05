using System;
using System.Collections.Generic;
using UnityEngine;

namespace SDW
{


    [Serializable]
    public struct EncounterTable
    {
        public int EncounterID;
        public EncounterSentiment Sentiment;
        public int EncounterStage;
        public EncounterResultType ResultType;
        public string EncounterText;
        public EncounterType Type;
        public int ChoiceCount;
        public List<string> ChoiceTexts;
        public RandomType RandomGrade;
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

        public EncounterTable(string[] fields)
        {
            this = default;
            ChoiceTexts = new List<string>();
            EncounterExitText = new List<string>();

            try
            {
                int.TryParse(fields[0], out EncounterID);
                Enum.TryParse<EncounterSentiment>(fields[1], true, out Sentiment);
                int.TryParse(fields[2], out EncounterStage);
                Enum.TryParse<EncounterResultType>(fields[3], true, out ResultType);
                EncounterText = fields[4].Replace("\\n", "\n");
                Enum.TryParse<EncounterType>(fields[5], true, out Type);
                int.TryParse(fields[6], out ChoiceCount);

                if (fields.Length > 7 && !string.IsNullOrEmpty(fields[7]))
                {
                    ChoiceTexts.AddRange(fields[7].Split('`'));
                }
                while (ChoiceTexts.Count < ChoiceCount)
                {
                    ChoiceTexts.Add("");
                }

                Enum.TryParse<RandomType>(fields[8], true, out RandomGrade);
                bool.TryParse(fields[9], out ResultOwned);

                string[] resultId = fields[10].Split('`');
                if (resultId.Length >= 3)
                {
                    bool.TryParse(resultId[0], out CanGetMoney);
                    int.TryParse(resultId[1], out StartID);
                    int.TryParse(resultId[2], out EndID);
                }

                int.TryParse(fields[11], out ResultMinCount);
                int.TryParse(fields[12], out ResultMaxCount);

                string[] resultMount = fields[13].Split('`');
                if (resultMount.Length >= 2)
                {
                    int.TryParse(resultMount[0], out ResultMoney);
                    int.TryParse(resultMount[1], out ResultNumber);
                }

                int.TryParse(fields[14], out ResultChoiceCount);

                if (fields.Length > 15 && !string.IsNullOrEmpty(fields[15]))
                {
                    EncounterExitText.AddRange(fields[15].Split('`'));
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[EncounterTable] CSV 파싱 오류! ID: {fields[0]}, 오류: {e.Message}");
            }
        }
    }
}