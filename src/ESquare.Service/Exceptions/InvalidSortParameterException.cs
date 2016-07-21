using System;

namespace ESquare.Service.Exceptions
{
    public class InvalidSortParameterException : Exception
    {
        public string Parameter { get; private set; }

        public InvalidSortParameterException(string parameter)
            : base("Invalid sort parameter")
        {
            Parameter = parameter;
        }
    }
}
