using System.Collections.Generic;

namespace WhisperAPI.Settings
{
    public class ApplicationSettings
    {
        public string ApiKey { get; set; }

        public string NlpApiBaseAddress { get; set; }

        public List<string> IrrelevantsIntents { get; set; }

        public string ContextLifeSpan { get; set; }
    }
}
