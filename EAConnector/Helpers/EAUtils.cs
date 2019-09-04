using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Text.RegularExpressions;
using System.Windows.Forms;


namespace EAInnovator
{
    public class EAUtils
    {
        private EA.Repository rep = null;
        private EA.Package rootPackage = null;
        private List<string> allowedTypes = new List<string>(new string[] { "Package", "Requirement", "UseCase" });
        private Dictionary<string, EA.Element> elementDictionary = new Dictionary<string, EA.Element>();
        private List<EA.Diagram> diagramList = new List<EA.Diagram>();
        private int diagramObjectCounter = 0;

        public EAUtils(EA.Repository rep)
        {
            this.rep = rep;
            initElementDictionary();
            setDiagramList();
        }

        private Boolean checkConditions(EA.Element element, List<IEACCondition> conditionList)
        {
            Boolean result = true;
            foreach (IEACCondition c in conditionList)
            {
                String compareValue = null;
                if (c.getVariable().Equals("Name"))
                    compareValue = element.Name;
                else if (c.getVariable().Equals("Notes"))
                    compareValue = element.Notes;
                else if (c.getVariable().Equals("Stereotype"))
                    compareValue = element.Stereotype;
                else if (c.getVariable().Equals("Type"))
                    compareValue = element.Type;
                else
                    compareValue = getTaggedValue(element, "", c.getVariable());
                if (compareValue == null)
                {
                    return false;
                }
                if (c.getComparator().Equals("=="))
                {
                    if (!compareValue.Equals(c.getValue())) result = false;
                }
                if (c.getComparator().Equals("!="))
                {
                    if (compareValue.Equals(c.getValue())) result = false;
                }
                if (c.getComparator().Equals("contains"))
                {
                    if (compareValue.IndexOf(c.getValue()) == -1) result = false;
                }
            }
            return result;
        }

        public void clearSuspect(String documentId)
        {
            List<EA.Element> elementList = getRequirementList(documentId, getPackageByName(rootPackage, "Trash Bin"));
            foreach (EA.Element element in elementList)
            {
                element.Locked = false;
                element.SetAppearance(1, 0, 11857588);
                element.Update();
                element.Locked = true;
                foreach (EA.Diagram diagram in diagramList)
                {
                    EA.DiagramObject diagramObject = diagram.GetDiagramObjectByID(element.ElementID, "");
                    if (diagramObject != null)
                        diagramObject.Update();
                    diagram.Update();
                }
            }
        }

        public void createConnector(EA.Element sourceElement, EA.Element targetElement, String connectorName)
        {
            EA.Collection collection = targetElement.Connectors;
            EA.Connector newConnector = collection.AddNew("Created by import", "");
            collection.Refresh();
            newConnector.Stereotype = connectorName;
            newConnector.Direction = "Source -> Destination";
            newConnector.ClientID = sourceElement.ElementID;
            newConnector.SupplierID = targetElement.ElementID;
            newConnector.Update();
            sourceElement.Locked = false;
            sourceElement.Update();
            sourceElement.Locked = true;
            targetElement.Locked = false;
            targetElement.Update();
            targetElement.Locked = true;
        }

        public EA.Diagram createDiagramm(EA.Package package, String name)
        {
            diagramObjectCounter = 1;
            EA.Collection diagramCollection = package.Diagrams;
            EA.Diagram diagram = diagramCollection.AddNew(name, "Package");
            diagram.Update();
            diagramCollection.Refresh();
            EA.Collection elementCollection = package.Elements;
            EA.Element element = elementCollection.AddNew("Legend", "Text");

            element.Notes = "<b>Legend</b>&nbsp;<ul><li><font color=\"blue\"><b>New Requirement</b></font></li>" +
               "<li><font color=\"red\"><b>Updated Requirement</b></font></li>" +
               "<li><font color=\"yellow\"><b>Deleted Requirement</b></font></li></ul>";
            element.Update();
            EA.Collection diagramObjectCollection = diagram.DiagramObjects;
            EA.DiagramObject diagramObject = diagramObjectCollection.AddNew("", "");
            diagramObject.ElementID = element.ElementID;
            diagramObject.top = -10;
            diagramObject.bottom = -30;
            diagramObject.left = 20;
            diagramObject.right = 180;
            diagramObject.Update();
            diagram.Update();
            return diagram;
        }

        public void createDiagramObject(EA.Diagram diagram, EA.Element element, int color)
        {
            EA.Collection c = diagram.DiagramObjects;
            EA.DiagramObject diagramObject = c.AddNew("", "");
            diagramObjectCounter += 1;
            diagramObject.ElementID = element.ElementID;
            diagramObject.top = 30 - (diagramObjectCounter * 60);
            diagramObject.bottom = diagramObject.top - 30;
            diagramObject.left = 20;
            diagramObject.right = 180;
            if (color != 0)
                diagramObject.BackgroundColor = color;
            diagramObject.Update();
            c.Refresh();
        }

        public EA.Element createElement(IEACItem ieacItem, EA.Diagram diagram)
        {
            EA.Collection collection = null;
            EA.Element parentElement = null;
            EA.Package parentPackage = getPackageByILMId(ieacItem.getContainedBy());

            if (parentPackage == null) // Requirement below Heading
            {
                parentElement = getElementByILMId(ieacItem.getContainedBy());
                parentElement.Locked = false;
                parentElement.Stereotype = "Heading";
                parentElement.Update();
                collection = parentElement.Elements;
            }
            else  // Requirement below Requirement
            {
                collection = parentPackage.Elements;
            }

            String name = getName(ieacItem.getAttributeList());
            if (name.Equals(""))
                name = "Unnamed Element";
            String notes = getNotes(ieacItem.getAttributeList());
            String stereotype = getStereotype(ieacItem.getAttributeList());
            String alias = getStereotype(ieacItem.getAttributeList());
            String keywords = getStereotype(ieacItem.getAttributeList());
            //EA.Collection collection = parentPackage.Elements;
            EA.Element newElement = collection.AddNew(getMaxStringLength(name), ieacItem.getCategory());
            newElement.Notes = notes;
            newElement.Stereotype = stereotype;
            newElement.SetAppearance(1, 0, 16770508);
            newElement.Update();
            setIntegrityMetaData(newElement, ieacItem);
            foreach (IEACAttribute attribute in ieacItem.getAttributeList())
            {
                if (!(attribute.getFieldMapping().getEAName().Equals("Name") ||
                   attribute.getFieldMapping().getEAName().Equals("Notes")
                   || attribute.getFieldMapping().getEAName().Equals("Stereotype")))
                {
                    if (!(attribute.getFieldMapping().getILMType().Equals("attachment") || attribute.getFieldMapping().getILMType().Equals("relationship")))
                        setTaggedValue(newElement, "", attribute.getFieldMapping().getEAName(), attribute.getValue());
                }
            }
            setTaggedValue(newElement, "EA::", "Name changed", "true");
            setTaggedValue(newElement, "EA::", "Notes changed", "true");
            setTaggedValue(newElement, "EA::", "Stereotype changed", "true");
            newElement.Update();
            this.elementDictionary.Add(ieacItem.getId(), newElement);
            createDiagramObject(diagram, newElement, 0);
            newElement.Locked = true;
            if (parentElement != null)
                parentElement.Locked = true;
            return newElement;
        }

        public EA.Package createPackage(IEACItem ieacItem)
        {
            EA.Package parentPackage = null;
            if (ieacItem.getContainedBy().Length == 0)    // Document item
                parentPackage = rootPackage;
            else
                parentPackage = getPackageByILMId(ieacItem.getContainedBy());
            String name = getName(ieacItem.getAttributeList());
            if (name.Equals(""))
                name = "Unnamed Package";
            String notes = getNotes(ieacItem.getAttributeList());
            EA.Collection collection = parentPackage.Packages;
            EA.Package newPackage = collection.AddNew(name, "Package"); // No creation without name
            newPackage.Notes = notes;
            newPackage.Update();
            setIntegrityMetaData(newPackage.Element, ieacItem);
            foreach (IEACAttribute attribute in ieacItem.getAttributeList())
            {
                if (!(attribute.getFieldMapping().getEAName().Equals("Name") || attribute.getFieldMapping().getEAName().Equals("Notes")))
                {
                    if (!(attribute.getFieldMapping().getILMType().Equals("attachment") || attribute.getFieldMapping().getILMType().Equals("relationship")))
                        setTaggedValue(newPackage.Element, "", attribute.getFieldMapping().getEAName(), attribute.getValue());
                }
            }
            setTaggedValue(newPackage.Element, "EA::", "Name changed", "true");
            setTaggedValue(newPackage.Element, "EA::", "Notes changed", "true");
            this.elementDictionary.Add(ieacItem.getId(), newPackage.Element);
            newPackage.Update();
            newPackage.Element.Locked = true;
            return newPackage;
        }

        public List<IEACItem> getDocument(EA.Element rootElement, List<IEACFieldMapping> fieldMappingList)
        {
            String documentId = rootElement.ElementGUID;
            List<IEACItem> ieacItemList = new List<IEACItem>();
            IEACItem ieacItem = new IEACItem(documentId, documentId);
            ieacItem.setMandatoryFieldValue("Type", rootElement.Type);
            ieacItem.getAttributeList().AddRange(getIEACAttributeList(rootElement, fieldMappingList, "document"));
            ieacItemList.Add(ieacItem);
            getElements(rootElement, ieacItem, documentId, ieacItemList, fieldMappingList);
            return ieacItemList;
        }

        private void getElements(EA.Element element, IEACItem ieacItem, String documentId, List<IEACItem> ieacItemList, List<IEACFieldMapping> fieldMappingList)
        {
            if (element.Type.Equals("Package"))
            {
                EA.Package package = rep.GetPackageByGuid(element.ElementGUID);
                foreach (EA.Package subPackage in package.Packages)
                {
                    IEACItem subIEACItem = new IEACItem(subPackage.PackageGUID, documentId);
                    subIEACItem.setMandatoryFieldValue("Type", subPackage.Element.Type);
                    subIEACItem.setMandatoryFieldValue("Contained By", ieacItem.getId());
                    subIEACItem.getAttributeList().AddRange(getIEACAttributeList(subPackage.Element, fieldMappingList, "content"));
                    ieacItemList.Add(subIEACItem);
                    // Rekursion
                    getElements(subPackage.Element, subIEACItem, documentId, ieacItemList, fieldMappingList);
                }
                foreach (EA.Element subElement in package.Elements)
                {
                    IEACItem subIEACItem = new IEACItem(subElement.ElementGUID, documentId);
                    subIEACItem.setMandatoryFieldValue("Type", subElement.Type);
                    subIEACItem.setMandatoryFieldValue("Contained By", ieacItem.getId());
                    subIEACItem.getAttributeList().AddRange(getIEACAttributeList(subElement, fieldMappingList, "content"));
                    ieacItemList.Add(subIEACItem);
                    // Rekursion
                    getElements(subElement, subIEACItem, documentId, ieacItemList, fieldMappingList);
                }
            }
            else
            {
                foreach (EA.Element subElement in element.Elements)
                {
                    ieacItem.getAttributeByILMName("Category").setValue("Heading");

                    IEACItem subIEACItem = new IEACItem(subElement.ElementGUID, documentId);
                    subIEACItem.setMandatoryFieldValue("Type", subElement.Type);
                    subIEACItem.setMandatoryFieldValue("Contained By", ieacItem.getId());
                    subIEACItem.getAttributeList().AddRange(getIEACAttributeList(subElement, fieldMappingList, "content"));
                    ieacItemList.Add(subIEACItem);
                    // Rekursion
                    getElements(subElement, subIEACItem, documentId, ieacItemList, fieldMappingList);
                }
            }
        }

        public EA.Element getElementByEAId(int id)
        {
            return rep.GetElementByID(id);
        }

        public EA.Element getElementByILMId(String id)
        {
            EA.Element element = null;
            if (elementDictionary.TryGetValue(id, out element))
                return element;
            else
                return null;
        }

        private List<IEACAttribute> getIEACAttributeList(EA.Element element, List<IEACFieldMapping> fieldMappingList, String context)
        {
            List<IEACAttribute> ieacAttributeList = new List<IEACAttribute>();
            foreach (IEACFieldMapping fieldMapping in fieldMappingList)
            {
                String value = null;
                if (fieldMapping.getContext().Equals(context))
                {
                    if (fieldMapping.getEAName().Equals("Name"))
                    {
                        value = element.Name;
                        if (value.Length == 0)
                            value = null;
                    }
                    else if (fieldMapping.getEAName().Equals("Notes"))
                    {
                        value = element.Notes;
                        if (value.Length == 0)
                            value = null;
                    }
                    else if (fieldMapping.getEAName().Equals("Stereotype"))
                    {
                        value = element.Stereotype;
                        if (value.Length == 0)
                            value = null;
                    }
                    else if (fieldMapping.getEAName().Equals("Type"))
                    {
                        value = element.Type;
                        if (value.Length == 0)
                            value = null;
                    }
                    else
                        value = getTaggedValue(element, "", fieldMapping.getEAName());
                    if (value == null && fieldMapping.getDefaultValue() != null)
                        value = fieldMapping.getDefaultValue();

                    if (value != null && checkConditions(element, fieldMapping.getConditionList()))
                    {
                        IEACAttribute attribute = new IEACAttribute(value, fieldMapping);
                        ieacAttributeList.Add(attribute);
                    }
                }
            }
            return ieacAttributeList;
        }

        private String getMaxStringLength(String s)
        {
            if (s.Length > 255)
                return s.Substring(0, 252) + "...";
            else
                return s;
        }

        private string getName(List<IEACAttribute> attributeList)
        {
            String name = "";
            foreach (IEACAttribute attribute in attributeList)
            {
                if (attribute.getFieldMapping().getEAName().Equals("Name"))
                {
                    String value = attribute.getValue();
                    value = Regex.Replace(value, "<[^>]*>", "");
                    value = value.Replace("&#160;", " ");
                    if (attribute.getFieldMapping().getAppend() != null)
                        name = name + attribute.getFieldMapping().getAppend() + value;
                    else
                        name = value;
                }
            }
            return name;
        }

        private string getNotes(List<IEACAttribute> attributeList)
        {
            String notes = "";
            foreach (IEACAttribute attribute in attributeList)
            {
                if (attribute.getFieldMapping().getEAName().Equals("Notes"))
                {
                    String value = attribute.getValue();
                    if (attribute.getFieldMapping().isRichContent())
                    {
                        value = value.Replace("<!-- MKS HTML -->", "");
                        value = value.Replace("<div>", "");
                        value = value.Replace("</div>", "");
                        notes = value;
                    }
                    else
                    {
                        if (attribute.getFieldMapping().getAppend() != null)
                            notes = notes + attribute.getFieldMapping().getAppend() + value;
                        else
                            notes = value;
                    }
                }
            }
            return notes;
        }

        /*
        public EA.Package getPackageByEAId(String eaId)
        {
           int id = Int32.Parse(eaId);
           return rep.GetPackageByID(id);
        }
        */

        public EA.Package getPackageByEAId(int id)
        {
            return rep.GetPackageByID(id);
        }

        public EA.Package getPackageByILMId(String id)
        {
            EA.Element element = this.getElementByILMId(id);
            if (element != null)
            {
                if (element.Type.Equals("Package"))
                    return rep.GetPackageByGuid(element.ElementGUID);
                else
                    return null;
            }
            else
            {
                return null;
            }
        }

        public EA.Package getPackageByGUID(String id)
        {
            return rep.GetPackageByGuid(id);
        }

        public EA.Package getPackageByName(EA.Package parentPackage, String name)
        {
            EA.Collection c = parentPackage.Packages;
            foreach (EA.Package p in c)
            {
                if (p.Name.Equals(name))
                    return p;
            }
            return null;
        }

        public EA.Package getPackageByTaggedValue(EA.Package package, String prefix, String name)
        {
            String value = getTaggedValue(package.Element, prefix, name);
            if (value == null)
            {
                EA.Collection childPackages = package.Packages;
                if (childPackages.Count > 0)
                {
                    foreach (EA.Package childPackage in childPackages)
                    {
                        if (getPackageByTaggedValue(childPackage, prefix, name) != null)
                            return getPackageByTaggedValue(childPackage, prefix, name);
                    }
                    return null;
                }
                else
                    return null;
            }
            else
                return package;
        }

        public EA.Repository getRepository()
        {
            return rep;
        }

        public List<EA.Element> getRequirementList(string documentId, EA.Package trashPackage)
        {
            List<EA.Element> l = new List<EA.Element>();
            foreach (EA.Element e in rep.GetElementSet("", 0))
            {
                if (trashPackage != null)
                {
                    if (trashPackage.PackageID != e.PackageID)
                    {
                        if (getTaggedValue(e, "ILM::", "Document ID") != null)
                        {
                            if ((e.Type.Equals("Requirement") || e.Type.Equals("UseCase")) && getTaggedValue(e, "ILM::", "Document ID").Equals(documentId))
                                l.Add(e);
                        }
                    }
                }
                else
                {
                    if (getTaggedValue(e, "ILM::", "Document ID") != null)
                    {
                        if ((e.Type.Equals("Requirement") || e.Type.Equals("UseCase")) && getTaggedValue(e, "ILM::", "Document ID").Equals(documentId))
                            l.Add(e);
                    }
                }
            }
            return l;
        }

        public EA.Package getRootPackage()
        {
            return this.rootPackage;
        }

        private string getStereotype(List<IEACAttribute> attributeList)
        {
            String type = "";
            foreach (IEACAttribute attribute in attributeList)
            {
                if (attribute.getFieldMapping().getEAName().Equals("Stereotype"))
                    type = attribute.getValue();
            }
            return type;
        }

        public String getTaggedValue(EA.Element element, string prefix, string name)
        {
            String value = null;
            EA.Collection c = element.TaggedValues;
            foreach (EA.TaggedValue t in c)
                if (t.Name.Equals(prefix + name))
                    value = t.Value;
            //Compatibility to Version 1.1
            if (value != null)
                return value;
            else
            {
                foreach (EA.TaggedValue t in c)
                    if (t.Name.Equals("Integrity " + name))
                        value = t.Value;
                return value;
            }
        }

        public String getTaggedValue(EA.Connector connector, string prefix, string name)
        {
            String value = null;
            EA.Collection c = connector.TaggedValues;
            foreach (EA.ConnectorTag t in c)
            {
                if (t.Name.Equals(prefix + name))
                {
                    value = t.Value;
                }
            }
            return value;
        }

        private void initElementDictionary()
        {
            elementDictionary.Clear();
            foreach (EA.Element e in rep.GetElementSet("", 0))
            {
                if (allowedTypes.Contains(e.Type))
                {
                    String id = getTaggedValue(e, "ILM::", "ID");
                    if (id != null)
                        elementDictionary.Add(id, e);
                }
            }
        }

        public void removeEntryFromElementDictionary(EA.Element element)
        {
            elementDictionary.Remove(getTaggedValue(element, "ILM::", "ID"));
        }

        public void setDiagramList()
        {
            String xmlQueryResult = rep.SQLQuery("select * from t_diagram dobj1;");
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlQueryResult);
            XmlNodeList xnList = xml.SelectNodes("/EADATA/Dataset_0/Data/Row");
            foreach (XmlNode xn in xnList)
                diagramList.Add(rep.GetDiagramByID(Convert.ToInt32(xn["Diagram_ID"].InnerText)));
        }

        public List<EA.Diagram> getDiagramList(EA.Package package)
        {
            diagramList.Clear();
            setDiagramList2(package);
            return diagramList;
        }

        public void setDiagramList(EA.Package package)
        {
            foreach (EA.Diagram diagram in package.Diagrams)
            {
                diagramList.Add(diagram);
            }
            if (package.Packages.Count > 0)
            {
                foreach (EA.Package childPackage in package.Packages)
                    setDiagramList(childPackage);
            }
        }

        public void setDiagramList2(EA.Package package)
        {
            String xmlQueryResult = rep.SQLQuery("select * from t_diagram where Package_ID=" + package.PackageID);
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlQueryResult);
            XmlNodeList xnList = xml.SelectNodes("/EADATA/Dataset_0/Data/Row");
            foreach (XmlNode xn in xnList)
                diagramList.Add(rep.GetDiagramByID(Convert.ToInt32(xn["Diagram_ID"].InnerText)));
            if (package.Packages.Count > 0)
            {
                foreach (EA.Package childPackage in package.Packages)
                    setDiagramList2(childPackage);
            }
        }

        public void setIntegrityMetaData(EA.Element element, IEACItem ieacItem)
        {
            String oldValue = getTaggedValue(element, "ILM::", "ID");
            if (!ieacItem.getId().Equals(oldValue))
                setTaggedValue(element, "ILM::", "ID", ieacItem.getId());
            oldValue = getTaggedValue(element, "ILM::", "Document ID");
            if (!ieacItem.getDocumentId().Equals(oldValue))
                setTaggedValue(element, "ILM::", "Document ID", ieacItem.getDocumentId());
            oldValue = getTaggedValue(element, "ILM::", "Type");
            if (!ieacItem.getType().Equals(oldValue))
                setTaggedValue(element, "ILM::", "Type", ieacItem.getType());
            if (ieacItem.getContains() != null)
            {
                oldValue = getTaggedValue(element, "ILM::", "Contains");
                if (!ieacItem.getContains().Equals(oldValue))
                    setTaggedValue(element, "ILM::", "Contains", ieacItem.getContains());
            }
            if (ieacItem.getContainedBy() != null)
            {
                oldValue = getTaggedValue(element, "ILM::", "Contained By");
                if (!ieacItem.getContainedBy().Equals(oldValue))
                    setTaggedValue(element, "ILM::", "Contained By", ieacItem.getContainedBy());
            }
            if (ieacItem.getDocumentShortTitle() != null)
            {
                oldValue = getTaggedValue(element, "ILM::", "Document Short Title");
                if (!ieacItem.getDocumentShortTitle().Equals(oldValue))
                    setTaggedValue(element, "ILM::", "Document Short Title", ieacItem.getDocumentShortTitle());
            }
            if (ieacItem.getCategory() != null)
            {
                oldValue = getTaggedValue(element, "ILM::", "Category");
                if (!ieacItem.getCategory().Equals(oldValue))
                    setTaggedValue(element, "ILM::", "Categrory", ieacItem.getCategory());
            }
            if (ieacItem.getCreatedBy() != null)
            {
                oldValue = getTaggedValue(element, "ILM::", "Created By");
                if (!ieacItem.getCreatedBy().Equals(oldValue))
                    setTaggedValue(element, "ILM::", "Created By", ieacItem.getCreatedBy());
            }
            if (ieacItem.getCreatedDate() != null)
            {
                oldValue = getTaggedValue(element, "ILM::", "Created Date");
                if (!ieacItem.getCreatedDate().Equals(oldValue))
                    setTaggedValue(element, "ILM::", "Created Date", ieacItem.getCreatedDate());
            }
            if (ieacItem.getModifiedBy() != null)
            {
                oldValue = getTaggedValue(element, "ILM::", "Modified By");
                if (!ieacItem.getModifiedBy().Equals(oldValue))
                    setTaggedValue(element, "ILM::", "Modified By", ieacItem.getModifiedBy());
            }
            if (ieacItem.getModifiedDate() != null)
            {
                oldValue = getTaggedValue(element, "ILM::", "Modified Date");
                if (!ieacItem.getModifiedDate().Equals(oldValue))
                    setTaggedValue(element, "ILM::", "Modified Date", ieacItem.getModifiedDate());
            }
        }

        public void setRootPackage(EA.Package rootPackage)
        {
            this.rootPackage = rootPackage;
        }

        public void setTaggedValue(EA.Element element, string prefix, string name, string value)
        {
            Boolean exists = false;
            EA.Collection c = element.TaggedValues;
            foreach (EA.TaggedValue t in c)
            {
                if (t.Name.Equals(prefix + name))
                {
                    exists = true;
                    if (value.Length > 255)
                    {
                        if (t.Notes.Equals(value))
                        {
                            t.Value = "<Memo>";
                            t.Notes = value;
                            t.Update();
                        }
                    }
                    else
                    {
                        if (!t.Value.Equals(value))
                        {
                            t.Value = value;
                            t.Update();
                        }
                    }
                }
            }
            if (exists == false)
            {
                EA.TaggedValue taggedValue = null;
                if (value.Length > 255)
                {
                    taggedValue = c.AddNew(prefix + name, "dummy");
                    taggedValue.Value = "<Memo>";
                    taggedValue.Notes = value;
                }
                else
                {
                    taggedValue = c.AddNew(prefix + name, value);
                }
                taggedValue.Update();
            }
        }

        public void setTaggedValue(EA.Connector connector, string prefix, string name, string value)
        {
            Boolean exists = false;
            EA.Collection c = connector.TaggedValues;
            foreach (EA.ConnectorTag t in c)
            {
                // Check if relevant TaggedValue (prefix::name) exists e.g. PLM::id
                if (t.Name.Equals(prefix + name))
                {
                    exists = true;
                    if (value.Length > 255)
                    {
                        if (t.Notes.Equals(value))
                        {
                            t.Value = "<Memo>";
                            t.Notes = value;
                            t.Update();
                        }
                    }
                    else
                    {
                        if (!t.Value.Equals(value))
                        {
                            t.Value = value;
                            t.Update();
                        }
                    }
                }
            }
            // No TaggedValue with relevant prefix and name found
            // Create new value
            if (exists == false)
            {
                EA.ConnectorTag taggedValue = null;
                if (value.Length > 255)
                {
                    taggedValue = c.AddNew(prefix + name, "dummy");
                    taggedValue.Value = "<Memo>";
                    taggedValue.Notes = value;
                }
                else
                {
                    taggedValue = c.AddNew(prefix + name, value);
                }
                taggedValue.Update();
            }
        }

        public void deleteTaggedValue(EA.Element element, String prefix, String name)
        {
            EA.Collection c = element.TaggedValues;
            short index = 0;
            foreach (EA.TaggedValue t in c)
            {
                if (t.Name.Equals(prefix + name))
                    break;
                index += 1;
            }
            c.Delete(index);
            element.Update();
        }

        public Boolean updateElement(EA.Element element, IEACItem ieacItem, EA.Diagram diagram)
        {
            Boolean changed = false;

            String newName = getName(ieacItem.getAttributeList());
            if (!element.Name.Equals(newName))
            {
                element.Locked = false;
                element.Name = getMaxStringLength(newName);
                setTaggedValue(element, "EA::", "Name changed", "true");
                changed = true;
            }
            String newNotes = getNotes(ieacItem.getAttributeList());
            if (!element.Notes.Equals(newNotes))
            {
                element.Locked = false;
                element.Notes = newNotes;
                setTaggedValue(element, "EA::", "Notes changed", "true");
                changed = true;
            }
            String newStereotype = getStereotype(ieacItem.getAttributeList());
            if (!element.Stereotype.Equals(newStereotype))
            {
                element.Locked = false;
                element.Stereotype = newStereotype;
                setTaggedValue(element, "EA::", "Stereotype changed", "true");
                changed = true;
            }
            foreach (IEACAttribute attribute in ieacItem.getAttributeList())
            {
                if (!(attribute.getFieldMapping().getEAName().Equals("Name") ||
                   attribute.getFieldMapping().getEAName().Equals("Notes") ||
                   attribute.getFieldMapping().getEAName().Equals("Stereotype")))
                {
                    if (!(attribute.getFieldMapping().getILMType().Equals("attachment") || attribute.getFieldMapping().getILMType().Equals("relationship")))
                    {
                        String oldValue = getTaggedValue(element, "", attribute.getFieldMapping().getEAName());
                        if (oldValue == null)
                        {
                            element.Locked = false;
                            setTaggedValue(element, "", attribute.getFieldMapping().getEAName(), attribute.getValue());
                            changed = true;
                        }
                        else
                        {
                            if (!oldValue.Equals(attribute.getValue()))
                            {
                                element.Locked = false;
                                setTaggedValue(element, "", attribute.getFieldMapping().getEAName(), attribute.getValue());
                                changed = true;
                            }
                        }
                    }
                }
            }
            if (changed)
            {
                setIntegrityMetaData(element, ieacItem);
                element.SetAppearance(1, 0, 13421823);
                element.Update();
                element.Locked = true;
                createDiagramObject(diagram, element, 0);
            }
            return changed;
        }

    }
}