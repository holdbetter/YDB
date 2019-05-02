using System;
using System.Collections.Generic;
using System.Text;

namespace YDB.Services
{
    public interface IPathDatabase
    {
        string GetDataBasePath(string filename);
    }
}
