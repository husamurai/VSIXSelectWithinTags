namespace SelectLTHashHashGT
{
    internal class Titler
    {
        private string endTag = string.Empty;
        private string startTag = string.Empty;

        protected Titler()
        {
        }
        protected Titler(string startTag, string endTag)
        {
            this.startTag = startTag;
            this.endTag = endTag;
        }

        protected string EndTag { get => endTag; set => endTag = value; }
        protected string StartTag { get => startTag; set => startTag = value; }
        protected string Title
        {
            get
            {
                return $"Selector of block {startTag} ... {endTag}";
            }
        }
        protected bool Valid
        {
            get
            {
                return !string.IsNullOrEmpty(startTag) && !string.IsNullOrEmpty(endTag);
            }
        }
    }
}