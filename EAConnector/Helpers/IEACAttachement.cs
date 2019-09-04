using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EAInnovator
{
    public class IEACAttachment
    {
        private String name = null;

        public IEACAttachment(String name)
        {
            this.name = name;
        }

        public String getName()
        {
            return name;
        }
    }
}