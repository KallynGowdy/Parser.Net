using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GrammarBuilder
{
    public interface IFactory<T>
    {
        T Get();
    }

    public interface IFactory<T, TParam>
    {
        T Get(TParam param);
    }

    public interface IFactory<T, TParam1, TParam2>
    {
        T Get(TParam1 param1, TParam2 param2);
    }
}
