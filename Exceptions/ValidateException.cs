using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIGestao.Exceptions
{
    public class ValidateException : Exception
    {
        public ValidateException(string message) : base(message)
        {
        }
    }
}
