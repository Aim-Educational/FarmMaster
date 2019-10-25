/**
 * Base class for validation handlers.
 *
 * For people reading the JS file, refer to the typescript typings to see what functions this class defines.
 *
 * @see `IValidationUIHandler`
 * @see `IValidationLogicHandler`
 */
export class IValidationHandlerCommon {
}
/**
 * The validation handler that should be responsible for *only* updating UI elements, based on
 * how a form/element is validated. No impactful logic (such as preventing a form from submitting) should be done
 * with UI handlers, but instead with @see `IValidationLogicHandler`
 */
export class IValidationUIHandler extends IValidationHandlerCommon {
}
/**
 * The validation handler that should be responsible for *only* impactful logic based off of failed/passed
 * validation of forms and elements.
 *
 * No UI code should be placed into this handler, as that should be done with @see `IValidationUIHandler`
 *
 * @see `ValidationLogicHandlerNoSubmit`
 */
export class IValidationLogicHandler extends IValidationHandlerCommon {
}
/**
 * A built-in logic handler who's only function is to prevent a form from submitting if there are any
 * validation errors.
 *
 * Name: "no-submit-form"
 */
export class ValidationLogicHandlerNoSubmit extends IValidationHandlerCommon {
    onElementFailedValidation(element, reasons) { }
    onElementPassedValidation(element) { }
    onFormPassedValidation(form) { }
    onFormFailedValidation(form, reasonsPerElement, formSubmitEvent) {
        if (formSubmitEvent)
            formSubmitEvent.preventDefault();
    }
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
export class IValidationFormElementSelector {
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
export class ValidationFormElementSelectorDefault extends IValidationFormElementSelector {
    onFindElementsToValidate(form) {
        const elements = [];
        form.querySelectorAll("input").forEach(e => elements.push(e));
        form.querySelectorAll("select").forEach(e => elements.push(e));
        form.querySelectorAll("textarea").forEach(e => elements.push(e));
        return elements;
    }
}
/**
 * A rule that performs validation on an element.
 *
 * Rules can be given parameters by user code.
 *
 * @see `ValidationRuleIsNotEmpty`
 */
export class ValidationRule {
    /**
     * A helper function that throws an `Error` with a standardised error message.
     *
     * This specific error function is used when there is a mis-match between the amount
     * of parameters expected from the user, and the amount actually given.
     *
     * @param expected The amount of parameters expected.
     * @param got      The amount of parameters given.
     */
    throwParamCountMismatch(expected, got) {
        throw new Error("Param count mistmatch. Expected " + expected + " parameters, but got " + got + " instead.");
    }
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
    getValueFromElement(element) {
        const elementAny = element;
        if (elementAny.value === undefined)
            throw new Error("Element doesn't contain a '.value' field. This rule cannot be used.");
        let value = elementAny.value;
        if (element instanceof HTMLInputElement && element.type === "number") {
            value = Number(value);
            if (value === NaN)
                throw new Error("Element is an <input> of type 'number', yet it's value cannot be converted to a number. Element: " + element);
        }
        return value;
    }
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
export class ValidationRuleIsNotEmpty extends ValidationRule {
    onValidateElement(element, params) {
        if (params.length > 0)
            super.throwParamCountMismatch(0, params.length);
        const elementAny = element;
        if (elementAny.value === undefined)
            return new ValidationResult(false, ["Element doesn't contain a '.value' field. This rule cannot be used."]);
        const value = elementAny.value;
        if (value === null)
            return new ValidationResult(false, ["Value cannot be null."]);
        if (typeof value === "string")
            return new ValidationResult(value.trim().length > 0, ["Value cannot be empty or full of whitespace."]);
        else
            return new ValidationResult(false, ["Value type '" + typeof value + "' is not supported by this rule."]);
    }
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
export class ValidationRuleRegex extends ValidationRule {
    onValidateElement(element, params) {
        if (params.length !== 1)
            super.throwParamCountMismatch(1, params.length);
        const value = super.getValueFromElement(element);
        if (typeof value === "string") {
            const result = (new RegExp(params[0])).test(value);
            return new ValidationResult(result, ["Value does not match pattern: " + params[0]]);
        }
        else
            return new ValidationResult(false, ["Value type '" + typeof value + "' is not supported by this rule."]);
    }
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
export class ValidationRuleMinMax extends ValidationRule {
    constructor(_isMin) {
        super();
        this._isMin = _isMin;
    }
    onValidateElement(element, params) {
        if (params.length !== 1)
            super.throwParamCountMismatch(1, params.length);
        const param = Number(params[0]);
        if (param === NaN)
            throw new Error("Parameter '" + params[0] + "' is not a valid number.");
        const value = super.getValueFromElement(element);
        if (typeof value === "string") {
            return (this._isMin)
                ? new ValidationResult(value.length >= param, ["Value must be at least " + param + " characters wide."])
                : new ValidationResult(value.length <= param, ["Value cannot be more than " + param + " characters wide."]);
        }
        else if (typeof value === "number") {
            return (this._isMin)
                ? new ValidationResult(value >= param, ["Value must be greater than or equal to " + param])
                : new ValidationResult(value <= param, ["Value must be less than or equal to " + param]);
        }
        else
            return new ValidationResult(false, ["Value type '" + typeof value + "' is not supported by this rule."]);
    }
}
/**
 * Contains the result of a validation attempt.
 */
export class ValidationResult {
    /**
     * @param passed  True if the validation passed. False otherwise.
     * @param reasons The reasons validation failed. This property is completely ignored on a successful validation.
     */
    constructor(passed, reasons) {
        this.passed = passed;
        this.reasons = reasons;
    }
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
export class ValidationPolicy {
    constructor(uiHandler, logicHandler, selector) {
        this.uiHandler = uiHandler;
        this.logicHandler = logicHandler;
        this.selector = selector;
    }
}
/**
 * The main class that provides all of the validation facilities of this library.
 *
 * This class is a static class, so all functions can be called like `Validation.someFunc`.
 *
 * Please see the wiki for this project on github for steps on how to get started using this library.
 */
export class Validation {
    static addHandlerImpl(name, handler, map) {
        if (map.has(name))
            throw new Error("There is already a handler called '" + name + "'");
        if (handler === null || handler === undefined)
            throw new Error("Parameter 'handler' is either null or undefined.");
        map.set(name, handler);
    }
    static validateElementImpl(element, ruleset, policy) {
        const reasonsFailed = [];
        const rulesetResult = parseRuleset(ruleset);
        for (let result of rulesetResult) {
            if (!this._rules.has(result.ruleName))
                throw new Error("There is no rule named '" + result.ruleName + "' when parsing ruleset: " + ruleset + "\nFor element: " + element);
            const rule = this._rules.get(result.ruleName);
            const validationResult = rule.onValidateElement(element, result.params);
            if (!validationResult.passed) {
                for (let reason of validationResult.reasons)
                    reasonsFailed.push(reason);
            }
        }
        return reasonsFailed;
    }
    static getPolicyByName(name) {
        if (this._policies.size === 0)
            throw new Error("No policies have been created.");
        let policyToUse = null;
        if (name !== null) {
            if (!this._policies.has(name))
                throw new Error("There is no policy named '" + name + "'");
            policyToUse = this._policies.get(name);
        }
        else
            policyToUse = this._policies.get(this._defaultPolicy);
        if (policyToUse === null)
            throw new Error("Internal error - policyToUse should not be null in this situation.");
        return policyToUse;
    }
    /**
     * Registers a UI handler.
     *
     * @param name    The name to associate with the handler.
     * @param handler The handler object.
     *
     * @see `IValidationUIHandler`
     */
    static addUIHandler(name, handler) {
        this.addHandlerImpl(name, handler, this._uiHandlers);
    }
    /**
     * Registers a logic handler.
     *
     * @param name    The name to associate with the handler.
     * @param handler The handler object.
     *
     * @see `IValidationLogicHandler`
     */
    static addLogicHandler(name, handler) {
        this.addHandlerImpl(name, handler, this._logicHandlers);
    }
    /**
     * Registers a selector.
     *
     * @param name     The name to associate with the selector.
     * @param selector The selector object.
     *
     * @see `IValidationFormElementSelector`
     */
    static addSelector(name, selector) {
        this.addHandlerImpl(name, selector, this._selectors);
    }
    /**
     * Registers a rule.
     *
     * @param name The name to associate with the rule. This name is also how rulesets make use of this rule.
     * @param rule The rule object.
     */
    static addRule(name, rule) {
        if (this._rules.has(name))
            throw new Error("There is already a rule called '" + name + "'");
        if (rule === null || rule === undefined)
            throw new Error("Parameter 'rule' is either null or undefined.");
        this._rules.set(name, rule);
    }
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
    static definePolicy(policyName, uiHandlerName, logicHandlerName, selectorName = "default") {
        if (this._policies.has(policyName))
            throw new Error("There is already a policy called '" + policyName + "'");
        if (!this._uiHandlers.has(uiHandlerName))
            throw new Error("There is no UI handler called '" + uiHandlerName + "'");
        if (!this._logicHandlers.has(logicHandlerName))
            throw new Error("There is no Logic handler called '" + logicHandlerName + "'");
        if (!this._selectors.has(selectorName))
            throw new Error("There is no Selector called '" + selectorName + "'");
        if (this._defaultPolicy === null)
            this._defaultPolicy = policyName;
        const ui = this._uiHandlers.get(uiHandlerName);
        const logic = this._logicHandlers.get(logicHandlerName);
        const selector = this._selectors.get(selectorName);
        this._policies.set(policyName, new ValidationPolicy(ui, logic, selector));
    }
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
    static validateElement(element, ruleset, policy = null) {
        const policyToUse = this.getPolicyByName(policy);
        // Perform validation.
        const reasonsFailed = this.validateElementImpl(element, ruleset, policyToUse);
        if (reasonsFailed.length == 0) { // Validation passed
            policyToUse.uiHandler.onElementPassedValidation(element);
            policyToUse.logicHandler.onElementPassedValidation(element);
        }
        else {
            policyToUse.uiHandler.onElementFailedValidation(element, reasonsFailed);
            policyToUse.logicHandler.onElementFailedValidation(element, reasonsFailed);
        }
        return reasonsFailed;
    }
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
    static validateForm(form, submitEvent = null, policy = null) {
        const policyToUse = this.getPolicyByName(policy);
        const elements = policyToUse.selector.onFindElementsToValidate(form);
        const reasons = new Map();
        for (let element of elements) {
            const ruleset = element.dataset.ruleset;
            if (!ruleset)
                continue;
            const reasonsFailed = this.validateElement(element, ruleset, policy);
            if (reasonsFailed.length > 0)
                reasons.set(element, reasonsFailed);
        }
        if (reasons.size === 0) {
            policyToUse.logicHandler.onFormPassedValidation(form);
            policyToUse.uiHandler.onFormPassedValidation(form);
        }
        else {
            policyToUse.logicHandler.onFormFailedValidation(form, reasons, submitEvent);
            policyToUse.uiHandler.onFormFailedValidation(form, reasons, submitEvent);
        }
        return reasons;
    }
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
    static hookupForm(form, policy = null) {
        form.addEventListener("submit", e => Validation.validateForm(form, e, policy));
        for (let element of this.getPolicyByName(policy).selector.onFindElementsToValidate(form))
            this.hookupElement(element, policy);
    }
    /**
     * Hooks onto the "change" event for the given element, which will then call the `onElementValueChange`
     * callback for the given policy's UI handler.
     *
     * @param element The element to hook onto.
     * @param policy  The name of the policy to use. If `null` is passed then the default policy is used.
     */
    static hookupElement(element, policy = null) {
        const policyToUse = this.getPolicyByName(policy);
        element.addEventListener("change", e => policyToUse.uiHandler.onElementValueChange(element));
    }
}
Validation._policies = new Map();
Validation._uiHandlers = new Map();
Validation._logicHandlers = new Map();
Validation._rules = new Map();
Validation._selectors = new Map();
Validation._defaultPolicy = null;
// BUILT-IN STUFF
Validation.addRule("not-empty", new ValidationRuleIsNotEmpty());
Validation.addRule("regex", new ValidationRuleRegex());
Validation.addRule("min", new ValidationRuleMinMax(true));
Validation.addRule("max", new ValidationRuleMinMax(false));
Validation.addLogicHandler("no-submit-form", new ValidationLogicHandlerNoSubmit());
Validation.addSelector("default", new ValidationFormElementSelectorDefault());
class RuleParserResult {
    constructor(ruleName, params) {
        this.ruleName = ruleName;
        this.params = params;
    }
}
// Exported simply so the tests can import it.
// Rulesets aren't complex enough to bother with a proper parser, so we get this hacked together one.
export function parseRuleset(ruleset) {
    const result = [];
    let ParseState;
    (function (ParseState) {
        ParseState[ParseState["NAME"] = 0] = "NAME";
        ParseState[ParseState["VALUE"] = 1] = "VALUE";
    })(ParseState || (ParseState = {}));
    let state = ParseState.NAME;
    let index = 0;
    while (true) {
        // Skip whitespace
        while (index < ruleset.length && /\s/.test(ruleset[index]))
            index++;
        if (index >= ruleset.length)
            break;
        let startIndex = index;
        switch (state) {
            case ParseState.NAME:
                while (true) {
                    if (index >= ruleset.length || ruleset[index] == ':' || ruleset[index] == ';')
                        break;
                    if (/\s/.test(ruleset[index]))
                        throw new Error("Unexpected whitespace at index " + index + " while reading rule name. Ruleset = " + ruleset);
                    index++;
                }
                result.push(new RuleParserResult(ruleset.substring(startIndex, index), []));
                state = (ruleset[index++] == ':') ? ParseState.VALUE : ParseState.NAME;
                break;
            case ParseState.VALUE:
                let buffer = "";
                while (true) {
                    if (index >= ruleset.length)
                        break;
                    // Handle escaping special characters.
                    if (index > 0 && ruleset[index - 1] == '\\' && (ruleset[index] == '&' || ruleset[index] == ';')) {
                        buffer += ruleset.substring(startIndex, index - 1); // - 1 to remove the backslash
                        startIndex = index++;
                        continue;
                    }
                    if (ruleset[index] == '&' || ruleset[index] == ';')
                        break;
                    index++;
                }
                if (index - startIndex > 0)
                    buffer += ruleset.substring(startIndex, index);
                result[result.length - 1].params.push(buffer.trim());
                state = (ruleset[index++] == '&') ? ParseState.VALUE : ParseState.NAME;
                break;
            default: throw "internal error";
        }
    }
    return result;
}
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiamFzdGVyLXZhbGlkYXRlLmpzIiwic291cmNlUm9vdCI6IiIsInNvdXJjZXMiOlsiLi4vc3JjL2phc3Rlci12YWxpZGF0ZS50cyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiQUFFQTs7Ozs7OztHQU9HO0FBQ0gsTUFBTSxPQUFnQix3QkFBd0I7Q0ErQjdDO0FBRUQ7Ozs7R0FJRztBQUNILE1BQU0sT0FBZ0Isb0JBQXFCLFNBQVEsd0JBQXdCO0NBZTFFO0FBRUQ7Ozs7Ozs7R0FPRztBQUNILE1BQU0sT0FBZ0IsdUJBQXdCLFNBQVEsd0JBQXdCO0NBQzdFO0FBRUQ7Ozs7O0dBS0c7QUFDSCxNQUFNLE9BQU8sOEJBQStCLFNBQVEsd0JBQXdCO0lBQ2pFLHlCQUF5QixDQUFDLE9BQW9CLEVBQUUsT0FBaUIsSUFBUyxDQUFDO0lBQzNFLHlCQUF5QixDQUFDLE9BQW9CLElBQVMsQ0FBQztJQUN4RCxzQkFBc0IsQ0FBQyxJQUFxQixJQUFTLENBQUM7SUFFdEQsc0JBQXNCLENBQUMsSUFBcUIsRUFBRSxpQkFBNkMsRUFBRSxlQUE2QjtRQUM3SCxJQUFHLGVBQWU7WUFDZCxlQUFlLENBQUMsY0FBYyxFQUFFLENBQUM7SUFDekMsQ0FBQztDQUNKO0FBRUQ7Ozs7Ozs7Ozs7R0FVRztBQUNILE1BQU0sT0FBZ0IsOEJBQThCO0NBU25EO0FBRUQ7Ozs7Ozs7OztHQVNHO0FBQ0gsTUFBTSxPQUFPLG9DQUFxQyxTQUFRLDhCQUE4QjtJQUM3RSx3QkFBd0IsQ0FBQyxJQUFxQjtRQUNqRCxNQUFNLFFBQVEsR0FBa0IsRUFBRSxDQUFDO1FBRW5DLElBQUksQ0FBQyxnQkFBZ0IsQ0FBQyxPQUFPLENBQUMsQ0FBQyxPQUFPLENBQUMsQ0FBQyxDQUFDLEVBQUUsQ0FBQyxRQUFRLENBQUMsSUFBSSxDQUFDLENBQUMsQ0FBQyxDQUFDLENBQUM7UUFDOUQsSUFBSSxDQUFDLGdCQUFnQixDQUFDLFFBQVEsQ0FBQyxDQUFDLE9BQU8sQ0FBQyxDQUFDLENBQUMsRUFBRSxDQUFDLFFBQVEsQ0FBQyxJ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SUFBSSxPQUFPLENBQUMsS0FBSyxHQUFDLENBQUMsQ0FBQyxJQUFJLElBQUksSUFBSSxDQUFDLE9BQU8sQ0FBQyxLQUFLLENBQUMsSUFBSSxHQUFHLElBQUksT0FBTyxDQUFDLEtBQUssQ0FBQyxJQUFJLEdBQUcsQ0FBQyxFQUFFO3dCQUMxRixNQUFNLElBQUksT0FBTyxDQUFDLFNBQVMsQ0FBQyxVQUFVLEVBQUUsS0FBSyxHQUFHLENBQUMsQ0FBQyxDQUFDLENBQUMsOEJBQThCO3dCQUNsRixVQUFVLEdBQUcsS0FBSyxFQUFFLENBQUM7d0JBQ3JCLFNBQVM7cUJBQ1o7b0JBRUQsSUFBRyxPQUFPLENBQUMsS0FBSyxDQUFDLElBQUksR0FBRyxJQUFJLE9BQU8sQ0FBQyxLQUFLLENBQUMsSUFBSSxHQUFHO3dCQUM3QyxNQUFNO29CQUVWLEtBQUssRUFBRSxDQUFDO2lCQUNYO2dCQUVELElBQUcsS0FBSyxHQUFHLFVBQVUsR0FBRyxDQUFDO29CQUNyQixNQUFNLElBQUksT0FBTyxDQUFDLFNBQVMsQ0FBQyxVQUFVLEVBQUUsS0FBSyxDQUFDLENBQUM7Z0JBRW5ELE1BQU0sQ0FBQyxNQUFNLENBQUMsTUFBTSxHQUFHLENBQUMsQ0FBQyxDQUFDLE1BQU0sQ0FBQyxJQUFJLENBQUMsTUFBTSxDQUFDLElBQUksRUFBRSxDQUFDLENBQUM7Z0JBQ3JELEtBQUssR0FBRyxDQUFDLE9BQU8sQ0FBQyxLQUFLLEVBQUUsQ0FBQyxJQUFJLEdBQUcsQ0FBQyxDQUFDLENBQUMsQ0FBQyxVQUFVLENBQUMsS0FBSyxDQUFDLENBQUMsQ0FBQyxVQUFVLENBQUMsSUFBSSxDQUFDO2dCQUN2RSxNQUFNO1lBRVYsT0FBTyxDQUFDLENBQUMsTUFBTSxnQkFBZ0IsQ0FBQztTQUNuQztLQUNKO0lBRUQsT0FBTyxNQUFNLENBQUM7QUFDbEIsQ0FBQyIsInNvdXJjZXNDb250ZW50IjpbImltcG9ydCB7IGlzTnVtYmVyIH0gZnJvbSBcInV0aWxcIjtcblxuLyoqXG4gKiBCYXNlIGNsYXNzIGZvciB2YWxpZGF0aW9uIGhhbmRsZXJzLlxuICogXG4gKiBGb3IgcGVvcGxlIHJlYWRpbmcgdGhlIEpTIGZpbGUsIHJlZmVyIHRvIHRoZSB0eXBlc2NyaXB0IHR5cGluZ3MgdG8gc2VlIHdoYXQgZnVuY3Rpb25zIHRoaXMgY2xhc3MgZGVmaW5lcy5cbiAqIFxuICogQHNlZSBgSVZhbGlkYXRpb25VSUhhbmRsZXJgXG4gKiBAc2VlIGBJVmFsaWRhdGlvbkxvZ2ljSGFuZGxlcmBcbiAqL1xuZXhwb3J0IGFic3RyYWN0IGNsYXNzIElWYWxpZGF0aW9uSGFuZGxlckNvbW1vbiB7XG4gICAgLyoqXG4gICAgICogQ2FsbGVkIHdoZW4gYW4gZWxlbWVudCBmYWlscyB2YWxpZGF0aW9uLlxuICAgICAqIFxuICAgICAqIEBwYXJhbSBlbGVtZW50IFRoZSBlbGVtZW50IHRoYXQgZmFpbGVkIHZhbGlkYXRpb24uXG4gICAgICogQHBhcmFtIHJlYXNvbnMgVGhlIHJlYXNvbnMgdGhhdCB0aGUgdmFsaWRhdGlvbiBmYWlsZWQuXG4gICAgICovXG4gICAgcHVibGljIGFic3RyYWN0IG9uRWxlbWVudEZhaWxlZFZhbGlkYXRpb24oZWxlbWVudDogSFRNTEVsZW1lbnQsIHJlYXNvbnM6IHN0cmluZ1tdKTogdm9pZDtcblxuICAgIC8qKlxuICAgICAqIENhbGxlZCB3aGVuIGFuIGVsZW1lbnQgcGFzc2VzIHZhbGlkYXRpb24uXG4gICAgICogXG4gICAgICogQHBhcmFtIGVsZW1lbnQgVGhlIGVsZW1lbnQgdGhhdCBwYXNzZWQgdmFsaWRhdGlvbi5cbiAgICAgKi9cbiAgICBwdWJsaWMgYWJzdHJhY3Qgb25FbGVtZW50UGFzc2VkVmFsaWRhdGlvbihlbGVtZW50OiBIVE1MRWxlbWVudCk6IHZvaWQ7XG5cbiAgICAvKipcbiAgICAgKiBDYWxsZWQgd2hlbiBhIGZvcm0gZmFpbHMgdmFsaWRhdGlvbiwgYmVjYXVzZSBhdCBsZWFzdCBvbmUgb2YgaXQncyBpbnB1dHMgZmFpbGVkLlxuICAgICAqIFxuICAgICAqIEBwYXJhbSBmb3JtICAgICAgICAgICAgICBUaGUgZm9ybSB0aGF0IGZhaWxlZCB2YWxpZGF0aW9uLlxuICAgICAqIEBwYXJhbSByZWFzb25zUGVyRWxlbWVudCBUaGUgcmVhc29ucyBlYWNoIGVsZW1lbnQgZmFpbGVkIHZhbGlkYXRpb24gKG9ubHkgdGhlIGVsZW1lbnRzIHRoYXQgKmRpZCogZmFpbCB2YWxpZGF0aW9uIHRoYXQgaXMpLlxuICAgICAqIEBwYXJhbSBmb3JtU3VibWl0RXZlbnQgICBbTnVsbGFibGVdIFRoZSBzdWJtaXQgZXZlbnQuIFRoaXMgaXMgbW9zdGx5IHBhc3NlZCBzbyBjb2RlIGNhbiBjYWxsIGAucHJldmVudERlZmF1bHRgIG9uIGl0LlxuICAgICAqL1xuICAgIHB1YmxpYyBhYnN0cmFjdCBvbkZvcm1GYWlsZWRWYWxpZGF0aW9uKGZvcm06IEhUTUxGb3JtRWxlbWVudCwgcmVhc29uc1BlckVsZW1lbnQ6IE1hcDxIVE1MRWxlbWVudCwgc3RyaW5nW10+LCBmb3JtU3VibWl0RXZlbnQ6IEV2ZW50IHwgbnVsbCk6IHZvaWQ7XG5cbiAgICAvKipcbiAgICAgKiBDYWxsZWQgd2hlbiBhIGZvcm0gcGFzc2VzIHZhbGlkYXRpb24uXG4gICAgICogXG4gICAgICogQHBhcmFtIGZvcm0gVGhlIGZvcm0gdGhhdCBwYXNzZWQgdmFsaWRhdGlvbi5cbiAgICAgKi9cbiAgICBwdWJsaWMgYWJzdHJhY3Qgb25Gb3JtUGFzc2VkVmFsaWRhdGlvbihmb3JtOiBIVE1MRm9ybUVsZW1lbnQpOiB2b2lkO1xufVxuXG4vKipcbiAqIFRoZSB2YWxpZGF0aW9uIGhhbmRsZXIgdGhhdCBzaG91bGQgYmUgcmVzcG9uc2libGUgZm9yICpvbmx5KiB1cGRhdGluZyBVSSBlbGVtZW50cywgYmFzZWQgb24gXG4gKiBob3cgYSBmb3JtL2VsZW1lbnQgaXMgdmFsaWRhdGVkLiBObyBpbXBhY3RmdWwgbG9naWMgKHN1Y2ggYXMgcHJldmVudGluZyBhIGZvcm0gZnJvbSBzdWJtaXR0aW5nKSBzaG91bGQgYmUgZG9uZVxuICogd2l0aCBVSSBoYW5kbGVycywgYnV0IGluc3RlYWQgd2l0aCBAc2VlIGBJVmFsaWRhdGlvbkxvZ2ljSGFuZGxlcmBcbiAqL1xuZXhwb3J0IGFic3RyYWN0IGNsYXNzIElWYWxpZGF0aW9uVUlIYW5kbGVyIGV4dGVuZHMgSVZhbGlkYXRpb25IYW5kbGVyQ29tbW9uIHtcbiAgICAvKipcbiAgICAgKiBDYWxsZWQgd2hlbmV2ZXIgYW4gZWxlbWVudCdzIHZhbHVlIGlzIGNoYW5nZWQuXG4gICAgICogXG4gICAgICogSW1hZ2luZSBhbiBlbGVtZW50IGZhaWxzIHRvIHZhbGlkYXRlLCBhbmQgdGhlIFVJIEhhbmRsZXIgc2hvd3MgYSBiaWcgZXJyb3IgYm94IHNvbWV3aGVyZSBhcm91bmQgaXQuXG4gICAgICogXG4gICAgICogVGhlbiBpbWFnaW5lIHRoYXQgeW91IHdhbnQgdGhlIFVJIEhhbmRsZXIgdG8gYmUgYWJsZSB0byBnZXQgcmlkIG9mIHRoYXQgZXJyb3IgYm94IGp1c3QgZnJvbSB0aGUgdXNlclxuICAgICAqIGludGVyYWN0aW5nIHdpdGggdGhlIGVsZW1lbnQgdGhhdCBmYWlsZWQgdmFsaWRhdGlvbiwgaW5zdGVhZCBvZiBoYXZpbmcgdG8gcGVyZm9ybSB2YWxpZGF0aW9uIGFnYWluIChlLmcuIHlvdSBtaWdodFxuICAgICAqIHdhbnQgdG8gc2F2ZSB2YWxpZGF0aW9uIGZvciBvbmx5IHdoZW4gdGhlIHN1Ym1pdCBidXR0b24gaXMgcHJlc3NlZCkuXG4gICAgICogXG4gICAgICogVGhpcyBpcyB0aGUgcHVycG9zZSBvZiB0aGlzIGNhbGxiYWNrLlxuICAgICAqIFxuICAgICAqIEBwYXJhbSBlbGVtZW50IFRoZSBlbGVtZW50IHdobydzIHZhbHVlIGhhcyBjaGFuZ2VkLCBvciBoYXMgYmVlbiBpbnRlcmFjdGVkIHdpdGggaW4gc29tZSB3YXkgdGhlIHVzZXIgY29kZSBkZWVtcyBpbXBvcnRhbnQuXG4gICAgICovXG4gICAgcHVibGljIGFic3RyYWN0IG9uRWxlbWVudFZhbHVlQ2hhbmdlKGVsZW1lbnQ6IEhUTUxFbGVtZW50KTogdm9pZDtcbn1cblxuLyoqXG4gKiBUaGUgdmFsaWRhdGlvbiBoYW5kbGVyIHRoYXQgc2hvdWxkIGJlIHJlc3BvbnNpYmxlIGZvciAqb25seSogaW1wYWN0ZnVsIGxvZ2ljIGJhc2VkIG9mZiBvZiBmYWlsZWQvcGFzc2VkXG4gKiB2YWxpZGF0aW9uIG9mIGZvcm1zIGFuZCBlbGVtZW50cy5cbiAqIFxuICogTm8gVUkgY29kZSBzaG91bGQgYmUgcGxhY2VkIGludG8gdGhpcyBoYW5kbGVyLCBhcyB0aGF0IHNob3VsZCBiZSBkb25lIHdpdGggQHNlZSBgSVZhbGlkYXRpb25VSUhhbmRsZXJgXG4gKiBcbiAqIEBzZWUgYFZhbGlkYXRpb25Mb2dpY0hhbmRsZXJOb1N1Ym1pdGBcbiAqL1xuZXhwb3J0IGFic3RyYWN0IGNsYXNzIElWYWxpZGF0aW9uTG9naWNIYW5kbGVyIGV4dGVuZHMgSVZhbGlkYXRpb25IYW5kbGVyQ29tbW9uIHtcbn1cblxuLyoqXG4gKiBBIGJ1aWx0LWluIGxvZ2ljIGhhbmRsZXIgd2hvJ3Mgb25seSBmdW5jdGlvbiBpcyB0byBwcmV2ZW50IGEgZm9ybSBmcm9tIHN1Ym1pdHRpbmcgaWYgdGhlcmUgYXJlIGFueVxuICogdmFsaWRhdGlvbiBlcnJvcnMuXG4gKiBcbiAqIE5hbWU6IFwibm8tc3VibWl0LWZvcm1cIlxuICovXG5leHBvcnQgY2xhc3MgVmFsaWRhdGlvbkxvZ2ljSGFuZGxlck5vU3VibWl0IGV4dGVuZHMgSVZhbGlkYXRpb25IYW5kbGVyQ29tbW9uIHtcbiAgICBwdWJsaWMgb25FbGVtZW50RmFpbGVkVmFsaWRhdGlvbihlbGVtZW50OiBIVE1MRWxlbWVudCwgcmVhc29uczogc3RyaW5nW10pOiB2b2lkIHt9ICAgIFxuICAgIHB1YmxpYyBvbkVsZW1lbnRQYXNzZWRWYWxpZGF0aW9uKGVsZW1lbnQ6IEhUTUxFbGVtZW50KTogdm9pZCB7fVxuICAgIHB1YmxpYyBvbkZvcm1QYXNzZWRWYWxpZGF0aW9uKGZvcm06IEhUTUxGb3JtRWxlbWVudCk6IHZvaWQge31cblxuICAgIHB1YmxpYyBvbkZvcm1GYWlsZWRWYWxpZGF0aW9uKGZvcm06IEhUTUxGb3JtRWxlbWVudCwgcmVhc29uc1BlckVsZW1lbnQ6IE1hcDxIVE1MRWxlbWVudCwgc3RyaW5nW10+LCBmb3JtU3VibWl0RXZlbnQ6IEV2ZW50IHwgbnVsbCk6IHZvaWQge1xuICAgICAgICBpZihmb3JtU3VibWl0RXZlbnQpXG4gICAgICAgICAgICBmb3JtU3VibWl0RXZlbnQucHJldmVudERlZmF1bHQoKTtcbiAgICB9XG59XG5cbi8qKlxuICogQSBzZWxlY3RvciBpcyByZXNwb25zaWJsZSBmb3Igc2VsZWN0aW5nIGFsbCBvZiB0aGUgZWxlbWVudHMgaW4gYSBmb3JtIHRvIHBlcmZvcm0gdmFsaWRhdGlvbiBvbi5cbiAqIFxuICogSW5zdGVhZCBvZiBoYXJkY29kaW5nIHdoaWNoIGVsZW1lbnRzIHRvIGluY2x1ZGUgKDxpbnB1dD4sIDxzZWxlY3Q+LCBldGMuKSBhbmQganVzdCAqYXNzdW1pbmcqIHRoYXQnbGwgYmUgZmluZSxcbiAqIHRoaXMgbGlicmFyeSB3aWxsIGluc3RlYWQgYWxsb3cgZWFjaCBwb2xpY3kgdG8gc3BlY2lmeSBhIHNlbGVjdG9yIHRvIHBlcmZvcm0gdGhpcyBmdW5jdGlvbi5cbiAqIFxuICogVGhpcyBhbGxvd3MgZnVsbCBjdXN0b21pc2F0aW9uIG9mIHdoaWNoIGVsZW1lbnRzIGFyZSB0byBiZSBpbmNsdWRlZCBpbiB2YWxpZGF0aW9uIG9mIGEgZm9ybSBzbyBjZXJ0YWluXG4gKiBzZXR1cHMgb3IgZWRnZSBjYXNlcyBjYW4gc3RpbGwgYmUgc3VwcG9ydGVkIHdpdGggYSBsaXR0bGUgZXh0cmEgZWZmb3J0IG9uIHRoZSB1c2VyLlxuICogXG4gKiBAc2VlIGBWYWxpZGF0aW9uRm9ybUVsZW1lbnRTZWxlY3RvckRlZmF1bHRgXG4gKi9cbmV4cG9ydCBhYnN0cmFjdCBjbGFzcyBJVmFsaWRhdGlvbkZvcm1FbGVtZW50U2VsZWN0b3Ige1xuICAgIC8qKlxuICAgICAqIENhbGxlZCB3aGVuIGEgdmFsaWRhdGlvbiBmdW5jdGlvbiBuZWVkcyB0byBrbm93IHdoaWNoIGVsZW1lbnRzIGluc2lkZSBhIGZvcm0gaXQgc2hvdWxkXG4gICAgICogaW5jbHVkZSBpbiB2YWxpZGF0aW9uLlxuICAgICAqIFxuICAgICAqIEBwYXJhbSBmb3JtIFRoZSBmb3JtIHRvIGdldCBlbGVtZW50cyBmcm9tLlxuICAgICAqIEByZXR1cm5zIEFuIGFycmF5IG9mIGBIVE1MRWxlbWVudGBzIHRvIGJlIGluY2x1ZGVkIGluIHZhbGlkYXRpb24uXG4gICAgICovXG4gICAgcHVibGljIGFic3RyYWN0IG9uRmluZEVsZW1lbnRzVG9WYWxpZGF0ZShmb3JtOiBIVE1MRm9ybUVsZW1lbnQpOiBIVE1MRWxlbWVudFtdO1xufVxuXG4vKipcbiAqIFRoZSBkZWZhdWx0IHNlbGVjdG9yIHRoYXQgc2hvdWxkIHdvcmsgZm9yIG1vc3QgYmFzaWMgZm9ybXMuXG4gKiBcbiAqIEluY2x1ZGVzOlxuICogICogQWxsIDxpbnB1dD4gdGFnc1xuICogICogQWxsIDxzZWxlY3Q+IHRhZ3NcbiAqICAqIEFsbCA8dGV4dGFyZWE+IHRhZ3NcbiAqIFxuICogTmFtZTogXCJkZWZhdWx0XCJcbiAqL1xuZXhwb3J0IGNsYXNzIFZhbGlkYXRpb25Gb3JtRWxlbWVudFNlbGVjdG9yRGVmYXVsdCBleHRlbmRzIElWYWxpZGF0aW9uRm9ybUVsZW1lbnRTZWxlY3RvciB7XG4gICAgcHVibGljIG9uRmluZEVsZW1lbnRzVG9WYWxpZGF0ZShmb3JtOiBIVE1MRm9ybUVsZW1lbnQpOiBIVE1MRWxlbWVudFtdIHtcbiAgICAgICAgY29uc3QgZWxlbWVudHM6IEhUTUxFbGVtZW50W10gPSBbXTtcblxuICAgICAgICBmb3JtLnF1ZXJ5U2VsZWN0b3JBbGwoXCJpbnB1dFwiKS5mb3JFYWNoKGUgPT4gZWxlbWVudHMucHVzaChlKSk7XG4gICAgICAgIGZvcm0ucXVlcnlTZWxlY3RvckFsbChcInNlbGVjdFwiKS5mb3JFYWNoKGUgPT4gZWxlbWVudHMucHVzaChlKSk7XG4gICAgICAgIGZvcm0ucXVlcnlTZWxlY3RvckFsbChcInRleHRhcmVhXCIpLmZvckVhY2goZSA9PiBlbGVtZW50cy5wdXNoKGUpKTtcblxuICAgICAgICByZXR1cm4gZWxlbWVudHM7XG4gICAgfVxufVxuXG4vKipcbiAqIEEgcnVsZSB0aGF0IHBlcmZvcm1zIHZhbGlkYXRpb24gb24gYW4gZWxlbWVudC5cbiAqIFxuICogUnVsZXMgY2FuIGJlIGdpdmVuIHBhcmFtZXRlcnMgYnkgdXNlciBjb2RlLlxuICogXG4gKiBAc2VlIGBWYWxpZGF0aW9uUnVsZUlzTm90RW1wdHlgXG4gKi9cbmV4cG9ydCBhYnN0cmFjdCBjbGFzcyBWYWxpZGF0aW9uUnVsZSB7XG4gICAgLyoqXG4gICAgICogQXR0ZW1wdHMgdG8gdmFsaWRhdGUgYW4gZWxlbWVudC5cbiAgICAgKiBcbiAgICAgKiBOb3RlczpcbiAgICAgKiAgVGhlIHJldHVybmVkIG9iamVjdCBoYXMgYSBgcmVhc29uc2AgcHJvcGVydHkgZGV0YWlsaW5nIHRoZSByZWFzb25zIHdoeVxuICAgICAqICB2YWxpZGF0aW9uIGZhaWxlZC4gVGhpcyBwcm9wZXJ0eSBpcyAqKm9ubHkqKiB1c2VkIG9uIGEgKmZhaWxlZCogdmFsaWRhdGlvblxuICAgICAqICBhbmQgaXMgY29tcGxldGVseSBpZ25vcmVkIG9uIGEgKnBhc3NpbmcqIHZhbGlkYXRpb24uIFxuICAgICAqIFxuICAgICAqICBUaGlzIG1lYW5zIHRoZSBpbXBsZW1lbnRpbmcgY29kZSBkb2VzIG5vdCBuZWVkIHRvIGdvIG91dCBvZiB0aGVpciB3YXkgdG8gYXZvaWQgcG9wdWxhdGluZyBcbiAgICAgKiAgdGhpcyBwcm9wZXJ0eSBpbiB0aGUgZXZlbnQgb2YgYSBzdWNjZXNzZnVsIHZhbGlkYXRpb24uXG4gICAgICogXG4gICAgICogQHBhcmFtIGVsZW1lbnQgVGhlIGVsZW1lbnQgdG8gdmFsaWRhdGUuXG4gICAgICogQHBhcmFtIHBhcmFtcyAgVGhlIHBhcmFtZXRlcnMgcGFzc2VkIGJ5IHRoZSB1c2VyIGNvZGUuXG4gICAgICogXG4gICAgICogQHJldHVybnMgQSBgVmFsaWRhdGlvblJlc3VsdGAgb2JqZWN0IGNvbnRhaW5pbmcgaW5mb3JtYXRpb24gYWJvdXQgdGhlIHZhbGlkYXRpb24uXG4gICAgICovXG4gICAgcHVibGljIGFic3RyYWN0IG9uVmFsaWRhdGVFbGVtZW50KGVsZW1lbnQ6IEhUTUxFbGVtZW50LCBwYXJhbXM6IHN0cmluZ1tdKTogVmFsaWRhdGlvblJlc3VsdDtcblxuICAgIC8qKlxuICAgICAqIEEgaGVscGVyIGZ1bmN0aW9uIHRoYXQgdGhyb3dzIGFuIGBFcnJvcmAgd2l0aCBhIHN0YW5kYXJkaXNlZCBlcnJvciBtZXNzYWdlLlxuICAgICAqIFxuICAgICAqIFRoaXMgc3BlY2lmaWMgZXJyb3IgZnVuY3Rpb24gaXMgdXNlZCB3aGVuIHRoZXJlIGlzIGEgbWlzLW1hdGNoIGJldHdlZW4gdGhlIGFtb3VudFxuICAgICAqIG9mIHBhcmFtZXRlcnMgZXhwZWN0ZWQgZnJvbSB0aGUgdXNlciwgYW5kIHRoZSBhbW91bnQgYWN0dWFsbHkgZ2l2ZW4uXG4gICAgICogXG4gICAgICogQHBhcmFtIGV4cGVjdGVkIFRoZSBhbW91bnQgb2YgcGFyYW1ldGVycyBleHBlY3RlZC5cbiAgICAgKiBAcGFyYW0gZ290ICAgICAgVGhlIGFtb3VudCBvZiBwYXJhbWV0ZXJzIGdpdmVuLlxuICAgICAqL1xuICAgIHByb3RlY3RlZCB0aHJvd1BhcmFtQ291bnRNaXNtYXRjaChleHBlY3RlZDogbnVtYmVyLCBnb3Q6IG51bWJlcik6IHZvaWQge1xuICAgICAgICB0aHJvdyBuZXcgRXJyb3IoXCJQYXJhbSBjb3VudCBtaXN0bWF0Y2guIEV4cGVjdGVkIFwiK2V4cGVjdGVkK1wiIHBhcmFtZXRlcnMsIGJ1dCBnb3QgXCIrZ290K1wiIGluc3RlYWQuXCIpO1xuICAgIH1cblxuICAgIC8qKlxuICAgICAqIEEgaGVscGVyIGZ1bmN0aW9uIHRvIGdldCB0aGUgdmFsdWUgZnJvbSB0aGUgZ2l2ZW4gZWxlbWVudC5cbiAgICAgKiBcbiAgICAgKiBBbGdvcml0aG06XG4gICAgICogIFRoZSBlbGVtZW50J3MgXCJzdHJpbmdcIiB2YWx1ZSBpcyB0YWtlbiBmcm9tIGl0cyBgLnZhbHVlYCBwcm9wZXJ0eS5cbiAgICAgKiAgICAgIElmIGAudmFsdWVgIGRvZXNuJ3QgZXhpc3QsIHRoZW4gYW4gZXJyb3IgaXMgdGhyb3duLlxuICAgICAqICBJZiB0aGUgZWxlbWVudCBpcyBhbiBgPGlucHV0IHR5cGU9XCJudW1iZXJcIj5gIHRoZW4gdGhlIFwic3RyaW5nXCIgdmFsdWUgaXMgY2FzdGVkIHRvIGEgbnVtYmVyLlxuICAgICAqICAgICAgSWYgdGhlIGNhc3QgZmFpbHMgdGhlbiBhbiBlcnJvciBpcyB0aHJvd24uXG4gICAgICogICAgICBUaGF0IG51bWJlciBpcyB0aGVuIHJldHVybmVkIGFzIHRoZSBmaW5hbCB2YWx1ZS5cbiAgICAgKiAgT3RoZXJ3aXNlLCB0aGUgXCJzdHJpbmdcIiB2YWx1ZSBpcyByZXR1cm5lZCBhcyB0aGUgZmluYWwgdmFsdWUuIFxuICAgICAqIFxuICAgICAqIEBwYXJhbSBlbGVtZW50IFRoZSBlbGVtZW50IHRvIGdldCB0aGUgdmFsdWUgZm9yLlxuICAgICAqIFxuICAgICAqIEByZXR1cm5zIFRoZSBgZWxlbWVudGAncyB2YWx1ZSBhcyBlaXRoZXIgYSBzdHJpbmcgb3IgbnVtYmVyLlxuICAgICAqL1xuICAgIHByb3RlY3RlZCBnZXRWYWx1ZUZyb21FbGVtZW50KGVsZW1lbnQ6IEhUTUxFbGVtZW50KTogc3RyaW5nIHwgbnVtYmVyIHtcbiAgICAgICAgY29uc3QgZWxlbWVudEFueTogYW55ID0gZWxlbWVudDtcbiAgICAgICAgaWYoZWxlbWVudEFueS52YWx1ZSA9PT0gdW5kZWZpbmVkKVxuICAgICAgICAgICAgdGhyb3cgbmV3IEVycm9yKFwiRWxlbWVudCBkb2Vzbid0IGNvbnRhaW4gYSAnLnZhbHVlJyBmaWVsZC4gVGhpcyBydWxlIGNhbm5vdCBiZSB1c2VkLlwiKTtcblxuICAgICAgICBsZXQgdmFsdWUgPSBlbGVtZW50QW55LnZhbHVlO1xuICAgICAgICBpZihlbGVtZW50IGluc3RhbmNlb2YgSFRNTElucHV0RWxlbWVudCAmJiBlbGVtZW50LnR5cGUgPT09IFwibnVtYmVyXCIpIHtcbiAgICAgICAgICAgIHZhbHVlID0gTnVtYmVyKHZhbHVlKTtcbiAgICAgICAgICAgIGlmKHZhbHVlID09PSBOYU4pXG4gICAgICAgICAgICAgICAgdGhyb3cgbmV3IEVycm9yKFwiRWxlbWVudCBpcyBhbiA8aW5wdXQ+IG9mIHR5cGUgJ251bWJlcicsIHlldCBpdCdzIHZhbHVlIGNhbm5vdCBiZSBjb252ZXJ0ZWQgdG8gYSBudW1iZXIuIEVsZW1lbnQ6IFwiK2VsZW1lbnQpO1xuICAgICAgICB9XG5cbiAgICAgICAgcmV0dXJuIHZhbHVlO1xuICAgIH1cbn1cblxuLyoqXG4gKiBBIHZhbGlkYXRpb24gcnVsZSB0aGF0IG9ubHkgcGFzc2VzIGlmOlxuICogICogVGhlIGdpdmVuIGBlbGVtZW50YCBoYXMgYSBgLnZhbHVlYCBwcm9wZXJ0eS5cbiAqICAqIFRoZSBgLnZhbHVlYCBwcm9wZXJ0eSBpcyBub3QgbnVsbC5cbiAqICAqIFRoZSBgLnZhbHVlYCBwcm9wZXJ0eSBpcyBhIHN0cmluZy5cbiAqICAqIFRoZSBgLnZhbHVlYCBwcm9wZXJ0eSBpcyBub3QgbWFkZSB1cCBlbnRpcmVseSBvZiB3aGl0ZXNwYWNlLCBhbmQgaGFzIGEgbGVuZ3RoIGdyZWF0ZXIgdGhhbiAwLlxuICogXG4gKiBSdWxlc2V0OiBcIm5vdC1lbXB0eVwiXG4gKi9cbmV4cG9ydCBjbGFzcyBWYWxpZGF0aW9uUnVsZUlzTm90RW1wdHkgZXh0ZW5kcyBWYWxpZGF0aW9uUnVsZSB7XG4gICAgcHVibGljIG9uVmFsaWRhdGVFbGVtZW50KGVsZW1lbnQ6IEhUTUxFbGVtZW50LCBwYXJhbXM6IHN0cmluZ1tdKTogVmFsaWRhdGlvblJlc3VsdCB7XG4gICAgICAgIGlmKHBhcmFtcy5sZW5ndGggPiAwKVxuICAgICAgICAgICAgc3VwZXIudGhyb3dQYXJhbUNvdW50TWlzbWF0Y2goMCwgcGFyYW1zLmxlbmd0aCk7XG5cbiAgICAgICAgY29uc3QgZWxlbWVudEFueTogYW55ID0gZWxlbWVudDtcbiAgICAgICAgaWYoZWxlbWVudEFueS52YWx1ZSA9PT0gdW5kZWZpbmVkKVxuICAgICAgICAgICAgcmV0dXJuIG5ldyBWYWxpZGF0aW9uUmVzdWx0KGZhbHNlLCBbXCJFbGVtZW50IGRvZXNuJ3QgY29udGFpbiBhICcudmFsdWUnIGZpZWxkLiBUaGlzIHJ1bGUgY2Fubm90IGJlIHVzZWQuXCJdKTtcbiAgICAgICAgXG4gICAgICAgIGNvbnN0IHZhbHVlID0gZWxlbWVudEFueS52YWx1ZTtcbiAgICAgICAgaWYodmFsdWUgPT09IG51bGwpXG4gICAgICAgICAgICByZXR1cm4gbmV3IFZhbGlkYXRpb25SZXN1bHQoZmFsc2UsIFtcIlZhbHVlIGNhbm5vdCBiZSBudWxsLlwiXSk7XG5cbiAgICAgICAgaWYodHlwZW9mIHZhbHVlID09PSBcInN0cmluZ1wiKVxuICAgICAgICAgICAgcmV0dXJuIG5ldyBWYWxpZGF0aW9uUmVzdWx0KHZhbHVlLnRyaW0oKS5sZW5ndGggPiAwLCBbXCJWYWx1ZSBjYW5ub3QgYmUgZW1wdHkgb3IgZnVsbCBvZiB3aGl0ZXNwYWNlLlwiXSk7XG4gICAgICAgIGVsc2VcbiAgICAgICAgICAgIHJldHVybiBuZXcgVmFsaWRhdGlvblJlc3VsdChmYWxzZSwgW1wiVmFsdWUgdHlwZSAnXCIrdHlwZW9mIHZhbHVlK1wiJyBpcyBub3Qgc3VwcG9ydGVkIGJ5IHRoaXMgcnVsZS5cIl0pO1xuICAgIH1cbn1cblxuLyoqXG4gKiBBIHZhbGlkYXRpb24gcnVsZSB0aGF0IG9ubHkgcGFzc2VzIGlmOlxuICogICogVGhlIGdpdmVuIGBlbGVtZW50YCBoYXMgYSBgLnZhbHVlYCBwcm9wZXJ0eS5cbiAqICAqIFRoZSBgLnZhbHVlYCBwcm9wZXJ0eSBpcyBhIHN0cmluZy5cbiAqICAqIFRoZSBgLnZhbHVlYCBwcm9wZXJ0eSBtYXRjaGVzIHRoZSBnaXZlbiByZWdleCAocGFyYW0gMCkuXG4gKiBcbiAqIFJ1bGVzZXQ6IFwicmVnZXg6IHtwYXR0ZXJufVwiXG4gKiBFeGFtcGxlOiBcInJlZ2V4OiAoXFx3K1xcLlxcdyspQChcXHcrXFwuXFx3KylcIiBmb3IgYSBiYXNpYyAoYnV0IGJhZCkgZW1haWwgcmVnZXguXG4gKiBSZW1pbmRlcjogQ2hhcmFjdGVycyAnJicgYW5kICc7JyBtdXN0IGJlIGVzY2FwZWQgd2hlbiB1c2VkIGluc2lkZSBvZiBwYXJhbWV0ZXJzLCBhcyB0aGV5IGhhdmUgc3BlY2lhbCBtZWFuaW5nLlxuICovXG5leHBvcnQgY2xhc3MgVmFsaWRhdGlvblJ1bGVSZWdleCBleHRlbmRzIFZhbGlkYXRpb25SdWxlIHtcbiAgICBwdWJsaWMgb25WYWxpZGF0ZUVsZW1lbnQoZWxlbWVudDogSFRNTEVsZW1lbnQsIHBhcmFtczogc3RyaW5nW10pOiBWYWxpZGF0aW9uUmVzdWx0IHtcbiAgICAgICAgaWYocGFyYW1zLmxlbmd0aCAhPT0gMSlcbiAgICAgICAgICAgIHN1cGVyLnRocm93UGFyYW1Db3VudE1pc21hdGNoKDEsIHBhcmFtcy5sZW5ndGgpO1xuXG4gICAgICAgIGNvbnN0IHZhbHVlID0gc3VwZXIuZ2V0VmFsdWVGcm9tRWxlbWVudChlbGVtZW50KTtcbiAgICAgICAgaWYodHlwZW9mIHZhbHVlID09PSBcInN0cmluZ1wiKSB7XG4gICAgICAgICAgICBjb25zdCByZXN1bHQgPSAobmV3IFJlZ0V4cChwYXJhbXNbMF0pKS50ZXN0KHZhbHVlKTtcbiAgICAgICAgICAgIHJldHVybiBuZXcgVmFsaWRhdGlvblJlc3VsdChyZXN1bHQsIFtcIlZhbHVlIGRvZXMgbm90IG1hdGNoIHBhdHRlcm46IFwiK3BhcmFtc1swXV0pO1xuICAgICAgICB9XG4gICAgICAgIGVsc2VcbiAgICAgICAgICAgIHJldHVybiBuZXcgVmFsaWRhdGlvblJlc3VsdChmYWxzZSwgW1wiVmFsdWUgdHlwZSAnXCIrdHlwZW9mIHZhbHVlK1wiJyBpcyBub3Qgc3VwcG9ydGVkIGJ5IHRoaXMgcnVsZS5cIl0pO1xuICAgIH1cbn1cblxuLyoqXG4gKiAoTWluKVxuICogQSB2YWxpZGF0aW9uIHJ1bGUgdGhhdCBvbmx5IHBhc3NlcyBpZjpcbiAqICAqIFRoZSBnaXZlbiBgZWxlbWVudGAgaGFzIGEgYC52YWx1ZWAgcHJvcGVydHkuXG4gKiAgKiBUaGUgYC52YWx1ZWAgcHJvcGVydHkgaXMgYSBzdHJpbmcgb3IgbnVtYmVyLlxuICogICogSWYgYC52YWx1ZWAgaXMgYSBzdHJpbmcsIHRoZW4gaXQncyBsZW5ndGggbXVzdCBiZSA+PSB0aGUgd2FudGVkIG1pbiBsZW5ndGggKHBhcmFtIDApLlxuICogICogSWYgYC52YWx1ZWAgaXMgYSBudW1iZXIsIHRoZW4gaXQncyB2YWx1ZSBtdXN0IGJlID49IHRoZSB3YW50ZWQgbWluIHZhbHVlIChwYXJhbSAwKS5cbiAqIFxuICogUnVsZXNldDogXCJtaW46IHthbW91bnR9XCJcbiAqIEV4YW1wbGU6IFwibWluOiAyMFwiXG4gKiBcbiAqIChNYXgpXG4gKiBBIHZhbGlkYXRpb24gcnVsZSB0aGF0IG9ubHkgcGFzc2VzIGlmOlxuICogICogVGhlIGdpdmVuIGBlbGVtZW50YCBoYXMgYSBgLnZhbHVlYCBwcm9wZXJ0eS5cbiAqICAqIFRoZSBgLnZhbHVlYCBwcm9wZXJ0eSBpcyBhIHN0cmluZyBvciBudW1iZXIuXG4gKiAgKiBJZiBgLnZhbHVlYCBpcyBhIHN0cmluZywgdGhlbiBpdCdzIGxlbmd0aCBtdXN0IGJlIDw9IHRoZSB3YW50ZWQgbWF4IGxlbmd0aCAocGFyYW0gMCkuXG4gKiAgKiBJZiBgLnZhbHVlYCBpcyBhIG51bWJlciwgdGhlbiBpdCdzIHZhbHVlIG11c3QgYmUgPD0gdGhlIHdhbnRlZCBtYXggdmFsdWUgKHBhcmFtIDApLlxuICogXG4gKiBSdWxlc2V0OiBcIm1heDoge2Ftb3VudH1cIlxuICogRXhhbXBsZTogXCJtYXg6IDIwXCJcbiAqL1xuZXhwb3J0IGNsYXNzIFZhbGlkYXRpb25SdWxlTWluTWF4IGV4dGVuZHMgVmFsaWRhdGlvblJ1bGUge1xuICAgIGNvbnN0cnVjdG9yKHByaXZhdGUgX2lzTWluOiBib29sZWFuKSB7IC8vIHRydWUgPSA+PS4gZmFsc2UgPSA8PVxuICAgICAgICBzdXBlcigpO1xuICAgIH1cblxuICAgIHB1YmxpYyBvblZhbGlkYXRlRWxlbWVudChlbGVtZW50OiBIVE1MRWxlbWVudCwgcGFyYW1zOiBzdHJpbmdbXSk6IFZhbGlkYXRpb25SZXN1bHQge1xuICAgICAgICBpZihwYXJhbXMubGVuZ3RoICE9PSAxKVxuICAgICAgICAgICAgc3VwZXIudGhyb3dQYXJhbUNvdW50TWlzbWF0Y2goMSwgcGFyYW1zLmxlbmd0aCk7XG5cbiAgICAgICAgY29uc3QgcGFyYW0gPSBOdW1iZXIocGFyYW1zWzBdKTtcbiAgICAgICAgaWYocGFyYW0gPT09IE5hTilcbiAgICAgICAgICAgIHRocm93IG5ldyBFcnJvcihcIlBhcmFtZXRlciAnXCIrcGFyYW1zWzBdK1wiJyBpcyBub3QgYSB2YWxpZCBudW1iZXIuXCIpO1xuXG4gICAgICAgIGNvbnN0IHZhbHVlID0gc3VwZXIuZ2V0VmFsdWVGcm9tRWxlbWVudChlbGVtZW50KTtcbiAgICAgICAgaWYodHlwZW9mIHZhbHVlID09PSBcInN0cmluZ1wiKSB7XG4gICAgICAgICAgICByZXR1cm4gKHRoaXMuX2lzTWluKVxuICAgICAgICAgICAgICAgICAgICA/IG5ldyBWYWxpZGF0aW9uUmVzdWx0KHZhbHVlLmxlbmd0aCA+PSBwYXJhbSwgW1wiVmFsdWUgbXVzdCBiZSBhdCBsZWFzdCBcIitwYXJhbStcIiBjaGFyYWN0ZXJzIHdpZGUuXCJdKVxuICAgICAgICAgICAgICAgICAgICA6IG5ldyBWYWxpZGF0aW9uUmVzdWx0KHZhbHVlLmxlbmd0aCA8PSBwYXJhbSwgW1wiVmFsdWUgY2Fubm90IGJlIG1vcmUgdGhhbiBcIitwYXJhbStcIiBjaGFyYWN0ZXJzIHdpZGUuXCJdKTtcbiAgICAgICAgfVxuICAgICAgICBlbHNlIGlmKHR5cGVvZiB2YWx1ZSA9PT0gXCJudW1iZXJcIikge1xuICAgICAgICAgICAgcmV0dXJuICh0aGlzLl9pc01pbilcbiAgICAgICAgICAgICAgICAgICAgPyBuZXcgVmFsaWRhdGlvblJlc3VsdCh2YWx1ZSA+PSBwYXJhbSwgW1wiVmFsdWUgbXVzdCBiZSBncmVhdGVyIHRoYW4gb3IgZXF1YWwgdG8gXCIrcGFyYW1dKVxuICAgICAgICAgICAgICAgICAgICA6IG5ldyBWYWxpZGF0aW9uUmVzdWx0KHZhbHVlIDw9IHBhcmFtLCBbXCJWYWx1ZSBtdXN0IGJlIGxlc3MgdGhhbiBvciBlcXVhbCB0byBcIitwYXJhbV0pO1xuICAgICAgICB9XG4gICAgICAgIGVsc2VcbiAgICAgICAgICAgIHJldHVybiBuZXcgVmFsaWRhdGlvblJlc3VsdChmYWxzZSwgW1wiVmFsdWUgdHlwZSAnXCIrdHlwZW9mIHZhbHVlK1wiJyBpcyBub3Qgc3VwcG9ydGVkIGJ5IHRoaXMgcnVsZS5cIl0pO1xuICAgIH1cbn1cblxuLyoqXG4gKiBDb250YWlucyB0aGUgcmVzdWx0IG9mIGEgdmFsaWRhdGlvbiBhdHRlbXB0LlxuICovXG5leHBvcnQgY2xhc3MgVmFsaWRhdGlvblJlc3VsdCB7XG4gICAgLyoqXG4gICAgICogQHBhcmFtIHBhc3NlZCAgVHJ1ZSBpZiB0aGUgdmFsaWRhdGlvbiBwYXNzZWQuIEZhbHNlIG90aGVyd2lzZS5cbiAgICAgKiBAcGFyYW0gcmVhc29ucyBUaGUgcmVhc29ucyB2YWxpZGF0aW9uIGZhaWxlZC4gVGhpcyBwcm9wZXJ0eSBpcyBjb21wbGV0ZWx5IGlnbm9yZWQgb24gYSBzdWNjZXNzZnVsIHZhbGlkYXRpb24uXG4gICAgICovXG4gICAgcHVibGljIGNvbnN0cnVjdG9yKHB1YmxpYyBwYXNzZWQ6IGJvb2xlYW4sIHB1YmxpYyByZWFzb25zOiBzdHJpbmdbXSkge1xuICAgIH1cbn1cblxuLyoqXG4gKiBBIHBvbGljeSBpcyBhIHNldCBjb25zaXN0aW5nIG9mOlxuICogICogQSBVSSBoYW5kbGVyIChgSVZhbGlkYXRpb25VSUhhbmRsZXJgKVxuICogICogQSBsb2dpYyBoYW5kbGVyIChgSVZhbGlkYXRpb25Mb2dpY0hhbmRsZXJgKVxuICogICogQSBzZWxlY3RvciAoYElWYWxpZGF0aW9uRm9ybUVsZW1lbnRTZWxlY3RvcmApXG4gKiBcbiAqIFBvbGljaWVzIGFyZSB1c2VkIHRvIGdyb3VwIHRoZXNlIHZhcmlvdXMgb2JqZWN0cyB1bmRlciBhIGNvbW1vbiBuYW1lIChlLmcuICdib290c3RyYXAtdGhlbWUtb25lJywgJ2ZvdW5kYXRpb24tYWxsb3ctZmFpbGVkLWZvcm1zJywgZXRjLilcbiAqIHRvIGFsbG93IHRoZSB1c2VyIGNvZGUgdG8gZWFzaWx5IHNwZWNpZnkgd2hhdCBzZXRzIG9mIG9iamVjdHMgdGhhdCB0aGVpciB2YWxpZGF0aW9uIHNob3VsZCB1c2UuXG4gKi9cbmV4cG9ydCBjbGFzcyBWYWxpZGF0aW9uUG9saWN5IHtcbiAgICBwdWJsaWMgY29uc3RydWN0b3IoXG4gICAgICAgIHB1YmxpYyB1aUhhbmRsZXI6IElWYWxpZGF0aW9uVUlIYW5kbGVyLCBcbiAgICAgICAgcHVibGljIGxvZ2ljSGFuZGxlcjogSVZhbGlkYXRpb25Mb2dpY0hhbmRsZXIsXG4gICAgICAgIHB1YmxpYyBzZWxlY3RvcjogSVZhbGlkYXRpb25Gb3JtRWxlbWVudFNlbGVjdG9yXG4gICAgKSB7XG4gICAgfVxufVxuXG4vKipcbiAqIFRoZSBtYWluIGNsYXNzIHRoYXQgcHJvdmlkZXMgYWxsIG9mIHRoZSB2YWxpZGF0aW9uIGZhY2lsaXRpZXMgb2YgdGhpcyBsaWJyYXJ5LlxuICogXG4gKiBUaGlzIGNsYXNzIGlzIGEgc3RhdGljIGNsYXNzLCBzbyBhbGwgZnVuY3Rpb25zIGNhbiBiZSBjYWxsZWQgbGlrZSBgVmFsaWRhdGlvbi5zb21lRnVuY2AuXG4gKiBcbiAqIFBsZWFzZSBzZWUgdGhlIHdpa2kgZm9yIHRoaXMgcHJvamVjdCBvbiBnaXRodWIgZm9yIHN0ZXBzIG9uIGhvdyB0byBnZXQgc3RhcnRlZCB1c2luZyB0aGlzIGxpYnJhcnkuXG4gKi9cbmV4cG9ydCBjbGFzcyBWYWxpZGF0aW9uIHtcbiAgICBwcml2YXRlIHN0YXRpYyBfcG9saWNpZXMgICAgICAgICAgICAgICAgICAgICAgICA9IG5ldyBNYXA8c3RyaW5nLCBWYWxpZGF0aW9uUG9saWN5PigpO1xuICAgIHByaXZhdGUgc3RhdGljIF91aUhhbmRsZXJzICAgICAgICAgICAgICAgICAgICAgID0gbmV3IE1hcDxzdHJpbmcsIElWYWxpZGF0aW9uVUlIYW5kbGVyPigpO1xuICAgIHByaXZhdGUgc3RhdGljIF9sb2dpY0hhbmRsZXJzICAgICAgICAgICAgICAgICAgID0gbmV3IE1hcDxzdHJpbmcsIElWYWxpZGF0aW9uTG9naWNIYW5kbGVyPigpO1xuICAgIHByaXZhdGUgc3RhdGljIF9ydWxlcyAgICAgICAgICAgICAgICAgICAgICAgICAgID0gbmV3IE1hcDxzdHJpbmcsIFZhbGlkYXRpb25SdWxlPigpO1xuICAgIHByaXZhdGUgc3RhdGljIF9zZWxlY3RvcnMgICAgICAgICAgICAgICAgICAgICAgID0gbmV3IE1hcDxzdHJpbmcsIElWYWxpZGF0aW9uRm9ybUVsZW1lbnRTZWxlY3Rvcj4oKTtcbiAgICBwcml2YXRlIHN0YXRpYyBfZGVmYXVsdFBvbGljeTogc3RyaW5nIHwgbnVsbCAgICA9IG51bGw7XG5cbiAgICBwcml2YXRlIHN0YXRpYyBhZGRIYW5kbGVySW1wbDxUPihuYW1lOiBzdHJpbmcsIGhhbmRsZXI6IFQsIG1hcDogTWFwPHN0cmluZywgVD4pOiB2b2lkIHtcbiAgICAgICAgaWYobWFwLmhhcyhuYW1lKSlcbiAgICAgICAgICAgIHRocm93IG5ldyBFcnJvcihcIlRoZXJlIGlzIGFscmVhZHkgYSBoYW5kbGVyIGNhbGxlZCAnXCIrbmFtZStcIidcIik7XG5cbiAgICAgICAgaWYoaGFuZGxlciA9PT0gbnVsbCB8fCBoYW5kbGVyID09PSB1bmRlZmluZWQpXG4gICAgICAgICAgICB0aHJvdyBuZXcgRXJyb3IoXCJQYXJhbWV0ZXIgJ2hhbmRsZXInIGlzIGVpdGhlciBudWxsIG9yIHVuZGVmaW5lZC5cIik7XG5cbiAgICAgICAgbWFwLnNldChuYW1lLCBoYW5kbGVyKTtcbiAgICB9XG5cbiAgICBwcml2YXRlIHN0YXRpYyB2YWxpZGF0ZUVsZW1lbnRJbXBsKGVsZW1lbnQ6IEhUTUxFbGVtZW50LCBydWxlc2V0OiBzdHJpbmcsIHBvbGljeTogVmFsaWRhdGlvblBvbGljeSk6IHN0cmluZ1tdIHtcbiAgICAgICAgY29uc3QgcmVhc29uc0ZhaWxlZDogc3RyaW5nW10gPSBbXTtcbiAgICAgICAgY29uc3QgcnVsZXNldFJlc3VsdCA9IHBhcnNlUnVsZXNldChydWxlc2V0KTtcblxuICAgICAgICBmb3IobGV0IHJlc3VsdCBvZiBydWxlc2V0UmVzdWx0KSB7XG4gICAgICAgICAgICBpZighdGhpcy5fcnVsZXMuaGFzKHJlc3VsdC5ydWxlTmFtZSkpXG4gICAgICAgICAgICAgICAgdGhyb3cgbmV3IEVycm9yKFwiVGhlcmUgaXMgbm8gcnVsZSBuYW1lZCAnXCIrcmVzdWx0LnJ1bGVOYW1lK1wiJyB3aGVuIHBhcnNpbmcgcnVsZXNldDogXCIrcnVsZXNldCtcIlxcbkZvciBlbGVtZW50OiBcIitlbGVtZW50KTtcblxuICAgICAgICAgICAgY29uc3QgcnVsZSA9IHRoaXMuX3J1bGVzLmdldChyZXN1bHQucnVsZU5hbWUpITtcbiAgICAgICAgICAgIGNvbnN0IHZhbGlkYXRpb25SZXN1bHQgPSBydWxlLm9uVmFsaWRhdGVFbGVtZW50KGVsZW1lbnQsIHJlc3VsdC5wYXJhbXMpO1xuXG4gICAgICAgICAgICBpZighdmFsaWRhdGlvblJlc3VsdC5wYXNzZWQpIHtcbiAgICAgICAgICAgICAgICBmb3IobGV0IHJlYXNvbiBvZiB2YWxpZGF0aW9uUmVzdWx0LnJlYXNvbnMpXG4gICAgICAgICAgICAgICAgICAgIHJlYXNvbnNGYWlsZWQucHVzaChyZWFzb24pO1xuICAgICAgICAgICAgfVxuICAgICAgICB9XG4gICAgICAgIFxuICAgICAgICByZXR1cm4gcmVhc29uc0ZhaWxlZDtcbiAgICB9XG5cbiAgICBwcml2YXRlIHN0YXRpYyBnZXRQb2xpY3lCeU5hbWUobmFtZTogc3RyaW5nIHwgbnVsbCk6IFZhbGlkYXRpb25Qb2xpY3kge1xuICAgICAgICBpZih0aGlzLl9wb2xpY2llcy5zaXplID09PSAwKVxuICAgICAgICAgICAgdGhyb3cgbmV3IEVycm9yKFwiTm8gcG9saWNpZXMgaGF2ZSBiZWVuIGNyZWF0ZWQuXCIpO1xuXG4gICAgICAgIGxldCBwb2xpY3lUb1VzZTogVmFsaWRhdGlvblBvbGljeSB8IG51bGwgPSBudWxsO1xuICAgICAgICBpZihuYW1lICE9PSBudWxsKVxuICAgICAgICB7XG4gICAgICAgICAgICBpZighdGhpcy5fcG9saWNpZXMuaGFzKG5hbWUpKVxuICAgICAgICAgICAgICAgIHRocm93IG5ldyBFcnJvcihcIlRoZXJlIGlzIG5vIHBvbGljeSBuYW1lZCAnXCIrbmFtZStcIidcIik7XG5cbiAgICAgICAgICAgIHBvbGljeVRvVXNlID0gdGhpcy5fcG9saWNpZXMuZ2V0KG5hbWUpITtcbiAgICAgICAgfVxuICAgICAgICBlbHNlXG4gICAgICAgICAgICBwb2xpY3lUb1VzZSA9IHRoaXMuX3BvbGljaWVzLmdldCh0aGlzLl9kZWZhdWx0UG9saWN5ISkhO1xuXG4gICAgICAgIGlmKHBvbGljeVRvVXNlID09PSBudWxsKVxuICAgICAgICAgICAgdGhyb3cgbmV3IEVycm9yKFwiSW50ZXJuYWwgZXJyb3IgLSBwb2xpY3lUb1VzZSBzaG91bGQgbm90IGJlIG51bGwgaW4gdGhpcyBzaXR1YXRpb24uXCIpO1xuXG4gICAgICAgIHJldHVybiBwb2xpY3lUb1VzZTtcbiAgICB9XG5cbiAgICAvKipcbiAgICAgKiBSZWdpc3RlcnMgYSBVSSBoYW5kbGVyLlxuICAgICAqIFxuICAgICAqIEBwYXJhbSBuYW1lICAgIFRoZSBuYW1lIHRvIGFzc29jaWF0ZSB3aXRoIHRoZSBoYW5kbGVyLlxuICAgICAqIEBwYXJhbSBoYW5kbGVyIFRoZSBoYW5kbGVyIG9iamVjdC5cbiAgICAgKiBcbiAgICAgKiBAc2VlIGBJVmFsaWRhdGlvblVJSGFuZGxlcmAgXG4gICAgICovXG4gICAgcHVibGljIHN0YXRpYyBhZGRVSUhhbmRsZXIobmFtZTogc3RyaW5nLCBoYW5kbGVyOiBJVmFsaWRhdGlvblVJSGFuZGxlcik6IHZvaWQge1xuICAgICAgICB0aGlzLmFkZEhhbmRsZXJJbXBsKG5hbWUsIGhhbmRsZXIsIHRoaXMuX3VpSGFuZGxlcnMpO1xuICAgIH1cblxuICAgIC8qKlxuICAgICAqIFJlZ2lzdGVycyBhIGxvZ2ljIGhhbmRsZXIuXG4gICAgICogXG4gICAgICogQHBhcmFtIG5hbWUgICAgVGhlIG5hbWUgdG8gYXNzb2NpYXRlIHdpdGggdGhlIGhhbmRsZXIuXG4gICAgICogQHBhcmFtIGhhbmRsZXIgVGhlIGhhbmRsZXIgb2JqZWN0LlxuICAgICAqIFxuICAgICAqIEBzZWUgYElWYWxpZGF0aW9uTG9naWNIYW5kbGVyYCBcbiAgICAgKi9cbiAgICBwdWJsaWMgc3RhdGljIGFkZExvZ2ljSGFuZGxlcihuYW1lOiBzdHJpbmcsIGhhbmRsZXI6IElWYWxpZGF0aW9uTG9naWNIYW5kbGVyKTogdm9pZCB7XG4gICAgICAgIHRoaXMuYWRkSGFuZGxlckltcGwobmFtZSwgaGFuZGxlciwgdGhpcy5fbG9naWNIYW5kbGVycyk7XG4gICAgfVxuXG4gICAgLyoqXG4gICAgICogUmVnaXN0ZXJzIGEgc2VsZWN0b3IuXG4gICAgICogXG4gICAgICogQHBhcmFtIG5hbWUgICAgIFRoZSBuYW1lIHRvIGFzc29jaWF0ZSB3aXRoIHRoZSBzZWxlY3Rvci5cbiAgICAgKiBAcGFyYW0gc2VsZWN0b3IgVGhlIHNlbGVjdG9yIG9iamVjdC5cbiAgICAgKiBcbiAgICAgKiBAc2VlIGBJVmFsaWRhdGlvbkZvcm1FbGVtZW50U2VsZWN0b3JgIFxuICAgICAqL1xuICAgIHB1YmxpYyBzdGF0aWMgYWRkU2VsZWN0b3IobmFtZTogc3RyaW5nLCBzZWxlY3RvcjogSVZhbGlkYXRpb25Gb3JtRWxlbWVudFNlbGVjdG9yKTogdm9pZCB7XG4gICAgICAgIHRoaXMuYWRkSGFuZGxlckltcGwobmFtZSwgc2VsZWN0b3IsIHRoaXMuX3NlbGVjdG9ycyk7XG4gICAgfVxuXG4gICAgLyoqXG4gICAgICogUmVnaXN0ZXJzIGEgcnVsZS5cbiAgICAgKiBcbiAgICAgKiBAcGFyYW0gbmFtZSBUaGUgbmFtZSB0byBhc3NvY2lhdGUgd2l0aCB0aGUgcnVsZS4gVGhpcyBuYW1lIGlzIGFsc28gaG93IHJ1bGVzZXRzIG1ha2UgdXNlIG9mIHRoaXMgcnVsZS5cbiAgICAgKiBAcGFyYW0gcnVsZSBUaGUgcnVsZSBvYmplY3QuXG4gICAgICovXG4gICAgcHVibGljIHN0YXRpYyBhZGRSdWxlKG5hbWU6IHN0cmluZywgcnVsZTogVmFsaWRhdGlvblJ1bGUpOiB2b2lkIHtcbiAgICAgICAgaWYodGhpcy5fcnVsZXMuaGFzKG5hbWUpKVxuICAgICAgICAgICAgdGhyb3cgbmV3IEVycm9yKFwiVGhlcmUgaXMgYWxyZWFkeSBhIHJ1bGUgY2FsbGVkICdcIituYW1lK1wiJ1wiKTtcblxuICAgICAgICBpZihydWxlID09PSBudWxsIHx8IHJ1bGUgPT09IHVuZGVmaW5lZClcbiAgICAgICAgICAgIHRocm93IG5ldyBFcnJvcihcIlBhcmFtZXRlciAncnVsZScgaXMgZWl0aGVyIG51bGwgb3IgdW5kZWZpbmVkLlwiKTtcblxuICAgICAgICB0aGlzLl9ydWxlcy5zZXQobmFtZSwgcnVsZSk7XG4gICAgfVxuXG4gICAgLyoqXG4gICAgICogRGVmaW5lcyBhIHBvbGljeS5cbiAgICAgKiBcbiAgICAgKiBOb3RlczpcbiAgICAgKiAgVGhlIGZpcnN0IHBvbGljeSB0byBiZSBkZWZpbmVkIHdpbGwgYmVjb21lIHRoZSAnZGVmYXVsdCcgcG9saWN5IC0gdGhlIHBvbGljeSB0aGF0XG4gICAgICogIHdpbGwgYmUgdXNlZCBpZiB0aGUgdXNlciBjb2RlIGRvZXNuJ3Qgc3BlY2lmeSBhIHNwZWNpZmljIHBvbGljeSB0byB1c2UgZm9yIGNlcnRhaW5cbiAgICAgKiAgZnVuY3Rpb25zLlxuICAgICAqIFxuICAgICAqIEBwYXJhbSBwb2xpY3lOYW1lICAgICAgICBUaGUgbmFtZSB0byBnaXZlIHRoaXMgcG9saWN5LiBcbiAgICAgKiBAcGFyYW0gdWlIYW5kbGVyTmFtZSAgICAgVGhlIG5hbWUgb2YgdGhlIFVJIGhhbmRsZXIgdG8gdXNlLlxuICAgICAqIEBwYXJhbSBsb2dpY0hhbmRsZXJOYW1lICBUaGUgbmFtZSBvZiB0aGUgbG9naWMgaGFuZGxlciB0byB1c2UuXG4gICAgICogQHBhcmFtIHNlbGVjdG9yTmFtZSAgICAgIFRoZSBuYW1lIG9mIHRoZSBzZWxlY3RvciB0byB1c2UuIERlZmF1bHRzIHRvIFwiZGVmYXVsdFwiLCB3aGljaCBpcyBhIGJ1aWx0LWluIHNlbGVjdG9yLlxuICAgICAqL1xuICAgIHB1YmxpYyBzdGF0aWMgZGVmaW5lUG9saWN5KFxuICAgICAgICBwb2xpY3lOYW1lOiBzdHJpbmcsIFxuICAgICAgICB1aUhhbmRsZXJOYW1lOiBzdHJpbmcsIFxuICAgICAgICBsb2dpY0hhbmRsZXJOYW1lOiBzdHJpbmcsXG4gICAgICAgIHNlbGVjdG9yTmFtZTogc3RyaW5nID0gXCJkZWZhdWx0XCJcbiAgICApOiB2b2lkIHtcbiAgICAgICAgaWYodGhpcy5fcG9saWNpZXMuaGFzKHBvbGljeU5hbWUpKVxuICAgICAgICAgICAgdGhyb3cgbmV3IEVycm9yKFwiVGhlcmUgaXMgYWxyZWFkeSBhIHBvbGljeSBjYWxsZWQgJ1wiK3BvbGljeU5hbWUrXCInXCIpO1xuXG4gICAgICAgIGlmKCF0aGlzLl91aUhhbmRsZXJzLmhhcyh1aUhhbmRsZXJOYW1lKSlcbiAgICAgICAgICAgIHRocm93IG5ldyBFcnJvcihcIlRoZXJlIGlzIG5vIFVJIGhhbmRsZXIgY2FsbGVkICdcIit1aUhhbmRsZXJOYW1lK1wiJ1wiKTtcbiAgICAgICAgaWYoIXRoaXMuX2xvZ2ljSGFuZGxlcnMuaGFzKGxvZ2ljSGFuZGxlck5hbWUpKVxuICAgICAgICAgICAgdGhyb3cgbmV3IEVycm9yKFwiVGhlcmUgaXMgbm8gTG9naWMgaGFuZGxlciBjYWxsZWQgJ1wiK2xvZ2ljSGFuZGxlck5hbWUrXCInXCIpO1xuICAgICAgICBpZighdGhpcy5fc2VsZWN0b3JzLmhhcyhzZWxlY3Rvck5hbWUpKVxuICAgICAgICAgICAgdGhyb3cgbmV3IEVycm9yKFwiVGhlcmUgaXMgbm8gU2VsZWN0b3IgY2FsbGVkICdcIitzZWxlY3Rvck5hbWUrXCInXCIpO1xuXG4gICAgICAgIGlmKHRoaXMuX2RlZmF1bHRQb2xpY3kgPT09IG51bGwpXG4gICAgICAgICAgICB0aGlzLl9kZWZhdWx0UG9saWN5ID0gcG9saWN5TmFtZTtcblxuICAgICAgICBjb25zdCB1aSA9IHRoaXMuX3VpSGFuZGxlcnMuZ2V0KHVpSGFuZGxlck5hbWUpITtcbiAgICAgICAgY29uc3QgbG9naWMgPSB0aGlzLl9sb2dpY0hhbmRsZXJzLmdldChsb2dpY0hhbmRsZXJOYW1lKSE7XG4gICAgICAgIGNvbnN0IHNlbGVjdG9yID0gdGhpcy5fc2VsZWN0b3JzLmdldChzZWxlY3Rvck5hbWUpITtcbiAgICAgICAgdGhpcy5fcG9saWNpZXMuc2V0KHBvbGljeU5hbWUsIG5ldyBWYWxpZGF0aW9uUG9saWN5KHVpLCBsb2dpYywgc2VsZWN0b3IpKVxuICAgIH1cblxuICAgIC8qKlxuICAgICAqIFZhbGlkYXRlcyB0aGUgZ2l2ZW4gZWxlbWVudCwgdXNpbmcgdGhlIHNwZWNpZmllZCBydWxlc2V0IGFuZCBwb2xpY3kuXG4gICAgICogXG4gICAgICogT24gc3VjY2VzczpcbiAgICAgKiAgKiBUaGUgYG9uRWxlbWVudFBhc3NlZFZhbGlkYXRpb25gIGZ1bmN0aW9uIGZvciB0aGUgZ2l2ZW4gcG9saWN5J3MgVUkgaGFuZGxlciBpcyBjYWxsZWQuXG4gICAgICogICogVGhlIGBvbkVsZW1lbnRQYXNzZWRWYWxpZGF0aW9uYCBmdW5jdGlvbiBmb3IgdGhlIGdpdmVuIHBvbGljeSdzIGxvZ2ljIGhhbmRsZXIgaXMgY2FsbGVkLlxuICAgICAqICAqIEFuIGVtcHR5IGFycmF5IChsZW5ndGggb2YgMCkgaXMgcmV0dXJuZWQuXG4gICAgICogXG4gICAgICogT24gZmFpbDpcbiAgICAgKiAgKiBUaGUgYG9uRWxlbWVudEZhaWxlZFZhbGlkYXRpb25gIGZ1bmN0aW9uIGZvciB0aGUgZ2l2ZW4gcG9saWN5J3MgVUkgaGFuZGxlciBpcyBjYWxsZWQuXG4gICAgICogICogVGhlIGBvbkVsZW1lbnRGYWlsZWRWYWxpZGF0aW9uYCBmdW5jdGlvbiBmb3IgdGhlIGdpdmVuIHBvbGljeSdzIGxvZ2ljIGhhbmRsZXIgaXMgY2FsbGVkLlxuICAgICAqICAqIEFuIGFycmF5IG9mIGFsbCB2YWxpZGF0aW9uIGVycm9ycyBpcyByZXR1cm5lZC4gVGhpcyBjYW4gc3RpbGwgYmUgYW4gZW1wdHkgYXJyYXkgaWYgdGhlIHJ1bGVzIHVzZWQgYnkgdGhlIHJ1bGVzZXQgZG9uJ3QgcHJvdmlkZSBlcnJvciBtZXNzYWdlcy5cbiAgICAgKiBcbiAgICAgKiBOb3RlczpcbiAgICAgKiAgV2hpbGUgaXQncyB0cnVlIHRoYXQgZmFpbHVyZSBhbmQgc3VjY2VzcyBjYW4gYm90aCByZXN1bHQgaW4gdGhlIHNhbWUgcmV0dXJuIHZhbHVlLCBpdCdzIGJlc3QgdG8ganVzdCBhc3N1bWUgdGhhdCBhblxuICAgICAqICBlbXB0eSBhcnJheSByZXN1bHRlZCBpbiBhIHN1Y2Nlc3NmdWwgdmFsaWRhdGlvbi5cbiAgICAgKiBcbiAgICAgKiAgVGhpcyBpcyBiZWNhdXNlIGEgcnVsZSBzaG91bGQgKiphbHdheXMqKiBwcm92aWRlIGFuIGVycm9yIG1lc3NhZ2UsIGFuZCBpZiBpdCBkb2Vzbid0IHRoZW4gaXQncyBwb29ybHkgbWFkZSBhbmQgc2hvdWxkXG4gICAgICogIGJlIGZpeGVkLlxuICAgICAqIFxuICAgICAqIEBwYXJhbSBlbGVtZW50IFRoZSBlbGVtZW50IHRvIHZhbGlkYXRlLlxuICAgICAqIEBwYXJhbSBydWxlc2V0IFRoZSBydWxlc2V0IHRoYXQgZGVzY3JpYmVzIGhvdyB0byB2YWxpZGF0ZSB0aGUgZ2l2ZW4gZWxlbWVudC5cbiAgICAgKiBAcGFyYW0gcG9saWN5ICBUaGUgbmFtZSBvZiB0aGUgcG9saWN5IHRvIHVzZS4gSWYgYG51bGxgLCB0aGVuIHRoZSBkZWZhdWx0IHBvbGljeSBpcyB1c2VkLlxuICAgICAqIFxuICAgICAqIEByZXR1cm5zIFNlZSAnT24gc3VjY2VzcycgYW5kICdPbiBmYWlsJy5cbiAgICAgKi9cbiAgICBwdWJsaWMgc3RhdGljIHZhbGlkYXRlRWxlbWVudChlbGVtZW50OiBIVE1MRWxlbWVudCwgcnVsZXNldDogc3RyaW5nLCBwb2xpY3k6IHN0cmluZyB8IG51bGwgPSBudWxsKTogc3RyaW5nW10ge1xuICAgICAgICBjb25zdCBwb2xpY3lUb1VzZSA9IHRoaXMuZ2V0UG9saWN5QnlOYW1lKHBvbGljeSk7XG5cbiAgICAgICAgLy8gUGVyZm9ybSB2YWxpZGF0aW9uLlxuICAgICAgICBjb25zdCByZWFzb25zRmFpbGVkID0gdGhpcy52YWxpZGF0ZUVsZW1lbnRJbXBsKGVsZW1lbnQsIHJ1bGVzZXQsIHBvbGljeVRvVXNlKTtcbiAgICAgICAgaWYocmVhc29uc0ZhaWxlZC5sZW5ndGggPT0gMCkgeyAvLyBWYWxpZGF0aW9uIHBhc3NlZFxuICAgICAgICAgICAgcG9saWN5VG9Vc2UudWlIYW5kbGVyLm9uRWxlbWVudFBhc3NlZFZhbGlkYXRpb24oZWxlbWVudCk7XG4gICAgICAgICAgICBwb2xpY3lUb1VzZS5sb2dpY0hhbmRsZXIub25FbGVtZW50UGFzc2VkVmFsaWRhdGlvbihlbGVtZW50KTtcbiAgICAgICAgfVxuICAgICAgICBlbHNlIHtcbiAgICAgICAgICAgIHBvbGljeVRvVXNlLnVpSGFuZGxlci5vbkVsZW1lbnRGYWlsZWRWYWxpZGF0aW9uKGVsZW1lbnQsIHJlYXNvbnNGYWlsZWQpO1xuICAgICAgICAgICAgcG9saWN5VG9Vc2UubG9naWNIYW5kbGVyLm9uRWxlbWVudEZhaWxlZFZhbGlkYXRpb24oZWxlbWVudCwgcmVhc29uc0ZhaWxlZCk7XG4gICAgICAgIH1cblxuICAgICAgICByZXR1cm4gcmVhc29uc0ZhaWxlZDtcbiAgICB9XG5cbiAgICAvKipcbiAgICAgKiBWYWxpZGF0ZXMgYSBmb3JtIGJ5IHZhbGlkYXRpbmcgYWxsIG9mIHRoZSBlbGVtZW50cyBzcGVjaWZpZWQgYnkgdGhlIGdpdmVuIHBvbGljeSdzIHNlbGVjdG9yLlxuICAgICAqIFxuICAgICAqIEVsZW1lbnRzOlxuICAgICAqICBBbGwgZWxlbWVudHMgdG8gYmUgdmFsaWRhdGVkIGFyZSBkZXRlcm1pbmVkIGJ5IGNhbGxpbmcgdGhlIGBvbkZpbmRFbGVtZW50c1RvVmFsaWRhdGVgIGZ1bmN0aW9uIGZvclxuICAgICAqICB0aGUgZ2l2ZW4gcG9saWN5J3Mgc2VsZWN0b3IuIEBzZWUgYElWYWxpZGF0aW9uRm9ybUVsZW1lbnRTZWxlY3RvcmBcbiAgICAgKiBcbiAgICAgKiBWYWxpZGF0aW9uOlxuICAgICAqICBUaGUgcnVsZXNldCBmb3IgZWFjaCBlbGVtZW50IGlzIGRldGVybWluZWQgYnkgbG9va2luZyBmb3IgYGVsZW1lbnQuZGF0YXNldC5ydWxlc2V0YC4gVGhpcyBtZWFuc1xuICAgICAqICB0aGF0IHRoZSB3YXkgdG8gc3BlY2lmeSB0aGUgcnVsZXNldCBmb3IgZWFjaCBlbGVtZW50LCBpcyB0byBkbyBzb21ldGhpbmcgbGlrZSBgPGlucHV0IGRhdGEtcnVsZXNldD0nbm90LWVtcHR5JyAvPmAuXG4gICAgICogXG4gICAgICogIFRoZSBgVmFsaWRhdGlvbi52YWxpZGF0ZUVsZW1lbnRgIGZ1bmN0aW9uIGlzIHRoZW4gY2FsbGVkIG9uIHRoZSBlbGVtZW50LCB1c2luZyBhbGwgdGhlIGluZm9ybWF0aW9uIHRoaXMgZnVuY3Rpb24ga25vd3MgYWJvdXQuXG4gICAgICogXG4gICAgICogIElmIGFuIGVsZW1lbnQgZG9lcyBub3QgaGF2ZSBhIHJ1bGVzZXQsIHRoZW4gaXQgaXMgaWdub3JlZC5cbiAgICAgKiBcbiAgICAgKiBPbiBzdWNjZXNzOlxuICAgICAqICBUaGlzIGhhcHBlbnMgaWYgZXZlcnkgc2luZ2xlIG5vbi1pZ25vcmVkIGVsZW1lbnQgcGFzc2VzIHZhbGlkYXRpb24uXG4gICAgICogICogVGhlIGBvbkZvcm1QYXNzZWRWYWxpZGF0aW9uYCBmdW5jdGlvbiBmb3IgdGhlIGdpdmVuIHBvbGljeSdzIFVJIGhhbmRsZXIgaXMgY2FsbGVkLlxuICAgICAqICAqIFRoZSBgb25Gb3JtUGFzc2VkVmFsaWRhdGlvbmAgZnVuY3Rpb24gZm9yIHRoZSBnaXZlbiBwb2xpY3kncyBsb2dpYyBoYW5kbGVyIGlzIGNhbGxlZC5cbiAgICAgKiAgKiBBbiBlbXB0eSBtYXAgKGAuc2l6ZWAgb2YgMCkgaXMgcmV0dXJuZWQuXG4gICAgICogXG4gICAgICogT24gZmFpbGVkOlxuICAgICAqICBUaGlzIGhhcHBlbnMgaWYgYW55IG5vbi1pZ25vcmVkIGVsZW1lbnQgZmFpbHMgdmFsaWRhdGlvbi5cbiAgICAgKiAgKiBUaGUgYG9uRm9ybUZhaWxlZFZhbGlkYXRpb25gIGZ1bmN0aW9uIGZvciB0aGUgZ2l2ZW4gcG9saWN5J3MgVUkgaGFuZGxlciBpcyBjYWxsZWQuXG4gICAgICogICogVGhlIGBvbkZvcm1GYWlsZWRWYWxpZGF0aW9uYCBmdW5jdGlvbiBmb3IgdGhlIGdpdmVuIHBvbGljeSdzIGxvZ2ljIGhhbmRsZXIgaXMgY2FsbGVkLlxuICAgICAqICAqIEEgbWFwLCB3aGVyZSB0aGUga2V5IGlzIGFuIGVsZW1lbnQgdGhhdCBmYWlsZWQgdmFsaWRhdGlvbiwgYW5kIHRoZSB2YWx1ZSBpcyBhIHN0cmluZyBhcnJheSBjb250YWluaW5nIHRoZSByZWFzb25zICp3aHkqIGl0IGZhaWxlZCwgaXMgcmV0dXJuZWQuXG4gICAgICogXG4gICAgICogTm90ZXM6XG4gICAgICogIEl0IGlzIHVwIHRvIHRoZSBwb2xpY3kncyBsb2dpYyBoYW5kbGVyIHRvIGNhbGwgYHN1Ym1pdEV2ZW50LnByZXZlbnREZWZhdWx0YCB0byBwcmV2ZW50IGEgZm9ybSBmcm9tIHN1Ym1pdHRpbmcgb24gZmFpbGluZyB2YWxpZGF0aW9uLlxuICAgICAqICBcbiAgICAgKiBAcGFyYW0gZm9ybSAgICAgICAgVGhlIGZvcm0gdG8gdmFsaWRhdGUuXG4gICAgICogQHBhcmFtIHN1Ym1pdEV2ZW50IFRoZSBzdWJtaXQgZXZlbnQgZm9yIHRoZSBnaXZlbiBmb3JtLiBUaGlzIGlzIG9ubHkgcGFzc2VkIGlmIHZhbGlkYXRpb24gaXMgYmVpbmcgcGVyZm9ybWVkIGFzIHBhcnQgb2YgdGhlIGBvbnN1Ym1pdGAgb3IgYGFkZEV2ZW50SGFuZGxlcignc3VibWl0JylgIGV2ZW50cyBmb3IgYSBmb3JtLlxuICAgICAqIEBwYXJhbSBwb2xpY3kgICAgICBUaGUgbmFtZSBvZiB0aGUgcG9saWN5IHRvIHVzZSBmb3IgdmFsaWRhdGlvbi4gSWYgYG51bGxgIGlzIHBhc3NlZCB0aGVuIHRoZSBkZWZhdWx0IHBvbGljeSBpcyB1c2VkLlxuICAgICAqIFxuICAgICAqIEByZXR1cm5zIFNlZSBgT24gc3VjY2Vzc2AgYW5kIGBPbiBmYWlsZWRgLlxuICAgICAqL1xuICAgIHB1YmxpYyBzdGF0aWMgdmFsaWRhdGVGb3JtKGZvcm06IEhUTUxGb3JtRWxlbWVudCwgc3VibWl0RXZlbnQ6IEV2ZW50IHwgbnVsbCA9IG51bGwsIHBvbGljeTogc3RyaW5nIHwgbnVsbCA9IG51bGwpOiBNYXA8SFRNTEVsZW1lbnQsIHN0cmluZ1tdPiB7XG4gICAgICAgIGNvbnN0IHBvbGljeVRvVXNlID0gdGhpcy5nZXRQb2xpY3lCeU5hbWUocG9saWN5KTtcbiAgICAgICAgY29uc3QgZWxlbWVudHMgPSBwb2xpY3lUb1VzZS5zZWxlY3Rvci5vbkZpbmRFbGVtZW50c1RvVmFsaWRhdGUoZm9ybSk7XG4gICAgICAgIGNvbnN0IHJlYXNvbnMgPSBuZXcgTWFwPEhUTUxFbGVtZW50LCBzdHJpbmdbXT4oKTtcblxuICAgICAgICBmb3IobGV0IGVsZW1lbnQgb2YgZWxlbWVudHMpIHtcbiAgICAgICAgICAgIGNvbnN0IHJ1bGVzZXQgPSBlbGVtZW50LmRhdGFzZXQucnVsZXNldDtcbiAgICAgICAgICAgIGlmKCFydWxlc2V0KVxuICAgICAgICAgICAgICAgIGNvbnRpbnVlO1xuXG4gICAgICAgICAgICBjb25zdCByZWFzb25zRmFpbGVkID0gdGhpcy52YWxpZGF0ZUVsZW1lbnQoZWxlbWVudCwgcnVsZXNldCwgcG9saWN5KTtcbiAgICAgICAgICAgIGlmKHJlYXNvbnNGYWlsZWQubGVuZ3RoID4gMClcbiAgICAgICAgICAgICAgICByZWFzb25zLnNldChlbGVtZW50LCByZWFzb25zRmFpbGVkKTtcbiAgICAgICAgfVxuXG4gICAgICAgIGlmKHJlYXNvbnMuc2l6ZSA9PT0gMCkge1xuICAgICAgICAgICAgcG9saWN5VG9Vc2UubG9naWNIYW5kbGVyLm9uRm9ybVBhc3NlZFZhbGlkYXRpb24oZm9ybSk7XG4gICAgICAgICAgICBwb2xpY3lUb1VzZS51aUhhbmRsZXIub25Gb3JtUGFzc2VkVmFsaWRhdGlvbihmb3JtKTtcbiAgICAgICAgfVxuICAgICAgICBlbHNlIHtcbiAgICAgICAgICAgIHBvbGljeVRvVXNlLmxvZ2ljSGFuZGxlci5vbkZvcm1GYWlsZWRWYWxpZGF0aW9uKGZvcm0sIHJlYXNvbnMsIHN1Ym1pdEV2ZW50KTtcbiAgICAgICAgICAgIHBvbGljeVRvVXNlLnVpSGFuZGxlci5vbkZvcm1GYWlsZWRWYWxpZGF0aW9uKGZvcm0sIHJlYXNvbnMsIHN1Ym1pdEV2ZW50KTtcbiAgICAgICAgfVxuXG4gICAgICAgIHJldHVybiByZWFzb25zO1xuICAgIH1cblxuICAgIC8qKlxuICAgICAqIEhvb2tzIG9udG8gYSBmb3JtJ3MgXCJzdWJtaXRcIiBldmVudCB0byBwZXJmb3JtIHZhbGlkYXRpb24gd2hlbiB0aGUgZm9ybSBpcyBzdWJtaXR0ZWQuXG4gICAgICogXG4gICAgICogTm90ZXM6XG4gICAgICogIFRoaXMgZnVuY3Rpb24sIHVzaW5nIHRoZSBnaXZlbiBwb2xpY3kncyBzZWxlY3RvciwgYWxzbyBjYWxscyBgVmFsaWRhdGlvbi5ob29rdXBFbGVtZW50YCBvbiBlYWNoXG4gICAgICogIGVsZW1lbnQgdGhhdCBpcyB0byBiZSB2YWxpZGF0ZWQuXG4gICAgICogXG4gICAgICogIEl0IGlzIHVwIHRvIHRoZSBwb2xpY3kncyBsb2dpYyBoYW5kbGVyIHRvIGNhbGwgYHByZXZlbnREZWZhdWx0YCB0byBzdG9wIHRoZSBmb3JtIGZyb20gc3VibWl0dGluZ1xuICAgICAqICBvbiB2YWxpZGF0aW9uIGZhaWx1cmUuXG4gICAgICogXG4gICAgICogQHBhcmFtIGZvcm0gICBUaGUgZm9ybSB0byBob29rIG9udG8uXG4gICAgICogQHBhcmFtIHBvbGljeSBUaGUgbmFtZSBvZiB0aGUgcG9saWN5IHRvIHVzZS4gSWYgYG51bGxgIGlzIHBhc3NlZCB0aGVuIHRoZSBkZWZhdWx0IHBvbGljeSBpcyB1c2VkLlxuICAgICAqL1xuICAgIHB1YmxpYyBzdGF0aWMgaG9va3VwRm9ybShmb3JtOiBIVE1MRm9ybUVsZW1lbnQsIHBvbGljeTogc3RyaW5nIHwgbnVsbCA9IG51bGwpOiB2b2lkIHtcbiAgICAgICAgZm9ybS5hZGRFdmVudExpc3RlbmVyKFwic3VibWl0XCIsIGUgPT4gVmFsaWRhdGlvbi52YWxpZGF0ZUZvcm0oZm9ybSwgZSwgcG9saWN5KSk7XG5cbiAgICAgICAgZm9yKGxldCBlbGVtZW50IG9mIHRoaXMuZ2V0UG9saWN5QnlOYW1lKHBvbGljeSkuc2VsZWN0b3Iub25GaW5kRWxlbWVudHNUb1ZhbGlkYXRlKGZvcm0pKVxuICAgICAgICAgICAgdGhpcy5ob29rdXBFbGVtZW50KGVsZW1lbnQsIHBvbGljeSk7XG4gICAgfVxuICAgIFxuICAgIC8qKlxuICAgICAqIEhvb2tzIG9udG8gdGhlIFwiY2hhbmdlXCIgZXZlbnQgZm9yIHRoZSBnaXZlbiBlbGVtZW50LCB3aGljaCB3aWxsIHRoZW4gY2FsbCB0aGUgYG9uRWxlbWVudFZhbHVlQ2hhbmdlYFxuICAgICAqIGNhbGxiYWNrIGZvciB0aGUgZ2l2ZW4gcG9saWN5J3MgVUkgaGFuZGxlci5cbiAgICAgKiBcbiAgICAgKiBAcGFyYW0gZWxlbWVudCBUaGUgZWxlbWVudCB0byBob29rIG9udG8uXG4gICAgICogQHBhcmFtIHBvbGljeSAgVGhlIG5hbWUgb2YgdGhlIHBvbGljeSB0byB1c2UuIElmIGBudWxsYCBpcyBwYXNzZWQgdGhlbiB0aGUgZGVmYXVsdCBwb2xpY3kgaXMgdXNlZC5cbiAgICAgKi9cbiAgICBwdWJsaWMgc3RhdGljIGhvb2t1cEVsZW1lbnQoZWxlbWVudDogSFRNTEVsZW1lbnQsIHBvbGljeTogc3RyaW5nIHwgbnVsbCA9IG51bGwpOiB2b2lkIHtcbiAgICAgICAgY29uc3QgcG9saWN5VG9Vc2UgPSB0aGlzLmdldFBvbGljeUJ5TmFtZShwb2xpY3kpO1xuICAgICAgICBlbGVtZW50LmFkZEV2ZW50TGlzdGVuZXIoXCJjaGFuZ2VcIiwgZSA9PiBwb2xpY3lUb1VzZS51aUhhbmRsZXIub25FbGVtZW50VmFsdWVDaGFuZ2UoZWxlbWVudCkpO1xuICAgIH1cbn1cblxuLy8gQlVJTFQtSU4gU1RVRkZcblZhbGlkYXRpb24uYWRkUnVsZSAgICAgICAgKFwibm90LWVtcHR5XCIsICAgICAgbmV3IFZhbGlkYXRpb25SdWxlSXNOb3RFbXB0eSgpKTtcblZhbGlkYXRpb24uYWRkUnVsZSAgICAgICAgKFwicmVnZXhcIiwgICAgICAgICAgbmV3IFZhbGlkYXRpb25SdWxlUmVnZXgoKSk7XG5WYWxpZGF0aW9uLmFkZFJ1bGUgICAgICAgIChcIm1pblwiLCAgICAgICAgICAgIG5ldyBWYWxpZGF0aW9uUnVsZU1pbk1heCh0cnVlKSk7XG5WYWxpZGF0aW9uLmFkZFJ1bGUgICAgICAgIChcIm1heFwiLCAgICAgICAgICAgIG5ldyBWYWxpZGF0aW9uUnVsZU1pbk1heChmYWxzZSkpO1xuVmFsaWRhdGlvbi5hZGRMb2dpY0hhbmRsZXIoXCJuby1zdWJtaXQtZm9ybVwiLCBuZXcgVmFsaWRhdGlvbkxvZ2ljSGFuZGxlck5vU3VibWl0KCkpO1xuVmFsaWRhdGlvbi5hZGRTZWxlY3RvciAgICAoXCJkZWZhdWx0XCIsICAgICAgICBuZXcgVmFsaWRhdGlvbkZvcm1FbGVtZW50U2VsZWN0b3JEZWZhdWx0KCkpO1xuXG5jbGFzcyBSdWxlUGFyc2VyUmVzdWx0IHtcbiAgICBwdWJsaWMgY29uc3RydWN0b3IocHVibGljIHJ1bGVOYW1lOiBzdHJpbmcsIHB1YmxpYyBwYXJhbXM6IHN0cmluZ1tdKSB7XG4gICAgfVxufVxuLy8gRXhwb3J0ZWQgc2ltcGx5IHNvIHRoZSB0ZXN0cyBjYW4gaW1wb3J0IGl0LlxuLy8gUnVsZXNldHMgYXJlbid0IGNvbXBsZXggZW5vdWdoIHRvIGJvdGhlciB3aXRoIGEgcHJvcGVyIHBhcnNlciwgc28gd2UgZ2V0IHRoaXMgaGFja2VkIHRvZ2V0aGVyIG9uZS5cbmV4cG9ydCBmdW5jdGlvbiBwYXJzZVJ1bGVzZXQocnVsZXNldDogc3RyaW5nKTogUnVsZVBhcnNlclJlc3VsdFtdIHtcbiAgICBjb25zdCByZXN1bHQ6IFJ1bGVQYXJzZXJSZXN1bHRbXSA9IFtdO1xuXG4gICAgZW51bSBQYXJzZVN0YXRlIHtcbiAgICAgICAgTkFNRSxcbiAgICAgICAgVkFMVUVcbiAgICB9XG4gICAgbGV0IHN0YXRlID0gUGFyc2VTdGF0ZS5OQU1FO1xuICAgIGxldCBpbmRleCA9IDA7XG5cbiAgICB3aGlsZSh0cnVlKSB7XG4gICAgICAgIC8vIFNraXAgd2hpdGVzcGFjZVxuICAgICAgICB3aGlsZShpbmRleCA8IHJ1bGVzZXQubGVuZ3RoICYmIC9cXHMvLnRlc3QocnVsZXNldFtpbmRleF0pKVxuICAgICAgICAgICAgaW5kZXgrKztcblxuICAgICAgICBpZihpbmRleCA+PSBydWxlc2V0Lmxlbmd0aClcbiAgICAgICAgICAgIGJyZWFrO1xuICAgICAgICAgICAgXG4gICAgICAgIGxldCBzdGFydEluZGV4ID0gaW5kZXg7XG4gICAgICAgIHN3aXRjaChzdGF0ZSkge1xuICAgICAgICAgICAgY2FzZSBQYXJzZVN0YXRlLk5BTUU6XG4gICAgICAgICAgICAgICAgd2hpbGUodHJ1ZSkge1xuICAgICAgICAgICAgICAgICAgICBpZihpbmRleCA+PSBydWxlc2V0Lmxlbmd0aCB8fCBydWxlc2V0W2luZGV4XSA9PSAnOicgfHwgcnVsZXNldFtpbmRleF0gPT0gJzsnKVxuICAgICAgICAgICAgICAgICAgICAgICAgYnJlYWs7XG5cbiAgICAgICAgICAgICAgICAgICAgaWYoL1xccy8udGVzdChydWxlc2V0W2luZGV4XSkpXG4gICAgICAgICAgICAgICAgICAgICAgICB0aHJvdyBuZXcgRXJyb3IoXCJVbmV4cGVjdGVkIHdoaXRlc3BhY2UgYXQgaW5kZXggXCIraW5kZXgrXCIgd2hpbGUgcmVhZGluZyBydWxlIG5hbWUuIFJ1bGVzZXQgPSBcIitydWxlc2V0KTtcblxuICAgICAgICAgICAgICAgICAgICBpbmRleCsrO1xuICAgICAgICAgICAgICAgIH1cblxuICAgICAgICAgICAgICAgIHJlc3VsdC5wdXNoKG5ldyBSdWxlUGFyc2VyUmVzdWx0KHJ1bGVzZXQuc3Vic3RyaW5nKHN0YXJ0SW5kZXgsIGluZGV4KSwgW10pKTtcbiAgICAgICAgICAgICAgICBzdGF0ZSA9IChydWxlc2V0W2luZGV4KytdID09ICc6JykgPyBQYXJzZVN0YXRlLlZBTFVFIDogUGFyc2VTdGF0ZS5OQU1FO1xuICAgICAgICAgICAgICAgIGJyZWFrO1xuXG4gICAgICAgICAgICBjYXNlIFBhcnNlU3RhdGUuVkFMVUU6XG4gICAgICAgICAgICAgICAgbGV0IGJ1ZmZlciA9IFwiXCI7XG4gICAgICAgICAgICAgICAgd2hpbGUodHJ1ZSkge1xuICAgICAgICAgICAgICAgICAgICBpZihpbmRleCA+PSBydWxlc2V0Lmxlbmd0aClcbiAgICAgICAgICAgICAgICAgICAgICAgIGJyZWFrO1xuXG4gICAgICAgICAgICAgICAgICAgIC8vIEhhbmRsZSBlc2NhcGluZyBzcGVjaWFsIGNoYXJhY3RlcnMuXG4gICAgICAgICAgICAgICAgICAgIGlmKGluZGV4ID4gMCAmJiBydWxlc2V0W2luZGV4LTFdID09ICdcXFxcJyAmJiAocnVsZXNldFtpbmRleF0gPT0gJyYnIHx8IHJ1bGVzZXRbaW5kZXhdID09ICc7JykpIHtcbiAgICAgICAgICAgICAgICAgICAgICAgIGJ1ZmZlciArPSBydWxlc2V0LnN1YnN0cmluZyhzdGFydEluZGV4LCBpbmRleCAtIDEpOyAvLyAtIDEgdG8gcmVtb3ZlIHRoZSBiYWNrc2xhc2hcbiAgICAgICAgICAgICAgICAgICAgICAgIHN0YXJ0SW5kZXggPSBpbmRleCsrO1xuICAgICAgICAgICAgICAgICAgICAgICAgY29udGludWU7XG4gICAgICAgICAgICAgICAgICAgIH1cblxuICAgICAgICAgICAgICAgICAgICBpZihydWxlc2V0W2luZGV4XSA9PSAnJicgfHwgcnVsZXNldFtpbmRleF0gPT0gJzsnKVxuICAgICAgICAgICAgICAgICAgICAgICAgYnJlYWs7XG5cbiAgICAgICAgICAgICAgICAgICAgaW5kZXgrKztcbiAgICAgICAgICAgICAgICB9XG5cbiAgICAgICAgICAgICAgICBpZihpbmRleCAtIHN0YXJ0SW5kZXggPiAwKVxuICAgICAgICAgICAgICAgICAgICBidWZmZXIgKz0gcnVsZXNldC5zdWJzdHJpbmcoc3RhcnRJbmRleCwgaW5kZXgpO1xuICAgICAgICAgICAgICAgIFxuICAgICAgICAgICAgICAgIHJlc3VsdFtyZXN1bHQubGVuZ3RoIC0gMV0ucGFyYW1zLnB1c2goYnVmZmVyLnRyaW0oKSk7XG4gICAgICAgICAgICAgICAgc3RhdGUgPSAocnVsZXNldFtpbmRleCsrXSA9PSAnJicpID8gUGFyc2VTdGF0ZS5WQUxVRSA6IFBhcnNlU3RhdGUuTkFNRTtcbiAgICAgICAgICAgICAgICBicmVhaztcblxuICAgICAgICAgICAgZGVmYXVsdDogdGhyb3cgXCJpbnRlcm5hbCBlcnJvclwiO1xuICAgICAgICB9XG4gICAgfVxuXG4gICAgcmV0dXJuIHJlc3VsdDtcbn0iXX0=