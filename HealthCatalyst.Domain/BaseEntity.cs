using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HealthCatalyst.Domain
{
    public abstract class BaseEntity
    {        
        public int Id { get; set; }
    }
}