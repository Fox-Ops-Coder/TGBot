using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.Entities
{
    [Table("Profession")]
    public sealed class Profession
    {
        [Column(TypeName = "integer")]
        [Key]
        public int ProfessionId { get; set; }

        [Column(TypeName = "text")]
        [Required]
        public string ProfessionName { get; set; }

        [NotMapped]
        public ICollection<Cource> Cources { get; set; }

        [NotMapped]
        public ICollection<Vacancy> Vacancies { get; set; }

        public Profession()
        {
            Cources = new List<Cource>();
            Vacancies = new List<Vacancy>();
        }
    }
}