using System.ComponentModel.DataAnnotations;

namespace Library.Model
{
    public class Genres
    {
        [Key]
        public int Id_genre { get; set; }
        public string Name_genre { get; set; }
    }
}
