using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAInnovator.Converters
{
    public class ItemDef
    {
        public string ItemType;
        public string Stereotype;
        public string MetaType;
        public string[] Tags = new string[] { "item_number", "state", "major_rev", "description" };
        public string[] NameParts = new string[] { "name" };
    }

    public class ItemNode
    {
        public ItemDef relationship;
        public ItemDef relatedItem;
        public bool recursive = false;
        public ItemNode[] relationships;
    }

    public static class ModelDef
    {
        private const string MAIN_PACKAGE = "MBSE Model";

        public static ItemNode modelDef = new ItemNode
        {
            relationship = null,
            relatedItem = new ItemDef { ItemType = MAIN_PACKAGE, MetaType = "Package", Tags = null },

            relationships = new ItemNode[]
            { 
                // Model  -> Function
                new ItemNode
                {
                    relationship = new ItemDef { ItemType = "MBSE Model Functional Element", MetaType = "Flow" },
                    relatedItem  = new ItemDef { ItemType = "MBSE Function", MetaType = "Block", Stereotype = "function",
                                                 Tags = new string [] { "item_number", "state", "major_rev", "description"},
                                                 NameParts = new string [] {"name"}
                                               },

                    relationships = new ItemNode[]
                    { 
                        // 1. Function -> Function
                        new ItemNode
                        {
                            relationship = new ItemDef { ItemType = "MBSE Function Structure", MetaType = "Flow", Stereotype = "association" },
                            relatedItem  = new ItemDef { ItemType = "MBSE Function", MetaType = "Block", Stereotype = "function",
                                                         Tags = new string [] { "item_number", "state", "major_rev", "description"},
                                                         NameParts = new string [] {"name"}
                                                       },
                            recursive = true
                        },

                        // 2. Function -> Requirement
                        new ItemNode
                        {
                            relationship = new ItemDef { ItemType = "MBSE Function Requirement", MetaType = "Dependency", Stereotype = "satisfy" },
                            relatedItem  = new ItemDef { ItemType = "Requirement", MetaType = "Requirement", Stereotype = "requirement", NameParts = new string [] {"req_rm_title"} },
                        }
                    }
                },
                // Model -> Logical Element
                new ItemNode
                {
                    relationship = new ItemDef { ItemType = "MBSE Model Logical Element", MetaType = "Flow" },
                    relatedItem  = new ItemDef { ItemType = "MBSE Logical Element", MetaType = "Block", Stereotype = "system",
                                                 Tags = new string [] { "item_number", "state", "major_rev", "description"},
                                                 NameParts = new string [] {"name"}
                                               },

                    relationships = new ItemNode[]
                    { 
                        // 1. Logical -> Logical
                        new ItemNode
                        {
                            relationship = new ItemDef { ItemType = "MBSE Logical Structure", MetaType = "Flow", Stereotype = "association" },
                            relatedItem  = new ItemDef { ItemType = "MBSE Logical Element", MetaType = "Block", Stereotype = "system", NameParts = new string [] {"name"} },
                            recursive = true
                        },

                        // 2. Logical -> Function
                        new ItemNode
                        {
                            relationship = new ItemDef { ItemType = "MBSE Logical Function", MetaType = "Flow", Stereotype = "association" },
                            relatedItem  = new ItemDef { ItemType = "MBSE Function", MetaType = "Block", Stereotype = "system", NameParts = new string [] {"name"} },

                            //relationships = new ItemNode[]
                            //{
                            //    // 1. Logical -> Logical
                            //    new ItemNode
                            //    {
                            //        relationship = new ItemDef { ItemType = "Logical System Element BOM", MetaType = "Flow", Stereotype = "association" },
                            //        relatedItem  = new ItemDef { ItemType = "Logical System Element", MetaType = "Block", Stereotype = "system", NameParts = new string [] {"name"} },
                            //        recursive = true
                            //    },

                            //    // 2. Logical -> Part
                            //    new ItemNode
                            //    {
                            //        relationship = new ItemDef { ItemType = "Logical System Element Part", MetaType = "Flow", Stereotype = "realize" },
                            //        relatedItem  = new ItemDef { ItemType = "Part", MetaType = "Part", Stereotype = "physical element" },
                            //    },

                            //    // 3. Logical -> Requirement
                            //    new ItemNode
                            //    {
                            //        relationship = new ItemDef { ItemType = "Logical System Element Requirement", MetaType = "Dependency", Stereotype = "satisfy" },
                            //        relatedItem  = new ItemDef { ItemType = "Requirement", MetaType = "Requirement", Stereotype = "requirement", NameParts = new string [] {"req_rm_title"} },
                            //    }
                            //}
                        },

                        // 3. Logical -> Requirement
                        new ItemNode
                        {
                            relationship = new ItemDef { ItemType = "MBSE Logical Requirement", MetaType = "Dependency", Stereotype = "satisfy" },
                            relatedItem  = new ItemDef { ItemType = "Requirement", MetaType = "Requirement", Stereotype = "requirement", NameParts = new string [] {"req_rm_title"} },
                        }
                    }
                }
            }
        };
    }
}
