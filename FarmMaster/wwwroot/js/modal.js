export class Modal {
    static askForReason() {
        const modal = document.getElementById("modalReason");
        if (modal === null) {
            alert("Dev error: No element with ID of 'modalReason'");
            return null;
        }
        const inputReason = modal.querySelector("input.reason");
        if (inputReason === null) {
            alert("Dev error: Modal does not contain an <input> with class of 'reason'");
            return null;
        }
        return new Promise((resolve, reject) => {
            $(modal)
                .modal({
                onApprove: function () {
                    resolve(inputReason.value);
                    inputReason.value = "";
                },
                onDeny: function () {
                    reject("User canceled action.");
                    inputReason.value = "";
                }
            })
                .modal("show");
        });
    }
}
export default Modal;
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoibW9kYWwuanMiLCJzb3VyY2VSb290IjoiIiwic291cmNlcyI6WyIuLi8uLi9TY3JpcHRzL21vZGFsLnRzIl0sIm5hbWVzIjpbXSwibWFwcGluZ3MiOiJBQUFBLE1BQU0sT0FBTyxLQUFLO0lBQ1AsTUFBTSxDQUFDLFlBQVk7UUFDdEIsTUFBTSxLQUFLLEdBQUcsUUFBUSxDQUFDLGNBQWMsQ0FBQyxhQUFhLENBQUMsQ0FBQztRQUNyRCxJQUFJLEtBQUssS0FBSyxJQUFJLEVBQUU7WUFDaEIsS0FBSyxDQUFDLGdEQUFnRCxDQUFDLENBQUM7WUFDeEQsT0FBTyxJQUFJLENBQUM7U0FDZjtRQUVELE1BQU0sV0FBVyxHQUFHLEtBQUssQ0FBQyxhQUFhLENBQUMsY0FBYyxDQUFxQixDQUFDO1FBQzVFLElBQUksV0FBVyxLQUFLLElBQUksRUFBRTtZQUN0QixLQUFLLENBQUMscUVBQXFFLENBQUMsQ0FBQztZQUM3RSxPQUFPLElBQUksQ0FBQztTQUNmO1FBRUQsT0FBTyxJQUFJLE9BQU8sQ0FBQyxDQUFDLE9BQU8sRUFBRSxNQUFNLEVBQUUsRUFBRTtZQUNuQyxDQUFDLENBQUMsS0FBSyxDQUFDO2lCQUNILEtBQUssQ0FBQztnQkFDSCxTQUFTLEVBQUU7b0JBQ1AsT0FBTyxDQUFDLFdBQVcsQ0FBQyxLQUFLLENBQUMsQ0FBQztvQkFDM0IsV0FBVyxDQUFDLEtBQUssR0FBRyxFQUFFLENBQUM7Z0JBQzNCLENBQUM7Z0JBRUQsTUFBTSxFQUFFO29CQUNKLE1BQU0sQ0FBQyx1QkFBdUIsQ0FBQyxDQUFDO29CQUNoQyxXQUFXLENBQUMsS0FBSyxHQUFHLEVBQUUsQ0FBQztnQkFDM0IsQ0FBQzthQUNKLENBQUM7aUJBQ0QsS0FBSyxDQUFDLE1BQU0sQ0FBQyxDQUFDO1FBQ3ZCLENBQUMsQ0FBQyxDQUFDO0lBQ1AsQ0FBQztDQUNKO0FBRUQsZUFBZSxLQUFLLENBQUMiLCJzb3VyY2VzQ29udGVudCI6WyJleHBvcnQgY2xhc3MgTW9kYWwge1xyXG4gICAgcHVibGljIHN0YXRpYyBhc2tGb3JSZWFzb24oKTogUHJvbWlzZTxzdHJpbmc+IHwgbnVsbCB7XHJcbiAgICAgICAgY29uc3QgbW9kYWwgPSBkb2N1bWVudC5nZXRFbGVtZW50QnlJZChcIm1vZGFsUmVhc29uXCIpO1xyXG4gICAgICAgIGlmIChtb2RhbCA9PT0gbnVsbCkge1xyXG4gICAgICAgICAgICBhbGVydChcIkRldiBlcnJvcjogTm8gZWxlbWVudCB3aXRoIElEIG9mICdtb2RhbFJlYXNvbidcIik7XHJcbiAgICAgICAgICAgIHJldHVybiBudWxsO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgY29uc3QgaW5wdXRSZWFzb24gPSBtb2RhbC5xdWVyeVNlbGVjdG9yKFwiaW5wdXQucmVhc29uXCIpIGFzIEhUTUxJbnB1dEVsZW1lbnQ7XHJcbiAgICAgICAgaWYgKGlucHV0UmVhc29uID09PSBudWxsKSB7XHJcbiAgICAgICAgICAgIGFsZXJ0KFwiRGV2IGVycm9yOiBNb2RhbCBkb2VzIG5vdCBjb250YWluIGFuIDxpbnB1dD4gd2l0aCBjbGFzcyBvZiAncmVhc29uJ1wiKTtcclxuICAgICAgICAgICAgcmV0dXJuIG51bGw7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICByZXR1cm4gbmV3IFByb21pc2UoKHJlc29sdmUsIHJlamVjdCkgPT4ge1xyXG4gICAgICAgICAgICAkKG1vZGFsKVxyXG4gICAgICAgICAgICAgICAgLm1vZGFsKHtcclxuICAgICAgICAgICAgICAgICAgICBvbkFwcHJvdmU6IGZ1bmN0aW9uICgpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgcmVzb2x2ZShpbnB1dFJlYXNvbi52YWx1ZSk7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGlucHV0UmVhc29uLnZhbHVlID0gXCJcIjtcclxuICAgICAgICAgICAgICAgICAgICB9LFxyXG5cclxuICAgICAgICAgICAgICAgICAgICBvbkRlbnk6IGZ1bmN0aW9uICgpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgcmVqZWN0KFwiVXNlciBjYW5jZWxlZCBhY3Rpb24uXCIpO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBpbnB1dFJlYXNvbi52YWx1ZSA9IFwiXCI7XHJcbiAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgfSlcclxuICAgICAgICAgICAgICAgIC5tb2RhbChcInNob3dcIik7XHJcbiAgICAgICAgfSk7XHJcbiAgICB9XHJcbn1cclxuXHJcbmV4cG9ydCBkZWZhdWx0IE1vZGFsOyJdfQ==