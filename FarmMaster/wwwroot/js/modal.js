export class Modal {
    static askForReason(then) {
        const modal = document.getElementById("modalReason");
        if (modal === null) {
            alert("Dev error: No element with ID of 'modalReason'");
            return;
        }
        const inputReason = modal.querySelector("input.reason");
        if (inputReason === null) {
            alert("Dev error: Modal does not contain an <input> with class of 'reason'");
            return;
        }
        $(modal)
            .modal({
            onApprove: function () {
                then(inputReason.value);
                inputReason.value = "";
            }
        })
            .modal("show");
    }
}
export default Modal;
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoibW9kYWwuanMiLCJzb3VyY2VSb290IjoiIiwic291cmNlcyI6WyIuLi8uLi9TY3JpcHRzL21vZGFsLnRzIl0sIm5hbWVzIjpbXSwibWFwcGluZ3MiOiJBQUFBLE1BQU0sT0FBTyxLQUFLO0lBQ1AsTUFBTSxDQUFDLFlBQVksQ0FBQyxJQUE4QjtRQUNyRCxNQUFNLEtBQUssR0FBRyxRQUFRLENBQUMsY0FBYyxDQUFDLGFBQWEsQ0FBQyxDQUFDO1FBQ3JELElBQUksS0FBSyxLQUFLLElBQUksRUFBRTtZQUNoQixLQUFLLENBQUMsZ0RBQWdELENBQUMsQ0FBQztZQUN4RCxPQUFPO1NBQ1Y7UUFFRCxNQUFNLFdBQVcsR0FBRyxLQUFLLENBQUMsYUFBYSxDQUFDLGNBQWMsQ0FBcUIsQ0FBQztRQUM1RSxJQUFJLFdBQVcsS0FBSyxJQUFJLEVBQUU7WUFDdEIsS0FBSyxDQUFDLHFFQUFxRSxDQUFDLENBQUM7WUFDN0UsT0FBTztTQUNWO1FBRUQsQ0FBQyxDQUFDLEtBQUssQ0FBQzthQUNILEtBQUssQ0FBQztZQUNILFNBQVMsRUFBRTtnQkFDUCxJQUFJLENBQUMsV0FBVyxDQUFDLEtBQUssQ0FBQyxDQUFDO2dCQUN4QixXQUFXLENBQUMsS0FBSyxHQUFHLEVBQUUsQ0FBQztZQUMzQixDQUFDO1NBQ0osQ0FBQzthQUNELEtBQUssQ0FBQyxNQUFNLENBQUMsQ0FBQztJQUN2QixDQUFDO0NBQ0o7QUFFRCxlQUFlLEtBQUssQ0FBQyIsInNvdXJjZXNDb250ZW50IjpbImV4cG9ydCBjbGFzcyBNb2RhbCB7XHJcbiAgICBwdWJsaWMgc3RhdGljIGFza0ZvclJlYXNvbih0aGVuOiAocmVhc29uOiBzdHJpbmcpID0+IHZvaWQpOiB2b2lkIHtcclxuICAgICAgICBjb25zdCBtb2RhbCA9IGRvY3VtZW50LmdldEVsZW1lbnRCeUlkKFwibW9kYWxSZWFzb25cIik7XHJcbiAgICAgICAgaWYgKG1vZGFsID09PSBudWxsKSB7XHJcbiAgICAgICAgICAgIGFsZXJ0KFwiRGV2IGVycm9yOiBObyBlbGVtZW50IHdpdGggSUQgb2YgJ21vZGFsUmVhc29uJ1wiKTtcclxuICAgICAgICAgICAgcmV0dXJuO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgY29uc3QgaW5wdXRSZWFzb24gPSBtb2RhbC5xdWVyeVNlbGVjdG9yKFwiaW5wdXQucmVhc29uXCIpIGFzIEhUTUxJbnB1dEVsZW1lbnQ7XHJcbiAgICAgICAgaWYgKGlucHV0UmVhc29uID09PSBudWxsKSB7XHJcbiAgICAgICAgICAgIGFsZXJ0KFwiRGV2IGVycm9yOiBNb2RhbCBkb2VzIG5vdCBjb250YWluIGFuIDxpbnB1dD4gd2l0aCBjbGFzcyBvZiAncmVhc29uJ1wiKTtcclxuICAgICAgICAgICAgcmV0dXJuO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgJChtb2RhbClcclxuICAgICAgICAgICAgLm1vZGFsKHtcclxuICAgICAgICAgICAgICAgIG9uQXBwcm92ZTogZnVuY3Rpb24gKCkge1xyXG4gICAgICAgICAgICAgICAgICAgIHRoZW4oaW5wdXRSZWFzb24udmFsdWUpO1xyXG4gICAgICAgICAgICAgICAgICAgIGlucHV0UmVhc29uLnZhbHVlID0gXCJcIjtcclxuICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgfSlcclxuICAgICAgICAgICAgLm1vZGFsKFwic2hvd1wiKTsgICAgICAgXHJcbiAgICB9XHJcbn1cclxuXHJcbmV4cG9ydCBkZWZhdWx0IE1vZGFsOyJdfQ==