using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace NetCoreSWebApp.Models
{
    public partial class ActorFilm
    {
        [Required, StringLength(12)]
        public string FilmId { get; set; }

        [Required, StringLength(12)] 
        public string ActorId { get; set; }
        
        [Required, StringLength(256)]
        public string Postac { get; set; } 

        public ActorFilm()
        {
            FilmId = "";
            ActorId = "";
            Postac = "";
        }
}

}
