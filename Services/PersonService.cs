using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Net.Http.Headers;
using CsvHelper;
using CsvHelper.Configuration;
using Entities;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services.Helpers;

namespace Services;

public class PersonService : IPersonService
{
    private readonly IPersonsRepository _personsRepository;
    
    
    
    //private PersonDbContext personDbContext;
    public PersonService(IPersonsRepository personsRepository)
    {
        _personsRepository = personsRepository;


    }

    /*public PersonService(PersonDbContext personDbContext)
    {
        this.personDbContext = personDbContext;
    }*/

    /*public PersonResponse ConvertPersonToPersonResponse(Person person)
    {
        PersonResponse personResponse = person.ToPersonResponse();
        personResponse.PersonCountry = _countriesService.GetCountryByCountryId(person.PersonCountryId)?.CountryName;
        return personResponse;
    }*/



    public async Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest)
    {
        //Validate PersonName
        /* if(string.IsNullOrEmpty(personAddRequest.PersonName)){
             throw new ArgumentException("PersonName can't be blank");
         }*/

        //Model Validations
        /*ValidationContext validationContext=new ValidationContext(personAddRequest);
        List<ValidationResult> validationResults=new List<ValidationResult>();
        bool isValid=Validator.TryValidateObject(personAddRequest,validationContext,validationResults,true);
        if(!isValid){
            throw new ArgumentException(validationResults.FirstOrDefault()?.ErrorMessage);
        }*/
        if (personAddRequest == null)
        {
            throw new ArgumentNullException(nameof(personAddRequest));
        }
        ValidationHelper.ModelValidation(personAddRequest);

        //convert personAddRequest into Person type
        Person person = personAddRequest.ToPerson();
        //generate PersonId
        person.PersonId = Guid.NewGuid();
        //add person object to persons list
        await _personsRepository.AddPerson(person);
        //await _db.SaveChangesAsync();
        //_db.sp_InsertPerson(person);

        //convert the Person object into PersonResponse type
        //PersonResponse personResponse=person.ToPersonResponse();
        //personResponse.PersonCountry=_countriesService.GetCountryByCountryId(person.PersonCountryId)?.CountryName;
        return person.ToPersonResponse();
    }

    public async Task<List<PersonResponse>> GetAllPersons()
    {
        //Select*from Persons
        var persons = await _personsRepository.GetAllPersons();
        return persons.Select(temp => temp.ToPersonResponse()).ToList();
       
        //return _db.sp_GetAllPersons()
        //    .Select(temp => ConvertPersonToPersonResponse(temp)).ToList();
    }

    public async Task<PersonResponse?> GetPersonByPersonId(Guid?personId)
    {
        if (personId == null)
        {
            return null;
        }

        Person? person = await _personsRepository.GetPersonByPersonId(personId.Value);
        if (person == null)
            return null;

        return person.ToPersonResponse();
    }

    public async Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString)
    {
        List<Person> persons = searchBy switch
        {
            nameof(PersonResponse.PersonName) => await _personsRepository.GetFilteredPersons(temp => temp.PersonName.Contains(searchString)),
            nameof(PersonResponse.PersonEmail) => await _personsRepository.GetFilteredPersons(temp => temp.PersonEmail.Contains(searchString)),
            nameof(PersonResponse.PersonDOB) => await _personsRepository.GetFilteredPersons(temp => temp.PersonDOB.ToString("dd MMMM yyyy").Contains(searchString)),
            nameof(PersonResponse.PersonGender) => await _personsRepository.GetFilteredPersons(temp => temp.PersonGender.Contains(searchString)),
            nameof(PersonResponse.CountryId) => await _personsRepository.GetFilteredPersons(temp => temp.Country.CountryName.Contains(searchString)),
            nameof(PersonResponse.PersonAddress) => await _personsRepository.GetFilteredPersons(temp => temp.PersonAddress.Contains(searchString)),
             _ => await _personsRepository.GetAllPersons()
        };
        return persons.Select(temp => temp.ToPersonResponse()).ToList();
    }

    public async Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> allpersons, string sortBy, SortOrderOptions sortOrder)
    {
        if (string.IsNullOrEmpty(sortBy))
            return allpersons;

        List<PersonResponse> sortedPersons = (sortBy, sortOrder)
        switch
        {
            (nameof(PersonResponse.PersonName), SortOrderOptions.ASC) => allpersons.OrderBy(temp => temp.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),
            (nameof(PersonResponse.PersonName), SortOrderOptions.DESC) => allpersons.OrderByDescending(temp => temp.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),
            (nameof(PersonResponse.PersonEmail), SortOrderOptions.ASC) => allpersons.OrderBy(temp => temp.PersonEmail, StringComparer.OrdinalIgnoreCase).ToList(),
            (nameof(PersonResponse.PersonEmail), SortOrderOptions.DESC) => allpersons.OrderByDescending(temp => temp.PersonEmail, StringComparer.OrdinalIgnoreCase).ToList(),
            (nameof(PersonResponse.PersonDOB), SortOrderOptions.ASC) => allpersons.OrderBy(temp => temp.PersonDOB).ToList(),
            (nameof(PersonResponse.PersonDOB), SortOrderOptions.DESC) => allpersons.OrderByDescending(temp => temp.PersonDOB).ToList(),
            (nameof(PersonResponse.PersonAge), SortOrderOptions.ASC) => allpersons.OrderBy(temp => temp.PersonAge).ToList(),
            (nameof(PersonResponse.PersonAge), SortOrderOptions.DESC) => allpersons.OrderByDescending(temp => temp.PersonAge).ToList(),
            (nameof(PersonResponse.PersonGender), SortOrderOptions.ASC) => allpersons.OrderBy(temp => temp.PersonGender, StringComparer.OrdinalIgnoreCase).ToList(),
            (nameof(PersonResponse.PersonGender), SortOrderOptions.DESC) => allpersons.OrderByDescending(temp => temp.PersonGender, StringComparer.OrdinalIgnoreCase).ToList(),
            (nameof(PersonResponse.Country), SortOrderOptions.ASC) => allpersons.OrderBy(temp => temp.Country, StringComparer.OrdinalIgnoreCase).ToList(),
            (nameof(PersonResponse.Country), SortOrderOptions.DESC) => allpersons.OrderByDescending(temp => temp.Country, StringComparer.OrdinalIgnoreCase).ToList(),
            (nameof(PersonResponse.PersonAddress), SortOrderOptions.ASC) => allpersons.OrderBy(temp => temp.PersonAddress, StringComparer.OrdinalIgnoreCase).ToList(),
            (nameof(PersonResponse.PersonAddress), SortOrderOptions.DESC) => allpersons.OrderByDescending(temp => temp.PersonAddress, StringComparer.OrdinalIgnoreCase).ToList(),
            (nameof(PersonResponse.PersonReceiveNewsLetters), SortOrderOptions.ASC) => allpersons.OrderBy(temp => temp.PersonReceiveNewsLetters).ToList(),
            (nameof(PersonResponse.PersonReceiveNewsLetters), SortOrderOptions.DESC) => allpersons.OrderByDescending(temp => temp.PersonReceiveNewsLetters).ToList(),
            _ => allpersons
        };
        return sortedPersons;
    }

    public async Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest)
    {
        if (personUpdateRequest == null)
            throw new ArgumentNullException(nameof(Person));

        //validation
        ValidationHelper.ModelValidation(personUpdateRequest);

        //get matching person object to update
        Person? matchingPerson = await _personsRepository.GetPersonByPersonId(personUpdateRequest.PersonId);
        if (matchingPerson == null)
        {
            throw new ArgumentException("Given person id doesn't exist");
        }

        //update all details
        matchingPerson.PersonName = personUpdateRequest.PersonName;
        matchingPerson.PersonEmail = personUpdateRequest.PersonEmail;
        matchingPerson.PersonDOB = (DateTime)personUpdateRequest.PersonDOB;
        matchingPerson.PersonGender = personUpdateRequest.PersonGender.ToString();
        matchingPerson.CountryId = (Guid)personUpdateRequest.CountryId;
        //matchingPerson.PersonCountry = personUpdateRequest.PersonCountry;
        matchingPerson.PersonAddress = personUpdateRequest.PersonAddress;
        matchingPerson.PersonReceiveNewsLetters = personUpdateRequest.PersonReceiveNewsLetters;
        await _personsRepository.UpdatePerson(matchingPerson);
        return matchingPerson.ToPersonResponse();

    }

    public async Task<bool> DeletePerson(Guid? personId)
    {
        if (personId == null)
        {
            throw new ArgumentNullException(nameof(personId));
        }
        Person? person = await _personsRepository.GetPersonByPersonId(personId.Value);
        if (person == null)
            return false;

        await _personsRepository.DeletePersonByPersonId(personId.Value);
        //await _db.SaveChangesAsync();
        return true;

    }

    public async Task<MemoryStream> GetPersonsCSV()
    {
        MemoryStream memoryStream = new MemoryStream();
        StreamWriter streamWriter = new StreamWriter(memoryStream);

        CsvConfiguration csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture);
        CsvWriter csvWriter = new CsvWriter(streamWriter,CultureInfo.InvariantCulture,leaveOpen:true);

        //csvWriter.WriteHeader<PersonResponse>(); //PersonId, PersonName....
        csvWriter.WriteField(nameof(PersonResponse.PersonName));
        csvWriter.WriteField(nameof(PersonResponse.PersonEmail));
        csvWriter.WriteField(nameof(PersonResponse.PersonDOB));
        csvWriter.WriteField(nameof(PersonResponse.PersonAge));
        csvWriter.WriteField(nameof(PersonResponse.PersonGender));
        csvWriter.WriteField(nameof(PersonResponse.CountryId));
        csvWriter.WriteField(nameof(PersonResponse.Country));
        csvWriter.WriteField(nameof(PersonResponse.PersonAddress));
        csvWriter.WriteField(nameof(PersonResponse.PersonReceiveNewsLetters));
        csvWriter.NextRecord();

        List<PersonResponse> persons = await GetAllPersons();

        foreach(PersonResponse person in persons)
        {
            csvWriter.WriteField(person.PersonName);
            csvWriter.WriteField(person.PersonEmail);
            if (person.PersonDOB.HasValue)
                csvWriter.WriteField(person.PersonDOB.Value.ToString("yyyy-MM-dd"));
            else
                csvWriter.WriteField("");
            csvWriter.WriteField(person.PersonAge);
            csvWriter.WriteField(person.PersonGender);
            csvWriter.WriteField(person.CountryId);
            csvWriter.WriteField(person.Country);
            csvWriter.WriteField(person.PersonAddress);
            csvWriter.WriteField(person.PersonReceiveNewsLetters);
            csvWriter.NextRecord();
            csvWriter.Flush();
        }

        //await csvWriter.WriteRecordsAsync(persons); //1,abc...

        memoryStream.Position = 0;
        return memoryStream;
    }

    public async Task<MemoryStream> GetPersonsExcel()
    {
        MemoryStream memoryStream = new MemoryStream();
        using (ExcelPackage excelPackage=new ExcelPackage(memoryStream))
        {
            ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("PersonsSheet");
            worksheet.Cells["A1"].Value = "Person Name";
            worksheet.Cells["B1"].Value = "Email";
            worksheet.Cells["C1"].Value = "Date of Birth";
            worksheet.Cells["D1"].Value = "Age";
            worksheet.Cells["E1"].Value = "Gender";
            worksheet.Cells["F1"].Value = "Country";
            worksheet.Cells["G1"].Value = "Address";
            worksheet.Cells["H1"].Value = "Receive News Letters";

            //For styling
            using (ExcelRange headerCells = worksheet.Cells["A1:H1"])
            {
                headerCells.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                headerCells.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightCyan);
                headerCells.Style.Font.Bold = true;
            }

            int row = 2;
            List<PersonResponse> persons = await GetAllPersons();
            foreach(PersonResponse person in persons){
                worksheet.Cells[row, 1].Value = person.PersonName;
                worksheet.Cells[row, 2].Value = person.PersonEmail;
                if(person.PersonDOB.HasValue)
                    worksheet.Cells[row, 3].Value = person.PersonDOB.Value.ToString("yyyy-MM-dd");
                worksheet.Cells[row, 4].Value = person.PersonAge;
                worksheet.Cells[row, 5].Value = person.PersonGender;
                worksheet.Cells[row, 6].Value = person.Country;
                worksheet.Cells[row, 7].Value = person.PersonAddress;
                worksheet.Cells[row, 8].Value = person.PersonReceiveNewsLetters;

                row++;
            }
            worksheet.Cells[$"A1:H{row}"].AutoFitColumns();

            await excelPackage.SaveAsync();
        }
        memoryStream.Position = 0;
        return memoryStream;
    }
}
