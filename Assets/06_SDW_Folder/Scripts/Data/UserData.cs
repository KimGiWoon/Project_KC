using System;

namespace SDW
{
    [Serializable]
    public class UserData
    {
        public string Email;
        public string JoinDate;
        public string Nickname;
        // public string Uid;

        /// <summary>
        /// UserData 초기화
        /// </summary>
        /// <param name="email">설정할 email</param>
        /// <param name="joinDate">설정할 joinDate</param>
        /// <param name="nickname">설정할 nickname</param>
        /// <param name="uid">설정할 uid</param>
        // public UserData(string email, string joinDate, string nickname, string uid)
        public UserData(string email, string joinDate, string nickname)
        {
            Email = email;
            JoinDate = joinDate;
            Nickname = nickname;
        }
    }
}