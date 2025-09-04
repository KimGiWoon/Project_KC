namespace SDW
{
    public struct SpawnMonster
    {
        public int MonsterID;
        public int MonsterLevel;
        public int MonsterNum;

        public SpawnMonster(int monsterID, int monsterLevel, int monsterNum)
        {
            MonsterID = monsterID;
            MonsterLevel = monsterLevel;
            MonsterNum = monsterNum;
        }
    }
}