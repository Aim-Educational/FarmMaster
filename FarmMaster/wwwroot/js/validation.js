var ValidationRuleInfo = (function () {
    function ValidationRuleInfo() {
        this.name = "";
        this.params = [];
    }
    ValidationRuleInfo.fromString = function (ruleString) {
        var rule = new ValidationRuleInfo();
        var readingName = true;
        var leftBracketCount = 0;
        var paramIndex = 0;
        for (var _i = 0, ruleString_1 = ruleString; _i < ruleString_1.length; _i++) {
            var char = ruleString_1[_i];
            if (readingName) {
                if (char == ';')
                    break;
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
    };
    return ValidationRuleInfo;
}());
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
            if (fieldInput === null || (fieldInput.dataset.validateDirectParent !== undefined && fieldInput.parentElement !== fieldSection))
                return;
            var fieldError = fieldSection.querySelector(".ui.error.message, .ui.red.prompt");
            var fieldName = fieldInput.name;
            var rules = fieldInput.dataset.validationRules.split("¬");
            var addError = function (error) {
                allErrors.push(error);
                fieldError.classList.add("visible");
                fieldError.classList.remove("hidden");
                fieldError.innerText = error;
                fieldSection.classList.add("error");
            };
            for (var _i = 0, rules_1 = rules; _i < rules_1.length; _i++) {
                var ruleString = rules_1[_i];
                var rule = ValidationRuleInfo.fromString(ruleString);
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
                        var regex = new RegExp(rule.params[0]);
                        if (fieldInput.value.length > 0 && !regex.test(fieldInput.value))
                            addError("The " + fieldName + " field does not match the regex: " + rule.params[0]);
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