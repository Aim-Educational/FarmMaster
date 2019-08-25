import { getCookie } from "./cookies.js"

export enum FarmAjaxMessageType {
    None,
    Information,
    Warning,
    Error
}

export enum FarmAjaxMessageFormat {
    Default,
    UnorderedList
}

export class FarmAjaxMessageResponse {
    messageFormat: FarmAjaxMessageFormat;
    messageType: FarmAjaxMessageType;
    message: string;

    constructor(type: FarmAjaxMessageType, message: string, format: FarmAjaxMessageFormat) {
        this.messageType = type;
        this.message = message;
        this.messageFormat = format;
    }

    public populateMessageBox(box: HTMLElement) {
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

                let ul = document.createElement("ul");
                box.appendChild(ul);

                for (let item of this.message.split("\n")) {
                    let li = document.createElement("li");
                    li.innerText = item;
                    ul.appendChild(li);
                }
                break;

            default: break;
        }
    }
}

export class FarmAjaxMessageAndValueResponse<T> extends FarmAjaxMessageResponse {
    public value: T | null;

    constructor(message: FarmAjaxMessageResponse, value: T | null) {
        super(message.messageType, message.message, message.messageFormat);
        this.value = value;
    }
}

export class FarmAjaxGenericValue<T> {
    public value: T;

    constructor(value: T) {
        this.value = value;
    }
}

export class FarmAjax {
    static postWithMessageResponse(url: string, data: any, onDone: (response: FarmAjaxMessageResponse) => void) {
        FarmAjax
            .doAjax(url, data)
            .done(function (response: FarmAjaxMessageResponse) {
                onDone(new FarmAjaxMessageResponse(
                    response.messageType,
                    response.message,
                    response.messageFormat
                ));
            })
            .fail((error) => onDone(new FarmAjaxMessageResponse(
                FarmAjaxMessageType.Error, 
                JSON.stringify(error),
                FarmAjaxMessageFormat.UnorderedList
            )));
    }

    static postWithMessageAndValueResponse<T>(url: string, data: any, onDone: (response: FarmAjaxMessageAndValueResponse<T | null>) => void) {
        FarmAjax
            .doAjax(url, data)
            .done(function (response: FarmAjaxMessageAndValueResponse<T>) {
                let message = new FarmAjaxMessageResponse(
                    response.messageType,
                    response.message,
                    response.messageFormat
                );
                onDone(new FarmAjaxMessageAndValueResponse(message, response.value));
            })
            .fail((error) => {
                let message = new FarmAjaxMessageResponse(
                    FarmAjaxMessageType.Error,
                    JSON.stringify(error),
                    FarmAjaxMessageFormat.UnorderedList
                );
                onDone(new FarmAjaxMessageAndValueResponse(message, null));
            });
    }

    private static doAjax(url: string, data: any): JQuery.jqXHR {
        if (data === null)
            data = {};

        if (data.SessionToken !== undefined)
            throw "Please don't define your own 'SessionToken' field, FarmAjax will handle that for you.";

        data.SessionToken = getCookie("FarmMasterAuth");
        return $.ajax({
            type: "POST",
            url: url,
            contentType: "application/json",
            dataType: "json",
            data: JSON.stringify(data)
        });
    }
}