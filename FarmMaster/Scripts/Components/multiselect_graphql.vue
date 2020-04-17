<template>
    <vue-multiselect :value="value"
                     track-by="name"
                     label="name"
                     :placeholder="placeholder"
                     :allow-empty="false"
                     :options="values"
                     :disabled="disabled"
                     @input="$emit('input', $event)">
    </vue-multiselect>
</template>

<script>
const queries = {
    species: {
        query: `query {
            specieses {
                items {
                    id
                    name    
                }
            }
        }`,
        accessor: "specieses"
    }
};

export default {
    data() {
        return {
            values: []
        };
    },

    props: {
        value: Object,
        placeholder: String,
        mode: {
            type: String,
            default: "edit",
            validator(v) {
                return ["edit", "view"].indexOf(v) !== -1;
            }
        },
        entityType: {
            required: true,
            type: String,
            validator(v) {
                return Object.keys(queries).indexOf(v) !== -1;
            }
        }
    },

    created() {
        this.updateValues();
    },

    computed: {
        disabled() {
            return this.mode !== "edit";
        }
    },

    methods: {
        updateValues() {
            libs.Ajax
                .graphql(queries[this.entityType].query)
                .then(data => this.values = data[queries[this.entityType].accessor].items)
                .catch(e => console.log(e));
        }
    }
}
</script>