export * from "https://unpkg.com/dranges@latest/dist/dranges.min.js"
export * from "https://unpkg.com/jaster-validate@latest/dist/jaster-validate.min.js"
export * from "/js/component_table.js"
export * from "/js/component_modal.js"
export * from "/js/component_select.js"
export * from "/js/component_characteristics.js"
export * from "/js/farm_ajax.js"
export * from "/js/cookies.js"

import { IValidationUIHandler, Validation } from "https://unpkg.com/jaster-validate@latest/dist/jaster-validate.min.js"
import { drange } from "https://unpkg.com/dranges@latest/dist/dranges.min.js"

export class SemanticUIHandler extends IValidationUIHandler {
    onElementFailedValidation(element, reasonsFailed) {
        const reason = drange(reasonsFailed).reduce("", (s1, s2) => s1 + '\n' + s2);
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
        let parent = element.parentElement;
        while ((parent !== null || parent !== undefined) && !parent.classList.contains("field"))
            parent = parent.parentElement;

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