<template>
    <div class="notes">
        <template v-if="mode === 'edit'">
            <div class="input-wrapper inline">
                <label>Category</label>
                <textbox :mode="mode"
                         v-model="category"
                         placeholder="Keep notes organised">
                </textbox>
            </div>
            <div class="input-wrapper inline">
                <label>Note</label>
                <textbox :mode="mode"
                         v-model="note"
                         placeholder="Andy Smells">
                </textbox>
            </div>
            <button class="blue top bottom margined"
                    @click="onCreateNote">
                Create note
            </button>
        </template>

        <value-table :selection="mode === 'edit' ? 'multiple' : 'none'"
                     :rows="[{ name: 'Category', bind: 'category', sort: true },
                             { name: 'Note', bind: 'content', sort: false }
                            ]"
                     :values="notes"
                     @selected="onSelected">
            <template v-slot:tfoot>
                <button class="red"
                        :disabled="selectedNotes.length === 0"
                        v-if="mode === 'edit'"
                        @click="onDeleteNotes">
                    Delete selected notes ({{ selectedNotes.length }})
                </button>
            </template>
        </value-table>
    </div>
</template>

<script>
import Textbox from "./textbox";
import Table from "./table_values";
import Common from "./_common";

// Generic query generators.
// Since most of the queries will only really differ in their GraphQL type name, this saves some typing.
function genericQuery(typeName) {
    return `query GetNotes($id:ID!) {
        ${typeName}(id:$id) {
            notes {
                id
                category
                content
            }    
        }    
    }`;
}

function genericAdd(typeName) {
    return `mutation AddNote($id:ID!, $category:String!, $content:String!) {
        ${typeName}(id:$id) {
            addNote(category:$category, content:$content)
        }
    }`;
}

function genericDelete(typeName) {
    return `mutation DeleteNotes($id:ID!, $noteIds:[ID!]!) {
        ${typeName}(id:$id) {
            deleteNotes(ids:$noteIds)    
        }    
    }`;
}

const queries = {
    contact: {
        query: genericQuery("contact"),
        add: genericAdd("contact"),
        delete: genericDelete("contact")
    },
    
    species: {
        query: genericQuery("species"),
        add: genericAdd("species"),
        delete: genericDelete("species")
    },

    breed: {
        query: genericQuery("breed"),
        add: genericAdd("breed"),
        delete: genericDelete("breed")
    }
};

export default {
    data() {
        return {
            category: "",
            note: "",
            notes: [],
            selectedNotes: []
        }
    },

    props: {
        mode: Common.vueProps.viewOrEdit,

        parentType: {
            type: String,
            required: true,
            validator(v) {
                return Object.keys(queries).indexOf(v) !== -1;
            }
        },

        parentId: {
            type: Number,
            required: true
        }
    },

    methods: {
        onError(error) {
            console.log(error);
            alert(JSON.stringify(error));
        },

        selectNotesFromQuery(data) {
            return data[this.parentType].notes;
        },
        
        onCreateNote() {
            libs.Ajax
                .graphql(queries[this.parentType].add, { id: this.parentId, category: this.category, content: this.note })
                .then(_ => { 
                    this.refreshNotes()
                    this.note = "";
                    this.category = "";
                })
                .catch(e => this.onError(e));
        },

        onDeleteNotes() {
            libs.Ajax
                .graphql(queries[this.parentType].delete, { id: this.parentId, noteIds: this.selectedNotes.map(n => n.id) })
                .then(_ => this.refreshNotes())
                .catch(e => this.onError(e));
        },

        refreshNotes() {
            libs.Ajax
                .graphql(queries[this.parentType].query, { id: this.parentId })
                .then(data => this.notes = this.selectNotesFromQuery(data))
                .catch(e => this.onError(e));
        },

        onSelected(e) {
            this.selectedNotes = e.selectedValues;
        }
    },

    created() {
        this.refreshNotes();
    },

    components: {
        'textbox': Textbox,
        'value-table': Table
    }
}
</script>