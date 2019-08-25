import { FarmAjax, FarmAjaxMessageType } from "./farm_ajax.js";

export enum ComponentCharacteristicsValueType {
    Error_Unknown,
    TimeSpan,
    Text
}

export class ComponentCharacteristicsValue {
    public name: string = "";
    public type: ComponentCharacteristicsValueType = ComponentCharacteristicsValueType.Error_Unknown;
    public value: any = null;
}

export class ComponentCharacteristics {
    public static getValuesAjax(
        boxError: HTMLDivElement | null,
        divSegment: HTMLDivElement | null,
        table: HTMLTableElement,
        funcDelete: (value: ComponentCharacteristicsValue) => void,
        ajaxUrl: string,
        type: string,
        id: string
    ) {
        if (divSegment !== null)
            divSegment.classList.add("loading");

        FarmAjax.postWithMessageAndValueResponse<ComponentCharacteristicsValue[]>(
            ajaxUrl,
            { type: type, id: id },
            responseAndValue => {
                if (divSegment !== null)
                    divSegment.classList.remove("loading");

                if (responseAndValue.messageType !== FarmAjaxMessageType.None) {
                    if (boxError !== null)
                        responseAndValue.populateMessageBox(boxError);
                    return;
                }

                let tbody = table.tBodies.item(0)!;
                tbody.innerHTML = "";

                for (let characteristic of responseAndValue.value!) {
                    ComponentCharacteristics.addValue(table, funcDelete, characteristic);
                }
            }
        );
    }

    public static addValue(
        table: HTMLTableElement,
        funcDelete: (value: ComponentCharacteristicsValue) => void,
        value: ComponentCharacteristicsValue
    ) {
        let tbody = table.tBodies.item(0)!;

        let tr = document.createElement("tr");
        tbody.appendChild(tr);

        let tdName = document.createElement("td");
        tdName.innerText = value.name;
        tr.appendChild(tdName);

        let tdType = document.createElement("td");
        tr.appendChild(tdType);

        let tdValue = document.createElement("td");
        tr.appendChild(tdValue);

        let tdActions = document.createElement("td");
        tr.appendChild(tdActions);

        let btnDelete = document.createElement("button");
        btnDelete.classList.add("ui", "secondary", "button");
        btnDelete.innerText = "Delete";
        btnDelete.onclick = () => funcDelete(value);
        tdActions.appendChild(btnDelete);

        let div = document.createElement("div");
        div.classList.add("ui", "form");
        tdValue.appendChild(div);

        switch (value.type) {
            case ComponentCharacteristicsValueType.TimeSpan:
                tdType.innerText = "TimeSpan";

                var input = document.createElement("input");
                input.type = "text";
                input.placeholder = "#d #m #s";
                input.value = value.value;
                div.appendChild(input);
                break;

            case ComponentCharacteristicsValueType.Text:
                tdType.innerText = "Text";

                var input = document.createElement("input");
                input.type = "text";
                input.placeholder = "Text";
                input.value = value.value;
                div.appendChild(input);
                break;

            default: break;
        }
    }
}