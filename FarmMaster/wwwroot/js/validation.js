var Validation = (function () {
    function Validation() {
    }
    Validation.hookupForm = function (form) {
        var actualForm = null;
        if (typeof form === "string")
            actualForm = document.getElementById(form);
        else
            actualForm = form;
        actualForm.addEventListener("submit", function (e) {
            if (!Validation.validateForm(actualForm))
                e.preventDefault();
        }, false);
    };
    Validation.validateForm = function (form) {
        var allErrors = [];
        form.querySelectorAll(".field")
            .forEach(function (fieldSection) {
            var fieldInput = fieldSection.querySelector("input.needs.validation[data-validation-rules]");
            if (fieldInput === null || fieldInput.parentElement !== fieldSection)
                return;
            var fieldError = fieldSection.querySelector(".ui.error.message, .ui.red.prompt");
            var fieldName = fieldInput.name;
            var rules = fieldInput.dataset.validationRules.split("~");
            var addError = function (error) {
                allErrors.push(error);
                fieldError.classList.add("visible");
                fieldError.classList.remove("hidden");
                fieldError.innerText = error;
                fieldSection.classList.add("error");
            };
            for (var _i = 0, rules_1 = rules; _i < rules_1.length; _i++) {
                var rule = rules_1[_i];
                switch (rule) {
                    case "empty":
                        if (fieldInput.value.length == 0)
                            addError("The " + fieldName + " field is required.");
                        break;
                    default: break;
                }
            }
        });
        var divAllErrors = form.querySelector("#divAllErrors");
        divAllErrors.innerHTML = "";
        if (divAllErrors !== undefined && divAllErrors !== null) {
            divAllErrors.classList.add("visible");
            var list = document.createElement("ul");
            divAllErrors.appendChild(list);
            for (var _i = 0, allErrors_1 = allErrors; _i < allErrors_1.length; _i++) {
                var error = allErrors_1[_i];
                var item = document.createElement("li");
                item.innerText = error;
                list.appendChild(item);
            }
        }
        return allErrors.length == 0;
    };
    return Validation;
}());
//# sourceMappingURL=validation.js.map