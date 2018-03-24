using Pradox.ConsolePlus.Core;
using System;


namespace Pradox.ConsolePlus.Helpers
{
    static class AttributeHelper
    {
        public static CommandModuleAttribute GetAttribute(Type t)
        {
            var attribute = (CommandModuleAttribute)Attribute.GetCustomAttribute(t, typeof(CommandModuleAttribute));

            return attribute;
        }
    }
}
