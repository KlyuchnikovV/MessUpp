
// Electron constant variables. //

const electron = require('electron');
const app = electron.app;
const BrowserWindow = electron.BrowserWindow;
var http = require('http');
var fs = require('fs');

var mainWindow = null;

app.on('window-all-closed',
    function()
    {
        if (process.platform != 'darwin')
            app.quit();
    }
);

app.on('ready',
    function()
    {
        mainWindow = new BrowserWindow({width: 1010, height: 700, resizable: true});
        mainWindow.loadURL('file://' + __dirname + '/app/login.html');
        //mainWindow.loadURL('file://' + __dirname + '/index.html');
        //mainWindow.loadURL('file://' + __dirname + '/main.html');
        mainWindow.on('closed',
            function()
            {
                mainWindow = null;
            }
		);
    }
);
