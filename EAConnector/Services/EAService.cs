using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Aras.IOM;

namespace EAInnovator.Services
{
    public sealed class EAService
    {
        private static volatile EAService instance;
        private static object syncRoot = new Object();
        public static EAService _
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new EAService();
                    }
                }

                return instance;
            }
        }
    }
}