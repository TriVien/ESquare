/// <binding Clean='clean' />
"use strict";

var gulp = require("gulp"),
    rimraf = require("rimraf"),
    concat = require("gulp-concat"),
    cssmin = require("gulp-cssmin"),
    uglify = require("gulp-uglify");

var paths = {
    webroot: "./wwwroot/",
    npmSrc: "./node_modules/",
    npmLibs: "./wwwroot/lib/npmlibs/"
};

paths.js = paths.webroot + "js/**/*.js";
paths.minJs = paths.webroot + "js/**/*.min.js";
paths.css = paths.webroot + "css/**/*.css";
paths.minCss = paths.webroot + "css/**/*.min.css";
paths.concatJsDest = paths.webroot + "js/site.min.js";
paths.concatCssDest = paths.webroot + "css/site.min.css";

// ========= CLEAN
gulp.task("clean:js", function (cb) {
    rimraf(paths.concatJsDest, cb);
});

gulp.task("clean:css", function (cb) {
    rimraf(paths.concatCssDest, cb);
});

gulp.task("clean", ["clean:js", "clean:css"]);
// ================

// ========= MINIFY
gulp.task("min:js", function () {
    return gulp.src([paths.js, "!" + paths.minJs], { base: "." })
        .pipe(concat(paths.concatJsDest))
        .pipe(uglify())
        .pipe(gulp.dest("."));
});

gulp.task("min:css", function () {
    return gulp.src([paths.css, "!" + paths.minCss])
        .pipe(concat(paths.concatCssDest))
        .pipe(cssmin())
        .pipe(gulp.dest("."));
});
// =================

gulp.task("min", ["min:js", "min:css"]);

// =========== COPY NPM LIBS TO WWWROOT
gulp.task("copy-deps:systemjs", function () {
    return gulp.src(paths.npmSrc + '/systemjs/dist/**/*.*', { base: paths.npmSrc + '/systemjs/dist/' })
         .pipe(gulp.dest(paths.npmLibs + '/systemjs/'));
});

gulp.task("copy-deps:angular2", function () {
    return gulp.src(paths.npmSrc + '/angular2/bundles/**/*.js', { base: paths.npmSrc + '/angular2/bundles/' })
         .pipe(gulp.dest(paths.npmLibs + '/angular2/'));
});

gulp.task("copy-deps:es6-shim", function () {
    return gulp.src(paths.npmSrc + '/es6-shim/es6-sh*', { base: paths.npmSrc + '/es6-shim/' })
         .pipe(gulp.dest(paths.npmLibs + '/es6-shim/'));

});
gulp.task("copy-deps:es6-promise", function () {
    return gulp.src(paths.npmSrc + '/es6-promise/dist/**/*.*', { base: paths.npmSrc + '/es6-promise/dist/' })
         .pipe(gulp.dest(paths.npmLibs + '/es6-promise/'));
});

gulp.task("copy-deps:rxjs", function () {
    return gulp.src(paths.npmSrc + '/rxjs/bundles/*.*', { base: paths.npmSrc + '/rxjs/bundles/' })
         .pipe(gulp.dest(paths.npmLibs + '/rxjs/'));
});

gulp.task("copy-deps", ["copy-deps:rxjs", 'copy-deps:angular2', 'copy-deps:systemjs', 'copy-deps:es6-shim', 'copy-deps:es6-promise']);
// =================
