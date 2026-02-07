namespace LostFilmMonitoring.BLL.Tests.Commands;

[ExcludeFromCodeCoverage]
internal class SaveSubscriptionCommandTests
{
    private Mock<IUserDao>? userDao;
    private Mock<Common.ILogger>? logger;
    private Mock<IValidator<EditSubscriptionRequestModel>>? validator;
    private Mock<IDal>? dal;
    private Mock<IConfiguration>? configuration;
    private Mock<IModelPersister>? persister;
    private Mock<ISubscriptionDao>? subscriptionDAO;
    private Mock<IFeedDao>? feedDAO;
    private Mock<ISeriesDao>? seriesDAO;
    private Mock<ITorrentFileDao>? torrentFileDAO;
    private Dictionary<string, User>? usersCollection;
    private Dictionary<string, Subscription[]>? subscriptionsCollection;
    private Dictionary<string, Series>? seriesCollection;
    private Dictionary<string, TorrentFile>? torrentFileCollection;
    private Dictionary<string, SortedSet<FeedItem>>? feedItemsCollection;

    [SetUp]
    public void Setup()
    {
        this.userDao = new();
        this.logger = new();
        this.feedDAO = new();
        this.dal = new();
        this.validator = new();
        this.configuration = new();
        this.persister = new();
        this.seriesDAO = new();
        this.torrentFileDAO = new();
        this.subscriptionDAO = new();
        this.dal.Setup(x => x.Subscription).Returns(this.subscriptionDAO.Object);
        this.dal.Setup(x => x.Feed).Returns(this.feedDAO.Object);
        this.dal.Setup(x => x.User).Returns(this.userDao.Object);
        this.dal.Setup(x => x.Series).Returns(this.seriesDAO.Object);
        this.dal.Setup(x => x.TorrentFile).Returns(this.torrentFileDAO.Object);
        this.logger.Setup(l => l.CreateScope(It.IsAny<string>())).Returns(this.logger.Object);
        DefineDatabaseState();
        SetupStorage();
    }

    [Test]
    public void Constructor_should_throw_exception_when_logger_null()
    {
        var action = () => new SaveSubscriptionCommand(
            null!,
            this.validator!.Object,
            this.dal!.Object,
            this.configuration!.Object,
            this.persister!.Object
        );
        action.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("logger");
    }

    [Test]
    public void Constructor_should_throw_exception_when_validator_null()
    {
        var action = () => new SaveSubscriptionCommand(
            this.logger!.Object,
            null!,
            this.dal!.Object,
            this.configuration!.Object,
            this.persister!.Object
        );
        action.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("validator");
    }

    [Test]
    public void Constructor_should_throw_exception_when_dal_null()
    {
        var action = () => new SaveSubscriptionCommand(
            this.logger!.Object,
            this.validator!.Object,
            null!,
            this.configuration!.Object,
            this.persister!.Object
        );
        action.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("dal");
    }

    [Test]
    public void Constructor_should_throw_exception_when_configuration_null()
    {
        var action = () => new SaveSubscriptionCommand(
            this.logger!.Object,
            this.validator!.Object,
            this.dal!.Object,
            null!,
            this.persister!.Object
        );
        action.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("configuration");
    }

    [Test]
    public void Constructor_should_throw_exception_when_persister_null()
    {
        var action = () => new SaveSubscriptionCommand(
            this.logger!.Object,
            this.validator!.Object,
            this.dal!.Object,
            this.configuration!.Object,
            null!
        );
        action.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("persister");
    }

    [Test]
    public async Task ExecuteAsync_should_return_error_when_request_is_null()
    {
        var command = GetCommand();
        var response = await command.ExecuteAsync(null);
        Verify(response, "model", ErrorMessages.RequestNull);
    }

    [Test]
    public async Task ExecuteAsync_should_return_error_when_request_userId_is_null()
    {
        var command = GetCommand();
        var response = await command.ExecuteAsync(new EditSubscriptionRequestModel() { UserId = null });
        Verify(response, nameof(EditSubscriptionRequestModel.UserId), string.Format(ErrorMessages.FieldEmpty, nameof(EditUserRequestModel.UserId)));
    }

    [Test]
    public async Task ExecuteAsync_should_return_error_when_request_items_is_null()
    {
        var command = GetCommand();
        var response = await command.ExecuteAsync(new EditSubscriptionRequestModel() { UserId = "123", Items = null });
        Verify(response, nameof(EditSubscriptionRequestModel.Items), string.Format(ErrorMessages.FieldEmpty, nameof(EditSubscriptionRequestModel.Items)));
    }

    [Test]
    public async Task ExecuteAsync_should_return_error_when_validator_returned_validation_error()
    {
        var validationError = ValidationResult.Fail("property", "message");
        validator!.Setup(x => x.ValidateAsync(It.IsAny<EditSubscriptionRequestModel>())).ReturnsAsync(validationError);

        var command = GetCommand();
        var response = await command.ExecuteAsync(new EditSubscriptionRequestModel() { UserId = "123", Items = [ new SubscriptionItem() ] });
        response.ValidationResult.Should().Be(validationError);
    }

    [Test]
    public async Task ExecuteAsync_should_add_series_to_subscription()
    {
        var request = new EditSubscriptionRequestModel
        {
            UserId = "user#123",
            Items =
            [
                new SubscriptionItem
                {
                    SeriesId = "11111111-1111-1111-1111-111111111111",
                    Quality = Quality.H720,
                },
               new SubscriptionItem
                {
                    SeriesId = "22222222-2222-2222-2222-222222222222",
                    Quality = Quality.H720,
                }
            ]
        };

        validator!.Setup(x => x.ValidateAsync(It.IsAny<EditSubscriptionRequestModel>())).ReturnsAsync(ValidationResult.Ok);
        await GetCommand().ExecuteAsync(request);

        // load user
        this.userDao!.Verify(x => x.LoadAsync(request.UserId), Times.Once);
        // load user feed
        this.feedDAO!.Verify(x => x.LoadUserFeedAsync(request.UserId), Times.Once);
        // load subscription
        this.subscriptionDAO!.Verify(x => x.LoadAsync(request.UserId), Times.Once);
        // load torrent file
        this.torrentFileDAO!.Verify(x => x.LoadBaseFileAsync("51439"), Times.Once);
        // load list of trackers
        this.configuration!.Verify(x => x.GetTorrentAnnounceList(usersCollection![request.UserId].TrackerId), Times.Once);
        // save torrent file for user
        this.torrentFileDAO!.Verify(x => x.SaveUserFileAsync(request.UserId, It.Is<TorrentFile>(t => t.FileName == "The.Flash.S08E13.720p.rus.LostFilm.TV.mp4")), Times.Once);
        // var user feed
        this.feedDAO.Verify(x => x.SaveUserFeedAsync(request.UserId, It.IsAny<FeedItem[]>()), Times.Once);
        // do not delete any torrent files
        this.torrentFileDAO.Verify(x => x.DeleteUserFileAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public async Task ExecuteAsync_should_replace_series_quality()
    {
        var request = new EditSubscriptionRequestModel
        {
            UserId = "user#123",
            Items =
            [
                new SubscriptionItem
                {
                    SeriesId = "22222222-2222-2222-2222-222222222222",
                    Quality = Quality.H1080,
                }
            ]
        };

        validator!.Setup(x => x.ValidateAsync(It.IsAny<EditSubscriptionRequestModel>())).ReturnsAsync(ValidationResult.Ok);
        await GetCommand().ExecuteAsync(request);

        // load user
        this.userDao!.Verify(x => x.LoadAsync(request.UserId), Times.Once);
        // load user feed
        this.feedDAO!.Verify(x => x.LoadUserFeedAsync(request.UserId), Times.Once);
        // load subscription
        this.subscriptionDAO!.Verify(x => x.LoadAsync(request.UserId), Times.Once);
        // load torrent file
        this.torrentFileDAO!.Verify(x => x.LoadBaseFileAsync("51439"), Times.Once);
        // load list of trackers
        this.configuration!.Verify(x => x.GetTorrentAnnounceList(usersCollection![request.UserId].TrackerId), Times.Once);
        // save torrent file for user
        this.torrentFileDAO!.Verify(x => x.SaveUserFileAsync(request.UserId, It.Is<TorrentFile>(t => t.FileName == "The.Flash.S08E13.720p.rus.LostFilm.TV.mp4")), Times.Once);
        // var user feed
        this.feedDAO!.Verify(x => x.SaveUserFeedAsync(request.UserId, It.IsAny<FeedItem[]>()), Times.Once);
        // do delete old torrent file
        this.torrentFileDAO!.Verify(x => x.DeleteUserFileAsync(request.UserId, "The.Flash.S08E13.720p.rus.LostFilm.TV.mp4.torrent"), Times.Once);
    }
    
    private static Stream GetTorrent(string torrentId)
        => Assembly.GetExecutingAssembly().GetManifestResourceStream($"LostFilmMonitoring.BLL.Tests.{torrentId}.torrent")!;

    private void DefineDatabaseState()
    {
        // there is a user
        this.usersCollection = new Dictionary<string, User>
        {
            { "user#123", new User("user#123", "tracker#123")  }
        };

        // and this user has a subscription
        this.subscriptionsCollection = new Dictionary<string, Subscription[]>
        {
            { "user#123", new [] { new Subscription("Series#2", Quality.H720) } }
        };

        this.feedItemsCollection = new Dictionary<string, SortedSet<FeedItem>>
        {
            { "user#123", new SortedSet<FeedItem>() { new FeedItem { Title = "Series#2_Title", Link = "user#123/The.Flash.S08E13.720p.rus.LostFilm.TV.mp4.torrent" } } }
        };

        this.seriesCollection = new Dictionary<string, Series>
        {
            {
                "Series#1",
                new Series(
                    Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    "Series#1",
                    new DateTime(2022, 5, 24, 8, 34, 11, DateTimeKind.Utc),
                    "Series#1_Title",
                    "http://tracktor.in/rssdownloader.php?id=51439",
                    "http://tracktor.in/rssdownloader.php?id=51439",
                    "http://tracktor.in/rssdownloader.php?id=51439")
            },
            {
                "Series#2",
                new Series(
                    Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    "Series#2",
                    new DateTime(2022, 5, 24, 8, 34, 11, DateTimeKind.Utc),
                    "Series#2_Title",
                    "http://tracktor.in/rssdownloader.php?id=51439",
                    "http://tracktor.in/rssdownloader.php?id=51439",
                    "http://tracktor.in/rssdownloader.php?id=51439")
            },
        };

        this.torrentFileCollection = new Dictionary<string, TorrentFile>
        {
            { "51439", new TorrentFile("51439", GetTorrent("51439")) }
        };
    }

    private void SetupStorage()
    {
        foreach(var kvp in usersCollection!)
        {
            this.userDao!.Setup(x => x.LoadAsync(kvp.Key)).ReturnsAsync(kvp.Value);
        }

        foreach(var kvp in subscriptionsCollection!)
        {
            this.subscriptionDAO!.Setup(x => x.LoadAsync(kvp.Key)).ReturnsAsync(kvp.Value);
        }

        this.seriesDAO!.Setup(x => x.LoadAsync()).ReturnsAsync(seriesCollection!.Values.ToArray());

        foreach(var kvp in seriesCollection!)
        {
            this.seriesDAO!.Setup(x => x.LoadAsync(kvp.Key)).ReturnsAsync(kvp.Value);
        }

        foreach(var kvp in torrentFileCollection!)
        {
            this.torrentFileDAO!.Setup(x => x.LoadBaseFileAsync(kvp.Key)).ReturnsAsync(kvp.Value);
        }

        foreach(var kvp in feedItemsCollection!)
        {
            this.feedDAO!.Setup(x => x.LoadUserFeedAsync(kvp.Key)).ReturnsAsync(kvp.Value);
        }
    }

    private static void Verify(EditSubscriptionResponseModel response, string errorKey, string errorValue)
    {
        response.ValidationResult.Should().NotBeNull();
        response.ValidationResult.IsValid.Should().BeFalse();
        response.ValidationResult.Errors.Should().ContainSingle().Which.Should().BeEquivalentTo(new { Key = errorKey, Value = errorValue });
    }

    private SaveSubscriptionCommand GetCommand()
        => new (this.logger!.Object, this.validator!.Object, this.dal!.Object, this.configuration!.Object, this.persister!.Object);
}
