/**
 * Base class for validation handlers.
 *
 * For people reading the JS file, refer to the typescript typings to see what functions this class defines.
 *
 * @see `IValidationUIHandler`
 * @see `IValidationLogicHandler`
 */
export declare abstract class IValidationHandlerCommon {
    /**
     * Called when an element fails validation.
     *
     * @param element The element that failed validation.
     * @param reasons The reasons that the validation failed.
     */
    abstract onElementFailedValidation(element: HTMLElement, reasons: string[]): void;
    /**
     * Called when an element passes validation.
     *
     * @param element The element that passed validation.
     */
    abstract onElementPassedValidation(element: HTMLElement): void;
    /**
     * Called when a form fails validation, because at least one of it's inputs failed.
     *
     * @param form              The form that failed validation.
     * @param reasonsPerElement The reasons each element failed validation (only the elements that *did* fail validation that is).
     * @param formSubmitEvent   [Nullable] The submit event. This is mostly passed so code can call `.preventDefault` on it.
     */
    abstract onFormFailedValidation(form: HTMLFormElement, reasonsPerElement: Map<HTMLElement, string[]>, formSubmitEvent: Event | null): void;
    /**
     * Called when a form passes validation.
     *
     * @param form The form that passed validation.
     */
    abstract onFormPassedValidation(form: HTMLFormElement): void;
}
/**
 * The validation handler that should be responsible for *only* updating UI elements, based on
 * how a form/element is validated. No impactful logic (such as preventing a form from submitting) should be done
 * with UI handlers, but instead with @see `IValidationLogicHandler`
 */
export declare abstract class IValidationUIHandler extends IValidationHandlerCommon {
    /**
     * Called whenever an element's value is changed.
     *
     * Imagine an element fails to validate, and the UI Handler shows a big error box somewhere around it.
     *
     * Then imagine that you want the UI Handler to be able to get rid of that error box just from the user
     * interacting with the element that failed validation, instead of having to perform validation again (e.g. you might
     * want to save validation for only when the submit button is pressed).
     *
     * This is the purpose of this callback.
     *
     * @param element The element who's value has changed, or has been interacted with in some way the user code deems important.
     */
    abstract onElementValueChange(element: HTMLElement): void;
}
/**
 * The validation handler that should be responsible for *only* impactful logic based off of failed/passed
 * validation of forms and elements.
 *
 * No UI code should be placed into this handler, as that should be done with @see `IValidationUIHandler`
 *
 * @see `ValidationLogicHandlerNoSubmit`
 */
export declare abstract class IValidationLogicHandler extends IValidationHandlerCommon {
}
/**
 * A built-in logic handler who's only function is to prevent a form from submitting if there are any
 * validation errors.
 *
 * Name: "no-submit-form"
 */
export declare class ValidationLogicHandlerNoSubmit extends IValidationHandlerCommon {
    onElementFailedValidation(element: HTMLElement, reasons: string[]): void;
    onElementPassedValidation(element: HTMLElement): void;
    onFormPassedValidation(form: HTMLFormElement): void;
    onFormFailedValidation(form: HTMLFormElement, reasonsPerElement: Map<HTMLElement, string[]>, formSubmitEvent: Event | null): void;
}
/**
 * A selector is responsible for selecting all of the elements in a form to perform validation on.
 *
 * Instead of hardcoding which elements to include (<input>, <select>, etc.) and just *assuming* that'll be fine,
 * this library will instead allow each policy to specify a selector to perform this function.
 *
 * This allows full customisation of which elements are to be included in validation of a form so certain
 * setups or edge cases can still be supported with a little extra effort on the user.
 *
 * @see `ValidationFormElementSelectorDefault`
 */
export declare abstract class IValidationFormElementSelector {
    /**
     * Called when a validation function needs to know which elements inside a form it should
     * include in validation.
     *
     * @param form The form to get elements from.
     * @returns An array of `HTMLElement`s to be included in validation.
     */
    abstract onFindElementsToValidate(form: HTMLFormElement): HTMLElement[];
}
/**
 * The default selector that should work for most basic forms.
 *
 * Includes:
 *  * All <input> tags
 *  * All <select> tags
 *  * All <textarea> tags
 *
 * Name: "default"
 */
export declare class ValidationFormElementSelectorDefault extends IValidationFormElementSelector {
    onFindElementsToValidate(form: HTMLFormElement): HTMLElement[];
}
/**
 * A rule that performs validation on an element.
 *
 * Rules can be given parameters by user code.
 *
 * @see `ValidationRuleIsNotEmpty`
 */
export declare abstract class ValidationRule {
    /**
     * Attempts to validate an element.
     *
     * Notes:
     *  The returned object has a `reasons` property detailing the reasons why
     *  validation failed. This property is **only** used on a *failed* validation
     *  and is completely ignored on a *passing* validation.
     *
     *  This means the implementing code does not need to go out of their way to avoid populating
     *  this property in the event of a successful validation.
     *
     * @param element The element to validate.
     * @param params  The parameters passed by the user code.
     *
     * @returns A `ValidationResult` object containing information about the validation.
     */
    abstract onValidateElement(element: HTMLElement, params: string[]): ValidationResult;
    /**
     * A helper function that throws an `Error` with a standardised error message.
     *
     * This specific error function is used when there is a mis-match between the amount
     * of parameters expected from the user, and the amount actually given.
     *
     * @param expected The amount of parameters expected.
     * @param got      The amount of parameters given.
     */
    protected throwParamCountMismatch(expected: number, got: number): void;
    /**
     * A helper function to get the value from the given element.
     *
     * Algorithm:
     *  The element's "string" value is taken from its `.value` property.
     *      If `.value` doesn't exist, then an error is thrown.
     *  If the element is an `<input type="number">` then the "string" value is casted to a number.
     *      If the cast fails then an error is thrown.
     *      That number is then returned as the final value.
     *  Otherwise, the "string" value is returned as the final value.
     *
     * @param element The element to get the value for.
     *
     * @returns The `element`'s value as either a string or number.
     */
    protected getValueFromElement(element: HTMLElement): string | number;
}
/**
 * A validation rule that only passes if:
 *  * The given `element` has a `.value` property.
 *  * The `.value` property is not null.
 *  * The `.value` property is a string.
 *  * The `.value` property is not made up entirely of whitespace, and has a length greater than 0.
 *
 * Ruleset: "not-empty"
 */
export declare class ValidationRuleIsNotEmpty extends ValidationRule {
    onValidateElement(element: HTMLElement, params: string[]): ValidationResult;
}
/**
 * A validation rule that only passes if:
 *  * The given `element` has a `.value` property.
 *  * The `.value` property is a string.
 *  * The `.value` property matches the given regex (param 0).
 *
 * Ruleset: "regex: {pattern}"
 * Example: "regex: (\w+\.\w+)@(\w+\.\w+)" for a basic (but bad) email regex.
 * Reminder: Characters '&' and ';' must be escaped when used inside of parameters, as they have special meaning.
 */
export declare class ValidationRuleRegex extends ValidationRule {
    onValidateElement(element: HTMLElement, params: string[]): ValidationResult;
}
/**
 * (Min)
 * A validation rule that only passes if:
 *  * The given `element` has a `.value` property.
 *  * The `.value` property is a string or number.
 *  * If `.value` is a string, then it's length must be >= the wanted min length (param 0).
 *  * If `.value` is a number, then it's value must be >= the wanted min value (param 0).
 *
 * Ruleset: "min: {amount}"
 * Example: "min: 20"
 *
 * (Max)
 * A validation rule that only passes if:
 *  * The given `element` has a `.value` property.
 *  * The `.value` property is a string or number.
 *  * If `.value` is a string, then it's length must be <= the wanted max length (param 0).
 *  * If `.value` is a number, then it's value must be <= the wanted max value (param 0).
 *
 * Ruleset: "max: {amount}"
 * Example: "max: 20"
 */
export declare class ValidationRuleMinMax extends ValidationRule {
    private _isMin;
    constructor(_isMin: boolean);
    onValidateElement(element: HTMLElement, params: string[]): ValidationResult;
}
/**
 * Contains the result of a validation attempt.
 */
export declare class ValidationResult {
    passed: boolean;
    reasons: string[];
    /**
     * @param passed  True if the validation passed. False otherwise.
     * @param reasons The reasons validation failed. This property is completely ignored on a successful validation.
     */
    constructor(passed: boolean, reasons: string[]);
}
/**
 * A policy is a set consisting of:
 *  * A UI handler (`IValidationUIHandler`)
 *  * A logic handler (`IValidationLogicHandler`)
 *  * A selector (`IValidationFormElementSelector`)
 *
 * Policies are used to group these various objects under a common name (e.g. 'bootstrap-theme-one', 'foundation-allow-failed-forms', etc.)
 * to allow the user code to easily specify what sets of objects that their validation should use.
 */
export declare class ValidationPolicy {
    uiHandler: IValidationUIHandler;
    logicHandler: IValidationLogicHandler;
    selector: IValidationFormElementSelector;
    constructor(uiHandler: IValidationUIHandler, logicHandler: IValidationLogicHandler, selector: IValidationFormElementSelector);
}
/**
 * The main class that provides all of the validation facilities of this library.
 *
 * This class is a static class, so all functions can be called like `Validation.someFunc`.
 *
 * Please see the wiki for this project on github for steps on how to get started using this library.
 */
export declare class Validation {
    private static _policies;
    private static _uiHandlers;
    private static _logicHandlers;
    private static _rules;
    private static _selectors;
    private static _defaultPolicy;
    private static addHandlerImpl;
    private static validateElementImpl;
    private static getPolicyByName;
    /**
     * Registers a UI handler.
     *
     * @param name    The name to associate with the handler.
     * @param handler The handler object.
     *
     * @see `IValidationUIHandler`
     */
    static addUIHandler(name: string, handler: IValidationUIHandler): void;
    /**
     * Registers a logic handler.
     *
     * @param name    The name to associate with the handler.
     * @param handler The handler object.
     *
     * @see `IValidationLogicHandler`
     */
    static addLogicHandler(name: string, handler: IValidationLogicHandler): void;
    /**
     * Registers a selector.
     *
     * @param name     The name to associate with the selector.
     * @param selector The selector object.
     *
     * @see `IValidationFormElementSelector`
     */
    static addSelector(name: string, selector: IValidationFormElementSelector): void;
    /**
     * Registers a rule.
     *
     * @param name The name to associate with the rule. This name is also how rulesets make use of this rule.
     * @param rule The rule object.
     */
    static addRule(name: string, rule: ValidationRule): void;
    /**
     * Defines a policy.
     *
     * Notes:
     *  The first policy to be defined will become the 'default' policy - the policy that
     *  will be used if the user code doesn't specify a specific policy to use for certain
     *  functions.
     *
     * @param policyName        The name to give this policy.
     * @param uiHandlerName     The name of the UI handler to use.
     * @param logicHandlerName  The name of the logic handler to use.
     * @param selectorName      The name of the selector to use. Defaults to "default", which is a built-in selector.
     */
    static definePolicy(policyName: string, uiHandlerName: string, logicHandlerName: string, selectorName?: string): void;
    /**
     * Validates the given element, using the specified ruleset and policy.
     *
     * On success:
     *  * The `onElementPassedValidation` function for the given policy's UI handler is called.
     *  * The `onElementPassedValidation` function for the given policy's logic handler is called.
     *  * An empty array (length of 0) is returned.
     *
     * On fail:
     *  * The `onElementFailedValidation` function for the given policy's UI handler is called.
     *  * The `onElementFailedValidation` function for the given policy's logic handler is called.
     *  * An array of all validation errors is returned. This can still be an empty array if the rules used by the ruleset don't provide error messages.
     *
     * Notes:
     *  While it's true that failure and success can both result in the same return value, it's best to just assume that an
     *  empty array resulted in a successful validation.
     *
     *  This is because a rule should **always** provide an error message, and if it doesn't then it's poorly made and should
     *  be fixed.
     *
     * @param element The element to validate.
     * @param ruleset The ruleset that describes how to validate the given element.
     * @param policy  The name of the policy to use. If `null`, then the default policy is used.
     *
     * @returns See 'On success' and 'On fail'.
     */
    static validateElement(element: HTMLElement, ruleset: string, policy?: string | null): string[];
    /**
     * Validates a form by validating all of the elements specified by the given policy's selector.
     *
     * Elements:
     *  All elements to be validated are determined by calling the `onFindElementsToValidate` function for
     *  the given policy's selector. @see `IValidationFormElementSelector`
     *
     * Validation:
     *  The ruleset for each element is determined by looking for `element.dataset.ruleset`. This means
     *  that the way to specify the ruleset for each element, is to do something like `<input data-ruleset='not-empty' />`.
     *
     *  The `Validation.validateElement` function is then called on the element, using all the information this function knows about.
     *
     *  If an element does not have a ruleset, then it is ignored.
     *
     * On success:
     *  This happens if every single non-ignored element passes validation.
     *  * The `onFormPassedValidation` function for the given policy's UI handler is called.
     *  * The `onFormPassedValidation` function for the given policy's logic handler is called.
     *  * An empty map (`.size` of 0) is returned.
     *
     * On failed:
     *  This happens if any non-ignored element fails validation.
     *  * The `onFormFailedValidation` function for the given policy's UI handler is called.
     *  * The `onFormFailedValidation` function for the given policy's logic handler is called.
     *  * A map, where the key is an element that failed validation, and the value is a string array containing the reasons *why* it failed, is returned.
     *
     * Notes:
     *  It is up to the policy's logic handler to call `submitEvent.preventDefault` to prevent a form from submitting on failing validation.
     *
     * @param form        The form to validate.
     * @param submitEvent The submit event for the given form. This is only passed if validation is being performed as part of the `onsubmit` or `addEventHandler('submit')` events for a form.
     * @param policy      The name of the policy to use for validation. If `null` is passed then the default policy is used.
     *
     * @returns See `On success` and `On failed`.
     */
    static validateForm(form: HTMLFormElement, submitEvent?: Event | null, policy?: string | null): Map<HTMLElement, string[]>;
    /**
     * Hooks onto a form's "submit" event to perform validation when the form is submitted.
     *
     * Notes:
     *  This function, using the given policy's selector, also calls `Validation.hookupElement` on each
     *  element that is to be validated.
     *
     *  It is up to the policy's logic handler to call `preventDefault` to stop the form from submitting
     *  on validation failure.
     *
     * @param form   The form to hook onto.
     * @param policy The name of the policy to use. If `null` is passed then the default policy is used.
     */
    static hookupForm(form: HTMLFormElement, policy?: string | null): void;
    /**
     * Hooks onto the "change" event for the given element, which will then call the `onElementValueChange`
     * callback for the given policy's UI handler.
     *
     * @param element The element to hook onto.
     * @param policy  The name of the policy to use. If `null` is passed then the default policy is used.
     */
    static hookupElement(element: HTMLElement, policy?: string | null): void;
}
declare class RuleParserResult {
    ruleName: string;
    params: string[];
    constructor(ruleName: string, params: string[]);
}
export declare function parseRuleset(ruleset: string): RuleParserResult[];
export {};
//# sourceMappingURL=jaster-validate.d.ts.map