import { FarmAjax, FarmAjaxMessageType } from "./farm_ajax.js";
export class ComponentTable {
    static onAddAjax(inputName, inputValue, boxError, segTable, segInput, ajaxUrl, reason, id, deleteFunc) {
        let name = inputName.value;
        let value = inputValue.value;
        let displayValue = "";
        let deleteFuncParam = "";
        if (inputValue.type === "text") {
            displayValue = inputValue.value;
            deleteFuncParam = name;
        }
        else if (inputValue.type === "hidden") { // Special Fomantic UI Style dropdown
            let menu = inputValue.parentElement.querySelector("div.menu");
            menu.querySelectorAll("div.item")
                .forEach(item => {
                if (displayValue.length > 0)
                    return;
                if (item.dataset.value == value || item.innerHTML == value)
                    displayValue = item.innerHTML;
            });
            deleteFuncParam = null;
        }
        boxError.classList.remove("visible");
        segTable.classList.add("loading");
        segInput.classList.add("loading");
        FarmAjax.postWithMessageResponse(ajaxUrl, {
            Name: name,
            Value: value,
            Reason: reason,
            Id: id
        }, response => {
            segTable.classList.remove("loading");
            segInput.classList.remove("loading");
            if (response.messageType !== FarmAjaxMessageType.Information)
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
                segTable.querySelector("table").tBodies.item(0).appendChild(tr);
            }
        });
    }
    static onDeleteAjax(boxError, segTable, ajaxUrl, reason, value, id) {
        boxError.classList.remove("visible");
        segTable.classList.add("loading");
        FarmAjax.postWithMessageResponse(ajaxUrl, {
            Id: id,
            Name: value,
            Reason: reason
        }, response => {
            segTable.classList.remove("loading");
            if (response.messageType !== FarmAjaxMessageType.Information)
                response.populateMessageBox(boxError);
            if (response.messageType === FarmAjaxMessageType.Information) {
                // Find the row with the name, and delete it.
                segTable
                    .querySelectorAll("tbody tr")
                    .forEach((row) => {
                    let td = row.cells.item(0);
                    if (td.innerText === value || td.dataset.name == value) {
                        segTable.querySelector("tbody").removeChild(row);
                        return;
                    }
                });
            }
        });
    }
    static setupPagingTable(boxError, table, ajaxPageCount, ajaxRender, entityType = null, itemsPerPage = null) {
        let tableFooter = table.tFoot;
        let segment = table.parentElement;
        if (segment !== null && segment.classList.contains("segment"))
            segment.classList.add("loading");
        else
            segment = null;
        FarmAjax.postWithMessageAndValueResponse(ajaxPageCount, { itemsPerPage: itemsPerPage, entityType: entityType }, responseAndValue => {
            if (segment !== null)
                segment.classList.remove("loading");
            if (responseAndValue.messageType !== FarmAjaxMessageType.Information)
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
                for (let i = 0; i < responseAndValue.value.value; i++) {
                    let a = document.createElement("a");
                    a.innerText = "" + (i + 1);
                    a.classList.add("item");
                    a.onclick = function () {
                        ComponentTable.getPage(boxError, table, ajaxRender, i, entityType, itemsPerPage);
                        tableFooter.querySelectorAll("a").forEach(item => item.classList.remove("active"));
                        a.classList.add("active");
                    };
                    div.appendChild(a);
                    let divider = document.createElement("div");
                    divider.classList.add("divider");
                    div.appendChild(divider);
                }
                ComponentTable.getPage(boxError, table, ajaxRender, 0, entityType, itemsPerPage);
            }
        });
    }
    static getPage(boxError, table, ajaxRender, pageToRender, entityType = null, itemsPerPage = null) {
        let tableBody = table.tBodies.item(0);
        let segment = table.parentElement;
        if (segment !== null && segment.classList.contains("segment"))
            segment.classList.add("loading");
        else
            segment = null;
        FarmAjax.postWithMessageAndValueResponse(ajaxRender, { pageToRender: pageToRender, itemsPerPage: itemsPerPage, entityType: entityType }, responseAndValue => {
            if (segment !== null)
                segment.classList.remove("loading");
            if (responseAndValue.messageType !== FarmAjaxMessageType.Information)
                responseAndValue.populateMessageBox(boxError);
            else if (tableBody !== null) {
                tableBody.innerHTML = (responseAndValue).value;
            }
        });
    }
}
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiY29tcG9uZW50X3RhYmxlLmpzIiwic291cmNlUm9vdCI6IiIsInNvdXJjZXMiOlsiLi4vLi4vU2NyaXB0cy9jb21wb25lbnRfdGFibGUudHMiXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IkFBQUEsT0FBTyxFQUFFLFFBQVEsRUFBRSxtQkFBbUIsRUFBd0IsTUFBTSxnQkFBZ0IsQ0FBQztBQUVyRixNQUFNLE9BQU8sY0FBYztJQUNoQixNQUFNLENBQUMsU0FBUyxDQUNuQixTQUEyQixFQUMzQixVQUE0QixFQUM1QixRQUF3QixFQUN4QixRQUF3QixFQUN4QixRQUF3QixFQUN4QixPQUFlLEVBQ2YsTUFBYyxFQUNkLEVBQVUsRUFDVixVQUFrQztRQUVsQyxJQUFJLElBQUksR0FBSSxTQUFTLENBQUMsS0FBSyxDQUFDO1FBQzVCLElBQUksS0FBSyxHQUFHLFVBQVUsQ0FBQyxLQUFLLENBQUM7UUFDN0IsSUFBSSxZQUFZLEdBQVcsRUFBRSxDQUFDO1FBQzlCLElBQUksZUFBZSxHQUFrQixFQUFFLENBQUM7UUFFeEMsSUFBSSxVQUFVLENBQUMsSUFBSSxLQUFLLE1BQU0sRUFBRTtZQUM1QixZQUFZLEdBQUcsVUFBVSxDQUFDLEtBQUssQ0FBQztZQUNoQyxlQUFlLEdBQUcsSUFBSSxDQUFDO1NBQzFCO2FBQ0ksSUFBSSxVQUFVLENBQUMsSUFBSSxLQUFLLFFBQVEsRUFBRSxFQUFFLHFDQUFxQztZQUMxRSxJQUFJLElBQUksR0FBRyxVQUFVLENBQUMsYUFBYyxDQUFDLGFBQWEsQ0FBQyxVQUFVLENBQUUsQ0FBQztZQUVoRSxJQUFJLENBQUMsZ0JBQWdCLENBQUMsVUFBVSxDQUFDO2lCQUM1QixPQUFPLENBQUMsSUFBSSxDQUFDLEVBQUU7Z0JBQ1osSUFBSSxZQUFZLENBQUMsTUFBTSxHQUFHLENBQUM7b0JBQ3ZCLE9BQU87Z0JBRVgsSUFBSyxJQUF1QixDQUFDLE9BQU8sQ0FBQyxLQUFLLElBQUksS0FBSyxJQUFJLElBQUksQ0FBQyxTQUFTLElBQUksS0FBSztvQkFDMUUs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Z0JBQ3pCLFNBQVMsQ0FBQyxTQUFTLEdBQVEsQ0FBQyxnQkFBZ0IsQ0FBQyxDQUFDLEtBQUssQ0FBQzthQUN2RDtRQUNMLENBQUMsQ0FDSixDQUFDO0lBQ04sQ0FBQztDQUNKIiwic291cmNlc0NvbnRlbnQiOlsiaW1wb3J0IHsgRmFybUFqYXgsIEZhcm1BamF4TWVzc2FnZVR5cGUsIEZhcm1BamF4R2VuZXJpY1ZhbHVlIH0gZnJvbSBcIi4vZmFybV9hamF4LmpzXCI7XHJcblxyXG5leHBvcnQgY2xhc3MgQ29tcG9uZW50VGFibGUge1xyXG4gICAgcHVibGljIHN0YXRpYyBvbkFkZEFqYXgoXHJcbiAgICAgICAgaW5wdXROYW1lOiBIVE1MSW5wdXRFbGVtZW50LFxyXG4gICAgICAgIGlucHV0VmFsdWU6IEhUTUxJbnB1dEVsZW1lbnQsXHJcbiAgICAgICAgYm94RXJyb3I6IEhUTUxEaXZFbGVtZW50LFxyXG4gICAgICAgIHNlZ1RhYmxlOiBIVE1MRGl2RWxlbWVudCxcclxuICAgICAgICBzZWdJbnB1dDogSFRNTERpdkVsZW1lbnQsXHJcbiAgICAgICAgYWpheFVybDogc3RyaW5nLFxyXG4gICAgICAgIHJlYXNvbjogc3RyaW5nLFxyXG4gICAgICAgIGlkOiBudW1iZXIsXHJcbiAgICAgICAgZGVsZXRlRnVuYzogKG5hbWU6IHN0cmluZykgPT4gdm9pZFxyXG4gICAgKSB7XHJcbiAgICAgICAgbGV0IG5hbWUgID0gaW5wdXROYW1lLnZhbHVlO1xyXG4gICAgICAgIGxldCB2YWx1ZSA9IGlucHV0VmFsdWUudmFsdWU7XHJcbiAgICAgICAgbGV0IGRpc3BsYXlWYWx1ZTogc3RyaW5nID0gXCJcIjtcclxuICAgICAgICBsZXQgZGVsZXRlRnVuY1BhcmFtOiBzdHJpbmcgfCBudWxsID0gXCJcIjtcclxuXHJcbiAgICAgICAgaWYgKGlucHV0VmFsdWUudHlwZSA9PT0gXCJ0ZXh0XCIpIHtcclxuICAgICAgICAgICAgZGlzcGxheVZhbHVlID0gaW5wdXRWYWx1ZS52YWx1ZTtcclxuICAgICAgICAgICAgZGVsZXRlRnVuY1BhcmFtID0gbmFtZTtcclxuICAgICAgICB9XHJcbiAgICAgICAgZWxzZSBpZiAoaW5wdXRWYWx1ZS50eXBlID09PSBcImhpZGRlblwiKSB7IC8vIFNwZWNpYWwgRm9tYW50aWMgVUkgU3R5bGUgZHJvcGRvd25cclxuICAgICAgICAgICAgbGV0IG1lbnUgPSBpbnB1dFZhbHVlLnBhcmVudEVsZW1lbnQhLnF1ZXJ5U2VsZWN0b3IoXCJkaXYubWVudVwiKSE7XHJcblxyXG4gICAgICAgICAgICBtZW51LnF1ZXJ5U2VsZWN0b3JBbGwoXCJkaXYuaXRlbVwiKVxyXG4gICAgICAgICAgICAgICAgLmZvckVhY2goaXRlbSA9PiB7XHJcbiAgICAgICAgICAgICAgICAgICAgaWYgKGRpc3BsYXlWYWx1ZS5sZW5ndGggPiAwKVxyXG4gICAgICAgICAgICAgICAgICAgICAgICByZXR1cm47XHJcblxyXG4gICAgICAgICAgICAgICAgICAgIGlmICgoaXRlbSBhcyBIVE1MRGl2RWxlbWVudCkuZGF0YXNldC52YWx1ZSA9PSB2YWx1ZSB8fCBpdGVtLmlubmVySFRNTCA9PSB2YWx1ZSlcclxuICAgICAgICAgICAgICAgICAgICAgICAgZGlzcGxheVZhbHVlID0gaXRlbS5pbm5lckhUTUw7XHJcbiAgICAgICAgICAgICAgICB9KTtcclxuXHJcbiAgICAgICAgICAgIGRlbGV0ZUZ1bmNQYXJhbSA9IG51bGw7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICBib3hFcnJvci5jbGFzc0xpc3QucmVtb3ZlKFwidmlzaWJsZVwiKTtcclxuICAgICAgICBzZWdUYWJsZS5jbGFzc0xpc3QuYWRkKFwibG9hZGluZ1wiKTtcclxuICAgICAgICBzZWdJbnB1dC5jbGFzc0xpc3QuYWRkKFwibG9hZGluZ1wiKTtcclxuXHJcbiAgICAgICAgRmFybUFqYXgucG9zdFdpdGhNZXNzYWdlUmVzcG9uc2UoXHJcbiAgICAgICAgICAgIGFqYXhVcmwsXHJcbiAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICAgIE5hbWU6IG5hbWUsXHJcbiAgICAgICAgICAgICAgICBWYWx1ZTogdmFsdWUsXHJcbiAgICAgICAgICAgICAgICBSZWFzb246IHJlYXNvbixcclxuICAgICAgICAgICAgICAgIElkOiBpZFxyXG4gICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICByZXNwb25zZSA9PiB7XHJcbiAgICAgICAgICAgICAgICBzZWdUYWJsZS5jbGFzc0xpc3QucmVtb3ZlKFwibG9hZGluZ1wiKTtcclxuICAgICAgICAgICAgICAgIHNlZ0lucHV0LmNsYXNzTGlzdC5yZW1vdmUoXCJsb2FkaW5nXCIpO1xyXG5cclxuICAgICAgICAgICAgICAgIGlmIChyZXNwb25zZS5tZXNzYWdlVHlwZSAhPT0gRmFybUFqYXhNZXNzYWdlVHlwZS5JbmZvcm1hdGlvbilcclxuICAgICAgICAgICAgICAgICAgICByZXNwb25zZS5wb3B1bGF0ZU1lc3NhZ2VCb3goYm94RXJyb3IpO1xyXG5cclxuICAgICAgICAgICAgICAgIGlmIChyZXNwb25zZS5tZXNzYWdlVHlwZSA9PT0gRmFybUFqYXhNZXNzYWdlVHlwZS5JbmZvcm1hdGlvbikge1xyXG4gICAgICAgICAgICAgICAgICAgIGxldCB0ciA9IGRvY3VtZW50LmNyZWF0ZUVsZW1lbnQoXCJ0clwiKTtcclxuXHJcbiAgICAgICAgICAgICAgICAgICAgbGV0IHRkID0gZG9jdW1lbnQuY3JlYXRlRWxlbWVudChcInRkXCIpO1xyXG4gICAgICAgICAgICAgICAgICAgIHRkLmlubmVyVGV4dCA9IG5hbWU7XHJcbiAgICAgICAgICAgICAgICAgICAgdHIuYXBwZW5kQ2hpbGQodGQpO1xyXG5cclxuICAgICAgICAgICAgICAgICAgICBpZiAodmFsdWUgIT09IGRpc3BsYXlWYWx1ZSlcclxuICAgICAgICAgICAgICAgICAgICAgICAgdGQuZGF0YXNldC5uYW1lID0gdmFsdWU7IC8vIEUuZyBGb3IgZHJvcGRvd25zLCB0aGUgdmlzdWFsIHZhbHVlIChkaXNwbGF5VmFsdWUpIGFuZCBhY3R1YWwgdmFsdWUgKHZhbHVlKSBhcmUgZGlmZmVyZW50LlxyXG5cclxuICAgICAgICAgICAgICAgICAgICB0ZCA9IGRvY3VtZW50LmNyZWF0ZUVsZW1lbnQoXCJ0ZFwiKTtcclxuICAgICAgICAgICAgICAgICAgICB0ZC5pbm5lclRleHQgPSBkaXNwbGF5VmFsdWU7XHJcbiAgICAgICAgICAgICAgICAgICAgdHIuYXBwZW5kQ2hpbGQodGQpO1xyXG5cclxuICAgICAgICAgICAgICAgICAgICB0ZCA9IGRvY3VtZW50LmNyZWF0ZUVsZW1lbnQoXCJ0ZFwiKTtcclxuXHJcbiAgICAgICAgICAgICAgICAgICAgaWYgKGRlbGV0ZUZ1bmNQYXJhbSAhPT0gbnVsbCkge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBsZXQgYSA9IGRvY3VtZW50LmNyZWF0ZUVsZW1lbnQoXCJhXCIpO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBhLmNsYXNzTGlzdC5hZGQoXCJ1aVwiLCBcInJlZFwiLCBcImludmVydGVkXCIsIFwiYnV0dG9uXCIpO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBhLmlubmVyVGV4dCA9IFwiUmVtb3ZlXCI7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGEub25jbGljayA9ICgpID0+IGRlbGV0ZUZ1bmMoZGVsZXRlRnVuY1BhcmFtISk7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHRkLmFwcGVuZENoaWxkKGEpO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICB0ci5hcHBlbmRDaGlsZCh0ZCk7XHJcbiAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgICAgIGVsc2Uge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICB0ZC5pbm5lckhUTUwgPSBcIlBsZWFzZSByZWZyZXNoIHRoZSBwYWdlXCI7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHRyLmFwcGVuZENoaWxkKHRkKTtcclxuICAgICAgICAgICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICAgICAgICAgIC8vIENsZWFyIGlucHV0c1xyXG4gICAgICAgICAgICAgICAgICAgIHNlZ0lucHV0XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIC5xdWVyeVNlbGVjdG9yQWxsKFwiaW5wdXRcIilcclxuICAgICAgICAgICAgICAgICAgICAgICAgLmZvckVhY2goaSA9PiBpLnZhbHVlID0gXCJcIik7XHJcblxyXG4gICAgICAgICAgICAgICAgICAgIHNlZ1RhYmxlLnF1ZXJ5U2VsZWN0b3IoXCJ0YWJsZVwiKSEudEJvZGllcy5pdGVtKDApIS5hcHBlbmRDaGlsZCh0cik7XHJcbiAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICApO1xyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBzdGF0aWMgb25EZWxldGVBamF4KFxyXG4gICAgICAgIGJveEVycm9yOiBIVE1MRGl2RWxlbWVudCxcclxuICAgICAgICBzZWdUYWJsZTogSFRNTERpdkVsZW1lbnQsXHJcbiAgICAgICAgYWpheFVybDogc3RyaW5nLFxyXG4gICAgICAgIHJlYXNvbjogc3RyaW5nLFxyXG4gICAgICAgIHZhbHVlOiBzdHJpbmcsXHJcbiAgICAgICAgaWQ6IG51bWJlclxyXG4gICAgKSB7XHJcbiAgICAgICAgYm94RXJyb3IuY2xhc3NMaXN0LnJlbW92ZShcInZpc2libGVcIik7XHJcbiAgICAgICAgc2VnVGFibGUuY2xhc3NMaXN0LmFkZChcImxvYWRpbmdcIik7XHJcblxyXG4gICAgICAgIEZhcm1BamF4LnBvc3RXaXRoTWVzc2FnZVJlc3BvbnNlKFxyXG4gICAgICAgICAgICBhamF4VXJsLFxyXG4gICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgICBJZDogaWQsXHJcbiAgICAgICAgICAgICAgICBOYW1lOiB2YWx1ZSxcclxuICAgICAgICAgICAgICAgIFJlYXNvbjogcmVhc29uXHJcbiAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgIHJlc3BvbnNlID0+IHtcclxuICAgICAgICAgICAgICAgIHNlZ1RhYmxlLmNsYXNzTGlzdC5yZW1vdmUoXCJsb2FkaW5nXCIpO1xyXG5cclxuICAgICAgICAgICAgICAgIGlmIChyZXNwb25zZS5tZXNzYWdlVHlwZSAhPT0gRmFybUFqYXhNZXNzYWdlVHlwZS5JbmZvcm1hdGlvbilcclxuICAgICAgICAgICAgICAgICAgICByZXNwb25zZS5wb3B1bGF0ZU1lc3NhZ2VCb3goYm94RXJyb3IpO1xyXG5cclxuICAgICAgICAgICAgICAgIGlmIChyZXNwb25zZS5tZXNzYWdlVHlwZSA9PT0gRmFybUFqYXhNZXNzYWdlVHlwZS5JbmZvcm1hdGlvbikge1xyXG4gICAgICAgICAgICAgICAgICAgIC8vIEZpbmQgdGhlIHJvdyB3aXRoIHRoZSBuYW1lLCBhbmQgZGVsZXRlIGl0LlxyXG4gICAgICAgICAgICAgICAgICAgIHNlZ1RhYmxlXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIC5xdWVyeVNlbGVjdG9yQWxsKFwidGJvZHkgdHJcIilcclxuICAgICAgICAgICAgICAgICAgICAgICAgLmZvckVhY2goKHJvdykgPT4ge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgbGV0IHRkID0gKHJvdyBhcyBIVE1MVGFibGVSb3dFbGVtZW50KS5jZWxscy5pdGVtKDApITtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGlmICh0ZC5pbm5lclRleHQgPT09IHZhbHVlIHx8IHRkLmRhdGFzZXQubmFtZSA9PSB2YWx1ZSkge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIHNlZ1RhYmxlLnF1ZXJ5U2VsZWN0b3IoXCJ0Ym9keVwiKSEucmVtb3ZlQ2hpbGQocm93KTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICByZXR1cm47XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIH0pO1xyXG4gICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgKTtcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgc3RhdGljIHNldHVwUGFnaW5nVGFibGUoXHJcbiAgICAgICAgYm94RXJyb3I6IEhUTUxEaXZFbGVtZW50LFxyXG4gICAgICAgIHRhYmxlOiBIVE1MVGFibGVFbGVtZW50LFxyXG4gICAgICAgIGFqYXhQYWdlQ291bnQ6IHN0cmluZyxcclxuICAgICAgICBhamF4UmVuZGVyOiBzdHJpbmcsXHJcbiAgICAgICAgZW50aXR5VHlwZTogc3RyaW5nIHwgbnVsbCA9IG51bGwsXHJcbiAgICAgICAgaXRlbXNQZXJQYWdlOiBudW1iZXIgfCBudWxsID0gbnVsbFxyXG4gICAgKSB7XHJcbiAgICAgICAgbGV0IHRhYmxlRm9vdGVyID0gdGFibGUudEZvb3QhO1xyXG5cclxuICAgICAgICBsZXQgc2VnbWVudCA9IHRhYmxlLnBhcmVudEVsZW1lbnQ7XHJcbiAgICAgICAgaWYgKHNlZ21lbnQgIT09IG51bGwgJiYgc2VnbWVudC5jbGFzc0xpc3QuY29udGFpbnMoXCJzZWdtZW50XCIpKVxyXG4gICAgICAgICAgICBzZWdtZW50LmNsYXNzTGlzdC5hZGQoXCJsb2FkaW5nXCIpO1xyXG4gICAgICAgIGVsc2VcclxuICAgICAgICAgICAgc2VnbWVudCA9IG51bGw7XHJcblxyXG4gICAgICAgIEZhcm1BamF4LnBvc3RXaXRoTWVzc2FnZUFuZFZhbHVlUmVzcG9uc2U8RmFybUFqYXhHZW5lcmljVmFsdWU8bnVtYmVyPj4oXHJcbiAgICAgICAgICAgIGFqYXhQYWdlQ291bnQsXHJcbiAgICAgICAgICAgIHsgaXRlbXNQZXJQYWdlOiBpdGVtc1BlclBhZ2UsIGVudGl0eVR5cGU6IGVudGl0eVR5cGUgfSxcclxuICAgICAgICAgICAgcmVzcG9uc2VBbmRWYWx1ZSA9PiB7XHJcbiAgICAgICAgICAgICAgICBpZiAoc2VnbWVudCAhPT0gbnVsbClcclxuICAgICAgICAgICAgICAgICAgICBzZWdtZW50LmNsYXNzTGlzdC5yZW1vdmUoXCJsb2FkaW5nXCIpO1xyXG5cclxuICAgICAgICAgICAgICAgIGlmIChyZXNwb25zZUFuZFZhbHVlLm1lc3NhZ2VUeXBlICE9PSBGYXJtQWpheE1lc3NhZ2VUeXBlLkluZm9ybWF0aW9uKVxyXG4gICAgICAgICAgICAgICAgICAgIHJlc3BvbnNlQW5kVmFsdWUucG9wdWxhdGVNZXNzYWdlQm94KGJveEVycm9yKTtcclxuICAgICAgICAgICAgICAgIGVsc2UgaWYgKHRhYmxlRm9vdGVyICE9PSBudWxsKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgdGFibGVGb290ZXIuaW5uZXJIVE1MID0gXCJcIjtcclxuXHJcbiAgICAgICAgICAgICAgICAgICAgbGV0IHRyID0gZG9jdW1lbnQuY3JlYXRlRWxlbWVudChcInRyXCIpO1xyXG4gICAgICAgICAgICAgICAgICAgIHRhYmxlRm9vdGVyLmFwcGVuZENoaWxkKHRyKTtcclxuXHJcbiAgICAgICAgICAgICAgICAgICAgbGV0IHRoID0gZG9jdW1lbnQuY3JlYXRlRWxlbWVudChcInRoXCIpO1xyXG4gICAgICAgICAgICAgICAgICAgIHRoLmNvbFNwYW4gPSA5OTk5O1xyXG4gICAgICAgICAgICAgICAgICAgIHRyLmFwcGVuZENoaWxkKHRoKTtcclxuXHJcbiAgICAgICAgICAgICAgICAgICAgbGV0IGRpdiA9IGRvY3VtZW50LmNyZWF0ZUVsZW1lbnQoJ2RpdicpO1xyXG4gICAgICAgICAgICAgICAgICAgIGRpdi5jbGFzc0xpc3QuYWRkKFwidWlcIiwgXCJjZW50ZXJcIiwgXCJhbGlnbmVkXCIsIFwicGFnaW5hdGlvblwiLCBcIm1lbnVcIik7XHJcbiAgICAgICAgICAgICAgICAgICAgdGguYXBwZW5kQ2hpbGQoZGl2KTtcclxuXHJcbiAgICAgICAgICAgICAgICAgICAgZm9yIChsZXQgaSA9IDA7IGkgPCByZXNwb25zZUFuZFZhbHVlLnZhbHVlIS52YWx1ZSE7IGkrKykge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBsZXQgYSA9IGRvY3VtZW50LmNyZWF0ZUVsZW1lbnQoXCJhXCIpO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBhLmlubmVyVGV4dCA9IFwiXCIgKyAoaSArIDEpO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBhLmNsYXNzTGlzdC5hZGQoXCJpdGVtXCIpO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBhLm9uY2xpY2sgPSBmdW5jdGlvbiAoKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBDb21wb25lbnRUYWJsZS5nZXRQYWdlKGJveEVycm9yLCB0YWJsZSwgYWpheFJlbmRlciwgaSwgZW50aXR5VHlwZSwgaXRlbXNQZXJQYWdlKTtcclxuXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICB0YWJsZUZvb3Rlci5xdWVyeVNlbGVjdG9yQWxsKFwiYVwiKSEuZm9yRWFjaChpdGVtID0+IGl0ZW0uY2xhc3NMaXN0LnJlbW92ZShcImFjdGl2ZVwiKSk7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBhLmNsYXNzTGlzdC5hZGQoXCJhY3RpdmVcIik7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgICAgICAgICAgZGl2LmFwcGVuZENoaWxkKGEpO1xyXG5cclxuICAgICAgICAgICAgICAgICAgICAgICAgbGV0IGRpdmlkZXIgPSBkb2N1bWVudC5jcmVhdGVFbGVtZW50KFwiZGl2XCIpO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBkaXZpZGVyLmNsYXNzTGlzdC5hZGQoXCJkaXZpZGVyXCIpO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBkaXYuYXBwZW5kQ2hpbGQoZGl2aWRlcik7XHJcbiAgICAgICAgICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgICAgICAgICBDb21wb25lbnRUYWJsZS5nZXRQYWdlKGJveEVycm9yLCB0YWJsZSwgYWpheFJlbmRlciwgMCwgZW50aXR5VHlwZSwgaXRlbXNQZXJQYWdlKTtcclxuICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgfVxyXG4gICAgICAgICk7XHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIHN0YXRpYyBnZXRQYWdlKFxyXG4gICAgICAgIGJveEVycm9yOiBIVE1MRGl2RWxlbWVudCxcclxuICAgICAgICB0YWJsZTogSFRNTFRhYmxlRWxlbWVudCxcclxuICAgICAgICBhamF4UmVuZGVyOiBzdHJpbmcsXHJcbiAgICAgICAgcGFnZVRvUmVuZGVyOiBudW1iZXIsXHJcbiAgICAgICAgZW50aXR5VHlwZTogc3RyaW5nIHwgbnVsbCA9IG51bGwsXHJcbiAgICAgICAgaXRlbXNQZXJQYWdlOiBudW1iZXIgfCBudWxsID0gbnVsbFxyXG4gICAgKSB7XHJcbiAgICAgICAgbGV0IHRhYmxlQm9keSA9IHRhYmxlLnRCb2RpZXMuaXRlbSgwKSE7XHJcblxyXG4gICAgICAgIGxldCBzZWdtZW50ID0gdGFibGUucGFyZW50RWxlbWVudDtcclxuICAgICAgICBpZiAoc2VnbWVudCAhPT0gbnVsbCAmJiBzZWdtZW50LmNsYXNzTGlzdC5jb250YWlucyhcInNlZ21lbnRcIikpXHJcbiAgICAgICAgICAgIHNlZ21lbnQuY2xhc3NMaXN0LmFkZChcImxvYWRpbmdcIik7XHJcbiAgICAgICAgZWxzZVxyXG4gICAgICAgICAgICBzZWdtZW50ID0gbnVsbDtcclxuXHJcbiAgICAgICAgRmFybUFqYXgucG9zdFdpdGhNZXNzYWdlQW5kVmFsdWVSZXNwb25zZTxGYXJtQWpheEdlbmVyaWNWYWx1ZTxzdHJpbmc+PihcclxuICAgICAgICAgICAgYWpheFJlbmRlcixcclxuICAgICAgICAgICAgeyBwYWdlVG9SZW5kZXI6IHBhZ2VUb1JlbmRlciwgaXRlbXNQZXJQYWdlOiBpdGVtc1BlclBhZ2UsIGVudGl0eVR5cGU6IGVudGl0eVR5cGUgfSxcclxuICAgICAgICAgICAgcmVzcG9uc2VBbmRWYWx1ZSA9PiB7XHJcbiAgICAgICAgICAgICAgICBpZiAoc2VnbWVudCAhPT0gbnVsbClcclxuICAgICAgICAgICAgICAgICAgICBzZWdtZW50LmNsYXNzTGlzdC5yZW1vdmUoXCJsb2FkaW5nXCIpO1xyXG5cclxuICAgICAgICAgICAgICAgIGlmIChyZXNwb25zZUFuZFZhbHVlLm1lc3NhZ2VUeXBlICE9PSBGYXJtQWpheE1lc3NhZ2VUeXBlLkluZm9ybWF0aW9uKVxyXG4gICAgICAgICAgICAgICAgICAgIHJlc3BvbnNlQW5kVmFsdWUucG9wdWxhdGVNZXNzYWdlQm94KGJveEVycm9yKTtcclxuICAgICAgICAgICAgICAgIGVsc2UgaWYgKHRhYmxlQm9keSAhPT0gbnVsbCkge1xyXG4gICAgICAgICAgICAgICAgICAgIHRhYmxlQm9keS5pbm5lckhUTUwgPSA8YW55PihyZXNwb25zZUFuZFZhbHVlKS52YWx1ZTtcclxuICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgfVxyXG4gICAgICAgICk7XHJcbiAgICB9XHJcbn0iXX0=