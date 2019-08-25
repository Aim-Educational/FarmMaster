export class ComponentModal {
    // Functions like this are special helper functions that coincide with certain partials.
    // Partial: _ComponentModalContactActionReason
    public static showContactActionReasonModal(onAccept: (reason: string) => void) {
        let elemModal = $("#modalContactActionReason"); // GlobalConstants.IdModalContactActionReason
        let inputReason = document.getElementById("modalContactActionReason")!.querySelector("input")!;
        ComponentModal.showModal(elemModal, () => onAccept(inputReason.value));
    }

    public static showAreYouSureModal(onAccept: () => void) {
        let elemModal = $("#modalAreYouSure"); // GlobalConstants.IdModalAreYouSure
        ComponentModal.showModal(elemModal, onAccept);
    }

    public static showModal(elemModal: JQuery<HTMLElement>, onAccept: () => void) {
        elemModal
            .modal({
                onApprove: function () {
                    elemModal.modal("hide");
                    onAccept();
                }
            })
            .modal('show');
    }
}