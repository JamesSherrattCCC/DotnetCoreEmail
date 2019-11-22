using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace EmailDaemon.DataTypes
{
    public class Job
    {
        [Key]
        public int Id { get; }
        public string JobName { get; set; }
        public string Department { get; set; }
        public IEnumerable<Email> Emails { get; set; }
    }
}
