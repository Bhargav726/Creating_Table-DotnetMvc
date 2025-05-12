using System;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace ServiceContracts;

public interface IPersonService
{
    /// <summary>
    /// Adds a new person into the list of persons
    /// </summary>
    /// <param name="personAddRequest"></param>
    /// <returns>Returns the same person details, along with newly generated PersonId</returns>
    Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest);

    /// <summary>
    /// Returns all persons
    /// </summary>
    /// <returns>Returns a list of objects of PersonResponse type</returns>
    Task<List<PersonResponse>> GetAllPersons();

    /// <summary>
    /// Returns the person object based on the given person id
    /// </summary>
    /// <param name="personId"></param>
    /// <returns>Returns matching person object</returns>
    Task<PersonResponse?> GetPersonByPersonId(Guid? personId);

    /// <summary>
    /// Returns all person object that matches with the given search field and search string
    /// </summary>
    /// <param name="searchBy"></param>
    /// <param name="searchString"></param>
    /// <returns>Returns all matching persons based on the given search field and search string</returns>
    Task<List<PersonResponse>> GetFilteredPersons(string searchBy,string? searchString);

    /// <summary>
    /// Returns sorted list of persons
    /// </summary>
    /// <param name="allpersons">Represents list of persons to sort</param>
    /// <param name="sortBy">Name of the property (key), based on sort</param>
    /// <param name="sortOrder">ASC OR DESC</param>
    /// <returns>Returns sorted persons as PersonResponse list</returns>
    Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse>allpersons,string sortBy, SortOrderOptions sortOrder);


    /// <summary>
    /// Updates the specified person details based on the given person Id
    /// </summary>
    /// <param name="personUpdateRequest">Person details to update,including person id</param>
    /// <returns>Returns the person response object after updation</returns>
    Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest);
    //PersonResponse AddPerson(PersonUpdateRequest person_update_request);

    /// <summary>
    /// Deletes a person based on the given person id
    /// </summary>
    /// <param name="personId">PersonId to delete</param>
    /// <returns>Returns true, if the deletion is successful otherwise false</returns>
    Task<bool> DeletePerson(Guid? personId);

    /// <summary>
    /// Returns persons as CSV (coma seperated values)
    /// </summary>
    /// <returns>Returns the memory stream with CSV data</returns>
    Task<MemoryStream> GetPersonsCSV();

    /// <summary>
    /// Returns persons as Excel
    /// </summary>
    /// <returns>Returns the memory stream with Excel data of persons</returns>
    Task<MemoryStream> GetPersonsExcel();
}
