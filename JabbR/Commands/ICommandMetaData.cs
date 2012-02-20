using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JabbR.Commands
{
    public interface ICommandMetaData
    {
        string Name { get; }
        string Usage { get; }
        float Weight { get; }
    }
}