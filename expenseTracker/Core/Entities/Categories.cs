using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    public class Categories
    {
        public int Id { get; set; }
        public string Name { get; set; }
       public string UserId { get; set; }  // New property to associate with the user
    }

}
