export declare class Dropdown {
    readonly dropdownNode: HTMLDivElement;
    readonly inputNode: HTMLInputElement;
    readonly menuNode: HTMLDivElement;
    readonly defaultValue: string;
    private _refreshFunc;
    constructor(dropdownNodeOrId: HTMLDivElement | string);
    addItem({ name, value, isSelected }: {
        name: string;
        value?: string | null;
        isSelected: boolean;
    }): HTMLDivElement;
    clear(): void;
    refresh(): void;
    fromRefreshFunc(func: Function): void;
    fromGraphQL({ query, parameters, dataGetter }: {
        query: string;
        parameters?: object | null;
        dataGetter: (data: any) => {
            name: string;
            value: string;
        }[];
    }): void;
    private fromNameIdGraphQL;
    fromContactGraphQL(): void;
    fromSpeciesGraphQL(): void;
    fromHoldingGraphQL(): void;
    fromAnimalGroupGraphQL(): void;
    fromBreedGraphQL(): void;
    fromLifeEventGraphQL(): void;
}
export default Dropdown;
//# sourceMappingURL=dropdown.d.ts.map