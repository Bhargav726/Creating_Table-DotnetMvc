using System;
using System.ComponentModel.DataAnnotations;
using Entities;
using ServiceContracts.Enums;

namespace ServiceContracts.DTO;

public class PersonUpdateRequest
{
    /// <summary>
    /// Represents the DTO class that contains the person details to update
    /// </summary>
    
    [Required(ErrorMessage ="Person Id can't be blank")]
    public Guid PersonId {get;set;}
    [Required(ErrorMessage ="Person Name can't be blank")]
    public string? PersonName{get;set;}
    [Required(ErrorMessage ="Email can't be blank")]
    [EmailAddress(ErrorMessage ="Enter valid Email")]
    public string? PersonEmail{get;set;}
    public DateTime PersonDOB{get;set;}
    public GenderOptions? PersonGender{get;set;}
    public Guid CountryId{get;set;}
    public string? PersonAddress{get;set;}
    public bool PersonReceiveNewsLetters{get;set;}

    /// <summary>
    /// Converts the current object of PersonAddRequest into a new object of Person type
    /// </summary>
    /// <returns>Returns Person Object</returns>
    public Person ToPerson(){
        return new Person(){PersonId=PersonId,PersonName=PersonName,PersonEmail=PersonEmail,PersonDOB=PersonDOB,PersonGender=PersonGender.ToString(),CountryId=CountryId,PersonAddress=PersonAddress,PersonReceiveNewsLetters=PersonReceiveNewsLetters};
    }
}
