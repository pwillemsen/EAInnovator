using System;
using System.Collections.Generic;
using System.Xml;

/// <summary>
/// Utilities for Sparx Systems Enterprise Architect connection
/// </summary>
namespace EAInnovator
{
    public class EAUtils
    {
        private EA.Repository rep = null;
        private List<string> allowedTypes = new List<string>(new string[] { "Package", "Requirement", "UseCase" });
        private List<EA.Diagram> diagramList = new List<EA.Diagram>();

        public EAUtils(EA.Repository rep)
        {
            /// <summary>
            /// Initialize EA active repository
            /// </summary>
            this.rep = rep;
            SetDiagramList();
        }

        public void SetDiagramList()
        {
            /// <summary>
            /// Store all EA diagrams in List 
            /// </summary>
            String xmlQueryResult = rep.SQLQuery("select * from t_diagram dobj1;");
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlQueryResult);
            XmlNodeList xnList = xml.SelectNodes("/EADATA/Dataset_0/Data/Row");
            foreach (XmlNode xn in xnList)
                diagramList.Add(rep.GetDiagramByID(Convert.ToInt32(xn["Diagram_ID"].InnerText)));
        }

        public EA.Package GetPackageByName(EA.Package parentPackage, String name)
        {
            /// <summary>
            /// Get EA Package element by its name 
            /// </summary>
            EA.Collection c = parentPackage.Packages;
            foreach (EA.Package p in c)
            {
                if (p.Name.Equals(name))
                    return p;
            }
            return null;
        }
    }
}