using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AddressBook.Models;
using AddressBook.Services;

namespace AddressBook.Controllers
{
	public class ContactsController : ApiController
	{
		private readonly IContactService _contactService;

		public ContactsController(IContactService contactService)
		{
			_contactService = contactService;
		}

		// GET api/contacts
		public IEnumerable<Contact> Get()
		{
			return _contactService.GetContacts();
		}

		// GET api/contacts/5
		public Contact Get(int id)
		{
			var contact = _contactService.Get(id);

			if (contact == null)
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
		public void Post([FromBody]Contact contact)
		{
			try
			{
				_contactService.Add(contact);
			}
			catch (Exception)
			{
				var response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
				{
					Content = new StringContent("An unexpected error occurred. The Contact was not saved."),
					ReasonPhrase = "Contact Not Saved"
				};
				throw new HttpResponseException(response);
			}
		}

		// PUT api/contacts/5
		public void Put([FromBody]Contact contact)
		{
			var success = _contactService.Update(contact);

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
			var success = _contactService.Delete(id);

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