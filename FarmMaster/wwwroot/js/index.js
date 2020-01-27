// TODO: In release builds, bundle all these files into one.
export * from "/lib/dranges/dist/dranges.min.js"
export * from "/lib/jaster-validate/dist/jaster-validate.min.js"
export * from "/js/component_select.js"
export * from "/js/farm_ajax.js"
export * from "/js/cookies.js"
export * from "/js/modal.js"
export * from "/js/characteristic_helper.js";
export * from "/js/graphql.js";
export * from "/js/component_table.js";
export * from "/js/dropdown.js";

import { IValidationUIHandler, Validation } from "/lib/jaster-validate/dist/jaster-validate.min.js"
import { drange } from "/lib/dranges/dist/dranges.min.js"

export class SemanticUIHandler extends IValidationUIHandler {
    onElementFailedValidation(element, reasonsFailed) {
        const reason = drange(reasonsFailed).reduce("", (s1, s2) => (s1.length > 0) ? s1 + '\n' + s2 : s2);
        const errorBox = this.ensureErrorBoxExists(element);

        errorBox.classList.remove("hidden");
        errorBox.classList.add("visible");
        errorBox.innerText = reason;
    }

    onElementPassedValidation(element) {
        this.onElementValueChange(element);
    }

    onElementValueChange(element) {
        const errorBox = this.ensureErrorBoxExists(element);

        errorBox.classList.remove("visible");
        errorBox.classList.add("hidden");
    }

    onFormPassedValidation(form) { }
    onFormFailedValidation(form, reasonsFailedPerElement, submitEvent) { }

    ensureErrorBoxExists(element) {
        let parent = element.parentNode;
        while ((parent !== null || parent !== undefined) && !parent.classList.contains("field"))
            parent = parent.parentNode;

        if (parent === null || parent === undefined)
            alert("Dev error, could not find a parent with class 'field' ;(");

        const existingDiv = parent.querySelector(".jvalidsemantic")
        if (existingDiv !== null)
            return existingDiv;

        const div = document.createElement("div");
        parent.appendChild(div);

        // styling
        div.classList.add("ui", "basic", "red", "pointing", "prompt", "label", "transition", "hidden");

        // identification
        div.classList.add("jvalidsemantic");

        return div;
    }
}

Validation.addUIHandler("semantic-ui", new SemanticUIHandler());
Validation.definePolicy("semantic-ui", "semantic-ui", "no-submit-form");