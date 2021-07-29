using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.Entities
{
    public abstract class TagRecordBase
    {
        [Key]
        [Column(TypeName = "integer")]
        public int RecordId { get; set; }

        [Column(TypeName = "integer")]
        public int TagId { get; set; }

        [NotMapped]
        public Tag Tag;

        protected TagRecordBase()
        {
        }

        protected TagRecordBase(TagRecordBase source)
        {
            RecordId = source.RecordId;
            TagId = source.TagId;
            Tag = source.Tag;
        }
    }
}