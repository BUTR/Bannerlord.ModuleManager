const path = require("path");

module.exports = {
    mode: "production",
    entry: "./blmodulemanager.js",
    output: {
        path: path.resolve(__dirname, "dist"),
        filename: "blmodulemanager.js",
        library: "blmodulemanager",
        libraryTarget: "umd",
        globalObject: "this"
    },
};