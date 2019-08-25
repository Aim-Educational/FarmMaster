/** A class that contains information about a rule. */
declare class ValidationRuleInfo {
    /** The name of the rule. */
    name: string;
    /** The parameters given to the rule. */
    params: string[];
    constructor();
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
    static fromString(ruleString: string): ValidationRuleInfo;
}
declare enum ValidationFailed {
    No = 0,
    Yes = 1
}
declare enum IgnoreEmptyFields {
    No = 0,
    Yes = 1
}
declare class ValidationRuleResult {
    failed: ValidationFailed;
    reason: string;
    constructor(failed: ValidationFailed, reason: string);
}
declare abstract class ValidationRule {
    readonly expectedParamCount: number;
    readonly name: string;
    readonly ignoreEmpty: IgnoreEmptyFields;
    constructor(name: string, expectedParamCount: number, ignoreEmpty: IgnoreEmptyFields);
    abstract doValidate(target: HTMLInputElement, params: string[]): ValidationRuleResult;
}
declare class ValidationRuleEmpty extends ValidationRule {
    constructor();
    doValidate(target: HTMLInputElement, params: string[]): ValidationRuleResult;
}
declare class ValidationRuleChecked extends ValidationRule {
    constructor();
    doValidate(target: HTMLInputElement, params: string[]): ValidationRuleResult;
}
declare class ValidationRuleRegex extends ValidationRule {
    constructor();
    doValidate(target: HTMLInputElement, params: string[]): ValidationRuleResult;
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
declare class Validation {
    private static rules;
    static addRule(rule: ValidationRule): void;
    static hookupForm(form: string | HTMLFormElement): void;
    static validateForm(form: HTMLFormElement): boolean;
}
//# sourceMappingURL=validation.d.ts.map