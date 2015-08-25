using System;
using System.Linq;

using LinqToDB;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Erm;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context.Implementation
{
    public sealed class ErmContext : IErmContext
    {
        private const int ActivityStatusCompleted = 2;
        private const int RegardingObjectReference = 1;

        // TODO {all, 24.08.2015}: Когда появится сервис идентификаторов сущностей - переиспользовать его
        private const long FirmTypeId = 146;
        private const long ClientTypeId = 200;

        private readonly IDataContext _context;

        public ErmContext(IDataContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            _context = context;
        }

        public IQueryable<Account> Accounts
        {
            get
            {
                return from account in _context.GetTable<Account>()
                       where account.IsActive && !account.IsDeleted
                       select account;
            }
        }

        public IQueryable<Category> Categories
        {
            get
            {
                return from category in _context.GetTable<Category>()
                       where category.IsActive && !category.IsDeleted
                       select category;
            }
        }

        public IQueryable<CategoryGroup> CategoryGroups
        {
            get
            {
                return from categoryGroup in _context.GetTable<CategoryGroup>()
                       where categoryGroup.IsActive && !categoryGroup.IsDeleted
                       select categoryGroup;
            }
        }

        public IQueryable<BranchOfficeOrganizationUnit> BranchOfficeOrganizationUnits
        {
            get
            {
                return from branchOfficeOrganizationUnit in _context.GetTable<BranchOfficeOrganizationUnit>()
                       where branchOfficeOrganizationUnit.IsActive && !branchOfficeOrganizationUnit.IsDeleted
                       select branchOfficeOrganizationUnit;
            }
        }

        public IQueryable<CategoryFirmAddress> CategoryFirmAddresses
        {
            get
            {
                return from categoryFirmAddress in _context.GetTable<CategoryFirmAddress>()
                       where categoryFirmAddress.IsActive && !categoryFirmAddress.IsDeleted
                       select categoryFirmAddress;
            }
        }

        public IQueryable<CategoryOrganizationUnit> CategoryOrganizationUnits
        {
            get
            {
                return from categoryOrganizationUnit in _context.GetTable<CategoryOrganizationUnit>()
                       where categoryOrganizationUnit.IsActive && !categoryOrganizationUnit.IsDeleted
                       select categoryOrganizationUnit;
            }
        }

        public IQueryable<Firm> Firms
        {
            get
            {
                return from firm in _context.GetTable<Firm>()
                       where firm.IsActive && !firm.IsDeleted && !firm.ClosedForAscertainment
                       select firm;
            }
        }

        public IQueryable<FirmAddress> FirmAddresses
        {
            get
            {
                return from firmAddress in _context.GetTable<FirmAddress>()
                       where firmAddress.IsActive && !firmAddress.IsDeleted && !firmAddress.ClosedForAscertainment
                       select firmAddress;
            }
        }

        public IQueryable<FirmContact> FirmContacts
        {
            get
            {
                return from firmContact in _context.GetTable<FirmContact>()
                       select firmContact;
            }
        }

        public IQueryable<LegalPerson> LegalPersons
        {
            get
            {
                return from legalPerson in _context.GetTable<LegalPerson>()
                       where legalPerson.IsActive && !legalPerson.IsDeleted
                       select legalPerson;
            }
        }

        public IQueryable<Client> Clients
        {
            get
            {
                return from client in _context.GetTable<Client>()
                       where client.IsActive && !client.IsDeleted
                       select client;
            }
        }

        public IQueryable<Contact> Contacts
        {
            get
            {
                return from contact in _context.GetTable<Contact>()
                       where contact.IsActive && !contact.IsDeleted
                       select contact;
            }
        }

        public IQueryable<Order> Orders
        {
            get
            {
                return from order in _context.GetTable<Order>()
                       where order.IsActive && !order.IsDeleted
                       select order;
            }
        }

        public IQueryable<Project> Projects
        {
            get
            {
                return from project in _context.GetTable<Project>()
                       where project.IsActive
                       select project;
            }
        }

        public IQueryable<Territory> Territories
        {
            get
            {
                return from territory in _context.GetTable<Territory>()
                       where territory.IsActive
                       select territory;
            }
        }

        public IQueryable<Appointment> Appointments
        {
            get
            {
                return from appointment in _context.GetTable<Appointment>()
                       where appointment.IsActive && !appointment.IsDeleted && appointment.Status == ActivityStatusCompleted
                       select appointment;
            }
        }

        public IQueryable<AppointmentReference> AppointmentClients
        {
            get
            {
                return from reference in _context.GetTable<AppointmentReference>()
                       where reference.Reference == RegardingObjectReference && reference.ReferencedType == ClientTypeId
                       select reference;
            }
        }

        public IQueryable<AppointmentReference> AppointmentFirms
        {
            get
            {
                return from reference in _context.GetTable<AppointmentReference>()
                       where reference.Reference == RegardingObjectReference && reference.ReferencedType == FirmTypeId
                       select reference;
            }
        }


        public IQueryable<Phonecall> Phonecalls
        {
            get
            {
                return from appointment in _context.GetTable<Phonecall>()
                       where appointment.IsActive && !appointment.IsDeleted && appointment.Status == ActivityStatusCompleted
                       select appointment;
            }
        }

        public IQueryable<PhonecallReference> PhonecallClients
        {
            get
            {
                return from reference in _context.GetTable<PhonecallReference>()
                       where reference.Reference == RegardingObjectReference && reference.ReferencedType == ClientTypeId
                       select reference;
            }
        }

        public IQueryable<PhonecallReference> PhonecallFirms
        {
            get
            {
                return from reference in _context.GetTable<PhonecallReference>()
                       where reference.Reference == RegardingObjectReference && reference.ReferencedType == FirmTypeId
                       select reference;
            }
        }

        public IQueryable<Task> Tasks
        {
            get
            {
                return from appointment in _context.GetTable<Task>()
                       where appointment.IsActive && !appointment.IsDeleted && appointment.Status == ActivityStatusCompleted
                       select appointment;
            }
        }

        public IQueryable<TaskReference> TaskClients
        {
            get
            {
                return from reference in _context.GetTable<TaskReference>()
                       where reference.Reference == RegardingObjectReference && reference.ReferencedType == ClientTypeId
                       select reference;
            }
        }

        public IQueryable<TaskReference> TaskFirms
        {
            get
            {
                return from reference in _context.GetTable<TaskReference>()
                       where reference.Reference == RegardingObjectReference && reference.ReferencedType == FirmTypeId
                       select reference;
            }
        }

        public IQueryable<Letter> Letters
        {
            get
            {
                return from appointment in _context.GetTable<Letter>()
                       where appointment.IsActive && !appointment.IsDeleted && appointment.Status == ActivityStatusCompleted
                       select appointment;
            }
        }

        public IQueryable<LetterReference> LetterClients
        {
            get
            {
                return from reference in _context.GetTable<LetterReference>()
                       where reference.Reference == RegardingObjectReference && reference.ReferencedType == ClientTypeId
                       select reference;
            }
        }

        public IQueryable<LetterReference> LetterFirms
        {
            get
            {
                return from reference in _context.GetTable<LetterReference>()
                       where reference.Reference == RegardingObjectReference && reference.ReferencedType == FirmTypeId
                       select reference;
            }
        }
    }
}