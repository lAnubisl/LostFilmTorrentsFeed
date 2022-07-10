namespace LostFilmMonitoring.DAO.Azure.Tests
{
    [ExcludeFromCodeCoverage]
    public class AzureBlobStorageModelPersisterTests
    {
        [Test]
        public async Task LoadAsync_should_deserialize_json_file()
        {
            var client = new Mock<IAzureBlobStorageClient>();
            client
                .Setup(x => x.DownloadStringAsync("models", "ReteOrgItems.json"))
                .ReturnsAsync(GetFile("ReteOrgItems.json"));
            var logger = new ConsoleLogger("test");
            var persister = new AzureBlobStorageModelPersister(client.Object, logger);
            var model = await persister.LoadAsync<SortedSet<FeedItemResponse>>("ReteOrgItems");
            Assert.That(model, Is.Not.Null);
        }
        
        private static string GetFile(string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"{assembly.GetName().Name}.Resources.{fileName}";
            using var stream = assembly.GetManifestResourceStream(resourceName);
            using var reader = new StreamReader(stream!);
            return reader.ReadToEnd();
        }
    }
}
