using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotA.Model.Attributes
{
    /// <summary>
    /// By default, the option text will just be the enum text. Hierarchies are
    /// separated by underscore. Optionally, other text can be added for display. These
    /// are also separated by underscore if hierarchical.
    /// </summary>
    public sealed class OptionText : Attribute
    {
        public string Description { get; set; }
        public OptionText(string desc)
        {
            Description = desc;
        }
    }

    /// <summary>
    /// JSON identifiers for certain elements
    /// </summary>
    public sealed class JID : Attribute
    {
        public string[] IDs { get; set; }
        public JID(params string[] ids)
        {
            IDs = ids;
        }
    }

    /// <summary>
    /// Indicates what section types, if any, have a special handler within that implementation of Parseable
    /// </summary>
    public sealed class SpecialHandlerSectionMethod : Attribute
    {
        public string SectionType { get; set; }
        public SpecialHandlerSectionMethod(string sectionType)
        {
            SectionType = sectionType;
        }
    }

    /// <summary>
    /// For enums defined in Dota already, the prefix is removed from each value for easier coding. It is kept to make
    /// it easier to parse files.
    /// </summary>
    public sealed class Prefix : Attribute
    {
        public string Value { get; set; }
        public Prefix(string val)
        {
            Value = val;
        }
    }

    /// <summary>
    /// Determines which field should store the value provided. If blank it will be
    /// stored in the "Amount" field.
    /// </summary>
    public sealed class ValueDest : Attribute
    {
        public string DestProperty { get; set; }
        public ValueDest(string prop = "Amount")
        {
            DestProperty = prop;
        }
    }

    /// <summary>
    /// For this effect class, we expect to see certain other attributes
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public sealed class ExpectedEntry : Attribute
    {
        public string Indicator { get; set; }
        public string DestField { get; set; }
        public ExpectedEntry(string ind, string dest)
        {
            Indicator = ind;
            DestField = dest;
        }
    }

    public sealed class InputHeader : Attribute
    {
        public string Header { get; set; }
        public InputHeader(string header)
        {
            Header = header;
        }
    }

    public sealed class ImageFolder : Attribute
    {
        public string FolderName { get; set; }
        public ImageFolder(string folder)
        {
            FolderName = folder;
        }
    }

    /// <summary>
    /// Indicates that a given effect should be assumed to be an active (passive is the default)
    /// </summary>
    public sealed class ActiveEffect : Attribute
    {
        public ActiveEffect() { }
    }

    /// <summary>
    /// Indicates that a given value is a percentage
    /// </summary>
    public sealed class Percentage : Attribute
    {
        public Percentage() { }
    }

    /// <summary>
    /// Indicates that this comes as a negative number that needs to be flipped to a positive one
    /// </summary>
    public sealed class FlipNegative : Attribute
    {
        public FlipNegative() { }
    }

    public sealed class DefaultEntryProperty : Attribute
    {
        public string PropertyName { get; set; }
        public DefaultEntryProperty(string propertyName)
        {
            PropertyName = propertyName;
        }
    }
}
