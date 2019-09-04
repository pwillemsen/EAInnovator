using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EAInnovator
{
    public class IEACItem
    {
        private String id = null;
        public void setId(String id) { this.id = id; }
        public String getId() { return id; }
        private String documentId = null;
        public String getDocumentId() { return documentId; }
        private String type = null;
        public String getType() { return this.type; }
        private String category = null;
        public String getCategory() { return this.category; }
        private String documentShortTitle = null;
        public String getDocumentShortTitle() { return this.documentShortTitle; }
        private String contains = null;
        public String getContains() { return this.contains; }
        private String containedBy = null;
        public String getContainedBy() { return this.containedBy; }
        private String modifiedBy = null;
        public String getModifiedBy() { return this.modifiedBy; }
        private String modifiedDate = null;
        public String getModifiedDate() { return this.modifiedDate; }
        private String createdBy = null;
        public String getCreatedBy() { return this.createdBy; }
        private String createdDate = null;
        public String getCreatedDate() { return this.createdDate; }
        private List<IEACAttribute> attributeList = new List<IEACAttribute>();
        public void addAttribute(IEACAttribute attribute) { this.attributeList.Add(attribute); }
        public List<IEACAttribute> getAttributeList() { return this.attributeList; }
        private Boolean exportCondition = true;
        public void setExportCondition(Boolean exportCondition) { this.exportCondition = exportCondition; }
        public Boolean isExportCondition() { return this.exportCondition; }

        public IEACItem(String id, String documentId)
        {
            this.id = id;
            this.documentId = documentId;
        }

        public void setMandatoryFieldValue(string fieldName, string fieldValue)
        {
            if (fieldName.Equals("Type"))
                this.type = fieldValue;
            else if (fieldName.Equals("Contains"))
                this.contains = fieldValue;
            else if (fieldName.Equals("Contained By"))
                this.containedBy = fieldValue;
            else if (fieldName.Equals("Document Short Title"))
                this.documentShortTitle = fieldValue;
            else if (fieldName.Equals("Category"))
            {
                if (fieldValue == null)
                    this.category = "Document";
                else
                    this.category = fieldValue;
            }
            else if (fieldName.Equals("Created By"))
                this.createdBy = fieldValue;
            else if (fieldName.Equals("Created Date"))
                this.createdDate = fieldValue;
            else if (fieldName.Equals("Modified By"))
                this.modifiedBy = fieldValue;
            else if (fieldName.Equals("Modified Date"))
                this.modifiedDate = fieldValue;
        }

        public IEACAttribute getAttributeByILMName(String attributeName)
        {
            foreach (IEACAttribute attribute in attributeList)
            {
                if (attribute.getFieldMapping().getILMName().Equals(attributeName))
                    return attribute;
            }
            return null;
        }

    }
}
