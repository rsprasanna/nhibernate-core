﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Engine;
using NHibernate.Mapping.ByCode;
using NHibernate.Proxy;
using NUnit.Framework;
using NHibernate.Linq;

namespace NHibernate.Test.NHSpecificTest.GH2043
{
	using System.Threading.Tasks;
	using System.Threading;
	[TestFixture]
	public class FixtureAsync : TestCaseMappingByCode
	{
		private Guid _entityWithClassProxy2Id;
		private Guid _entityWithInterfaceProxy2Id;
		private Guid _entityWithClassLookupId;
		private Guid _entityWithInterfaceLookupId;

		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<EntityWithClassProxyDefinition>(rc =>
			{
				rc.Table("ProxyDefinition");
				rc.Proxy(typeof(EntityWithClassProxyDefinition));

				rc.Id(x => x.Id);
				rc.Property(x => x.Name);
			});

			mapper.Class<EntityWithInterfaceProxyDefinition>(rc =>
			{
				rc.Table("IProxyDefinition");
				rc.Proxy(typeof(IEntityProxy));

				rc.Id(x => x.Id);
				rc.Property(x => x.Name);
			});

			mapper.Class<EntityWithClassLookup>(rc =>
			{
				rc.Id(x => x.Id);
				rc.Property(x => x.Name);
				rc.ManyToOne(x => x.EntityLookup, x  => x.Class(typeof(EntityWithClassProxyDefinition)));
			});

			mapper.Class<EntityWithInterfaceLookup>(rc =>
			{
				rc.Id(x => x.Id);
				rc.Property(x => x.Name);
				rc.ManyToOne(x => x.EntityLookup, x => x.Class(typeof(EntityWithInterfaceProxyDefinition)));
			});

			mapper.Class<EntityAssigned>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.Assigned));
				rc.Property(x => x.Name);
				rc.ManyToOne(x => x.Parent);
			});

			mapper.Class<EntityWithAssignedBag>(
				rc =>
				{
					rc.Id(x => x.Id, m => m.Generator(Generators.Assigned));
					rc.Property(x => x.Name);
					rc.Bag(
						x => x.Children,
						m =>
						{
							m.Inverse(true);
							m.Cascade(Mapping.ByCode.Cascade.All | Mapping.ByCode.Cascade.DeleteOrphans);
						},
						cm => { cm.OneToMany(); });
				});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void OnSetUp()
		{
			using(var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var entityCP1 = new EntityWithClassProxyDefinition
								{
									Id = Guid.NewGuid(),
									Name = "Name 1"
								};

				var entityCP2 = new EntityWithClassProxyDefinition
								{
									Id = Guid.NewGuid(),
									Name = "Name 2"
								};
				_entityWithClassProxy2Id = entityCP2.Id;

				var entityIP1 = new EntityWithInterfaceProxyDefinition
								{
									Id = Guid.NewGuid(),
									Name = "Name 1"
								};

				var entityIP2 = new EntityWithInterfaceProxyDefinition
								{
									Id = Guid.NewGuid(),
									Name = "Name 2"
								};
				_entityWithInterfaceProxy2Id = entityIP2.Id;

				session.Save(entityCP1);
				session.Save(entityCP2);
				session.Save(entityIP1);
				session.Save(entityIP2);

				var entityCL = new EntityWithClassLookup
								{
									Id = Guid.NewGuid(),
									Name = "Name 1",
									EntityLookup = (EntityWithClassProxyDefinition)session.Load(typeof(EntityWithClassProxyDefinition), entityCP1.Id)
								};
				_entityWithClassLookupId = entityCL.Id;

				var entityIL = new EntityWithInterfaceLookup
								{
									Id = Guid.NewGuid(),
									Name = "Name 1",
									EntityLookup = (IEntityProxy)session.Load(typeof(EntityWithInterfaceProxyDefinition), entityIP1.Id)
								};
				_entityWithInterfaceLookupId = entityIL.Id;

				session.Save(entityCL);
				session.Save(entityIL);

				session.Flush();
				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Delete("from System.Object");

				session.Flush();
				transaction.Commit();
			}
		}

		[Test]
		public async Task UpdateEntityWithClassLookupAsync()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var entityToUpdate = await (session.Query<EntityWithClassLookup>()
											.FirstAsync(e => e.Id == _entityWithClassLookupId));

				entityToUpdate.EntityLookup = (EntityWithClassProxyDefinition) await (session.LoadAsync(typeof(EntityWithClassProxyDefinition), _entityWithClassProxy2Id));

				await (session.UpdateAsync(entityToUpdate));
				await (session.FlushAsync());
				await (transaction.CommitAsync());
			}
		}

		[Test]
		public async Task UpdateEntityWithInterfaceLookupAsync()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var entityToUpdate = await (session.Query<EntityWithInterfaceLookup>()
											.FirstAsync(e => e.Id == _entityWithInterfaceLookupId));

				entityToUpdate.EntityLookup = (IEntityProxy) await (session.LoadAsync(typeof(EntityWithInterfaceProxyDefinition), _entityWithInterfaceProxy2Id));

				await (session.UpdateAsync(entityToUpdate));
				await (session.FlushAsync());
				await (transaction.CommitAsync());
			}
		}

		[Test]
		public async Task TransientProxySaveAsync()
		{
			var id = 10;

			using (var session = OpenSession())
			using(var t = session.BeginTransaction())
			{
				var e = new EntityAssigned() {Id = id, Name = "a"};

				await (session.SaveAsync(e));
				await (session.FlushAsync());
				await (t.CommitAsync());
			}

			using (var session = OpenSession())
			using(var t = session.BeginTransaction())
			{
				var e = await (GetTransientProxyAsync(session, id));
				await (session.SaveAsync(e));
				await (session.FlushAsync());
				
				await (t.CommitAsync());
			}

			using (var session = OpenSession())
			{
				var entity = await (session.GetAsync<EntityAssigned>(id));
				Assert.That(entity, Is.Not.Null, "Transient proxy wasn't saved");
			}
		}

		[Test]
		public async Task TransientProxyBagCascadeSaveAsync()
		{
			var id = 10;

			using (var session = OpenSession())
			using(var t = session.BeginTransaction())
			{
				var e = new EntityAssigned() {Id = id, Name = "a"};
				await (session.SaveAsync(e));
				await (session.FlushAsync());
				await (t.CommitAsync());
			}

			using (var session = OpenSession())
			using(var t = session.BeginTransaction())
			{
				var child = await (GetTransientProxyAsync(session, id));
				var parent = new EntityWithAssignedBag()
				{
					Id = 1, Name = "p", Children =
					{
						child
					}
				};
				child.Parent = parent;

				await (session.SaveAsync(parent));
				await (session.FlushAsync());
				
				await (t.CommitAsync());
			}

			using (var session = OpenSession())
			{
				var entity = await (session.GetAsync<EntityAssigned>(id));
				Assert.That(entity, Is.Not.Null, "Transient proxy wasn't saved");
			}
		}

		[Test]
		public async Task TransientProxyDetectionFromUserCodeAsync()
		{
			var id = 10;

			using (var session = OpenSession())
			using (var t = session.BeginTransaction())
			{
				var e = new EntityAssigned() {Id = id, Name = "a"};
				await (session.SaveAsync(e));
				await (session.FlushAsync());
				await (t.CommitAsync());
			}

			using (var session = OpenSession())
			using (var t = session.BeginTransaction())
			{
				var child = await (GetTransientProxyAsync(session, id));
				Assert.That(await (ForeignKeys.IsTransientSlowAsync(typeof(EntityAssigned).FullName, child, session.GetSessionImplementation(), CancellationToken.None)), Is.True);
				await (t.CommitAsync());
			}
		}

		private static async Task<EntityAssigned> GetTransientProxyAsync(ISession session, int id, CancellationToken cancellationToken = default(CancellationToken))
		{
			EntityAssigned e;
			e = await (session.LoadAsync<EntityAssigned>(id, cancellationToken));
			e.Name = "b";
			await (session.DeleteAsync(e, cancellationToken));
			await (session.FlushAsync(cancellationToken));
			Assert.That(e.IsProxy(), Is.True, "Failed test set up");
			return e;
		}
	}
}