class ComponentSelectOption {
    public value: string;
    public description: string;
}

class ComponentSelect {
    public static populateFromAjaxWithMessageResponse(
        inputSelect: HTMLSelectElement | HTMLInputElement | HTMLDivElement,
        boxError:    HTMLDivElement | null,
        ajaxUrl:     string,
        data:        any
    ): void {
        if ((inputSelect instanceof HTMLInputElement && inputSelect.type == "hidden")
            || (inputSelect instanceof HTMLSelectElement && inputSelect.parentElement.classList.contains("dropdown")))
            inputSelect = <HTMLDivElement>inputSelect.parentElement;

        if (!inputSelect.classList.contains("ui") && !inputSelect.classList.contains("dropdown"))
            throw "When using Fomantic UI style dropdowns, the 'ui dropdown' classes must be used.";

        inputSelect.classList.add("loading");
        FarmAjax.postWithMessageAndValueResponse<ComponentSelectOption[]>(ajaxUrl, data, (response) =>
        {
            inputSelect.classList.remove("loading");

            if (response.message.messageType == FarmAjaxMessageType.Error) {
                response.message.populateMessageBox(boxError);
                return;
            }

            if (inputSelect instanceof HTMLSelectElement) {
                while (inputSelect.item.length > 0)
                    inputSelect.remove(0);

                for (let value of response.value) {
                    let option = document.createElement("option");
                    option.value = value.value;
                    option.innerText = value.description;
                    inputSelect.add(option);
                }
            }
            else if (inputSelect instanceof HTMLDivElement) {
                let menu = inputSelect.querySelector("div.menu");
                menu.querySelectorAll("div.item").forEach(e => menu.removeChild(e));

                let text = inputSelect.querySelector("div.text");
                text.innerHTML = "";

                let input = inputSelect.querySelector("input");
                input.value = "";

                for (let value of response.value) {
                    let div = document.createElement("div");
                    div.classList.add("item");
                    div.dataset.value = value.value;
                    div.innerText = value.description;
                    menu.appendChild(div);
                }
            }
            else
                throw typeof inputSelect;
        });
    }
}