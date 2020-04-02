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

const queries = {
    contact: {
        query: `query GetContactNotes($id:ID!) {
            contact(id:$id) {
                notes {
                    id
                    category
                    content
                }    
            }    
        }`,

        add: `mutation AddNoteToContact($id:ID!, $category:String!, $content:String!) {
            contact(id:$id) {
                addNote(category:$category, content:$content)
            }
        }`,

        delete: `mutation DeleteNotesFromContact($id:ID!, $noteIds:[ID!]!) {
            contact(id:$id) {
                deleteNotes(ids:$noteIds)    
            }    
        }`
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
        mode: {
            type: String,
            default: "edit",
            validator(v) {
                return ["edit", "view"].indexOf(v) !== -1;
            }
        },

        parentType: {
            type: String,
            required: true,
            validator(v) {
                return ["contact"].indexOf(v) !== -1;
            }
        },

        parentId: {
            type: Number,
            required: true
        }
    },

    methods: {
        onError(error) {
            alert(JSON.stringify(error));
        },

        selectNotesFromQuery(data) {
            switch(this.parentType) {
                case "contact":
                    return data.contact.notes;

                default: throw this.parentType;
            }
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