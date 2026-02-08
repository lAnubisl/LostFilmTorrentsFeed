namespace LostFilmMonitoring.BLL.Tests;

[ExcludeFromCodeCoverage]
public class ConfigurationTests
{
    private Mock<IConfigurationValuesProvider>? providerMock;

    [SetUp]
    public void Setup()
    {
        providerMock = new();
    }

    [Test]
    public void AllProperties_should_return_values()
    {
        providerMock!.Setup(x => x.GetValue("BASEURL")).Returns("BASEURL");
        providerMock!.Setup(x => x.GetValue("BASEFEEDCOOKIE")).Returns("BASEFEEDCOOKIE");
        providerMock!.Setup(x => x.GetValue("BASELINKUID")).Returns("BASELINKUID");
        providerMock!.Setup(x => x.GetValue("TORRENTTRACKERS")).Returns("#1{0},#2{0},#3{0}");
        providerMock!.Setup(x => x.GetValue("IMAGESDIRECTORY")).Returns("IMAGESDIRECTORY");
        providerMock!.Setup(x => x.GetValue("TORRENTSDIRECTORY")).Returns("TORRENTSDIRECTORY");
        var service = GetService();
        service.BaseUID.Should().BeEquivalentTo("BASELINKUID");
        service.BaseUSESS.Should().BeEquivalentTo("BASEFEEDCOOKIE");
        service.BaseUrl.Should().BeEquivalentTo("BASEURL");
        service.GetTorrentAnnounceList("USERID").Should().BeEquivalentTo(["#1USERID", "#2USERID", "#3USERID"]);
    }

    [Test]
    public void GetTorrentAnnounceList_should_have_default_value()
    {
        providerMock!.Setup(x => x.GetValue("BASEURL")).Returns("BASEURL");
        providerMock!.Setup(x => x.GetValue("BASEFEEDCOOKIE")).Returns("BASEFEEDCOOKIE");
        providerMock!.Setup(x => x.GetValue("BASELINKUID")).Returns("BASELINKUID");
        providerMock!.Setup(x => x.GetValue("TORRENTTRACKERS")).Returns("#1{0},#2{0},#3{0}");
        providerMock!.Setup(x => x.GetValue("IMAGESDIRECTORY")).Returns("IMAGESDIRECTORY");
        providerMock!.Setup(x => x.GetValue("TORRENTSDIRECTORY")).Returns("TORRENTSDIRECTORY");
        var service = GetService();
        service.GetTorrentAnnounceList(null!).Should().BeEquivalentTo(["#1BASELINKUID", "#2BASELINKUID", "#3BASELINKUID"]);
    }

    [Test]
    public void Constructor_should_fail_when_BASEURL_not_set()
    {
        providerMock!.Setup(x => x.GetValue("BASEURL")).Returns(null as string);
        providerMock!.Setup(x => x.GetValue("BASEFEEDCOOKIE")).Returns("BASEFEEDCOOKIE");
        providerMock!.Setup(x => x.GetValue("BASELINKUID")).Returns("BASELINKUID");
        providerMock!.Setup(x => x.GetValue("TORRENTTRACKERS")).Returns("#1{0},#2{0},#3{0}");
        providerMock!.Setup(x => x.GetValue("IMAGESDIRECTORY")).Returns("IMAGESDIRECTORY");
        providerMock!.Setup(x => x.GetValue("TORRENTSDIRECTORY")).Returns("TORRENTSDIRECTORY");
        
        var action = () => GetService();
        action.Should().Throw<Exception>().WithMessage("Environment variable 'BASEURL' is not defined.");
    }

    [Test]
    public void Constructor_should_fail_when_BASEFEEDCOOKIEL_not_set()
    {
        providerMock!.Setup(x => x.GetValue("BASEURL")).Returns("BASEURL");
        providerMock!.Setup(x => x.GetValue("BASEFEEDCOOKIE")).Returns(null as string);
        providerMock!.Setup(x => x.GetValue("BASELINKUID")).Returns("BASELINKUID");
        providerMock!.Setup(x => x.GetValue("TORRENTTRACKERS")).Returns("#1{0},#2{0},#3{0}");
        providerMock!.Setup(x => x.GetValue("IMAGESDIRECTORY")).Returns("IMAGESDIRECTORY");
        providerMock!.Setup(x => x.GetValue("TORRENTSDIRECTORY")).Returns("TORRENTSDIRECTORY");

        var action = () => GetService();
        action.Should().Throw<Exception>().WithMessage("Environment variable 'BASEFEEDCOOKIE' is not defined.");
    }

    [Test]
    public void Constructor_should_fail_when_BASELINKUID_not_set()
    {
        providerMock!.Setup(x => x.GetValue("BASEURL")).Returns("BASEURL");
        providerMock!.Setup(x => x.GetValue("BASEFEEDCOOKIE")).Returns("BASEFEEDCOOKIE");
        providerMock!.Setup(x => x.GetValue("BASELINKUID")).Returns(null as string);
        providerMock!.Setup(x => x.GetValue("TORRENTTRACKERS")).Returns("#1{0},#2{0},#3{0}");
        providerMock!.Setup(x => x.GetValue("IMAGESDIRECTORY")).Returns("IMAGESDIRECTORY");
        providerMock!.Setup(x => x.GetValue("TORRENTSDIRECTORY")).Returns("TORRENTSDIRECTORY");

        var action = () => GetService();
        action.Should().Throw<Exception>().WithMessage("Environment variable 'BASELINKUID' is not defined.");
    }

    [Test]
    public void Constructor_should_fail_when_TORRENTTRACKERS_not_set()
    {
        providerMock!.Setup(x => x.GetValue("BASEURL")).Returns("BASEURL");
        providerMock!.Setup(x => x.GetValue("BASEFEEDCOOKIE")).Returns("BASEFEEDCOOKIE");
        providerMock!.Setup(x => x.GetValue("BASELINKUID")).Returns("BASELINKUID");
        providerMock!.Setup(x => x.GetValue("TORRENTTRACKERS")).Returns(null as string);
        providerMock!.Setup(x => x.GetValue("IMAGESDIRECTORY")).Returns("IMAGESDIRECTORY");
        providerMock!.Setup(x => x.GetValue("TORRENTSDIRECTORY")).Returns("TORRENTSDIRECTORY");

        var action = () => GetService();
        action.Should().Throw<Exception>().WithMessage("Environment variable 'TORRENTTRACKERS' is not defined.");
    }

    private Configuration GetService() => new (providerMock!.Object);
}
