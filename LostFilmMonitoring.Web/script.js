// Configuration and constants
const baseApiUri = config.baseApiUri;
const baseDataUri = config.baseDataUri;
const imagesBaseUri = config.imagesBaseUri;
const baseRssUri = config.baseRssUri;

/**
 * Initializes the user subscription page
 */
const initMySubscriptionPage = () => {
    const userId = getCookie("UserId");
    const linkElement = document.getElementById("rssLink");
    const spanElement = document.getElementById("userIdSpan");
    const link = `${baseRssUri}${userId}.xml`;
    
    linkElement.textContent = link;
    spanElement.textContent = userId;
    linkElement.setAttribute('href', link);
    
    loadUser(userId);
    loadFeedItems(link);
};

/**
 * Loads RSS feed items from the provided link
 * @param {string} link - The RSS feed URL
 */
const loadFeedItems = async (link) => {
    try {
        const data = await fetchXML(link);
        const parser = new DOMParser();
        const xmlDoc = parser.parseFromString(data, "text/xml");
        const items = xmlDoc.children[0].children[0].children;
        const rssItemsContainer = document.getElementById("rssItems");
        
        Array.from(items).forEach(item => {
            const title = item.children[0].textContent;
            const link = item.children[1].textContent;
            const date = item.children[2].textContent;
            rssItemsContainer.appendChild(
                createRssItemElement(title, new Date(date).toLocaleString(), link)
            );
        });
    } catch (error) {
        showError(`Failed to load feed items: ${error.message}`);
    }
};

/**
 * Loads user data based on user ID
 * @param {string} userId - The user's ID
 */
const loadUser = async (userId) => {
    try {
        const model = { UserId: userId };
        const data = await postJSONAsync(`${baseApiUri}GetUserFunction`, model);
        document.getElementById('TrackerId').value = data.TrackerId;
    } catch (error) {
        showError(`Failed to load user data: ${error.message}`);
    }
};

/**
 * Loads items from the data source
 */
const loadItems = async () => {
    try {
        const data = await fetchJSONAsync(`${baseDataUri}index.json`);
        const imageMap = new Map(
            data.Items.map(item => [item.Name, item.ImageFileName])
        );
        
        // Add items to their respective containers
        appendItemsToContainer("series-group-items-24-hours", data.Last24HoursItems, imageMap, createElement);
        appendItemsToContainer("series-group-items-7-days", data.Last7DaysItems, imageMap, createElement);
        appendItemsToContainer("series-group-items-older", data.OlderItems, imageMap, createElementOldItem);
        
        addClickEvents();
        getCurrentSelections();
    } catch (error) {
        showError(`Failed to load items: ${error.message}`);
    }
};

/**
 * Helper function to append items to a container
 * @param {string} containerId - The ID of the container element
 * @param {Array} items - The array of items to append
 * @param {Map} imageMap - Map of image names
 * @param {Function} createElementFunc - Function to create elements
 */
const appendItemsToContainer = (containerId, items, imageMap, createElementFunc) => {
    const container = document.getElementById(containerId);
    items.forEach(item => {
        container.appendChild(createElementFunc(item, imageMap));
    });
};

/**
 * Escapes special characters in a string
 * @param {string} str - The string to escape
 * @returns {string} The escaped string
 */
const escape = (str) => {
    return str
        .replaceAll('«', "&laquo;")
        .replaceAll('»', "&raquo;");
};

/**
 * Saves user subscription changes
 */
const saveChanges = async () => {
    const saveButton = document.getElementById("fixed-save-changes-link");
    buttonChangeStyleToProcessing(saveButton);
    
    const selectedItems = Array.from(document.getElementsByClassName("series-item-selected"));
    const items = selectedItems.map(item => ({
        SeriesName: escape(item.querySelector('input').value),
        Quality: item.querySelector('select').value
    }));

    const userId = getCookie("UserId");
    if (!userId) {
        showError("Зарегистрируйтесь или выполните вход, чтобы сохранить подписки");
        buttonChangeStyleToError(saveButton);
        hideButtonAfterDelay(saveButton);
        return;
    }

    const model = { UserId: userId, Items: items };

    try {
        const data = await postJSONAsync(`${baseApiUri}SubscriptionUpdateFunction`, model);
        
        if (data && data.ValidationResult && data.ValidationResult.IsValid === false) {
            throw new Error("Validation failed");
        }
        
        buttonChangeStyleToCompleted(saveButton);
    } catch (error) {
        showError("Что-то пошло не так");
        buttonChangeStyleToError(saveButton);
    } finally {
        hideButtonAfterDelay(saveButton);
    }
};

/**
 * Hides a button after a delay
 * @param {HTMLElement} button - The button to hide
 * @param {number} delay - Delay in milliseconds
 */
const hideButtonAfterDelay = (button, delay = 3000) => {
    setTimeout(() => {
        button.style.display = 'none';
        buttonChangeStyleToDefault(button);
    }, delay);
};

/**
 * Displays an error message to the user
 * @param {string} message - The error message
 */
const showError = (message) => {
    console.error(message);
    alert(message);
};

/**
 * Resets button style to default
 * @param {HTMLElement} button - The button element
 */
const buttonChangeStyleToDefault = (button) => {
    button.style.backgroundColor = ""; // Reset to default color
    button.innerHTML = '<i class="fas fa-save"></i>'; // Reset to save icon
};

/**
 * Changes button style to processing state
 * @param {HTMLElement} button - The button element
 */
const buttonChangeStyleToProcessing = (button) => {
    button.style.backgroundColor = "#FF9800"; // Orange color
    button.innerHTML = '<i class="fas fa-spinner fa-spin"></i>'; // Processing icon
};

/**
 * Changes button style to completed state
 * @param {HTMLElement} button - The button element
 */
const buttonChangeStyleToCompleted = (button) => {
    button.style.backgroundColor = "#4CAF50"; // Green color
    button.innerHTML = '<i class="fas fa-check"></i>'; // Completed icon
};

/**
 * Changes button style to error state
 * @param {HTMLElement} button - The button element
 */
const buttonChangeStyleToError = (button) => {
    button.style.backgroundColor = "#F44336"; // Red color
    button.innerHTML = '<i class="fas fa-exclamation-triangle"></i>'; // Error icon
};

/**
 * Gets current user selections
 */
const getCurrentSelections = async () => {
    const userId = getCookie("UserId");
    if (!userId) return;

    try {
        const data = await fetchJSONAsync(`${baseDataUri}subscription_${userId}.json`);
        
        data.forEach(item => {
            const seriesElement = document.getElementById(createElementId(item.SeriesName));
            if (seriesElement) {
                seriesElement.classList.add("series-item-selected");
                const selectBoxElement = document.getElementById(createSelectBoxElementId(item.SeriesName));
                if (selectBoxElement) {
                    selectBoxElement.value = item.Quality;
                }
            }
        });
    } catch (error) {
        showError(`Failed to get current selections: ${error.message}`);
    }
};

/**
 * Creates an RSS item element
 * @param {string} title - The item title
 * @param {string} date - The formatted date
 * @param {string} link - The item link
 * @returns {HTMLElement} The created element
 */
const createRssItemElement = (title, date, link) => {
    return createElementFromHTML(`
        <div>
            <span>${date}</span>
            <a href="${link}" rel="noreferrer">${title}</a>
        </div>
    `);
};

/**
 * Gets the image name for a series
 * @param {string} name - The series name
 * @param {Map} imagesMap - Map of image names
 * @returns {string} The image name
 */
const getImageName = (name, imagesMap) => {
    if (imagesMap.has(name)) {
        const imageFileName = imagesMap.get(name);
        if (imageFileName && imageFileName !== "") {
            return imageFileName.substring(0, imageFileName.indexOf('.'));
        }
    }
    return name.replace(/:/g, "_");
};

/**
 * Creates a series element
 * @param {string} name - The series name
 * @param {Map} imagesMap - Map of image names
 * @returns {HTMLElement} The created element
 */
const createElement = (name, imagesMap) => {
    const displayName = getDisplayName(name);
    const imageName = getImageName(name, imagesMap);
    const elementId = createElementId(name);
    const selectBoxId = createSelectBoxElementId(name);
    
    return createElementFromHTML(`
        <div class="series-item" id="${elementId}">
            <input type="hidden" value="${name}">
            <div class="series-title">${displayName}</div>
            <img src="${imagesBaseUri}${imageName}.jpg" width="120" height="160">
            <a onclick="window.event.stopPropagation();" target="_blank" href="https://www.google.by/search?q=Трейлер ${displayName}">Смотреть трейлер</a>
            <select id="${selectBoxId}" onclick="window.event.stopPropagation();">
                <option>SD</option>
                <option>1080</option>
                <option>MP4</option>
            </select>
        </div>
    `);
};

/**
 * Creates an old series element
 * @param {string} name - The series name
 * @param {Map} imagesMap - Map of image names
 * @returns {HTMLElement} The created element
 */
const createElementOldItem = (name, imagesMap) => {
    const displayName = getDisplayName(name);
    const elementId = createElementId(name);
    const selectBoxId = createSelectBoxElementId(name);
    
    return createElementFromHTML(`
        <div class="series-item" id="${elementId}">
            <input type="hidden" value="${name}">
            <div class="series-title">${displayName}</div>
            <select id="${selectBoxId}" onclick="window.event.stopPropagation();">
                <option>SD</option>
                <option>1080</option>
                <option>MP4</option>
            </select>
            <a onclick="window.event.stopPropagation();" target="_blank" href="https://www.google.by/search?q=Трейлер ${displayName}">Смотреть трейлер</a>
        </div>
    `);
};

/**
 * Creates a select box element ID
 * @param {string} name - The series name
 * @returns {string} The select box element ID
 */
const createSelectBoxElementId = (name) => {
    return `selectBox_${createElementId(name)}`;
};

/**
 * Creates an element ID from a name
 * @param {string} name - The name to convert to an ID
 * @returns {string} The element ID
 */
const createElementId = (name) => {
    return name.replace(/[\W_]+/g, "_");
};

/**
 * Gets a cookie value (actually from localStorage)
 * @param {string} key - The key to retrieve
 * @returns {string} The cookie value
 */
const getCookie = (key) => {
    return localStorage.getItem(key) ?? "";
};

/**
 * Sets a cookie value (actually in localStorage)
 * @param {string} key - The key to set
 * @param {string} value - The value to store
 */
const setCookie = (key, value) => {
    localStorage.setItem(key, value);
};

/**
 * Gets the display name from a series name
 * @param {string} name - The full series name
 * @returns {string} The display name
 */
const getDisplayName = (name) => {
    const parenIndex = name.indexOf(" (");
    return parenIndex > -1 ? name.substr(0, parenIndex) : name;
};

/**
 * Adds click event listeners to series items
 */
const addClickEvents = () => {
    // Add click events to series items
    document.querySelectorAll('.series-item').forEach(item => {
        item.addEventListener('click', () => {
            item.classList.toggle("series-item-selected");
            showSaveSubscriptionsButton();
        });

        const selectElement = item.querySelector('select');
        if (selectElement) {
            selectElement.addEventListener('change', () => {
                showSaveSubscriptionsButton();
            });
        }
    });
    
    // Global click handler for save button
    document.addEventListener('click', (event) => {
        const saveButton = document.getElementById("fixed-save-changes-link");
        if (saveButton.style.display === 'none' && !event.target.closest('#fixed-save-changes-link')) {
            showSaveSubscriptionsButton();
        }
    });
};

/**
 * Shows the save subscriptions button
 */
const showSaveSubscriptionsButton = () => {
    const fixedButton = document.getElementById("fixed-save-changes-link");
    fixedButton.style.display = 'flex';
};

/**
 * Creates an element from an HTML string
 * @param {string} htmlString - The HTML string
 * @returns {HTMLElement} The created element
 */
const createElementFromHTML = (htmlString) => {
    const div = document.createElement('div');
    div.innerHTML = htmlString.trim();
    return div.firstChild;
};

/**
 * Fetches XML data from a URL
 * @param {string} url - The URL to fetch from
 * @returns {Promise<string>} The XML data
 */
const fetchXML = (url) => {
    return new Promise((resolve, reject) => {
        const xhr = new XMLHttpRequest();
        xhr.open('GET', url, true);
        xhr.responseType = 'text';
        xhr.onload = () => {
            if (xhr.status === 200) {
                resolve(xhr.response);
            } else {
                reject(new Error(`Status: ${xhr.status}`));
            }
        };
        xhr.onerror = () => reject(new Error('Network error'));
        xhr.send();
    });
};

/**
 * Fetches JSON data from a URL
 * @param {string} url - The URL to fetch from
 * @returns {Promise<Object>} The JSON data
 */
const fetchJSONAsync = (url) => {
    return new Promise((resolve, reject) => {
        const xhr = new XMLHttpRequest();
        xhr.open('GET', url, true);
        xhr.responseType = 'json';
        xhr.onload = () => {
            if (xhr.status === 200) {
                resolve(xhr.response);
            } else {
                reject(new Error(`Status: ${xhr.status}`));
            }
        };
        xhr.onerror = () => reject(new Error('Network error'));
        xhr.send();
    });
};

/**
 * Legacy getJSON function for backward compatibility
 * @param {string} url - The URL to fetch from
 * @param {Function} callback - The callback function
 */
const getJSON = (url, callback) => {
    fetchJSONAsync(url)
        .then(data => callback(null, data))
        .catch(error => callback(error.message, null));
};

/**
 * Posts JSON data to a URL
 * @param {string} url - The URL to post to
 * @param {Object} model - The data to post
 * @returns {Promise<Object>} The response data
 */
const postJSONAsync = (url, model) => {
    return new Promise((resolve, reject) => {
        const xhr = new XMLHttpRequest();
        xhr.open("POST", url, true);
        xhr.setRequestHeader("Content-Type", "application/json");
        xhr.responseType = 'json';
        xhr.onload = () => {
            if (xhr.status === 200) {
                resolve(xhr.response);
            } else {
                reject(new Error(`Status: ${xhr.status}`));
            }
        };
        xhr.onerror = () => reject(new Error('Network error'));
        xhr.send(JSON.stringify(model));
    });
};

/**
 * Legacy postJSON function for backward compatibility
 * @param {string} url - The URL to post to
 * @param {Object} model - The data to post
 * @param {Function} callback - The callback function
 */
const postJSON = (url, model, callback) => {
    postJSONAsync(url, model)
        .then(data => callback(null, data))
        .catch(error => callback(error.message, null));
};

/**
 * Registers a new user
 */
const register = async () => {
    try {
        const userId = getCookie("UserId");
        const trackerId = document.getElementById('TrackerId').value;
        
        const model = { UserId: userId, TrackerId: trackerId };
        const data = await postJSONAsync(`${baseApiUri}RegisterFunction`, model);
        
        if (data.UserId) {
            setCookie("UserId", data.UserId);
            window.location.href = "/index.html";
            return;
        }
        
        throw new Error("Registration failed");
    } catch (error) {
        showError(`Registration failed: ${error.message}`);
    }
};

/**
 * Signs in an existing user
 */
const signIn = async () => {
    try {
        const userId = document.getElementById('userId').value;
        const model = { UserId: userId };
        
        const data = await postJSONAsync(`${baseApiUri}SignInFunction`, model);
        
        if (data.Success) {
            setCookie("UserId", userId);
            window.location.href = "/index.html";
            return;
        }
        
        throw new Error("Sign in failed");
    } catch (error) {
        showError(`Sign in failed: ${error.message}`);
    }
};

/**
 * Checks if a user is logged in
 * @returns {boolean} True if user is logged in
 */
const loggedInUser = () => {
    return getCookie("UserId") !== "";
};

/**
 * Initializes the menu based on login status
 */
const initMenu = () => {
    if (loggedInUser()) {
        document.getElementById("my-subscription-link").style.display = 'inline-block';
    } else {
        document.getElementById("sign-in-link").style.display = 'inline-block';
        document.getElementById("sign-up-link").style.display = 'inline-block';
    }
};