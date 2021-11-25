const path = require("path");

module.exports = {
    mode: "production",
    entry: "./src/index.js",
    devtool: "source-map",
    mode: "production",
    output: {
        filename: "index.js",
        path: path.resolve(__dirname, "dist"),
        library: "blmodulemanager",
        libraryTarget: "umd",
        umdNamedDefine: true,
        globalObject: "this",
        clean: true,
    },
    resolve: {
        extensions: ['', '.ts', '.tsx','.webpack.js', '.web.js', '.js']
    },
    module: {
        rules: [
          {
            test: /\.js$/,
            use: ["source-map-loader"],
            exclude: /node_modules/,
            enforce: "pre"
          }
        ]
    }
};