var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    }
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
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
var ValidationFailed;
(function (ValidationFailed) {
    ValidationFailed[ValidationFailed["No"] = 0] = "No";
    ValidationFailed[ValidationFailed["Yes"] = 1] = "Yes";
})(ValidationFailed || (ValidationFailed = {}));
var IgnoreEmptyFields;
(function (IgnoreEmptyFields) {
    IgnoreEmptyFields[IgnoreEmptyFields["No"] = 0] = "No";
    IgnoreEmptyFields[IgnoreEmptyFields["Yes"] = 1] = "Yes";
})(IgnoreEmptyFields || (IgnoreEmptyFields = {}));
var ValidationRuleResult = (function () {
    function ValidationRuleResult(failed, reason) {
        this.failed = failed;
        this.reason = reason;
    }
    return ValidationRuleResult;
}());
var ValidationRule = (function () {
    function ValidationRule(name, expectedParamCount, ignoreEmpty) {
        this.expectedParamCount = 0;
        this.name = name;
        this.expectedParamCount = expectedParamCount;
        this.ignoreEmpty = ignoreEmpty;
    }
    return ValidationRule;
}());
var ValidationRuleEmpty = (function (_super) {
    __extends(ValidationRuleEmpty, _super);
    function ValidationRuleEmpty() {
        return _super.call(this, "empty", 0, IgnoreEmptyFields.No) || this;
    }
    ValidationRuleEmpty.prototype.doValidate = function (target, params) {
        return (target.value.length > 0
            ? null
            : new ValidationRuleResult(ValidationFailed.Yes, "is empty."));
    };
    return ValidationRuleEmpty;
}(ValidationRule));
var ValidationRuleChecked = (function (_super) {
    __extends(ValidationRuleChecked, _super);
    function ValidationRuleChecked() {
        return _super.call(this, "checked", 0, IgnoreEmptyFields.No) || this;
    }
    ValidationRuleChecked.prototype.doValidate = function (target, params) {
        return (target.checked
            ? null
            : new ValidationRuleResult(ValidationFailed.Yes, "is not checked."));
    };
    return ValidationRuleChecked;
}(ValidationRule));
var ValidationRuleRegex = (function (_super) {
    __extends(ValidationRuleRegex, _super);
    function ValidationRuleRegex() {
        return _super.call(this, "regex", 1, IgnoreEmptyFields.Yes) || this;
    }
    ValidationRuleRegex.prototype.doValidate = function (target, params) {
        return (new RegExp(params[0]).test(target.value)
            ? null
            : new ValidationRuleResult(ValidationFailed.Yes, "does not match pattern: " + params[0]));
    };
    return ValidationRuleRegex;
}(ValidationRule));
var Validation = (function () {
    function Validation() {
    }
    Validation.addRule = function (rule) {
        this.rules.push(rule);
    };
    Validation.hookupForm = function (form) {
        var actualForm = null;
        if (typeof form === "string")
            actualForm = document.getElementById(form);
        else
            actualForm = form;
        actualForm.addEventListener("submit", function (e) {
            try {
                if (!Validation.validateForm(actualForm))
                    e.preventDefault();
            }
            catch (ex) {
                e.preventDefault();
                throw ex;
            }
        }, false);
    };
    Validation.validateForm = function (form) {
        var _this = this;
        var allErrors = [];
        var handledInputs = [];
        form.querySelectorAll(".field")
            .forEach(function (fieldSection) {
            var fieldInput = fieldSection.querySelector("input.needs.validation[data-validation-rules]");
            if (fieldInput === null)
                return;
            if (handledInputs.filter(function (i) { return i == fieldInput; }).length == 1)
                return;
            handledInputs.push(fieldInput);
            fieldSection = fieldInput.parentElement;
            while (!fieldSection.classList.contains("field"))
                fieldSection = fieldSection.parentElement;
            var fieldError = fieldSection.querySelector(".ui.error.message, .ui.red.prompt");
            var fieldName = fieldInput.name;
            var errorPrefix = "The " + fieldName.split('.').slice(-1)[0] + " field ";
            var rules = fieldInput.dataset.validationRules.split("¬");
            if (fieldError !== null) {
                fieldError.classList.remove("visible");
            }
            var _loop_1 = function (ruleString) {
                var ruleInfo = ValidationRuleInfo.fromString(ruleString);
                var ruleFilter = _this.rules.filter(function (v) { return v.name === ruleInfo.name; });
                if (ruleFilter.length == 0)
                    throw "There is no ValidationRule registered for rule: " + ruleInfo.name;
                if (ruleFilter.length > 1)
                    throw "There are more than one ValidationRules registered for rule: " + ruleInfo.name;
                var rule = ruleFilter[0];
                if (rule.expectedParamCount > -1 && ruleInfo.params.length != rule.expectedParamCount)
                    throw "Expected " + rule.expectedParamCount + " parameters for rule " + ruleInfo.name + " but got " + ruleInfo.params.length + " parameters instead.";
                if (fieldInput.value.length == 0 && rule.ignoreEmpty == IgnoreEmptyFields.Yes)
                    return "continue";
                var result = rule.doValidate(fieldInput, ruleInfo.params);
                if (result !== null && result.failed == ValidationFailed.Yes) {
                    allErrors.push(errorPrefix + result.reason);
                    fieldError.classList.add("visible");
                    fieldError.innerHTML = allErrors.slice(-1)[0];
                }
            };
            for (var _i = 0, rules_1 = rules; _i < rules_1.length; _i++) {
                var ruleString = rules_1[_i];
                _loop_1(ruleString);
            }
        });
        var divAllErrors = form.querySelector("#divAllErrors");
        if (divAllErrors !== undefined && divAllErrors !== null) {
            divAllErrors.innerHTML = "";
            var list = document.createElement("ul");
            divAllErrors.appendChild(list);
            for (var _i = 0, allErrors_1 = allErrors; _i < allErrors_1.length; _i++) {
                var error = allErrors_1[_i];
                var item = document.createElement("li");
                item.innerText = error;
                list.appendChild(item);
            }
            if (allErrors.length > 0)
                divAllErrors.classList.add("visible");
            else
                divAllErrors.classList.remove("visible");
        }
        return allErrors.length == 0;
    };
    Validation.rules = [
        new ValidationRuleEmpty(),
        new ValidationRuleChecked(),
        new ValidationRuleRegex()
    ];
    return Validation;
}());
//# sourceMappingURL=validation.js.map