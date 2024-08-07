﻿namespace Parcel.CoreEngine.Document
{
    /// <remarks>
    /// Remark-cz: Keep it clean and implicit. No need for additional adnorments or information.
    /// </remarks>
    public sealed class ParcelNode
    {
        #region Constructors
        public ParcelNode(string name, string target, Dictionary<string, string>? attributes = null)
        {
            Name = name;
            Target = target;
            Attributes = attributes ?? [];
        }
        public ParcelNode(string target, Dictionary<string, string>? attributes = null)
        {
            Name = target.Split(':').Last();
            Target = target;
            Attributes = attributes ?? [];
        }
        #endregion

        #region Properties
        public string Name { get; set; }
        public string Target { get; set; }
        public Dictionary<string, string> Attributes { get; set; }
        public List<string> Tags { get; set; }
        #endregion
    }
}
