namespace SDW
{
    public struct BattleStageUIDataFileData
    {
        public int IncludedChapter;
        public int IncludedStage;
        public string StageName;
        public string BackgroundImageName;

        /// <summary>
        /// BattleStageUIDataFileData 초기화
        /// </summary>
        /// <param name="fields">초기화 데이터</param>
        public BattleStageUIDataFileData(string[] fields)
        {
            IncludedChapter = int.Parse(fields[0]);
            IncludedStage = int.Parse(fields[1]);
            StageName = fields[2];
            BackgroundImageName = fields[3];
        }
    }
}