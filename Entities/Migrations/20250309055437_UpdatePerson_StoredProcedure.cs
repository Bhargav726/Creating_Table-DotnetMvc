using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePerson_StoredProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string sp_InsertPerson = @"
            CREATE PROCEDURE [dbo].[InsertPerson]
            (@PersonId Uniqueidentifier, @PersonName nvarchar(40),@PersonEmail nvarchar(40),@PersonDOB datetime2(7),@PersonGender nvarchar(10),@CountryId uniqueidentifier, @PersonAddress nvarchar(100),@PersonReceiveNewsLetters bit)
            AS BEGIN 
            INSERT INTO [dbo].[Persons](PersonId, PersonName, PersonEmail, PersonDOB, PersonGender, CountryId, PersonAddress, PersonReceiveNewsLetters) VALUES (@PersonId, @PersonName, @PersonEmail,@PersonDOB, @PersonGender, @CountryId,@PersonAddress,@PersonReceiveNewsLetters)
            END";
            migrationBuilder.Sql(sp_InsertPerson);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string sp_InsertPerson = @"
            DROP PROCEDURE [dbo].[InsertPerson]
            ";
            migrationBuilder.Sql(sp_InsertPerson);
        }
    }
}
