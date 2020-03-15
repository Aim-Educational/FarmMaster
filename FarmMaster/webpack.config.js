const VueLoaderPlugin = require("vue-loader/lib/plugin");
const HardSourceWebpackPlugin = require('hard-source-webpack-plugin');
const webpack = require("webpack");
const path = require("path");

module.exports = {
    mode: "production",
    entry : {
        libs: "./Scripts/Bundles/bundle_libs.js",
        controls: "./Scripts/Bundles/bundle_controls.js",
        external_controls: "./Scripts/Bundles/bundle_thirdparty_controls.js"
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
                include: path.resolve(__dirname, "Scripts/"),
                exclude: path.resolve(__dirname, "node_modules/"),
                loader: "vue-loader"
            },
            {
                test: /\.js$/,
                include: path.resolve(__dirname, "Scripts/"),
                exclude: path.resolve(__dirname, "node_modules/"),
                loader: "babel-loader"
            },
            {
                test: /\.tsx?$/,
                include: path.resolve(__dirname, "Scripts/"),
                exclude: path.resolve(__dirname, "node_modules/"),
                use: "ts-loader",
            }
        ]
    },
    resolve: {
        alias: {
            vue$: "vue/dist/vue.esm.js"
        },
        extensions: ["*", ".js", ".vue", ".json", ".ts"]
    },
    plugins: [
        new VueLoaderPlugin(),
        new HardSourceWebpackPlugin(),
        new webpack.SourceMapDevToolPlugin({
            filename: "[file].map",
            publicPath: "/js/"
        })
    ],
    performance: {
        hints: false
    }
};