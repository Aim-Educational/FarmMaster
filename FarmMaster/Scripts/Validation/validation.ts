class ValidationRuleInfo {
    name: string;
    params: string[];

    constructor() {
        this.name = "";
        this.params = [];
    }

    static fromString(ruleString: string): ValidationRuleInfo
    {
        let rule                = new ValidationRuleInfo();
        let readingName         = true;
        let leftBracketCount    = 0;
        let paramIndex          = 0;
        for (let char of ruleString) {
            if (readingName) {
                if (char == '[') {
                    leftBracketCount = 1;
                    readingName = false;
                    continue;
                }

                rule.name += char;
                continue;
            }

            if (leftBracketCount == 0) {
                if (char == '[') {
                    leftBracketCount++;
                    continue;
                }
                else
                    break;
            }

            while (rule.params.length <= paramIndex)
                rule.params.push("");

            if (char == '[') {
                leftBracketCount++;
                rule.params[paramIndex] += char;
                continue;
            }

            if (char == ']') {
                leftBracketCount--;

                if (leftBracketCount == 0)
                    paramIndex++;
                else
                    rule.params[paramIndex] += char;

                continue;
            }

            rule.params[paramIndex] += char;
        }

        if (leftBracketCount > 0)
            throw "Square bracket mis-match, too many '[' or not enough ']'.";

        return rule;
    }
}

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
                if (fieldInput === null || (fieldInput.dataset.validateDirectParent !== undefined && fieldInput.parentElement !== fieldSection))
                    return;

                let fieldError = fieldSection.querySelector<HTMLDivElement>(".ui.error.message, .ui.red.prompt");
                let fieldName = fieldInput.name;
                let rules = fieldInput.dataset.validationRules.split("¬");

                let addError = (error: string) =>
                {
                    allErrors.push(error);
                    fieldError.classList.add("visible");
                    fieldError.classList.remove("hidden");
                    fieldError.innerText = error;

                    fieldSection.classList.add("error");
                };

                for (let ruleString of rules) {
                    let rule = ValidationRuleInfo.fromString(ruleString);

                    switch (rule.name) {
                        case "empty":
                            if (fieldInput.value.length == 0)
                                addError("The " + fieldName + " field is required.");
                            break;

                        case "checked":
                            if (!fieldInput.checked)
                                addError("The " + fieldName + " field must be checked.");
                            break;

                        case "regex":
                            if (rule.params.length != 1)
                                throw "Expected 1 parameter for rule 'regex', but got " + rule.params.length + " instead.";

                            let regex = new RegExp(rule.params[0]);
                            if (fieldInput.value.length > 0 && !regex.test(fieldInput.value))
                                addError("The " + fieldName + " field does not match the regex: " + rule.params[0]);
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