<template>
    <modal ref="modal">
        <template v-slot:header>
            <h2>Delete</h2>
        </template>
        <template v-slot:body>
            <div style="flex-direction:column; display:flex; width:100%;">
                <p style="margin-top: 0;">To delete this item, please enter DELETE into the box below.</p>
                <p>THIS ACTION CANNOT BE UNDONE.</p>
                <input class="farm-master" type="text" placeholder="Please enter DELETE" v-model="deleteText" />
            </div>
        </template>
        <template v-slot:footer>
            <div class="button-group">
                <button @click="hide()">
                    Cancel
                </button>
                <button class="red"
                        @click="$emit('click', $event)"
                        :disabled="deleteText !== 'DELETE'">
                    Delete
                </button>
            </div>
        </template>
    </modal>
</template>

<script>
import Modal from "./modal"

export default {
    data() {
        return {
            deleteText: ""
        }
    },

    methods: {
        show() {
            this.$refs.modal.show();
        },

        hide() {
            this.$refs.modal.hide();
            this.deleteText = "";
        }
    },

    components: {
        "modal": Modal
    }
}
</script>