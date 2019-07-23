var FarmAjaxMessageType;
(function (FarmAjaxMessageType) {
    FarmAjaxMessageType[FarmAjaxMessageType["None"] = 0] = "None";
    FarmAjaxMessageType[FarmAjaxMessageType["Information"] = 1] = "Information";
    FarmAjaxMessageType[FarmAjaxMessageType["Warning"] = 2] = "Warning";
    FarmAjaxMessageType[FarmAjaxMessageType["Error"] = 3] = "Error";
})(FarmAjaxMessageType || (FarmAjaxMessageType = {}));
var FarmAjaxMessageFormat;
(function (FarmAjaxMessageFormat) {
    FarmAjaxMessageFormat[FarmAjaxMessageFormat["Default"] = 0] = "Default";
    FarmAjaxMessageFormat[FarmAjaxMessageFormat["UnorderedList"] = 1] = "UnorderedList";
})(FarmAjaxMessageFormat || (FarmAjaxMessageFormat = {}));
var FarmAjaxMessageResponse = (function () {
    function FarmAjaxMessageResponse(type, message, format) {
        this.messageType = type;
        this.message = message;
        this.messageFormat = format;
    }
    FarmAjaxMessageResponse.prototype.populateMessageBox = function (box) {
        box.classList.remove("info", "error", "warning");
        switch (this.messageType) {
            case FarmAjaxMessageType.Error:
                box.classList.add("error");
                break;
            case FarmAjaxMessageType.Information:
                box.classList.add("info");
                break;
            case FarmAjaxMessageType.Warning:
                box.classList.add("warning");
                break;
            default: break;
        }
        box.classList.add("visible");
        switch (this.messageFormat) {
            case FarmAjaxMessageFormat.Default:
                box.innerHTML = this.message;
                break;
            case FarmAjaxMessageFormat.UnorderedList:
                box.innerHTML = "";
                var ul = document.createElement("ul");
                box.appendChild(ul);
                for (var _i = 0, _a = this.message.split("\n"); _i < _a.length; _i++) {
                    var item = _a[_i];
                    var li = document.createElement("li");
                    li.innerText = item;
                    ul.appendChild(li);
                }
                break;
            default: break;
        }
    };
    return FarmAjaxMessageResponse;
}());
var FarmAjaxMessageAndValueResponse = (function () {
    function FarmAjaxMessageAndValueResponse(message, value) {
        this.message = message;
        this.value = value;
    }
    return FarmAjaxMessageAndValueResponse;
}());
var FarmAjax = (function () {
    function FarmAjax() {
    }
    FarmAjax.postWithMessageResponse = function (url, data, onDone) {
        FarmAjax
            .doAjax(url, data)
            .done(function (response) {
            onDone(new FarmAjaxMessageResponse(response.messageType, response.message, response.messageFormat));
        })
            .fail(function (error) { return onDone(new FarmAjaxMessageResponse(FarmAjaxMessageType.Error, JSON.stringify(error), FarmAjaxMessageFormat.UnorderedList)); });
    };
    FarmAjax.postWithMessageAndValueResponse = function (url, data, onDone) {
        FarmAjax
            .doAjax(url, data)
            .done(function (response) {
            var message = new FarmAjaxMessageResponse(response.message.messageType, response.message.message, response.message.messageFormat);
            onDone(new FarmAjaxMessageAndValueResponse(message, response.value));
        })
            .fail(function (error) {
            var message = new FarmAjaxMessageResponse(FarmAjaxMessageType.Error, JSON.stringify(error), FarmAjaxMessageFormat.UnorderedList);
            onDone(new FarmAjaxMessageAndValueResponse(message, null));
        });
    };
    FarmAjax.doAjax = function (url, data) {
        if (data === null)
            data = {};
        if (data.SessionToken !== undefined)
            throw "Please don't define your own 'SessionToken' field, FarmAjax will handle that for you.";
        data.SessionToken = Cookies.get("FarmMasterAuth");
        return $.ajax({
            type: "POST",
            url: url,
            contentType: "application/json",
            dataType: "json",
            data: JSON.stringify(data)
        });
    };
    return FarmAjax;
}());
//# sourceMappingURL=farm_ajax.js.map