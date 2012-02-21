using System;
using System.ComponentModel.Composition;

namespace JabbR.Commands
{
    [MetadataAttribute, AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class CommandMetadataAttribute : ExportAttribute
    {
        public CommandMetadataAttribute()
            : base(typeof(ICommand))
        {
        }

        public string Name { get; set; }
        public string Usage { get; set; }
        public float Weight { get; set; }
    }
}