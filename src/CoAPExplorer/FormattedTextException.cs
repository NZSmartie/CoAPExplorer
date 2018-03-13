using System;

namespace CoAPExplorer
{
    public class FormattedTextException : Exception
    {
        public FormattedTextException(string message, int line, int offset) 
            : base(message)
        {
            Line = line;
            Offset = offset;
        }

        public FormattedTextException(string message, int line, int offset, Exception innerException) 
            : base(message, innerException)
        {
            Line = line;
            Offset = offset;
        }

        public int Line { get; }

        public int Offset { get; }
    }
}
