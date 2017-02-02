using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Team7ADProjectMVC.Exceptions
{
    public class InventoryAndDisbursementUpdateException : Exception
    {
        public InventoryAndDisbursementUpdateException()
        {
        }

        public InventoryAndDisbursementUpdateException(string message)
        : base(message)
        {
        }
    }
}