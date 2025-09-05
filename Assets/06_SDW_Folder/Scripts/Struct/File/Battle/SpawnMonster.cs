namespace SDW
{
    public struct SpawnMonster
    {
        public int MonsterID;
        public int MonsterLevel;
        public int MonsterNum;

        /// <summary>
        /// SpanMonster 초기화
        /// </summary>
        /// <param name="monsterID">몬스터 ID</param>
        /// <param name="monsterLevel">몬스터 Level</param>
        /// <param name="monsterNum">몬스터 수</param>
        public SpawnMonster(int monsterID, int monsterLevel, int monsterNum)
        {
            MonsterID = monsterID;
            MonsterLevel = monsterLevel;
            MonsterNum = monsterNum;
        }
    }
}