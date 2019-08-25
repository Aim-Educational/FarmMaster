"use strict";
/** A class that contains information about a rule. */
class ValidationRuleInfo {
    ///
    constructor() {
        this.name = "";
        this.params = [];
    }
    /**
     * Parses a rule string into an instance of ValidationRuleInfo.
     *
     * @param ruleString The rule string to parse.
     *
     * @throws If there is a mis match between the count of left square brackets ('[') and right square brackets.
     *
     * Format:
     *  The format of a rule string is: NAME[PARAM1][PARAM2]...[PARAM_N]
     * */
    static fromString(ruleString) {
        let rule = new ValidationRuleInfo();
        let readingName = true;
        let leftBracketCount = 0;
        let paramIndex = 0;
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
class ValidationRuleResult {
    constructor(failed, reason) {
        this.failed = failed;
        this.reason = reason;
    }
}
class ValidationRule {
    constructor(name, expectedParamCount, ignoreEmpty) {
        this.expectedParamCount = 0;
        this.name = name;
        this.expectedParamCount = expectedParamCount;
        this.ignoreEmpty = ignoreEmpty;
    }
}
class ValidationRuleEmpty extends ValidationRule {
    constructor() {
        super("empty", 0, IgnoreEmptyFields.No);
    }
    doValidate(target, params) {
        return (target.value.length > 0
            ? new ValidationRuleResult(ValidationFailed.No, "")
            : new ValidationRuleResult(ValidationFailed.Yes, "is empty."));
    }
}
class ValidationRuleChecked extends ValidationRule {
    constructor() {
        super("checked", 0, IgnoreEmptyFields.No);
    }
    doValidate(target, params) {
        return (target.checked
            ? new ValidationRuleResult(ValidationFailed.No, "")
            : new ValidationRuleResult(ValidationFailed.Yes, "is not checked."));
    }
}
class ValidationRuleRegex extends ValidationRule {
    constructor() {
        super("regex", 1, IgnoreEmptyFields.Yes);
    }
    doValidate(target, params) {
        return (new RegExp(params[0]).test(target.value)
            ? new ValidationRuleResult(ValidationFailed.No, "")
            : new ValidationRuleResult(ValidationFailed.Yes, "does not match pattern: " + params[0]));
    }
}
/**
 * A class that provides automatic validation capabilities to forms.
 *
 * Validation is done via 'data-' attributes.
 *
 * Attributes:
 *  'data-validation-rules' is a list of rules delmitated by the '¬' character, and contains all of the validation rules to apply
 *  against the field.
 * */
class Validation {
    static addRule(rule) {
        this.rules.push(rule);
    }
    static hookupForm(form) {
        // Get the form.
        let actualForm = null;
        if (typeof form === "string")
            actualForm = document.getElementById(form);
        else
            actualForm = form;
        // If we don't validate or an exception is thrown, stop the form from submitting.
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
    }
    static validateForm(form) {
        let allErrors = [];
        let handledInputs = [];
        form.querySelectorAll(".field")
            .forEach(fieldSection => {
            // Only get inputs with validation rules.
            let fieldInput = fieldSection.querySelector("input.needs.validation[data-validation-rules]");
            if (fieldInput === null)
                return;
            // Don't handle the same input multiple times (which is possible for fields nested inside other fields).
            if (handledInputs.filter(i => i == fieldInput).length == 1)
                return;
            handledInputs.push(fieldInput);
            // querySelector will look deeper than a single node level, so we need to work our way back up the the deepest 'field' parent.
            fieldSection = fieldInput.parentElement;
            while (!fieldSection.classList.contains("field"))
                fieldSection = fieldSection.parentElement;
            // Get the error message box (if it exists), and several other pieces of data.
            let fieldError = fieldSection.querySelector(".ui.error.message, .ui.red.prompt");
            let fieldName = fieldInput.name;
            let errorPrefix = "The " + fieldName.split('.').slice(-1)[0] + " field ";
            let rules = fieldInput.dataset.validationRules.split("¬");
            // Hide the error box in case this isn't the first time validation was run (so old errors don't stick around).
            if (fieldError !== null) {
                fieldError.classList.remove("visible");
            }
            // Apply all rules that the element specifies.
            for (let ruleString of rules) {
                let ruleInfo = ValidationRuleInfo.fromString(ruleString);
                let ruleFilter = this.rules.filter(v => v.name === ruleInfo.name);
                if (ruleFilter.length == 0)
                    throw "There is no ValidationRule registered for rule: " + ruleInfo.name;
                if (ruleFilter.length > 1)
                    throw "There are more than one ValidationRules registered for rule: " + ruleInfo.name;
                let rule = ruleFilter[0];
                if (rule.expectedParamCount > -1 && ruleInfo.params.length != rule.expectedParamCount)
                    throw "Expected " + rule.expectedParamCount + " parameters for rule " + ruleInfo.name + " but got " + ruleInfo.params.length + " parameters instead.";
                if (fieldInput.value.length == 0 && rule.ignoreEmpty == IgnoreEmptyFields.Yes)
                    continue;
                let result = rule.doValidate(fieldInput, ruleInfo.params);
                if (result !== null && result.failed == ValidationFailed.Yes) {
                    allErrors.push(errorPrefix + result.reason);
                    fieldError.classList.add("visible");
                    fieldError.innerHTML = allErrors.slice(-1)[0];
                }
            }
        });
        // Populate the div containing all of the errors found.
        let divAllErrors = form.querySelector("#divAllErrors");
        if (divAllErrors !== undefined && divAllErrors !== null) {
            divAllErrors.innerHTML = "";
            let list = document.createElement("ul");
            divAllErrors.appendChild(list);
            for (let error of allErrors) {
                let item = document.createElement("li");
                item.innerText = error;
                list.appendChild(item);
            }
            if (allErrors.length > 0)
                divAllErrors.classList.add("visible");
            else
                divAllErrors.classList.remove("visible");
        }
        return allErrors.length == 0;
    }
}
Validation.rules = [
    new ValidationRuleEmpty(),
    new ValidationRuleChecked(),
    new ValidationRuleRegex()
];
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoidmFsaWRhdGlvbi5qcyIsInNvdXJjZVJvb3QiOiIiLCJzb3VyY2VzIjpbIi4uLy4uL1NjcmlwdHMvdmFsaWRhdGlvbi50cyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiO0FBQUEsc0RBQXNEO0FBQ3RELE1BQU0sa0JBQWtCO0lBT3BCLEdBQUc7SUFDSDtRQUNJLElBQUksQ0FBQyxJQUFJLEdBQUcsRUFBRSxDQUFDO1FBQ2YsSUFBSSxDQUFDLE1BQU0sR0FBRyxFQUFFLENBQUM7SUFDckIsQ0FBQztJQUVEOzs7Ozs7Ozs7U0FTSztJQUNMLE1BQU0sQ0FBQyxVQUFVLENBQUMsVUFBa0I7UUFFaEMsSUFBSSxJQUFJLEdBQWtCLElBQUksa0JBQWtCLEVBQUUsQ0FBQztRQUNuRCxJQUFJLFdBQVcsR0FBVyxJQUFJLENBQUM7UUFDL0IsSUFBSSxnQkFBZ0IsR0FBTSxDQUFDLENBQUM7UUFDNUIsSUFBSSxVQUFVLEdBQVksQ0FBQyxDQUFDO1FBQzVCLEtBQUssSUFBSSxJQUFJLElBQUksVUFBVSxFQUFFO1lBQ3pCLElBQUksV0FBVyxFQUFFO2dCQUNiLElBQUksSUFBSSxJQUFJLEdBQUcsRUFBRTtvQkFDYixnQkFBZ0IsR0FBRyxDQUFDLENBQUM7b0JBQ3JCLFdBQVcsR0FBRyxLQUFLLENBQUM7b0JBQ3BCLFNBQVM7aUJBQ1o7Z0JBRUQsSUFBSSxDQUFDLElBQUksSUFBSSxJQUFJLENBQUM7Z0JBQ2xCLFNBQVM7YUFDWjtZQUVELElBQUksZ0JBQWdCLElBQUksQ0FBQyxFQUFFO2dCQUN2QixJQUFJLElBQUksSUFBSSxHQUFHLEVBQUU7b0JBQ2IsZ0JBQWdCLEVBQUUsQ0FBQztvQkFDbkIsU0FBUztpQkFDWjs7b0JBRUcsTUFBTTthQUNiO1lBRUQsT0FBTyxJQUFJLENBQUMsTUFBTSxDQUFDLE1BQU0sSUFBSSxVQUFVO2dCQUNuQyxJQUFJLENBQUMsTUFBTSxDQUFDLElBQUksQ0FBQyxFQUFFLENBQUMsQ0FBQztZQUV6QixJQUFJLElBQUksSUFBSSxHQUFHLEVBQUU7Z0JB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SUFBSSxtQkFBbUIsRUFBRTtJQUN6QixJQUFJLHFCQUFxQixFQUFFO0lBQzNCLElBQUksbUJBQW1CLEVBQUU7Q0FDNUIsQ0FBQyIsInNvdXJjZXNDb250ZW50IjpbIi8qKiBBIGNsYXNzIHRoYXQgY29udGFpbnMgaW5mb3JtYXRpb24gYWJvdXQgYSBydWxlLiAqL1xyXG5jbGFzcyBWYWxpZGF0aW9uUnVsZUluZm8ge1xyXG4gICAgLyoqIFRoZSBuYW1lIG9mIHRoZSBydWxlLiAqL1xyXG4gICAgbmFtZTogc3RyaW5nO1xyXG5cclxuICAgIC8qKiBUaGUgcGFyYW1ldGVycyBnaXZlbiB0byB0aGUgcnVsZS4gKi9cclxuICAgIHBhcmFtczogc3RyaW5nW107XHJcblxyXG4gICAgLy8vXHJcbiAgICBjb25zdHJ1Y3RvcigpIHtcclxuICAgICAgICB0aGlzLm5hbWUgPSBcIlwiO1xyXG4gICAgICAgIHRoaXMucGFyYW1zID0gW107XHJcbiAgICB9XHJcblxyXG4gICAgLyoqXHJcbiAgICAgKiBQYXJzZXMgYSBydWxlIHN0cmluZyBpbnRvIGFuIGluc3RhbmNlIG9mIFZhbGlkYXRpb25SdWxlSW5mby5cclxuICAgICAqIFxyXG4gICAgICogQHBhcmFtIHJ1bGVTdHJpbmcgVGhlIHJ1bGUgc3RyaW5nIHRvIHBhcnNlLlxyXG4gICAgICogXHJcbiAgICAgKiBAdGhyb3dzIElmIHRoZXJlIGlzIGEgbWlzIG1hdGNoIGJldHdlZW4gdGhlIGNvdW50IG9mIGxlZnQgc3F1YXJlIGJyYWNrZXRzICgnWycpIGFuZCByaWdodCBzcXVhcmUgYnJhY2tldHMuXHJcbiAgICAgKiBcclxuICAgICAqIEZvcm1hdDpcclxuICAgICAqICBUaGUgZm9ybWF0IG9mIGEgcnVsZSBzdHJpbmcgaXM6IE5BTUVbUEFSQU0xXVtQQVJBTTJdLi4uW1BBUkFNX05dXHJcbiAgICAgKiAqL1xyXG4gICAgc3RhdGljIGZyb21TdHJpbmcocnVsZVN0cmluZzogc3RyaW5nKTogVmFsaWRhdGlvblJ1bGVJbmZvXHJcbiAgICB7XHJcbiAgICAgICAgbGV0IHJ1bGUgICAgICAgICAgICAgICAgPSBuZXcgVmFsaWRhdGlvblJ1bGVJbmZvKCk7XHJcbiAgICAgICAgbGV0IHJlYWRpbmdOYW1lICAgICAgICAgPSB0cnVlO1xyXG4gICAgICAgIGxldCBsZWZ0QnJhY2tldENvdW50ICAgID0gMDtcclxuICAgICAgICBsZXQgcGFyYW1JbmRleCAgICAgICAgICA9IDA7XHJcbiAgICAgICAgZm9yIChsZXQgY2hhciBvZiBydWxlU3RyaW5nKSB7XHJcbiAgICAgICAgICAgIGlmIChyZWFkaW5nTmFtZSkge1xyXG4gICAgICAgICAgICAgICAgaWYgKGNoYXIgPT0gJ1snKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgbGVmdEJyYWNrZXRDb3VudCA9IDE7XHJcbiAgICAgICAgICAgICAgICAgICAgcmVhZGluZ05hbWUgPSBmYWxzZTtcclxuICAgICAgICAgICAgICAgICAgICBjb250aW51ZTtcclxuICAgICAgICAgICAgICAgIH1cclxuXHJcbiAgICAgICAgICAgICAgICBydWxlLm5hbWUgKz0gY2hhcjtcclxuICAgICAgICAgICAgICAgIGNvbnRpbnVlO1xyXG4gICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICBpZiAobGVmdEJyYWNrZXRDb3VudCA9PSAwKSB7XHJcbiAgICAgICAgICAgICAgICBpZiAoY2hhciA9PSAnWycpIHtcclxuICAgICAgICAgICAgICAgICAgICBsZWZ0QnJhY2tldENvdW50Kys7XHJcbiAgICAgICAgICAgICAgICAgICAgY29udGludWU7XHJcbiAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICBlbHNlXHJcbiAgICAgICAgICAgICAgICAgICAgYnJlYWs7XHJcbiAgICAgICAgICAgIH1cclxuXHJcbiAgICAgICAgICAgIHdoaWxlIChydWxlLnBhcmFtcy5sZW5ndGggPD0gcGFyYW1JbmRleClcclxuICAgICAgICAgICAgICAgIHJ1bGUucGFyYW1zLnB1c2goXCJcIik7XHJcblxyXG4gICAgICAgICAgICBpZiAoY2hhciA9PSAnWycpIHtcclxuICAgICAgICAgICAgICAgIGxlZnRCcmFja2V0Q291bnQrKztcclxuICAgICAgICAgICAgICAgIHJ1bGUucGFyYW1zW3BhcmFtSW5kZXhdICs9IGNoYXI7XHJcbiAgICAgICAgICAgICAgICBjb250aW51ZTtcclxuICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgaWYgKGNoYXIgPT0gJ10nKSB7XHJcbiAgICAgICAgICAgICAgICBsZWZ0QnJhY2tldENvdW50LS07XHJcblxyXG4gICAgICAgICAgICAgICAgaWYgKGxlZnRCcmFja2V0Q291bnQgPT0gMClcclxuICAgICAgICAgICAgICAgICAgICBwYXJhbUluZGV4Kys7XHJcbiAgICAgICAgICAgICAgICBlbHNlXHJcbiAgICAgICAgICAgICAgICAgICAgcnVsZS5wYXJhbXNbcGFyYW1JbmRleF0gKz0gY2hhcjtcclxuXHJcbiAgICAgICAgICAgICAgICBjb250aW51ZTtcclxuICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgcnVsZS5wYXJhbXNbcGFyYW1JbmRleF0gKz0gY2hhcjtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIGlmIChsZWZ0QnJhY2tldENvdW50ID4gMClcclxuICAgICAgICAgICAgdGhyb3cgXCJTcXVhcmUgYnJhY2tldCBtaXMtbWF0Y2gsIHRvbyBtYW55ICdbJyBvciBub3QgZW5vdWdoICddJy5cIjtcclxuXHJcbiAgICAgICAgcmV0dXJuIHJ1bGU7XHJcbiAgICB9XHJcbn1cclxuXHJcbmVudW0gVmFsaWRhdGlvbkZhaWxlZCB7XHJcbiAgICBObyxcclxuICAgIFllc1xyXG59XHJcblxyXG5lbnVtIElnbm9yZUVtcHR5RmllbGRzIHtcclxuICAgIE5vLFxyXG4gICAgWWVzXHJcbn1cclxuXHJcbmNsYXNzIFZhbGlkYXRpb25SdWxlUmVzdWx0IHtcclxuICAgIGZhaWxlZDogVmFsaWRhdGlvbkZhaWxlZDtcclxuICAgIHJlYXNvbjogc3RyaW5nO1xyXG5cclxuICAgIGNvbnN0cnVjdG9yKGZhaWxlZDogVmFsaWRhdGlvbkZhaWxlZCwgcmVhc29uOiBzdHJpbmcpIHtcclxuICAgICAgICB0aGlzLmZhaWxlZCA9IGZhaWxlZDtcclxuICAgICAgICB0aGlzLnJlYXNvbiA9IHJlYXNvbjtcclxuICAgIH1cclxufVxyXG5cclxuYWJzdHJhY3QgY2xhc3MgVmFsaWRhdGlvblJ1bGUge1xyXG4gICAgcHVibGljIHJlYWRvbmx5IGV4cGVjdGVkUGFyYW1Db3VudDogbnVtYmVyID0gMDtcclxuICAgIHB1YmxpYyByZWFkb25seSBuYW1lOiBzdHJpbmc7XHJcbiAgICBwdWJsaWMgcmVhZG9ubHkgaWdub3JlRW1wdHk6IElnbm9yZUVtcHR5RmllbGRzO1xyXG5cclxuICAgIGNvbnN0cnVjdG9yKG5hbWU6IHN0cmluZywgZXhwZWN0ZWRQYXJhbUNvdW50OiBudW1iZXIsIGlnbm9yZUVtcHR5OiBJZ25vcmVFbXB0eUZpZWxkcykge1xyXG4gICAgICAgIHRoaXMubmFtZSA9IG5hbWU7XHJcbiAgICAgICAgdGhpcy5leHBlY3RlZFBhcmFtQ291bnQgPSBleHBlY3RlZFBhcmFtQ291bnQ7XHJcbiAgICAgICAgdGhpcy5pZ25vcmVFbXB0eSA9IGlnbm9yZUVtcHR5O1xyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBhYnN0cmFjdCBkb1ZhbGlkYXRlKHRhcmdldDogSFRNTElucHV0RWxlbWVudCwgcGFyYW1zOiBzdHJpbmdbXSk6IFZhbGlkYXRpb25SdWxlUmVzdWx0O1xyXG59XHJcblxyXG5jbGFzcyBWYWxpZGF0aW9uUnVsZUVtcHR5IGV4dGVuZHMgVmFsaWRhdGlvblJ1bGUge1xyXG4gICAgY29uc3RydWN0b3IoKSB7XHJcbiAgICAgICAgc3VwZXIoXCJlbXB0eVwiLCAwLCBJZ25vcmVFbXB0eUZpZWxkcy5Obyk7XHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIGRvVmFsaWRhdGUodGFyZ2V0OiBIVE1MSW5wdXRFbGVtZW50LCBwYXJhbXM6IHN0cmluZ1tdKTogVmFsaWRhdGlvblJ1bGVSZXN1bHQge1xyXG4gICAgICAgIHJldHVybiAoXHJcbiAgICAgICAgICAgIHRhcmdldC52YWx1ZS5sZW5ndGggPiAwXHJcbiAgICAgICAgICAgICAgICA/IG5ldyBWYWxpZGF0aW9uUnVsZVJlc3VsdChWYWxpZGF0aW9uRmFpbGVkLk5vLCBcIlwiKVxyXG4gICAgICAgICAgICAgICAgOiBuZXcgVmFsaWRhdGlvblJ1bGVSZXN1bHQoVmFsaWRhdGlvbkZhaWxlZC5ZZXMsIFwiaXMgZW1wdHkuXCIpXHJcbiAgICAgICAgKTtcclxuICAgIH1cclxufVxyXG5cclxuY2xhc3MgVmFsaWRhdGlvblJ1bGVDaGVja2VkIGV4dGVuZHMgVmFsaWRhdGlvblJ1bGUge1xyXG4gICAgY29uc3RydWN0b3IoKSB7XHJcbiAgICAgICAgc3VwZXIoXCJjaGVja2VkXCIsIDAsIElnbm9yZUVtcHR5RmllbGRzLk5vKTtcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgZG9WYWxpZGF0ZSh0YXJnZXQ6IEhUTUxJbnB1dEVsZW1lbnQsIHBhcmFtczogc3RyaW5nW10pOiBWYWxpZGF0aW9uUnVsZVJlc3VsdCB7XHJcbiAgICAgICAgcmV0dXJuIChcclxuICAgICAgICAgICAgdGFyZ2V0LmNoZWNrZWRcclxuICAgICAgICAgICAgICAgID8gbmV3IFZhbGlkYXRpb25SdWxlUmVzdWx0KFZhbGlkYXRpb25GYWlsZWQuTm8sIFwiXCIpXHJcbiAgICAgICAgICAgICAgICA6IG5ldyBWYWxpZGF0aW9uUnVsZVJlc3VsdChWYWxpZGF0aW9uRmFpbGVkLlllcywgXCJpcyBub3QgY2hlY2tlZC5cIilcclxuICAgICAgICApO1xyXG4gICAgfVxyXG59XHJcblxyXG5jbGFzcyBWYWxpZGF0aW9uUnVsZVJlZ2V4IGV4dGVuZHMgVmFsaWRhdGlvblJ1bGUge1xyXG4gICAgY29uc3RydWN0b3IoKSB7XHJcbiAgICAgICAgc3VwZXIoXCJyZWdleFwiLCAxLCBJZ25vcmVFbXB0eUZpZWxkcy5ZZXMpO1xyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBkb1ZhbGlkYXRlKHRhcmdldDogSFRNTElucHV0RWxlbWVudCwgcGFyYW1zOiBzdHJpbmdbXSk6IFZhbGlkYXRpb25SdWxlUmVzdWx0IHtcclxuICAgICAgICByZXR1cm4gKFxyXG4gICAgICAgICAgICBuZXcgUmVnRXhwKHBhcmFtc1swXSkudGVzdCh0YXJnZXQudmFsdWUpXHJcbiAgICAgICAgICAgICAgICA/IG5ldyBWYWxpZGF0aW9uUnVsZVJlc3VsdChWYWxpZGF0aW9uRmFpbGVkLk5vLCBcIlwiKVxyXG4gICAgICAgICAgICAgICAgOiBuZXcgVmFsaWRhdGlvblJ1bGVSZXN1bHQoVmFsaWRhdGlvbkZhaWxlZC5ZZXMsIFwiZG9lcyBub3QgbWF0Y2ggcGF0dGVybjogXCIrcGFyYW1zWzBdKVxyXG4gICAgICAgICk7XHJcbiAgICB9XHJcbn1cclxuXHJcbi8qKiBcclxuICogQSBjbGFzcyB0aGF0IHByb3ZpZGVzIGF1dG9tYXRpYyB2YWxpZGF0aW9uIGNhcGFiaWxpdGllcyB0byBmb3Jtcy5cclxuICogXHJcbiAqIFZhbGlkYXRpb24gaXMgZG9uZSB2aWEgJ2RhdGEtJyBhdHRyaWJ1dGVzLlxyXG4gKiBcclxuICogQXR0cmlidXRlczpcclxuICogICdkYXRhLXZhbGlkYXRpb24tcnVsZXMnIGlzIGEgbGlzdCBvZiBydWxlcyBkZWxtaXRhdGVkIGJ5IHRoZSAnwqwnIGNoYXJhY3RlciwgYW5kIGNvbnRhaW5zIGFsbCBvZiB0aGUgdmFsaWRhdGlvbiBydWxlcyB0byBhcHBseVxyXG4gKiAgYWdhaW5zdCB0aGUgZmllbGQuXHJcbiAqICovXHJcbmNsYXNzIFZhbGlkYXRpb24ge1xyXG4gICAgcHJpdmF0ZSBzdGF0aWMgcnVsZXM6IFZhbGlkYXRpb25SdWxlW10gPSBbXHJcbiAgICAgICAgbmV3IFZhbGlkYXRpb25SdWxlRW1wdHkoKSxcclxuICAgICAgICBuZXcgVmFsaWRhdGlvblJ1bGVDaGVja2VkKCksXHJcbiAgICAgICAgbmV3IFZhbGlkYXRpb25SdWxlUmVnZXgoKVxyXG4gICAgXTtcclxuXHJcbiAgICBzdGF0aWMgYWRkUnVsZShydWxlOiBWYWxpZGF0aW9uUnVsZSk6IHZvaWQge1xyXG4gICAgICAgIHRoaXMucnVsZXMucHVzaChydWxlKTtcclxuICAgIH1cclxuXHJcbiAgICBzdGF0aWMgaG9va3VwRm9ybShmb3JtOiBzdHJpbmcgfCBIVE1MRm9ybUVsZW1lbnQpIHtcclxuICAgICAgICAvLyBHZXQgdGhlIGZvcm0uXHJcbiAgICAgICAgbGV0IGFjdHVhbEZvcm06IEhUTUxGb3JtRWxlbWVudCB8IG51bGwgPSBudWxsO1xyXG4gICAgICAgIGlmICh0eXBlb2YgZm9ybSA9PT0gXCJzdHJpbmdcIilcclxuICAgICAgICAgICAgYWN0dWFsRm9ybSA9IDxIVE1MRm9ybUVsZW1lbnQ+ZG9jdW1lbnQuZ2V0RWxlbWVudEJ5SWQoZm9ybSk7XHJcbiAgICAgICAgZWxzZVxyXG4gICAgICAgICAgICBhY3R1YWxGb3JtID0gZm9ybTtcclxuXHJcbiAgICAgICAgLy8gSWYgd2UgZG9uJ3QgdmFsaWRhdGUgb3IgYW4gZXhjZXB0aW9uIGlzIHRocm93biwgc3RvcCB0aGUgZm9ybSBmcm9tIHN1Ym1pdHRpbmcuXHJcbiAgICAgICAgYWN0dWFsRm9ybS5hZGRFdmVudExpc3RlbmVyKFwic3VibWl0XCIsIGZ1bmN0aW9uIChlKSB7XHJcbiAgICAgICAgICAgIHRyeSB7XHJcbiAgICAgICAgICAgICAgICBpZiAoIVZhbGlkYXRpb24udmFsaWRhdGVGb3JtKGFjdHVhbEZvcm0hKSlcclxuICAgICAgICAgICAgICAgICAgICBlLnByZXZlbnREZWZhdWx0KCk7XHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgY2F0Y2goZXgpIHtcclxuICAgICAgICAgICAgICAgIGUucHJldmVudERlZmF1bHQoKTtcclxuICAgICAgICAgICAgICAgIHRocm93IGV4O1xyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgfSwgZmFsc2UpO1xyXG4gICAgfVxyXG5cclxuICAgIHN0YXRpYyB2YWxpZGF0ZUZvcm0oZm9ybTogSFRNTEZvcm1FbGVtZW50KTogYm9vbGVhbiB7XHJcbiAgICAgICAgbGV0IGFsbEVycm9yczogc3RyaW5nW10gPSBbXTtcclxuICAgICAgICBsZXQgaGFuZGxlZElucHV0czogSFRNTElucHV0RWxlbWVudFtdID0gW107XHJcblxyXG4gICAgICAgIGZvcm0ucXVlcnlTZWxlY3RvckFsbChcIi5maWVsZFwiKVxyXG4gICAgICAgICAgICAuZm9yRWFjaChmaWVsZFNlY3Rpb24gPT5cclxuICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgICAgLy8gT25seSBnZXQgaW5wdXRzIHdpdGggdmFsaWRhdGlvbiBydWxlcy5cclxuICAgICAgICAgICAgICAgIGxldCBmaWVsZElucHV0ID0gZmllbGRTZWN0aW9uLnF1ZXJ5U2VsZWN0b3I8SFRNTElucHV0RWxlbWVudD4oXCJpbnB1dC5uZWVkcy52YWxpZGF0aW9uW2RhdGEtdmFsaWRhdGlvbi1ydWxlc11cIik7XHJcbiAgICAgICAgICAgICAgICBpZiAoZmllbGRJbnB1dCA9PT0gbnVsbClcclxuICAgICAgICAgICAgICAgICAgICByZXR1cm47XHJcblxyXG4gICAgICAgICAgICAgICAgLy8gRG9uJ3QgaGFuZGxlIHRoZSBzYW1lIGlucHV0IG11bHRpcGxlIHRpbWVzICh3aGljaCBpcyBwb3NzaWJsZSBmb3IgZmllbGRzIG5lc3RlZCBpbnNpZGUgb3RoZXIgZmllbGRzKS5cclxuICAgICAgICAgICAgICAgIGlmIChoYW5kbGVkSW5wdXRzLmZpbHRlcihpID0+IGkgPT0gZmllbGRJbnB1dCkubGVuZ3RoID09IDEpXHJcbiAgICAgICAgICAgICAgICAgICAgcmV0dXJuO1xyXG5cclxuICAgICAgICAgICAgICAgIGhhbmRsZWRJbnB1dHMucHVzaChmaWVsZElucHV0KTtcclxuXHJcbiAgICAgICAgICAgICAgICAvLyBxdWVyeVNlbGVjdG9yIHdpbGwgbG9vayBkZWVwZXIgdGhhbiBhIHNpbmdsZSBub2RlIGxldmVsLCBzbyB3ZSBuZWVkIHRvIHdvcmsgb3VyIHdheSBiYWNrIHVwIHRoZSB0aGUgZGVlcGVzdCAnZmllbGQnIHBhcmVudC5cclxuICAgICAgICAgICAgICAgIGZpZWxkU2VjdGlvbiA9IDxFbGVtZW50PmZpZWxkSW5wdXQucGFyZW50RWxlbWVudDtcclxuICAgICAgICAgICAgICAgIHdoaWxlICghZmllbGRTZWN0aW9uLmNsYXNzTGlzdC5jb250YWlucyhcImZpZWxkXCIpKVxyXG4gICAgICAgICAgICAgICAgICAgIGZpZWxkU2VjdGlvbiA9IGZpZWxkU2VjdGlvbi5wYXJlbnRFbGVtZW50ITtcclxuXHJcbiAgICAgICAgICAgICAgICAvLyBHZXQgdGhlIGVycm9yIG1lc3NhZ2UgYm94IChpZiBpdCBleGlzdHMpLCBhbmQgc2V2ZXJhbCBvdGhlciBwaWVjZXMgb2YgZGF0YS5cclxuICAgICAgICAgICAgICAgIGxldCBmaWVsZEVycm9yID0gZmllbGRTZWN0aW9uLnF1ZXJ5U2VsZWN0b3I8SFRNTERpdkVsZW1lbnQ+KFwiLnVpLmVycm9yLm1lc3NhZ2UsIC51aS5yZWQucHJvbXB0XCIpITtcclxuICAgICAgICAgICAgICAgIGxldCBmaWVsZE5hbWUgPSBmaWVsZElucHV0Lm5hbWU7XHJcbiAgICAgICAgICAgICAgICBsZXQgZXJyb3JQcmVmaXggPSBcIlRoZSBcIiArIGZpZWxkTmFtZS5zcGxpdCgnLicpLnNsaWNlKC0xKVswXSArIFwiIGZpZWxkIFwiO1xyXG4gICAgICAgICAgICAgICAgbGV0IHJ1bGVzID0gZmllbGRJbnB1dC5kYXRhc2V0LnZhbGlkYXRpb25SdWxlcyEuc3BsaXQoXCLCrFwiKTtcclxuXHJcbiAgICAgICAgICAgICAgICAvLyBIaWRlIHRoZSBlcnJvciBib3ggaW4gY2FzZSB0aGlzIGlzbid0IHRoZSBmaXJzdCB0aW1lIHZhbGlkYXRpb24gd2FzIHJ1biAoc28gb2xkIGVycm9ycyBkb24ndCBzdGljayBhcm91bmQpLlxyXG4gICAgICAgICAgICAgICAgaWYgKGZpZWxkRXJyb3IgIT09IG51bGwpIHtcclxuICAgICAgICAgICAgICAgICAgICBmaWVsZEVycm9yLmNsYXNzTGlzdC5yZW1vdmUoXCJ2aXNpYmxlXCIpO1xyXG4gICAgICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgICAgIC8vIEFwcGx5IGFsbCBydWxlcyB0aGF0IHRoZSBlbGVtZW50IHNwZWNpZmllcy5cclxuICAgICAgICAgICAgICAgIGZvciAobGV0IHJ1bGVTdHJpbmcgb2YgcnVsZXMpIHtcclxuICAgICAgICAgICAgICAgICAgICBsZXQgcnVsZUluZm8gPSBWYWxpZGF0aW9uUnVsZUluZm8uZnJvbVN0cmluZyhydWxlU3RyaW5nKTtcclxuICAgICAgICAgICAgICAgICAgICBsZXQgcnVsZUZpbHRlciA9IHRoaXMucnVsZXMuZmlsdGVyKHYgPT4gdi5uYW1lID09PSBydWxlSW5mby5uYW1lKTtcclxuICAgICAgICAgICAgICAgICAgICBpZiAocnVsZUZpbHRlci5sZW5ndGggPT0gMClcclxuICAgICAgICAgICAgICAgICAgICAgICAgdGhyb3cgXCJUaGVyZSBpcyBubyBWYWxpZGF0aW9uUnVsZSByZWdpc3RlcmVkIGZvciBydWxlOiBcIiArIHJ1bGVJbmZvLm5hbWU7XHJcbiAgICAgICAgICAgICAgICAgICAgaWYgKHJ1bGVGaWx0ZXIubGVuZ3RoID4gMSlcclxuICAgICAgICAgICAgICAgICAgICAgICAgdGhyb3cgXCJUaGVyZSBhcmUgbW9yZSB0aGFuIG9uZSBWYWxpZGF0aW9uUnVsZXMgcmVnaXN0ZXJlZCBmb3IgcnVsZTogXCIgKyBydWxlSW5mby5uYW1lO1xyXG5cclxuICAgICAgICAgICAgICAgICAgICBsZXQgcnVsZSA9IHJ1bGVGaWx0ZXJbMF07XHJcbiAgICAgICAgICAgICAgICAgICAgaWYgKHJ1bGUuZXhwZWN0ZWRQYXJhbUNvdW50ID4gLTEgJiYgcnVsZUluZm8ucGFyYW1zLmxlbmd0aCAhPSBydWxlLmV4cGVjdGVkUGFyYW1Db3VudClcclxuICAgICAgICAgICAgICAgICAgICAgICAgdGhyb3cgXCJFeHBlY3RlZCBcIiArIHJ1bGUuZXhwZWN0ZWRQYXJhbUNvdW50ICsgXCIgcGFyYW1ldGVycyBmb3IgcnVsZSBcIiArIHJ1bGVJbmZvLm5hbWUgKyBcIiBidXQgZ290IFwiICsgcnVsZUluZm8ucGFyYW1zLmxlbmd0aCArIFwiIHBhcmFtZXRlcnMgaW5zdGVhZC5cIjtcclxuXHJcbiAgICAgICAgICAgICAgICAgICAgaWYgKGZpZWxkSW5wdXQudmFsdWUubGVuZ3RoID09IDAgJiYgcnVsZS5pZ25vcmVFbXB0eSA9PSBJZ25vcmVFbXB0eUZpZWxkcy5ZZXMpXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGNvbnRpbnVlO1xyXG5cclxuICAgICAgICAgICAgICAgICAgICBsZXQgcmVzdWx0ID0gcnVsZS5kb1ZhbGlkYXRlKGZpZWxkSW5wdXQsIHJ1bGVJbmZvLnBhcmFtcyk7XHJcbiAgICAgICAgICAgICAgICAgICAgaWYgKHJlc3VsdCAhPT0gbnVsbCAmJiByZXN1bHQuZmFpbGVkID09IFZhbGlkYXRpb25GYWlsZWQuWWVzKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGFsbEVycm9ycy5wdXNoKGVycm9yUHJlZml4ICsgcmVzdWx0LnJlYXNvbik7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGZpZWxkRXJyb3IuY2xhc3NMaXN0LmFkZChcInZpc2libGVcIik7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGZpZWxkRXJyb3IuaW5uZXJIVE1MID0gYWxsRXJyb3JzLnNsaWNlKC0xKVswXTtcclxuICAgICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICApO1xyXG5cclxuICAgICAgICAvLyBQb3B1bGF0ZSB0aGUgZGl2IGNvbnRhaW5pbmcgYWxsIG9mIHRoZSBlcnJvcnMgZm91bmQuXHJcbiAgICAgICAgbGV0IGRpdkFsbEVycm9ycyA9IGZvcm0ucXVlcnlTZWxlY3RvcihcIiNkaXZBbGxFcnJvcnNcIik7XHJcbiAgICAgICAgaWYgKGRpdkFsbEVycm9ycyAhPT0gdW5kZWZpbmVkICYmIGRpdkFsbEVycm9ycyAhPT0gbnVsbCkge1xyXG4gICAgICAgICAgICBkaXZBbGxFcnJvcnMuaW5uZXJIVE1MID0gXCJcIjtcclxuXHJcbiAgICAgICAgICAgIGxldCBsaXN0ID0gZG9jdW1lbnQuY3JlYXRlRWxlbWVudChcInVsXCIpO1xyXG4gICAgICAgICAgICBkaXZBbGxFcnJvcnMuYXBwZW5kQ2hpbGQobGlzdCk7XHJcblxyXG4gICAgICAgICAgICBmb3IgKGxldCBlcnJvciBvZiBhbGxFcnJvcnMpIHtcclxuICAgICAgICAgICAgICAgIGxldCBpdGVtID0gZG9jdW1lbnQuY3JlYXRlRWxlbWVudChcImxpXCIpO1xyXG4gICAgICAgICAgICAgICAgaXRlbS5pbm5lclRleHQgPSBlcnJvcjtcclxuICAgICAgICAgICAgICAgIGxpc3QuYXBwZW5kQ2hpbGQoaXRlbSk7XHJcbiAgICAgICAgICAgIH1cclxuXHJcbiAgICAgICAgICAgIGlmIChhbGxFcnJvcnMubGVuZ3RoID4gMClcclxuICAgICAgICAgICAgICAgIGRpdkFsbEVycm9ycy5jbGFzc0xpc3QuYWRkKFwidmlzaWJsZVwiKTtcclxuICAgICAgICAgICAgZWxzZVxyXG4gICAgICAgICAgICAgICAgZGl2QWxsRXJyb3JzLmNsYXNzTGlzdC5yZW1vdmUoXCJ2aXNpYmxlXCIpO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgcmV0dXJuIGFsbEVycm9ycy5sZW5ndGggPT0gMDtcclxuICAgIH1cclxufSJdfQ==