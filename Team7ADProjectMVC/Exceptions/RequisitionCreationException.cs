using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Team7ADProjectMVC.Exceptions
{
    public class RequisitionCreationException : Exception
    {
        public RequisitionCreationException()
        {
        }

        public RequisitionCreationException(string message)
        : base(message)
        {
        }
    }
}