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
        $(<any>this.menuNode.parentNode).dropdown({ forceSelection: false });

        this._refreshFunc = function () { alert("No refresh function has been assigned."); };
    }

    // FUNCTIONS TO MANIPULATE ITEMS

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

        if (isSelected) {
            // Fomantic UI's own functions are too buggy to use, so we do things ourselves
            item.classList.add("active", "selected");
            this.inputNode.value = (!value) ? name : value;

            const textbox = <HTMLDivElement>this.dropdownNode.querySelector("div.dropdown div.text");
            textbox.innerText = name;

            this.inputNode.dispatchEvent(new Event("change"));
        }

        return item;
    }

    public clear() {
        while (this.menuNode.firstChild)
            this.menuNode.removeChild(this.menuNode.firstChild);

        const textbox = <HTMLDivElement>this.dropdownNode.querySelector("div.dropdown div.text");
        textbox.innerText = "";
    }

    public refresh() {
        this._refreshFunc();
    }

    // GENERIC DATA SOURCES

    public fromRefreshFunc(func: Function) {
        this._refreshFunc = func;
    }

    public fromGraphQL(
        { query,         parameters = null,          dataGetter }:
        { query: string, parameters?: object | null, dataGetter: (data: any) => { name: string, value: string }[] }
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
    }

    // COMMON DROPDOWN DATA SOURCES
    //
    // If the data source is used in more than one page, its probably best to put it here.

    private fromNameIdGraphQL(entityName: string) {
        this.fromGraphQL({
            query: `query GetOwners {
                ${entityName} {
                    id
                    name
                }
            }`,

            dataGetter: (json: any) => json[entityName].map(function (v: { name: string, id: string }) {
                return { name: v.name, value: String(v.id) };
            })
        });
    }

    public fromContactGraphQL() {
        this.fromNameIdGraphQL("contacts");
    }

    public fromSpeciesGraphQL() {
        this.fromNameIdGraphQL("species");
    }

    public fromHoldingGraphQL() {
        this.fromNameIdGraphQL("holdings");
    }

    public fromAnimalGroupGraphQL() {
        this.fromNameIdGraphQL("animalGroups");
    }

    public fromBreedGraphQL() {
        this.fromNameIdGraphQL("breeds");
    }

    public fromLifeEventGraphQL() {
        this.fromNameIdGraphQL("lifeEvents");
    }
}

export default Dropdown;