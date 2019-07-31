var ComponentCharacteristicsValueType;
(function (ComponentCharacteristicsValueType) {
    ComponentCharacteristicsValueType[ComponentCharacteristicsValueType["Error_Unknown"] = 0] = "Error_Unknown";
    ComponentCharacteristicsValueType[ComponentCharacteristicsValueType["TimeSpan"] = 1] = "TimeSpan";
})(ComponentCharacteristicsValueType || (ComponentCharacteristicsValueType = {}));
var ComponentCharacteristicsValue = (function () {
    function ComponentCharacteristicsValue() {
    }
    return ComponentCharacteristicsValue;
}());
var ComponentCharacteristics = (function () {
    function ComponentCharacteristics() {
    }
    ComponentCharacteristics.getValuesAjax = function (boxError, divSegment, table, ajaxUrl, type, id) {
        if (divSegment !== null)
            divSegment.classList.add("loading");
        FarmAjax.postWithMessageAndValueResponse(ajaxUrl, { type: type, id: id }, function (responseAndValue) {
            if (divSegment !== null)
                divSegment.classList.remove("loading");
            if (responseAndValue.messageType !== FarmAjaxMessageType.None) {
                responseAndValue.populateMessageBox(boxError);
                return;
            }
            var tbody = table.tBodies.item(0);
            tbody.innerHTML = "";
            for (var _i = 0, _a = responseAndValue.value; _i < _a.length; _i++) {
                var characteristic = _a[_i];
                ComponentCharacteristics.addValue(table, characteristic);
            }
        });
    };
    ComponentCharacteristics.addValue = function (table, value) {
        var tbody = table.tBodies.item(0);
        var tr = document.createElement("tr");
        tbody.appendChild(tr);
        var tdName = document.createElement("td");
        tdName.innerText = value.name;
        tr.appendChild(tdName);
        var tdValue = document.createElement("td");
        tr.appendChild(tdValue);
        var div = document.createElement("div");
        div.classList.add("ui", "form");
        tdValue.appendChild(div);
        switch (value.type) {
            case ComponentCharacteristicsValueType.TimeSpan:
                tr.dataset.type = "TimeSpan";
                var input = document.createElement("input");
                input.type = "text";
                input.placeholder = "#d #m #s";
                input.innerText = value.value;
                div.appendChild(input);
                break;
            default: break;
        }
    };
    return ComponentCharacteristics;
}());
//# sourceMappingURL=component_characteristics.js.map