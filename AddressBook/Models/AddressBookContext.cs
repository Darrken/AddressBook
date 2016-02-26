namespace AddressBook.Models
{
	using System.Data.Entity;

	public class AddressBookContext : DbContext
	{
		public AddressBookContext() : base("name=AddressBookContext") { }

		public virtual DbSet<Contact> Contacts { get; set; }
	}
}