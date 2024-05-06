namespace BarryFileMan.Models.Config
{
    public class UserTMDBConfig
    {
        public string? ApiKey { get; set; }
        public bool IncludeAdult { get; set; }

        public UserTMDBConfig() : this(null, false) { }
        public UserTMDBConfig(string? apiKey, bool includeAdult) 
        { 
            ApiKey = apiKey; 
            IncludeAdult = includeAdult; 
        }
    }
}
