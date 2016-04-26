using System;

namespace Trash.Pma.Exceptions
{
    public class MethodDoesNotExistException : Exception
    {
        public MethodDoesNotExistException(string name) : base($"{name}は存在しません。") { }
    }
}
