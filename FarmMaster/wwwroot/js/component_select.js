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
            if (response.message.messageType == FarmAjaxMessageType.Error) {
                response.message.populateMessageBox(boxError);
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
                var text = inputSelect.querySelector("div.text");
                text.innerHTML = "";
                var input = inputSelect.querySelector("input");
                input.value = "";
                for (var _b = 0, _c = response.value; _b < _c.length; _b++) {
                    var value = _c[_b];
                    var div = document.createElement("div");
                    div.classList.add("item");
                    div.dataset.value = value.value;
                    div.innerText = value.description;
                    menu_1.appendChild(div);
                }
            }
            else
                throw typeof inputSelect;
        });
    };
    return ComponentSelect;
}());
//# sourceMappingURL=component_select.js.map