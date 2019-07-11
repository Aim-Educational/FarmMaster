var FarmAjaxMessageType;
(function (FarmAjaxMessageType) {
    FarmAjaxMessageType[FarmAjaxMessageType["None"] = 0] = "None";
    FarmAjaxMessageType[FarmAjaxMessageType["Warning"] = 1] = "Warning";
    FarmAjaxMessageType[FarmAjaxMessageType["Information"] = 2] = "Information";
    FarmAjaxMessageType[FarmAjaxMessageType["Error"] = 3] = "Error";
})(FarmAjaxMessageType || (FarmAjaxMessageType = {}));
var FarmAjaxMessageResponse = (function () {
    function FarmAjaxMessageResponse(type, message) {
        this.type = type;
        this.message = message;
    }
    FarmAjaxMessageResponse.prototype.populateMessageBox = function (box) {
        box.classList.remove("info", "error", "warning");
        switch (this.type) {
            case FarmAjaxMessageType.Error:
                box.classList.add("error");
                break;
            case FarmAjaxMessageType.Information:
                box.classList.add("info");
                break;
            default: break;
        }
        box.classList.add("visible");
        box.innerHTML = this.message;
    };
    return FarmAjaxMessageResponse;
}());
var FarmAjax = (function () {
    function FarmAjax() {
    }
    FarmAjax.postWithMessageResponse = function (url, data, onDone) {
        if (data.sessionToken !== undefined)
            throw "Please don't define your own 'sessionToken' field, FarmAjax will handle that for you.";
        data.sessionToken = Cookies.get("FarmMasterAuth");
        $.ajax({
            type: "POST",
            url: url,
            contentType: "application/json",
            dataType: "json",
            data: JSON.stringify(data)
        })
            .done(function (response) {
            onDone(response);
        })
            .fail(function (error) { return onDone(new FarmAjaxMessageResponse(FarmAjaxMessageType.Error, JSON.stringify(error))); });
    };
    return FarmAjax;
}());
//# sourceMappingURL=farm_ajax.js.map