export declare class ComponentTable {
    static onAddAjax(inputName: HTMLInputElement, inputValue: HTMLInputElement, boxError: HTMLDivElement, segTable: HTMLDivElement, segInput: HTMLDivElement, ajaxUrl: string, reason: string, id: number, deleteFunc: (name: string) => void): void;
    static onDeleteAjax(boxError: HTMLDivElement, segTable: HTMLDivElement, ajaxUrl: string, reason: string, value: string, id: number): void;
    static setupPagingTable(boxError: HTMLDivElement, table: HTMLTableElement, ajaxPageCount: string, ajaxRender: string, entityType?: string | null, itemsPerPage?: number | null): void;
    static getPage(boxError: HTMLDivElement, table: HTMLTableElement, ajaxRender: string, pageToRender: number, entityType?: string | null, itemsPerPage?: number | null): void;
}
//# sourceMappingURL=component_table.d.ts.map