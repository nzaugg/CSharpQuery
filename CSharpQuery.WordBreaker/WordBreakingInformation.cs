using System.Collections.Generic;

namespace CSharpQuery.WordBreaker
{
    public class WordBreakingInformation
    {
        public IDictionary<string, string> Substitutions { get; set; }
        public IList<char> Whitespace { get; set; }
        public IDictionary<string, string> NoiseWords { get; set; }
    }
}