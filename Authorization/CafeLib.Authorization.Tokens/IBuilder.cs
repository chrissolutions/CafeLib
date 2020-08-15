using System;
using System.Collections.Generic;
using System.Text;

namespace CafeLib.Authorization.Tokens
{
    public interface IBuilder<out T>
    {
        T Build();
    }
}
