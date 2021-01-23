using System;

namespace Utility.Development
{
    public class EmptyArrayException : Exception
    {
        public EmptyArrayException() : base("Array did not contain any elements.")
        {

        }
    }
}
