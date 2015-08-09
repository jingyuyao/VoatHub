namespace VoatHub.Models
{
    public interface IOauthTokenMinimum
    {
        string access_token { get; set; }
        string token_type { get; set; }
        int expires_in { get; set; }
    }

    public interface IOauthToken : IOauthTokenMinimum
    {
        string scope { get; set; }
        string refresh_token { get; set; }
    }
}
