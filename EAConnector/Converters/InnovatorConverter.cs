using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;

using Aras.IOM;
using EAInnovator.Services;
using EAInnovator.Helpers;

namespace EAInnovator.Converters
{
    public class InnovatorConverter
    {
        private Dictionary<string, string> classSubstitution = new Dictionary<string, string>()
        {
            { "Block", "Class" },
            { "Part", "Object" },
        };

        public Item RootItem { get; private set; }

        public InnovatorConverter(Item rootItem)
        {
            RootItem = rootItem;
        }

        /// ----------------------------------------------------------------------------------------------------------------
        /// PLM to EA
        /// ----------------------------------------------------------------------------------------------------------------

        public EA.Element GetEAElementInPackage(Item innovatorItem, EA.Package activePackage)
        {
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

        public EA.Element GetEAElementByIDInPackage(string itemID, EA.Package activePackage)
        {
            foreach (EA.Element activeElement in activePackage.Elements)
            {
                if (activeElement.Tag.Length != 0)
                {
                    if (activeElement.Tag.IndexOf(itemID) > 0)
                    {
                        return activeElement;
                    }
                }
            }
            return null;
        }

        public EA.Package CreateEAPackage(EA.Repository Repository, string packageName, EA.Package parentPackage)
        {
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
                package = activeRepository.getPackageByName(startPackage, packageName);
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

        public EA.Element CreateEAInnovatorClass(Item innovatorItem, EA.Package classPackage, EAUtils activeRepository)
        {
            EA.Element returnElement = GetEAElementInPackage(innovatorItem, classPackage); ;
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
                    case "table": // polymorphic, federated
                    case null: // relationship type
                        EA.Element classElement = classPackage.Elements.AddNew(innovatorItem.getProperty("name"), "Table");
                        classElement.Alias = innovatorItem.getProperty("name").ToLower().Replace(" ", "_");
                        classElement.Notes = innovatorItem.getProperty("description", "");
                        classElement.Created = createdOn;
                        if (modifiedOn > createdOn)
                        {
                            classElement.Modified = modifiedOn;
                        }
                        classElement.Version = innovatorItem.getProperty("generation", "");
                        classElement.Tag = "[PLM]" + innovatorItem.getID();
                        classElement.Update();
                        classPackage.Update();
                        returnElement = classElement;
                        break;
                    default:
                        break;
                }
            }
            return returnElement;            
        }
        public EA.Element CreateEAInnovatorConnector(Item innovatorItem, EA.Package activePackage, EAUtils activeRepository)
        {
            string sourceId = innovatorItem.getProperty("source_id", "");
            string relatedId = innovatorItem.getProperty("related_id", "");
            string relationshipName = innovatorItem.getProperty("name", "");
            if (sourceId != null && relatedId != null)
            {
                EA.Element parentElement = GetEAElementByIDInPackage(sourceId, activePackage);
                EA.Element childElement = GetEAElementByIDInPackage(relatedId, activePackage);

                try
                {
                    // Add connector between structure elements
                    EA.Connector structureConnector = parentElement.Connectors.AddNew(relationshipName, "Association");
                    structureConnector.SupplierID = childElement.ElementID;
                    structureConnector.Update();
                }
                catch
                {
                    // No parent block element
                }
            }
            return null;
        }

        /// <summary>
        /// Load from PLM and create in EA
        /// </summary>
        public void LoadDatamodel(EA.Repository Repository)
        {
            if (System.Diagnostics.Debugger.Launch()) System.Diagnostics.Debugger.Break();
            EA.Collection collection = Repository.Models; // Get model collection
            EAUtils activeRepository = new EAUtils(Repository);


            EA.Package package = CreateEAPackage(Repository, "Innovator", null);
            EA.Package typePackage = CreateEAPackage(Repository, "ItemTypes", package);
            EA.Package relationPackage = CreateEAPackage(Repository, "RelationshipTypes", package);

            // Load ItemTypes
            Item innovatorItemTypes = InnovatorService._.GetItemTypes();
            // Load RelationshipTypes
            Item innovatorRelationshipTypes = InnovatorService._.GetRelationshipTypes();

            // Create EA objects
            int count = innovatorItemTypes.getItemCount();
            for (int i = 0; i < count; i++)
            {
                Item innovatorItemType = innovatorItemTypes.getItemByIndex(i);
                string typeName = innovatorItemType.getType();
                CreateEAInnovatorClass(innovatorItemType, typePackage, activeRepository);
                
            }
            typePackage.Update();
            package.Update();
            collection.Refresh();

            count = innovatorRelationshipTypes.getItemCount();
            for (int j = 0; j < count; j++)
            {
                Item innovatorRelationshipType = innovatorRelationshipTypes.getItemByIndex(j);
                string typeName = innovatorRelationshipType.getType();
                CreateEAInnovatorClass(innovatorRelationshipType, relationPackage, activeRepository);
            }
            relationPackage.Update();
            package.Update();
            collection.Refresh();

            //for (int k = 0; k < count; k++)
            //{
            //    Item innovatorRelationshipType = innovatorRelationshipTypes.getItemByIndex(k);
            //    string typeName = innovatorRelationshipType.getType();
            //    CreateEAInnovatorConnector(innovatorRelationshipType, typePackage, activeRepository);
            //}

            collection.Refresh();
        }
    }
}
