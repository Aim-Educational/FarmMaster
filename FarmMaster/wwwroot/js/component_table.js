var ComponentTable = (function () {
    function ComponentTable() {
    }
    ComponentTable.onAddAjax = function (inputName, inputValue, boxError, segTable, segInput, ajaxUrl, reason, id, deleteFunc) {
        var name = inputName.value;
        var value = inputValue.value;
        var displayValue = inputValue.innerHTML;
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
                td.innerText = displayValue;
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
    ComponentTable.onDeleteAjax = function (boxError, segTable, ajaxUrl, reason, value, id) {
        boxError.classList.remove("visible");
        segTable.classList.add("loading");
        FarmAjax.postWithMessageResponse(ajaxUrl, {
            Id: id,
            Name: value,
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
                        if (td.innerText === value) {
                            segTable.querySelector("tbody").removeChild(row);
                            return;
                        }
                    });
                });
            }
        });
    };
    ComponentTable.setupPagingTable = function (boxError, table, ajaxPageCount, ajaxRender, entityType, itemsPerPage) {
        if (entityType === void 0) { entityType = null; }
        if (itemsPerPage === void 0) { itemsPerPage = null; }
        var tableFooter = table.tFoot;
        FarmAjax.postWithMessageAndValueResponse(ajaxPageCount, { itemsPerPage: itemsPerPage, entityType: entityType }, function (responseAndValue) {
            if (responseAndValue.messageType !== FarmAjaxMessageType.None)
                responseAndValue.populateMessageBox(boxError);
            else if (tableFooter !== null) {
                tableFooter.innerHTML = "";
                var tr = document.createElement("tr");
                tableFooter.appendChild(tr);
                var th = document.createElement("th");
                th.colSpan = 9999;
                tr.appendChild(th);
                var div = document.createElement('div');
                div.classList.add("ui", "center", "aligned", "pagination", "menu");
                th.appendChild(div);
                var _loop_1 = function (i) {
                    var a = document.createElement("a");
                    a.innerText = "" + (i + 1);
                    a.classList.add("item");
                    a.onclick = function () {
                        ComponentTable.getPage(boxError, table, ajaxRender, i, entityType, itemsPerPage);
                        tableFooter.querySelectorAll("a").forEach(function (item) { return item.classList.remove("active"); });
                        a.classList.add("active");
                    };
                    div.appendChild(a);
                    var divider = document.createElement("div");
                    divider.classList.add("divider");
                    div.appendChild(divider);
                };
                for (var i = 0; i < responseAndValue.value.value; i++) {
                    _loop_1(i);
                }
            }
        });
    };
    ComponentTable.getPage = function (boxError, table, ajaxRender, pageToRender, entityType, itemsPerPage) {
        if (entityType === void 0) { entityType = null; }
        if (itemsPerPage === void 0) { itemsPerPage = null; }
        var tableBody = table.tBodies.item(0);
        FarmAjax.postWithMessageAndValueResponse(ajaxRender, { pageToRender: pageToRender, itemsPerPage: itemsPerPage, entityType: entityType }, function (responseAndValue) {
            if (responseAndValue.messageType !== FarmAjaxMessageType.None)
                responseAndValue.populateMessageBox(boxError);
            else if (tableBody !== null) {
                tableBody.innerHTML = (responseAndValue).value;
            }
        });
    };
    return ComponentTable;
}());
//# sourceMappingURL=component_table.js.map