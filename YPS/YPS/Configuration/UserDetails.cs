namespace YPS
{
    public class UserDetails
    {
        public string TwitterId { get; set; }
        public string Name { get; set; }
        public string ScreenName { get; set; }
        public string Token { get; set; }
        public string TokenSecret { get; set; }
        public bool IsAuthenticated
        {
            get
            {
                return !string.IsNullOrWhiteSpace(Token);
            }
        }

        public string Expires { get; set; }
        public string Token_type { get; set; }
        public string id_token { get; set; }
    }
}
