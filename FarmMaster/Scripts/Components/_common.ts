export default {
    vueProps: {
        viewOrEdit: {
            type: String,
            default: "edit",
            validator(v: string) {
                return ["edit", "view"].indexOf(v) !== -1;
            }
        }
    }
}