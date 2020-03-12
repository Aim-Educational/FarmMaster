﻿// Include gulp
let gulp = require('gulp');

// Include Our Plugins
let sass        = require('gulp-sass');
let cleanCSS    = require('gulp-clean-css');
let concat      = require("gulp-concat");
let order       = require("gulp-order");

// Configure plugins
sass.compiler = require("sass");

// Path configs
const paths = {
    src: {
        sass: "Styles/!(_)*.scss",
        sass_watch: "Styles/*.scss",
    },

    dest: {
        sass: "wwwroot/css"
    }
};

// Compile & Minify Our Sass
const cssOrder = [
    "**"
];

gulp.task('compile-sass', function () {
    return gulp.src(paths.src.sass)
        .pipe(sass())
        .pipe(gulp.dest(paths.dest.sass));
});

gulp.task("minify-css", function () {
    return gulp.src(paths.dest.sass + "/!(bundle.min.css|site.css)")
        .pipe(order(cssOrder))
        .pipe(concat("bundle.min.css"))
        .pipe(cleanCSS({ level: 2 }))
        .pipe(gulp.dest(paths.dest.sass));
});

gulp.task("sass", gulp.series("compile-sass", "minify-css"));

// Watch Files For Changes
gulp.task('watch', function () {
    gulp.watch(paths.src.sass_watch, gulp.series(["sass"]));
});

// Default Task
gulp.task('default', gulp.series(['sass']));