<template>
    <div class="control-window">
        <header>
            <h1>{{ title }}</h1>
        </header>
        <div class="content">
            <slot></slot>
        </div>
    </div>
</template>

<script>
export default {
    props: {
        title: String
    }
}
</script>