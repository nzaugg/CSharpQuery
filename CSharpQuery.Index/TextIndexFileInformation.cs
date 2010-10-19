namespace CSharpQuery.Index
{
    public class TextIndexFileInformation
    {
        public string BeginFile = (char)01 + "CSharpQuery_Index" + (char)01;
        public string BeginRecord = (char)02 + "";
        public string BeginField = (char)03 + "";
        public string FieldInfoDelimeter = (char)04 + "";
        public string EndField = (char)05 + "";
        public string EndRecord = (char)06 + "\r\n";
        public string EndFile = (char)07 + "";
    }
}