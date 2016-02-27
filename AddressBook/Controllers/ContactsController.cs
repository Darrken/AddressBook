using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AddressBook.Models;
using AddressBook.Services;
using Microsoft.Ajax.Utilities;

namespace AddressBook.Controllers
{
	public class ContactsController : ApiController
	{
		private readonly IContactsService _contactsService;

		public ContactsController(IContactsService contactsService)
		{
			_contactsService = contactsService;
		}

		// GET api/contacts
		public IEnumerable<Contact> Get()
		{
			return _contactsService.GetContacts();
		}

		// GET api/contacts/5
		public Contact Get(int id)
		{
			var contact = _contactsService.Get(id);

			if (contact.Id == 0)
			{
				var response = new HttpResponseMessage(HttpStatusCode.NotFound)
				{
					Content = new StringContent($"No Contact found with Id = {id}"),
					ReasonPhrase = "Contact Id Not Found"
				};
				throw new HttpResponseException(response);
			}
			
			return contact;
		}

		// POST api/contacts
		public Contact Post([FromBody]Contact contact)
		{
			Contact newContact;

			if (contact.FirstName.IsNullOrWhiteSpace() || contact.LastName.IsNullOrWhiteSpace() ||
			    contact.Email.IsNullOrWhiteSpace())
			{
				var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
				{
					Content = new StringContent("The Contact is missing one or more required fields."),
					ReasonPhrase = "Contact Missing Fields"
				};
				throw new HttpResponseException(response);
			}

			try
			{
				newContact = _contactsService.Add(contact);
			}
			catch (Exception)
			{
				var response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
				{
					Content = new StringContent("An unexpected error occurred. The Contact was not saved."),
					ReasonPhrase = "Unexpected Error"
				};
				throw new HttpResponseException(response);
			}

			return newContact;
		}

		// PUT api/contacts/5
		public void Put([FromBody]Contact contact)
		{
			if (contact.FirstName.IsNullOrWhiteSpace() || contact.LastName.IsNullOrWhiteSpace() ||
				contact.Email.IsNullOrWhiteSpace())
			{
				var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
				{
					Content = new StringContent("The Contact is missing one or more required fields."),
					ReasonPhrase = "Contact Missing Fields"
				};
				throw new HttpResponseException(response);
			}

			var success = _contactsService.Update(contact);

			if (!success)
			{
				var response = new HttpResponseMessage(HttpStatusCode.NotFound)
				{
					Content = new StringContent($"No Contact found with Id = {contact.Id}"),
					ReasonPhrase = "Contact Id Not Found"
				};
				throw new HttpResponseException(response);
			}
		}

		// DELETE api/contacts/5
		public void Delete(int id)
		{
			var success = _contactsService.Delete(id);

			if (!success)
			{
				var response = new HttpResponseMessage(HttpStatusCode.NotFound)
				{
					Content = new StringContent($"No Contact found with Id = {id}"),
					ReasonPhrase = "Contact Id Not Found"
				};
				throw new HttpResponseException(response);
			}
		}
	}
}