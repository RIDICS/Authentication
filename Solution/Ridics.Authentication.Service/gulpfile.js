/// <binding Clean="clean"/>

const gulp = require("gulp");
const bundleConfig = require("./bundleconfig.json");

const baseGulpfile = require("../baseGulpfile");

baseGulpfile.initialize(gulp, {bundleConfig});
baseGulpfile.registerTasks(gulp);
