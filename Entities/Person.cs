using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{

    /// <summary>
    /// Person domain model class
    /// </summary>
    public class Person
    {
        [Key]
        public Guid PersonId { get; set; } = Guid.NewGuid();
        [StringLength(40)]
        public string? PersonName {get; set;}
        [StringLength(40)]
        public string? PersonEmail {get; set;}
        public DateTime PersonDOB {get; set;}
        //public double PersonAge { get; set; }
        [StringLength(10)]
        public string? PersonGender {get; set;}
        [Required]
        public Guid? CountryId {get; set;}
        [StringLength(100)]
        public string? PersonAddress {get; set;}
        public bool PersonReceiveNewsLetters {get; set;}
        [ForeignKey("CountryId")]
        public virtual Country? Country { get; set; }

    }
}
