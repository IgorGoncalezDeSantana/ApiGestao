using APIGestao.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIGestao.Contracts
{
    interface IValidate
    {
        public void Validate(EValidateType type);
    }
}
