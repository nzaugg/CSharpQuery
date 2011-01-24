using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;

namespace CSharpQuery.WordBreaker
{
    public class WordBreakingInformationRetriever : IWordBreakingInformationRetriever
    {
        private IDictionary<string, string> substitutions = new Dictionary<string, string>();
        private IList<char> whitespace = new List<char>();
        private IDictionary<string, string> noiseWords = new Dictionary<string, string>();
        private string databasePath;
        private readonly CultureInfo cultureInfo;

        public WordBreakingInformationRetriever(string databasePath, CultureInfo cultureInfo)
        {
            this.databasePath = databasePath;
            this.cultureInfo = cultureInfo;
        }

        public WordBreakingInformation GetWordBreakingInformation()
        {
            string folder = databasePath;

            // Load the global Substitutions List
            string filename = Path.Combine(folder, string.Format("Substitutions.global.txt"));
            LoadSubstitutions(filename);

            // Load the regional Substitutions List
            filename = Path.Combine(folder, string.Format("Substitutions.{0}.txt", cultureInfo));
            LoadSubstitutions(filename);

            // Load the global whitespace list
            filename = Path.Combine(folder, string.Format("Whitespace.global.txt"));
            LoadWhitespace(filename);

            // Load the regional whitespace list
            filename = Path.Combine(folder, string.Format("Whitespace.{0}.txt", cultureInfo));
            LoadWhitespace(filename);

            // Load the global noise words list
            filename = Path.Combine(folder, string.Format("NoiseWords.global.txt"));
            LoadNoiseWords(filename);

            // Load the regional noise words list
            filename = Path.Combine(folder, string.Format("NoiseWords.{0}.txt", cultureInfo));
            LoadNoiseWords(filename);

            return null;
        }

        private void LoadNoiseWords(string filename)
        {
            if (noiseWords == null)
                noiseWords = new Dictionary<string, string>();

            if (!File.Exists(filename))
                return;

            TextReader rdr = new StreamReader(File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read), Encoding.Unicode);
            string line;
            while ((line = rdr.ReadLine()) != null)
            {
                line = line.Trim();
                if (!noiseWords.ContainsKey(line))
                    noiseWords.Add(line, null);
            }
            rdr.Close();
        }

        private void LoadWhitespace(string filename)
        {
            // Format: {char}\r\n
            if (whitespace == null)
                whitespace = new List<char>();

            if (!File.Exists(filename))
                return;

            TextReader rdr = new StreamReader(File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read), Encoding.Unicode);
            string line;
            while ((line = rdr.ReadLine()) != null)
            {
                // there should only be 1 char
                if (line.Length != 1)
                {
                    Trace.WriteLine(string.Format("Invlaid Line in file '{0}'. Line: {1}", filename, line));
                    continue; // ignore this
                }
                char chr = line[0];
                if (!whitespace.Contains(chr))
                    whitespace.Add(chr);
            }
            rdr.Close();
            whitespace.Add('\r');
            whitespace.Add('\n');
        }

        private void LoadSubstitutions(string filename)
        {
            // Format: À=A\r\n  OR À=\r\n
            if (substitutions == null)
                substitutions = new Dictionary<string, string>();

            if (!File.Exists(filename))
                return;

            TextReader rdr = new StreamReader(File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read), Encoding.Unicode);
            string line;
            while ((line = rdr.ReadLine()) != null)
            {
                string leftSide = line.Substring(0, line.IndexOf('=', 1));
                string rightSide = line.Substring(line.IndexOf('=', 1)).Replace("=", "");
                if (substitutions.ContainsKey(leftSide))
                    substitutions[leftSide] = rightSide;
                else
                    substitutions.Add(leftSide, rightSide);
            }
            rdr.Close();
        }
    }
}