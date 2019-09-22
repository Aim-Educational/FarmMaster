export class Modal {
    public static askForReason(then: (reason: string) => void): void {
        const modal = document.getElementById("modalReason");
        if (modal === null) {
            alert("Dev error: No element with ID of 'modalReason'");
            return;
        }

        const inputReason = modal.querySelector("input.reason") as HTMLInputElement;
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