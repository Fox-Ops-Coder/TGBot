using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.Entities
{
    [Table("CourceTagRecord")]
    public sealed class CourceTagRecord : TagRecordBase
    {
        [Column(TypeName = "integer")]
        public int CourceId { get; set; }

        [NotMapped]
        public Cource Cource;

        public CourceTagRecord()
        {
        }

        public CourceTagRecord(CourceTagRecord source) : base(source)
        {
            CourceId = source.CourceId;
            Cource = source.Cource;
        }
    }
}