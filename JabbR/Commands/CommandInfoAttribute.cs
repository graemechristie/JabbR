using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JabbR.Commands
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class CommandInfoAttribute : Attribute
    {
        public string Name { get; set; }
        public string Usage { get; set; }
        public float Weight { get; set; }
    }
}