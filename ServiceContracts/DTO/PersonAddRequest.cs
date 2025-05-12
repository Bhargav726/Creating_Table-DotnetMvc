using System;
using System.ComponentModel.DataAnnotations;
using Entities;
using ServiceContracts.Enums;
namespace ServiceContracts.DTO;

/// <summary>
/// Acts as a DTO(Data Transfer Object) for inserting a new person
/// </summary>
public class PersonAddRequest
{
    [Required(ErrorMessage ="Person Name can't be blank")]
    public string? PersonName{get;set;}
    [Required(ErrorMessage ="Email can't be blank")]
    [EmailAddress(ErrorMessage ="Enter valid Email")]
    //[DataType(DataType.EmailAddress)]
    public string? PersonEmail{get;set;}
    public DateTime? PersonDOB{get;set;}
    public double PersonAge { get; set; }
    public GenderOptions? PersonGender{get;set;}
    [Required(ErrorMessage ="Please select a country")]
    public Guid? CountryId{get;set;}
    public string? PersonAddress{get;set;}
    public bool PersonReceiveNewsLetters{get;set;}

    /// <summary>
    /// Converts the current object of PersonAddRequest into a new object of Person type
    /// </summary>
    /// <returns></returns>
    public Person ToPerson(){
        return new Person(){PersonName=PersonName,PersonEmail=PersonEmail,PersonDOB= (DateTime)PersonDOB, PersonGender=PersonGender.ToString(),CountryId = CountryId,PersonAddress=PersonAddress,PersonReceiveNewsLetters=PersonReceiveNewsLetters};
    }
}
