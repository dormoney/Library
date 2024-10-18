using System.ComponentModel.DataAnnotations;

namespace Library.Model
{
    public class Readers
    {
        [Key]
        public int Id_reader { get; set; }
        public string First_name { get; set; }
        public string Last_name { get; set;}
        public int Birth_year { get; set;}
        public string Contact_info { get; set; }
    }
}
