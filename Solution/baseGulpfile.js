const
    concat = require("gulp-concat"),
    cssmin = require("gulp-cssmin"),
    uglify = require("gulp-uglify"),
    sass = require("gulp-dart-sass"),
    typescript = require("gulp-typescript"),
    sourcemaps = require("gulp-sourcemaps"),
    del = require("del"),
    yarn = require("gulp-yarn"),
    fs = require("fs"),
    footer = require('gulp-footer'),
    tslint = require("gulp-tslint"),
    stylelint = require("gulp-stylelint"),
    modernizr = require("gulp-modernizr");

const paths = {
    webroot: `./wwwroot/`,
    nodemodules: `./node_modules/`,
    packageJson: `./package.json`,
    yarnRc: `./.yarnrc`,
    yarnLock: `./yarn.lock`,
    style: `./Styles/`,
    sass: `./Styles/**/*.scss`,
    scripts: `./Scripts`,
    typescript: `./Scripts/**/*.ts`,
    tsconfig: `./Scripts/tsconfig.json`,

    obj: `./obj/`,
    sourceCache: `front-end-cache`,
};

const regex = {
    css: /\.css$/,
    html: /\.(html|htm)$/,
    js: /\.js$/
};

const taskNames = {
    concatSource: "concatSource",
    lintTs: "lint:ts",
    lintTsFixer: "lint:ts:fix",
    lintSass: "lint:sass",
    lintSassFixer: "lint:sass:fix",
    compileSass: "compile:sass",
    compileTypescript: "compile:typescript",
    sassWatch: "watch:sass",
    typescriptWatch: "watch:typescript",
    packageWatch: "watch:package",
    watch: "watch",
    cleanCss: "clean:css",
    cleanJs: "clean:js",
    cleanDeps: "clean:deps",
    cleanDevDeps: "clean:dev-deps",
    cleanFrontEndCache: "clean:front-cache",
    clean: "clean",
    downloadAllDeps: "download",
    downloadProductionDeps: "download:production-deps",
    deleteProductionPackageJson: "delete-production-package-json",
    bundleJs: "bundle:js",
    bundleCss: "bundle:css",
    bundleAndMinifyJs: "minify:js",
    bundleAndMinifyCss: "minify:css",
    bundleAndMinify: "minify",
    modernizr: "modernizr",
    main: "main",
};

exports.taskNames = taskNames;

const tasks = [];
const aggregators = [];

exports.initialize = (gulp, options) => {
    const {bundleConfig, areaName} = options;

    console.log(typeof areaName);
    if (typeof areaName === "string") {
        paths.webroot = `${areaName}/wwwroot/`;
        paths.style = `${areaName}/Styles/`;
        paths.sass = `${areaName}/Styles/**/*.scss`;
        paths.scripts = `${areaName}/Scripts`;
        paths.typescript = `${areaName}/Scripts/**/*.ts`;
        paths.tsconfig = `${areaName}/Scripts/tsconfig.json`;
    }

    paths.css = paths.webroot + "css/";
    paths.js = paths.webroot + "js/";
    paths.runtimedeps = paths.webroot + "node_modules/";

    const tsProject = typescript.createProject(paths.tsconfig);

    let enableSwallowTsError = false;

    function swallowTsError(error) {
        process.stderr.write(`${error.toString()}\n`);

        this.emit("end");
    }

    const getBundles = regexPattern => bundleConfig.filter(
        bundle => regexPattern.test(bundle.outputFileName)
    );

    tasks[taskNames.concatSource] = () => gulp.src(paths.sass, paths.typescript)
        .pipe(concat(paths.sourceCache))
        .pipe(footer(JSON.stringify(bundleConfig)))
        .pipe(gulp.dest(paths.obj));

    tasks[taskNames.lintTs] = () => tsProject.src()
        .pipe(tslint({
            formatter: "verbose"
        }))
        .pipe(tslint.report());

    tasks[taskNames.lintTsFixer] = () => tsProject.src()
        .pipe(tslint({
            formatter: "verbose",
            fix: true
        }))
        .pipe(gulp.dest(paths.scripts));

    tasks[taskNames.lintSass] = () => gulp.src(paths.sass)
        .pipe(stylelint({
            failAfterError: true,
            reporters: [
                {formatter: "string", console: true},
            ],
        }));

    tasks[taskNames.lintSassFixer] = () => gulp.src(paths.sass)
        .pipe(stylelint({
            failAfterError: true,
            reporters: [
                {formatter: "string", console: true},
            ],
            fix: true
        }))
        .pipe(gulp.dest(paths.style));

    tasks[taskNames.compileTypescript] = () => {
        const tsResult = tsProject.src()
            .pipe(sourcemaps.init())
            .pipe(
                tsProject().on("error",
                    enableSwallowTsError ? swallowTsError : () => process.exit(1)
                )
            );

        return tsResult.js
            .pipe(sourcemaps.write("."))
            .pipe(gulp.dest(paths.js));
    };

    tasks[taskNames.compileSass] = () => gulp.src(paths.sass)
        .pipe(sourcemaps.init())
        .pipe(sass().on("error", sass.logError))
        .pipe(sourcemaps.write("."))
        .pipe(gulp.dest(paths.css));

    tasks[taskNames.sassWatch] = () => gulp.watch(paths.sass, gulp.parallel(taskNames.bundleAndMinifyCss));

    tasks[taskNames.typescriptWatch] = () => gulp.watch(paths.typescript, gulp.parallel(taskNames.bundleAndMinifyJs));

    tasks[taskNames.packageWatch] = () => gulp.watch(paths.packageJson, gulp.parallel(taskNames.downloadProductionDeps));

    tasks[taskNames.modernizr] = () => gulp.src([paths.js + "*.js", paths.js + "**/*.js"])
        .pipe(modernizr())
        .pipe(gulp.dest(paths.js));

    const bundleJsTasksName = [];
    const bundleConfigsJs = getBundles(regex.js);
    for (const bundleConfigKey in bundleConfigsJs) {
        if (!bundleConfigsJs.hasOwnProperty(bundleConfigKey)) {
            continue;
        }

        const taskName = `${taskNames.bundleJs}:${bundleConfigKey}`;

        tasks[taskName] = () => {
            const bundle = bundleConfigsJs[bundleConfigKey];

            const inputs = [];
            for (const input of bundle.inputFiles) {
                inputs.push(`${paths.webroot}${input}`);
            }

            let gulpStream = gulp.src(inputs, {base: "."})
                .pipe(sourcemaps.init())
                .pipe(concat(`${paths.webroot}${bundle.outputFileName}`));

            if (bundle.minify && bundle.minify.enabled) {
                gulpStream = gulpStream.pipe(uglify());
            }

            return gulpStream.pipe(sourcemaps.write("."))
                .pipe(gulp.dest("."));
        };

        bundleJsTasksName[bundleConfigKey] = taskName;
    }

    aggregators[taskNames.bundleJs] = () => bundleJsTasksName.length > 0
        ? gulp.parallel(...bundleJsTasksName)
        : done => {
            done();
        };

    aggregators[taskNames.bundleAndMinifyJs] = () => gulp.series(
        gulp.parallel(
            taskNames.lintTs,
            taskNames.compileTypescript
        ),
        taskNames.modernizr,
        taskNames.bundleJs
    );

    const bundleCssTasksName = [];
    const bundleConfigsCss = getBundles(regex.css);
    for (const bundleConfigKey in bundleConfigsCss) {
        if (!bundleConfigsCss.hasOwnProperty(bundleConfigKey)) {
            continue;
        }

        const taskName = `${taskNames.bundleCss}:${bundleConfigKey}`;

        tasks[taskName] = () => {
            const bundle = bundleConfigsCss[bundleConfigKey];

            const inputs = [];
            for (const input of bundle.inputFiles) {
                inputs.push(`${paths.webroot}${input}`);
            }

            let gulpStream = gulp.src(inputs, {base: "."})
                .pipe(concat(`${paths.webroot}${bundle.outputFileName}`));

            if (bundle.minify && bundle.minify.enabled) {
                gulpStream = gulpStream
                    .pipe(cssmin());
            }

            return gulpStream.pipe(gulp.dest("."));
        };

        bundleCssTasksName[bundleConfigKey] = taskName;
    }

    aggregators[taskNames.bundleCss] = () => bundleCssTasksName.length > 0
        ? gulp.parallel(...bundleCssTasksName)
        : done => {
            done();
        };

    aggregators[taskNames.bundleAndMinifyCss] = () => gulp.series(
        gulp.parallel(
            taskNames.compileSass,
            taskNames.lintSass
        ),
        taskNames.bundleCss
    );

    aggregators[taskNames.bundleAndMinify] = () => gulp.parallel(
        taskNames.concatSource,
        taskNames.bundleAndMinifyJs,
        taskNames.bundleAndMinifyCss
    );

    tasks[taskNames.downloadProductionDeps] = () => {
        if (!fs.existsSync(paths.webroot)) {
            fs.mkdirSync(paths.webroot);
        }

        return gulp.src(["./package.json", "../yarn.lock"])
            .pipe(gulp.dest(paths.webroot))
            .pipe(yarn(
                {
                    production: true,
                    noProgress: true,
                    noBinLinks: true,
                    nonInteractive: true,
                    ignoreScripts: true, // is it good idea?
                }
            ));
    };

    tasks[taskNames.deleteProductionPackageJson] = () => del([
        `${paths.webroot}/package.json`,
        `${paths.webroot}/yarn.lock`,
        `${paths.webroot}/.yarnrc`,
    ]);

    aggregators[taskNames.downloadAllDeps] = () => gulp.series(
        taskNames.downloadProductionDeps,
        taskNames.deleteProductionPackageJson
    );

    tasks[taskNames.cleanJs] = () => del([paths.js]);

    tasks[taskNames.cleanCss] = () => del([paths.css]);

    tasks[taskNames.cleanDeps] = () => del([paths.runtimedeps]);

    tasks[taskNames.cleanFrontEndCache] = () => del([paths.obj + paths.sourceCache]);

    aggregators[taskNames.clean] = () => gulp.parallel(taskNames.cleanJs, taskNames.cleanCss, taskNames.cleanDeps, taskNames.cleanFrontEndCache);

    aggregators[taskNames.main] = () => gulp.series(
        taskNames.downloadAllDeps,
        taskNames.bundleAndMinify
    );

    if (
        fs.existsSync("../skip-gulp-run")
        && process.argv.indexOf("--force") < 0
        && process.argv.indexOf("--configuration=Release") < 0
    ) {
        aggregators["default"] = () => (done) => done();
    } else {
        aggregators["default"] = () => gulp.series(taskNames.main);
    }
};

exports.rewriteTask = (taskName, callback) => {
    tasks[taskName] = callback;
};

exports.rewriteAggregator = (taskName, callback) => {
    aggregators[taskName] = callback;
};

exports.registerTasks = gulp => {
    for (const task in tasks) {
        gulp.task(task, tasks[task]);
    }

    for (const aggregator in aggregators) {
        gulp.task(aggregator, aggregators[aggregator]());
    }
};
