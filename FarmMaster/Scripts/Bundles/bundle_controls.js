import Window from "../Components/window.vue";
import Textbox from "../Components/textbox.vue";
import Alert from "../Components/alert.vue";
import VueMultiselect from "vue-multiselect";
import FarmTable from "../Components/table.vue";

Vue.component("window", Window);
Vue.component("textbox", Textbox);
Vue.component("alert", Alert);
Vue.component("vue-multiselect", VueMultiselect);
Vue.component("farm-table", FarmTable);

export default {
    Window,
    Textbox,
    Alert,
    VueMultiselect,
    FarmTable
}