using System.Globalization;

namespace CSharpQuery.Index
{
    public class TextFileAccessContext
    {
        public TextFileAccessContext(string name, string directory, CultureInfo culture)
        {
            Name = name;
            Directory = directory;
            Culture = culture;
        }

        public string Name { get; private set; }
        public string Directory { get; private set; }
        public CultureInfo Culture { get; private set; }
    }
}