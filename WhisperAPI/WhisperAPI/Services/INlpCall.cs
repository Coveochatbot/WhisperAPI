using WhisperAPI.Models.NLPAPI;

namespace WhisperAPI.Services
{
    public interface INlpCall
    {
        NlpAnalysis GetNlpAnalyses(string sentence);
    }
}
