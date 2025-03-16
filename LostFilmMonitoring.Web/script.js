var baseApiUri = config.baseApiUri;
var baseDataUri = config.baseDataUri;
var imagesBaseUri = config.imagesBaseUri;
var baseRssUri = config.baseRssUri;

var initMySubscriptionPage = function() {
    var userId = getCookie("UserId");
    var linkElement = document.getElementById("rssLink");
    var spanElement = document.getElementById("userIdSpan");
    var link = baseRssUri + userId + ".xml";
    linkElement.innerHTML = link;
    spanElement.innerHTML = userId;
    linkElement.setAttribute('href', link);
    loadUser(userId);
    loadFeedItems(link);
}

var loadFeedItems = function(link) {
    getXML(link, function(err, data) {
        if (err !== null) {
            alert('Something went wrong: ' + err);
        } else {
            var parser = new DOMParser();
            var xmlDoc = parser.parseFromString(data,"text/xml");
            var items = xmlDoc.children[0].children[0].children;
            for (var i = 0; i < items.length; i++) {
                var title = items[i].children[0].innerHTML;
                var link = items[i].children[1].innerHTML;
                var date = items[i].children[2].innerHTML;
                document.getElementById("rssItems").appendChild(createRssItemElement(title, new Date(date).toLocaleString(), link));
            }
        }
    });
}

var loadUser = function(userId) {
    var model = {
        "UserId" : userId
    };

    postJSON(baseApiUri + "GetUserFunction", model, function(err, data) {
        if (err !== null) {
            alert('Something went wrong: ' + err);
        } else {
            document.getElementById('TrackerId').value = data.TrackerId;
        }
    });
}

var loadItems = function() {
    getJSON(baseDataUri + 'index.json',
        function(err, data) {
        if (err !== null) {
            alert('Something went wrong: ' + err);
        } else {
            var map = new Map();
            data.Items.forEach(item => {
                map.set(item.Name, item.ImageFileName);
            });

            data.Last24HoursItems.forEach(item => {
                document.getElementById("series-group-items-24-hours").appendChild(createElement(item, map));
            });
            data.Last7DaysItems.forEach(item => {
                document.getElementById("series-group-items-7-days").appendChild(createElement(item, map));
            });
            data.OlderItems.forEach(item => {
                document.getElementById("series-group-items-older").appendChild(createElementOldItem(item, map));
            });

            addClickEvents();
            getCurrentSelections();
        }
    });
}

var escape = function(str) {
	return str
		.replaceAll('«', "&laquo;")
		.replaceAll('»', "&raquo;");
}

var saveChanges = function() {
    hideSaveSubscriptionsButton();
    document.getElementById("savingSpan").style.display = 'inline-block';
    var items = [];
    Array.from(document.getElementsByClassName("series-item-selected")).forEach(item => {
        items.push({
            SeriesName: escape(item.childNodes[0].value),
            Quality: item.childNodes[4].value
        })
    });

    var model = {
        UserId: getCookie("UserId"),
        Items: items
    };

    postJSON(baseApiUri + "SubscriptionUpdateFunction", model, function(err, data) {
        if (err !== null) {
            alert('Something went wrong: ' + err);
        } else {
            document.getElementById("savingSpan").style.display = 'none';
            document.getElementById("savedSpan").style.display = 'inline-block';
            setTimeout(() => {
                document.getElementById("savedSpan").style.display = 'none';
              }, 1000);
        }
    });
}

var getCurrentSelections = function() {
    var uid = getCookie("UserId");
    if (uid == "") return;

    getJSON(baseDataUri + "subscription_" + uid + ".json",
        function(err, data) {
        if (err !== null) {
            alert('Something went wrong: ' + err);
        } else {
            data.forEach(item => {
                var seriesElement = document.getElementById(createElementId(item.SeriesName));
                seriesElement.classList.add("series-item-selected");
                var selectBoxElement = document.getElementById(createSelectBoxElementId(item.SeriesName));
                selectBoxElement.value = item.Quality;
            });
        }
    });
}

var createRssItemElement = function(title, date, link){
    return createElementFromHTML(
    "<div>" +
        "<span>" + date + "</span>" +
        "<a href=\"" + link + "\" rel=\"noreferrer\">" + title + "</a>"+
    "</div>"
    );
}

var getImageName = function(name, imagesMap) {
	if (imagesMap.has(name)) {
		var imageFileName = imagesMap.get(name);
		if (imageFileName && imageFileName != "")
		{
			return imageFileName.substring(0, imageFileName.indexOf('.'));
		}
	}
    return name.replace(/:/g, "_");
}

var createElement = function(name, imagesMap) {
    var displayName = getDisplayName(name);
    var imageName = getImageName(name, imagesMap);
    var elementId = createElementId(name);
    var selectBoxId = createSelectBoxElementId(name);
    
    return createElementFromHTML(
        "<div class=\"series-item\" id=" + elementId + ">" +
            "<input type=\"hidden\" value=\"" + name + "\">" +
            "<div class=\"series-title\">" + displayName + "</div>" +
            "<img src=\"" + imagesBaseUri + imageName + ".jpg\" width=\"120\" height=\"160\">" +
            "<a onclick=\"window.event.stopPropagation();\" target=\"_blank\" href=\"https://www.google.by/search?q=Трейлер " + displayName + "\">Смотреть трейлер</a>" +
            "<select id=\"" + selectBoxId + "\" onclick=\"window.event.stopPropagation();\">" +
                "<option>SD</option>" +
                "<option>1080</option>" +
                "<option>MP4</option>" +
            "</select>" +
        "</div>"
    );
}

var createElementOldItem = function(name, imagesMap) {
    var displayName = getDisplayName(name);
    var elementId = createElementId(name);
    var selectBoxId = createSelectBoxElementId(name);
    
    return createElementFromHTML(
        "<div class=\"series-item\" id=" + elementId + ">" +
            "<input type=\"hidden\" value=\"" + name + "\">" +
            "<div class=\"series-title\">" + displayName + "</div>" +
            "<select id=\"" + selectBoxId + "\" onclick=\"window.event.stopPropagation();\">" +
                "<option>SD</option>" +
                "<option>1080</option>" +
                "<option>MP4</option>" +
            "</select>" +
            "<a onclick=\"window.event.stopPropagation();\" target=\"_blank\" href=\"https://www.google.by/search?q=Трейлер " + displayName + "\">Смотреть трейлер</a>" +
        "</div>"
    );
}

var createSelectBoxElementId = function(name) {
    return "selectBox_" + createElementId(name);
}

var createElementId = function(name) {
    return name.replace(/[\W_]+/g,"_");;
}

var getCookie = function(cname) {
    return localStorage.getItem(cname) ?? "";
}

var setCookie = function(cname, cvalue, exdays) {
    localStorage.setItem(cname, cvalue);
}

var getDisplayName = function(name) {
    return name.substr(0, name.indexOf(" ("));
}

var addClickEvents = function() {
    document.querySelectorAll('.series-item').forEach(item => {
        item.addEventListener('click', event => {
            item.classList.toggle("series-item-selected");
            showSaveSubscriptionsButton();
        });

        item.getElementsByTagName("select")[0].addEventListener('change', event => {
            showSaveSubscriptionsButton();
        });
    });
}

var showSaveSubscriptionsButton = function() {
    document.getElementById("save-changes-link").style.display = 'inline-block';
}

var hideSaveSubscriptionsButton = function() {
    document.getElementById("save-changes-link").style.display = 'none';
}

var createElementFromHTML = function(htmlString) {
    var div = document.createElement('div');
    div.innerHTML = htmlString.trim();
    return div.firstChild;
}

var getXML = function(url, callback) {
    var xhr = new XMLHttpRequest();
    xhr.open('GET', url, true);
    xhr.responseType = 'text';
    xhr.onload = function() {
        var status = xhr.status;
        if (status === 200) {
            callback(null, xhr.response);
        } else {
            callback(status, xhr.response);
        }
    };
    xhr.send();
};

var getJSON = function(url, callback) {
    var xhr = new XMLHttpRequest();
    xhr.open('GET', url, true);
    xhr.responseType = 'json';
    xhr.onload = function() {
        var status = xhr.status;
        if (status === 200) {
            callback(null, xhr.response);
        } else {
            callback(status, xhr.response);
        }
    };
    xhr.send();
};

var postJSON = function(url, model, callback){
    let xhr = new XMLHttpRequest();
    xhr.open("POST", url, true);
    xhr.setRequestHeader("Content-Type", "application/json");
    xhr.responseType = 'json';
    xhr.onreadystatechange = function () {
        if (xhr.readyState === 4 && xhr.status === 200) {
            callback(null, xhr.response);
        }
    };

    var data = JSON.stringify(model);
    xhr.send(data);
}

var register = function() {
    var model = {
        "UserId" : getCookie("UserId"),
        "TrackerId" : document.getElementById('TrackerId').value
    };

    postJSON(baseApiUri + "RegisterFunction", model, function(err, data) {
        if (err !== null) {
            alert('Something went wrong: ' + err);
        } else {
            if (data.UserId != "") {
                setCookie("UserId", data.UserId, 365);
                document.location.href = "/index.html";
                return;
            }

            alert('Something went wrong: ');
        }
    });
}

var signIn = function() {
    var model = {
        "UserId" : document.getElementById('userId').value
    };

    postJSON(baseApiUri + "SignInFunction", model, function(err, data) {
        if (err !== null) {
            alert('Something went wrong: ' + err);
        } else {
            if (data.Success) {
                setCookie("UserId", document.getElementById('userId').value, 265);
                document.location.href = "/index.html";
                return;
            }

            alert('Something went wrong: ');
        }
    });
}

var initMenu = function() {
    if (getCookie("UserId") == "") {
        document.getElementById("sign-in-link").style.display = 'inline-block';
        document.getElementById("sign-up-link").style.display = 'inline-block';
    } else {
        document.getElementById("my-subscription-link").style.display = 'inline-block';
    }
}