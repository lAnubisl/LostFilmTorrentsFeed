# Frontend - Static Website

## Architecture

Vanilla JavaScript (ES6+), no frameworks. Static files hosted on Azure Blob Storage.

## Configuration

The `config.js` file contains a `config` object with environment-specific endpoints that are injected at deployment time by the `update-config.js` script. The `config.js` file in the repository should contain placeholder values, not hardcoded environment URLs.

The `update-config.js` script replaces the following config object keys with environment-specific values during deployment:
- `baseApiUri`: Azure Function API endpoint
- `baseDataUri`: Blob storage models container endpoint
- `imagesBaseUri`: Blob storage images container endpoint
- `baseRssUri`: Blob storage RSS feeds container endpoint

Example structure after deployment:

```javascript
const config = {
    baseApiUri: "https://apilostfilmfeeddev.byalex.dev/api/",
    baseDataUri: "https://datalostfilmfeeddev.byalex.dev/models/",
    imagesBaseUri: "https://datalostfilmfeeddev.byalex.dev/images/",
    baseRssUri: "https://datalostfilmfeeddev.byalex.dev/rssfeeds/"
};
```

## API Communication

```javascript
// POST to Azure Function
const postJSONAsync = (url, model) => {
    return new Promise((resolve, reject) => {
        const xhr = new XMLHttpRequest();
        xhr.open("POST", url, true);
        xhr.setRequestHeader("Content-Type", "application/json");
        xhr.responseType = 'json';
        xhr.onload = () => xhr.status === 200 ? resolve(xhr.response) : reject();
        xhr.send(JSON.stringify(model));
    });
};

// GET from blob storage
const fetchJSONAsync = (url) => {
    // Similar pattern with GET request
};
```

## Authentication

- Stores user's `TrackerId` under `localStorage` key `"UserId"`
- No passwords - users authenticate via `TrackerId` from LostFilm.tv
- Helper: Read from `localStorage.getItem("UserId")` to retrieve stored user ID

## Key Files

- `index.html`: Series selection page
- `mysubscription.html`: User subscription management
- `register.html`: New user registration
- `signin.html`: Existing user sign-in
- `script.js`: All JavaScript logic
- `update-config.js`: Deployment script to inject environment URLs

## Data Flow

1. Load series list from `models/index.json`
2. Load user subscriptions from `models/subscription_{userId}.json`
3. User makes changes → POST to API
4. RSS feed available at `rssfeeds/{userId}.xml`

## Notes

- All async operations use Promises
- No build step required
- CORS configured on backend for frontend domains
