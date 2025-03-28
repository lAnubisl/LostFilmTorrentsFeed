// Configuration and constants
const baseApiUri = config.baseApiUri;
const baseDataUri = config.baseDataUri;
const imagesBaseUri = config.imagesBaseUri;
const baseRssUri = config.baseRssUri;

/**
 * Creates an element from an HTML string
 * @param {string} htmlString - The HTML string
 * @returns {HTMLElement} The created element
 */
const createElementFromHTML = (htmlString) => {
    const div = document.createElement('div');
    div.innerHTML = htmlString.trim();
    
    // Change this to div.childNodes to support multiple top-level nodes.
    return div.firstChild;
};

/**
 * Gets a cookie value (actually from localStorage)
 * @param {string} key - The key to retrieve
 * @returns {string} The cookie value
 */
const getCookie = (key) => {
    return localStorage.getItem(key);
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
 * Escapes special characters in a string
 * @param {string} str - The string to escape
 * @returns {string} The escaped string
 */
const escapeHtml = (str) => {
    return str
        .replaceAll('«', "&laquo;")
        .replaceAll('»', "&raquo;");
};

/**
 * Fetches XML data from a URL
 * @param {string} url - The URL to fetch from
 * @returns {Promise<string>} The XML data
 */
const fetchXML = async (url) => {
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`HTTP error! Status: ${response.status}`);
        }
        return await response.text();
    } catch (error) {
        console.error("Error fetching XML:", error);
        throw error;
    }
};

/**
 * Fetches JSON data from a URL
 * @param {string} url - The URL to fetch from
 * @returns {Promise<Object>} The JSON data
 */
const fetchJSONAsync = async (url) => {
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`HTTP error! Status: ${response.status}`);
        }
        return await response.json();
    } catch (error) {
        console.error("Error fetching JSON:", error);
        throw error;
    }
};

/**
 * Legacy getJSON function for backward compatibility
 * @param {string} url - The URL to fetch from
 * @param {Function} callback - The callback function
 */
const getJSON = (url, callback) => {
    fetchJSONAsync(url)
        .then(data => callback(data))
        .catch(error => console.error("Error in getJSON:", error));
};

/**
 * Posts JSON data to a URL
 * @param {string} url - The URL to post to
 * @param {Object} model - The data to post
 * @returns {Promise<Object>} The response data
 */
const postJSONAsync = async (url, model) => {
    try {
        const response = await fetch(url, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(model)
        });
        
        if (!response.ok) {
            throw new Error(`HTTP error! Status: ${response.status}`);
        }
        
        return await response.json();
    } catch (error) {
        console.error("Error posting JSON:", error);
        throw error;
    }
};

/**
 * Legacy postJSON function for backward compatibility
 * @param {string} url - The URL to post to
 * @param {Object} model - The data to post
 * @param {Function} callback - The callback function
 */
const postJSON = (url, model, callback) => {
    postJSONAsync(url, model)
        .then(data => callback(data))
        .catch(error => console.error("Error in postJSON:", error));
};

/**
 * Displays an error message to the user
 * @param {string} message - The error message
 */
const showError = (message) => {
    const errorElement = document.getElementById("error-message");
    errorElement.textContent = message;
    errorElement.style.display = "block";
    
    setTimeout(() => {
        errorElement.style.display = "none";
    }, 5000);
};

/**
 * Creates an element ID from a name
 * @param {string} name - The name to convert to an ID
 * @returns {string} The element ID
 */
const createElementId = (name) => {
    return `series-item-${name.toLowerCase().replace(/[^a-z0-9]/g, '-')}`;
};

/**
 * Creates a select box element ID
 * @param {string} name - The series name
 * @returns {string} The select box element ID
 */
const createSelectBoxElementId = (name) => {
    return `select-box-${name.toLowerCase().replace(/[^a-z0-9]/g, '-')}`;
};

/**
 * Gets the display name from a series name
 * @param {string} name - The full series name
 * @returns {string} The display name
 */
const getDisplayName = (name) => {
    const parts = name.split(' / ');
    return parts.length > 1 ? parts[1] : name;
};

/**
 * Gets the image name for a series
 * @param {string} name - The series name
 * @param {Map} imagesMap - Map of image names
 * @returns {string} The image name
 */
const getImageName = (name, imagesMap) => {
    try {
        const imageFileName = imagesMap.get(name);
        return imageFileName ? `${imagesBaseUri}${imageFileName}` : `${imagesBaseUri}default.jpg`;
    } catch (error) {
        console.error(`Error getting image for ${name}:`, error);
        return `${imagesBaseUri}default.jpg`;
    }
};

/**
 * Resets button style to default
 * @param {HTMLElement} button - The button element
 */
const buttonChangeStyleToDefault = (button) => {
    button.classList.remove("fixed-save-changes-link-processing");
    button.classList.remove("fixed-save-changes-link-completed");
    button.classList.remove("fixed-save-changes-link-error");
};

/**
 * Changes button style to processing state
 * @param {HTMLElement} button - The button element
 */
const buttonChangeStyleToProcessing = (button) => {
    buttonChangeStyleToDefault(button);
    button.classList.add("fixed-save-changes-link-processing");
};

/**
 * Changes button style to completed state
 * @param {HTMLElement} button - The button element
 */
const buttonChangeStyleToCompleted = (button) => {
    buttonChangeStyleToDefault(button);
    button.classList.add("fixed-save-changes-link-completed");
};

/**
 * Changes button style to error state
 * @param {HTMLElement} button - The button element
 */
const buttonChangeStyleToError = (button) => {
    buttonChangeStyleToDefault(button);
    button.classList.add("fixed-save-changes-link-error");
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
 * Hides the counters
 */
const hideCounters = () => {
    document.querySelector('.selection-counters').style.display = 'none';
};

/**
 * Shows the counters
 */
const showCounters = () => {
    document.querySelector('.selection-counters').style.display = 'flex';
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
        <div class="rss-item">
            <div class="rss-item-title">${title}</div>
            <div class="rss-item-date">${date}</div>
            <a href="${link}" target="_blank" class="rss-item-link">Download</a>
        </div>
    `);
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
            <img src="${imageName}" alt="${displayName}" />
            <div class="series-item-content">
                <div class="series-item-title">${displayName}</div>
                <input type="hidden" value="${name}" />
                <select id="${selectBoxId}">
                    <option value="SD">SD</option>
                    <option value="1080">1080</option>
                    <option value="MP4">MP4</option>
                    <option value="SD 1080">SD 1080</option>
                    <option value="SD MP4">SD MP4</option>
                    <option value="1080 MP4">1080 MP4</option>
                    <option value="SD 1080 MP4">SD 1080 MP4</option>
                </select>
            </div>
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
            <div class="series-item-content">
                <div class="series-item-title">${displayName}</div>
                <input type="hidden" value="${name}" />
                <select id="${selectBoxId}">
                    <option value="SD">SD</option>
                    <option value="1080">1080</option>
                    <option value="MP4">MP4</option>
                    <option value="SD 1080">SD 1080</option>
                    <option value="SD MP4">SD MP4</option>
                    <option value="1080 MP4">1080 MP4</option>
                    <option value="SD 1080 MP4">SD 1080 MP4</option>
                </select>
            </div>
        </div>
    `);
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
 * Updates the selection counters for SD, 1080, and MP4 options
 */
const updateSelectionCounters = () => {
    let sdCount = 0;
    let count1080 = 0;
    let mp4Count = 0;
    
    document.querySelectorAll('.series-item-selected select').forEach(select => {
        const value = select.value;
        if (value.includes('SD')) sdCount++;
        if (value.includes('1080')) count1080++;
        if (value.includes('MP4')) mp4Count++;
    });
    
    document.querySelector('#counter-sd .counter-value').textContent = sdCount;
    document.querySelector('#counter-1080 .counter-value').textContent = count1080;
    document.querySelector('#counter-mp4 .counter-value').textContent = mp4Count;
    if (sdCount > 0 || count1080 > 0 || mp4Count > 0) {
        showCounters();
    } else {
        hideCounters();
    }
};

/**
 * Shows the save subscriptions button
 */
const showSaveSubscriptionsButton = () => {
    const fixedButton = document.getElementById("fixed-save-changes-link");
    fixedButton.style.display = 'flex';
    
    // Also show the counters
    showCounters();
};

/**
 * Adds click event listeners to series items
 */
const addClickEvents = () => {
    document.querySelectorAll('.series-item').forEach(item => {
        item.addEventListener('click', () => {
            item.classList.toggle("series-item-selected");
            showSaveSubscriptionsButton();
            updateSelectionCounters();
        });
    });
    
    document.querySelectorAll('.series-item select').forEach(select => {
        select.addEventListener('click', (event) => {
            event.stopPropagation();
        });
        
        select.addEventListener('change', () => {
            const seriesItem = select.closest('.series-item');
            if (!seriesItem.classList.contains('series-item-selected')) {
                seriesItem.classList.add('series-item-selected');
                showSaveSubscriptionsButton();
            }
            updateSelectionCounters();
        });
    });
};

/**
 * Gets current user selections
 */
const getCurrentSelections = async () => {
    const userId = getCookie("UserId");
    if (!userId) return;
    
    try {
        const data = await fetchJSONAsync(`${baseDataUri}subscription_${userId}.json`);
        
        data.Items.forEach(item => {
            const seriesElement = document.getElementById(createElementId(item.SeriesName));
            
            if (seriesElement) {
                const selectBoxElement = document.getElementById(createSelectBoxElementId(item.SeriesName));
                seriesElement.classList.add("series-item-selected");
                selectBoxElement.value = item.Quality;
            }
        });
        updateSelectionCounters();
    } catch (error) {
        showError(`Failed to get current selections: ${error.message}`);
    }
};

/**
 * Loads items from the data source
 */
const loadItems = async () => {
    try {
        const data = await fetchJSONAsync(`${baseDataUri}index.json`);
        const imageMap = new Map(
            data.Images.map(img => [img.SeriesName, img.ImageName])
        );
        
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
        document.getElementById("userNameSpan").textContent = data.UserName;
    } catch (error) {
        showError(`Failed to load user data: ${error.message}`);
    }
};

/**
 * Saves user subscription changes
 */
const saveChanges = async () => {
    const saveButton = document.getElementById("fixed-save-changes-link");
    buttonChangeStyleToProcessing(saveButton);
    
    const selectedItems = Array.from(document.getElementsByClassName("series-item-selected"));
    const items = selectedItems.map(item => ({
        SeriesName: escapeHtml(item.querySelector('input').value),
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
 * Checks if a user is logged in
 * @returns {boolean} True if user is logged in
 */
const loggedInUser = () => {
    const userId = getCookie("UserId");
    return userId !== null && userId !== undefined && userId !== "";
};

/**
 * Initializes the menu based on login status
 */
const initMenu = () => {
    const signInLink = document.getElementById("signInLink");
    const signOutLink = document.getElementById("signOutLink");
    const mySubscriptionLink = document.getElementById("mySubscriptionLink");
    
    if (loggedInUser()) {
        signInLink.style.display = "none";
        signOutLink.style.display = "block";
        mySubscriptionLink.style.display = "block";
    }
};

/**
 * Registers a new user
 */
const register = async () => {
    const userNameInput = document.getElementById("userNameInput");
    const passwordInput = document.getElementById("passwordInput");
    const confirmPasswordInput = document.getElementById("confirmPasswordInput");
    
    if (passwordInput.value !== confirmPasswordInput.value) {
        showError("Passwords do not match");
        return;
    }
    
    const model = {
        UserName: userNameInput.value,
        Password: passwordInput.value
    };
    
    try {
        const data = await postJSONAsync(`${baseApiUri}RegisterFunction`, model);
        setCookie("UserId", data.UserId);
        window.location.href = "index.html";
    } catch (error) {
        showError("Registration failed");
    }
};

/**
 * Signs in an existing user
 */
const signIn = async () => {
    const userNameInput = document.getElementById("userNameInput");
    const passwordInput = document.getElementById("passwordInput");
    
    const model = {
        UserName: userNameInput.value,
        Password: passwordInput.value
    };
    
    try {
        const data = await postJSONAsync(`${baseApiUri}SignInFunction`, model);
        
        if (data.Success) {
            setCookie("UserId", data.UserId);
            window.location.href = "index.html";
        } else {
            showError("Invalid username or password");
        }
    } catch (error) {
        showError("Sign in failed");
    }
};

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

initMenu();
initMySubscriptionPage();