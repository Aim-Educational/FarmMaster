var FarmAjaxMessageType;
(function (FarmAjaxMessageType) {
    FarmAjaxMessageType[FarmAjaxMessageType["None"] = 0] = "None";
    FarmAjaxMessageType[FarmAjaxMessageType["Warning"] = 1] = "Warning";
    FarmAjaxMessageType[FarmAjaxMessageType["Information"] = 2] = "Information";
    FarmAjaxMessageType[FarmAjaxMessageType["Error"] = 3] = "Error";
})(FarmAjaxMessageType || (FarmAjaxMessageType = {}));
var FarmAjaxMessageResponse = (function () {
    function FarmAjaxMessageResponse() {
    }
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
            .fail(function (error) { return onDone({
            message: JSON.stringify(error),
            type: FarmAjaxMessageType.Error
        }); });
    };
    return FarmAjax;
}());
//# sourceMappingURL=farm_ajax.js.map