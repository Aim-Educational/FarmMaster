<template>
    <input class="farm-master" 
           :name="name" 
           :type="type" 
           :placeholder="placeholder"
           :value="value"
           @input="$emit('input', $event.target.value)" />
</template>

<script>
export default {
    props: {
        isPassword: Boolean,
        name: String,
        placeholder: String,
        value: String
    },

    computed: {
        type() {
            return this.isPassword ? "password" : "text";
        }
    }
}
</script>