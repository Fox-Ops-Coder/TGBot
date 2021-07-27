﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.Entities
{
    [Table("Vacancy")]
    public sealed class Vacancy
    {
        [Column(TypeName = "integer")]
        [Key]
        public int VacancyId { get; set; }

        [Column(TypeName = "integer")]
        [Required]
        public int ProfessionId { get; set; }

        [Column(TypeName = "text")]
        [Required]
        public string VacancyName { get; set; }

        [Column(TypeName = "text")]
        [Required]
        public string Url { get; set; }

        public Profession Profession;
    }
}