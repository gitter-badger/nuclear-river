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

        IQueryable<ActivityBase<Appointment>> Appointments { get; }

        IQueryable<ActivityReference<Appointment>> AppointmentClients { get; }
        
        IQueryable<ActivityReference<Appointment>> AppointmentFirms { get; }
        
        IQueryable<ActivityBase<Phonecall>> Phonecalls { get; }
        
        IQueryable<ActivityReference<Phonecall>> PhonecallClients { get; }
        
        IQueryable<ActivityReference<Phonecall>> PhonecallFirms { get; }
        
        IQueryable<ActivityBase<Task>> Tasks { get; }
        
        IQueryable<ActivityReference<Task>> TaskClients { get; }
        
        IQueryable<ActivityReference<Task>> TaskFirms { get; }
        
        IQueryable<ActivityBase<Letter>> Letter { get; }
        
        IQueryable<ActivityReference<Letter>> LetterClients { get; }
        
        IQueryable<ActivityReference<Letter>> LetterFirms { get; }
    }
}