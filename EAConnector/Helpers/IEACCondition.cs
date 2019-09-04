using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EAInnovator
{
    public class IEACCondition
    {
        private String variable = null;
        private String comparator = "unknown comparator";
        private String value = null;

        public IEACCondition(String conditionString)
        {
            String[] sa = null;
            if (conditionString.IndexOf("==") > 0)
            {
                comparator = "==";
                sa = conditionString.Split(new string[] { "==" }, StringSplitOptions.None);
            }
            else if (conditionString.IndexOf("!=") > 0)
            {
                sa = conditionString.Split(new string[] { "!=" }, StringSplitOptions.None);
                comparator = "!=";
            }
            else if (conditionString.IndexOf("contains") > 0)
            {
                sa = conditionString.Split(new string[] { "contains" }, StringSplitOptions.None);
                comparator = "contains";
            }
            variable = sa[0].Trim();
            value = sa[1].Trim();
        }

        public String toString()
        {
            return variable + " " + comparator + " " + value;
        }

        public String getVariable()
        {
            return variable;
        }

        public String getComparator()
        {
            return comparator;
        }

        public String getValue()
        {
            return value;
        }
    }
}
