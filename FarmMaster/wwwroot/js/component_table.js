var ComponentTable = (function () {
    function ComponentTable() {
    }
    ComponentTable.onAddAjax = function (inputName, inputValue, boxError, segTable, segInput, ajaxUrl, reason, id, deleteFunc) {
        var name = inputName.value;
        var value = inputValue.value;
        boxError.classList.remove("visible");
        segTable.classList.add("loading");
        segInput.classList.add("loading");
        FarmAjax.postWithMessageResponse(ajaxUrl, {
            Name: name,
            Value: value,
            Reason: reason,
            Id: id
        }, function (response) {
            segTable.classList.remove("loading");
            segInput.classList.remove("loading");
            if (response.messageType !== FarmAjaxMessageType.None)
                response.populateMessageBox(boxError);
            else {
                var tr = document.createElement("tr");
                var td = document.createElement("td");
                td.innerText = name;
                tr.appendChild(td);
                td = document.createElement("td");
                td.innerText = value;
                tr.appendChild(td);
                td = document.createElement("td");
                var a = document.createElement("a");
                a.classList.add("ui", "red", "inverted", "button");
                a.innerText = "Remove";
                a.onclick = function () { return deleteFunc; };
                td.appendChild(a);
                tr.appendChild(td);
                segInput
                    .querySelectorAll("input")
                    .forEach(function (i) { return i.value = ""; });
                segTable.querySelector("table").tBodies.item(0).appendChild(tr);
            }
        });
    };
    ComponentTable.onDeleteAjax = function (boxError, segTable, ajaxUrl, reason, name, id) {
        boxError.classList.remove("visible");
        segTable.classList.add("loading");
        FarmAjax.postWithMessageResponse(ajaxUrl, {
            Id: id,
            Name: name,
            Reason: reason
        }, function (response) {
            segTable.classList.remove("loading");
            if (response.messageType !== FarmAjaxMessageType.None)
                response.populateMessageBox(boxError);
            else {
                segTable
                    .querySelectorAll("tbody tr")
                    .forEach(function (row) {
                    row.querySelectorAll("td").forEach(function (td) {
                        if (td.innerText === name) {
                            segTable.querySelector("tbody").removeChild(row);
                            return;
                        }
                    });
                });
            }
        });
    };
    return ComponentTable;
}());
//# sourceMappingURL=component_table.js.map