using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Team7ADProjectMVC.Exceptions
{
    public class RequisitionAndPOCreationException : Exception
    {
        public RequisitionAndPOCreationException()
        {
        }

        public RequisitionAndPOCreationException(string message)
        : base(message)
        {
        }
    }
}