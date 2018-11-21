using System;
using System.Collections.Generic;
using System.Linq;

namespace WhisperAPI.Models.MegaGenial
{
    public class ModuleDetector
    {
        private static readonly Dictionary<Module, string> _vocabularyByModule = new Dictionary<Module, string>
        {
            { Module.Keypad, "dessins boutons quatres 4" },
            { Module.Maze, "quadrillé triangle rouge cercles verts point blanc 6 par six maze labyrinthe" },
            { Module.Memory, "mémoire memoire quatres chiffres 1234 gros nombres boutons premier deuxième troisième quatrième deuxieme troisieme quatrieme" },
            { Module.Password, "lettres submit 5 cinq flèches fleches tableau vert haut bas mot de passe password premier deuxième troisième quatrième cinquième deuxieme troisieme quatrieme cinquieme" },
            { Module.SimonSays, "simon says 4 carrés jaune bleu rouge vert clignote clignotant flash quatre" },
            { Module.WhosFirst, "mots étiquettes etiquettes boutons 6 six 2 colonnes rangées rangees deux they are blank read red you your you're their they're empty reed leeds there display says no lead hold on you are c c see ready first no blank nothing yes what u h h h left right middle okay wait press you you are your you're u r u uh huh staccato what question done next hold sure like whos who's" },
            { Module.WireComplicated, "fils complique compliqué stripes coupé coupe couper étoile etoile lumiere lumière rouge bleu piles ports vertical verticaux"},
            { Module.WireSequence, "coupé coupe couper fils premier deuxième troisième quatrième cinquième sixième septième septieme huitième huitieme neuvieme neuvième 123456789 deuxieme troisieme quatrieme cinquieme sixieme A B C a b c rouge bleu noir" },
            { Module.WireSimple, "3 trois 4 quatre 5 cinq 6 six couleurs fils simple premier deuxième troisième quatrième cinquième sixième deuxieme troisieme quatrieme cinquieme sixieme coupé coupe couper horizontal horizontaux" },
            { Module.None, string.Empty },
        };

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
                if (_vocabularyByModule[module].Split(" ").Contains(word))
                {
                    score += 1;
                }
            }
            return score;
        }
    }
}
