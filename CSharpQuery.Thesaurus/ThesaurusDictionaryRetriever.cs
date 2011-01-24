using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace CSharpQuery.Thesaurus
{
    public class ThesaurusDictionaryRetriever : IThesaurusDictionaryRetriever
    {
        private readonly string databasePath;
        protected SortedList<string, SortedList<string, int>> thesaurusDictionary;

        public ThesaurusDictionaryRetriever(string databasePath)
        {
            this.databasePath = databasePath;
        }

        public SortedList<string, SortedList<string, int>> GetThesaurus()
        {
            // Thesaurus.global.xml
            var filename = Path.Combine(databasePath, "Thesaurus.global.xml");
            LoadThesaurus(filename);

            // Thesaurus.en-US.xml
            filename = Path.Combine(databasePath, "Thesaurus.global.xml");
            LoadThesaurus(filename);

            return thesaurusDictionary ?? new SortedList<string, SortedList<string, int>>();
        }

        private void LoadThesaurus(string filename)
        {
            if (!File.Exists(filename))
                return;
            // Sample XML
            //<XML ID="Microsoft Search Thesaurus">
            //    <thesaurus xmlns="x-schema:tsSchema.xml">
            //    <diacritics_sensitive>0</diacritics_sensitive>
            //        <expansion>
            //            <sub>abduct ion</sub>
            //            <sub>abduction</sub>
            //        </expansion>
            //        <expansion>
            //            <sub>abe lard</sub>
            //            <sub>abelard</sub>
            //        </expansion>
            //    </thesaurus>
            //</XML>

            // Read the XML
            var xmlDoc = XDocument.Load(new StreamReader(File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read)));

            var query = from e in xmlDoc.Descendants("expansion")
                        select e;

            foreach (var element in query)
            {
                var subs = (from s in element.Descendants("sub")
                            select s.Value).ToList();
                var syns = new SortedList<string, int>();
                foreach (var syn in subs)
                    syns.Add(syn, 0);

                foreach (var wrd in syns.Keys)
                {
                    if (thesaurusDictionary.ContainsKey(wrd))
                    {
                        foreach (var w in syns.Keys)
                        {
                            if (thesaurusDictionary.ContainsKey(w))
                                continue;
                            thesaurusDictionary[wrd].Add(w, 0);
                        }
                    }
                    else
                        thesaurusDictionary.Add(wrd, syns);
                }
            }
        }
    }
}