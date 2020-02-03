export declare class PagedTable {
    readonly query: string;
    readonly queryToList: (data: any) => {
        id: number;
        items: string[];
    }[];
    readonly ITEMS_PER_PAGE: number;
    readonly tableNode: HTMLTableElement;
    readonly tbody: HTMLTableSectionElement;
    readonly tfooter: HTMLTableSectionElement;
    readonly pageItems: HTMLDivElement;
    readonly pageName: string;
    readonly controller: string;
    readonly actionEdit: string;
    readonly actionDelete: string;
    constructor(tableNodeOrId: HTMLTableElement | string, query: string, queryToList: (data: any) => {
        id: number;
        items: string[];
    }[]);
    refreshPageCount(): void;
    setPage(page: number): void;
}
//# sourceMappingURL=table.d.ts.map