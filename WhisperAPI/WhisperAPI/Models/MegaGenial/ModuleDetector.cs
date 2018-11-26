using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace WhisperAPI.Models.MegaGenial
{
    public class ModuleDetector
    {
        private static readonly Dictionary<Module, List<int[]>> _intVocabularyByModule = new Dictionary<Module, List<int[]>>();
        private static readonly Dictionary<Module, string> _vocabularyByModule = new Dictionary<Module, string>();

        public ModuleDetector()
        {
            if (!_vocabularyByModule.Any())
            {
                LoadVocabularyFromFile();
                GenerateIntVocabulary();
            }
        }

        public static void RefreshVocabulariesFromFile()
        {
            LoadVocabularyFromFile();
            GenerateIntVocabulary();
        }

        public List<(Module, int)> DetectModuleList(string textContent)
        {
            var scoresByModule = this.GetScoresByModule(textContent);
            var nonZeroModules = scoresByModule.Where(x => x.Value != 0);
            if (nonZeroModules.Count() == 0)
            {
                return Enumerable.Empty<(Module, int)>().ToList();
            }
            else
            {
                return nonZeroModules.OrderByDescending(x => x.Value).Take(3).Select(x => (x.Key, x.Value)).ToList();
            }
        }

        private Dictionary<Module, int> GetScoresByModule(string textContent)
        {
            var scoreByModule = new Dictionary<Module, int>();
            foreach (var module in (Module[])Enum.GetValues(typeof(Module)))
            {
                scoreByModule.Add(module, this.GetScore(module, textContent));
            }

            return scoreByModule;
        }

        private int GetScore(Module module, string textContent)
        {
            var score = 0;
            foreach (var word in textContent.ToLower().Split(" "))
            {
                if (word.Length <= 3 && this.IsSimpleWordInModule(_vocabularyByModule[module], word))
                {
                    score += 1;
                }
                else if (word.Length > 3 && this.IsComplexWordInModule(_intVocabularyByModule[module], word))
                {
                    score += 1;
                }
            }

            return score;
        }

        private bool IsSimpleWordInModule(string vocabulary, string word)
        {
            return vocabulary.Split(" ").Contains(word);
        }

        private bool IsComplexWordInModule(List<int[]> intVocabulary, string word)
        {
            var intWord = DistanceCalculator.ConvertWord(word);
            foreach (var moduleWord in intVocabulary)
            {
                if (DistanceCalculator.GetDistance(intWord, moduleWord) <= 1)
                {
                    return true;
                }
            }

            return false;
        }

        private static void LoadVocabularyFromFile()
        {
            var text = File.ReadAllText("vocabulary.json", Encoding.GetEncoding("iso-8859-1"));
            var deserializedDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(text);
            foreach (var (module, vocabulary) in deserializedDictionary)
            {
                var moduleEnum = (Module)Enum.Parse(typeof(Module), module, true);
                _vocabularyByModule[moduleEnum] = vocabulary;
            }
        }

        private static void GenerateIntVocabulary()
        {
            foreach (var (module, vocabulary) in _vocabularyByModule)
            {
                _intVocabularyByModule[module] = vocabulary.Split(' ').Select(x => DistanceCalculator.ConvertWord(x)).ToList();
            }
        }
    }
}
