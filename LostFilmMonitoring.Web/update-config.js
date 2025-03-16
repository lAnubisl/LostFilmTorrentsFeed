const fs = require('fs');

const config = {
    baseApiUri: process.env.BASE_API_URI || "https://apilostfilmfeeddev.byalex.dev/api/",
    baseDataUri: process.env.BASE_DATA_URI || "https://datalostfilmfeeddev.byalex.dev/models/",
    imagesBaseUri: process.env.IMAGES_BASE_URI || "https://datalostfilmfeeddev.byalex.dev/images/",
    baseRssUri: process.env.BASE_RSS_URI || "https://datalostfilmfeeddev.byalex.dev/rssfeeds/"
};

const configContent = `const config = ${JSON.stringify(config, null, 2)};`;
fs.writeFileSync('config.js', configContent); 