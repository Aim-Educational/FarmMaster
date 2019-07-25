var ComponentSelectOption = (function () {
    function ComponentSelectOption() {
    }
    return ComponentSelectOption;
}());
var ComponentSelect = (function () {
    function ComponentSelect() {
    }
    ComponentSelect.populateFromAjaxWithMessageResponse = function (inputSelect, boxError, ajaxUrl, data) {
        if ((inputSelect instanceof HTMLInputElement && inputSelect.type == "hidden")
            || (inputSelect instanceof HTMLSelectElement && inputSelect.parentElement.classList.contains("dropdown")))
            inputSelect = inputSelect.parentElement;
        if (!inputSelect.classList.contains("ui") && !inputSelect.classList.contains("dropdown"))
            throw "When using Fomantic UI style dropdowns, the 'ui dropdown' classes must be used.";
        inputSelect.classList.add("loading");
        FarmAjax.postWithMessageAndValueResponse(ajaxUrl, data, function (response) {
            inputSelect.classList.remove("loading");
            if (response.messageType == FarmAjaxMessageType.Error) {
                response.populateMessageBox(boxError);
                return;
            }
            if (inputSelect instanceof HTMLSelectElement) {
                while (inputSelect.item.length > 0)
                    inputSelect.remove(0);
                for (var _i = 0, _a = response.value; _i < _a.length; _i++) {
                    var value = _a[_i];
                    var option = document.createElement("option");
                    option.value = value.value;
                    option.innerText = value.description;
                    inputSelect.add(option);
                }
            }
            else if (inputSelect instanceof HTMLDivElement) {
                var menu_1 = inputSelect.querySelector("div.menu");
                menu_1.querySelectorAll("div.item").forEach(function (e) { return menu_1.removeChild(e); });
                for (var _b = 0, _c = response.value; _b < _c.length; _b++) {
                    var value = _c[_b];
                    var div = document.createElement("div");
                    div.classList.add("item");
                    div.dataset.value = value.value;
                    div.innerText = value.description;
                    menu_1.appendChild(div);
                }
                var input_1 = inputSelect.querySelector("input");
                input_1.value = (input_1.dataset.defaultValue) ? input_1.dataset.defaultValue : "";
                var text_1 = inputSelect.querySelector("div.text");
                text_1.innerHTML = "";
                menu_1.querySelectorAll("div.item").forEach(function (item) {
                    if (item.dataset.value == input_1.value)
                        text_1.innerHTML = item.innerHTML;
                });
            }
            else
                throw typeof inputSelect;
        });
    };
    return ComponentSelect;
}());
//# sourceMappingURL=component_select.js.map