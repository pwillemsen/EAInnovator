using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace EAInnovator
{
    public class IEACFieldMapping
    {
        private String ilm = null;
        private String ea = null;
        private String ilmType = null;
        private String eaType = null;
        private String context = null;
        private List<IEACCondition> conditionList = new List<IEACCondition>();
        private String convertType = null;
        private Boolean richContent = false;
        private String append = null;
        private Boolean multiValue = false;
        private String attachmentField = null;
        private String defaultValue = null;
        private Dictionary<string, string> valueDic = new Dictionary<string, string>();

        public void setILMName(String ilmName)
        {
            ilm = ilmName;
        }

        public void setEAName(String eaName)
        {
            ea = eaName;
        }

        public void setConditionList(String conditions)
        {
            if (conditions != null)
            {
                List<String> cList = conditions.Split(',').ToList();
                foreach (String c in cList)
                {
                    conditionList.Add(new IEACCondition(c));
                }
            }
        }

        public void setConvertType(String convertType)
        {
            this.convertType = convertType;
        }

        public String getILMName()
        {
            return ilm;
        }

        public String getEAName()
        {
            return ea;
        }

        public List<IEACCondition> getConditionList()
        {
            return conditionList;
        }

        public String getConvertType()
        {
            return convertType;
        }

        public void setRichContent(Boolean richContent)
        {
            this.richContent = richContent;
        }

        public Boolean isRichContent()
        {
            return richContent;
        }

        public void setAppend(String value)
        {
            this.append = value;
        }

        public String getAppend()
        {
            return this.append;
        }

        public String getContext()
        {
            return context;
        }

        public void setContext(String context)
        {
            this.context = context;
        }

        public void setILMType(String ilmType)
        {
            this.ilmType = ilmType;
        }

        public String getILMType()
        {
            return ilmType;
        }

        public void setEAType(String eaType)
        {
            this.eaType = eaType;
        }

        public String getEAType()
        {
            return eaType;
        }

        public void setMultiValue(Boolean multiValue)
        {
            this.multiValue = multiValue;
        }

        public Boolean isMultiValue()
        {
            return multiValue;
        }

        public void setAttachmentField(String attachmentField)
        {
            this.attachmentField = attachmentField;
        }

        public String getAttachmentField()
        {
            return attachmentField;
        }

        public String getDefaultValue()
        {
            return this.defaultValue;
        }

        public void setDefaultValue(String defaultValue)
        {
            this.defaultValue = defaultValue;
        }

        public void setValueDic(XmlNodeList nodeList, string mode)
        {
            foreach (XmlNode n in nodeList)
            {
                string ilm = null;
                string ea = null;
                if (n.Attributes["ILM"] != null)
                    ilm = n.Attributes["ILM"].Value;
                if (n.Attributes["EA"] != null)
                    ilm = n.Attributes["EA"].Value;
                if (ea == null)
                    ea = ilm;
                if (mode.Equals("import"))
                    valueDic.Add(ilm, ea);
                else
                    valueDic.Add(ea, ilm);
            }
        }

        public Dictionary<string, string> getValueDic()
        {
            return valueDic;
        }
    }
}