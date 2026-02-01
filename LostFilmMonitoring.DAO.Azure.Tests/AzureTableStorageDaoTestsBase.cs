namespace LostFilmMonitoring.DAO.Azure.Tests;

[ExcludeFromCodeCoverage]
public abstract class AzureTableStorageDaoTestsBase<T> where T
    : BaseAzureTableStorageDao
{
    protected Mock<TableServiceClient>? serviceClient;
    protected Mock<TableClient>? tableClient;
    protected Mock<Common.ILogger>? logger;

    [SetUp]
    public void SetUp()
    {
        this.logger = new();
        this.logger.Setup(l => l.CreateScope(It.IsAny<string>())).Returns(this.logger.Object);
        this.serviceClient = new Mock<TableServiceClient>();
        this.tableClient = new Mock<TableClient>();
        this.serviceClient
            .Setup(x => x.GetTableClient(It.IsAny<string>()))
            .Returns(tableClient.Object);
    }

    protected abstract T GetDao();
}
