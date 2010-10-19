namespace CSharpQuery.Index
{
    public class TextIndexFileInformation
    {
        public virtual string BeginFile
        {
            get { return (char) 01 + "CSharpQuery_Index" + (char) 01; }
        }

        public virtual string BeginRecord
        {
            get { return (char) 02 + ""; }
        }

        public virtual string BeginField
        {
            get { return (char) 03 + ""; }
        }

        public virtual string FieldInfoDelimeter
        {
            get { return (char) 04 + ""; }
        }

        public virtual string EndField
        {
            get { return (char) 05 + ""; }
        }

        public virtual string EndRecord
        {
            get { return (char) 06 + "\r\n"; }
        }

        public virtual string EndFile
        {
            get { return (char) 07 + ""; }
        }
    }
}