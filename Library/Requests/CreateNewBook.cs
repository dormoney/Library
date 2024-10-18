using System.ComponentModel.DataAnnotations;

namespace Library.Requests
{
    public class CreateNewBook
    {
        [Required]
        public string Title { get; set; }
        public string Author { get; set; }
        public int Year_public { get; set; }
        public string Description { get; set; }
        [Required]
        public int Copies { get; set; }
        [Required]
        public int Id_genre { get; set; }
    }
}
