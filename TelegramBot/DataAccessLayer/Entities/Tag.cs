using System.Collections.Generic;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.Entities
{
    [Table("Tag")]
    public sealed class Tag
    {
        [Key]
        [Column(TypeName = "integer")]
        public int TagId { get; set; }

        [Required]
        [Column(TypeName = "text")]
        public string TagName { get; set; }

        public ICollection<CourceTagRecord> CourceTagRecords;

        public ICollection<VacancyTagRecord> VacancyTagRecords;

        public Tag()
        {
            CourceTagRecords = new List<CourceTagRecord>();
            VacancyTagRecords = new List<VacancyTagRecord>();
        }
    }
}