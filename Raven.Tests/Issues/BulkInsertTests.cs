﻿using Raven.Client.Connection;
using Raven.Client.Document;
using Raven.Json.Linq;
using Xunit;

namespace Raven.Tests.Issues
{
	public class BulkInsertTests : RavenTest
	{
		[Fact]
		public void CanCreateAndDisposeUsingBulk()
		{
			using(var store = NewRemoteDocumentStore())
			{
				var bulkInsertOperation = new RemoteBulkInsertOperation((ServerClient) store.DatabaseCommands);
				bulkInsertOperation.Dispose();
			}
		}

		[Fact]
		public void CanInsertSingleDocument()
		{
			using (var store = NewRemoteDocumentStore())
			{
				var bulkInsertOperation = new RemoteBulkInsertOperation((ServerClient)store.DatabaseCommands);
				bulkInsertOperation.Write("test", new RavenJObject(), new RavenJObject{{"test","passed"}});
				bulkInsertOperation.Dispose();

				Assert.Equal("passed", store.DatabaseCommands.Get("test").DataAsJson.Value<string>("test"));
			}
		}

		[Fact]
		public void CanInsertSeveralDocuments()
		{
			using (var store = NewRemoteDocumentStore())
			{
				var bulkInsertOperation = new RemoteBulkInsertOperation((ServerClient)store.DatabaseCommands);
				bulkInsertOperation.Write("one", new RavenJObject(), new RavenJObject { { "test", "passed" } });
				bulkInsertOperation.Write("two", new RavenJObject(), new RavenJObject { { "test", "passed" } });
				bulkInsertOperation.Dispose();

				Assert.Equal("passed", store.DatabaseCommands.Get("one").DataAsJson.Value<string>("test"));
				Assert.Equal("passed", store.DatabaseCommands.Get("two").DataAsJson.Value<string>("test"));
			}
		}

		[Fact]
		public void CanInsertSeveralDocumentsInSeveralBatches()
		{
			using (var store = NewRemoteDocumentStore())
			{
				var bulkInsertOperation = new RemoteBulkInsertOperation((ServerClient)store.DatabaseCommands, batchSize:2);
				bulkInsertOperation.Write("one", new RavenJObject(), new RavenJObject { { "test", "passed" } });
				bulkInsertOperation.Write("two", new RavenJObject(), new RavenJObject { { "test", "passed" } });
				bulkInsertOperation.Write("three", new RavenJObject(), new RavenJObject { { "test", "passed" } });
				bulkInsertOperation.Dispose();

				Assert.Equal("passed", store.DatabaseCommands.Get("one").DataAsJson.Value<string>("test"));
				Assert.Equal("passed", store.DatabaseCommands.Get("two").DataAsJson.Value<string>("test"));
				Assert.Equal("passed", store.DatabaseCommands.Get("three").DataAsJson.Value<string>("test"));
			}
		}
	}
}