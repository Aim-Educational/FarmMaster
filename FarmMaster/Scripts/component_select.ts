import { FarmAjax, FarmAjaxMessageType } from "./farm_ajax.js";

export class ComponentSelectOption {
    public value: string = "";
    public description: string = "";

    // Not returned by server, but other helper functions:
    public dataset: DOMStringMap = {};
}

export class ComponentSelect {
    public static populateFromAjaxWithMessageResponse(
        inputSelect: HTMLSelectElement | HTMLInputElement | HTMLDivElement,
        boxError:    HTMLDivElement | null,
        ajaxUrl:     string,
        data:        any
    ): void {
        if ((inputSelect instanceof HTMLInputElement && inputSelect.type == "hidden")
            || (inputSelect instanceof HTMLSelectElement && inputSelect.parentElement!.classList.contains("dropdown")))
            inputSelect = <HTMLDivElement>inputSelect.parentElement;

        if (!inputSelect.classList.contains("ui") && !inputSelect.classList.contains("dropdown"))
            throw "When using Fomantic UI style dropdowns, the 'ui dropdown' classes must be used.";

        inputSelect.classList.add("loading");
        FarmAjax.postWithMessageAndValueResponse<ComponentSelectOption[]>(ajaxUrl, data, (response) =>
        {
            inputSelect.classList.remove("loading");

            if (response.messageType == FarmAjaxMessageType.Error) {
                response.populateMessageBox(<HTMLElement>boxError);
                return;
            }

            if (inputSelect instanceof HTMLSelectElement) {
                while (inputSelect.item.length > 0)
                    inputSelect.remove(0);

                for (let value of response.value!) {
                    let option = document.createElement("option");
                    option.value = value.value;
                    option.innerText = value.description;
                    inputSelect.add(option);
                }
            }
            else if (inputSelect instanceof HTMLDivElement) {
                let menu = inputSelect.querySelector("div.menu")!;
                menu.querySelectorAll("div.item")!.forEach(e => menu.removeChild(e));

                // Add all the items.
                for (let value of response.value!) {
                    let div = document.createElement("div");
                    div.classList.add("item");
                    div.dataset.value = value.value;
                    div.innerText = value.description;
                    menu.appendChild(div);
                }

                // If there's a default value, set all the appropriate things, otherwise just clear the value.
                let input = inputSelect.querySelector("input")!;
                input.value = (input.dataset.defaultValue) ? input.dataset.defaultValue : "";
                
                let text = inputSelect.querySelector("div.text")!;
                text.innerHTML = "";

                menu.querySelectorAll("div.item").forEach((item_: HTMLDivElement | Element) => {
                    let item = item_ as HTMLDivElement;
                    if (item.dataset.value == input.value)
                        text.innerHTML = item.innerHTML;
                });
            }
            else
                throw typeof inputSelect;
        });
    }

    public static asContentSelector(inputSelect: HTMLSelectElement | HTMLInputElement) {
        inputSelect.addEventListener("change", function ()
        {
            let idContent: string = "";

            if (inputSelect instanceof HTMLSelectElement) {
                let item = inputSelect.selectedOptions.item(0)!;
                idContent = (item.dataset.contentId) ? item.dataset.contentId : item.value;
            }
            else {
                let options = ComponentSelect.getOptions(inputSelect);
                let item = options.filter(opt => opt.value == inputSelect.value)[0];
                idContent = (item.dataset.contentId) ? item.dataset.contentId : item.value;
            }

            // Hide all other options.
            let options = ComponentSelect.getOptions(inputSelect);
            for (let option of options) {
                document.getElementById((option.dataset.contentId) ? option.dataset.contentId : option.value)!
                    .classList.add("transition", "hidden");
            }

            // Show the one we want.
            document.getElementById(idContent)!.classList.remove("transition", "hidden");
        });
    }

    // A universal way to get the options of either a normal <select>, or a Fomantic UI style select.
    public static getOptions(inputSelect: HTMLSelectElement | HTMLInputElement | HTMLDivElement): ComponentSelectOption[] {
        if ((inputSelect instanceof HTMLSelectElement || inputSelect instanceof HTMLInputElement)
            && inputSelect.parentElement!.classList.contains("dropdown")) {
            inputSelect = <HTMLDivElement>inputSelect.parentElement;
        }

        if (inputSelect instanceof HTMLSelectElement) {
            let list: ComponentSelectOption[] = [];
            for (let i = 0; i < inputSelect.item.length; i++) {
                let item = inputSelect.options.item(i)!;
                list.push({ value: item.value.toLowerCase(), description: item.innerText, dataset: item.dataset });
            }

            return list;
        }
        if (inputSelect instanceof HTMLDivElement) {
            let list: ComponentSelectOption[] = [];

            inputSelect.querySelectorAll(".menu .item")!
                .forEach(function (item_: HTMLDivElement | Element) {
                    let item = item_ as HTMLDivElement;
                    let value = (item.dataset.value) ? item.dataset.value : item.innerText;
                    list.push({ value: value.toLowerCase(), description: item.innerText, dataset: item.dataset });
                });

            return list;
        }

        throw "Unsupported type: " + inputSelect;
    }
}