using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrendsValley.Models.Models
{
    public class State
    {
        [Key]
        public int State_Id { get; set; }

        public string State_Name { get; set; }
    }
}
