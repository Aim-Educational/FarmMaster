<template>
    <fm-table :rows="rows"
              :selection="selection"
              :values="values"
              @sort="onSort"
              v-on="$listeners"
              ref="table">
        <template v-slot:tfoot>
            <div class="paging">
                <button :disabled="!pageInfo.hasPreviousPage"
                        @click="onBack">
                    Back
                </button>
                <button :disabled="!pageInfo.hasNextPage"
                        @click="onForward">
                    Forward
                </button>
            </div>
        </template>
    </fm-table>
</template>

<script>
import FarmTableBase from "./table_base";

const ITEMS_PER_PAGE = 25;

/**
 */
export default {
    props: {
        rows: FarmTableBase.props.rows,
        selection: FarmTableBase.props.selection,
        queryRoot: String,
        itemsQuery: String
    },

    data() {
        return {
            pageInfo: { startCursor: 0, endCursor: 0 },
            values: [],
            lastSortEvent: {}
        }
    },

    computed: {
        fullQuery() {
            return `
            query GetItems($after:String!, $order: String) {
                ${this.queryRoot}(after: $after, order: $order, first: ${ITEMS_PER_PAGE}) {
                    items {
                        ${this.itemsQuery}
                    }
                    pageInfo {
                        endCursor
                        hasNextPage
                        hasPreviousPage
                        startCursor
                    }
                }
            }`;
        }
    },

    methods: {
        onSort(event) {
            this.lastSortEvent = event;
            libs.Ajax
                .graphql(this.fullQuery,
                { 
                    after: ""+this.pageInfo.startCursor, 
                    order: (!event.row) ? null : (event.asc) ? event.row.bind : event.row.bind + "_desc"
                })
                .then(json => {
                    var data = json[this.queryRoot];
                    this.values = data.items;
                    this.pageInfo = data.pageInfo;
                })
                .catch(e => console.log(e));
        },

        onForward() {
            this.pageInfo.startCursor = this.pageInfo.endCursor;
            this.onSort(this.lastSortEvent);
        },

        onBack() {
            var start     = Number(this.pageInfo.startCursor);
            var remainder = start % ITEMS_PER_PAGE;
            if(remainder > 0)
                this.pageInfo.startCursor = ""+(start - remainder);
            else
                this.pageInfo.startCursor = ""+(start - ITEMS_PER_PAGE);

            this.onSort(this.lastSortEvent);
        }
    },

    created() {
        this.onSort({});
    },

    components: {
        'fm-table': FarmTableBase
    }
}
</script>