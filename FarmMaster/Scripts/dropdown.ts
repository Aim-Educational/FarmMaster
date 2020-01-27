import { GraphQL } from "./graphql.js"

export class Dropdown {
    public readonly dropdownNode: HTMLDivElement;
    public readonly inputNode: HTMLInputElement;
    public readonly menuNode: HTMLDivElement;
    public readonly defaultValue: string;
    private _refreshFunc: Function;

    constructor(dropdownNodeOrId: HTMLDivElement | string) {
        if (dropdownNodeOrId instanceof HTMLDivElement)
            this.dropdownNode = dropdownNodeOrId;
        else
            this.dropdownNode = <HTMLDivElement>document.getElementById(dropdownNodeOrId);

        if (!this.dropdownNode)
            throw "The dropdown node is null. Parameter given was: " + dropdownNodeOrId;

        // For now, we only support Fomantic UI style, since I doubt it'll be replaced anytime soon.
        this.inputNode = <HTMLInputElement>this.dropdownNode.querySelector("div.dropdown input[type=hidden]");
        if (!this.inputNode)
            throw "Could not find the input element. Is your markup in Fomantic UI style?";

        this.menuNode = <HTMLDivElement>this.dropdownNode.querySelector("div.dropdown div.menu");
        if (!this.menuNode)
            throw "Could not find the menu element. Is your markup in Fomantic UI style?";

        this.defaultValue = <string>this.inputNode.dataset.defaultValue;

        const refreshButton = <HTMLElement>this.dropdownNode.querySelector("label .button[data-type=refresh]");
        refreshButton.addEventListener("click", () => this.refresh());

        // Just to make sure Fomantic UI knows about it, saving the user the hassle.
        $(<any>this.menuNode.parentNode).dropdown();

        this._refreshFunc = function () { alert("No refresh function has been assigned."); };
    }

    public addItem(
        // Gotta love TS' syntax.
        { name,         value = null,          isSelected = false }:
        { name: string, value?: string | null, isSelected: boolean }
    ): HTMLDivElement {
        const item = this.menuNode.appendChild(document.createElement("div"));
        item.classList.add("item");
        item.innerText = name;

        if (value)
            item.dataset.value = value;

        if (isSelected)
            $(<any>this.menuNode.parentNode).dropdown("set selected", value || name);

        return item;
    }

    public clear() {
        while (this.menuNode.firstChild)
            this.menuNode.removeChild(this.menuNode.firstChild);
    }

    public refresh() {
        this._refreshFunc();
    }

    public fromRefreshFunc(func: Function) {
        this._refreshFunc = func;
    }

    public fromGraphQL(
        { query,         parameters = null,          dataGetter }:
        { query: string, parameters?: object | null, dataGetter: (data: object) => { name: string, value: string }[] }
    ) {
        this.fromRefreshFunc(() => {
            GraphQL
                .query(query, parameters)
                .then((data) => {
                    this.clear();

                    const nameValuePairs = dataGetter(data);
                    for (const pair of nameValuePairs)
                        this.addItem({ name: pair.name, value: pair.value, isSelected: pair.value == this.defaultValue });
                })
                .catch((reason) => {
                    alert("TEMP error handling: " + JSON.stringify(reason));
                });
        });
        this.refresh();
        this.refresh(); // Very weird bug where, during the first refresh, Fomantic UI won't perform "set selected" correctly.
    }
}

export default Dropdown;