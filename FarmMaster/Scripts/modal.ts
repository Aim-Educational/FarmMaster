export class Modal {
    public static askForReason(): Promise<string> | null {
        const modal = document.getElementById("modalReason");
        if (modal === null) {
            alert("Dev error: No element with ID of 'modalReason'");
            return null;
        }

        const inputReason = modal.querySelector("input.reason") as HTMLInputElement;
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