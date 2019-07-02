class Validation {
    static hookupForm(form: string | HTMLFormElement) {
        let actualForm: HTMLFormElement = null;

        if (typeof form === "string")
            actualForm = <HTMLFormElement>document.getElementById(form);
        else
            actualForm = form;

        actualForm.addEventListener("submit", function (e) {
            if (!Validation.validateForm(actualForm))
                e.preventDefault();
        }, false);
    }

    static validateForm(form: HTMLFormElement): boolean {
        let allErrors: string[] = [];

        form.querySelectorAll(".field")
            .forEach(fieldSection =>
            {
                let fieldInput = fieldSection.querySelector<HTMLInputElement>("input.needs.validation[data-validation-rules]");
                if (fieldInput === null || fieldInput.parentElement !== fieldSection)
                    return;

                let fieldError = fieldSection.querySelector<HTMLDivElement>(".ui.error.message, .ui.red.prompt");
                let fieldName = fieldInput.name;
                let rules = fieldInput.dataset.validationRules.split("~");

                let addError = (error: string) =>
                {
                    allErrors.push(error);
                    fieldError.classList.add("visible");
                    fieldError.classList.remove("hidden");
                    fieldError.innerText = error;

                    fieldSection.classList.add("error");
                };

                for (let rule of rules) {
                    switch (rule) {
                        case "empty":
                            if (fieldInput.value.length == 0)
                                addError("The " + fieldName + " field is required.");
                            break;

                        default: break;
                    }
                }
            }
        );

        let divAllErrors = form.querySelector("#divAllErrors");
        divAllErrors.innerHTML = "";
        if (divAllErrors !== undefined && divAllErrors !== null) {
            divAllErrors.classList.add("visible");

            let list = document.createElement("ul");
            divAllErrors.appendChild(list);

            for (let error of allErrors) {
                let item = document.createElement("li");
                item.innerText = error;
                list.appendChild(item);
            }
        }

        return allErrors.length == 0;
    }
}