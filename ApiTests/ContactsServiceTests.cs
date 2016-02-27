using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using AddressBook.Models;
using AddressBook.Services;
using Moq;
using NUnit.Framework;

namespace ApiTests
{
	[TestFixture]
	public class ContactsServiceTests
	{
		private ContactsService _contactsService;
		private Mock<DbSet<Contact>> _mockDataSet;
		private Mock<AddressBookContext> _mockContext;
		private IQueryable<Contact> _contacts;

		[SetUp]
		public void Init()
		{
			_contacts = new List<Contact>
			{
				new Contact
				{
					Id = 1,
					Address = "123 Adams Street",
					Email = "kja@gmail.com",
					FirstName = "Ken",
					LastName = "Adams",
					Phone = "801.345.1234"
				},
				new Contact
				{
					Id = 2,
					Address = "321 Jefferson Street",
					Email = "jake@gmail.com",
					FirstName = "Jake",
					LastName = "Romaine",
					Phone = "301.332.4324"
				},
				new Contact
				{
					Id = 3,
					Address = "111 Washington Street",
					Email = "george@gmail.com",
					FirstName = "George",
					LastName = "Lenny",
					Phone = "201.343.6844"
				}
			}.AsQueryable();

			_mockDataSet = new Mock<DbSet<Contact>>();
			_mockDataSet.As<IQueryable<Contact>>().Setup(m => m.Provider).Returns(_contacts.Provider);
			_mockDataSet.As<IQueryable<Contact>>().Setup(m => m.Expression).Returns(_contacts.Expression);
			_mockDataSet.As<IQueryable<Contact>>().Setup(m => m.ElementType).Returns(_contacts.ElementType);
			_mockDataSet.As<IQueryable<Contact>>().Setup(m => m.GetEnumerator()).Returns(_contacts.GetEnumerator);
			
			_mockContext = new Mock<AddressBookContext>();
			_mockContext.Setup(c => c.Contacts).Returns(_mockDataSet.Object);
			_contactsService = new ContactsService(_mockContext.Object);
		}

		[Test]
		public void Add_saves_a_new_Contact()
		{
			var contactToAdd = new Contact
			{
				Address = "49 Berkley Avenue",
				Email = "test@outlook.com",
				FirstName = "Danielle",
				LastName = "Casella",
				Phone = "801.555.1234"
			};

			_contactsService.Add(contactToAdd);

			_mockDataSet.Verify(m => m.Add(It.IsAny<Contact>()), Times.Once());
			_mockContext.Verify(m => m.SaveChanges(), Times.Once());
		}

		[Test]
		public void GetContacts_gets_all_Contacts()
		{
			var results = _contactsService.GetContacts();

			Assert.AreEqual(3, results.Count);
			Assert.AreEqual(1, results[0].Id);
			Assert.AreEqual("Ken", results[0].FirstName);
			Assert.AreEqual("801.345.1234", results[0].Phone);
			Assert.AreEqual("Romaine", results[1].LastName);
			Assert.AreEqual("george@gmail.com", results[2].Email);
		}

		[Test]
		public void Get_retrieves_a_single_Contact()
		{
			var result = _contactsService.Get(1);

			Assert.AreEqual(1, result.Id);
			Assert.AreEqual("Ken", result.FirstName);
			Assert.AreEqual("801.345.1234", result.Phone);
		}
	}
}