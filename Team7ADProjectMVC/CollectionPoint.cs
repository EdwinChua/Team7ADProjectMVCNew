
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


namespace Team7ADProjectMVC
{

using System;
    using System.Collections.Generic;
    
public partial class CollectionPoint
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public CollectionPoint()
    {

        this.Departments = new HashSet<Department>();

    }


    public int CollectionPointId { get; set; }

    public string PlaceName { get; set; }

    public Nullable<System.TimeSpan> CollectTime { get; set; }

    public Nullable<int> EmployeeId { get; set; }



    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<Department> Departments { get; set; }

    public virtual Employee Employee { get; set; }

}

}
