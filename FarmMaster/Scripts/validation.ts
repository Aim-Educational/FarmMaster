/** A class that contains information about a rule. */
class ValidationRuleInfo {
    /** The name of the rule. */
    name: string;

    /** The parameters given to the rule. */
    params: string[];

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

enum ValidationFailed {
    No,
    Yes
}

enum IgnoreEmptyFields {
    No,
    Yes
}

class ValidationRuleResult {
    failed: ValidationFailed;
    reason: string;

    constructor(failed: ValidationFailed, reason: string) {
        this.failed = failed;
        this.reason = reason;
    }
}

abstract class ValidationRule {
    public readonly expectedParamCount: number = 0;
    public readonly name: string;
    public readonly ignoreEmpty: IgnoreEmptyFields;

    constructor(name: string, expectedParamCount: number, ignoreEmpty: IgnoreEmptyFields) {
        this.name = name;
        this.expectedParamCount = expectedParamCount;
        this.ignoreEmpty = ignoreEmpty;
    }

    public abstract doValidate(target: HTMLInputElement, params: string[]): ValidationRuleResult;
}

class ValidationRuleEmpty extends ValidationRule {
    constructor() {
        super("empty", 0, IgnoreEmptyFields.No);
    }

    public doValidate(target: HTMLInputElement, params: string[]): ValidationRuleResult {
        return (
            target.value.length > 0
                ? null
                : new ValidationRuleResult(ValidationFailed.Yes, "is empty.")
        );
    }
}

class ValidationRuleChecked extends ValidationRule {
    constructor() {
        super("checked", 0, IgnoreEmptyFields.No);
    }

    public doValidate(target: HTMLInputElement, params: string[]): ValidationRuleResult {
        return (
            target.checked
                ? null
                : new ValidationRuleResult(ValidationFailed.Yes, "is not checked.")
        );
    }
}

class ValidationRuleRegex extends ValidationRule {
    constructor() {
        super("regex", 1, IgnoreEmptyFields.Yes);
    }

    public doValidate(target: HTMLInputElement, params: string[]): ValidationRuleResult {
        return (
            new RegExp(params[0]).test(target.value)
                ? null
                : new ValidationRuleResult(ValidationFailed.Yes, "does not match pattern: "+params[0])
        );
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
    private static rules: ValidationRule[] = [
        new ValidationRuleEmpty(),
        new ValidationRuleChecked(),
        new ValidationRuleRegex()
    ];

    static addRule(rule: ValidationRule): void {
        this.rules.push(rule);
    }

    static hookupForm(form: string | HTMLFormElement) {
        // Get the form.
        let actualForm: HTMLFormElement = null;
        if (typeof form === "string")
            actualForm = <HTMLFormElement>document.getElementById(form);
        else
            actualForm = form;

        // If we don't validate or an exception is thrown, stop the form from submitting.
        actualForm.addEventListener("submit", function (e) {
            try {
                if (!Validation.validateForm(actualForm))
                    e.preventDefault();
            }
            catch(ex) {
                e.preventDefault();
                throw ex;
            }
        }, false);
    }

    static validateForm(form: HTMLFormElement): boolean {
        let allErrors: string[] = [];
        let handledInputs: HTMLInputElement[] = [];

        form.querySelectorAll(".field")
            .forEach(fieldSection =>
            {
                // Only get inputs with validation rules.
                let fieldInput = fieldSection.querySelector<HTMLInputElement>("input.needs.validation[data-validation-rules]");
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
                let fieldError = fieldSection.querySelector<HTMLDivElement>(".ui.error.message, .ui.red.prompt");
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
            }
        );

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