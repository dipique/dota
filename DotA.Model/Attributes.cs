using System;

namespace DotA.Model.Attributes
{
    /// <summary>
    /// By default, the option text will just be the enum text. Hierarchies are
    /// separated by underscore. Optionally, other text can be added for display. These
    /// are also separated by underscore if hierarchical.
    /// </summary>
    public sealed class DisplayText : Attribute
    {
        public string Text { get; set; }
        public DisplayText(string desc)
        {
            Text = desc;
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
    /// Sometimes there will be an effect class identifier that needs to be assigned to a different
    /// field. This attribute stores these so it can be assigned differently based on ID.
    /// </summary>
    public sealed class AltJID : Attribute
    {
        public string ID { get; set; }
        public string Dest { get; set; }
        public AltJID(string id, string dest)
        {
            ID = id;
            Dest = dest;
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

    /// <summary>
    /// Indicates that this should not be displayed
    /// </summary>
    public sealed class NoDisplay : Attribute
    {
        public NoDisplay() { }
    }

    /// <summary>
    /// Indicates that this cannot be edited
    /// </summary>
    public sealed class DisplayOnly : Attribute
    {
        public DisplayOnly() { }
    }


    public sealed class DefaultEntryProperty : Attribute
    {
        public string PropertyName { get; set; }
        public DefaultEntryProperty(string propertyName)
        {
            PropertyName = propertyName;
        }
    }

    /// <summary>
    /// Indicates in what order the field should be arranged. Items of the same Order can also
    /// be grouped, causing them to have visually distinguishable display characteristics.
    /// </summary>
    public sealed class FieldOrder : Attribute
    {
        public int Order { get; set; }
        public string GroupName { get; set; }
        public FieldOrder(int val, string group = null)
        {
            Order = val;
            GroupName = group;
        }
    }

    public sealed class PrimaryKey: Attribute
    {
        public PrimaryKey() { }
    }
}
