declare global {
    interface Window {
        RequestVerificationToken: string;
    }
}
export declare enum FarmAjaxMessageType {
    None = 0,
    Information = 1,
    Warning = 2,
    Error = 3
}
export declare enum FarmAjaxMessageFormat {
    Default = 0,
    UnorderedList = 1
}
export declare class FarmAjaxMessageResponse {
    messageFormat: FarmAjaxMessageFormat;
    messageType: FarmAjaxMessageType;
    message: string;
    constructor(type: FarmAjaxMessageType, message: string, format: FarmAjaxMessageFormat);
    populateMessageBox(box: HTMLElement): void;
}
export declare class FarmAjaxMessageAndValueResponse<T> extends FarmAjaxMessageResponse {
    value: T | null;
    constructor(message: FarmAjaxMessageResponse, value: T | null);
}
export declare class FarmAjaxGenericValue<T> {
    value: T;
    constructor(value: T);
}
export declare class FarmAjax {
    static postWithMessageResponse(url: string, data: any, onDone: (response: FarmAjaxMessageResponse) => void): void;
    static postWithMessageAndValueResponse<T>(url: string, data: any, onDone: (response: FarmAjaxMessageAndValueResponse<T | null>) => void): void;
    private static doAjax;
}
//# sourceMappingURL=farm_ajax.d.ts.map