﻿namespace Parcel.NExT.Interpreter.Scripting
{
    [Serializable]
    public class RecursiveParsingException : Exception
    {
        public RecursiveParsingException()
        {
        }

        public RecursiveParsingException(string message) : base(message)
        {
        }

        public RecursiveParsingException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}