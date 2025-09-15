namespace SDW
{
    public struct UserInfo
    {
        public string Nickname;
        public string Email;
        public string UserId;
        public int IconNumber;

        public UserInfo(string nickname, string email, string userId, int iconNumber)
        {
            Nickname = nickname;
            Email = email;
            UserId = userId;
            IconNumber = iconNumber;
        }
    }
}