import { FarmAjax, FarmAjaxMessageType, FarmAjaxGenericValue } from "./farm_ajax.js";

export class ComponentTable {
    public static onAddAjax(
        inputName: HTMLInputElement,
        inputValue: HTMLInputElement,
        boxError: HTMLDivElement,
        segTable: HTMLDivElement,
        segInput: HTMLDivElement,
        ajaxUrl: string,
        reason: string,
        id: number,
        deleteFunc: (name: string) => void
    ) {
        let name  = inputName.value;
        let value = inputValue.value;
        let displayValue: string = "";
        let deleteFuncParam: string | null = "";

        if (inputValue.type === "text") {
            displayValue = inputValue.value;
            deleteFuncParam = name;
        }
        else if (inputValue.type === "hidden") { // Special Fomantic UI Style dropdown
            let menu = inputValue.parentElement!.querySelector("div.menu")!;

            menu.querySelectorAll("div.item")
                .forEach(item => {
                    if (displayValue.length > 0)
                        return;

                    if ((item as HTMLDivElement).dataset.value == value || item.innerHTML == value)
                        displayValue = item.innerHTML;
                });

            deleteFuncParam = null;
        }

        boxError.classList.remove("visible");
        segTable.classList.add("loading");
        segInput.classList.add("loading");

        FarmAjax.postWithMessageResponse(
            ajaxUrl,
            {
                Name: name,
                Value: value,
                Reason: reason,
                Id: id
            },
            response => {
                segTable.classList.remove("loading");
                segInput.classList.remove("loading");

                if (response.messageType !== FarmAjaxMessageType.None)
                    response.populateMessageBox(boxError);

                if (response.messageType === FarmAjaxMessageType.Information) {
                    let tr = document.createElement("tr");

                    let td = document.createElement("td");
                    td.innerText = name;
                    tr.appendChild(td);

                    if (value !== displayValue)
                        td.dataset.name = value; // E.g For dropdowns, the visual value (displayValue) and actual value (value) are different.

                    td = document.createElement("td");
                    td.innerText = displayValue;
                    tr.appendChild(td);

                    td = document.createElement("td");

                    if (deleteFuncParam !== null) {
                        let a = document.createElement("a");
                        a.classList.add("ui", "red", "inverted", "button");
                        a.innerText = "Remove";
                        a.onclick = () => deleteFunc(deleteFuncParam);
                        td.appendChild(a);
                        tr.appendChild(td);
                    }
                    else {
                        td.innerHTML = "Please refresh the page";
                        tr.appendChild(td);
                    }

                    // Clear inputs
                    segInput
                        .querySelectorAll("input")
                        .forEach(i => i.value = "");

                    segTable.querySelector("table")!.tBodies.item(0)!.appendChild(tr);
                }
            }
        );
    }

    public static onDeleteAjax(
        boxError: HTMLDivElement,
        segTable: HTMLDivElement,
        ajaxUrl: string,
        reason: string,
        value: string,
        id: number
    ) {
        boxError.classList.remove("visible");
        segTable.classList.add("loading");

        FarmAjax.postWithMessageResponse(
            ajaxUrl,
            {
                Id: id,
                Name: value,
                Reason: reason
            },
            response => {
                segTable.classList.remove("loading");

                if (response.messageType !== FarmAjaxMessageType.None)
                    response.populateMessageBox(boxError);

                if (response.messageType === FarmAjaxMessageType.Information) {
                    // Find the row with the name, and delete it.
                    segTable
                        .querySelectorAll("tbody tr")
                        .forEach((row) => {
                            let td = (row as HTMLTableRowElement).cells.item(0)!;
                            if (td.innerText === value || td.dataset.name == value) {
                                segTable.querySelector("tbody")!.removeChild(row);
                                return;
                            }
                        });
                }
            }
        );
    }

    public static setupPagingTable(
        boxError: HTMLDivElement,
        table: HTMLTableElement,
        ajaxPageCount: string,
        ajaxRender: string,
        entityType: string | null = null,
        itemsPerPage: number | null = null
    ) {
        let tableFooter = table.tFoot!;

        let segment = table.parentElement;
        if (segment !== null && segment.classList.contains("segment"))
            segment.classList.add("loading");
        else
            segment = null;

        FarmAjax.postWithMessageAndValueResponse<FarmAjaxGenericValue<number>>(
            ajaxPageCount,
            { itemsPerPage: itemsPerPage, entityType: entityType },
            responseAndValue => {
                if (segment !== null)
                    segment.classList.remove("loading");

                if (responseAndValue.messageType !== FarmAjaxMessageType.None)
                    responseAndValue.populateMessageBox(boxError);
                else if (tableFooter !== null) {
                    tableFooter.innerHTML = "";

                    let tr = document.createElement("tr");
                    tableFooter.appendChild(tr);

                    let th = document.createElement("th");
                    th.colSpan = 9999;
                    tr.appendChild(th);

                    let div = document.createElement('div');
                    div.classList.add("ui", "center", "aligned", "pagination", "menu");
                    th.appendChild(div);

                    for (let i = 0; i < responseAndValue.value!.value!; i++) {
                        let a = document.createElement("a");
                        a.innerText = "" + (i + 1);
                        a.classList.add("item");
                        a.onclick = function () {
                            ComponentTable.getPage(boxError, table, ajaxRender, i, entityType, itemsPerPage);

                            tableFooter.querySelectorAll("a")!.forEach(item => item.classList.remove("active"));
                            a.classList.add("active");
                        }
                        div.appendChild(a);

                        let divider = document.createElement("div");
                        divider.classList.add("divider");
                        div.appendChild(divider);
                    }

                    ComponentTable.getPage(boxError, table, ajaxRender, 0, entityType, itemsPerPage);
                }
            }
        );
    }

    public static getPage(
        boxError: HTMLDivElement,
        table: HTMLTableElement,
        ajaxRender: string,
        pageToRender: number,
        entityType: string | null = null,
        itemsPerPage: number | null = null
    ) {
        let tableBody = table.tBodies.item(0)!;

        let segment = table.parentElement;
        if (segment !== null && segment.classList.contains("segment"))
            segment.classList.add("loading");
        else
            segment = null;

        FarmAjax.postWithMessageAndValueResponse<FarmAjaxGenericValue<string>>(
            ajaxRender,
            { pageToRender: pageToRender, itemsPerPage: itemsPerPage, entityType: entityType },
            responseAndValue => {
                if (segment !== null)
                    segment.classList.remove("loading");

                if (responseAndValue.messageType !== FarmAjaxMessageType.None)
                    responseAndValue.populateMessageBox(boxError);
                else if (tableBody !== null) {
                    tableBody.innerHTML = <any>(responseAndValue).value;
                }
            }
        );
    }
}