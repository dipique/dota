using System;
using System.Collections.Generic;
using System.Text;

using DotA.Model;
using DotA.Model.Enums;

namespace DotA.Parsing
{
    class DotAParser
    {
        //https://raw.githubusercontent.com/kil0gram/dota2/initmaster/dotadata/Model/ItemsClass.cs
        //This was used as a starting point for this method, but had to be re-written for the most part
        public static List<Item> ParseItemsText(string[] text)
        {
            bool itemfound = false;

            //list to hold our parsed items.
            List<Item> items = new List<Item>();

            //this will be used to store this sectoion
            //of the item.
            List<string> curitem = new List<string>();

            //item object will will be populating
            Item item = new Item();

            //lets go line by line to start parsing.
            foreach (string line in text)
            {
                //clean up the text, remove quotes.
                string line_noquotes = line.Replace("\"", "");
                string trimmed_clean = line.Replace("\"", "").Replace("\t", "").Replace("_", " ").Trim();

                //if line starts with item_ then
                //this is where we will start capturing.
                if (line_noquotes.StartsWith("	item_"))
                {
                    item = new Item();
                    itemfound = true;
                    item.Name = trimmed_clean.Replace("item ", "");
                    item.Name = StringManipulation.UppercaseFirst(item.name);
                    curitem.Add(trimmed_clean);
                }

                //if we are on a current item then lets do
                //some other operations to gather details
                if (itemfound == true)
                {
                    //parse ID
                    if (trimmed_clean.StartsWith("ID"))
                    {
                        item.id = trimmed_clean.Replace("ID", "").Split('/')[0];
                        curitem.Add(line);
                    }

                    //parse cast range
                    if (trimmed_clean.StartsWith("AbilityCastRange"))
                    {
                        item.castrange = trimmed_clean.Replace("AbilityCastRange", "");
                        curitem.Add(trimmed_clean);
                    }

                    //parse cast point
                    if (trimmed_clean.StartsWith("AbilityCastPoint"))
                    {
                        item.castpoint = trimmed_clean.Replace("AbilityCastPoint", "");
                        curitem.Add(trimmed_clean);
                    }

                    //parse cast point
                    if (trimmed_clean.StartsWith("AbilityCooldown"))
                    {
                        item.cooldown = trimmed_clean.Replace("AbilityCooldown", "");
                        curitem.Add(trimmed_clean);
                    }

                    if (trimmed_clean.StartsWith("AbilityManaCost"))
                    {
                        item.manacost = trimmed_clean.Replace("AbilityManaCost", "");
                        curitem.Add(trimmed_clean);
                    }

                    if (trimmed_clean.StartsWith("ItemCost"))
                    {
                        item.ItemCost = trimmed_clean.Replace("ItemCost", "");
                        curitem.Add(trimmed_clean);
                    }
                    //
                    if (trimmed_clean.StartsWith("ItemShopTags"))
                    {
                        item.ItemShopTags = trimmed_clean.Replace("ItemShopTags", "");
                        curitem.Add(trimmed_clean);
                    }

                    if (trimmed_clean.StartsWith("ItemQuality"))
                    {
                        item.ItemQuality = trimmed_clean.Replace("ItemQuality", ""); ;
                        curitem.Add(trimmed_clean);
                    }

                    if (trimmed_clean.StartsWith("ItemAliases"))
                    {
                        item.ItemAliases = trimmed_clean.Replace("ItemAliases", ""); ;
                        curitem.Add(trimmed_clean);
                    }

                    if (trimmed_clean.StartsWith("ItemStackable"))
                    {
                        item.ItemStackable = trimmed_clean.Replace("ItemStackable", ""); ;
                        curitem.Add(trimmed_clean);
                    }

                    if (trimmed_clean.StartsWith("ItemShareability"))
                    {
                        item.ItemShareability = trimmed_clean.Replace("ItemShareability", ""); ;
                        curitem.Add(trimmed_clean);
                    }

                    if (trimmed_clean.StartsWith("ItemShareability"))
                    {
                        item.ItemShareability = trimmed_clean.Replace("ItemShareability", ""); ;
                        curitem.Add(trimmed_clean);
                    }

                    //end current item, save to list
                    if (trimmed_clean.StartsWith("//="))
                    {


                        //add to our list of items/
                        items.Add(item);
                        curitem.Add(trimmed_clean);
                        itemfound = false;
                    }

                }

            }

            return items;
        }

    }
}
