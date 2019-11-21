using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmailDaemon.DataTypes
{
    class Email
    {

        [Required]
        public string Id { get; set; }
        public string User { get; set; }
        public string Sender { get; set; }
        public string Subject { get; set; }
        public DateTimeOffset DateRetrieved { get; set; }
        public string Body { get; set; }
        public Job job { get; set; }

    }
}
