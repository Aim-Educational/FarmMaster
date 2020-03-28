<template>
    <button @click="onClick">
        {{ buttonText }}
    </button>
</template>

<script>
export default {
    data() {
        return {
            toggled: false
        }
    },

    props: {
        untoggledText: String,
        toggledText: String,
        value: Boolean
    },

    computed: {
        buttonText() {
            return this.toggled ? this.toggledText : this.untoggledText;
        }
    },

    methods: {
        onClick(e) {
            this.toggled = !this.toggled;
            this.$emit("input", this.toggled);
        }
    },

    created() {
        this.toggled = this.value;
    }
}
</script>