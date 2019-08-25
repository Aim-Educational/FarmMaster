export declare enum ComponentCharacteristicsValueType {
    Error_Unknown = 0,
    TimeSpan = 1,
    Text = 2
}
export declare class ComponentCharacteristicsValue {
    name: string;
    type: ComponentCharacteristicsValueType;
    value: any;
    isInherited: boolean;
}
export declare class ComponentCharacteristics {
    static getValuesAjax(boxError: HTMLDivElement | null, divSegment: HTMLDivElement | null, table: HTMLTableElement, funcDelete: (value: ComponentCharacteristicsValue) => void, ajaxUrl: string, type: string, id: string): void;
    static addValue(table: HTMLTableElement, funcDelete: (value: ComponentCharacteristicsValue) => void, value: ComponentCharacteristicsValue): void;
}
//# sourceMappingURL=component_characteristics.d.ts.map