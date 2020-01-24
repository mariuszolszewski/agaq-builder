using System;

namespace AgaQ.UI.Inspector
{
    /// <summary>
    /// Translation key for propery label.
    /// </summary>
    public class LabelKeyAttribute : Attribute
    {
        public readonly string key;

        public LabelKeyAttribute(string key)
        {
            this.key = key;
        }
    }
}
