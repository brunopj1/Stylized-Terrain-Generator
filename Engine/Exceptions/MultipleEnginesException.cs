using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Exceptions;

class MultipleEnginesException : Exception
{
    public MultipleEnginesException()
        : base("Only one instance of EngineBase can be created at a time.")
    {
    }
}
