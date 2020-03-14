<template>
    <div class="alert" :class="{ [type]: true, 'show': !closed }">
        <div class="content">
            <slot></slot>
        </div>
        <div class="close" @click="toggle">
            <i class="las la-times"></i>
        </div>
    </div>
</template>

<script>
export default {
    data: function() {
        return {
            closed: false
        };
    },

    props: {
        type: {
            type: String,
            required: true,
            validator: function(v) {
                return ["error", "info"].indexOf(v) !== -1;
            }
        },

        show: {
            type: Boolean,
            default: false
        }
    },

    methods: {
        toggle() {
            this.closed = !this.closed;
        }
    },

    created() {
        this.closed = !this.show;
    }
}
</script>