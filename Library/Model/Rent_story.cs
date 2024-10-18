using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Library.Model;

namespace Library.Model
{
    public class Rent_story
    {
        [Key]
        public int Id_rent { get; set; }
        [Required]
        [ForeignKey("Books")]
        public int Id_book { get; set; }
        [Required]
        [ForeignKey("Readers")]
        public int Id_reader { get; set; }
        [Required]
        public DateTime RentalDate { get; set; }
        [Required]
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public Books Books { get; set; }
        public Readers Readers { get; set; }
    }
}
