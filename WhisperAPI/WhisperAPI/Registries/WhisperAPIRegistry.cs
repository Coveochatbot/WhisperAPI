using WhisperAPI.Services;

namespace WhisperAPI.Registries
{
    public class WhisperApiRegistry : StructureMap.Registry
    {
        public WhisperApiRegistry()
        {
            this.For<ISuggestionsService>().Use<SuggestionsService>();
        }
    }
}
