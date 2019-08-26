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
            if (response.messageType !== FarmAjaxMessageType.None)
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
            if (response.messageType !== FarmAjaxMessageType.None)
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
            if (responseAndValue.messageType !== FarmAjaxMessageType.None)
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
            if (responseAndValue.messageType !== FarmAjaxMessageType.None)
                responseAndValue.populateMessageBox(boxError);
            else if (tableBody !== null) {
                tableBody.innerHTML = (responseAndValue).value;
            }
        });
    }
}
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiY29tcG9uZW50X3RhYmxlLmpzIiwic291cmNlUm9vdCI6IiIsInNvdXJjZXMiOlsiLi4vLi4vU2NyaXB0cy9jb21wb25lbnRfdGFibGUudHMiXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IkFBQUEsT0FBTyxFQUFFLFFBQVEsRUFBRSxtQkFBbUIsRUFBd0IsTUFBTSxnQkFBZ0IsQ0FBQztBQUVyRixNQUFNLE9BQU8sY0FBYztJQUNoQixNQUFNLENBQUMsU0FBUyxDQUNuQixTQUEyQixFQUMzQixVQUE0QixFQUM1QixRQUF3QixFQUN4QixRQUF3QixFQUN4QixRQUF3QixFQUN4QixPQUFlLEVBQ2YsTUFBYyxFQUNkLEVBQVUsRUFDVixVQUFrQztRQUVsQyxJQUFJLElBQUksR0FBSSxTQUFTLENBQUMsS0FBSyxDQUFDO1FBQzVCLElBQUksS0FBSyxHQUFHLFVBQVUsQ0FBQyxLQUFLLENBQUM7UUFDN0IsSUFBSSxZQUFZLEdBQVcsRUFBRSxDQUFDO1FBQzlCLElBQUksZUFBZSxHQUFrQixFQUFFLENBQUM7UUFFeEMsSUFBSSxVQUFVLENBQUMsSUFBSSxLQUFLLE1BQU0sRUFBRTtZQUM1QixZQUFZLEdBQUcsVUFBVSxDQUFDLEtBQUssQ0FBQztZQUNoQyxlQUFlLEdBQUcsSUFBSSxDQUFDO1NBQzFCO2FBQ0ksSUFBSSxVQUFVLENBQUMsSUFBSSxLQUFLLFFBQVEsRUFBRSxFQUFFLHFDQUFxQztZQUMxRSxJQUFJLElBQUksR0FBRyxVQUFVLENBQUMsYUFBYyxDQUFDLGFBQWEsQ0FBQyxVQUFVLENBQUUsQ0FBQztZQUVoRSxJQUFJLENBQUMsZ0JBQWdCLENBQUMsVUFBVSxDQUFDO2lCQUM1QixPQUFPLENBQUMsSUFBSSxDQUFDLEVBQUU7Z0JBQ1osSUFBSSxZQUFZLENBQUMsTUFBTSxHQUFHLENBQUM7b0JBQ3ZCLE9BQU87Z0JBRVgsSUFBSyxJQUF1QixDQUFDLE9BQU8sQ0FBQyxLQUFLLElBQUksS0FBSyxJQUFJLElBQUksQ0FBQyxTQUFTLElBQUksS0FBSztvQkFDMUUs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QkFDekIsU0FBUyxDQUFDLFNBQVMsR0FBUSxDQUFDLGdCQUFnQixDQUFDLENBQUMsS0FBSyxDQUFDO2FBQ3ZEO1FBQ0wsQ0FBQyxDQUNKLENBQUM7SUFDTixDQUFDO0NBQ0oiLCJzb3VyY2VzQ29udGVudCI6WyJpbXBvcnQgeyBGYXJtQWpheCwgRmFybUFqYXhNZXNzYWdlVHlwZSwgRmFybUFqYXhHZW5lcmljVmFsdWUgfSBmcm9tIFwiLi9mYXJtX2FqYXguanNcIjtcclxuXHJcbmV4cG9ydCBjbGFzcyBDb21wb25lbnRUYWJsZSB7XHJcbiAgICBwdWJsaWMgc3RhdGljIG9uQWRkQWpheChcclxuICAgICAgICBpbnB1dE5hbWU6IEhUTUxJbnB1dEVsZW1lbnQsXHJcbiAgICAgICAgaW5wdXRWYWx1ZTogSFRNTElucHV0RWxlbWVudCxcclxuICAgICAgICBib3hFcnJvcjogSFRNTERpdkVsZW1lbnQsXHJcbiAgICAgICAgc2VnVGFibGU6IEhUTUxEaXZFbGVtZW50LFxyXG4gICAgICAgIHNlZ0lucHV0OiBIVE1MRGl2RWxlbWVudCxcclxuICAgICAgICBhamF4VXJsOiBzdHJpbmcsXHJcbiAgICAgICAgcmVhc29uOiBzdHJpbmcsXHJcbiAgICAgICAgaWQ6IG51bWJlcixcclxuICAgICAgICBkZWxldGVGdW5jOiAobmFtZTogc3RyaW5nKSA9PiB2b2lkXHJcbiAgICApIHtcclxuICAgICAgICBsZXQgbmFtZSAgPSBpbnB1dE5hbWUudmFsdWU7XHJcbiAgICAgICAgbGV0IHZhbHVlID0gaW5wdXRWYWx1ZS52YWx1ZTtcclxuICAgICAgICBsZXQgZGlzcGxheVZhbHVlOiBzdHJpbmcgPSBcIlwiO1xyXG4gICAgICAgIGxldCBkZWxldGVGdW5jUGFyYW06IHN0cmluZyB8IG51bGwgPSBcIlwiO1xyXG5cclxuICAgICAgICBpZiAoaW5wdXRWYWx1ZS50eXBlID09PSBcInRleHRcIikge1xyXG4gICAgICAgICAgICBkaXNwbGF5VmFsdWUgPSBpbnB1dFZhbHVlLnZhbHVlO1xyXG4gICAgICAgICAgICBkZWxldGVGdW5jUGFyYW0gPSBuYW1lO1xyXG4gICAgICAgIH1cclxuICAgICAgICBlbHNlIGlmIChpbnB1dFZhbHVlLnR5cGUgPT09IFwiaGlkZGVuXCIpIHsgLy8gU3BlY2lhbCBGb21hbnRpYyBVSSBTdHlsZSBkcm9wZG93blxyXG4gICAgICAgICAgICBsZXQgbWVudSA9IGlucHV0VmFsdWUucGFyZW50RWxlbWVudCEucXVlcnlTZWxlY3RvcihcImRpdi5tZW51XCIpITtcclxuXHJcbiAgICAgICAgICAgIG1lbnUucXVlcnlTZWxlY3RvckFsbChcImRpdi5pdGVtXCIpXHJcbiAgICAgICAgICAgICAgICAuZm9yRWFjaChpdGVtID0+IHtcclxuICAgICAgICAgICAgICAgICAgICBpZiAoZGlzcGxheVZhbHVlLmxlbmd0aCA+IDApXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHJldHVybjtcclxuXHJcbiAgICAgICAgICAgICAgICAgICAgaWYgKChpdGVtIGFzIEhUTUxEaXZFbGVtZW50KS5kYXRhc2V0LnZhbHVlID09IHZhbHVlIHx8IGl0ZW0uaW5uZXJIVE1MID09IHZhbHVlKVxyXG4gICAgICAgICAgICAgICAgICAgICAgICBkaXNwbGF5VmFsdWUgPSBpdGVtLmlubmVySFRNTDtcclxuICAgICAgICAgICAgICAgIH0pO1xyXG5cclxuICAgICAgICAgICAgZGVsZXRlRnVuY1BhcmFtID0gbnVsbDtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIGJveEVycm9yLmNsYXNzTGlzdC5yZW1vdmUoXCJ2aXNpYmxlXCIpO1xyXG4gICAgICAgIHNlZ1RhYmxlLmNsYXNzTGlzdC5hZGQoXCJsb2FkaW5nXCIpO1xyXG4gICAgICAgIHNlZ0lucHV0LmNsYXNzTGlzdC5hZGQoXCJsb2FkaW5nXCIpO1xyXG5cclxuICAgICAgICBGYXJtQWpheC5wb3N0V2l0aE1lc3NhZ2VSZXNwb25zZShcclxuICAgICAgICAgICAgYWpheFVybCxcclxuICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgICAgTmFtZTogbmFtZSxcclxuICAgICAgICAgICAgICAgIFZhbHVlOiB2YWx1ZSxcclxuICAgICAgICAgICAgICAgIFJlYXNvbjogcmVhc29uLFxyXG4gICAgICAgICAgICAgICAgSWQ6IGlkXHJcbiAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgIHJlc3BvbnNlID0+IHtcclxuICAgICAgICAgICAgICAgIHNlZ1RhYmxlLmNsYXNzTGlzdC5yZW1vdmUoXCJsb2FkaW5nXCIpO1xyXG4gICAgICAgICAgICAgICAgc2VnSW5wdXQuY2xhc3NMaXN0LnJlbW92ZShcImxvYWRpbmdcIik7XHJcblxyXG4gICAgICAgICAgICAgICAgaWYgKHJlc3BvbnNlLm1lc3NhZ2VUeXBlICE9PSBGYXJtQWpheE1lc3NhZ2VUeXBlLk5vbmUpXHJcbiAgICAgICAgICAgICAgICAgICAgcmVzcG9uc2UucG9wdWxhdGVNZXNzYWdlQm94KGJveEVycm9yKTtcclxuXHJcbiAgICAgICAgICAgICAgICBpZiAocmVzcG9uc2UubWVzc2FnZVR5cGUgPT09IEZhcm1BamF4TWVzc2FnZVR5cGUuSW5mb3JtYXRpb24pIHtcclxuICAgICAgICAgICAgICAgICAgICBsZXQgdHIgPSBkb2N1bWVudC5jcmVhdGVFbGVtZW50KFwidHJcIik7XHJcblxyXG4gICAgICAgICAgICAgICAgICAgIGxldCB0ZCA9IGRvY3VtZW50LmNyZWF0ZUVsZW1lbnQoXCJ0ZFwiKTtcclxuICAgICAgICAgICAgICAgICAgICB0ZC5pbm5lclRleHQgPSBuYW1lO1xyXG4gICAgICAgICAgICAgICAgICAgIHRyLmFwcGVuZENoaWxkKHRkKTtcclxuXHJcbiAgICAgICAgICAgICAgICAgICAgaWYgKHZhbHVlICE9PSBkaXNwbGF5VmFsdWUpXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHRkLmRhdGFzZXQubmFtZSA9IHZhbHVlOyAvLyBFLmcgRm9yIGRyb3Bkb3ducywgdGhlIHZpc3VhbCB2YWx1ZSAoZGlzcGxheVZhbHVlKSBhbmQgYWN0dWFsIHZhbHVlICh2YWx1ZSkgYXJlIGRpZmZlcmVudC5cclxuXHJcbiAgICAgICAgICAgICAgICAgICAgdGQgPSBkb2N1bWVudC5jcmVhdGVFbGVtZW50KFwidGRcIik7XHJcbiAgICAgICAgICAgICAgICAgICAgdGQuaW5uZXJUZXh0ID0gZGlzcGxheVZhbHVlO1xyXG4gICAgICAgICAgICAgICAgICAgIHRyLmFwcGVuZENoaWxkKHRkKTtcclxuXHJcbiAgICAgICAgICAgICAgICAgICAgdGQgPSBkb2N1bWVudC5jcmVhdGVFbGVtZW50KFwidGRcIik7XHJcblxyXG4gICAgICAgICAgICAgICAgICAgIGlmIChkZWxldGVGdW5jUGFyYW0gIT09IG51bGwpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgbGV0IGEgPSBkb2N1bWVudC5jcmVhdGVFbGVtZW50KFwiYVwiKTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgYS5jbGFzc0xpc3QuYWRkKFwidWlcIiwgXCJyZWRcIiwgXCJpbnZlcnRlZFwiLCBcImJ1dHRvblwiKTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgYS5pbm5lclRleHQgPSBcIlJlbW92ZVwiO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBhLm9uY2xpY2sgPSAoKSA9PiBkZWxldGVGdW5jKGRlbGV0ZUZ1bmNQYXJhbSk7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHRkLmFwcGVuZENoaWxkKGEpO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICB0ci5hcHBlbmRDaGlsZCh0ZCk7XHJcbiAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgICAgIGVsc2Uge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICB0ZC5pbm5lckhUTUwgPSBcIlBsZWFzZSByZWZyZXNoIHRoZSBwYWdlXCI7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHRyLmFwcGVuZENoaWxkKHRkKTtcclxuICAgICAgICAgICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICAgICAgICAgIC8vIENsZWFyIGlucHV0c1xyXG4gICAgICAgICAgICAgICAgICAgIHNlZ0lucHV0XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIC5xdWVyeVNlbGVjdG9yQWxsKFwiaW5wdXRcIilcclxuICAgICAgICAgICAgICAgICAgICAgICAgLmZvckVhY2goaSA9PiBpLnZhbHVlID0gXCJcIik7XHJcblxyXG4gICAgICAgICAgICAgICAgICAgIHNlZ1RhYmxlLnF1ZXJ5U2VsZWN0b3IoXCJ0YWJsZVwiKSEudEJvZGllcy5pdGVtKDApIS5hcHBlbmRDaGlsZCh0cik7XHJcbiAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICApO1xyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBzdGF0aWMgb25EZWxldGVBamF4KFxyXG4gICAgICAgIGJveEVycm9yOiBIVE1MRGl2RWxlbWVudCxcclxuICAgICAgICBzZWdUYWJsZTogSFRNTERpdkVsZW1lbnQsXHJcbiAgICAgICAgYWpheFVybDogc3RyaW5nLFxyXG4gICAgICAgIHJlYXNvbjogc3RyaW5nLFxyXG4gICAgICAgIHZhbHVlOiBzdHJpbmcsXHJcbiAgICAgICAgaWQ6IG51bWJlclxyXG4gICAgKSB7XHJcbiAgICAgICAgYm94RXJyb3IuY2xhc3NMaXN0LnJlbW92ZShcInZpc2libGVcIik7XHJcbiAgICAgICAgc2VnVGFibGUuY2xhc3NMaXN0LmFkZChcImxvYWRpbmdcIik7XHJcblxyXG4gICAgICAgIEZhcm1BamF4LnBvc3RXaXRoTWVzc2FnZVJlc3BvbnNlKFxyXG4gICAgICAgICAgICBhamF4VXJsLFxyXG4gICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgICBJZDogaWQsXHJcbiAgICAgICAgICAgICAgICBOYW1lOiB2YWx1ZSxcclxuICAgICAgICAgICAgICAgIFJlYXNvbjogcmVhc29uXHJcbiAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgIHJlc3BvbnNlID0+IHtcclxuICAgICAgICAgICAgICAgIHNlZ1RhYmxlLmNsYXNzTGlzdC5yZW1vdmUoXCJsb2FkaW5nXCIpO1xyXG5cclxuICAgICAgICAgICAgICAgIGlmIChyZXNwb25zZS5tZXNzYWdlVHlwZSAhPT0gRmFybUFqYXhNZXNzYWdlVHlwZS5Ob25lKVxyXG4gICAgICAgICAgICAgICAgICAgIHJlc3BvbnNlLnBvcHVsYXRlTWVzc2FnZUJveChib3hFcnJvcik7XHJcblxyXG4gICAgICAgICAgICAgICAgaWYgKHJlc3BvbnNlLm1lc3NhZ2VUeXBlID09PSBGYXJtQWpheE1lc3NhZ2VUeXBlLkluZm9ybWF0aW9uKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgLy8gRmluZCB0aGUgcm93IHdpdGggdGhlIG5hbWUsIGFuZCBkZWxldGUgaXQuXHJcbiAgICAgICAgICAgICAgICAgICAgc2VnVGFibGVcclxuICAgICAgICAgICAgICAgICAgICAgICAgLnF1ZXJ5U2VsZWN0b3JBbGwoXCJ0Ym9keSB0clwiKVxyXG4gICAgICAgICAgICAgICAgICAgICAgICAuZm9yRWFjaCgocm93KSA9PiB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBsZXQgdGQgPSAocm93IGFzIEhUTUxUYWJsZVJvd0VsZW1lbnQpLmNlbGxzLml0ZW0oMCkhO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgaWYgKHRkLmlubmVyVGV4dCA9PT0gdmFsdWUgfHwgdGQuZGF0YXNldC5uYW1lID09IHZhbHVlKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgc2VnVGFibGUucXVlcnlTZWxlY3RvcihcInRib2R5XCIpIS5yZW1vdmVDaGlsZChyb3cpO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIHJldHVybjtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgICAgICAgICAgfSk7XHJcbiAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICApO1xyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBzdGF0aWMgc2V0dXBQYWdpbmdUYWJsZShcclxuICAgICAgICBib3hFcnJvcjogSFRNTERpdkVsZW1lbnQsXHJcbiAgICAgICAgdGFibGU6IEhUTUxUYWJsZUVsZW1lbnQsXHJcbiAgICAgICAgYWpheFBhZ2VDb3VudDogc3RyaW5nLFxyXG4gICAgICAgIGFqYXhSZW5kZXI6IHN0cmluZyxcclxuICAgICAgICBlbnRpdHlUeXBlOiBzdHJpbmcgfCBudWxsID0gbnVsbCxcclxuICAgICAgICBpdGVtc1BlclBhZ2U6IG51bWJlciB8IG51bGwgPSBudWxsXHJcbiAgICApIHtcclxuICAgICAgICBsZXQgdGFibGVGb290ZXIgPSB0YWJsZS50Rm9vdCE7XHJcblxyXG4gICAgICAgIGxldCBzZWdtZW50ID0gdGFibGUucGFyZW50RWxlbWVudDtcclxuICAgICAgICBpZiAoc2VnbWVudCAhPT0gbnVsbCAmJiBzZWdtZW50LmNsYXNzTGlzdC5jb250YWlucyhcInNlZ21lbnRcIikpXHJcbiAgICAgICAgICAgIHNlZ21lbnQuY2xhc3NMaXN0LmFkZChcImxvYWRpbmdcIik7XHJcbiAgICAgICAgZWxzZVxyXG4gICAgICAgICAgICBzZWdtZW50ID0gbnVsbDtcclxuXHJcbiAgICAgICAgRmFybUFqYXgucG9zdFdpdGhNZXNzYWdlQW5kVmFsdWVSZXNwb25zZTxGYXJtQWpheEdlbmVyaWNWYWx1ZTxudW1iZXI+PihcclxuICAgICAgICAgICAgYWpheFBhZ2VDb3VudCxcclxuICAgICAgICAgICAgeyBpdGVtc1BlclBhZ2U6IGl0ZW1zUGVyUGFnZSwgZW50aXR5VHlwZTogZW50aXR5VHlwZSB9LFxyXG4gICAgICAgICAgICByZXNwb25zZUFuZFZhbHVlID0+IHtcclxuICAgICAgICAgICAgICAgIGlmIChzZWdtZW50ICE9PSBudWxsKVxyXG4gICAgICAgICAgICAgICAgICAgIHNlZ21lbnQuY2xhc3NMaXN0LnJlbW92ZShcImxvYWRpbmdcIik7XHJcblxyXG4gICAgICAgICAgICAgICAgaWYgKHJlc3BvbnNlQW5kVmFsdWUubWVzc2FnZVR5cGUgIT09IEZhcm1BamF4TWVzc2FnZVR5cGUuTm9uZSlcclxuICAgICAgICAgICAgICAgICAgICByZXNwb25zZUFuZFZhbHVlLnBvcHVsYXRlTWVzc2FnZUJveChib3hFcnJvcik7XHJcbiAgICAgICAgICAgICAgICBlbHNlIGlmICh0YWJsZUZvb3RlciAhPT0gbnVsbCkge1xyXG4gICAgICAgICAgICAgICAgICAgIHRhYmxlRm9vdGVyLmlubmVySFRNTCA9IFwiXCI7XHJcblxyXG4gICAgICAgICAgICAgICAgICAgIGxldCB0ciA9IGRvY3VtZW50LmNyZWF0ZUVsZW1lbnQoXCJ0clwiKTtcclxuICAgICAgICAgICAgICAgICAgICB0YWJsZUZvb3Rlci5hcHBlbmRDaGlsZCh0cik7XHJcblxyXG4gICAgICAgICAgICAgICAgICAgIGxldCB0aCA9IGRvY3VtZW50LmNyZWF0ZUVsZW1lbnQoXCJ0aFwiKTtcclxuICAgICAgICAgICAgICAgICAgICB0aC5jb2xTcGFuID0gOTk5OTtcclxuICAgICAgICAgICAgICAgICAgICB0ci5hcHBlbmRDaGlsZCh0aCk7XHJcblxyXG4gICAgICAgICAgICAgICAgICAgIGxldCBkaXYgPSBkb2N1bWVudC5jcmVhdGVFbGVtZW50KCdkaXYnKTtcclxuICAgICAgICAgICAgICAgICAgICBkaXYuY2xhc3NMaXN0LmFkZChcInVpXCIsIFwiY2VudGVyXCIsIFwiYWxpZ25lZFwiLCBcInBhZ2luYXRpb25cIiwgXCJtZW51XCIpO1xyXG4gICAgICAgICAgICAgICAgICAgIHRoLmFwcGVuZENoaWxkKGRpdik7XHJcblxyXG4gICAgICAgICAgICAgICAgICAgIGZvciAobGV0IGkgPSAwOyBpIDwgcmVzcG9uc2VBbmRWYWx1ZS52YWx1ZSEudmFsdWUhOyBpKyspIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgbGV0IGEgPSBkb2N1bWVudC5jcmVhdGVFbGVtZW50KFwiYVwiKTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgYS5pbm5lclRleHQgPSBcIlwiICsgKGkgKyAxKTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgYS5jbGFzc0xpc3QuYWRkKFwiaXRlbVwiKTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgYS5vbmNsaWNrID0gZnVuY3Rpb24gKCkge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgQ29tcG9uZW50VGFibGUuZ2V0UGFnZShib3hFcnJvciwgdGFibGUsIGFqYXhSZW5kZXIsIGksIGVudGl0eVR5cGUsIGl0ZW1zUGVyUGFnZSk7XHJcblxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgdGFibGVGb290ZXIucXVlcnlTZWxlY3RvckFsbChcImFcIikhLmZvckVhY2goaXRlbSA9PiBpdGVtLmNsYXNzTGlzdC5yZW1vdmUoXCJhY3RpdmVcIikpO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgYS5jbGFzc0xpc3QuYWRkKFwiYWN0aXZlXCIpO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGRpdi5hcHBlbmRDaGlsZChhKTtcclxuXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGxldCBkaXZpZGVyID0gZG9jdW1lbnQuY3JlYXRlRWxlbWVudChcImRpdlwiKTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgZGl2aWRlci5jbGFzc0xpc3QuYWRkKFwiZGl2aWRlclwiKTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgZGl2LmFwcGVuZENoaWxkKGRpdmlkZXIpO1xyXG4gICAgICAgICAgICAgICAgICAgIH1cclxuXHJcbiAgICAgICAgICAgICAgICAgICAgQ29tcG9uZW50VGFibGUuZ2V0UGFnZShib3hFcnJvciwgdGFibGUsIGFqYXhSZW5kZXIsIDAsIGVudGl0eVR5cGUsIGl0ZW1zUGVyUGFnZSk7XHJcbiAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICApO1xyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBzdGF0aWMgZ2V0UGFnZShcclxuICAgICAgICBib3hFcnJvcjogSFRNTERpdkVsZW1lbnQsXHJcbiAgICAgICAgdGFibGU6IEhUTUxUYWJsZUVsZW1lbnQsXHJcbiAgICAgICAgYWpheFJlbmRlcjogc3RyaW5nLFxyXG4gICAgICAgIHBhZ2VUb1JlbmRlcjogbnVtYmVyLFxyXG4gICAgICAgIGVudGl0eVR5cGU6IHN0cmluZyB8IG51bGwgPSBudWxsLFxyXG4gICAgICAgIGl0ZW1zUGVyUGFnZTogbnVtYmVyIHwgbnVsbCA9IG51bGxcclxuICAgICkge1xyXG4gICAgICAgIGxldCB0YWJsZUJvZHkgPSB0YWJsZS50Qm9kaWVzLml0ZW0oMCkhO1xyXG5cclxuICAgICAgICBsZXQgc2VnbWVudCA9IHRhYmxlLnBhcmVudEVsZW1lbnQ7XHJcbiAgICAgICAgaWYgKHNlZ21lbnQgIT09IG51bGwgJiYgc2VnbWVudC5jbGFzc0xpc3QuY29udGFpbnMoXCJzZWdtZW50XCIpKVxyXG4gICAgICAgICAgICBzZWdtZW50LmNsYXNzTGlzdC5hZGQoXCJsb2FkaW5nXCIpO1xyXG4gICAgICAgIGVsc2VcclxuICAgICAgICAgICAgc2VnbWVudCA9IG51bGw7XHJcblxyXG4gICAgICAgIEZhcm1BamF4LnBvc3RXaXRoTWVzc2FnZUFuZFZhbHVlUmVzcG9uc2U8RmFybUFqYXhHZW5lcmljVmFsdWU8c3RyaW5nPj4oXHJcbiAgICAgICAgICAgIGFqYXhSZW5kZXIsXHJcbiAgICAgICAgICAgIHsgcGFnZVRvUmVuZGVyOiBwYWdlVG9SZW5kZXIsIGl0ZW1zUGVyUGFnZTogaXRlbXNQZXJQYWdlLCBlbnRpdHlUeXBlOiBlbnRpdHlUeXBlIH0sXHJcbiAgICAgICAgICAgIHJlc3BvbnNlQW5kVmFsdWUgPT4ge1xyXG4gICAgICAgICAgICAgICAgaWYgKHNlZ21lbnQgIT09IG51bGwpXHJcbiAgICAgICAgICAgICAgICAgICAgc2VnbWVudC5jbGFzc0xpc3QucmVtb3ZlKFwibG9hZGluZ1wiKTtcclxuXHJcbiAgICAgICAgICAgICAgICBpZiAocmVzcG9uc2VBbmRWYWx1ZS5tZXNzYWdlVHlwZSAhPT0gRmFybUFqYXhNZXNzYWdlVHlwZS5Ob25lKVxyXG4gICAgICAgICAgICAgICAgICAgIHJlc3BvbnNlQW5kVmFsdWUucG9wdWxhdGVNZXNzYWdlQm94KGJveEVycm9yKTtcclxuICAgICAgICAgICAgICAgIGVsc2UgaWYgKHRhYmxlQm9keSAhPT0gbnVsbCkge1xyXG4gICAgICAgICAgICAgICAgICAgIHRhYmxlQm9keS5pbm5lckhUTUwgPSA8YW55PihyZXNwb25zZUFuZFZhbHVlKS52YWx1ZTtcclxuICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgfVxyXG4gICAgICAgICk7XHJcbiAgICB9XHJcbn0iXX0=