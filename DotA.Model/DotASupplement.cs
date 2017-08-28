using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotA.Model
{
    /// <summary>
    /// This class contains data that changes the final DotAData contents from
    /// the unaltered read based on DotA files and configuration settings. This
    /// may be because data is incorrect or because there is additional data
    /// that needs to be added.
    /// </summary>
    public class DotASupplement
    {
        #region Text changes to source entry values

        public List<EntryEdit> EntryEdits { get; set; } = new List<EntryEdit>();
        public List<EntryAdd> EntryAdds { get; set; } = new List<EntryAdd>();

        /// <summary>
        /// Class that contains edits to items post-processing
        /// </summary>
        public class ItemEdit : SupplementTarget
        {
            public object Value { get; set; }

            public void SetValue<T>(T item ) where T: Parseable
            {
                //Get the target property
                object setObject = null;
            }
        }

        /// <summary>
        /// Class for modifications to existing entries in text input files (pre-processing)
        /// </summary>
        public class EntryEdit: EntrySupplement
        {
            //If this is true, the title is edited instead of the value
            public bool TitleEdit { get; set; } = false;
        }

        /// <summary>
        /// Class for new adding entries to text input files (pre-processing)
        /// </summary>
        public class EntryAdd: EntrySupplement
        {
            public string TitleValue { get; set; }
        }

        /// <summary>
        /// Base class for entry modifications
        /// </summary>
        public abstract class EntrySupplement : SupplementTarget
        {
            //The new value placed in this position
            public string NewValue { get; set; }
        }

        /// <summary>
        /// Base class for nagivating item hierarchies to modify values
        /// </summary>
        public abstract class SupplementTarget
        {
            //Hero, ability, item
            public Type EntityType { get; set; }

            public string EntityName { get; set; }

            //Allows edits to navigate parameter hierarchy. Typical value would be "AbilitySpecial", "02", "movement_speed"
            public List<string> TargetHierarchy { get; set; } = new List<string>();
        }

        #endregion
    }
}
