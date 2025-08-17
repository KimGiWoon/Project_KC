using System;

namespace SDW
{
    [Serializable]
    public class UserData
    {
        public string Email;
        public string JoinData;
        public string Nickname;

        public UserData(string email, string joinData, string nickname)
        {
            Email = email;
            JoinData = joinData;
            Nickname = nickname;
        }
    }
}