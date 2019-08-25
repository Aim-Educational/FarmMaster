export declare class ComponentSelectOption {
    value: string;
    description: string;
    dataset: DOMStringMap;
}
export declare class ComponentSelect {
    static populateFromAjaxWithMessageResponse(inputSelect: HTMLSelectElement | HTMLInputElement | HTMLDivElement, boxError: HTMLDivElement | null, ajaxUrl: string, data: any): void;
    static asContentSelector(inputSelect: HTMLSelectElement | HTMLInputElement): void;
    static getOptions(inputSelect: HTMLSelectElement | HTMLInputElement | HTMLDivElement): ComponentSelectOption[];
}
//# sourceMappingURL=component_select.d.ts.map