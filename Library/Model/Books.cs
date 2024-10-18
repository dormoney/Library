using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Model
{
    public class Books
    {
        [Key]
        public int Id_book { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public int Year_public { get; set; }
        public string Description { get; set; }
        public int Copies { get; set; }
        [Required]
        [ForeignKey("Genres")]
        public int Id_genre { get; set; }
        public Genres Genres { get; set; }
    }
}
