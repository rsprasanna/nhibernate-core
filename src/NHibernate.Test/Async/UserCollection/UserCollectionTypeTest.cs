﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.UserCollection
{
	using System.Threading.Tasks;
	[TestFixture]
	public class UserCollectionTypeTestAsync : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override string[] Mappings
		{
			get { return new string[] {"UserCollection.UserPermissions.hbm.xml"}; }
		}

		[Test]
		public async Task BasicOperationAsync()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			User u = new User("max");
			u.EmailAddresses.Add(new Email("max@hibernate.org"));
			u.EmailAddresses.Add(new Email("max.andersen@jboss.com"));
			await (s.SaveAsync(u));
			await (t.CommitAsync());
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			User u2 = (User) await (s.CreateCriteria(typeof(User)).UniqueResultAsync());
			Assert.IsTrue(NHibernateUtil.IsInitialized(u2.EmailAddresses));
			Assert.AreEqual(2, u2.EmailAddresses.Count);
			await (s.DeleteAsync(u2));
			await (t.CommitAsync());
			s.Close();
		}
	}
}
