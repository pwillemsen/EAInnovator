using System;
using System.Collections.Generic;
using System.Globalization;

using Aras.IOM;
using EAInnovator.Services;
using EAInnovator.Helpers;

namespace EAInnovator.Converters
{
    /// <summary>
    /// Aras Innovator artifacts to Sparx Systems Enterprise Architect elements conversion and vice versa
    /// </summary>
    public class InnovatorConverter
    {
        private Dictionary<string, EA.Element> elementDictionary = new Dictionary<string, EA.Element>();

        public Item RootItem { get; private set; }

        public InnovatorConverter(Item rootItem)
        {
            RootItem = rootItem;
        }

        /// ----------------------------------------------------------------------------------------------------------------
        /// PLM to EA
        /// ----------------------------------------------------------------------------------------------------------------

        public EA.Element SetEAElementInPackage(EA.Package activePackage)
        {
            /// <summary>
            /// Create initial dictionary of EA Elements witinn the package
            /// NOTE: may be placed in EAUtils on initialization
            /// <paramref name="activePackage"/> EA Package
            /// </summary>
            foreach (EA.Element activeElement in activePackage.Elements)
            {
                if (activeElement.Tag.Length != 0)
                {
                    elementDictionary.Add(activeElement.Tag.Replace("[PLM]", ""), activeElement);
                }
            }
            return null;
        }

        public EA.Element GetEAElementInPackage(Item innovatorItem, EA.Package activePackage)
        {
            /// <summary>
            /// Return EA Element by element [PLM] Tag
            /// NOTE: Slow performance
            /// <paramref name="innovatorItem"/> Innovator Item (ItemType)
            /// <paramref name="activePackage"/> EA Package
            /// </summary>
            foreach (EA.Element activeElement in activePackage.Elements)
            {
                if (activeElement.Tag.Length != 0)
                {
                    if (activeElement.Tag.IndexOf(innovatorItem.getID()) > 0 || activeElement.Tag.IndexOf(innovatorItem.getProperty("config_id", "-")) > 0)
                    {
                        return activeElement;
                    }
                }
            }
            return null;
        }

        public EA.Element GetEAElementByInnovatorID(String id)
        {
            /// <summary>
            /// Return EA Element from dictionary
            /// <paramref name="id"/> Innovator ID
            /// </summary>
            EA.Element element = null;
            if (elementDictionary.TryGetValue(id, out element))
                return element;
            else
                return null;
        }

        public EA.Package CreateEAPackage(EA.Repository Repository, string packageName, EA.Package parentPackage)
        {
            /// <summary>
            /// Create EA package from root or as sub-package
            /// <paramref name="Repository"/> Active repository
            /// <paramref name="packageName"/> Name of the new package
            /// <paramref name="parentPackage"/> Name of the parent package (root if empty)
            /// </summary>
            EA.Collection collection = Repository.Models; // Get model collection
            EA.Package rootPackage = collection.GetAt(0);
            EA.Package package = null;
            EAUtils activeRepository = new EAUtils(Repository);
            
            if (rootPackage != null)
            {
                EA.Package startPackage = rootPackage;
                if (parentPackage != null)
                {
                    // Check from the parent package
                    startPackage = parentPackage;
                }
                // Check if a package with packageName already exists
                package = activeRepository.GetPackageByName(startPackage, packageName);
                if (package != null)
                {
                    // Package exists and we start collect all elements in a dictionary for performance reasons
                    SetEAElementInPackage(package);
                }
            }
            
            if (rootPackage != null && package == null && parentPackage == null)
            {
                // Package does not exist and no parent package, create a new package in the root package
                package = rootPackage.Packages.AddNew(packageName, "");
                package.Update();
                package.Element.Stereotype = "model";
            }
            else if (rootPackage != null && package == null && parentPackage != null)
            {
                // Package does not exist, create a new package from the parent package
                package = parentPackage.Packages.AddNew(packageName, "");
                package.Update();
                package.Element.Stereotype = "model";
            }
            else if (package == null)
            {
                // Create a new root package
                package = collection.AddNew(packageName, "Package");
                package.Update();
            }

            return package;
        }

        public EA.Element CreateEAElement(Item innovatorItem, EA.Package classPackage)
        {
            /// <summary>
            /// <paramref name="innovatorItem"/> Innovator Item (ItemType)
            /// <paramref name="classPackage"/> EA Package (Innovator/ItemTypes or Innovator/RelationshipTypes)
            /// </summary>
            string itemID = innovatorItem.getID();
            EA.Element returnElement = GetEAElementByInnovatorID(itemID);
            if (returnElement == null)
            {
                // Dictionary may be empty, so we need to check by using the package
                // Unfortunately this check is much slower
                returnElement = GetEAElementInPackage(innovatorItem, classPackage);
            }
            if (returnElement == null)
            { 
                DateTime createdOn = Tools.GetDate(innovatorItem.getProperty("created_on", ""));
                string innovatorModifiedOn = innovatorItem.getProperty("modified_on", "");
                // In rare cases the modified_on date is empty
                DateTime modifiedOn = DateTime.Parse("1900-01-01", CultureInfo.InvariantCulture);
                if (innovatorModifiedOn != "")
                {
                    modifiedOn = Tools.GetDate(innovatorItem.getProperty("modified_on", ""));
                }

                string implementationType = innovatorItem.getProperty("implementation_type");
                switch (implementationType)
                {
                    case "table": // TODO: polymorphic, federated need to be implemented
                    case null: // relationship type
                        EA.Element classElement = classPackage.Elements.AddNew(innovatorItem.getProperty("name"), "Table");
                        classElement.Alias = innovatorItem.getProperty("label");
                        classElement.Notes = innovatorItem.getProperty("description", "");
                        classElement.Created = createdOn;
                        if (modifiedOn > createdOn)
                        {
                            classElement.Modified = modifiedOn;
                        }
                        classElement.Version = innovatorItem.getProperty("generation", "");
                        classElement.Tag = "[PLM]" + itemID;
                        classElement.Update();
                        classPackage.Update();
                        returnElement = classElement;
                        break;
                    default:
                        break;
                }
            }
            if (!elementDictionary.ContainsKey(itemID))
            {
                elementDictionary.Add(itemID, returnElement);
            }
            return returnElement;            
        }

        public EA.Element CreateEAConnector(Item innovatorItem)
        {
            /// <summary>
            /// Create EA Connector between two EA Elements (SupplierID)
            /// <paramref name="innovatorItem"/> Innovator Item (RelationshipType)
            /// </summary>
            string sourceId = innovatorItem.getProperty("source_id", "");
            string relatedId = innovatorItem.getProperty("related_id", "");
            string relationshipName = innovatorItem.getProperty("name", "");
            if (sourceId != null && relatedId != null)
            {
                try
                {
                    EA.Element parentElement = GetEAElementByInnovatorID(sourceId);
                    EA.Element childElement = GetEAElementByInnovatorID(relatedId);
                    bool connectorExists = false;
                    foreach (EA.Connector parentConnector in parentElement.Connectors)
                    {
                        // Check if the connector already exists
                        if (parentConnector.Name == relationshipName)
                        {
                            connectorExists = true;
                        }
                    }
                    if (!connectorExists)
                    {
                        // Add connector between structure elements
                        EA.Connector structureConnector = parentElement.Connectors.AddNew(relationshipName, "Association");
                        structureConnector.SupplierID = childElement.ElementID;
                        structureConnector.Update();
                    }
                }
                catch
                {
                    // No parent block element or no connectors
                }
            }
            return null;
        }

        public void LoadDatamodel(EA.Repository Repository)
        {
            /// <summary>
            /// Load data model from PLM and create elements in EA
            /// </summary>
            
            // if (System.Diagnostics.Debugger.Launch()) System.Diagnostics.Debugger.Break();
            EA.Collection collection = Repository.Models; // Get model collection
            EAUtils activeRepository = new EAUtils(Repository);

            EA.Package package = CreateEAPackage(Repository, "Innovator", null);
            EA.Package typePackage = CreateEAPackage(Repository, "ItemTypes", package);
            EA.Package relationPackage = CreateEAPackage(Repository, "RelationshipTypes", package);

            // Load ItemTypes
            Item innovatorItemTypes = InnovatorService._.GetItemTypes();
            // Load RelationshipTypes
            Item innovatorRelationshipTypes = InnovatorService._.GetRelationshipTypes();

            // Create EA Elements (<<Table>>) in Innovator/ItemTypes Package
            int count = innovatorItemTypes.getItemCount();
            for (int i = 0; i < count; i++)
            {
                Item innovatorItemType = innovatorItemTypes.getItemByIndex(i);
                string typeName = innovatorItemType.getType();
                CreateEAElement(innovatorItemType, typePackage);
            }

            // Create EA Elements (<<Table>>) in Innovator/RelationshipTypes Package
            count = innovatorRelationshipTypes.getItemCount();
            for (int j = 0; j < count; j++)
            {
                Item innovatorRelationshipType = innovatorRelationshipTypes.getItemByIndex(j);
                string typeName = innovatorRelationshipType.getType();
                CreateEAElement(innovatorRelationshipType, relationPackage);
            }

            // Create EA Connectors between tables (Relationships)
            for (int k = 0; k < count; k++)
            {
                Item innovatorRelationshipType = innovatorRelationshipTypes.getItemByIndex(k);
                string typeName = innovatorRelationshipType.getType();
                CreateEAConnector(innovatorRelationshipType);
            }

            collection.Refresh();
        }
    }
}
