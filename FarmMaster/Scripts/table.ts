import GraphQL from "./graphql.js";

export class PagedTable {
    public readonly ITEMS_PER_PAGE: number = 25;

    public readonly tableNode: HTMLTableElement;
    public readonly tbody: HTMLTableSectionElement;
    public readonly tfooter: HTMLTableSectionElement;
    public readonly pageItems: HTMLDivElement;

    public readonly pageName: string;
    public readonly controller: string;
    public readonly actionEdit: string;
    public readonly actionDelete: string;

    constructor(
        tableNodeOrId: HTMLTableElement | string,
        public readonly query: string,
        public readonly queryToList: (data: any) => { id: number, items: string[] }[],
        public readonly configFunc?: (id: number,           controller: string,            actionEdit: string,
                                      actionDelete: string, buttonEdit: HTMLAnchorElement, buttonDelete: HTMLAnchorElement) => void
    ) {
        if (tableNodeOrId instanceof HTMLTableElement)
            this.tableNode = tableNodeOrId;
        else
            this.tableNode = <HTMLTableElement>document.getElementById(tableNodeOrId);

        if (!this.tableNode)
            throw "The table node is null. Parameter given was: " + tableNodeOrId;

        this.tbody = (this.tableNode.tBodies.length > 0) ? this.tableNode.tBodies.item(0)! : this.tableNode.createTBody();
        this.tfooter = (this.tableNode.tFoot) ? this.tableNode.tFoot : this.tableNode.createTFoot();

        this.pageName = this.tableNode.dataset.graphqlPageCountName!;
        this.controller = this.tableNode.dataset.controller!;
        this.actionEdit = this.tableNode.dataset.actionEdit!;
        this.actionDelete = this.tableNode.dataset.actionDelete!;

        const tr = this.tfooter.appendChild(document.createElement("tr"));
        const th = tr.appendChild(document.createElement("th"));
        th.colSpan = 9999;
        this.pageItems = th.appendChild(document.createElement("div"));
        this.pageItems.classList.add("ui", "center", "aligned", "pagination", "menu");

        // Param names are just short hand versions of the ones in the typings for configFunc above.
        configFunc = configFunc ? configFunc : (id, c, ae, ad, be, bd) => {
            be.href = `/${c}/${ae}/${id}`;
            bd.href = `/${c}/${ad}/${id}`;
        };

        this.query = query;
        this.refreshPageCount();
        this.setPage(0);
    }

    public refreshPageCount() {
        GraphQL.query(
            `query GetPageCount{
                pageCount {
                    ${this.pageName}
                }
            }`, null)
            .then((data: { pageCount: any }) => {
                while (this.pageItems.firstChild)
                    this.pageItems.removeChild(this.pageItems.firstChild);

                for (let i = 0; i < data.pageCount[this.pageName]; i++) {
                    const a = this.pageItems.appendChild(document.createElement("a"));
                    a.classList.add("item");
                    a.innerText = "" + (i + 1);
                    a.onclick = () => {
                        this.setPage(i);
                    };
                }
            })
            .catch(e => alert("Failed to fetch page: " + JSON.stringify(e)));
    }

    public setPage(page: number) {
        GraphQL
            .query(this.query, { skip: (this.ITEMS_PER_PAGE * page), take: this.ITEMS_PER_PAGE })
            .then(data => {
                while (this.tbody.firstChild)
                    this.tbody.removeChild(this.tbody.firstChild);

                const items = this.queryToList(data);
                let i = 0;
                for (const itemRow of items) {
                    const tr = this.tbody.appendChild(document.createElement("tr"));
                    for (const item of itemRow.items) {
                        const td = tr.appendChild(document.createElement("td"));
                        td.innerText = item;
                    }

                    const td = tr.appendChild(document.createElement("td"));
                    const div = td.appendChild(document.createElement("div"));
                    div.classList.add("ui", "two", "buttons");

                    const buttonEdit = div.appendChild(document.createElement("a"));
                    const divider = div.appendChild(document.createElement("div"));
                    const buttonDelete = div.appendChild(document.createElement("a"));

                    buttonEdit.classList.add("ui", "inverted", "tiny", "secondary", "left", "button");
                    buttonDelete.classList.add("ui", "inverted", "tiny", "secondary", "right", "button");
                    divider.classList.add("or", "divider");

                    buttonEdit.innerText = "Edit";
                    buttonDelete.innerText = "Delete";

                    this.configFunc!(
                        itemRow.id,
                        this.controller,
                        this.actionEdit,
                        this.actionDelete,
                        buttonEdit,
                        buttonDelete
                    );
                }
            });
    }
}