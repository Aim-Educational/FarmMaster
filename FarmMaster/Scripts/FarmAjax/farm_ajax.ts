enum FarmAjaxMessageType {
    None,
    Warning,
    Information,
    Error
}

class FarmAjaxMessageResponse {
    type: FarmAjaxMessageType;
    message: string;
}

class FarmAjax {
    static postWithMessageResponse(url: string, data: any, onDone: (response: FarmAjaxMessageResponse) => void) {
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
        .done(function (response: FarmAjaxMessageResponse) {
            onDone(response);
        })
        .fail((error) => onDone({
            message: JSON.stringify(error),
            type: FarmAjaxMessageType.Error
        }));
    }
}