using System;
using System.Collections.Generic;
using System.Text;

namespace CafeLib.Data
{
    public interface IDatabase
    {
        string DatabaseName { get; }
        string ConnectionString { get; }
        IStorage GetStorage();
    }
}
