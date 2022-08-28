using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace NetCoreSWebApp.Models
{
    public partial class ActorName
    {
        [Required, StringLength(12)]
        public string Id { get; set; } 

        [Required, StringLength(128)] 
        public string Name { get; set; }

        public ActorName()
        {
            Id = "";
            Name = "";
        }
    }
}
