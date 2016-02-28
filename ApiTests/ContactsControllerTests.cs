using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using AddressBook.Controllers;
using AddressBook.Models;
using AddressBook.Services;
using Moq;
using NUnit.Framework;

namespace ApiTests
{
	[TestFixture]
	public class ContactsControllerTests
	{
		private List<Contact> _contacts;
		private Mock<IContactsService> _mockService;
		private ContactsController _controller;

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
					Email = "george@outlook.com",
					FirstName = "George",
					LastName = "Lenny",
					Phone = "201.343.6844"
				}
			};

			_mockService = new Mock<IContactsService>();
			_mockService.Setup(c => c.GetContacts()).Returns(_contacts);
			_mockService.Setup(c => c.Get(It.IsAny<int>())).Returns(_contacts[0]);
			_mockService.Setup(c => c.Add(It.IsAny<Contact>())).Returns(_contacts[0]);
			_mockService.Setup(c => c.Update(It.IsAny<Contact>())).Returns(true);
			_mockService.Setup(c => c.Delete(It.IsAny<int>())).Returns(true);

			_controller = new ContactsController(_mockService.Object);
		}

		[Test]
		public void GetReturnsListOfContacts()
		{
			// Act
			var response = _controller.Get();

			// Assert
			Assert.AreEqual(3, response.Count());
		}

		[Test]
		public void GetByIdReturnsASingleContact()
		{
			// Act
			var response = _controller.Get(1);

			// Assert
			Assert.AreEqual(1, response.Id);
			Assert.AreEqual("Ken", response.FirstName);
			Assert.AreEqual("801.345.1234", response.Phone);
		}

		[Test]
		public void GetByIdThrowsExceptionWithInvalidId()
		{
			// Arrange
			_mockService.Setup(c => c.Get(It.IsAny<int>())).Returns(new Contact());

			// Act & Assert
			Assert.That(() => _controller.Get(5), Throws.TypeOf<HttpResponseException>());
		}

		[Test]
		public void PostReturnsContactWhenSuccessful()
		{
			// Act
			var response = _controller.Post(_contacts[0]);

			// Assert
			_mockService.Verify(m => m.Add(It.IsAny<Contact>()), Times.Once);
			Assert.AreEqual(1, response.Id);
			Assert.AreEqual("Ken", response.FirstName);
			Assert.AreEqual("801.345.1234", response.Phone);
		}

		[Test]
		public void PostThrowsExceptionWhenRequiredFieldsAreNullOrEmpty()
		{
			// Arrange
			var contact = new Contact
			{
				Email = null,
				FirstName = "",
				LastName = "Adams",
				Phone = "801.345.1234"
			};

			// Act & Assert
			Assert.That(() => _controller.Post(contact), Throws.TypeOf<HttpResponseException>());
			_mockService.Verify(m => m.Add(It.IsAny<Contact>()), Times.Never);
			
			// TODO: more checks with mixes of null and empty strings
		}

		[Test]
		public void PutCallsUpdateWhenSuccessful()
		{
			// Act
			_controller.Put(_contacts[0]);

			// Assert
			_mockService.Verify(m => m.Update(It.IsAny<Contact>()), Times.Once);
		}

		[Test]
		public void PutThrowsExceptionWhenRequiredFieldsAreNullOrEmpty()
		{
			// Arrange
			var contact = new Contact
			{
				Email = null,
				FirstName = "",
				LastName = "Adams",
				Phone = "801.345.1234"
			};

			// Act & Assert
			Assert.That(() => _controller.Put(contact), Throws.TypeOf<HttpResponseException>());
			_mockService.Verify(m => m.Update(It.IsAny<Contact>()), Times.Never);

			// TODO: more checks with mixes of null and empty strings
		}

		[Test]
		public void PutThrowsExceptionWhenUpdateFails()
		{
			// Arrange
			_mockService.Setup(c => c.Update(It.IsAny<Contact>())).Returns(false);

			// Act & Assert
			Assert.That(() => _controller.Put(_contacts[0]), Throws.TypeOf<HttpResponseException>());
			_mockService.Verify(m => m.Update(It.IsAny<Contact>()), Times.Once);
		}

		[Test]
		public void DeleteThrowsExceptionWhenDeleteFails()
		{
			// Arrange
			_mockService.Setup(c => c.Delete(It.IsAny<int>())).Returns(false);

			// Act & Assert
			Assert.That(() => _controller.Delete(5), Throws.TypeOf<HttpResponseException>());
			_mockService.Verify(m => m.Delete(It.IsAny<int>()), Times.Once);
		}

		[Test]
		public void SearchReturnsFilteredResults()
		{
			// Act
			var results = _controller.Search("Jake").ToList();

			// Assert
			Assert.AreEqual(1, results.Count);
			Assert.AreEqual("Romaine", results[0].LastName);

			// Act
			results = _controller.Search("gmail").ToList();

			// Assert
			Assert.AreEqual(2, results.Count);
		}

		[Test]
		public void SearchReturnsAllResultsWhenParamIsNullOrEmpty()
		{
			// Act
			var results = _controller.Search(null).ToList();

			// Assert
			Assert.AreEqual(3, results.Count);

			// Act
			results = _controller.Search("").ToList();

			// Assert
			Assert.AreEqual(3, results.Count);
		}
	}
}