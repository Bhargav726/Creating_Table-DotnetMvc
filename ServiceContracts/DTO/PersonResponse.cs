using System;
using Entities;
using ServiceContracts.Enums;

namespace ServiceContracts.DTO;

/// <summary>
/// Represents DTO class that is used as return type of most methods of Persons Service
/// </summary>
public class PersonResponse
{
    public  Guid PersonId{get;set;}
    public string? PersonName{get;set;}
    public string? PersonEmail{get;set;}
    public DateTime? PersonDOB{get;set;}
    public double? PersonAge{get;set;}
    public string? PersonGender{get;set;}
    public Guid CountryId{get;set;}
    public string? Country{get;set;}
    public string? PersonAddress{get;set;}
    public bool PersonReceiveNewsLetters{get;set;}

    /// <summary>
    /// Compares the current object with the parameter object
    /// </summary>
    /// <param name="obj">The PersonResponse Object to compare</param>
    /// <returns>True or False, indicating weather all person details are matched with the specified parameter object </returns>
    public override bool Equals(object? obj)
    {
        if(obj==null)
        return false;
        if(obj.GetType()!=typeof(PersonResponse))
        return false;
        PersonResponse person= (PersonResponse)obj;
        return PersonId == person.PersonId && PersonName==person.PersonName && PersonDOB==person.PersonDOB && PersonEmail==person.PersonEmail && PersonGender==person.PersonGender && CountryId==person.CountryId && PersonAge==person.PersonAge && PersonAddress==person.PersonAddress && PersonReceiveNewsLetters==person.PersonReceiveNewsLetters;
    }

    public override int GetHashCode()
    {
        throw new NotImplementedException();
    }

    public override string ToString()
    {
        return $"Person Id:{PersonId},Person Name:{PersonName},Person Email:{PersonEmail}, PersonDOB:{PersonDOB},Person Gender:{PersonGender}, Person CountryId:{CountryId},Country:{Country}, Person Address:{PersonAddress},Recive New Letters:{PersonReceiveNewsLetters}";
    }

    public PersonUpdateRequest ToPersonUpdateRequest(){
        return new PersonUpdateRequest(){PersonId=PersonId,PersonName=PersonName,PersonEmail=PersonEmail,PersonDOB= (DateTime)PersonDOB, PersonGender=(GenderOptions)Enum.Parse(typeof(GenderOptions),PersonGender,true),PersonAddress=PersonAddress,CountryId=CountryId,PersonReceiveNewsLetters=PersonReceiveNewsLetters};
    }
}
public static class PersonExtensions{

    /// <summary>
    /// An Extension method to convert an object of Person class into PersonResponse class
    /// </summary>
    /// <param name="person">Returns the converted PersonResponse object</param>
    public static PersonResponse ToPersonResponse(this Person person)
    {
        //person=>PersonResponse
        return new PersonResponse() { PersonId = person.PersonId, PersonName = person.PersonName, PersonDOB = person.PersonDOB,PersonEmail = person.PersonEmail, PersonGender = person.PersonGender, CountryId = (Guid)person.CountryId, PersonAge = (person.PersonDOB != null) ? Math.Round((DateTime.Now - person.PersonDOB).TotalDays / 365.25) : null, PersonAddress = person.PersonAddress, PersonReceiveNewsLetters = person.PersonReceiveNewsLetters, Country = person.Country?.CountryName };
    }
}
