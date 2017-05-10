using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HealthCatalyst.Domain.Data
{
    public class User : BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public int Age { get; set; }
        public string Interests { get; set; }
        public string PictureFile { get; set; }
    }
}