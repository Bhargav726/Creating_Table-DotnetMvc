using System.ComponentModel.DataAnnotations;

namespace Entities
{
    /// <summary>
    /// Domain Model for Country
    /// </summary>
    public class Country
    {
        [Key]
        public Guid CountryId { get; set; } = Guid.NewGuid();
        public string? CountryName { get; set; }

        public static implicit operator Country?(string? v)
        {
            if (string.IsNullOrWhiteSpace(v))
                return null; // Return null if input is empty

            return new Country
            {
                CountryName = v // Assign the string value to CountryName
            };
        }

        //public static implicit operator Country?(string? v)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
