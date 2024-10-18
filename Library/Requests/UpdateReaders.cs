using System.ComponentModel.DataAnnotations;

namespace Library.Requests
{
    public class UpdateReaders
    {
        [Required]
        public string First_name { get; set; }
        [Required]
        public string Last_name { get; set; }
        [Required]
        public int Birth_year { get; set; }
        [Required]
        public string Contact_info { get; set; }
    }
}
