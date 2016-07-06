/// <vs BeforeBuild='css, scss, plugins' />
var gulp = require('gulp');
var concat = require('gulp-concat');
var sass = require('gulp-sass');
var sourcemaps = require('gulp-sourcemaps');
var babel = require('gulp-babel');
var browserify = require('browserify');
var source = require('vinyl-source-stream');
var buffer = require('vinyl-buffer');
var uglify = require('gulp-uglify');
var gutil = require('gulp-util');


var plugins = [
    'src/plugins/jquery-2.1.1.min.js',
    'src/plugins/jquery.dataTables.min.js',
    'src/plugins/bootstrap.min.js',
    'src/plugins/dataTables.bootstrap.js',
    'src/plugins/bootstrap-switch.min.js',
    'src/plugins/moment.min.js',
    'src/plugins/moment_es.js',
    'node_modules/select2/select2.js',
    'node_modules/select2/select2_locale_es.js',
    'src/plugins/**/*.js'
];

var theme_js = [
    'src/js/metronic.js',
    'src/js/layout.js',
    'src/js/bootbox.js',
    'src/js/bootstrap-editable.js',
    'src/js/bootstrap-switch.min.js',
    'src/js/cent11.js'
];

var theme_css = [
    'src/css/bootstrap.min.css',
    'src/css/bootstrap-switch.css',
    'src/plugins/dataTables.bootstrap.css',
    'src/css/font-awesome.min.css',
    'src/css/simple-line-icons.css',
    'src/css/login2.css',
    'src/css/components-md.css',
    'src/css/plugins-md.css',
    'src/css/layout.css',
    'src/css/default.css',
    'src/css/Site.css',
    'src/css/bootstrap-editable.css'
];

gulp.task('css', function () {
    return gulp.src(theme_css)
        .pipe(concat('cent11.concat.css'))
        .pipe(gulp.dest('./Content'));
});


gulp.task('plugins', function () {
    return gulp.src(plugins)
        .pipe(concat('plugins.concat.js'))
        .pipe(gulp.dest('./Scripts'));
});

gulp.task('js', ['plugins'], function () {
    return gulp.src(theme_js)
        .pipe(concat('colegio_terciario.concat.js'))
        .pipe(gulp.dest('./Scripts'));
});

gulp.task('app_js', function () {
    var b = browserify({
        entries: 'src/js/equivalencias.js',
        debug: true
    });

    return b.bundle()
        .pipe(source('equivalencias.js'))
        .pipe(buffer())
        .pipe(sourcemaps.init({ loadMaps: true }))
            // Add transformation tasks to the pipeline here.
            .pipe(uglify().transform(require('babelify')))
            .on('error', gutil.log)
        .pipe(sourcemaps.write('./'))
        .pipe(gulp.dest('./Scripts/'));
});

gulp.task('default', ['css', 'js'], function () {
    // place code for your default task here
});
