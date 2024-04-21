const path = require('path');

module.exports = {
    entry: './MyFreeFormForm/wwwroot/js/Statistics.js', // Adjust this path
    output: {
        filename: 'bundle.js',
        path: path.resolve('./MyFreeFormForm/wwwroot/js/', 'StatsDistro'),
    },
    devtool: 'eval-source-map'
};
