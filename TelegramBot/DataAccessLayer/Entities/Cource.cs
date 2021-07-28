using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.Entities
{
    [Table("Cource")]
    public sealed class Cource
    {
        [Column(TypeName = "integer")]
        [Key]
        public int CourceId { get; set; }

        [Column(TypeName = "integer")]
        [Required]
        public int ProfessionId { get; set; }

        [Column(TypeName = "text")]
        [Required]
        public string CourceName { get; set; }

        [Column(TypeName = "text")]
        [Required]
        public string Url { get; set; }

        [NotMapped]
        public Profession Profession;

        [NotMapped]
        public ICollection<CourceTagRecord> CourceTagRecords;

        public Cource()
        {
            CourceTagRecords = new List<CourceTagRecord>();
        }
    }
}