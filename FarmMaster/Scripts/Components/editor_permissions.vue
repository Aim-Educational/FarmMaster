<template>
    <div class="editor-permissions">
        <alert type="error"
               class="bottom margined"
               :class="{ 'show': errors.length > 0}"> <!--Cus for some reason :show doesn't work, ./shrug-->
               <header>Error</header>
               <ul>
                   <li v-for="error in errors" :key="error">
                       {{ error }}
                    </li>
               </ul>
        </alert>

        <alert class="bottom margined" type="info" show>
            <header>Note</header>
            <p>
                Users will have to sign out, then log back in before their new permissions take affect.
            </p>
        </alert>

        <div class="perm"
             v-for="perm in allPermissions"
             :key="perm">
             <input type="checkbox" :value="perm" v-model="myPermissions" :disabled="mode === 'view'" />
             <label>{{ perm }}</label>
        </div>

        <button class="top margined blue fluid"
                v-if="mode == 'edit'"
                @click="updatePerms">
            Update permissions
        </button>
    </div>
</template>

<script>
import Alert from "./alert";

export default {
    props: {
        mode: String,

        username: {
            type: String,
            required: true
        }
    },

    data: function(){
        return {
            allPermissions: [],
            myPermissions: [],
            errors: []
        };
    },

    created() {
        libs.Ajax.graphql(`
        query($username: String!) {
            permissions
            user(username:$username) {
                permissions
            }
        }`, { username: this.username })
        .then(json => { 
            this.allPermissions = json.permissions.sort();
            this.myPermissions = json.user.permissions;
        })
        .catch(e => this.errors = e.errors);
    },

    methods: {
        updatePerms() {
            const permsToAdd = this.allPermissions.filter(p => this.myPermissions.indexOf(p) !== -1);
            const permsToRemove = this.allPermissions.filter(p => this.myPermissions.indexOf(p) === -1);

            libs.Ajax.graphql(`
            mutation($username: String!, $permsToAdd: [String]!, $permsToRemove: [String]!) {
                user(username:$username) {
                    grantPermissions(permissions:$permsToAdd)
                    revokePermissions(permissions:$permsToRemove)
                }
            }`, { username: this.username, permsToAdd: permsToAdd, permsToRemove: permsToRemove })
            .then(v => {
                this.$emit("on-valid-update");

                return v;
            })
            .catch(e => { 
                console.log(e);
                this.errors = (e && e.errors) || [e.message];
            });
        }
    },

    components: {
        'alert': Alert
    }
}
</script>