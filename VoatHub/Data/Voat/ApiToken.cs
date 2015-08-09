﻿namespace VoatHub.Data.Voat
{
    public class ApiToken : IOauthTokenMinimum
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public string userName { get; set; }
    }
}