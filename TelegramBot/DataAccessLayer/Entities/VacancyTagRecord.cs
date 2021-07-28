using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.Entities
{
    public sealed class VacancyTagRecord : TagRecordBase
    {
        [Column(TypeName = "integer")]
        public int VacancyId { get; set; }

        [NotMapped]
        public Vacancy Vacancy;

        public VacancyTagRecord()
        {
        }

        public VacancyTagRecord(VacancyTagRecord source)
        {
            VacancyId = source.VacancyId;
            Vacancy = source.Vacancy;
        }
    }
}