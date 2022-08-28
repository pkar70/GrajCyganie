using System;
using System.Collections.Generic;

namespace NetCoreSWebApp.Models
{
    public class vAktorzyFilmu
    {
        public string Id { get; set; } = null!;
        public string ActorId { get; set; } = null!;
        public string FilmId { get; set; } = null!;
        public string Postac { get; set; } = null!;
        public string Name { get; set; } = null!;
    }
}
