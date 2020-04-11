<template>
    <input class="farm-master" 
           v-if="mode === 'edit'"
           :name="name" 
           :type="type" 
           :placeholder="placeholder"
           :value="value"
           @input="$emit('input', $event.target.value)" />
    <div v-else>
        {{ this.value }}
    </div>
</template>

<script>
export default {
    props: {
        type: String,
        name: String,
        placeholder: String,
        value: String,
        mode: {
            type: String,
            default: "edit",
            validator(v) {
                return ["edit", "view"].indexOf(v) !== -1;
            }
        }
    }
}
</script>