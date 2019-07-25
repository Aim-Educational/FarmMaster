var ComponentModal = (function () {
    function ComponentModal() {
    }
    ComponentModal.showContactActionReasonModal = function (onAccept) {
        var elemModal = $("#modalContactActionReason");
        var inputReason = document.getElementById("modalContactActionReason").querySelector("input");
        ComponentModal.showModal(elemModal, function () { return onAccept(inputReason.value); });
    };
    ComponentModal.showAreYouSureModal = function (onAccept) {
        var elemModal = $("#modalAreYouSure");
        ComponentModal.showModal(elemModal, onAccept);
    };
    ComponentModal.showModal = function (elemModal, onAccept) {
        elemModal
            .modal({
            onApprove: function () {
                elemModal.modal("hide");
                onAccept();
            }
        })
            .modal('show');
    };
    return ComponentModal;
}());
//# sourceMappingURL=component_modal.js.map