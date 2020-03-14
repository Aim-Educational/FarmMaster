import Window from "../Components/window.vue";
import Textbox from "../Components/textbox.vue";
import Alert from "../Components/alert.vue";

Vue.component("window", Window);
Vue.component("textbox", Textbox);
Vue.component("alert", Alert);

export default {
    Window,
    Textbox,
    Alert
}