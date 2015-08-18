using System.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Erm;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context
{
    public interface IErmContext
    {
        IQueryable<Account> Accounts { get; }

        IQueryable<BranchOfficeOrganizationUnit> BranchOfficeOrganizationUnits { get; }

        IQueryable<Category> Categories { get; }

        IQueryable<CategoryGroup> CategoryGroups { get; }

        IQueryable<CategoryFirmAddress> CategoryFirmAddresses { get; }

        IQueryable<CategoryOrganizationUnit> CategoryOrganizationUnits { get; }

        IQueryable<Firm> Firms { get; }

        IQueryable<FirmAddress> FirmAddresses { get; }

        IQueryable<FirmContact> FirmContacts { get; }

        IQueryable<LegalPerson> LegalPersons { get; }

        IQueryable<Client> Clients { get; }

        IQueryable<Contact> Contacts { get; }

        IQueryable<Order> Orders { get; }

        IQueryable<Project> Projects { get; }

        IQueryable<Territory> Territories { get; }

        IQueryable<Appointment> Appointments { get; }

        IQueryable<AppointmentReference> AppointmentClients { get; }

        IQueryable<AppointmentReference> AppointmentFirms { get; }
        
        IQueryable<Phonecall> Phonecalls { get; }
        
        IQueryable<PhonecallReference> PhonecallClients { get; }
        
        IQueryable<PhonecallReference> PhonecallFirms { get; }
        
        IQueryable<Task> Tasks { get; }
        
        IQueryable<TaskReference> TaskClients { get; }
        
        IQueryable<TaskReference> TaskFirms { get; }
        
        IQueryable<Letter> Letters { get; }
        
        IQueryable<LetterReference> LetterClients { get; }
        
        IQueryable<LetterReference> LetterFirms { get; }
    }
}