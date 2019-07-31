class ComponentTable {
    public static onAddAjax(
        inputName: HTMLInputElement,
        inputValue: HTMLInputElement,
        boxError: HTMLDivElement,
        segTable: HTMLDivElement,
        segInput: HTMLDivElement,
        ajaxUrl: string,
        reason: string,
        id: number,
        deleteFunc: Function
    ) {
        let name  = inputName.value;
        let value = inputValue.value;
        let displayValue = inputValue.innerHTML;

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
                else {
                    let tr = document.createElement("tr");

                    let td = document.createElement("td");
                    td.innerText = name;
                    tr.appendChild(td);

                    td = document.createElement("td");
                    td.innerText = displayValue;
                    tr.appendChild(td);

                    td = document.createElement("td");
                    let a = document.createElement("a");
                    a.classList.add("ui", "red", "inverted", "button");
                    a.innerText = "Remove";
                    a.onclick = () => deleteFunc;
                    td.appendChild(a);
                    tr.appendChild(td);

                    // Clear inputs
                    segInput
                        .querySelectorAll("input")
                        .forEach(i => i.value = "");

                    segTable.querySelector("table").tBodies.item(0).appendChild(tr);
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
                else {
                    // Find the row with the name, and delete it.
                    segTable
                        .querySelectorAll("tbody tr")
                        .forEach((row) => {
                            row.querySelectorAll("td").forEach(td => {
                                if (td.innerText === value) {
                                    segTable.querySelector("tbody").removeChild(row);
                                    return;
                                }
                            })
                        });
                }
            }
        );
    }
}