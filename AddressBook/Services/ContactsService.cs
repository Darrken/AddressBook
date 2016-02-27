using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AddressBook.Models;

namespace AddressBook.Services
{
	public interface IContactService
	{
		List<Contact> GetContacts();
		Contact Get(int id);
		Contact Add(Contact contact);
		bool Update(Contact contact);
		bool Delete(int id);
	}

	public class ContactsService : IContactService
	{
		private readonly AddressBookContext _context;

		public ContactsService(AddressBookContext context)
		{
			_context = context;
		}

		public List<Contact> GetContacts()
		{
			var contactList = _context.Contacts.ToList();

			return !contactList.Any() ? new List<Contact>() : contactList;
		}

		public Contact Get(int id)
		{
			return _context.Contacts.SingleOrDefault(c => c.Id == id);
		}

		public Contact Add(Contact contact)
		{
			var newContact = _context.Contacts.Add(contact);
			_context.SaveChanges();

			return newContact;
		}

		public bool Update(Contact contact)
		{
			var contactToUpdate = _context.Contacts.SingleOrDefault(c => c.Id == contact.Id);

			if (contactToUpdate == null) return false;

			contactToUpdate.Address = contact.Address;
			contactToUpdate.Email = contact.Email;
			contactToUpdate.FirstName = contact.FirstName;
			contactToUpdate.LastName = contact.LastName;
			contactToUpdate.Phone = contact.Phone;
			_context.SaveChanges();

			return true;
		}

		public bool Delete(int id)
		{
			var contactToRemove = _context.Contacts.SingleOrDefault(c => c.Id == id);

			if (contactToRemove == null) return false;

			_context.Contacts.Remove(contactToRemove);
			_context.SaveChanges();

			return true;
		}
	}
}