<template>
    <div class="modal-background" 
         :class="{ 'hidden': !isShowing }"
         @click="hide()">
        <div class="modal"
             @click.stop>
            <div class="header">
                <slot name="header">
                </slot>
            </div>
            <div class="body">
                <slot name="body">
                </slot>
            </div>
            <div class="footer">
                <slot name="footer">
                </slot>
            </div>
        </div>
    </div>
</template>

<script>
export default {
    data() {
        return {
            isShowing: false
        };
    },

    methods: {
        show() {
            this.isShowing = true;
        },

        hide() {
            this.isShowing = false;
        }
    },

    created() {
        document.addEventListener("keydown", e => {
            if(e.key === "Escape" && this.isShowing)
                this.hide();
        });
    }
}
</script>