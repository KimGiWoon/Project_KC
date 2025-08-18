using System;

namespace SDW
{
    [Serializable]
    public class UserData
    {
        //# 한글 주석
        public string Email;
        public string JoinDate;
        public string Nickname;

        /// <summary>
        /// UserData 초기화
        /// </summary>
        /// <param name="email">설정할 email</param>
        /// <param name="joinDate">설정할 joinDate</param>
        /// <param name="nickname">설정할 nickname</param>
        public UserData(string email, string joinDate, string nickname)
        {
            //# 초기 설정
            Email = email;
            JoinDate = joinDate;
            Nickname = nickname;
        }
    }
}