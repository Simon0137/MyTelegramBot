namespace MyTelegramBot
{
    public class MyUser
    {
        public long Id { get; set; }
        public string Nickname { get; set; }
        public string Timezone { get; set; }

        public MyUser(long id, string nickname, string timezone)
        {
            this.Id = id;
            this.Nickname = nickname;
            this.Timezone = timezone;
        }
    }
}
