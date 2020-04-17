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
import Common from "./_common";
export default {
    props: {
        type: String,
        name: String,
        placeholder: String,
        value: String,
        mode: Common.vueProps.viewOrEdit
    }
}
</script>