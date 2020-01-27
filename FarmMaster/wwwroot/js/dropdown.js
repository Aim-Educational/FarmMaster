import { GraphQL } from "./graphql.js";
export class Dropdown {
    constructor(dropdownNodeOrId) {
        if (dropdownNodeOrId instanceof HTMLDivElement)
            this.dropdownNode = dropdownNodeOrId;
        else
            this.dropdownNode = document.getElementById(dropdownNodeOrId);
        if (!this.dropdownNode)
            throw "The dropdown node is null. Parameter given was: " + dropdownNodeOrId;
        // For now, we only support Fomantic UI style, since I doubt it'll be replaced anytime soon.
        this.inputNode = this.dropdownNode.querySelector("div.dropdown input[type=hidden]");
        if (!this.inputNode)
            throw "Could not find the input element. Is your markup in Fomantic UI style?";
        this.menuNode = this.dropdownNode.querySelector("div.dropdown div.menu");
        if (!this.menuNode)
            throw "Could not find the menu element. Is your markup in Fomantic UI style?";
        this.defaultValue = this.inputNode.dataset.defaultValue;
        const refreshButton = this.dropdownNode.querySelector("label .button[data-type=refresh]");
        refreshButton.addEventListener("click", () => this.refresh());
        // Just to make sure Fomantic UI knows about it, saving the user the hassle.
        $(this.menuNode.parentNode).dropdown({ forceSelection: false });
        this._refreshFunc = function () { alert("No refresh function has been assigned."); };
    }
    // FUNCTIONS TO MANIPULATE ITEMS
    addItem(
    // Gotta love TS' syntax.
    { name, value = null, isSelected = false }) {
        const item = this.menuNode.appendChild(document.createElement("div"));
        item.classList.add("item");
        item.innerText = name;
        if (value)
            item.dataset.value = value;
        if (isSelected)
            $(this.menuNode.parentNode).dropdown("set selected", value || name);
        return item;
    }
    clear() {
        while (this.menuNode.firstChild)
            this.menuNode.removeChild(this.menuNode.firstChild);
    }
    refresh() {
        this._refreshFunc();
    }
    // GENERIC DATA SOURCES
    fromRefreshFunc(func) {
        this._refreshFunc = func;
    }
    fromGraphQL({ query, parameters = null, dataGetter }) {
        this.fromRefreshFunc(() => {
            GraphQL
                .query(query, parameters)
                .then((data) => {
                this.clear();
                const nameValuePairs = dataGetter(data);
                for (const pair of nameValuePairs)
                    this.addItem({ name: pair.name, value: pair.value, isSelected: pair.value == this.defaultValue });
            })
                .catch((reason) => {
                alert("TEMP error handling: " + JSON.stringify(reason));
            });
        });
        this.refresh();
        this.refresh(); // Very weird bug where, during the first refresh, Fomantic UI won't perform "set selected" correctly.
    }
    // COMMON DROPDOWN DATA SOURCES
    //
    // If the data source is used in more than one page, its probably best to put it here.
    fromContactGraphQL() {
        this.fromGraphQL({
            query: `query GetOwners {
                contacts {
                    id
                    name
                }
            }`,
            dataGetter: (json) => json.contacts.map(function (v) {
                return { name: v.name, value: String(v.id) };
            })
        });
    }
}
export default Dropdown;
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiZHJvcGRvd24uanMiLCJzb3VyY2VSb290IjoiIiwic291cmNlcyI6WyIuLi8uLi9TY3JpcHRzL2Ryb3Bkb3duLnRzIl0sIm5hbWVzIjpbXSwibWFwcGluZ3MiOiJBQUFBLE9BQU8sRUFBRSxPQUFPLEVBQUUsTUFBTSxjQUFjLENBQUE7QUFFdEMsTUFBTSxPQUFPLFFBQVE7SUFPakIsWUFBWSxnQkFBeUM7UUFDakQsSUFBSSxnQkFBZ0IsWUFBWSxjQUFjO1lBQzFDLElBQUksQ0FBQyxZQUFZLEdBQUcsZ0JBQWdCLENBQUM7O1lBRXJDLElBQUksQ0FBQyxZQUFZLEdBQW1CLFFBQVEsQ0FBQyxjQUFjLENBQUMsZ0JBQWdCLENBQUMsQ0FBQztRQUVsRixJQUFJLENBQUMsSUFBSSxDQUFDLFlBQVk7WUFDbEIsTUFBTSxrREFBa0QsR0FBRyxnQkFBZ0IsQ0FBQztRQUVoRiw0RkFBNEY7UUFDNUYsSUFBSSxDQUFDLFNBQVMsR0FBcUIsSUFBSSxDQUFDLFlBQVksQ0FBQyxhQUFhLENBQUMsaUNBQWlDLENBQUMsQ0FBQztRQUN0RyxJQUFJLENBQUMsSUFBSSxDQUFDLFNBQVM7WUFDZixNQUFNLHdFQUF3RSxDQUFDO1FBRW5GLElBQUksQ0FBQyxRQUFRLEdBQW1CLElBQUksQ0FBQyxZQUFZLENBQUMsYUFBYSxDQUFDLHVCQUF1QixDQUFDLENBQUM7UUFDekYsSUFBSSxDQUFDLElBQUksQ0FBQyxRQUFRO1lBQ2QsTUFBTSx1RUFBdUUsQ0FBQztRQUVsRixJQUFJLENBQUMsWUFBWSxHQUFXLElBQUksQ0FBQyxTQUFTLENBQUMsT0FBTyxDQUFDLFlBQVksQ0FBQztRQUVoRSxNQUFNLGFBQWEsR0FBZ0IsSUFBSSxDQUFDLFlBQVksQ0FBQyxhQUFhLENBQUMsa0NBQWtDLENBQUMsQ0FBQztRQUN2RyxhQUFhLENBQUMsZ0JBQWdCLENBQUMsT0FBTyxFQUFFLEdBQUcsRUFBRSxDQUFDLElBQUksQ0FBQyxPQUFPLEVBQUUsQ0FBQyxDQUFDO1FBRTlELDRFQUE0RTtRQUM1RSxDQUFDLENB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Q0FBQyxJQUFJLEVBQUUsS0FBSyxFQUFFLE1BQU0sQ0FBQyxDQUFDLENBQUMsRUFBRSxDQUFDLEVBQUUsQ0FBQztZQUNqRCxDQUFDLENBQUM7U0FDTCxDQUFDLENBQUM7SUFDUCxDQUFDO0NBQ0o7QUFFRCxlQUFlLFFBQVEsQ0FBQyIsInNvdXJjZXNDb250ZW50IjpbImltcG9ydCB7IEdyYXBoUUwgfSBmcm9tIFwiLi9ncmFwaHFsLmpzXCJcclxuXHJcbmV4cG9ydCBjbGFzcyBEcm9wZG93biB7XHJcbiAgICBwdWJsaWMgcmVhZG9ubHkgZHJvcGRvd25Ob2RlOiBIVE1MRGl2RWxlbWVudDtcclxuICAgIHB1YmxpYyByZWFkb25seSBpbnB1dE5vZGU6IEhUTUxJbnB1dEVsZW1lbnQ7XHJcbiAgICBwdWJsaWMgcmVhZG9ubHkgbWVudU5vZGU6IEhUTUxEaXZFbGVtZW50O1xyXG4gICAgcHVibGljIHJlYWRvbmx5IGRlZmF1bHRWYWx1ZTogc3RyaW5nO1xyXG4gICAgcHJpdmF0ZSBfcmVmcmVzaEZ1bmM6IEZ1bmN0aW9uO1xyXG5cclxuICAgIGNvbnN0cnVjdG9yKGRyb3Bkb3duTm9kZU9ySWQ6IEhUTUxEaXZFbGVtZW50IHwgc3RyaW5nKSB7XHJcbiAgICAgICAgaWYgKGRyb3Bkb3duTm9kZU9ySWQgaW5zdGFuY2VvZiBIVE1MRGl2RWxlbWVudClcclxuICAgICAgICAgICAgdGhpcy5kcm9wZG93bk5vZGUgPSBkcm9wZG93bk5vZGVPcklkO1xyXG4gICAgICAgIGVsc2VcclxuICAgICAgICAgICAgdGhpcy5kcm9wZG93bk5vZGUgPSA8SFRNTERpdkVsZW1lbnQ+ZG9jdW1lbnQuZ2V0RWxlbWVudEJ5SWQoZHJvcGRvd25Ob2RlT3JJZCk7XHJcblxyXG4gICAgICAgIGlmICghdGhpcy5kcm9wZG93bk5vZGUpXHJcbiAgICAgICAgICAgIHRocm93IFwiVGhlIGRyb3Bkb3duIG5vZGUgaXMgbnVsbC4gUGFyYW1ldGVyIGdpdmVuIHdhczogXCIgKyBkcm9wZG93bk5vZGVPcklkO1xyXG5cclxuICAgICAgICAvLyBGb3Igbm93LCB3ZSBvbmx5IHN1cHBvcnQgRm9tYW50aWMgVUkgc3R5bGUsIHNpbmNlIEkgZG91YnQgaXQnbGwgYmUgcmVwbGFjZWQgYW55dGltZSBzb29uLlxyXG4gICAgICAgIHRoaXMuaW5wdXROb2RlID0gPEhUTUxJbnB1dEVsZW1lbnQ+dGhpcy5kcm9wZG93bk5vZGUucXVlcnlTZWxlY3RvcihcImRpdi5kcm9wZG93biBpbnB1dFt0eXBlPWhpZGRlbl1cIik7XHJcbiAgICAgICAgaWYgKCF0aGlzLmlucHV0Tm9kZSlcclxuICAgICAgICAgICAgdGhyb3cgXCJDb3VsZCBub3QgZmluZCB0aGUgaW5wdXQgZWxlbWVudC4gSXMgeW91ciBtYXJrdXAgaW4gRm9tYW50aWMgVUkgc3R5bGU/XCI7XHJcblxyXG4gICAgICAgIHRoaXMubWVudU5vZGUgPSA8SFRNTERpdkVsZW1lbnQ+dGhpcy5kcm9wZG93bk5vZGUucXVlcnlTZWxlY3RvcihcImRpdi5kcm9wZG93biBkaXYubWVudVwiKTtcclxuICAgICAgICBpZiAoIXRoaXMubWVudU5vZGUpXHJcbiAgICAgICAgICAgIHRocm93IFwiQ291bGQgbm90IGZpbmQgdGhlIG1lbnUgZWxlbWVudC4gSXMgeW91ciBtYXJrdXAgaW4gRm9tYW50aWMgVUkgc3R5bGU/XCI7XHJcblxyXG4gICAgICAgIHRoaXMuZGVmYXVsdFZhbHVlID0gPHN0cmluZz50aGlzLmlucHV0Tm9kZS5kYXRhc2V0LmRlZmF1bHRWYWx1ZTtcclxuXHJcbiAgICAgICAgY29uc3QgcmVmcmVzaEJ1dHRvbiA9IDxIVE1MRWxlbWVudD50aGlzLmRyb3Bkb3duTm9kZS5xdWVyeVNlbGVjdG9yKFwibGFiZWwgLmJ1dHRvbltkYXRhLXR5cGU9cmVmcmVzaF1cIik7XHJcbiAgICAgICAgcmVmcmVzaEJ1dHRvbi5hZGRFdmVudExpc3RlbmVyKFwiY2xpY2tcIiwgKCkgPT4gdGhpcy5yZWZyZXNoKCkpO1xyXG5cclxuICAgICAgICAvLyBKdXN0IHRvIG1ha2Ugc3VyZSBGb21hbnRpYyBVSSBrbm93cyBhYm91dCBpdCwgc2F2aW5nIHRoZSB1c2VyIHRoZSBoYXNzbGUuXHJcbiAgICAgICAgJCg8YW55PnRoaXMubWVudU5vZGUucGFyZW50Tm9kZSkuZHJvcGRvd24oeyBmb3JjZVNlbGVjdGlvbjogZmFsc2UgfSk7XHJcblxyXG4gICAgICAgIHRoaXMuX3JlZnJlc2hGdW5jID0gZnVuY3Rpb24gKCkgeyBhbGVydChcIk5vIHJlZnJlc2ggZnVuY3Rpb24gaGFzIGJlZW4gYXNzaWduZWQuXCIpOyB9O1xyXG4gICAgfVxyXG5cclxuICAgIC8vIEZVTkNUSU9OUyBUTyBNQU5JUFVMQVRFIElURU1TXHJcblxyXG4gICAgcHVibGljIGFkZEl0ZW0oXHJcbiAgICAgICAgLy8gR290dGEgbG92ZSBUUycgc3ludGF4LlxyXG4gICAgICAgIHsgbmFtZSwgICAgICAgICB2YWx1ZSA9IG51bGwsICAgICAgICAgIGlzU2VsZWN0ZWQgPSBmYWxzZSB9OlxyXG4gICAgICAgIHsgbmFtZTogc3RyaW5nLCB2YWx1ZT86IHN0cmluZyB8IG51bGwsIGlzU2VsZWN0ZWQ6IGJvb2xlYW4gfVxyXG4gICAgKTogSFRNTERpdkVsZW1lbnQge1xyXG4gICAgICAgIGNvbnN0IGl0ZW0gPSB0aGlzLm1lbnVOb2RlLmFwcGVuZENoaWxkKGRvY3VtZW50LmNyZWF0ZUVsZW1lbnQoXCJkaXZcIikpO1xyXG4gICAgICAgIGl0ZW0uY2xhc3NMaXN0LmFkZChcIml0ZW1cIik7XHJcbiAgICAgICAgaXRlbS5pbm5lclRleHQgPSBuYW1lO1xyXG5cclxuICAgICAgICBpZiAodmFsdWUpXHJcbiAgICAgICAgICAgIGl0ZW0uZGF0YXNldC52YWx1ZSA9IHZhbHVlO1xyXG5cclxuICAgICAgICBpZiAoaXNTZWxlY3RlZClcclxuICAgICAgICAgICAgJCg8YW55PnRoaXMubWVudU5vZGUucGFyZW50Tm9kZSkuZHJvcGRvd24oXCJzZXQgc2VsZWN0ZWRcIiwgdmFsdWUgfHwgbmFtZSlcclxuXHJcbiAgICAgICAgcmV0dXJuIGl0ZW07XHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIGNsZWFyKCkge1xyXG4gICAgICAgIHdoaWxlICh0aGlzLm1lbnVOb2RlLmZpcnN0Q2hpbGQpXHJcbiAgICAgICAgICAgIHRoaXMubWVudU5vZGUucmVtb3ZlQ2hpbGQodGhpcy5tZW51Tm9kZS5maXJzdENoaWxkKTtcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgcmVmcmVzaCgpIHtcclxuICAgICAgICB0aGlzLl9yZWZyZXNoRnVuYygpO1xyXG4gICAgfVxyXG5cclxuICAgIC8vIEdFTkVSSUMgREFUQSBTT1VSQ0VTXHJcblxyXG4gICAgcHVibGljIGZyb21SZWZyZXNoRnVuYyhmdW5jOiBGdW5jdGlvbikge1xyXG4gICAgICAgIHRoaXMuX3JlZnJlc2hGdW5jID0gZnVuYztcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgZnJvbUdyYXBoUUwoXHJcbiAgICAgICAgeyBxdWVyeSwgICAgICAgICBwYXJhbWV0ZXJzID0gbnVsbCwgICAgICAgICAgZGF0YUdldHRlciB9OlxyXG4gICAgICAgIHsgcXVlcnk6IHN0cmluZywgcGFyYW1ldGVycz86IG9iamVjdCB8IG51bGwsIGRhdGFHZXR0ZXI6IChkYXRhOiBhbnkpID0+IHsgbmFtZTogc3RyaW5nLCB2YWx1ZTogc3RyaW5nIH1bXSB9XHJcbiAgICApIHtcclxuICAgICAgICB0aGlzLmZyb21SZWZyZXNoRnVuYygoKSA9PiB7XHJcbiAgICAgICAgICAgIEdyYXBoUUxcclxuICAgICAgICAgICAgICAgIC5xdWVyeShxdWVyeSwgcGFyYW1ldGVycylcclxuICAgICAgICAgICAgICAgIC50aGVuKChkYXRhKSA9PiB7XHJcbiAgICAgICAgICAgICAgICAgICAgdGhpcy5jbGVhcigpO1xyXG5cclxuICAgICAgICAgICAgICAgICAgICBjb25zdCBuYW1lVmFsdWVQYWlycyA9IGRhdGFHZXR0ZXIoZGF0YSk7XHJcbiAgICAgICAgICAgICAgICAgICAgZm9yIChjb25zdCBwYWlyIG9mIG5hbWVWYWx1ZVBhaXJzKVxyXG4gICAgICAgICAgICAgICAgICAgICAgICB0aGlzLmFkZEl0ZW0oeyBuYW1lOiBwYWlyLm5hbWUsIHZhbHVlOiBwYWlyLnZhbHVlLCBpc1NlbGVjdGVkOiBwYWlyLnZhbHVlID09IHRoaXMuZGVmYXVsdFZhbHVlIH0pO1xyXG4gICAgICAgICAgICAgICAgfSlcclxuICAgICAgICAgICAgICAgIC5jYXRjaCgocmVhc29uKSA9PiB7XHJcbiAgICAgICAgICAgICAgICAgICAgYWxlcnQoXCJURU1QIGVycm9yIGhhbmRsaW5nOiBcIiArIEpTT04uc3RyaW5naWZ5KHJlYXNvbikpO1xyXG4gICAgICAgICAgICAgICAgfSk7XHJcbiAgICAgICAgfSk7XHJcbiAgICAgICAgdGhpcy5yZWZyZXNoKCk7XHJcbiAgICAgICAgdGhpcy5yZWZyZXNoKCk7IC8vIFZlcnkgd2VpcmQgYnVnIHdoZXJlLCBkdXJpbmcgdGhlIGZpcnN0IHJlZnJlc2gsIEZvbWFudGljIFVJIHdvbid0IHBlcmZvcm0gXCJzZXQgc2VsZWN0ZWRcIiBjb3JyZWN0bHkuXHJcbiAgICB9XHJcblxyXG4gICAgLy8gQ09NTU9OIERST1BET1dOIERBVEEgU09VUkNFU1xyXG4gICAgLy9cclxuICAgIC8vIElmIHRoZSBkYXRhIHNvdXJjZSBpcyB1c2VkIGluIG1vcmUgdGhhbiBvbmUgcGFnZSwgaXRzIHByb2JhYmx5IGJlc3QgdG8gcHV0IGl0IGhlcmUuXHJcblxyXG4gICAgcHVibGljIGZyb21Db250YWN0R3JhcGhRTCgpIHtcclxuICAgICAgICB0aGlzLmZyb21HcmFwaFFMKHtcclxuICAgICAgICAgICAgcXVlcnk6IGBxdWVyeSBHZXRPd25lcnMge1xyXG4gICAgICAgICAgICAgICAgY29udGFjdHMge1xyXG4gICAgICAgICAgICAgICAgICAgIGlkXHJcbiAgICAgICAgICAgICAgICAgICAgbmFtZVxyXG4gICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICB9YCxcclxuXHJcbiAgICAgICAgICAgIGRhdGFHZXR0ZXI6IChqc29uOiB7IGNvbnRhY3RzOiB7IG5hbWU6IHN0cmluZywgaWQ6IG51bWJlciB9W10gfSkgPT4ganNvbi5jb250YWN0cy5tYXAoZnVuY3Rpb24gKHYpIHtcclxuICAgICAgICAgICAgICAgIHJldHVybiB7IG5hbWU6IHYubmFtZSwgdmFsdWU6IFN0cmluZyh2LmlkKSB9O1xyXG4gICAgICAgICAgICB9KVxyXG4gICAgICAgIH0pO1xyXG4gICAgfVxyXG59XHJcblxyXG5leHBvcnQgZGVmYXVsdCBEcm9wZG93bjsiXX0=