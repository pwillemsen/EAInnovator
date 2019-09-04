using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EAInnovator
{
    public class IEACAttribute
    {
        private String value = null;
        private IEACFieldMapping fieldMapping = null;
        private List<IEACAttachment> attachmentList = new List<IEACAttachment>();

        public IEACAttribute(String value, IEACFieldMapping fieldMapping)
        {
            this.value = value;
            this.fieldMapping = fieldMapping;
        }

        public void setAttachmentList(List<IEACAttachment> attachmentList)
        {
            this.attachmentList = attachmentList;
        }

        public void setValue(String value)
        {
            this.value = value;
        }

        public String getValue()
        {
            return value;
        }

        public IEACFieldMapping getFieldMapping()
        {
            return fieldMapping;
        }

        public List<IEACAttachment> getAttachmentList()
        {
            return attachmentList;
        }
    }
}
