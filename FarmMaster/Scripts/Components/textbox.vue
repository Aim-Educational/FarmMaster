<template>
    <input :name="name" :type="type" :placeholder="placeholder" />
</template>

<script>
export default {
    props: {
        isPassword: Boolean,
        name: String,
        placeholder: String
    },

    computed: {
        type() {
            return this.isPassword ? "password" : "text";
        }
    }
}
</script>