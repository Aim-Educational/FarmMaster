/**
 * Exports certain values/functions that are shared between different components.
 * 
 * NOTE: Only things that are exclusive to Vue and Vue components should be placed here, anything else should be made into a library.
 */
export default {
    /// Common props that a vue component may have.
    vueProps: {
        /// Usually exposed as "mode", this is used by components that can be toggled between a "view" or "edit" mode.
        viewOrEdit: {
            type: String,
            default: "edit",
            validator(v: string) {
                return ["edit", "view"].indexOf(v) !== -1;
            }
        }
    }
}