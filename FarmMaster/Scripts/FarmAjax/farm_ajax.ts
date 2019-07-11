enum FarmAjaxMessageType {
    None,
    Warning,
    Information,
    Error
}

class FarmAjaxMessageResponse {
    type: FarmAjaxMessageType;
    message: string;

    constructor(type: FarmAjaxMessageType, message: string) {
        this.type = type;
        this.message = message;
    }

    public populateMessageBox(box: HTMLElement) {
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
    }
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
        .fail((error) => onDone(new FarmAjaxMessageResponse(
            FarmAjaxMessageType.Error, 
            JSON.stringify(error)
        )));
    }
}