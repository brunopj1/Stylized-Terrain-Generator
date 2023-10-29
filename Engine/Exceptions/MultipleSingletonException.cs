using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Exceptions;

class MultipleSingletonException<T> : Exception
{
    public MultipleSingletonException()
        : base($"Multiple instances of the singleton \"{nameof(T)}\" were created.")
    {
    }
}
