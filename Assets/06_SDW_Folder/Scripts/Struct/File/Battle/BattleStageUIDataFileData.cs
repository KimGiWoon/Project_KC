namespace SDW
{
    public struct BattleStageUIDataFileData
    {
        public int IncludedChapter;
        public int IncludedStage;
        public string StageName;
        public string BackgroundImageName;

        public BattleStageUIDataFileData(string[] fields)
        {
            IncludedChapter = int.Parse(fields[0]);
            IncludedStage = int.Parse(fields[1]);
            StageName = fields[2];
            BackgroundImageName = fields[3];
        }
    }
}