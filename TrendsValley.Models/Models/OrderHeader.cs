using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrendsValley.Models.Models
{
    public class OrderHeader
    {
        public int Id { get; set; }

        [ForeignKey("appUser")]
        public string AppUserId { get; set; }

        public AppUser appUser { get; set; }

        public double orderTotal { get; set; }

        public DateTime OrderDate { get; set; }

        public string? paymentStatus { get; set; }

        public string? orderStatus { get; set; }

        public string? trackingNumber { get; set; }

        [ForeignKey("city")]
        public int cityId { get; set; }

        public City city { get; set; }

        [ForeignKey("state")]
        public int stateId { get; set; }

        public State state { get; set; }
        public DateTime paymentDate { get; set; }

        public DateOnly paymentDueDate { get; set; }

        public string? sessionId { get; set; }

        public string? paymentIntentId { get; set; }

    }
}
