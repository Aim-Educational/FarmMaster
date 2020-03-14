const VueLoaderPlugin = require("vue-loader/lib/plugin");
const webpack = require("webpack");

module.exports = {
    mode: "production",
    entry : {
        util: "./Scripts/Bundles/bundle_util.js"
    },
    devtool: false,
    output: {
        library: "[name]",
        libraryExport: "default",
        filename: "[name].js"
    },
    module: {
        rules: [
            {
                test: /\.vue$/,
                include: /Scripts/,
                loader: "vue-loader"
            },
            {
                test: /\.js$/,
                include: /Scripts/,
                exclude: /node_modules/,
                loader: "babel-loader"
            }
        ]
    },
    resolve: {
        alias: {
            vue$: "vue/dist/vue.esm.js"
        },
        extensions: ["*", ".js", ".vue", ".json"]
    },
    plugins: [
        new VueLoaderPlugin(),
        new webpack.SourceMapDevToolPlugin({
            filename: "[file].map",
            publicPath: "/js/"
        })
    ]
};