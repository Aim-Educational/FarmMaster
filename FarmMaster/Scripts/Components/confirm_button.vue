<template>
    <button @click="onClick"
            @mouseout="onMouseOut">
            {{ buttonText }}
        </button>
</template>

<script>
export default {
    data() {
        return {
            text: "",
            showConfirm: false
        }
    },

    props: {
        prompt: String,
        confirm: String
    },

    computed: {
        buttonText() {
            return this.showConfirm ? this.confirm : this.prompt;
        }
    },

    methods: {
        onClick(e) {
            if(!this.showConfirm) {
                this.showConfirm = true;
                return;
            }

            this.showConfirm = false;
            this.$emit("click", e);
        },

        onMouseOut() {
            this.showConfirm = false;
        }
    }
}
</script>