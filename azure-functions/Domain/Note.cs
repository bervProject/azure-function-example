using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace azure_functions.Domain
{
    public class Note
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string CreatedBy { get; set; }
    }
}
